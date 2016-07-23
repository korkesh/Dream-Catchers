using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AutoTiling {

	public class TextureTilingEditorWindow : EditorWindow {

		public static TextureTilingEditorWindow window;

		public static Camera sceneCamera;
		public static AutoTextureTiling selected;
		public static List<Vector3> currentSelectedTriangles;
		public static Vector3 currentSelectedTriangleNormal;
		public static Direction currentSelectedFace;
		public static Material lineMaterial;
		public static Tool lastUsedTool;
		public static Tool currentTextureTool = Tool.None;

		private static Texture unityIcon;
		private static Texture moveIcon;
		private static Texture scaleIcon;
		private static Texture rotateIcon;

		private static int selectedToolIndex = 0;

		public static float offsetStepSize = .1f;
		public static float scaleStepSize = .1f;
		public static float rotationStepSize = 15f;

		[MenuItem("Window/Auto Texture Window")]
		public static void ShowWindow() {
			if (!window) {
				window = EditorWindow.GetWindow<TextureTilingEditorWindow>();
				window.titleContent = new GUIContent("Auto Tiling");
			}
			window.Repaint();
			if (!unityIcon || !moveIcon || !scaleIcon || !rotateIcon) {
				unityIcon =  LoadTextureWithName("unity-icon");
				moveIcon =   LoadTextureWithName("move-icon");
				scaleIcon =  LoadTextureWithName("scale-icon");
				rotateIcon = LoadTextureWithName("rotate-icon");
//				unityIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AutoTextureTilingTool/Icons/unity-icon.png");
//				moveIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AutoTextureTilingTool/Icons/move-icon.png");
//				scaleIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AutoTextureTilingTool/Icons/scale-icon.png");
//				rotateIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/AutoTextureTilingTool/Icons/rotate-icon.png");
			}
			switch (currentTextureTool) {
			case Tool.None:
				selectedToolIndex = 0;
				break;
			case Tool.Move:
				selectedToolIndex = 1;
				break;
			case Tool.Rotate:
				selectedToolIndex = 2;
				break;
			case Tool.Scale:
				selectedToolIndex = 3;
				break;
			}
		}

		private static Texture LoadTextureWithName(string textureFileName) {

			string[] iconGUIDs = AssetDatabase.FindAssets(textureFileName);
			Texture icon = null;
			if (iconGUIDs != null && iconGUIDs.Length > 0) {
				for (int i = 0; i < iconGUIDs.Length; i++) {
//					Debug.Log ("Trying to load: " + AssetDatabase.GUIDToAssetPath(iconGUIDs[i]));
					icon = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(iconGUIDs[i]));
					if (icon) {
						break;
					}
				}
			}
			if (!icon) {
				Debug.LogError("TextureTilingEditorWindow.LoadTextureWithName: texture with name \"" + textureFileName + "\" was not found in the project folder, it was probably deleted.");
			}
			return icon;

		}
		
		void OnGUI() {
//			if (GUILayout.Button (new GUIContent()) {
//
//			}
			if (Selection.objects == null || Selection.objects.Length < 1) {
				EditorGUILayout.HelpBox("There are no objects selected.", MessageType.Warning);
			}
			else if (Selection.objects.Length > 1) {
				EditorGUILayout.HelpBox("More than one object was selected. Multi editing not supported.", MessageType.Warning);
			}
			else if (Selection.activeGameObject != null) {
				if (TextureTilingEditorWindow.selected) {
					int newIndex = GUILayout.Toolbar(selectedToolIndex, 
						new GUIContent[]{new GUIContent(unityIcon), new GUIContent(moveIcon), new GUIContent(rotateIcon), new GUIContent(scaleIcon)}, 
						GUILayout.Width(180f), GUILayout.Height(32f));
						if (newIndex != selectedToolIndex) {
							selectedToolIndex = newIndex;
						if (selectedToolIndex == 0) {
//							Debug.LogWarning (GetType() + ".OnGUI: resetting current tool. lastUsedTool = " + lastUsedTool);
							Tools.current = lastUsedTool;
							if (Tools.current == Tool.None) {
								Debug.LogWarning (GetType() + ".OnGUI: setting current tool. Fallback to Move tool.");
								Tools.current = Tool.Move;
							}
							currentTextureTool = Tool.None;
						}
						else {
							if (Tools.current != Tool.None) {
								lastUsedTool = Tools.current;
							}
							Tools.current = Tool.None;
//							Debug.Log ("Setting Tools.current to none");
							switch(selectedToolIndex) {
							case 1:
								currentTextureTool = Tool.Move;
								break;
							case 2:
								currentTextureTool = Tool.Rotate;
								break;
							case 3:
								currentTextureTool = Tool.Scale;
								break;
							}
						}
						SceneView.RepaintAll();

					}
					Vector2 newOffset;
					Vector2 newScale;
					float newRotation;
					bool newUnifiedOffset;
					bool newUnifiedScale;
					int newMaterialIndex;
					bool newFlipX;
					bool newFlipY;
					EditorGUILayout.HelpBox(Selection.activeGameObject.name + ", " + TextureTilingEditorWindow.currentSelectedFace + " was selected.", MessageType.Info);

					bool changedAnything = false;
					string[] options = new string[selected.Renderer.sharedMaterials.Length];
					for (int i = 0; i < selected.Renderer.sharedMaterials.Length; i++) {
						if (selected.Renderer.sharedMaterials[i] == null) {
							options[i] = "[NULL]";
						}
						else {
							options[i] = selected.Renderer.sharedMaterials[i].name;
						}
					}
					switch (TextureTilingEditorWindow.currentSelectedFace) {
					case Direction.Back:
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newOffset = EditorGUILayout.Vector2Field("Offset", selected.useUnifiedOffset ? selected.topOffset : selected.backOffset, GUILayout.Width(150f));
						newUnifiedOffset = EditorGUILayout.Toggle("Use Unified Offset", selected.useUnifiedOffset);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						newScale = EditorGUILayout.Vector2Field("Scale", selected.useUnifiedScaling ? selected.topScale : selected.backScale, GUILayout.Width(150f));
						newUnifiedScale = EditorGUILayout.Toggle("Use Unified Scale", selected.useUnifiedScaling);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Rotation", GUILayout.Width(50f));
						newRotation = EditorGUILayout.FloatField(selected.backRotation, GUILayout.Width(80f));
						EditorGUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck()) {
							changedAnything = true;
							selected.useUnifiedOffset = newUnifiedOffset;
							selected.useUnifiedScaling = newUnifiedScale;
							selected.backOffset = newOffset;
							selected.backScale = newScale;
							selected.backRotation = newRotation;
							EditorUtility.SetDirty(selected);
						}
						if (GUILayout.Button("Fill Side with Texture")) {
							changedAnything = true;
							selected.SetTextureToFit(Direction.Back);
						}

						newMaterialIndex = EditorGUILayout.Popup("Material", selected.backMaterialIndex, options);
						if (newMaterialIndex != selected.backMaterialIndex && selected.Renderer.sharedMaterials[newMaterialIndex] != null) {
							selected.backMaterialIndex = newMaterialIndex;
							EditorUtility.SetDirty(selected);
							changedAnything = true;
						}

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newFlipX = EditorGUILayout.Toggle("Flip X", selected.backFlipX);
						newFlipY = EditorGUILayout.Toggle("Flip Y", selected.backFlipY);
						if (EditorGUI.EndChangeCheck()) {
							selected.backFlipX = newFlipX;
							selected.backFlipY = newFlipY;
							changedAnything = true;
						}
						EditorGUILayout.EndHorizontal();
						break;
					case Direction.Down:
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newOffset = EditorGUILayout.Vector2Field("Offset", selected.useUnifiedOffset ? selected.topOffset : selected.bottomOffset, GUILayout.Width(150f));
						newUnifiedOffset = EditorGUILayout.Toggle("Use Unified Offset", selected.useUnifiedOffset);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						newScale = EditorGUILayout.Vector2Field("Scale", selected.useUnifiedScaling ? selected.topScale : selected.bottomScale, GUILayout.Width(150f));
						newUnifiedScale = EditorGUILayout.Toggle("Use Unified Scale", selected.useUnifiedScaling);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Rotation", GUILayout.Width(50f));
						newRotation = EditorGUILayout.FloatField(selected.bottomRotation, GUILayout.Width(80f));
						EditorGUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck()) {
							changedAnything = true;
							selected.useUnifiedOffset = newUnifiedOffset;
							selected.useUnifiedScaling = newUnifiedScale;
							selected.bottomOffset = newOffset;
							selected.bottomScale = newScale;
							selected.bottomRotation = newRotation;
							EditorUtility.SetDirty(selected);
						}
						if (GUILayout.Button("Fill Side with Texture")) {
							changedAnything = true;
							selected.SetTextureToFit(Direction.Down);
						}

						newMaterialIndex = EditorGUILayout.Popup("Material", selected.bottomMaterialIndex, options);
						if (newMaterialIndex != selected.bottomMaterialIndex && selected.Renderer.sharedMaterials[newMaterialIndex] != null) {
							selected.bottomMaterialIndex = newMaterialIndex;
							EditorUtility.SetDirty(selected);
							changedAnything = true;
						}
						
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newFlipX = EditorGUILayout.Toggle("Flip X", selected.bottomFlipX);
						newFlipY = EditorGUILayout.Toggle("Flip Y", selected.bottomFlipY);
						if (EditorGUI.EndChangeCheck()) {
							selected.bottomFlipX = newFlipX;
							selected.bottomFlipY = newFlipY;
							changedAnything = true;
						}
						EditorGUILayout.EndHorizontal();
						break;
					case Direction.Forward:
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newOffset = EditorGUILayout.Vector2Field("Offset", selected.useUnifiedOffset ? selected.topOffset : selected.frontOffset, GUILayout.Width(150f));
						newUnifiedOffset = EditorGUILayout.Toggle("Use Unified Offset", selected.useUnifiedOffset);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						newScale = EditorGUILayout.Vector2Field("Scale", selected.useUnifiedScaling ? selected.topScale : selected.frontScale, GUILayout.Width(150f));
						newUnifiedScale = EditorGUILayout.Toggle("Use Unified Scale", selected.useUnifiedScaling);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Rotation", GUILayout.Width(50f));
						newRotation = EditorGUILayout.FloatField(selected.frontRotation, GUILayout.Width(80f));
						EditorGUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck()) {
							changedAnything = true;
							selected.useUnifiedOffset = newUnifiedOffset;
							selected.useUnifiedScaling = newUnifiedScale;
							selected.frontOffset = newOffset;
							selected.frontScale = newScale;
							selected.frontRotation = newRotation;
							EditorUtility.SetDirty(selected);
						}
						if (GUILayout.Button("Fill Side with Texture")) {
							changedAnything = true;
							selected.SetTextureToFit(Direction.Forward);
						}

						newMaterialIndex = EditorGUILayout.Popup("Material", selected.frontMaterialIndex, options);
						if (newMaterialIndex != selected.frontMaterialIndex && selected.Renderer.sharedMaterials[newMaterialIndex] != null) {
							selected.frontMaterialIndex = newMaterialIndex;
							EditorUtility.SetDirty(selected);
							changedAnything = true;
						}
						
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newFlipX = EditorGUILayout.Toggle("Flip X", selected.frontFlipX);
						newFlipY = EditorGUILayout.Toggle("Flip Y", selected.frontFlipY);
						if (EditorGUI.EndChangeCheck()) {
							selected.frontFlipX = newFlipX;
							selected.frontFlipY = newFlipY;
							changedAnything = true;
						}
						EditorGUILayout.EndHorizontal();
						break;
					case Direction.Left:
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newOffset = EditorGUILayout.Vector2Field("Offset", selected.useUnifiedOffset ? selected.topOffset : selected.leftOffset, GUILayout.Width(150f));
						newUnifiedOffset = EditorGUILayout.Toggle("Use Unified Offset", selected.useUnifiedOffset);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						newScale = EditorGUILayout.Vector2Field("Scale", selected.useUnifiedScaling ? selected.topScale : selected.leftScale, GUILayout.Width(150f));
						newUnifiedScale = EditorGUILayout.Toggle("Use Unified Scale", selected.useUnifiedScaling);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Rotation", GUILayout.Width(50f));
						newRotation = EditorGUILayout.FloatField(selected.leftRotation, GUILayout.Width(80f));
						EditorGUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck()) {
							changedAnything = true;
							selected.useUnifiedOffset = newUnifiedOffset;
							selected.useUnifiedScaling = newUnifiedScale;
							selected.leftOffset = newOffset;
							selected.leftScale = newScale;
							selected.leftRotation = newRotation;
							EditorUtility.SetDirty(selected);
						}
						if (GUILayout.Button("Fill Side with Texture")) {
							changedAnything = true;
							selected.SetTextureToFit(Direction.Left);
						}

						newMaterialIndex = EditorGUILayout.Popup("Material", selected.leftMaterialIndex, options);
						if (newMaterialIndex != selected.leftMaterialIndex && selected.Renderer.sharedMaterials[newMaterialIndex] != null) {
							selected.leftMaterialIndex = newMaterialIndex;
							EditorUtility.SetDirty(selected);
							changedAnything = true;
						}
						
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newFlipX = EditorGUILayout.Toggle("Flip X", selected.leftFlipX);
						newFlipY = EditorGUILayout.Toggle("Flip Y", selected.leftFlipY);
						if (EditorGUI.EndChangeCheck()) {
							selected.leftFlipX = newFlipX;
							selected.leftFlipY = newFlipY;
							changedAnything = true;
						}
						EditorGUILayout.EndHorizontal();
						break;
					case Direction.Right:
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newOffset = EditorGUILayout.Vector2Field("Offset", selected.useUnifiedOffset ? selected.topOffset : selected.rightOffset, GUILayout.Width(150f));
						newUnifiedOffset = EditorGUILayout.Toggle("Use Unified Offset", selected.useUnifiedOffset);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						newScale = EditorGUILayout.Vector2Field("Scale", selected.useUnifiedScaling ? selected.topScale : selected.rightScale, GUILayout.Width(150f));
						newUnifiedScale = EditorGUILayout.Toggle("Use Unified Scale", selected.useUnifiedScaling);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Rotation", GUILayout.Width(50f));
						newRotation = EditorGUILayout.FloatField(selected.rightRotation, GUILayout.Width(80f));
						EditorGUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck()) {
							changedAnything = true;
							selected.useUnifiedOffset = newUnifiedOffset;
							selected.useUnifiedScaling = newUnifiedScale;
							selected.rightOffset = newOffset;
							selected.rightScale = newScale;
							selected.rightRotation = newRotation;
							EditorUtility.SetDirty(selected);
						}
						if (GUILayout.Button("Fill Side with Texture")) {
							changedAnything = true;
							selected.SetTextureToFit(Direction.Right);
						}

						newMaterialIndex = EditorGUILayout.Popup("Material", selected.rightMaterialIndex, options);
						if (newMaterialIndex != selected.rightMaterialIndex && selected.Renderer.sharedMaterials[newMaterialIndex] != null) {
							selected.rightMaterialIndex = newMaterialIndex;
							EditorUtility.SetDirty(selected);
							changedAnything = true;
						}
						
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newFlipX = EditorGUILayout.Toggle("Flip X", selected.rightFlipX);
						newFlipY = EditorGUILayout.Toggle("Flip Y", selected.rightFlipY);
						if (EditorGUI.EndChangeCheck()) {
							selected.rightFlipX = newFlipX;
							selected.rightFlipY = newFlipY;
							changedAnything = true;
						}
						EditorGUILayout.EndHorizontal();
						break;
					case Direction.Up:
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newOffset = EditorGUILayout.Vector2Field("Offset", selected.useUnifiedOffset ? selected.topOffset : selected.topOffset, GUILayout.Width(150f));
						newUnifiedOffset = EditorGUILayout.Toggle("Use Unified Offset", selected.useUnifiedOffset);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						newScale = EditorGUILayout.Vector2Field("Scale", selected.useUnifiedScaling ? selected.topScale : selected.topScale, GUILayout.Width(150f));
						newUnifiedScale = EditorGUILayout.Toggle("Use Unified Scale", selected.useUnifiedScaling);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Rotation", GUILayout.Width(50f));
						newRotation = EditorGUILayout.FloatField(selected.topRotation, GUILayout.Width(80f));
						EditorGUILayout.EndHorizontal();
						if (EditorGUI.EndChangeCheck()) {
							changedAnything = true;
							selected.useUnifiedOffset = newUnifiedOffset;
							selected.useUnifiedScaling = newUnifiedScale;
							selected.topOffset = newOffset;
							selected.topScale = newScale;
							selected.topRotation = newRotation;
							EditorUtility.SetDirty(selected);
						}
						if (GUILayout.Button("Fill Side with Texture")) {
							changedAnything = true;
							selected.SetTextureToFit(Direction.Up);
						}

						newMaterialIndex = EditorGUILayout.Popup("Material", selected.topMaterialIndex, options);
						if (newMaterialIndex != selected.topMaterialIndex && selected.Renderer.sharedMaterials[newMaterialIndex] != null) {
							selected.topMaterialIndex = newMaterialIndex;
							EditorUtility.SetDirty(selected);
							changedAnything = true;
						}
						
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.BeginHorizontal();
						newFlipX = EditorGUILayout.Toggle("Flip X", selected.topFlipX);
						newFlipY = EditorGUILayout.Toggle("Flip Y", selected.topFlipY);
						if (EditorGUI.EndChangeCheck()) {
							selected.topFlipX = newFlipX;
							selected.topFlipY = newFlipY;
							changedAnything = true;
						}
						EditorGUILayout.EndHorizontal();
						break;
					}
					if (changedAnything) {
						if (selected.useBakedMesh) {
							GameObject prefab = PrefabUtility.GetPrefabParent(selected.gameObject) as GameObject;
							if (prefab) {
								PrefabUtility.ReplacePrefab(selected.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
							}
						}
						EditorApplication.MarkSceneDirty();
					}
					offsetStepSize = Mathf.Clamp01(EditorGUILayout.FloatField("Offset Snap Step", offsetStepSize));
					scaleStepSize = Mathf.Clamp01(EditorGUILayout.FloatField("Scale Snap Step", scaleStepSize));
					rotationStepSize = Mathf.Clamp(EditorGUILayout.FloatField("Rotation Snap Step", rotationStepSize), 0f, 360f);
				}
				else {
					AutoTextureTiling tempSelected_att = Selection.activeGameObject.GetComponent<AutoTextureTiling>();
					DynamicTextureTiling tempSelected_dtt = Selection.activeGameObject.GetComponent<DynamicTextureTiling>();
					if (!(tempSelected_att || tempSelected_dtt)) {
						EditorGUILayout.HelpBox("No side selected on " + Selection.activeGameObject.name + ". It maybe has no auto texture tiling component.", MessageType.Warning);
					}
					else {
						EditorGUILayout.HelpBox(Selection.activeGameObject.name + " selected. Click again for options.", MessageType.Warning);
					}
				}
			}
			else {
				EditorGUILayout.HelpBox("A strange error occurred.", MessageType.Error);
			}
		}

//		[ExecuteInEditMode]
//		void OnSceneGUI() {
//
//			if (currentTextureTool != Tool.None) {
//				Vector3 handlePosition = selected.transform.position;
//				float x = TextureTilingEditorWindow.currentSelectedFace == Direction.Left ? float.PositiveInfinity : 0f;
//				float y = TextureTilingEditorWindow.currentSelectedFace == Direction.Down ? float.PositiveInfinity : 0f;
//				float z = TextureTilingEditorWindow.currentSelectedFace == Direction.Back ? float.PositiveInfinity : 0f;
//				switch (TextureTilingEditorWindow.currentSelectedFace) {
//				case Direction.Back:
//					for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
//						y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
//						x += TextureTilingEditorWindow.currentSelectedTriangles[i].x;
//						if (z > TextureTilingEditorWindow.currentSelectedTriangles[i].z) {
//							z = TextureTilingEditorWindow.currentSelectedTriangles[i].z;
//						}
//					}
//					y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.y;
//					x = (x / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.x;
//					z *= selected.transform.localScale.z;
//					break;
//				case Direction.Forward:
//					for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
//						y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
//						x += TextureTilingEditorWindow.currentSelectedTriangles[i].x;
//						if (z < TextureTilingEditorWindow.currentSelectedTriangles[i].z) {
//							z = TextureTilingEditorWindow.currentSelectedTriangles[i].z;
//						}
//					}
//					y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.y;
//					x = (x / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.x;
//					z *= selected.transform.localScale.z;
//					break;
//				case Direction.Left:
//					for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
//						z += TextureTilingEditorWindow.currentSelectedTriangles[i].z;
//						y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
//						if (x > TextureTilingEditorWindow.currentSelectedTriangles[i].x) {
//							x = TextureTilingEditorWindow.currentSelectedTriangles[i].x;
//						}
//					}
//					z = (z / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.z;
//					y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.y;
//					x *= selected.transform.localScale.x;
//					break;
//				case Direction.Right:
//					for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
//						z += TextureTilingEditorWindow.currentSelectedTriangles[i].z;
//						y += TextureTilingEditorWindow.currentSelectedTriangles[i].y;
//						if (x < TextureTilingEditorWindow.currentSelectedTriangles[i].x) {
//							x = TextureTilingEditorWindow.currentSelectedTriangles[i].x;
//						}
//					}
//					z = (z / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.z;
//					y = (y / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.y;
//					x *= selected.transform.localScale.x;
//					break;
//				case Direction.Down:
//					for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
//						x += TextureTilingEditorWindow.currentSelectedTriangles[i].x;
//						z += TextureTilingEditorWindow.currentSelectedTriangles[i].z;
//						if (y > TextureTilingEditorWindow.currentSelectedTriangles[i].y) {
//							y = TextureTilingEditorWindow.currentSelectedTriangles[i].y;
//						}
//					}
//					x = (x / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.x;
//					z = (z / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.z;
//					y *= selected.transform.localScale.y;
//					break;
//				case Direction.Up:
//					for (int i = 0; i < TextureTilingEditorWindow.currentSelectedTriangles.Count; i++) {
//						x += TextureTilingEditorWindow.currentSelectedTriangles[i].x;
//						z += TextureTilingEditorWindow.currentSelectedTriangles[i].z;
//						if (y < TextureTilingEditorWindow.currentSelectedTriangles[i].y) {
//							y = TextureTilingEditorWindow.currentSelectedTriangles[i].y;
//						}
//					}
//					x = (x / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.x;
//					z = (z / TextureTilingEditorWindow.currentSelectedTriangles.Count) * selected.transform.localScale.z;
//					y *= selected.transform.localScale.y;
//					break;
//				}
//				handlePosition += (selected.transform.rotation * new Vector3(x, y, z));
//				switch (TextureTilingEditorWindow.currentTextureTool) {
//				case Tool.Move:
//					Handles.DoPositionHandle(handlePosition, selected.transform.rotation);
//					//					Handles.CubeCap(0, handlePosition, att.transform.rotation, 0.08f);
//					break;
//				case Tool.Scale:
//					Handles.DoScaleHandle(Vector3.one, handlePosition, selected.transform.rotation, 0.5f);
//					break;
//				case Tool.Rotate:
//					Handles.DoRotationHandle(selected.transform.rotation, handlePosition);
//					break;
//				}
//			}
//
//		}

		void OnSelectionChange() {

			if (Selection.activeGameObject == null
			    || (Selection.activeGameObject.GetComponent<AutoTextureTiling>() == null && Selection.activeGameObject.GetComponent<DynamicTextureTiling>() == null)) {
//				Debug.LogWarning (GetType() + ".OnSelectionChange: resetting current tool. lastUsedTool = " + lastUsedTool);
				Tools.current = lastUsedTool;
				if (Tools.current == Tool.None) {
					Debug.LogWarning (GetType() + "OnSelectionChange: setting current tool. Fallback to Move tool.");
					Tools.current = Tool.Move;
				}
//				Debug.Log("Resetting selected object.");
				selected = null;
				currentSelectedTriangles = new List<Vector3>();
				currentSelectedTriangleNormal = Vector3.zero;
			}
			else {
				selected = Selection.activeGameObject.GetComponent<AutoTextureTiling>();
//				if (selected) {
//					Debug.Log("Setting selected to " + selected.name);
//				}
//				else {
//					Debug.Log("Selected was not set.");
//				}
			}
			Repaint();

		}

		void OnDestroy() {

			currentTextureTool = Tool.None;
//			Debug.LogWarning (GetType() + ".OnDestroy: resetting current tool. lastUsedTool = " + lastUsedTool);
			Tools.current = lastUsedTool;
			if (Tools.current == Tool.None) {
				Debug.LogWarning (GetType() + "OnDestroy: setting current tool. Fallback to Move tool.");
				Tools.current = Tool.Move;
			}

		}

		public static void CreateLineMaterial () {
			if (!lineMaterial) {
				// Unity has a built-in shader that is useful for drawing
				// simple colored things.
				var shader = Shader.Find ("Hidden/Internal-Colored");
				lineMaterial = new Material (shader);
				lineMaterial.hideFlags = HideFlags.HideAndDontSave;
				// Turn on alpha blending
				lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				// Turn backface culling off
				lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				// Turn off depth writes
				lineMaterial.SetInt ("_ZWrite", 0);
			}
		}

		[MenuItem("GameObject/3D Object/Auto Tiled Texture Block")]
		public static void CreateAutoTilingTextureBlock() {
			
			GameObject cube = new GameObject ("Cube");
			cube.AddComponent<MeshFilter> ();
			cube.AddComponent<MeshRenderer> ();
			cube.AddComponent<BoxCollider> ();
			AutoTextureTiling atTiling = cube.AddComponent<AutoTextureTiling> ();
			string[] gridMaterialPaths = AssetDatabase.FindAssets("GridMat.mat");
//			Debug.Log ("gridMaterialPaths.Length = " + gridMaterialPaths.Length);
			Material gridMaterial = null;
			if (gridMaterialPaths != null && gridMaterialPaths.Length > 0) {
				for (int i = 0; i < gridMaterialPaths.Length; i++) {
//					Debug.Log ("gridMaterialPaths[i] = " + gridMaterialPaths[i]);
					gridMaterial = AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath(gridMaterialPaths[i]), typeof(Material)) as Material;
					if (gridMaterial != null) {
						break;
					}
				}
			} 
			if (!gridMaterial) {
				Debug.LogWarning("CreateAutoTilingTextureBlock: did not find the material \"GridMat.mat\" anywhere in the project folder.");
			}
			else {
				atTiling.Renderer.material = gridMaterial;
			}
			EditorUtility.SetDirty (atTiling.Renderer);
			Selection.activeGameObject = cube;
			
		}

	}

}