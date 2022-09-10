using UnityEditor;

namespace solar_a
{
    [CustomEditor(typeof(EffectPlayer))]
    public class AudioSystemEditor_EffectPlayer : Editor
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
