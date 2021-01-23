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
	float waveInfluence = 10f;
	Vector3 wavePos = Vector3.zero;
	/// <summary>
	/// False for going to the left, True for going to the right.
	/// </summary>
	bool waveDirection = false;


	/// <summary>
	/// Create a wave
	/// </summary>
	/// <param name="isLeft">Do we start on the left? If we start on the left, wave goes to the right. If we start on the right, wave goes to the left.</param>
	public void GenerateWave(bool isLeft) {
		waveDirection = isLeft;
		if (isLeft) {
			wavePos;
		}
	}

	void Update() {
		for(int i = 0; i < seaResolution; i++) {
				float zPos = Mathf.PerlinNoise(i * noiseScale + perlinNoiseAnimX, noiseScale);
				particlesArray[i].startColor = colorGradient.Evaluate(zPos);
				particlesArray[i].position = new Vector3(i * spacing, zPos  * heightScale, spacing);
		}

		perlinNoiseAnimX += 0.01f;

		particles.SetParticles(particlesArray, particlesArray.Length);
	}

}