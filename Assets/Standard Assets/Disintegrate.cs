using UnityEngine;
using System.Collections;

public class Disintegrate : MonoBehaviour {
	
	public float duration = 0.5f;
	public float deathDelay = 1;
	
	int triangleCount = 0;
	
	public void DesintegrateFX () {
		triangleCount = 0;
		MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter meshFilter in meshFilters) {
			StartCoroutine(Desintegrating(meshFilter));
		}
	}
	
	IEnumerator Desintegrating (MeshFilter meshFilter) {

		Mesh mesh = meshFilter.mesh;
		
		Vector3[] _vertices = mesh.vertices;
		Vector3[] _normals = mesh.normals;
		Vector2[] _uvs = mesh.uv;
		int[] _triangles = mesh.triangles;
		
		triangleCount += _triangles.Length;
		
		Vector3[] vertices = new Vector3[_triangles.Length];
		Vector3[] normals = new Vector3[_triangles.Length];
		Vector2[] uvs = new Vector2[_triangles.Length];
		int[] triangles = new int[_triangles.Length];
		for (int i=0; i<_triangles.Length; i+=3) {
			int t0 = _triangles[i+0];
			int t1 = _triangles[i+1];
			int t2 = _triangles[i+2];
			vertices[i+0] = _vertices[t0];
			vertices[i+1] = _vertices[t1];
			vertices[i+2] = _vertices[t2];
			normals[i+0] = _normals[t0];
			normals[i+1] = _normals[t1];
			normals[i+2] = _normals[t2];
			uvs[i+0] = _uvs[t0];
			uvs[i+1] = _uvs[t1];
			uvs[i+2] = _uvs[t2];
			triangles[i+0] = i+0;
			triangles[i+1] = i+1;
			triangles[i+2] = i+2;
		}
		
		mesh.Clear();
		mesh.name = "Desintegration";
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		meshFilter.mesh = mesh;
		
		float speed = 5;
		float time = duration;
		while (time > 0) {
			for (int i=0; i<triangles.Length; i+=3) {
				Vector3 normal = Vector3.Cross(vertices[i+1]-vertices[i+0], vertices[i+2]-vertices[i+0]);
				normal.Normalize();
				vertices[i+0] += normal * Mathfx.Berp(speed, 0, 1-time/duration) * Time.deltaTime;
				vertices[i+1] += normal * Mathfx.Berp(speed, 0, 1-time/duration) * Time.deltaTime;
				vertices[i+2] += normal * Mathfx.Berp(speed, 0, 1-time/duration) * Time.deltaTime;
			}
			time -= Time.deltaTime;
			mesh.vertices = vertices;
			yield return 0;
		}
		
		for (int i=0; i<triangles.Length; i+=3) {
			vertices[i+1] = vertices[i+0];
			vertices[i+2] = vertices[i+0];
			mesh.vertices = vertices;
			yield return 0;
		}
	}
	
}
