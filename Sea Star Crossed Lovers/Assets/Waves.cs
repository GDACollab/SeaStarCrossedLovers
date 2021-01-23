using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisruptiveWave : Object {
	public float direction;
	public float width;
	public float height;
	public float speed;
	public Vector3 pos;
	public float sizeOverTime;
	public float sizeModifier;
	/// <summary>
	/// Create a new DisruptiveWave (meant to be used in GenerateWave()). Right now, larger waves look better, so try to make big waves to match the ocean.
	/// </summary>
	/// <param name="waveDirection">-1 for left, 1 for right.</param>
	/// <param name="waveWidth">How big the wave should be.</param>
	/// <param name="waveHeight">How high the wave should be.</param>
	/// <param name="waveSpeed">How fast the wave should be.</param>
	/// <param name="waveSizeOverTime">% of how much the wave should grow over time.</param>
	public DisruptiveWave(float waveDirection = 1, float waveWidth = 5f, float waveHeight = 1.1f, float waveSpeed = 0.01f, float waveSizeOverTime = 0.9996f) {
		this.direction = waveDirection;
		this.width = waveWidth;
		this.height = waveHeight;
		this.speed = waveSpeed;
		this.sizeOverTime = 1;
		this.sizeModifier = waveSizeOverTime;
	}
}

/// <summary>
/// Uses the particle system. If the collision detection remains unwritten, you can just use this object's ParticleSystem component and get collisions from that.
/// Modified from https://rafalwilinski.medium.com/tutorial-particle-sea-in-unity3d-70ff1350fa9e
/// </summary>
public class Waves : MonoBehaviour {


	public ParticleSystem particles;
	private ParticleSystem.Particle[] particlesArray;

	public int seaResolution = 80;
	public float spacing = 0.5f;
	public float noiseScale = 0.1f;
	public float heightScale = 1f;

	private float perlinNoiseAnimX = 0.01f;

	List<DisruptiveWave> activeWaveList = new List<DisruptiveWave>();

	LineRenderer oceanLine;

	// Stuff for Ocean Texture.
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	Mesh oceanMesh;
	Vector3[] oceanMeshVertices;
	int[] oceanMeshTris;
	Vector3[] oceanMeshNormals;

	void Start() {
		particlesArray = new ParticleSystem.Particle[seaResolution];
		particles = GetComponent<ParticleSystem>();
		var main = particles.main;
		main.maxParticles = seaResolution;
		particles.Emit(seaResolution);
		particles.GetParticles(particlesArray);
		oceanLine = GetComponent<LineRenderer>();
		oceanLine.positionCount = seaResolution;
		oceanLine.useWorldSpace = true;

		meshFilter = GetComponent<MeshFilter>();
		oceanMesh = new Mesh();
		oceanMeshVertices = new Vector3[2 * seaResolution];
		oceanMeshTris = new int[(seaResolution - 1) * 3];
		oceanMeshNormals = new Vector3[2 * seaResolution];
		for (var i = 0; i < 2 * seaResolution; i++) { //Somewhat buggy? Need to fix the way the mesh displays with some textures
			oceanMeshNormals[i] = -Vector3.forward;
		}
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
			particlesArray[i].position = new Vector3(i * spacing, zPos  * heightScale, spacing);
			foreach (DisruptiveWave wave in activeWaveList) {
				if (Mathf.Abs(particlesArray[i].position.x - wave.pos.x) <= wave.width)
				{ //This equation on Desmos: https://www.desmos.com/calculator/jgudrypofv. Remove * zPos for removing wave randomness.
					particlesArray[i].position += new Vector3(0, Mathf.Cos((particlesArray[i].position.x - wave.pos.x) * Mathf.PI/2 * 1/wave.width) * wave.height * wave.sizeOverTime, 0);
				}
			}
			oceanLine.SetPosition(i, particlesArray[i].position + transform.position);
			if (i + 1 != seaResolution)
			{ //Draw Trapezoids.
				oceanMeshVertices[(i * 2)] = particlesArray[i].position;
				oceanMeshTris[(i * 3)] = i * 2;
				oceanMeshVertices[(i * 2) + 1] = new Vector3(particlesArray[i].position.x + spacing, -10, particlesArray[i].position.z);
				oceanMeshTris[(i * 3) + 1] = (i * 2) + 1;
				oceanMeshVertices[(i * 2) + 2] = particlesArray[i + 1].position;
				oceanMeshTris[(i * 3) + 2] = (i * 2) + 2;
			}
		}

		oceanMesh.vertices = oceanMeshVertices;
		oceanMesh.triangles = oceanMeshTris;
		oceanMesh.normals = oceanMeshNormals;
		meshFilter.mesh = oceanMesh;

		perlinNoiseAnimX += 0.001f;

		particles.SetParticles(particlesArray, particlesArray.Length);
		foreach (DisruptiveWave wave in activeWaveList) {
			wave.pos += new Vector3(wave.speed * wave.direction, 0, 0);
			wave.sizeOverTime *= wave.sizeModifier;
			if (wave.pos.x > particlesArray[particlesArray.Length - 1].position.x + wave.width) { //Is this wave done? 
				activeWaveList.Remove(wave);
			}
		}
	}

}