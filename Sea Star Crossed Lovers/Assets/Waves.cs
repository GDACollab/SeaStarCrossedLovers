using UnityEngine;
using System.Collections;

public class Waves : MonoBehaviour {

	// Modified from https://rafalwilinski.medium.com/tutorial-particle-sea-in-unity3d-70ff1350fa9e

	public ParticleSystem particles;
	private ParticleSystem.Particle[] particlesArray;

	public Gradient colorGradient;

	public int seaResolution = 50;
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