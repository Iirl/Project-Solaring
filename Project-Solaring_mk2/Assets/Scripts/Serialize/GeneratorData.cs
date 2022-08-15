using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Object",menuName ="Objector/Object Generator")]
public class GeneratorData :ScriptableObject
{
    [SerializeField, Header("產生器類別")]
    public GenerClass grtClass;
    [SerializeField, Header("產生物件")]
    public GameObject grtObject;
    [SerializeField, Header("指定產生距離")]
    public float grtDistance;
    [SerializeField, Tooltip("指定物件範圍")]
    public Vector3 grtPos = Vector3.zero;
    [SerializeField, Tooltip("指定物件旋轉")]
    public Quaternion grtRot = Quaternion.identity;
    [SerializeField, Header("指定物件生成數量上限")]
    public int grtLimit = 10;
}
