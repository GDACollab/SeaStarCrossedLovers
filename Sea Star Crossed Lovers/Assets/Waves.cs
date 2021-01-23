using UnityEngine;
using System.Collections;

/// <summary>
/// Uses the particle system. If the collision detection remains unwritten, you can just use this object's ParticleSystem component and get collisions from that.
/// </summary>
public class Waves : MonoBehaviour {

	// Modified from https://rafalwilinski.medium.com/tutorial-particle-sea-in-unity3d-70ff1350fa9e

	public ParticleSystem particles;
	private ParticleSystem.Particle[] particlesArray;

	public Gradient colorGradient;

	public int seaResolution = 80;
	public float spacing = 0.5f;
	public float noiseScale = 0.1f;
	public float heightScale = 1f;

	private float perlinNoiseAnimX = 0.01f;

	void Start() {
		particlesArray = new ParticleSystem.Particle[seaResolution];
		particles = GetComponent<ParticleSystem>();
		var main = particles.main;
		main.maxParticles = seaResolution;
		particles.Emit(seaResolution);
		particles.GetParticles(particlesArray);
	}

	/// <summary>
	/// How much to influence other particles by
	/// </summary>
	float waveInfluence;
	/// <summary>
	/// How tall the wave is.
	/// </summary>
	float waveHeight;
	Vector3 wavePos;
	/// <summary>
	/// -1 for left, 1 for right.
	/// </summary>
	float waveDirection;
	float waveSpeed = 0.05f;
	bool isWaving = false;

	/// <summary>
	/// Create a wave
	/// </summary>
	/// <param name="direction">-1 for left, 1 for right.</param>
	/// <param name="width">How wide the wave is.</param>
	/// <param name="height">How tall the wave is</param>
	public void GenerateWave(float direction = 1.0f, float width = 10.0f, float height = 0.5f) {
		waveDirection = direction;
		waveInfluence = width;
		waveHeight = height;
		if (direction == 1)
		{
			wavePos = particlesArray[0].position;
		}
		else {
			wavePos = particlesArray[particlesArray.Length - 1].position;
		}
		isWaving = true;
	}

	void Update() {
		for(int i = 0; i < seaResolution; i++) {
			float zPos = Mathf.PerlinNoise(i * noiseScale + perlinNoiseAnimX, noiseScale);
			particlesArray[i].startColor = colorGradient.Evaluate(zPos);
			particlesArray[i].position = new Vector3(i * spacing, zPos  * heightScale, spacing);
			if (isWaving) {
			}
		}

		perlinNoiseAnimX += 0.01f;

		particles.SetParticles(particlesArray, particlesArray.Length);
		if (isWaving) {
			wavePos += new Vector3(waveSpeed * waveDirection, 0, 0);
		}
	}

}