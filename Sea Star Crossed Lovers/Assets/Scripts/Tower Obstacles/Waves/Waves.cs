using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A wave that is added to the current wave simulation, called in GenerateWave.
/// </summary>
public class DisruptiveWave : Object
{
	/// <summary>
	/// -1 for wave going left, 1 for wave going right.
	/// </summary>
	public float direction;
	/// <summary>
	/// The width of the wave.
	/// </summary>
	public float width;
	/// <summary>
	/// Height of the wave.
	/// </summary>
	public float height;
	/// <summary>
	/// Speed of the wave.
	/// </summary>
	public float speed;
	/// <summary>
	/// Wave's current position.
	/// </summary>
	public Vector3 pos;
	/// <summary>
	/// How big the wave gets over time.
	/// </summary>
	public float sizeOverTime;
	/// <summary>
	/// How much the wave should change over time (> 1, < 2 for growing, > 0, < 1 for shrinking)
	/// </summary>
	public float sizeModifier;
	/// <summary>
	/// Create a new DisruptiveWave (meant to be used in GenerateWave()). Right now, larger waves look better, so try to make big waves to match the ocean.
	/// </summary>
	/// <param name="waveDirection">-1 for left, 1 for right.</param>
	/// <param name="waveWidth">How big the wave should be.</param>
	/// <param name="waveHeight">How high the wave should be.</param>
	/// <param name="waveSpeed">How fast the wave should be.</param>
	/// <param name="waveSizeOverTime">% of how much the wave should grow over time.</param>
	public DisruptiveWave(float waveDirection = 1, float waveWidth = 5f, float waveHeight = 1.1f, float waveSpeed = 0.01f, float waveSizeOverTime = 0.9996f)
	{
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
public class Waves : MonoBehaviour
{

	//Stuff for modifying the particle system.
	public ParticleSystem particles;
	private ParticleSystem.Particle[] particlesArray;

	/// <summary>
	/// How many particles?
	/// </summary>
	[Tooltip("However many particles to include in the simulation.")]
	public int seaResolution = 80;
	/// <summary>
	/// How far to space the particles apart.
	/// </summary>
	[Tooltip("How far to space the particles apart.")]
	public float spacing = 0.5f;
	/// <summary>
	/// Scale of the noise. Also acts as a kind of speed for the waves, so a lower noiseScale (above 0) will result in slower waves, and a higher noiseScale will result in faster waves.
	/// </summary>
	[Tooltip("Scale of the noise. Also acts as a kind of speed for the waves, so a lower noiseScale (above 0) will result in slower waves, and a higher noiseScale will result in faster waves.")]
	public float noiseScale = 0.1f;
	/// <summary>
	/// The actual speed of the waves. Lower is slower, higher is faster.
	/// </summary>
	[Tooltip("The actual speed of the waves. Lower is slower, higher is faster.")]
	public float waveSpeed = 0.001f;
	/// <summary>
	/// How tall to make the waves.
	/// </summary>
	[Tooltip("How tall to make the waves.")]
	public float heightScale = 1f;

	/// <summary>
	/// How far along the waves are. Increases by waveSpeed during Update.
	/// </summary>
	private float perlinNoiseAnimX = 0.01f;

	/// <summary>
	/// The list of all the DisruptiveWaves that are currently travelling along this here ocean.
	/// </summary>
	List<DisruptiveWave> activeWaveList = new List<DisruptiveWave>();

	// Stuff for the Ocean Mesh. You don't need to care about this.
	MeshFilter meshFilter; //Mesh filter renders the mesh we create for the ocean.
	Mesh oceanMesh; //The actual mesh we supply to the meshFilter.
	Vector3[] oceanMeshVertices;
	int[] oceanMeshTris;
	Vector3[] oceanMeshNormals;
	/// <summary>
	/// How deep the ocean is (how far down we render the triangles).
	/// </summary>
	[Tooltip("How deep or shallow the ocean is.")]
	public float oceanDepth = 5;

	void Start()
	{
		//Set up the ocean particles.
		particlesArray = new ParticleSystem.Particle[seaResolution];
		particles = GetComponent<ParticleSystem>();
		var main = particles.main;
		main.maxParticles = seaResolution;
		particles.Emit(seaResolution);
		particles.GetParticles(particlesArray);

		//Set up the mesh rendering.
		meshFilter = GetComponent<MeshFilter>();
		oceanMesh = new Mesh();
		oceanMeshVertices = new Vector3[3 * seaResolution];
		oceanMeshTris = new int[(seaResolution - 1) * 6];
		oceanMeshNormals = new Vector3[3 * seaResolution];
		//This tries to set the ocean normals to be facing in some kind of direction.
		for (var i = 0; i < 3 * seaResolution; i++)
		{ //Except nothing I do but scaling the Z by -1 flips the triangles. So we're scaling the Z by -1 in the Inspector. Sorry.
			oceanMeshNormals[i] = -Vector3.forward;
		}
		oceanMesh.vertices = oceanMeshVertices;
		oceanMesh.triangles = oceanMeshTris;
		oceanMesh.normals = oceanMeshNormals;
		meshFilter.mesh = oceanMesh; //The meshFilter renders our created mesh.
	}

	/// <summary>
	/// Create a wave across the current ocean.
	/// </summary>
	/// <param name="wave">Supply a Disruptive Wave. Just use new DisruptiveWave() if you're not sure what to put.</param>
	public void GenerateWave(DisruptiveWave wave)
	{
		if (wave.direction == 1) //Set the wave's position based on its direction.
		{
			wave.pos = particlesArray[0].position - new Vector3(wave.width, 0, 0);
		}
		else
		{
			wave.pos = particlesArray[particlesArray.Length - 1].position + new Vector3(wave.width, 0, 0);
		}
		activeWaveList.Add(wave);
	}

	void Update()
	{
		for (int i = 0; i < seaResolution; i++)
		{ //Go through all the particles.
			float zPos = Mathf.PerlinNoise(i * noiseScale + perlinNoiseAnimX, noiseScale); //This is what creates the bobbing.
			particlesArray[i].position = new Vector3(i * spacing, zPos * heightScale, spacing); //Now actually move the particles.
			foreach (DisruptiveWave wave in activeWaveList)
			{ //This is for adding waves on top of the current ocean. Use GenerateWave() to add DisruptiveWaves to the activeWaveList.
				if (Mathf.Abs(particlesArray[i].position.x - wave.pos.x) <= wave.width) //Is this point within the wave's influence?
				{ //This equation on Desmos: https://www.desmos.com/calculator/jgudrypofv. Remove * zPos for removing wave randomness.
					particlesArray[i].position += new Vector3(0, Mathf.Cos((particlesArray[i].position.x - wave.pos.x) * Mathf.PI / 2 * 1 / wave.width) * wave.height * wave.sizeOverTime, 0);
				}
			}
			if (i + 1 < seaResolution) //Touch this if you dare.
			{ //Define points for our triangles to use.
				oceanMeshVertices[(i * 3)] = particlesArray[i].position; //The position of our current particle
				oceanMeshVertices[(i * 3) + 1] = new Vector3(particlesArray[i].position.x, -oceanDepth, particlesArray[i].position.z); //The position below this particle.
				oceanMeshVertices[(i * 3) + 2] = new Vector3(particlesArray[i].position.x + spacing, -oceanDepth, particlesArray[i].position.z); //The position below the next particle.
				oceanMeshVertices[(i * 3) + 3] = particlesArray[i + 1].position; //The position of the next particle.
																				 //First triangle:
				oceanMeshTris[(i * 6)] = i * 3; //First point is this particle.
				oceanMeshTris[(i * 6) + 1] = (i * 3) + 1; //Then below this particle.
				oceanMeshTris[(i * 6) + 2] = (i * 3) + 3; //Then the next particle.
														  //Second Triangle.
				oceanMeshTris[(i * 6) + 5] = (i * 3) + 1; //Below this particle.
				oceanMeshTris[(i * 6) + 3] = (i * 3) + 2; //Below the next particle.
				oceanMeshTris[(i * 6) + 4] = (i * 3) + 3; //The next particle.
			}
		}

		oceanMesh.vertices = oceanMeshVertices;
		oceanMesh.triangles = oceanMeshTris;

		perlinNoiseAnimX += waveSpeed;

		particles.SetParticles(particlesArray, particlesArray.Length);
		List<DisruptiveWave> toRemoveWave = new List<DisruptiveWave>();
		foreach (DisruptiveWave wave in activeWaveList)
		{ //Update the DisruptiveWaves by moving them around and removing them if need be.
			wave.pos += new Vector3(wave.speed * wave.direction, 0, 0);
			wave.sizeOverTime *= wave.sizeModifier;
			if (wave.pos.x > particlesArray[particlesArray.Length - 1].position.x + wave.width)
			{ //Is this wave done? 
				toRemoveWave.Add(wave);
			}
		}
		foreach (DisruptiveWave toRemove in toRemoveWave)
		{
			activeWaveList.Remove(toRemove);
		}

		oceanMesh.RecalculateBounds();
	}

}