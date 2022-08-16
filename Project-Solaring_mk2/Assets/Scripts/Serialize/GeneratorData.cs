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
    [SerializeField, Header("���w���ͶZ��")]
    public float grtDistance;
    [SerializeField, Tooltip("���w����d��")]
    public Vector3 grtPos = Vector3.zero;
    [SerializeField, Tooltip("���w�������")]
    public Quaternion grtRot = Quaternion.identity;
    [SerializeField, Header("���w����ͦ��ƶq�W��")]
    public int grtLimit = 10;
}
