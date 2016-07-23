using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AutoTiling {

	public class MeshData {

		private List<Vector3> vertices = new List<Vector3>();
		private List<Vector3> normals = new List<Vector3>();
		private List<int>[] triangles = new List<int>[1];
		private List<Vector2> uv = new List<Vector2>();
		private List<Vector4> tangents = new List<Vector4>();
		
		public List<Vector3> Vertices {
			
			get {
				return vertices;
			}
			
		}
		
		public List<Vector3> Normals {
			
			get {
				return normals;
			}
			
		}
		
		public List<int>[] Triangles {
			
			get {
				return triangles;
			}
			
		}

		public List<Vector2> UV {
			
			get {
				return uv;
			}
			
		}

		public List<Vector4> Tangents {

			get {
				return tangents;
			}

		}

		public int subMeshCount {
			get {
				return triangles.Length;
			}

			set {
				List<int>[] newTriangleList = new List<int>[value];
				for (int i = 0; i < newTriangleList.Length; i++) {
					if (i < triangles.Length) {
						newTriangleList[i] = triangles[i];
					}
					else {
						newTriangleList[i] = new List<int>();
					}
				}
				triangles = newTriangleList;
//				Debug.Log ("Setting SubMeshCount to " + triangles.Length);
			}
		}
		
		public MeshData() { }
		
		public void AddQuadTriangles() {
			
			if (triangles == null || triangles.Length < 1) {
				triangles = new List<int>[1];
			}
			if (triangles[0] == null) {
				triangles[0] = new List<int>();
			}
			if (triangles == null) {
				Debug.LogError("triangles were not set!");
				return;
			}
			if (triangles[0] == null) {
				Debug.LogError("triangles[0] was not set!");
				return;
			}
			if (vertices == null) {
				Debug.LogError("Vertices were not set!");
				return;
			}
			triangles[0].Add(vertices.Count - 4);
			triangles[0].Add(vertices.Count - 3);
			triangles[0].Add(vertices.Count - 2);
			
			triangles[0].Add(vertices.Count - 4);
			triangles[0].Add(vertices.Count - 2);
			triangles[0].Add(vertices.Count - 1);

		}
		
		public void AddTriangle(int tri) {

			if (triangles == null || triangles.Length < 1) {
				triangles = new List<int>[1];
				triangles[0] = new List<int>();
			}
//			Debug.Log (GetType() + ".AddTriangle: Adding Triangle " + tri + " without specific material index.");
			triangles[0].Add(tri);

		}

		public void AddTriangle(int tri, int materialIndex) {

			if (materialIndex >= triangles.Length) {
				if (materialIndex > 0) {
					Debug.LogError(GetType() + ".AddTriangle: the material index is too high, set subMeshCount first.");
					return;
				}
				else {
					AddTriangle(tri);
					return;
				}
			}
			if (triangles[materialIndex] == null) {
				triangles[materialIndex] = new List<int>();
			}
//			Debug.Log (GetType() + ".AddTriangle: Adding Triangle " + tri + " to material: " + materialIndex);
			triangles[materialIndex].Add(tri);
			
		}
		
		public void SetTriangles(Mesh mesh) {

			this.triangles = new List<int>[mesh.subMeshCount];
			if (mesh == null) {
				this.triangles[0] = new List<int> ();
				return;
			}

			for (int i = 0; i < mesh.subMeshCount; i++) {
				this.triangles[i] = new List<int>(mesh.GetTriangles(i));
			}

		}

		public void SetTriangles(int[] newTriangles) {

			if (newTriangles == null) {
				this.triangles[0] = new List<int> ();
				return;
			}
			this.triangles[0] = new List<int> (newTriangles);

		}
		
		public void AddVertex(Vector3 vertex) {
			
			vertices.Add(vertex);

		}

		public void SetVertices(Vector3[] newVertices) {

			if (newVertices == null) {
				vertices = new List<Vector3> ();
				return;
			}
			vertices = new List<Vector3> (newVertices);

		}

		public void AddNormal(Vector3 normal) {

			normals.Add (normal);

		}

		public void SetNormals(Vector3[] newNormals) {

			if (newNormals == null) {
				normals = new List<Vector3>();
				return;
			}
			normals = new List<Vector3> (newNormals);

		}

		public void AddTangent(Vector4 tangent) {

			tangents.Add (tangent);

		}

		public void SetTangents(Vector4[] newTangents) {

			if (newTangents == null) {
				tangents = new List<Vector4>();
				return;
			}
			tangents = new List<Vector4> (newTangents);

		}
		
		public void AddUVCoordinates(Vector2[] uvCoordinates) {
			
			uv.AddRange (uvCoordinates);
			
		}
	
		public void AddUVCoordinate(Vector2 uvCoordinate) {

//			Debug.Log ("Adding UV Coordinate: " + uvCoordinate);
			uv.Add (uvCoordinate);
			
		}
	
	}

}
