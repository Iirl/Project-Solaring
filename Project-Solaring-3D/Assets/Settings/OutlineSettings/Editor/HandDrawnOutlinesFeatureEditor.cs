using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HandDrawnOutlines
{
    [CustomEditor(typeof(HandDrawnOutlinesFeature))]
    public class HandDrawnOutlinesFeatureEditor : Editor
    {
        SerializedObject outlineSettings;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (serializedObject.FindProperty("handDrawnOutlinesSettings").objectReferenceValue)
                outlineSettings = new SerializedObject(serializedObject.FindProperty("handDrawnOutlinesSettings").objectReferenceValue);

            HandDrawnOutlinesFeature myTarget = (HandDrawnOutlinesFeature)target;
            if (outlineSettings.UpdateIfRequiredOrScript()) myTarget.Create();

            //outlineSettings.GetIterator().Next(true);
            //EditorGUILayout.PropertyField(outlineSettings.GetIterator());

            EditorGUILayout.PropertyField(outlineSettings.FindProperty("_Color"));
            EditorGUILayout.PropertyField(outlineSettings.FindProperty("NumberOfContours"));
            EditorGUILayout.PropertyField(outlineSettings.FindProperty("Frequency"));
            EditorGUILayout.PropertyField(outlineSettings.FindProperty("Amplitude"));

            EditorGUILayout.PropertyField(outlineSettings.FindProperty("UseNoise"));
            if (outlineSettings.FindProperty("UseNoise").boolValue)
            {
                EditorGUILayout.PropertyField(outlineSettings.FindProperty("NoiseTex"));
                EditorGUILayout.PropertyField(outlineSettings.FindProperty("NoiseAmount"));
                EditorGUILayout.PropertyField(outlineSettings.FindProperty("NoiseTilling"));
            }

            EditorGUILayout.PropertyField(outlineSettings.FindProperty("TimeMultiplier"));
            EditorGUILayout.PropertyField(outlineSettings.FindProperty("Synchronize"));


            EditorGUILayout.PropertyField(outlineSettings.FindProperty("UseNoiseAlpha"));
            if (outlineSettings.FindProperty("UseNoiseAlpha").boolValue)
            {
                EditorGUILayout.PropertyField(outlineSettings.FindProperty("NoiseAlpha"));
                EditorGUILayout.PropertyField(outlineSettings.FindProperty("NoiseScaleAlpha"));
                EditorGUILayout.PropertyField(outlineSettings.FindProperty("MultiplierAlpha"));

                EditorGUILayout.PropertyField(outlineSettings.FindProperty("UseFlow"));
                if (outlineSettings.FindProperty("UseFlow").boolValue)
                {
                    EditorGUILayout.PropertyField(outlineSettings.FindProperty("SpeedXAlpha"));
                    EditorGUILayout.PropertyField(outlineSettings.FindProperty("SpeedYAlpha"));
                }

                EditorGUILayout.PropertyField(outlineSettings.FindProperty("UseRandomization"));
                if (outlineSettings.FindProperty("UseRandomization").boolValue)
                {
                    EditorGUILayout.PropertyField(outlineSettings.FindProperty("TimeMultiplierAlpha"));
                }
            }

            EditorGUILayout.PropertyField(outlineSettings.FindProperty("Width"));
            EditorGUILayout.PropertyField(outlineSettings.FindProperty("DepthThreshold"));
            EditorGUILayout.PropertyField(outlineSettings.FindProperty("NormalThreshold"));

            outlineSettings.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
        }
    }
}