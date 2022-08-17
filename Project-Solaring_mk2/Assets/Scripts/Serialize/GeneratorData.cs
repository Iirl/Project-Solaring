using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Object",menuName ="Objector/Object Generator")]
public class GeneratorData :ScriptableObject
{
    [SerializeField, Header("���;����O")]
    public GenerClass grtClass;
    [SerializeField, Header("���ͪ���")]
    public GameObject grtObject;
    [SerializeField, Header("�����O��Meteo�ɲ��ͤl����")]
    public List<Object> grtSubObject;
    [SerializeField, Header("����ɶ�")]
    public float grtIntervalTime =1f;
    [SerializeField, Tooltip("���ݮɶ�")]
    public float grtWaitTime = 5f;
    [SerializeField, Header("���w����d��")]
    public Vector3 grtPos = Vector3.zero;
    [SerializeField, Tooltip("���w�������")]
    public Quaternion grtRot = Quaternion.identity;
    [SerializeField, Tooltip("Create Probable"),Range(0,1)]
    public float grtProb =0.5f;
    [SerializeField, Header("���w����ͦ��ƶq�W��")]
    public int grtLimit = 10;
}
