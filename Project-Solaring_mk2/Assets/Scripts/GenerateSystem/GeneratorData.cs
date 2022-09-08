using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName ="New Object",menuName = "SolarPrject/Object Generator")]
public class GeneratorData :ScriptableObject
{
    [SerializeField, Header("產生器類別")]
    public GenerClass grtClass;
    [SerializeField, Header("產生物件基本設定")]
    public GameObject grtObject;
    [SerializeField, Tooltip("指定物件生成數量上限")]
    public int grtLimit = 10;
    [SerializeField, Header("時間設定"),Tooltip("延遲時間")]
    public float grtIntervalTime = 1f;
    [SerializeField, Tooltip("等待時間")]
    public float grtWaitTime = 5f;
    [SerializeField, Tooltip("自動消滅時間")]
    public float grtdestTime = 5f;
    //
    [SerializeField, Header("指定物件範圍"), Tooltip("若為定點則指定位置"), HideInInspector]
    public Vector3 grtPos = Vector3.zero;
    [SerializeField, Tooltip("生成距離調整"), Range(0, 100), HideInInspector]
    public float grtOffset;
    [SerializeField, Header("隨機產生子物件"), HideInInspector]
    public List<UnityEngine.Object> grtSubObject;
    [SerializeField, Tooltip("Create SubObject probablity"), Range(0, 1), HideInInspector]
    public float grtProb = 0.5f;
    [SerializeField, Header("是否隨機旋轉"), HideInInspector]
    public bool grtRandomRoation;
    [SerializeField, Tooltip("指定物件旋轉"), HideInInspector]
    public Quaternion grtRot = Quaternion.identity;

}

#if UNITY_EDITOR
[CustomEditor(typeof(GeneratorData))]
public class GenerateEditor :Editor
{
    SerializedProperty spClass;
    SerializedProperty spSubject;
    SerializedProperty spSubjectProb;
    SerializedProperty spPos;
    SerializedProperty spPosOffset;
    SerializedProperty spIsRandomRot;
    SerializedProperty spRandomRot;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();

        spClass = serializedObject.FindProperty("grtClass");
        spSubject = serializedObject.FindProperty("grtSubObject");
        spSubjectProb = serializedObject.FindProperty("grtProb");
        spPos = serializedObject.FindProperty("grtPos");
        spPosOffset = serializedObject.FindProperty("grtOffset");
        spIsRandomRot = serializedObject.FindProperty("grtRandomRoation");
        spRandomRot = serializedObject.FindProperty("grtRot");

        int i = spClass.enumValueFlag;
        bool rot = spIsRandomRot.boolValue;
        // 選轉開關
        EditorGUILayout.PropertyField(spIsRandomRot);
        if (!rot) EditorGUILayout.PropertyField(spRandomRot);
        if (i != 3) //生成位置設定
        {
            EditorGUILayout.PropertyField(spPos);
            EditorGUILayout.PropertyField(spPosOffset);

        }
        if (i == 1) //生成子物件設定
        {
            EditorGUILayout.PropertyField(spSubject);
            EditorGUILayout.PropertyField(spSubjectProb);
        } 

        serializedObject.ApplyModifiedProperties();
    }

}
#endif