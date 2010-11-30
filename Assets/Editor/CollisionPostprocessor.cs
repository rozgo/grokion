using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

class CollisionPostprocessor : AssetPostprocessor {
	
	void OnPostprocessModel (GameObject asset) {
		Apply(asset);
	}
	
	void Apply (GameObject asset) {
		
		Component[] children = asset.transform.GetComponentsInChildren(typeof(Transform));
		
		foreach (Transform child in children) {
		
			if (child.name.IndexOf("LevelGeometry") >= 0) {
				
				GameObject collisionObject = new GameObject("Collision");
				collisionObject.transform.parent = child.parent;
				collisionObject.transform.position = child.position;
				collisionObject.transform.rotation = child.rotation;
				collisionObject.transform.localScale = child.localScale;
				
				MeshFilter assetMeshFilter = (MeshFilter)child.GetComponent(typeof(MeshFilter));
				Vector3[] vertices = assetMeshFilter.sharedMesh.vertices;
				int[] triangles = assetMeshFilter.sharedMesh.triangles;
				ArrayList collisionTriangles = new ArrayList();
				for (int t=0; t<triangles.Length; t+=3) {
					Plane plane = new Plane();
					plane.Set3Points(vertices[triangles[t+0]], vertices[triangles[t+1]], vertices[triangles[t+2]]);
					if (Mathfx.Approx(plane.normal.y, 0.0f, 0.01f)) {
						collisionTriangles.Add(triangles[t+0]);
						collisionTriangles.Add(triangles[t+1]);
						collisionTriangles.Add(triangles[t+2]);
					}
				}
				
				Mesh collisionMesh = new Mesh();
				collisionMesh.name = "CollisionMesh";
				collisionMesh.vertices = assetMeshFilter.sharedMesh.vertices;
				collisionMesh.triangles = (int[])collisionTriangles.ToArray(typeof(int));
				collisionMesh.RecalculateNormals();
				collisionMesh.RecalculateBounds();
				collisionMesh.Optimize();
				
				string meshAssetPath = "Assets/Collisions/";
				string meshAssetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
				meshAssetName += child.name;
				meshAssetName += collisionMesh.name;
				meshAssetPath += meshAssetName.GetHashCode().ToString("X");
				AssetDatabase.CreateAsset(collisionMesh, meshAssetPath);
	
				MeshFilter collisionMeshFilter = (MeshFilter)collisionObject.AddComponent(typeof(MeshFilter));
				collisionMeshFilter.sharedMesh = collisionMesh;
				
				MeshRenderer meshRenderer = (MeshRenderer)collisionObject.AddComponent(typeof(MeshRenderer));
				meshRenderer.enabled = false;
				collisionObject.AddComponent(typeof(MeshCollider));
			}
		}
	}
}
