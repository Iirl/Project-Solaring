using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 自定義移動程式的畫面
    /// </summary>
    [CustomEditor(typeof(Simple_move))]
    public class SimpleMoveEditor : Editor
    {
        SerializedProperty spMethod;
        SerializedProperty spStraight;
        SerializedProperty spStopTracert;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            spMethod = serializedObject.FindProperty("moveMethod");
            spStraight = serializedObject.FindProperty("straightV3");
            spStopTracert = serializedObject.FindProperty("stopTracert");

	        if (spMethod.enumValueFlag == 0) EditorGUILayout.PropertyField(spStraight);
            else if (spMethod.enumValueFlag == 1) {}
            else if (spMethod.enumValueFlag == 2) EditorGUILayout.PropertyField(spStopTracert);
            else if (spMethod.enumValueFlag == 3) {}

            serializedObject.ApplyModifiedProperties();
        }
    }
}