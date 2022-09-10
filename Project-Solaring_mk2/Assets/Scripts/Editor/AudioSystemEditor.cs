using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace solar_a
{
    [CustomEditor(typeof(AudioSystem))]
    public class AudioSystemEditor : Editor
    {
        SerializedProperty spIsLoop;
        SerializedProperty spPTimes;
        SerializedProperty spWTime;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            spIsLoop = serializedObject.FindProperty("playLoop");
            spPTimes = serializedObject.FindProperty("playTimes");
            spWTime = serializedObject.FindProperty("waitTime");

            if (spIsLoop.boolValue)
            {
                EditorGUILayout.PropertyField(spPTimes);
                EditorGUILayout.PropertyField(spWTime);
            }
            serializedObject.ApplyModifiedProperties();

        }
    }

}
