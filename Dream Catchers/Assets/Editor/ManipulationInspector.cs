using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(ManipulationScript))]
public class ManipulationInspector : Editor {

    AnimBool m_ShowAppearenceFields;
    bool appearenceBtn = false;
    ManipulationScript manipScript;

    void OnEnable()
    {
        manipScript = (ManipulationScript)target;

        m_ShowAppearenceFields = new AnimBool(false);
        m_ShowAppearenceFields.valueChanged.AddListener(Repaint);
    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.LabelField ("Current State:     " + manipScript.currentObjectState.ToString());

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

            EditorGUILayout.PrefixLabel("Texture");
            EditorGUI.indentLevel++;
            manipScript.dreamTexture = (Texture)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Dream", manipScript.dreamTexture, typeof(Texture), false);
            manipScript.nightmareTexture = (Texture)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Nightmare", manipScript.nightmareTexture, typeof(Texture), false);
            EditorGUI.indentLevel--;

            EditorGUILayout.PrefixLabel("Mesh");
            EditorGUI.indentLevel++;
            manipScript.dreamMesh = (Mesh)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Dream", manipScript.dreamMesh, typeof(Mesh), false);
            manipScript.nightmareMesh = (Mesh)EditorGUI.ObjectField(GUILayoutUtility.GetRect(15, 15, "TextField"), "Nightmare", manipScript.nightmareMesh, typeof(Mesh), false);
            EditorGUI.indentLevel--;


            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFadeGroup();

        DrawDefaultInspector();


    }
}
