using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName ="New Object",menuName ="Objector/Object Generator")]
public class GeneratorData :ScriptableObject
{
    [SerializeField, Header("產生器類別")]
    public GenerClass grtClass;
    [SerializeField, Header("產生物件")]
    public GameObject grtObject;
    [SerializeField, Header("當類別為Meteo時產生子物件")]
    public List<UnityEngine.Object> grtSubObject;
    [SerializeField, Tooltip("Create SubObject probablity"), Range(0, 1)]
    public float grtProb = 0.5f;
    [SerializeField, Header("延遲時間")]
    public float grtIntervalTime =1f;
    [SerializeField, Tooltip("等待時間")]
    public float grtWaitTime = 5f;
    [SerializeField, Tooltip("自動消滅時間")]
    public float grtdestTime = 5f;
    [SerializeField, Header("指定物件範圍")]
    public Vector3 grtPos = Vector3.zero;
    [SerializeField, Tooltip("生成距離調整"), Range(0,100)]
    public float grtOffset;
    [SerializeField, Tooltip("是否隨機旋轉")]
    public bool grtRandomRoation;
    [SerializeField, Tooltip("指定物件旋轉")]
    public Quaternion grtRot = Quaternion.identity;
    [SerializeField, Header("指定物件生成數量上限")]
    public int grtLimit = 10;    

}
