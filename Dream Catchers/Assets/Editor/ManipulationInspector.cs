/*using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(ManipulationScript))]
public class ManipulationInspector : Editor {

    AnimBool m_ShowAppearenceFields;
    bool appearenceBtn = false;

    AnimBool m_ShowPhysicsFields;
    bool physicsBtn = false;

    bool isPlatform = false;


    ManipulationScript manipScript;

    void OnEnable()
    {
        manipScript = (ManipulationScript)target;

        m_ShowAppearenceFields = new AnimBool(false);
        m_ShowAppearenceFields.valueChanged.AddListener(Repaint);

        m_ShowPhysicsFields = new AnimBool(false);
        m_ShowPhysicsFields.valueChanged.AddListener(Repaint);

        if (manipScript.objectChangeType == ManipulationScript.CHANGE_TYPE.PLATFORM) isPlatform = true;

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

        GUIContent platformLabel = new GUIContent("Make Platform", "Requires Dream Mesh and Dream Collider");
        isPlatform = EditorGUILayout.Toggle(platformLabel, isPlatform);
        if (isPlatform)
        {
            manipScript.objectChangeType = ManipulationScript.CHANGE_TYPE.PLATFORM;
        }
        else 
        {
            manipScript.objectChangeType = ManipulationScript.CHANGE_TYPE.NONE;
        }



        DrawDefaultInspector();


    }
}*/
