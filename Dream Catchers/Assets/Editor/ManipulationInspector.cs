using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(ManipulationScript))]
public class ManipulationInspector : Editor {

    AnimBool m_ShowAppearenceFields;
    bool appearenceBtn = false;

    AnimBool m_ShowPhysicsFields;
    bool physicsBtn = false;

    int platformIndex = 0;

    ManipulationScript manipScript;

    void Awake()
    {
        manipScript = (ManipulationScript)target;

        m_ShowAppearenceFields = new AnimBool(false);
        m_ShowAppearenceFields.valueChanged.AddListener(Repaint);

        m_ShowPhysicsFields = new AnimBool(false);
        m_ShowPhysicsFields.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField ("Current State:     " + manipScript.currentObjectState.ToString(), EditorStyles.boldLabel);
        EditorGUILayout.LabelField(" ");

        // Appearence Changes
        appearenceBtn = EditorGUILayout.Toggle("Appearence Change", appearenceBtn);
        if (appearenceBtn)
        {
            m_ShowAppearenceFields.value = true;
        }
        else
        {
            m_ShowAppearenceFields.value = false;
        }

        if (EditorGUILayout.BeginFadeGroup(m_ShowAppearenceFields.faded))
        {

            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            manipScript.dreamTexture = (Texture)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Dream", manipScript.dreamTexture, typeof(Texture), true);
            manipScript.nightmareTexture = (Texture)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Nightmare", manipScript.nightmareTexture, typeof(Texture), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField(" ");
            EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            manipScript.dreamMesh = (Mesh)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Dream", manipScript.dreamMesh, typeof(Mesh), true);
            manipScript.nightmareMesh = (Mesh)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Nightmare", manipScript.nightmareMesh, typeof(Mesh), true);
            EditorGUI.indentLevel--;


            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFadeGroup();

        // Physics Changes
        EditorGUILayout.LabelField(" ");
        physicsBtn = EditorGUILayout.Toggle("Physics Change", physicsBtn);
        if (physicsBtn)
        {
            m_ShowPhysicsFields.value = true;
        }
        else
        {
            m_ShowPhysicsFields.value = false;
        }

        if (EditorGUILayout.BeginFadeGroup(m_ShowPhysicsFields.faded))
        {

            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Colliders", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            manipScript.dreamCollider = (Collider)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Dream", manipScript.dreamCollider, typeof(Collider), true);
            manipScript.nightmareCollider = (Collider)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Nightmare", manipScript.nightmareCollider, typeof(Collider), true);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFadeGroup();

        // Misc Properties
        EditorGUILayout.LabelField(" ");
        EditorGUILayout.LabelField("Misc. Properties", EditorStyles.boldLabel);

        // Platform Types
        GUIContent platformLabel = new GUIContent("No Platform");
        GUIContent platformDreamLabel = new GUIContent("Make Dream Platform", "Requires Dream Mesh and Dream Collider");
        GUIContent platformNightmareLabel = new GUIContent("Make Nightmare Platform", "Requires Nightmare Mesh and Nightmare Collider");
        GUIContent[] platformItems = { platformLabel, platformDreamLabel, platformNightmareLabel };

        platformIndex = ToggleList(platformIndex, platformItems);
        if(platformIndex == 1)
        {
            manipScript.isDreamPlatform = true;
            manipScript.isNightmarePlatform = false;
        }
        else if (platformIndex == 2)
        {
            manipScript.isDreamPlatform = false;
            manipScript.isNightmarePlatform = true;
        }
        else
        {
            manipScript.isDreamPlatform = false;
            manipScript.isNightmarePlatform = false;
        }
        //DrawDefaultInspector();


    }

    /// <summary>
    /// Displays a vertical list of toggles and returns the index of the selected item.
    /// </summary>
    public static int ToggleList(int selected, GUIContent[] items)
    {
        // Keep the selected index within the bounds of the items array
        selected = selected < 0 ? 0 : selected >= items.Length ? items.Length - 1 : selected;

        GUILayout.BeginVertical();
        for (int i = 0; i < items.Length; i++)
        {
            // Display toggle. Get if toggle changed.
            bool change = GUILayout.Toggle(selected == i, items[i]);
            // If changed, set selected to current index.
            if (change)
                selected = i;
        }
        GUILayout.EndVertical();

        // Return the currently selected item's index
        return selected;
    }
}
