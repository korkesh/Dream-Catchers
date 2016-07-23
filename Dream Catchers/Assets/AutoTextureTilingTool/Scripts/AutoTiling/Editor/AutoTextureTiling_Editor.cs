using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;

namespace AutoTiling {

	[CustomEditor(typeof(AutoTextureTiling))]
	public class AutoTextureTiling_Editor : Editor {

//		static Camera sceneCamera;
//		static List<Vector3> currentSelectedTriangles;
//		static Direction currentSelectedFace;
//		static Material lineMaterial;
//		static Tool lastUsedTool;
//		float amount = 0f;

		public override void OnInspectorGUI () {

			AutoTextureTiling Target = target as AutoTextureTiling;
			if (!Target) {
				Debug.LogError("AutoTextureTiling_Editor.OnInspectorGUI: target was not of type AutoTextureTiling.");
				Destroy(target);
				return;
			}

			bool changedAnything = false;
			EditorGUILayout.BeginHorizontal();
			if (Target.useBakedMesh) {
				if (GUILayout.Button("Delete Mesh Asset", GUILayout.Width(200f))) {
					Target.DeleteConnectedMesh();
					changedAnything = true;
				}
			}
			else {
				if (GUILayout.Button("Save Mesh Asset", GUILayout.Width(200f))) {
					Target.SaveMeshAsset();
					changedAnything = true;
				}
			}
			if (GUILayout.Button("ATT Window")) {
				TextureTilingEditorWindow.ShowWindow();
				TextureTilingEditorWindow.window.Repaint();
			}
			EditorGUILayout.EndHorizontal();

			bool oldUnifiedScaling = Target.useUnifiedScaling;
			Target.useUnifiedScaling = EditorGUILayout.Toggle("Use Unified Scaling", Target.useUnifiedScaling);
			if (Target.useUnifiedScaling) {
				EditorGUI.BeginChangeCheck ();
				Vector2 newValue = EditorGUILayout.Vector2Field ("Top Scale", Target.topScale);
				if (EditorGUI.EndChangeCheck () || oldUnifiedScaling != Target.useUnifiedScaling) {
					Target.topScale = newValue;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
			}
			else {
				EditorGUI.BeginChangeCheck ();
				Vector2 newValue = EditorGUILayout.Vector2Field ("Top Scale", Target.topScale);
				if (EditorGUI.EndChangeCheck () || oldUnifiedScaling != Target.useUnifiedScaling) {
					Target.topScale = newValue;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newValue = EditorGUILayout.Vector2Field ("Bottom Scale", Target.bottomScale);
				if (EditorGUI.EndChangeCheck () || oldUnifiedScaling != Target.useUnifiedScaling) {
					Target.bottomScale = newValue;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newValue = EditorGUILayout.Vector2Field ("Left Scale", Target.leftScale);
				if (EditorGUI.EndChangeCheck () || oldUnifiedScaling != Target.useUnifiedScaling) {
					Target.leftScale = newValue;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newValue = EditorGUILayout.Vector2Field ("Right Scale", Target.rightScale);
				if (EditorGUI.EndChangeCheck () || oldUnifiedScaling != Target.useUnifiedScaling) {
					Target.rightScale = newValue;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newValue = EditorGUILayout.Vector2Field ("Front Scale", Target.frontScale);
				if (EditorGUI.EndChangeCheck () || oldUnifiedScaling != Target.useUnifiedScaling) {
					Target.frontScale = newValue;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newValue = EditorGUILayout.Vector2Field ("Back Scale", Target.backScale);
				if (EditorGUI.EndChangeCheck () || oldUnifiedScaling != Target.useUnifiedScaling) {
					Target.backScale = newValue;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
			}

			EditorGUILayout.Space ();

			bool oldUnifiedOffset = Target.useUnifiedOffset;
			Target.useUnifiedOffset = EditorGUILayout.Toggle("Use Unified Offset", Target.useUnifiedOffset);
			if (Target.useUnifiedOffset) {
				EditorGUI.BeginChangeCheck ();
				Vector2 newOffset = EditorGUILayout.Vector2Field ("Top Offset", Target.topOffset);
				if (EditorGUI.EndChangeCheck () || oldUnifiedOffset != Target.useUnifiedOffset) {
					Target.topOffset = newOffset;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
			}
			else {
				EditorGUI.BeginChangeCheck ();
				Vector2 newOffset = EditorGUILayout.Vector2Field ("Top Offset", Target.topOffset);
				if (EditorGUI.EndChangeCheck () || oldUnifiedOffset != Target.useUnifiedOffset) {
					Target.topOffset = newOffset;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newOffset = EditorGUILayout.Vector2Field ("Bottom Offset", Target.bottomOffset);
				if (EditorGUI.EndChangeCheck () || oldUnifiedOffset != Target.useUnifiedOffset) {
					Target.bottomOffset = newOffset;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newOffset = EditorGUILayout.Vector2Field ("Left Offset", Target.leftOffset);
				if (EditorGUI.EndChangeCheck () || oldUnifiedOffset != Target.useUnifiedOffset) {
					Target.leftOffset = newOffset;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newOffset = EditorGUILayout.Vector2Field ("Right Offset", Target.rightOffset);
				if (EditorGUI.EndChangeCheck () || oldUnifiedOffset != Target.useUnifiedOffset) {
					Target.rightOffset = newOffset;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newOffset = EditorGUILayout.Vector2Field ("Front Offset", Target.frontOffset);
				if (EditorGUI.EndChangeCheck () || oldUnifiedOffset != Target.useUnifiedOffset) {
					Target.frontOffset = newOffset;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
				EditorGUI.BeginChangeCheck ();
				newOffset = EditorGUILayout.Vector2Field ("Back Offset", Target.backOffset);
				if (EditorGUI.EndChangeCheck () || oldUnifiedOffset != Target.useUnifiedOffset) {
					Target.backOffset = newOffset;
					EditorUtility.SetDirty(Target);
					changedAnything = true;
				}
			}

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			float newRotation = EditorGUILayout.FloatField("Top Rotation", Target.topRotation);
			if (EditorGUI.EndChangeCheck()) {
				Target.topRotation = newRotation;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUI.BeginChangeCheck();
			newRotation = EditorGUILayout.FloatField("Bottom Rotation", Target.bottomRotation);
			if (EditorGUI.EndChangeCheck()) {
				Target.bottomRotation = newRotation;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			newRotation = EditorGUILayout.FloatField("Left Rotation", Target.leftRotation);
			if (EditorGUI.EndChangeCheck()) {
				Target.leftRotation = newRotation;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUI.BeginChangeCheck();
			newRotation = EditorGUILayout.FloatField("Right Rotation", Target.rightRotation);
			if (EditorGUI.EndChangeCheck()) {
				Target.rightRotation = newRotation;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			newRotation = EditorGUILayout.FloatField("Front Rotation", Target.frontRotation);
			if (EditorGUI.EndChangeCheck()) {
				Target.frontRotation = newRotation;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUI.BeginChangeCheck();
			newRotation = EditorGUILayout.FloatField("Back Rotation", Target.backRotation);
			if (EditorGUI.EndChangeCheck()) {
				Target.backRotation = newRotation;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			bool newFlipValueX = EditorGUILayout.Toggle("Top Flip X", Target.topFlipX);
			bool newFlipValueY = EditorGUILayout.Toggle("Top Flip Y", Target.topFlipY);
			if (EditorGUI.EndChangeCheck()) {
				Target.topFlipX = newFlipValueX;
				Target.topFlipY = newFlipValueY;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			newFlipValueX = EditorGUILayout.Toggle("Bottom Flip X", Target.bottomFlipX);
			newFlipValueY = EditorGUILayout.Toggle("Bottom Flip Y", Target.bottomFlipY);
			if (EditorGUI.EndChangeCheck()) {
				Target.bottomFlipX = newFlipValueX;
				Target.bottomFlipY = newFlipValueY;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			newFlipValueX = EditorGUILayout.Toggle("Left Flip X", Target.leftFlipX);
			newFlipValueY = EditorGUILayout.Toggle("Left Flip Y", Target.leftFlipY);
			if (EditorGUI.EndChangeCheck()) {
				Target.leftFlipX = newFlipValueX;
				Target.leftFlipY = newFlipValueY;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			newFlipValueX = EditorGUILayout.Toggle("Right Flip X", Target.rightFlipX);
			newFlipValueY = EditorGUILayout.Toggle("Right Flip Y", Target.rightFlipY);
			if (EditorGUI.EndChangeCheck()) {
				Target.rightFlipX = newFlipValueX;
				Target.rightFlipY = newFlipValueY;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			newFlipValueX = EditorGUILayout.Toggle("Front Flip X", Target.frontFlipX);
			newFlipValueY = EditorGUILayout.Toggle("Front Flip Y", Target.frontFlipY);
			if (EditorGUI.EndChangeCheck()) {
				Target.frontFlipX = newFlipValueX;
				Target.frontFlipY = newFlipValueY;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			newFlipValueX = EditorGUILayout.Toggle("Back Flip X", Target.backFlipX);
			newFlipValueY = EditorGUILayout.Toggle("Back Flip Y", Target.backFlipY);
			if (EditorGUI.EndChangeCheck()) {
				Target.backFlipX = newFlipValueX;
				Target.backFlipY = newFlipValueY;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space ();

			string[] options = new string[Target.Renderer.sharedMaterials.Length];

			for (int i = 0; i < Target.Renderer.sharedMaterials.Length; i++) {
				if (Target.Renderer.sharedMaterials[i] == null) {
					options[i] = "[NULL]";
				}
				else {
					options[i] = Target.Renderer.sharedMaterials[i].name;
				}
			}

			int newMaterialIndex = EditorGUILayout.Popup("Top Material", Target.topMaterialIndex, options);
			if (newMaterialIndex != Target.topMaterialIndex && Target.Renderer.sharedMaterials[newMaterialIndex] != null) {
				Target.topMaterialIndex = newMaterialIndex;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			newMaterialIndex = EditorGUILayout.Popup("Bottom Material", Target.bottomMaterialIndex, options);
			if (newMaterialIndex != Target.bottomMaterialIndex && Target.Renderer.sharedMaterials[newMaterialIndex] != null) {
				Target.bottomMaterialIndex = newMaterialIndex;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			newMaterialIndex = EditorGUILayout.Popup("Left Material", Target.leftMaterialIndex, options);
			if (newMaterialIndex != Target.leftMaterialIndex && Target.Renderer.sharedMaterials[newMaterialIndex] != null) {
				Target.leftMaterialIndex = newMaterialIndex;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			newMaterialIndex = EditorGUILayout.Popup("Right Material", Target.rightMaterialIndex, options);
			if (newMaterialIndex != Target.rightMaterialIndex && Target.Renderer.sharedMaterials[newMaterialIndex] != null) {
				Target.rightMaterialIndex = newMaterialIndex;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			newMaterialIndex = EditorGUILayout.Popup("Front Material", Target.frontMaterialIndex, options);
			if (newMaterialIndex != Target.frontMaterialIndex && Target.Renderer.sharedMaterials[newMaterialIndex] != null) {
				Target.frontMaterialIndex = newMaterialIndex;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}
			newMaterialIndex = EditorGUILayout.Popup("Back Material", Target.backMaterialIndex, options);
			if (newMaterialIndex != Target.backMaterialIndex && Target.Renderer.sharedMaterials[newMaterialIndex] != null) {
				Target.backMaterialIndex = newMaterialIndex;
				EditorUtility.SetDirty(Target);
				changedAnything = true;
			}

			if (changedAnything) {
				if (Target.useBakedMesh) {
					GameObject prefab = PrefabUtility.GetPrefabParent(Target.gameObject) as GameObject;
					if (prefab) {
						PrefabUtility.ReplacePrefab(Target.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
					}
				}
				EditorApplication.MarkSceneDirty();
			}

		}

		[ExecuteInEditMode]
		public void OnSceneGUI() {

			if ((GUIUtility.hotControl != 0 && (Event.current.type == EventType.MouseDown && Event.current.button == 0)) || (Selection.activeGameObject != null && TextureTilingEditorWindow.selected == null)) {
				TextureTilingEditorWindow.currentSelectedTriangles = new List<Vector3>();
				TextureTilingEditorWindow.currentSelectedTriangleNormal = Vector3.zero;
				//				Debug.Log ("OnSceneGUI: mouse down detected.");
				RaycastHit hitInfo;
				if (!TextureTilingEditorWindow.sceneCamera) {
					GameObject sceneCameraObj = GameObject.Find( "SceneCamera" );
					if (sceneCameraObj != null) {
						TextureTilingEditorWindow.sceneCamera = sceneCameraObj.GetComponent<Camera>();
					}
				}
				if (TextureTilingEditorWindow.sceneCamera) {
					Vector3 mousePos = Event.current.mousePosition;
					//				Debug.Log ("Current mouse position: " + mousePos);
					//				Debug.Log ("Viewport height: " + Screen.height);
					mousePos.y = TextureTilingEditorWindow.sceneCamera.pixelHeight - Event.current.mousePosition.y;
					//				Debug.Log ("Current actual mouse position: " + mousePos);
					
					if (Physics.Raycast(TextureTilingEditorWindow.sceneCamera.ScreenPointToRay(mousePos), out hitInfo, float.PositiveInfinity)) {
						//					Debug.Log ("OnSceneGUI: hit something.");
						AutoTextureTiling att = hitInfo.transform.GetComponent<AutoTextureTiling>();
						if (!att) {
							att = hitInfo.transform.GetComponent<DynamicTextureTiling>();
						}
						if (att) {
							if (Tools.current != Tool.None) {
								TextureTilingEditorWindow.lastUsedTool = Tools.current;
							}
							if (TextureTilingEditorWindow.currentTextureTool != Tool.None) {
//								Debug.Log ("Setting Tools.current to none");
								Tools.current = Tool.None;
							}
							TextureTilingEditorWindow.selected = att;
//							TextureTilingEditorWindow.ShowWindow();
//							TextureTilingEditorWindow.window.Repaint();
							TextureTilingEditorWindow.currentSelectedFace = AutoTextureTiling.GetCubeProjectionDirectionForNormal(Quaternion.Inverse(att.transform.rotation) * hitInfo.normal);
//							Debug.Log ("Selected " + TextureTilingEditorWindow.currentSelectedFace.ToString() + " on " + att.name);
							Vector3 currentNormalSum = Vector3.zero;
							int normalCount = 0;
							for (int i = 0; i < att.meshFilter.sharedMesh.triangles.Length; i += 3) {
								Vector3 normals = Vector3.zero;
								Vector3[] currentTriangle = new Vector3[3];
								for (int j = 0; j < 3; j++) {
									int vertexIndex = att.meshFilter.sharedMesh.triangles[i + j];
									Vector3 normal = att.meshFilter.sharedMesh.normals[vertexIndex];
									normals += normal;
									normal = new Vector3(normal.x / att.transform.localScale.x, normal.y / att.transform.localScale.y, normal.z / att.transform.localScale.z);
									currentTriangle[j] = att.meshFilter.sharedMesh.vertices[vertexIndex] + (normal * 0.001f);
								}
								//								if (AutoTextureTiling.GetCubeProjectionDirectionForNormal(att.transform.rotation * (normals / 3f)) == TextureTilingEditorWindow.currentSelectedFace) {
								if (AutoTextureTiling.GetCubeProjectionDirectionForNormal(normals / 3f) == TextureTilingEditorWindow.currentSelectedFace) {
									//									for (int t = 0; t < currentTriangle.Length; t++) {
									//										if (!currentSelectedTriangles.Contains(currentTriangle[t]) {
									normalCount += 3;
									currentNormalSum += normals;
									TextureTilingEditorWindow.currentSelectedTriangles.AddRange(currentTriangle);
									//										}
									//									}
								}
							}
							TextureTilingEditorWindow.currentSelectedTriangleNormal = currentNormalSum / normalCount;
						}
						else {
							if (TextureTilingEditorWindow.window != null) {
//								Debug.LogWarning (GetType() + ".OnSceneGUI: resetting current tool. TextureTilingEditorWindow.lastUsedTool = " + TextureTilingEditorWindow.lastUsedTool);
								Tools.current = TextureTilingEditorWindow.lastUsedTool;
								if (Tools.current == Tool.None) {
									Debug.LogWarning (GetType() + ".OnSceneGUI: setting current tool. Fallback to Move tool.");
									Tools.current = Tool.Move;
								}
							}
						}
					}
				}
				else {
					Debug.LogError(GetType() + ".OnSceneGUI: there was no editor SceneCamera. This should not be possible.");
				}
			}
			
			if (TextureTilingEditorWindow.currentSelectedTriangles != null && TextureTilingEditorWindow.window != null) {
				AutoTextureTiling att = target as AutoTextureTiling;
				TextureTilingEditorWindow.CreateLineMaterial();
				TextureTilingEditorWindow.lineMaterial.SetPass(0);
				GL.PushMatrix();
				for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i += 3) {
					GL.Begin(GL.TRIANGLES);
					for (int j = 0; j < 3; j++) {
						GL.Color(new Color(1f, 0.5f, 0.5f, .5f));
						Vector3 currentVertex = TextureTilingEditorWindow.currentSelectedTriangles[i + j];
						GL.Vertex(att.transform.rotation * (new Vector3(currentVertex.x * att.transform.localScale.x, currentVertex.y * att.transform.localScale.y, currentVertex.z * att.transform.localScale.z)) + att.transform.position);
					}
					GL.End();
				}
				GL.PopMatrix();
				
				if (TextureTilingEditorWindow.currentTextureTool != Tool.None && TextureTilingEditorWindow.selected != null) {
					Vector3 handlePosition = TextureTilingEditorWindow.selected.transform.position;
					float x = TextureTilingEditorWindow.currentSelectedFace == Direction.Left ? float.PositiveInfinity : 0f;
					float y = TextureTilingEditorWindow.currentSelectedFace == Direction.Down ? float.PositiveInfinity : 0f;
					float z = TextureTilingEditorWindow.currentSelectedFace == Direction.Back ? float.PositiveInfinity : 0f;
					float currentTextureRotation = 0f;
					//					float currentTextureScale = 0f;
					switch (TextureTilingEditorWindow.currentSelectedFace) {
					case Direction.Back:
						for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
							y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
							x += TextureTilingEditorWindow.currentSelectedTriangles[i].x;
							if (z > TextureTilingEditorWindow.currentSelectedTriangles[i].z) {
								z = TextureTilingEditorWindow.currentSelectedTriangles[i].z;
							}
						}
						y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.y;
						x = (x / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.x;
						z *= TextureTilingEditorWindow.selected.transform.localScale.z;
						currentTextureRotation = TextureTilingEditorWindow.selected.backRotation;
//						currentTextureScale = TextureTilingEditorWindow.selected.backScale;
						break;
					case Direction.Forward:
						for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
							y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
							x += TextureTilingEditorWindow.currentSelectedTriangles[i].x;
							if (z < TextureTilingEditorWindow.currentSelectedTriangles[i].z) {
								z = TextureTilingEditorWindow.currentSelectedTriangles[i].z;
							}
						}
						y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.y;
						x = (x / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.x;
						z *= TextureTilingEditorWindow.selected.transform.localScale.z;
						currentTextureRotation = TextureTilingEditorWindow.selected.frontRotation;
//						currentTextureScale = TextureTilingEditorWindow.selected.frontScale;
						break;
					case Direction.Left:
						for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
							z += TextureTilingEditorWindow.currentSelectedTriangles[i].z;
							y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
							if (x > TextureTilingEditorWindow.currentSelectedTriangles[i].x) {
								x = TextureTilingEditorWindow.currentSelectedTriangles[i].x;
							}
						}
						z = (z / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.z;
						y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.y;
						x *= TextureTilingEditorWindow.selected.transform.localScale.x;
						currentTextureRotation = TextureTilingEditorWindow.selected.leftRotation;
//						currentTextureScale = TextureTilingEditorWindow.selected.leftScale;
						break;
					case Direction.Right:
						for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
							z += TextureTilingEditorWindow.currentSelectedTriangles[i].z;
							y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
							if (x < TextureTilingEditorWindow.currentSelectedTriangles[i].x) {
								x = TextureTilingEditorWindow.currentSelectedTriangles[i].x;
							}
						}
						z = (z / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.z;
						y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.y;
						x *= TextureTilingEditorWindow.selected.transform.localScale.x;
						currentTextureRotation = TextureTilingEditorWindow.selected.rightRotation;
//						currentTextureScale = TextureTilingEditorWindow.selected.rightScale;
						break;
					case Direction.Down:
						for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
							x += TextureTilingEditorWindow.currentSelectedTriangles[i].x;
							z += TextureTilingEditorWindow.currentSelectedTriangles[i].z;
							if (y > TextureTilingEditorWindow.currentSelectedTriangles[i].y) {
								y = TextureTilingEditorWindow.currentSelectedTriangles[i].y;
							}
						}
						x = (x / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.x;
						z = (z / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.z;
						y *= TextureTilingEditorWindow.selected.transform.localScale.y;
						currentTextureRotation = TextureTilingEditorWindow.selected.bottomRotation;
//						currentTextureScale = TextureTilingEditorWindow.selected.bottomScale;
						break;
					case Direction.Up:
						for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
							x += TextureTilingEditorWindow.currentSelectedTriangles[i].x;
							z += TextureTilingEditorWindow.currentSelectedTriangles[i].z;
//							y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
							if (y < TextureTilingEditorWindow.currentSelectedTriangles[i].y) {
								y = TextureTilingEditorWindow.currentSelectedTriangles[i].y;
							}
						}
						x = (x / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.x;
						z = (z / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.z;
//						y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * TextureTilingEditorWindow.selected.transform.localScale.y;
						y *= TextureTilingEditorWindow.selected.transform.localScale.y;
						currentTextureRotation = TextureTilingEditorWindow.selected.topRotation;
//						currentTextureScale = TextureTilingEditorWindow.selected.topScale;
						break;
					}
					handlePosition = handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * new Vector3(x, y, z));
					Quaternion rotationToCurrentSide = Quaternion.FromToRotation(Vector3.up, TextureTilingEditorWindow.currentSelectedTriangleNormal);
					float amountX = 0f;
					float amountY = 0f;
					float scaleAmount = 0f;

					float sizeModifier = HandleUtility.GetHandleSize(handlePosition);

					bool changedAnything = false;
					switch (TextureTilingEditorWindow.currentTextureTool) {
					case Tool.Move:
//						EditorGUI.BeginChangeCheck();
//						Handles.color = Color.red;
//						float amountX = 
//							ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(-currentTextureRotation, Vector3.up), 1f, Handles.ArrowCap);
//						Handles.color = Color.blue;
//						float amountY = 
//							ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(90f - currentTextureRotation, Vector3.up), 1f, Handles.ArrowCap);;
						switch(TextureTilingEditorWindow.currentSelectedFace) {
						case Direction.Back:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f + currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f + currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);;
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (att.useUnifiedOffset) {
									att.backOffset = new Vector2(att.topOffset.x + amountX, att.topOffset.y + amountY);
								}
								else {
									att.backOffset = new Vector2(att.backOffset.x + amountX, att.backOffset.y + amountY);
								}
							}
							Handles.color = Color.blue;
							if (ATTHandles.TextureAlignHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetCenter(TextureTilingEditorWindow.currentSelectedFace);
							}
							Handles.color = Color.white;
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.up * (att.transform.localScale.y / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetTop(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.down * (att.transform.localScale.y / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetBottom(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.left * (att.transform.localScale.x / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetLeft(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.right * (att.transform.localScale.x / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetRight(TextureTilingEditorWindow.currentSelectedFace);
							}
							break;
						case Direction.Down:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f + currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);;
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (att.useUnifiedOffset) {
									att.bottomOffset = new Vector2(att.topOffset.x + amountX, att.topOffset.y + amountY);
								}
								else {
									att.bottomOffset = new Vector2(att.bottomOffset.x + amountX, att.bottomOffset.y + amountY);
								}
							}
							Handles.color = Color.blue;
							if (ATTHandles.TextureAlignHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetCenter(TextureTilingEditorWindow.currentSelectedFace);
							}
							Handles.color = Color.white;
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.forward * (att.transform.localScale.z / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetLeft(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.back * (att.transform.localScale.z / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetRight(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.left * (att.transform.localScale.x / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetBottom(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.right * (att.transform.localScale.x / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetTop(TextureTilingEditorWindow.currentSelectedFace);
							}

							break;
						case Direction.Forward:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f - currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(-currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);;
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (att.useUnifiedOffset) {
									att.frontOffset = new Vector2(att.topOffset.x + amountX, att.topOffset.y + amountY);
								}
								else {
									att.frontOffset = new Vector2(att.frontOffset.x + amountX, att.frontOffset.y + amountY);
								}
							}
							Handles.color = Color.blue;
							if (ATTHandles.TextureAlignHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetCenter(TextureTilingEditorWindow.currentSelectedFace);
							}
							Handles.color = Color.white;
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.up * (att.transform.localScale.y / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetTop(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.down * (att.transform.localScale.y / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetBottom(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.left * (att.transform.localScale.x / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetLeft(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.right * (att.transform.localScale.x / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetRight(TextureTilingEditorWindow.currentSelectedFace);
							}

							break;
						case Direction.Left:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f - currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f - currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);;
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (att.useUnifiedOffset) {
									att.leftOffset = new Vector2(att.topOffset.x + amountX, att.topOffset.y + amountY);
								}
								else {
									att.leftOffset = new Vector2(att.leftOffset.x + amountX, att.leftOffset.y + amountY);
								}
							}
							Handles.color = Color.blue;
							if (ATTHandles.TextureAlignHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetCenter(TextureTilingEditorWindow.currentSelectedFace);
							}
							Handles.color = Color.white;
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.up * (att.transform.localScale.y / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetTop(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.down * (att.transform.localScale.y / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetBottom(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.back * (att.transform.localScale.z / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetLeft(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.forward * (att.transform.localScale.z / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetRight(TextureTilingEditorWindow.currentSelectedFace);
							}

							break;
						case Direction.Right:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180 + currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(90f + currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);;
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (att.useUnifiedOffset) {
									att.rightOffset = new Vector2(att.topOffset.x + amountX, att.topOffset.y + amountY);
								}
								else {
									att.rightOffset = new Vector2(att.rightOffset.x + amountX, att.rightOffset.y + amountY);
								}
							}
							Handles.color = Color.blue;
							if (ATTHandles.TextureAlignHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetCenter(TextureTilingEditorWindow.currentSelectedFace);
							}
							Handles.color = Color.white;
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.up * (att.transform.localScale.y / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetTop(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.down * (att.transform.localScale.y / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetBottom(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.back * (att.transform.localScale.z / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetLeft(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.forward * (att.transform.localScale.z / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetRight(TextureTilingEditorWindow.currentSelectedFace);
							}

							break;
						case Direction.Up:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f - currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TexturePositionHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f - currentTextureRotation, Vector3.up), sizeModifier, Handles.ArrowCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
//								if (att.useUnifiedOffset) {
									att.topOffset = new Vector2(att.topOffset.x + amountX, att.topOffset.y + amountY);
//								}
//								else {
//									att.topOffset = new Vector2(att.topOffset.x + amountX, att.topOffset.y + amountY);
//								}
							}
							Handles.color = Color.blue;
							if (ATTHandles.TextureAlignHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetCenter(TextureTilingEditorWindow.currentSelectedFace);
							}
							Handles.color = Color.white;
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.forward * (att.transform.localScale.z / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetLeft(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.back * (att.transform.localScale.z / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetRight(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.left * (att.transform.localScale.x / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetBottom(TextureTilingEditorWindow.currentSelectedFace);
							}
							if (ATTHandles.TextureAlignHandle(handlePosition + (TextureTilingEditorWindow.selected.transform.rotation * Vector3.right * (att.transform.localScale.x / 2f)), TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide, sizeModifier * 0.1f, Handles.CubeCap)) {
								att.AlignOffsetTop(TextureTilingEditorWindow.currentSelectedFace);
							}

							break;
						}
						break;
					case Tool.Scale:
						switch(TextureTilingEditorWindow.currentSelectedFace) {
						case Direction.Back:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.white;
							scaleAmount = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(225f + currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f + currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f + currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (scaleAmount != 0f) {
									float factor = (att.backScale.x + scaleAmount) / att.backScale.x;
									amountX = scaleAmount;
									amountY = (att.backScale.y * factor) - att.backScale.y;
								}
								if (att.useUnifiedScaling) {
									att.backScale = new Vector2(att.topScale.x + amountX, att.topScale.y + amountY);
								}
								else {
									att.backScale = new Vector2(att.backScale.x + amountX, att.backScale.y + amountY);
								}
							}
							break;
						case Direction.Down:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.white;
							scaleAmount = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(315f + currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f + currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (scaleAmount != 0f) {
									float factor = (att.bottomScale.x + scaleAmount) / att.bottomScale.x;
									amountX = scaleAmount;
									amountY = (att.bottomScale.y * factor) - att.bottomScale.y;
								}
								if (att.useUnifiedScaling) {
									att.bottomScale = new Vector2(att.topScale.x + amountX, att.topScale.y + amountY);
								}
								else {
									att.bottomScale = new Vector2(att.bottomScale.x + amountX, att.bottomScale.y + amountY);
								}
							}
							break;
						case Direction.Forward:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.white;
							scaleAmount = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(315f - currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270 - currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(-currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (scaleAmount != 0f) {
									float factor = (att.frontScale.x + scaleAmount) / att.frontScale.x;
									amountX = scaleAmount;
									amountY = (att.frontScale.y * factor) - att.frontScale.y;
								}
								if (att.useUnifiedScaling) {
									att.frontScale = new Vector2(att.topScale.x + amountX, att.topScale.y + amountY);
								}
								else {
									att.frontScale = new Vector2(att.frontScale.x + amountX, att.frontScale.y + amountY);
								}
							}
							break;
						case Direction.Left:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.white;
							scaleAmount = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(225f - currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f - currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f - currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (scaleAmount != 0f) {
									float factor = (att.leftScale.x + scaleAmount) / att.leftScale.x;
									amountX = scaleAmount;
									amountY = (att.leftScale.y * factor) - att.leftScale.y;
								}
								if (att.useUnifiedScaling) {
									att.leftScale = new Vector2(att.topScale.x + amountX, att.topScale.y + amountY);
								}
								else {
									att.leftScale = new Vector2(att.leftScale.x + amountX, att.leftScale.y + amountY);
								}
							}
							break;
						case Direction.Right:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.white;
							scaleAmount = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(135f + currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f + currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(90f + currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (scaleAmount != 0f) {
									float factor = (att.rightScale.x + scaleAmount) / att.rightScale.x;
									amountX = scaleAmount;
									amountY = (att.rightScale.y * factor) - att.rightScale.y;
								}
								if (att.useUnifiedScaling) {
									att.rightScale = new Vector2(att.topScale.x + amountX, att.topScale.y + amountY);
								}
								else {
									att.rightScale = new Vector2(att.rightScale.x + amountX, att.rightScale.y + amountY);
								}
							}
							break;
						case Direction.Up:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.white;
							scaleAmount = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(225f - currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f - currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							Handles.color = Color.blue;
							amountY = 
								ATTHandles.TextureScaleHandle(handlePosition, TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270f - currentTextureRotation, Vector3.up), sizeModifier * 0.1f, Handles.CubeCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								if (scaleAmount != 0f) {
									float factor = (att.topScale.x + scaleAmount) / att.topScale.x;
									amountX = scaleAmount; //(att.topScale.x * factor) - att.topScale.x;
									amountY = (att.topScale.y * factor) - att.topScale.y;
								}
								att.topScale = new Vector2(att.topScale.x + amountX, att.topScale.y + amountY);
							}
							break;
						}
//						Handles.DoScaleHandle(Vector3.one, handlePosition, TextureTilingEditorWindow.selected.transform.rotation, 1f);
						break;
					case Tool.Rotate:
						switch(TextureTilingEditorWindow.currentSelectedFace) {
						case Direction.Back:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureRotationHandle(handlePosition, 
								                                 TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(att.backRotation - 90f, Vector3.up), 
								                                 TextureTilingEditorWindow.selected.transform.rotation * TextureTilingEditorWindow.currentSelectedTriangleNormal,
								                                 att.transform.rotation * Vector3.up,
								                                 att.backRotation, 
								                                 sizeModifier * 0.1f, 
								                                 Handles.SphereCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								att.backRotation = 90f - amountX;
							}
							break;
						case Direction.Down:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureRotationHandle(handlePosition, 
								                                 TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(att.bottomRotation, Vector3.up), 
								                                 TextureTilingEditorWindow.selected.transform.rotation * TextureTilingEditorWindow.currentSelectedTriangleNormal,
								                                 att.transform.rotation * Vector3.back,
								                                 att.bottomRotation, 
								                                 sizeModifier * 0.1f, 
								                                 Handles.SphereCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								att.bottomRotation = -amountX;
							}
							break;
						case Direction.Forward:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureRotationHandle(handlePosition, 
								                                 TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(270 - att.frontRotation, Vector3.up), 
								                                 TextureTilingEditorWindow.selected.transform.rotation * TextureTilingEditorWindow.currentSelectedTriangleNormal,
								                                 att.transform.rotation * Vector3.up,
								                                 att.frontRotation, 
								                                 sizeModifier * 0.1f, 
								                                 Handles.SphereCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								att.frontRotation = amountX + 90;
							}
							break;
						case Direction.Left:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureRotationHandle(handlePosition, 
								                                 TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f - att.leftRotation, Vector3.up), 
								                                 TextureTilingEditorWindow.selected.transform.rotation * TextureTilingEditorWindow.currentSelectedTriangleNormal,
								                                 att.transform.rotation * Vector3.back,
								                                 att.leftRotation, 
								                                 sizeModifier * 0.1f, 
								                                 Handles.SphereCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								att.leftRotation = amountX;
							}
							break;
						case Direction.Right:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureRotationHandle(handlePosition, 
								                                 TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f + att.rightRotation, Vector3.up), 
								                                 TextureTilingEditorWindow.selected.transform.rotation * TextureTilingEditorWindow.currentSelectedTriangleNormal,
								                                 att.transform.rotation * Vector3.back,
								                                 att.rightRotation, 
								                                 sizeModifier * 0.1f, 
								                                 Handles.SphereCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								att.rightRotation = -amountX;
							}
							break;
						case Direction.Up:
							EditorGUI.BeginChangeCheck();
							Handles.color = Color.red;
							amountX = 
								ATTHandles.TextureRotationHandle(handlePosition, 
								                                 TextureTilingEditorWindow.selected.transform.rotation * rotationToCurrentSide * Quaternion.AngleAxis(180f - att.topRotation, Vector3.up), 
								                                 TextureTilingEditorWindow.selected.transform.rotation * TextureTilingEditorWindow.currentSelectedTriangleNormal,
								                                 att.transform.rotation * Vector3.back,
								                                 att.topRotation, 
								                                 sizeModifier * 0.1f, 
								                                 Handles.SphereCap);
							if (EditorGUI.EndChangeCheck()) {
								changedAnything = true;
								att.topRotation = amountX;
							}
							break;
						}
						break;
					}

//					switch (TextureTilingEditorWindow.currentSelectedFace) {
//					case Direction.Back:
//						break;
//					case Direction.Down:
//						break;
//					case Direction.Forward:
//						break;
//					case Direction.Left:
//						break;
//					case Direction.Right:
//						break;
//					case Direction.Up:
//						break;
//					}
						
					if (changedAnything) {
						if (att.useBakedMesh) {
							GameObject prefab = PrefabUtility.GetPrefabParent(att.gameObject) as GameObject;
							if (prefab) {
								PrefabUtility.ReplacePrefab(att.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
							}
						}
						EditorApplication.MarkSceneDirty();
					}
				}
			}
			
		}

	}

}
