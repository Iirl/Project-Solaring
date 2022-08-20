using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName ="New Object",menuName ="Objector/Object Generator")]
public class GeneratorData :ScriptableObject
{
    [SerializeField, Header("���;����O")]
    public GenerClass grtClass;
    [SerializeField, Header("���ͪ���")]
    public GameObject grtObject;
    [SerializeField, Header("�����O��Meteo�ɲ��ͤl����")]
    public List<UnityEngine.Object> grtSubObject;
    [SerializeField, Tooltip("Create SubObject probablity"), Range(0, 1)]
    public float grtProb = 0.5f;
    [SerializeField, Header("����ɶ�")]
    public float grtIntervalTime =1f;
    [SerializeField, Tooltip("���ݮɶ�")]
    public float grtWaitTime = 5f;
    [SerializeField, Tooltip("�۰ʮ����ɶ�")]
    public float grtdestTime = 5f;
    [SerializeField, Header("���w����d��")]
    public Vector3 grtPos = Vector3.zero;
    [SerializeField, Tooltip("�ͦ��Z���վ�"), Range(0,100)]
    public float grtOffset;
    [SerializeField, Tooltip("�O�_�H������")]
    public bool grtRandomRoation;
    [SerializeField, Tooltip("���w�������")]
    public Quaternion grtRot = Quaternion.identity;
    [SerializeField, Header("���w����ͦ��ƶq�W��")]
    public int grtLimit = 10;    

}
