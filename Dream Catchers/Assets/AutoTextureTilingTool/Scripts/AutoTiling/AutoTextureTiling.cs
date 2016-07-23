using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
//using System.IO;
using System.Linq;
#endif

namespace AutoTiling {

	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	/// <summary>
	/// Auto texture tiling.
	/// --------------------
	/// The base class of the Auto Texture Tiling Tool.
	/// Just add this component to any GameObject with a Mesh (no Skinned Meshes yet, sorry), and it will keep the UV scaling relative to the object scaling.
	/// 
	/// This will NOT update the UV scaling in a build! Use DynamicTextureTiling instead.
	/// </summary>
	public class AutoTextureTiling : MonoBehaviour {

		public bool useUnifiedScaling = false;
//		[System.Obsolete("No longer used. Use the new dynamic texturing instead.")]
		[SerializeField]
		private Vector2 _topScale = Vector2.one;
		[SerializeField]
		private Vector2 _bottomScale = Vector2.one;
		[SerializeField]
		private Vector2 _leftScale = Vector2.one;
		[SerializeField]
		private Vector2 _rightScale = Vector2.one;
		[SerializeField]
		private Vector2 _frontScale = Vector2.one;
		[SerializeField]
		private Vector2 _backScale = Vector2.one;

		public bool useUnifiedOffset = false;
		[SerializeField]
		private Vector2 _topOffset = Vector2.zero;
		[SerializeField]
		private Vector2 _bottomOffset = Vector2.zero;
		[SerializeField]
		private Vector2 _leftOffset = Vector2.zero;
		[SerializeField]
		private Vector2 _rightOffset = Vector2.zero;
		[SerializeField]
		private Vector2 _frontOffset = Vector2.zero;
		[SerializeField]
		private Vector2 _backOffset = Vector2.zero;

		[SerializeField]
		private float _topRotation = 0f;
		[SerializeField]
		private float _bottomRotation = 0f;
		[SerializeField]
		private float _leftRotation = 0f;
		[SerializeField]
		private float _rightRotation = 0f;
		[SerializeField]
		private float _frontRotation = 0f;
		[SerializeField]
		private float _backRotation = 0f;

		[SerializeField]
		private int _topMaterialIndex = 0;
		[SerializeField]
		private int _bottomMaterialIndex = 0;
		[SerializeField]
		private int _leftMaterialIndex = 0;
		[SerializeField]
		private int _rightMaterialIndex = 0;
		[SerializeField]
		private int _frontMaterialIndex = 0;
		[SerializeField]
		private int _backMaterialIndex = 0;

		[SerializeField]
		private bool _topFlipX = false;
		[SerializeField]
		private bool _topFlipY = false;
		[SerializeField]
		private bool _bottomFlipX = false;
		[SerializeField]
		private bool _bottomFlipY = false;
		[SerializeField]
		private bool _leftFlipX = false;
		[SerializeField]
		private bool _leftFlipY = false;
		[SerializeField]
		private bool _rightFlipX = false;
		[SerializeField]
		private bool _rightFlipY = false;
		[SerializeField]
		private bool _frontFlipX = false;
		[SerializeField]
		private bool _frontFlipY = false;
		[SerializeField]
		private bool _backFlipX = false;
		[SerializeField]
		private bool _backFlipY = false;

		[SerializeField]
		private bool _useBakedMesh;

		protected float scaleX;
		protected float scaleY;
		protected float scaleZ;

		private MeshFilter _meshFilter;
		private MeshRenderer meshRenderer;

		private static string extensionString = ".asset";
		private static string meshAssetPathString = "Assets/AutoTextureTilingTool/Meshes/";

		public MeshRenderer Renderer {

			get {
				if (!meshRenderer) {
					meshRenderer = GetComponent<MeshRenderer>();
				}
				if (!meshRenderer) {
					Debug.LogError(name + ": " + GetType() + ".Renderer_get: there was no MeshRenderer component attached.");
				}
				return meshRenderer;
			}

		}

		public MeshFilter meshFilter {

			get {
				if (!_meshFilter) {
					_meshFilter = GetComponent<MeshFilter>();
				}
				if (!_meshFilter) {
					Debug.LogError(name + ": " + GetType() + ".meshFilter_get: there was no MeshFilter component attached.");
				}
				return _meshFilter;
			}

		}

		public Vector2 topScale {

			get {
				return _topScale;
			}

			set {
				_topScale = value;
				CreateMeshAndUVs ();
			}

		}
		public Vector2 bottomScale {
			
			get {
				return _bottomScale;
			}
			
			set {
				_bottomScale = value;
				if (useUnifiedScaling) {
					_topScale = value;
				}
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 leftScale {
			
			get {
				return _leftScale;
			}
			
			set {
				_leftScale = value;
				if (useUnifiedScaling) {
					_topScale = value;
				}
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 rightScale {
			
			get {
				return _rightScale;
			}
			
			set {
				_rightScale = value;
				if (useUnifiedScaling) {
					_topScale = value;
				}
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 frontScale {
			
			get {
				return _frontScale;
			}
			
			set {
				_frontScale = value;
				if (useUnifiedScaling) {
					_topScale = value;
				}
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 backScale {
			
			get {
				return _backScale;
			}
			
			set {
				_backScale = value;
				if (useUnifiedScaling) {
					_topScale = value;
				}
				CreateMeshAndUVs ();
			}
			
		}

		
		public Vector2 topOffset {
			
			get {
				return _topOffset;
			}
			
			set {
				_topOffset = value;
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 bottomOffset {
			
			get {
				return _bottomOffset;
			}
			
			set {
				_bottomOffset = value;
				if (useUnifiedOffset) {
					_topOffset = value;
				}
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 leftOffset {
			
			get {
				return _leftOffset;
			}
			
			set {
				_leftOffset = value;
				if (useUnifiedOffset) {
					_topOffset = value;
				}
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 rightOffset {
			
			get {
				return _rightOffset;
			}
			
			set {
				_rightOffset = value;
				if (useUnifiedOffset) {
					_topOffset = value;
				}
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 frontOffset {
			
			get {
				return _frontOffset;
			}
			
			set {
				_frontOffset = value;
				if (useUnifiedOffset) {
					_topOffset = value;
				}
				CreateMeshAndUVs ();
			}
			
		}
		public Vector2 backOffset {
			
			get {
				return _backOffset;
			}
			
			set {
				_backOffset = value;
				if (useUnifiedOffset) {
					_topOffset = value;
				}
				CreateMeshAndUVs ();
			}
			
		}

		public float topRotation {
			get {
				return _topRotation;
			}
			set {
				_topRotation = value;
				CreateMeshAndUVs();
			}
		}
		public float bottomRotation {
			get {
				return _bottomRotation;
			}
			set {
				_bottomRotation = value;
				CreateMeshAndUVs();
			}
		}
		public float leftRotation {
			get {
				return _leftRotation;
			}
			set {
				_leftRotation = value;
				CreateMeshAndUVs();
			}
		}
		public float rightRotation {
			get {
				return _rightRotation;
			}
			set {
				_rightRotation = value;
				CreateMeshAndUVs();
			}
		}
		public float frontRotation {
			get {
				return _frontRotation;
			}
			set {
				_frontRotation = value;
				CreateMeshAndUVs();
			}
		}
		public float backRotation {
			get {
				return _backRotation;
			}
			set {
				_backRotation = value;
				CreateMeshAndUVs();
			}
		}

		public int topMaterialIndex {
			get {
				return _topMaterialIndex;
			}
			set {
				_topMaterialIndex = value;
				CreateMeshAndUVs();
			}
		}

		public int bottomMaterialIndex {
			get {
				return _bottomMaterialIndex;
			}
			set {
				_bottomMaterialIndex = value;
				CreateMeshAndUVs();
			}
		}

		public int leftMaterialIndex {
			get {
				return _leftMaterialIndex;
			}
			set {
				_leftMaterialIndex = value;
				CreateMeshAndUVs();
			}
		}

		public int rightMaterialIndex {
			get {
				return _rightMaterialIndex;
			}
			set {
				_rightMaterialIndex = value;
				CreateMeshAndUVs();
			}
		}

		public int frontMaterialIndex {
			get {
				return _frontMaterialIndex;
			}
			set {
				_frontMaterialIndex = value;
				CreateMeshAndUVs();
			}
		}

		public int backMaterialIndex {
			get {
				return _backMaterialIndex;
			}
			set {
				_backMaterialIndex = value;
				CreateMeshAndUVs();
			}
		}

		public bool useBakedMesh {
			get {
				return _useBakedMesh;
			}
			set {
				_useBakedMesh = value;
			}
		}

		public bool topFlipX {
			get {
				return _topFlipX;
			}
			set {
				_topFlipX = value;
				CreateMeshAndUVs();
			}
		}

		public bool topFlipY {
			get {
				return _topFlipY;
			}
			set {
				_topFlipY = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool bottomFlipX {
			get {
				return _bottomFlipX;
			}
			set {
				_bottomFlipX = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool bottomFlipY {
			get {
				return _bottomFlipY;
			}
			set {
				_bottomFlipY = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool leftFlipX {
			get {
				return _leftFlipX;
			}
			set {
				_leftFlipX = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool leftFlipY {
			get {
				return _leftFlipY;
			}
			set {
				_leftFlipY = value;
				CreateMeshAndUVs();
			}
		}

		public bool rightFlipX {
			get {
				return _rightFlipX;
			}
			set {
				_rightFlipX = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool rightFlipY {
			get {
				return _rightFlipY;
			}
			set {
				_rightFlipY = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool frontFlipX {
			get {
				return _frontFlipX;
			}
			set {
				_frontFlipX = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool frontFlipY {
			get {
				return _frontFlipY;
			}
			set {
				_frontFlipY = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool backFlipX {
			get {
				return _backFlipX;
			}
			set {
				_backFlipX = value;
				CreateMeshAndUVs();
			}
		}
		
		public bool backFlipY {
			get {
				return _backFlipY;
			}
			set {
				_backFlipY = value;
				CreateMeshAndUVs();
			}
		}
		
		void Awake() {

			_meshFilter = GetComponent<MeshFilter> ();
			if (!_meshFilter) {
				Debug.LogError(name + ": " + GetType() + ".Awake: there was no MeshFilter component attached.");
			}
			meshRenderer = GetComponent<MeshRenderer> ();
			if (!meshRenderer) {
				Debug.LogError(name + ": " + GetType() + ".Awake: there was no MeshRenderer component attached.");
			}
			scaleX = transform.localScale.x;
			scaleY = transform.localScale.y;
			scaleZ = transform.localScale.z;
#if UNITY_EDITOR
			if (meshFilter.sharedMesh != null) {
				if (!_useBakedMesh || !MeshPrefabExists()) {
					Mesh meshCopy = Mesh.Instantiate(meshFilter.sharedMesh) as Mesh;
					meshFilter.sharedMesh = meshCopy;
				}
			}
			else {
				_useBakedMesh = false;
				Debug.Log(name + ": " + GetType() + ".CreateMeshAndUVs: there was no mesh, adding a default cube mesh.");
				meshFilter.sharedMesh = new Mesh();
				EditorUtility.SetDirty(this);
				if (!Application.isPlaying) { EditorApplication.MarkSceneDirty(); }
			}
#endif
			CreateMeshAndUVs ();

		}

		// Update is called once per frame
#if UNITY_EDITOR
		void Update () {
		
			if (!Application.isPlaying) {
				if (scaleX != transform.localScale.x || scaleY != transform.localScale.y || scaleZ != transform.localScale.z) {
					scaleX = transform.localScale.x;
					scaleY = transform.localScale.y;
					scaleZ = transform.localScale.z;
					CreateMeshAndUVs ();
				}
				if (meshFilter.sharedMesh == null) {
					_useBakedMesh = false;
					Debug.Log(name + ": " + GetType() + ".Update: there was no mesh, adding a default cube mesh.");
					meshFilter.sharedMesh = new Mesh();
					EditorUtility.SetDirty(this);
					EditorApplication.MarkSceneDirty();
					CreateMeshAndUVs ();
				}

			}

		}

#endif
		public void AlignOffsetCenter(Direction side) {

			switch (side) {
			case Direction.Back:
				backOffset = Vector2.zero;
				break;
			case Direction.Down:
				bottomOffset = Vector2.zero;
				break;
			case Direction.Forward:
				frontOffset = Vector2.zero;
				break;
			case Direction.Left:
				leftOffset = Vector2.zero;
				break;
			case Direction.Right:
				rightOffset = Vector2.zero;
				break;
			case Direction.Up:
				topOffset = Vector2.zero;
				break;
			}

		}

		public void AlignOffsetTop(Direction side) {
			switch (side) {
			case Direction.Back:
				backOffset = new Vector2(backOffset.x, 1f - (((transform.localScale.y - backScale.y) / backScale.y) * .5f));
				break;
			case Direction.Down:
				bottomOffset = new Vector2(bottomOffset.x, 1f - (((transform.localScale.x - bottomScale.y) / bottomScale.y) * .5f));
				break;
			case Direction.Forward:
				frontOffset = new Vector2(frontOffset.x, 1f - (((transform.localScale.y - frontScale.y) / frontScale.y) * .5f));
				break;
			case Direction.Left:
				leftOffset = new Vector2(leftOffset.x, 1f - (((transform.localScale.y - leftScale.y) / leftScale.y) * .5f));
				break;
			case Direction.Right:
				rightOffset = new Vector2(rightOffset.x, 1f - (((transform.localScale.y - rightScale.y) / rightScale.y) * .5f));
				break;
			case Direction.Up:
				topOffset = new Vector2(topOffset.x, 1f - (((transform.localScale.x - topScale.y) / topScale.y) * .5f));
				break;
			}
		}

		public void AlignOffsetBottom(Direction side) {
			switch (side) {
			case Direction.Back:
				backOffset = new Vector2(backOffset.x, ((transform.localScale.y - backScale.y) / backScale.y) * .5f);
				break;
			case Direction.Down:
				bottomOffset = new Vector2(bottomOffset.x, ((transform.localScale.x - bottomScale.y) / bottomScale.y) * .5f);
				break;
			case Direction.Forward:
				frontOffset = new Vector2(frontOffset.x, ((transform.localScale.y - frontScale.y) / frontScale.y) * .5f);
				break;
			case Direction.Left:
				leftOffset = new Vector2(leftOffset.x, ((transform.localScale.y - leftScale.y) / leftScale.y) * .5f);
				break;
			case Direction.Right:
				rightOffset = new Vector2(rightOffset.x, ((transform.localScale.y - rightScale.y) / rightScale.y) * .5f);
				break;
			case Direction.Up:
				topOffset = new Vector2(topOffset.x, ((transform.localScale.x - topScale.y) / topScale.y) * .5f);
				break;
			}
		}
		
		public void AlignOffsetLeft(Direction side) {
			switch (side) {
			case Direction.Back:
				backOffset = new Vector2(((transform.localScale.x - backScale.x) / backScale.x) * .5f, backOffset.y);
				break;
			case Direction.Down:
				bottomOffset = new Vector2(((1f - (transform.localScale.z - bottomScale.x) / bottomScale.x) * .5f), bottomOffset.y);
				break;
			case Direction.Forward:
				frontOffset = new Vector2(((transform.localScale.x - frontScale.x) / frontScale.x) * .5f, frontOffset.y);
				break;
			case Direction.Left:
				leftOffset = new Vector2(((transform.localScale.z - leftScale.x) / leftScale.x) * .5f, leftOffset.y);
				break;
			case Direction.Right:
				rightOffset = new Vector2(((transform.localScale.z - rightScale.x) / rightScale.x) * .5f, rightOffset.y);
				break;
			case Direction.Up:
				topOffset = new Vector2(((1f - (transform.localScale.z - topScale.x) / topScale.x) * .5f), topOffset.y);
				break;
			}
		}
		
		public void AlignOffsetRight(Direction side) {
			switch (side) {
			case Direction.Back:
				backOffset = new Vector2((1f- ((transform.localScale.x - backScale.x) / backScale.x) * .5f), backOffset.y);
				break;
			case Direction.Down:
				bottomOffset = new Vector2((((transform.localScale.z - bottomScale.x) / bottomScale.x) * .5f), bottomOffset.y);
				break;
			case Direction.Forward:
				frontOffset = new Vector2((1f- ((transform.localScale.x - frontScale.x) / frontScale.x) * .5f), frontOffset.y);
				break;
			case Direction.Left:
				leftOffset = new Vector2((1f- ((transform.localScale.z - leftScale.x) / leftScale.x) * .5f), leftOffset.y);
				break;
			case Direction.Right:
				rightOffset = new Vector2((1f- ((transform.localScale.z - rightScale.x) / rightScale.x) * .5f), rightOffset.y);
				break;
			case Direction.Up:
				topOffset = new Vector2((((transform.localScale.z - topScale.x) / topScale.x) * .5f), topOffset.y);
				break;
			}
		}
		
		public void SetTextureToFit(Direction side) {

			switch (side) {
			case Direction.Back:
				backOffset = Vector2.zero;
				backRotation = 0f;
				backScale = new Vector2(transform.localScale.x, transform.localScale.y);
				break;
			case Direction.Down:
				bottomOffset = Vector2.zero;
				bottomRotation = 0f;
				bottomScale = new Vector2(transform.localScale.z, transform.localScale.x);
				break;
			case Direction.Forward:
				frontOffset = Vector2.zero;
				frontRotation = 0f;
				frontScale = new Vector2(transform.localScale.x, transform.localScale.y);
				break;
			case Direction.Left:
				leftOffset = Vector2.zero;
				leftRotation = 0f;
				leftScale = new Vector2(transform.localScale.z, transform.localScale.y);
				break;
			case Direction.Right:
				rightOffset = Vector2.zero;
				rightRotation = 0f;
				rightScale = new Vector2(transform.localScale.z, transform.localScale.y);
				break;
			case Direction.Up:
				topOffset = Vector2.zero;
				topRotation = 0f;
				topScale = new Vector2(transform.localScale.z, transform.localScale.x);
				break;
			}

		}

		protected void CreateMeshAndUVs() {

			if (meshFilter == null) {
				Debug.LogError(GetType() + ".CreateMeshAndUVs: meshFilter was not set, there is no MeshFilter component attached.");
				return;
			}

			if (!meshFilter.sharedMesh.isReadable) {
				Debug.LogError(GetType() + ".CreateMeshAndUVs: could not edit mesh. Please make sure that 'Read/Write Enabled' is checked in the import settings.");
				return;
			}

			MeshData meshData = new MeshData ();

#if UNITY_EDITOR
			if (meshFilter.sharedMesh.vertices.Length < 1) {
				meshData = CreateStandardCubeMesh();
				meshFilter.sharedMesh.subMeshCount = meshData.subMeshCount;
				meshFilter.sharedMesh.vertices = meshData.Vertices.ToArray();
				for (int i = 0; i < meshData.subMeshCount; i++) {
					meshFilter.sharedMesh.SetTriangles(meshData.Triangles[i].ToArray(), i);
				}
				meshFilter.sharedMesh.uv = meshData.UV.ToArray();
				meshFilter.sharedMesh.RecalculateBounds ();
				meshFilter.sharedMesh.RecalculateNormals ();
				EditorUtility.SetDirty(meshFilter);
				EditorUtility.SetDirty(this);
			}
			else {
				Vector3[] oldVertices = meshFilter.sharedMesh.vertices;
				Vector3[] oldNormals = meshFilter.sharedMesh.normals;
				if (oldVertices.Length < 3) {
					Debug.LogError(name + ": " + GetType() + ".CreateMeshAndUVs: there was something wrong with the mesh, not enough vertices: " + oldVertices.Length + ".");
					return;
				}
				meshData.SetVertices(oldVertices);
				if (oldNormals.Length != oldVertices.Length) {
					Debug.LogError(name + ": " + GetType() + ".CreateMeshAndUVs: there was something wrong with the mesh, there were " + oldNormals.Length + " normals, but " + oldVertices.Length + " vertices. They need to have the same count.");
					return;
				}
				meshData.SetNormals(oldNormals);
				meshData.SetTriangles(meshFilter.sharedMesh);
				meshData.SetTangents(meshFilter.sharedMesh.tangents);
				meshData = SplitMeshForCubeProjection(meshData);
//				Debug.Log ("Current subMeshCount: " + meshData.subMeshCount);
				meshFilter.sharedMesh.subMeshCount = meshData.subMeshCount;
				meshFilter.sharedMesh.vertices = meshData.Vertices.ToArray();
				for (int i = 0; i < meshData.subMeshCount; i++) {
					if (meshData.Triangles[i].Count > 0 && meshData.Triangles[i].Count % 3 != 0) {
						Debug.LogError(name + ": " + GetType() + ".CreateMeshAndUVs: there was something wrong with the mesh, triangles not divisible by 3. Triangles Count for material index " + i + ": " + meshData.Triangles[i].Count);
						return;
					}
//					foreach (int triangleIndex in meshData.Triangles[i]) {
//						Debug.Log ("Setting Triangle Index " + triangleIndex + " to material index: " + i);
//					}
					meshFilter.sharedMesh.SetTriangles(meshData.Triangles[i].ToArray(), i);
				}
				meshFilter.sharedMesh.normals = meshData.Normals.ToArray();
				meshFilter.sharedMesh.tangents = meshData.Tangents.ToArray();
				meshFilter.sharedMesh.uv = meshData.UV.ToArray();
			}
			if (!_useBakedMesh) {
				meshFilter.sharedMesh.name = "Mesh " + name;
			}
#else
			if (meshFilter.mesh.vertices.Length < 1) {
				meshData = CreateStandardCubeMesh();
				meshFilter.mesh.subMeshCount = meshData.subMeshCount;
				meshFilter.mesh.vertices = meshData.Vertices.ToArray();
				for (int i = 0; i < meshData.subMeshCount; i++) {
					meshFilter.mesh.SetTriangles(meshData.Triangles[i].ToArray(), i);
				}
				meshFilter.mesh.uv = meshData.UV.ToArray();
				meshFilter.mesh.RecalculateBounds ();
				meshFilter.mesh.RecalculateNormals ();
			}
			else {
				Vector3[] oldVertices = meshFilter.mesh.vertices;
				Vector3[] oldNormals = meshFilter.mesh.normals;
				if (oldVertices.Length < 3) {
					Debug.LogError(name + ": " + GetType() + ".CreateMeshAndUVs: there was something wrong with the mesh, not enough vertices: " + oldVertices.Length + ".");
					return;
				}
				meshData.SetVertices(oldVertices);
				if (oldNormals.Length != oldVertices.Length) {
					Debug.LogError(name + ": " + GetType() + ".CreateMeshAndUVs: there was something wrong with the mesh, there were " + oldNormals.Length + " normals, but " + oldVertices.Length + " vertices. They need to have the same count.");
					return;
				}
				meshData.SetNormals(oldNormals);
				meshData.SetTriangles(meshFilter.mesh);
				meshData.SetTangents(meshFilter.mesh.tangents);
				meshData = SplitMeshForCubeProjection(meshData);
				//				Debug.Log ("Current subMeshCount: " + meshData.subMeshCount);
				meshFilter.mesh.subMeshCount = meshData.subMeshCount;
				meshFilter.mesh.vertices = meshData.Vertices.ToArray();
				for (int i = 0; i < meshData.subMeshCount; i++) {
					if (meshData.Triangles[i].Count > 0 && meshData.Triangles[i].Count % 3 != 0) {
						Debug.LogError(name + ": " + GetType() + ".CreateMeshAndUVs: there was something wrong with the mesh, triangles not divisible by 3. Triangles Count for material index " + i + ": " + meshData.Triangles[i].Count);
						return;
					}
					//					foreach (int triangleIndex in meshData.Triangles[i]) {
					//						Debug.Log ("Setting Triangle Index " + triangleIndex + " to material index: " + i);
					//					}
					meshFilter.mesh.SetTriangles(meshData.Triangles[i].ToArray(), i);
				}
				meshFilter.mesh.normals = meshData.Normals.ToArray();
				meshFilter.mesh.tangents = meshData.Tangents.ToArray();
				meshFilter.mesh.uv = meshData.UV.ToArray();
			}
			meshFilter.mesh.name = "Mesh " + name;
#endif

		}

		private MeshData CreateStandardCubeMesh() {

			MeshData meshData = new MeshData ();

			meshData.AddVertex(new Vector3(- 0.5f, 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(0.5f, 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(0.5f, 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(- 0.5f, 0.5f, - 0.5f));
			
			meshData.AddQuadTriangles();
			meshData.AddUVCoordinates(QuadFaceUVs(Direction.Up));
			
			meshData.AddVertex(new Vector3(- 0.5f, - 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(0.5f, - 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(0.5f, - 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(- 0.5f, - 0.5f, 0.5f));
			
			meshData.AddQuadTriangles();
			meshData.AddUVCoordinates(QuadFaceUVs(Direction.Down));
			
			meshData.AddVertex(new Vector3(0.5f, - 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(0.5f, 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(- 0.5f, 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(- 0.5f, - 0.5f, 0.5f));
			
			meshData.AddQuadTriangles();
			meshData.AddUVCoordinates(QuadFaceUVs(Direction.Forward));
			
			meshData.AddVertex(new Vector3(- 0.5f, - 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(- 0.5f, 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(0.5f, 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(0.5f, - 0.5f, - 0.5f));
			
			meshData.AddQuadTriangles();
			meshData.AddUVCoordinates(QuadFaceUVs(Direction.Back));
			
			meshData.AddVertex(new Vector3(- 0.5f, - 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(- 0.5f, 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(- 0.5f, 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(- 0.5f, - 0.5f, - 0.5f));
			
			meshData.AddQuadTriangles();
			meshData.AddUVCoordinates(QuadFaceUVs(Direction.Left));
			
			meshData.AddVertex(new Vector3(0.5f, - 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(0.5f, 0.5f, - 0.5f));
			meshData.AddVertex(new Vector3(0.5f, 0.5f, 0.5f));
			meshData.AddVertex(new Vector3(0.5f, - 0.5f, 0.5f));
			
			meshData.AddQuadTriangles();
			meshData.AddUVCoordinates(QuadFaceUVs(Direction.Right));

			return meshData;
		}

		private Vector2[] QuadFaceUVs(Direction dir) {

			Vector2[] UVs = new Vector2[4];

			float x = 1f;
			float y = 1f;

			switch (dir) {
			case Direction.Up:
				x = (transform.localScale.z / topScale.x);
				y = (transform.localScale.x / topScale.y);
				UVs[0] = new Vector2(x + topOffset.x, 0f + topOffset.y);
				UVs[1] = new Vector2(x + topOffset.x, y + topOffset.y);
				UVs[2] = new Vector2(0f + topOffset.x, y + topOffset.y);
				UVs[3] = new Vector2(0f + topOffset.x, 0f + topOffset.y);
				break;
			case Direction.Down:
				x = (transform.localScale.z / (useUnifiedScaling ? topScale.x : bottomScale.x));
				y = (transform.localScale.x / (useUnifiedScaling ? topScale.y : bottomScale.y));
				UVs[0] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : bottomOffset.x), 0f + (useUnifiedOffset ? topOffset.y : bottomOffset.y));
				UVs[1] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : bottomOffset.x), y +  (useUnifiedOffset ? topOffset.y : bottomOffset.y));
				UVs[2] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : bottomOffset.x), y +  (useUnifiedOffset ? topOffset.y : bottomOffset.y));
				UVs[3] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : bottomOffset.x), 0f + (useUnifiedOffset ? topOffset.y : bottomOffset.y));
				break;
			case Direction.Left:
				x = (transform.localScale.z / (useUnifiedScaling ? topScale.x : leftScale.x));
				y = (transform.localScale.y / (useUnifiedScaling ? topScale.y : leftScale.y));
				UVs[0] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : leftOffset.x), 0f + (useUnifiedOffset ? topOffset.y : leftOffset.y));
				UVs[1] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : leftOffset.x), y +  (useUnifiedOffset ? topOffset.y : leftOffset.y));
				UVs[2] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : leftOffset.x), y +  (useUnifiedOffset ? topOffset.y : leftOffset.y));
				UVs[3] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : leftOffset.x), 0f + (useUnifiedOffset ? topOffset.y : leftOffset.y));
				break;
			case Direction.Right:
				x = (transform.localScale.z / (useUnifiedScaling ? topScale.x : rightScale.x));
				y = (transform.localScale.y / (useUnifiedScaling ? topScale.y : rightScale.y)) ;
				UVs[0] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : rightOffset.x), 0f + (useUnifiedOffset ? topOffset.y : rightOffset.y));
				UVs[1] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : rightOffset.x), y +  (useUnifiedOffset ? topOffset.y : rightOffset.y));
				UVs[2] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : rightOffset.x), y +  (useUnifiedOffset ? topOffset.y : rightOffset.y));
				UVs[3] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : rightOffset.x), 0f + (useUnifiedOffset ? topOffset.y : rightOffset.y));
				break;
			case Direction.Forward:
				x = (transform.localScale.x / (useUnifiedScaling ? topScale.x : frontScale.x));
				y = (transform.localScale.y / (useUnifiedScaling ? topScale.y : frontScale.y));
				UVs[0] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : frontOffset.x), 0f + (useUnifiedOffset ? topOffset.y : frontOffset.y));
				UVs[1] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : frontOffset.x), y +  (useUnifiedOffset ? topOffset.y : frontOffset.y));
				UVs[2] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : frontOffset.x), y +  (useUnifiedOffset ? topOffset.y : frontOffset.y));
				UVs[3] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : frontOffset.x), 0f + (useUnifiedOffset ? topOffset.y : frontOffset.y));
				break;
			case Direction.Back:
				x = (transform.localScale.x / (useUnifiedScaling ? topScale.x : backScale.x));
				y = (transform.localScale.y / (useUnifiedScaling ? topScale.y : backScale.y));
				UVs[0] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : backOffset.x), 0f + (useUnifiedOffset ? topOffset.y : backOffset.y));
				UVs[1] = new Vector2(x +  (useUnifiedOffset ? topOffset.x : backOffset.x), y +  (useUnifiedOffset ? topOffset.y : backOffset.y));
				UVs[2] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : backOffset.x), y +  (useUnifiedOffset ? topOffset.y : backOffset.y));
				UVs[3] = new Vector2(0f + (useUnifiedOffset ? topOffset.x : backOffset.x), 0f + (useUnifiedOffset ? topOffset.y : backOffset.y));
				break;
			}

			return UVs;

		}

		private MeshData SplitMeshForCubeProjection(MeshData data) {

//			Debug.Log ("Old count vertices: " + data.Vertices.Count + ", old count UVs: " + data.UV.Count);

			List<int> topTriangles = new List<int>();
			List<int> bottomTriangles = new List<int>();
			List<int> leftTriangles = new List<int>();
			List<int> rightTriangles = new List<int>();
			List<int> frontTriangles = new List<int>();
			List<int> backTriangles = new List<int>();

			for (int m = 0; m < data.Triangles.Length; m++) {
				for (int i = 0; i < data.Triangles[m].Count; i += 3) {
	//				Debug.Log ("Handling triangle entry: " + i);
					Vector3 triangleNormal = new Vector3();
					List<int> triangleVertIds = new List<int>();
					for (int tvId = 0; tvId < 3; tvId++) {
						int index = data.Triangles[m][i + tvId];
						triangleNormal += data.Normals[index];
						triangleVertIds.Add(index);
					}
					Direction triangleNormalDirection = GetCubeProjectionDirectionForNormal(triangleNormal.normalized);
					switch (triangleNormalDirection) {
					case Direction.Back:
						backTriangles.AddRange(triangleVertIds);
						break;
					case Direction.Down:
						bottomTriangles.AddRange(triangleVertIds);
						break;
					case Direction.Forward:
						frontTriangles.AddRange(triangleVertIds);
						break;
					case Direction.Left:
						leftTriangles.AddRange(triangleVertIds);
						break;
					case Direction.Right:
						rightTriangles.AddRange(triangleVertIds);
						break;
					case Direction.Up:
						topTriangles.AddRange(triangleVertIds);
						break;
					}
				}
			}

			MeshData newMeshData = new MeshData ();
			newMeshData.subMeshCount = Mathf.Max(new int[]{_backMaterialIndex, _bottomMaterialIndex, _frontMaterialIndex, _leftMaterialIndex, _rightMaterialIndex, _topMaterialIndex}) + 1;
			newMeshData = AddMeshDataForTriangleList (backTriangles, Vector3.back, newMeshData, data, _backMaterialIndex);
			newMeshData = AddMeshDataForTriangleList (bottomTriangles, Vector3.down, newMeshData, data, _bottomMaterialIndex);
			newMeshData = AddMeshDataForTriangleList (frontTriangles, Vector3.forward, newMeshData, data, _frontMaterialIndex);
			newMeshData = AddMeshDataForTriangleList (leftTriangles, Vector3.left, newMeshData, data, _leftMaterialIndex);
			newMeshData = AddMeshDataForTriangleList (rightTriangles, Vector3.right, newMeshData, data, _rightMaterialIndex);
			newMeshData = AddMeshDataForTriangleList (topTriangles, Vector3.up, newMeshData, data, _topMaterialIndex);

//			Debug.Log ("Count vertices: " + newMeshData.Vertices.Count + ", count normals: " + newMeshData.Normals.Count + ", count UVs: " + newMeshData.UV.Count);
//			foreach (var t in newMeshData.Triangles) {
//				Debug.Log("UV " + t + ": " + newMeshData.UV[t]);
//			}

			return newMeshData;

		}

		private MeshData AddMeshDataForTriangleList(List<int> triangleIds, Vector3 normalDirection, MeshData newData, MeshData oldData, int materialIndex) {

			Dictionary<int, int> oldIdNewIdMapping = new Dictionary<int, int> ();
			foreach (int vertId in triangleIds) {
				if (!oldIdNewIdMapping.ContainsKey(vertId)) {
					oldIdNewIdMapping[vertId] = newData.Vertices.Count;
//					Debug.Log (GetType() + ".AddMeshDataForTriangleList: adding triangle " + newData.Vertices.Count + " with material " + materialIndex);
					newData.AddTriangle(newData.Vertices.Count, materialIndex);
					newData.AddVertex(oldData.Vertices[vertId]);
					if (vertId < oldData.Tangents.Count) {
						newData.AddTangent(oldData.Tangents[vertId]);
					}
//					Debug.Log ("Adding Vertex: " + oldData.Vertices[vertId]);
					newData.AddNormal(oldData.Normals[vertId]);
					newData.AddUVCoordinate(VerticeUVByNormal(oldData.Vertices[vertId], normalDirection));
				}
				else {
//					Debug.Log (GetType() + ".AddMeshDataForTriangleList: adding triangle " + oldIdNewIdMapping[vertId] + " with material " + materialIndex);
					newData.AddTriangle(oldIdNewIdMapping[vertId], materialIndex);
				}
			}

			return newData;

		}

//		private MeshData AddMeshDataForTriangleList(List<int> triangleIds, Vector3 normalDirection, MeshData newData, MeshData oldData) {
//			
//			Dictionary<Vector3, int> oldIdNewIdMapping = new Dictionary<Vector3, int> ();
//			foreach (int vertId in triangleIds) {
//				if (!oldIdNewIdMapping.ContainsKey(oldData.Vertices[vertId])) {
//					oldIdNewIdMapping[oldData.Vertices[vertId]] = newData.Vertices.Count;
//					newData.AddTriangle(newData.Vertices.Count);
//					newData.AddVertex(oldData.Vertices[vertId]);
//					newData.AddNormal(oldData.Normals[vertId]);
//					newData.AddUVCoordinate(VerticeUVByNormal(oldData.Vertices[vertId], normalDirection));
//				}
//				else {
//					newData.AddTriangle(oldIdNewIdMapping[oldData.Vertices[vertId]]);
//				}
//			}
//			
//			return newData;
//			
//		}

		public static Direction GetCubeProjectionDirectionForNormal(Vector3 normal) {

			Direction uvDir = Direction.Up;
			float angle = Vector3.Angle (normal, Vector3.up);
			float newAngle = Vector3.Angle (normal, Vector3.down);
			if (newAngle < angle) {
				angle = newAngle;
				uvDir = Direction.Down;
			}
			newAngle = Vector3.Angle (normal, Vector3.left);
			if (newAngle < angle) {
				angle = newAngle;
				uvDir = Direction.Left;
			}
			newAngle = Vector3.Angle (normal, Vector3.right);
			if (newAngle < angle) {
				angle = newAngle;
				uvDir = Direction.Right;
			}
			newAngle = Vector3.Angle (normal, Vector3.forward);
			if (newAngle < angle) {
				angle = newAngle;
				uvDir = Direction.Forward;
			}
			newAngle = Vector3.Angle (normal, Vector3.back);
			if (newAngle < angle) {
				angle = newAngle;
				uvDir = Direction.Back;
			}

			return uvDir;

		}

		private Vector2 VerticeUVByNormal(Vector3 vertex, Vector3 normal) {

			Direction uvDir = GetCubeProjectionDirectionForNormal(normal);
			Vector2 uvCoord = new Vector2(1f, 1f);
			
			switch (uvDir) {
			case Direction.Up:
				uvCoord = Quaternion.Euler(0f, 0f, topRotation) * new Vector2(transform.localScale.z * vertex.z, transform.localScale.x * vertex.x);
				uvCoord.x = (uvCoord.x / topScale.x) + topOffset.x;
				uvCoord.y = (uvCoord.y / topScale.y) + topOffset.y;
				if (topFlipX) {
					uvCoord.x = 1 - uvCoord.x;
				}
				if (topFlipY) {
					uvCoord.y = 1 - uvCoord.y;
				}
				break;
			case Direction.Down:
				uvCoord = Quaternion.Euler(0f, 0f, bottomRotation) * new Vector2(transform.localScale.z * vertex.z, transform.localScale.x * vertex.x);
				uvCoord.x = (uvCoord.x / (useUnifiedScaling ? topScale.x : bottomScale.x)) + (useUnifiedOffset ? topOffset.x : bottomOffset.x);
				uvCoord.y = (uvCoord.y / (useUnifiedScaling ? topScale.y : bottomScale.y)) + (useUnifiedOffset ? topOffset.y : bottomOffset.y);
				if (bottomFlipX) {
					uvCoord.x = 1 - uvCoord.x;
				}
				if (bottomFlipY) {
					uvCoord.y = 1 - uvCoord.y;
				}
				break;
			case Direction.Left:
				uvCoord = Quaternion.Euler(0f, 0f, leftRotation) * new Vector2(transform.localScale.z * vertex.z, transform.localScale.y * vertex.y);
				uvCoord.x = (uvCoord.x / (useUnifiedScaling ? topScale.x : leftScale.x)) + (useUnifiedOffset ? topOffset.x : leftOffset.x);
				uvCoord.y = (uvCoord.y / (useUnifiedScaling ? topScale.y : leftScale.y)) + (useUnifiedOffset ? topOffset.y : leftOffset.y);
				if (leftFlipX) {
					uvCoord.x = 1 - uvCoord.x;
				}
				if (leftFlipY) {
					uvCoord.y = 1 - uvCoord.y;
				}
				break;
			case Direction.Right:
				uvCoord = Quaternion.Euler(0f, 0f, rightRotation) * new Vector2(transform.localScale.z * vertex.z, transform.localScale.y * vertex.y);
				uvCoord.x = (uvCoord.x / (useUnifiedScaling ? topScale.x : rightScale.x)) + (useUnifiedOffset ? topOffset.x : rightOffset.x);
				uvCoord.y = (uvCoord.y / (useUnifiedScaling ? topScale.y : rightScale.y)) + (useUnifiedOffset ? topOffset.y : rightOffset.y);
				if (rightFlipX) {
					uvCoord.x = 1 - uvCoord.x;
				}
				if (rightFlipY) {
					uvCoord.y = 1 - uvCoord.y;
				}
				break;
			case Direction.Forward:
				uvCoord = Quaternion.Euler(0f, 0f, frontRotation) * new Vector2(transform.localScale.x * vertex.x, transform.localScale.y * vertex.y);
				uvCoord.x = (uvCoord.x / (useUnifiedScaling ? topScale.x : frontScale.x)) + (useUnifiedOffset ? topOffset.x : frontOffset.x);
				uvCoord.y = (uvCoord.y / (useUnifiedScaling ? topScale.y : frontScale.y)) + (useUnifiedOffset ? topOffset.y : frontOffset.y);
				if (frontFlipX) {
					uvCoord.x = 1 - uvCoord.x;
				}
				if (frontFlipY) {
					uvCoord.y = 1 - uvCoord.y;
				}
				break;
			case Direction.Back:
				uvCoord = Quaternion.Euler(0f, 0f, backRotation) * new Vector2(transform.localScale.x * vertex.x, transform.localScale.y * vertex.y);
				uvCoord.x = (uvCoord.x / (useUnifiedScaling ? topScale.x : backScale.x)) + (useUnifiedOffset ? topOffset.x : backOffset.x);
				uvCoord.y = (uvCoord.y / (useUnifiedScaling ? topScale.y : backScale.y)) + (useUnifiedOffset ? topOffset.y : backOffset.y);
				if (backFlipX) {
					uvCoord.x = 1 - uvCoord.x;
				}
				if (backFlipY) {
					uvCoord.y = 1 - uvCoord.y;
				}
				break;
			}
			return uvCoord;

		}

#if UNITY_EDITOR
		public void SaveMeshAsset() {

			if (!meshFilter.sharedMesh) {
				Debug.LogError(name + ": " + GetType() + ".SaveMeshAsset: there was no mesh set.");
				return;
			}
			string currentMeshAssetPath = meshAssetPathString;
			string[] pathGUIDs = AssetDatabase.FindAssets("AutoTextureTilingTool");
			string foundPath = "";
			if (pathGUIDs == null || pathGUIDs.Length < 1) {
				Debug.LogError("No asset \"AutoTextureTilingTool\" was found.");
			}
			else {
				if (pathGUIDs.Length > 1) {
					Debug.LogWarning(GetType() + ".SaveMeshAsset: there is more than one path or asset called \"AutoTextureTilingTool\". There should only be one single path named \"AutoTextureTilingTool\"");
				}
				for (int i = 0; i < pathGUIDs.Length; i++) {
					foundPath = AssetDatabase.GUIDToAssetPath(pathGUIDs[i]);
					Debug.Log ("Found Asset: " + AssetDatabase.GUIDToAssetPath(pathGUIDs[i]));
					if (!string.IsNullOrEmpty(foundPath)) {
						currentMeshAssetPath = foundPath + "/Meshes/";
						break;
					}
				}
			}
			string[] pathParts = currentMeshAssetPath.Split('/');

			if (pathParts == null || pathParts.Length < 1) {
				Debug.LogError(GetType() + ".SaveMeshAsset: mesh asset path was set incorrectly.");
				return;
			}
			if (pathParts[0] != "Assets") {
				Debug.LogError(GetType() + ".SaveMeshAsset: mesh asset path has to start with \"Assets\".");
				return;
			}
			
			for (int i = 1; i < pathParts.Length; i++) {
				if (!string.IsNullOrEmpty(pathParts[i])) {
					string currentPath = pathParts[0];
					for (int curPathId = 1; curPathId < i; curPathId++) {
						currentPath += "/" + pathParts[curPathId];
					}
					if (!AssetDatabase.IsValidFolder(currentPath + "/" + pathParts[i])) {
						Debug.Log ("Creating folder " + currentPath + "/" + pathParts[i]);
						AssetDatabase.CreateFolder(currentPath, pathParts[i]);
					}
				}
			}
//			if (!AssetDatabase.IsValidFolder("Assets/AutoTextureTilingTool")) {
//				Debug.Log ("Creating folder AutoTextureTilingTool");
//				AssetDatabase.CreateFolder("Assets", "AutoTextureTilingTool");
//			}
//			if (!AssetDatabase.IsValidFolder("Assets/AutoTextureTilingTool/Meshes")) {
//				Debug.Log ("Creating folder Meshes");
//				AssetDatabase.CreateFolder("Assets/AutoTextureTilingTool", "Meshes");
//			}
			Mesh meshPrefab = AssetDatabase.LoadAssetAtPath(currentMeshAssetPath + meshFilter.sharedMesh.name + extensionString, typeof(Mesh)) as Mesh;
			if (meshPrefab) {
				if (meshPrefab != meshFilter.sharedMesh) {
					string[] meshNameParts = meshPrefab.name.Split('_');
					if (meshNameParts.Length > 1) {
						int numberSuffix = 0;
						if (int.TryParse(meshNameParts[meshNameParts.Length - 1],  out numberSuffix)) {
							numberSuffix++;
							string prefabName = currentMeshAssetPath + name + "_" + numberSuffix + extensionString;
							//							string prefabName = meshAssetPathString + EditorApplication.currentScene.Replace('/', '_').Replace('\\', '_') + "_" + name + "_" + numberSuffix + extensionString;
							meshPrefab = AssetDatabase.LoadAssetAtPath(prefabName, typeof(Mesh)) as Mesh;
							while (meshPrefab != null) {
								prefabName = currentMeshAssetPath + name + "_" + (++numberSuffix) + extensionString;
								//								prefabName = meshAssetPathString + EditorApplication.currentScene.Replace('/', '_').Replace('\\', '_') + "_" + name + "_" + (++numberSuffix) + extensionString;
								meshPrefab = AssetDatabase.LoadAssetAtPath(prefabName, typeof(Mesh)) as Mesh;
							}
							Debug.Log ("Creating mesh prefab at " + prefabName);
							_useBakedMesh = true;
							Mesh meshToSave = Mesh.Instantiate(meshFilter.sharedMesh);
							AssetDatabase.CreateAsset(meshToSave, prefabName);
							AssetDatabase.SaveAssets();
							meshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath(prefabName, typeof(Mesh)) as Mesh;
							EditorUtility.SetDirty(meshFilter.sharedMesh);
							EditorUtility.SetDirty(meshFilter);
							EditorUtility.SetDirty(this);
						}
					}
					else {
						Debug.LogError(name + ": " + GetType() + ".SaveMeshAsset: prefab name " + meshPrefab.name + " has no number suffix.");
					}
				}
			}
			else {
				int numberSuffix = 0;
				string prefabName = currentMeshAssetPath + name + "_" + numberSuffix + extensionString;
				//				string prefabName = meshAssetPathString + EditorApplication.currentScene.Replace('/', '_').Replace('\\', '_') + "_" + name + "_" + numberSuffix + extensionString;
				meshPrefab = AssetDatabase.LoadAssetAtPath(prefabName, typeof(Mesh)) as Mesh;
				while (meshPrefab != null) {
					prefabName = currentMeshAssetPath + name + "_" + (++numberSuffix) + extensionString;
					//					prefabName = meshAssetPathString + EditorApplication.currentScene.Replace('/', '_').Replace('\\', '_') + "_" + name + "_" + (++numberSuffix) + extensionString;
					meshPrefab = AssetDatabase.LoadAssetAtPath(prefabName, typeof(Mesh)) as Mesh;
				}
				Debug.Log ("Creating mesh prefab at " + prefabName);
				_useBakedMesh = true;
				Mesh meshToSave = Mesh.Instantiate(meshFilter.sharedMesh);
				AssetDatabase.CreateAsset(meshToSave, prefabName);
				AssetDatabase.SaveAssets();
				meshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath(prefabName, typeof(Mesh)) as Mesh;
				EditorUtility.SetDirty(meshFilter.sharedMesh);
				EditorUtility.SetDirty(meshFilter);
				EditorUtility.SetDirty(this);
			}

		}

		public void DeleteConnectedMesh() {

			string currentMeshAssetPath = meshAssetPathString;
			string[] pathGUIDs = AssetDatabase.FindAssets("AutoTextureTilingTool");
			string foundPath = "";
			if (pathGUIDs == null || pathGUIDs.Length < 1) {
				Debug.LogError("No asset \"AutoTextureTilingTool\" was found.");
			}
			else {
				if (pathGUIDs.Length > 1) {
					Debug.LogWarning(GetType() + ".SaveMeshAsset: there is more than one path or asset called \"AutoTextureTilingTool\". There should only be one single path named \"AutoTextureTilingTool\"");
				}
				for (int i = 0; i < pathGUIDs.Length; i++) {
					foundPath = AssetDatabase.GUIDToAssetPath(pathGUIDs[i]);
					Debug.Log ("Found Asset: " + AssetDatabase.GUIDToAssetPath(pathGUIDs[i]));
					if (!string.IsNullOrEmpty(foundPath)) {
						currentMeshAssetPath = foundPath + "/Meshes/";
						break;
					}
				}
			}
			Mesh meshPrefab = AssetDatabase.LoadAssetAtPath(currentMeshAssetPath + meshFilter.sharedMesh.name + extensionString, typeof(Mesh)) as Mesh;
			if (!meshPrefab && meshFilter.sharedMesh.name.EndsWith("(Clone)")) {
				meshPrefab = AssetDatabase.LoadAssetAtPath(currentMeshAssetPath + meshFilter.sharedMesh.name.Substring(0, meshFilter.sharedMesh.name.Length - 7) + extensionString, typeof(Mesh)) as Mesh;
			}
			if (meshPrefab) {
				Debug.Log(name + ": " + GetType() + ".SaveMeshAsset: deleting " + meshPrefab.name + ".");
				AutoTextureTiling[] listOfTextureTilingToolObjects = FindObjectsOfType<AutoTextureTiling>();
				for (int i = 0; i < listOfTextureTilingToolObjects.Length; i++) {
					if (listOfTextureTilingToolObjects[i].meshFilter.sharedMesh == meshPrefab) {
						listOfTextureTilingToolObjects[i].BreakMeshAssetConnection();
					}
				}
				if (!AssetDatabase.DeleteAsset(currentMeshAssetPath + meshPrefab.name + extensionString)) {
					Debug.LogError(name + ": " + GetType() + ".SaveMeshAsset: could not delete " + meshPrefab.name + ": failed to execute AssetDatabase.DeleteAsset.");
				}
				else {
					GameObject prefab = PrefabUtility.GetPrefabParent(this.gameObject) as GameObject;
					if (prefab) {
						Debug.LogWarning(GetType() + ".BreakMeshAssetConnection: mesh asset was deleted, but object was instance of a prefab. It is recommended to delete the prefab " + prefab.name + ".");
					}
				}
				AssetDatabase.SaveAssets();
			}
			else {
				_useBakedMesh = false;
				EditorUtility.SetDirty(this);
				Debug.LogError(name + ": " + GetType() + ".SaveMeshAsset: could not delete " + meshFilter.sharedMesh.name + ": Did not find the asset.");
			}
		}

		public void BreakMeshAssetConnection() {

			Debug.Log (name + ": " + GetType() + ".BreakMeshAssetConnection: reverting to on-the-run-created mesh.");
			if (_useBakedMesh) {
				if (meshFilter.sharedMesh) {
					Mesh meshCopy = Mesh.Instantiate(meshFilter.sharedMesh) as Mesh;
					meshFilter.sharedMesh = meshCopy;
					meshFilter.sharedMesh.name = "Mesh " + name;
				}
				_useBakedMesh = false;
				GameObject prefab = PrefabUtility.GetPrefabParent(this.gameObject) as GameObject;
				if (prefab) {
					PrefabUtility.DisconnectPrefabInstance(this.gameObject);
				}
				EditorUtility.SetDirty(this);
			}

		}
		
		public bool MeshPrefabExists() {

//			Debug.Log (name + ": " + GetType() + ".MeshPrefabExists: trying to find mesh asset with name " + meshFilter.sharedMesh.name + ".");
			string currentMeshAssetPath = meshAssetPathString;
			string[] pathGUIDs = AssetDatabase.FindAssets("AutoTextureTilingTool");
			string foundPath = "";
			if (pathGUIDs == null || pathGUIDs.Length < 1) {
				Debug.LogError("No asset \"AutoTextureTilingTool\" was found.");
			}
			else {
				if (pathGUIDs.Length > 1) {
					Debug.LogWarning(GetType() + ".SaveMeshAsset: there is more than one path or asset called \"AutoTextureTilingTool\". There should only be one single path named \"AutoTextureTilingTool\"");
				}
				for (int i = 0; i < pathGUIDs.Length; i++) {
					foundPath = AssetDatabase.GUIDToAssetPath(pathGUIDs[i]);
//					Debug.Log ("Found Asset: " + AssetDatabase.GUIDToAssetPath(pathGUIDs[i]));
					if (!string.IsNullOrEmpty(foundPath)) {
						currentMeshAssetPath = foundPath;
						break;
					}
				}
			}
			return AssetDatabase.LoadAssetAtPath(currentMeshAssetPath + meshFilter.sharedMesh.name + extensionString, typeof(Mesh)) as Mesh != null;

		}
#endif

//		#if UNITY_EDITOR
//		public void OnDrawGizmos() {
//
//			if (!meshFilter || (meshFilter && !meshFilter.mesh)) {
//				return;
//			}
//
//
//			for (int i = 0; i < meshFilter.mesh.vertices.Length; i++) {
//				Vector3 v = meshFilter.mesh.vertices[i];
//				int count = new List<Vector3>(meshFilter.mesh.vertices).FindAll(vertex => vertex == v).Count();
//				v.Scale(transform.localScale);
//				Gizmos.color = Color.red;
//				Gizmos.DrawLine(v + transform.position, v + transform.position + meshFilter.mesh.normals[i]);
//				switch (count) {
//				case 0:
//					Gizmos.color = Color.black;
//					break;
//				case 1:
//					Gizmos.color = Color.green;
//					break;
//				case 2:
//					Gizmos.color = Color.yellow;
//					break;
//				case 3:
//					Gizmos.color = Color.red;
//					break;
//				case 4:
//					Gizmos.color = Color.blue;
//					break;
//				case 5:
//					Gizmos.color = Color.white;
//					break;
//
//				}
//				Gizmos.DrawSphere(v + transform.position + meshFilter.mesh.normals[i], .05f);
//			}
//			
//		}
//		#endif
		
	}

	public enum Direction {

		Up,
		Down,
		Left,
		Right,
		Forward,
		Back,

	}

}
