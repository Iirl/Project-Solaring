using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace solar_a
{
    [CustomEditor(typeof(Rocket_Controll))]
    [CanEditMultipleObjects]
    public class RocketEditor : Editor
    {

        SerializedProperty spHasFire;
        SerializedProperty spParticle;
        SerializedProperty spFireMin;
        SerializedProperty spFireMax;
        SerializedProperty spFireBoost;
        SerializedProperty spFireVolume;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            spHasFire = serializedObject.FindProperty("hasParticleFile");
            spParticle = serializedObject.FindProperty("particle_fire");
            spFireMin = serializedObject.FindProperty("fireLenght_min");
            spFireMax = serializedObject.FindProperty("fireLenght_max");
            spFireBoost = serializedObject.FindProperty("fireBoost");
            spFireVolume = serializedObject.FindProperty("fire_volume");
            if (spHasFire.boolValue)
            {
                EditorGUILayout.PropertyField(spParticle);
                EditorGUILayout.PropertyField(spFireMin);
                EditorGUILayout.PropertyField(spFireMax);
                EditorGUILayout.PropertyField(spFireBoost);
                EditorGUILayout.PropertyField(spFireVolume);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}