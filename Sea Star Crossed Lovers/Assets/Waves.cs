using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisruptiveWave : Object {
	public float direction;
	public float width;
	public float height;
	public float speed;
	public Vector3 pos;
	// Use custom settings
	public DisruptiveWave(float waveDirection = 1, float waveWidth = 5.0f, float waveHeight = 2.5f, float waveSpeed = 0.01f) {
		this.direction = waveDirection;
		this.width = waveWidth;
		this.height = waveHeight;
		this.speed = waveSpeed;
	}
}

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

	List<DisruptiveWave> activeWaveList = new List<DisruptiveWave>();

	void Start() {
		particlesArray = new ParticleSystem.Particle[seaResolution];
		particles = GetComponent<ParticleSystem>();
		var main = particles.main;
		main.maxParticles = seaResolution;
		particles.Emit(seaResolution);
		particles.GetParticles(particlesArray);
	}

	public void GenerateWave(DisruptiveWave wave) {
		if (wave.direction == 1)
		{
			wave.pos = particlesArray[0].position - new Vector3(wave.width, 0, 0);
		}
		else {
			wave.pos = particlesArray[particlesArray.Length - 1].position + new Vector3(wave.width, 0, 0);
		}
		activeWaveList.Add(wave);
	}

	void Update() {
		for(int i = 0; i < seaResolution; i++) {
			float zPos = Mathf.PerlinNoise(i * noiseScale + perlinNoiseAnimX, noiseScale);
			particlesArray[i].startColor = colorGradient.Evaluate(zPos);
			particlesArray[i].position = new Vector3(i * spacing, zPos  * heightScale, spacing);
			foreach (DisruptiveWave wave in activeWaveList) {
				if (Mathf.Abs(particlesArray[i].position.x - wave.pos.x) <= wave.width) { //This equation comes from messing around with Desmos. If you have a better idea, I'm all for it.
					particlesArray[i].position += new Vector3(0, (-0.1f * Mathf.Pow((particlesArray[i].position.x - wave.pos.x), 2) + wave.height), 0);
				}
			}
		}

		perlinNoiseAnimX += 0.001f;

		particles.SetParticles(particlesArray, particlesArray.Length);
		foreach (DisruptiveWave wave in activeWaveList) {
			wave.pos += new Vector3(wave.speed * wave.direction, 0, 0);
			if (wave.pos.x > particlesArray[particlesArray.Length - 1].position.x + wave.width) { //Is this wave done? 
				activeWaveList.Remove(wave);
			}
		}
	}

}