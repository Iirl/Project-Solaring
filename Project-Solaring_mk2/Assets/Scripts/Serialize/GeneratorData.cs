using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Object",menuName ="Objector/Object Generator")]
public class GeneratorData :ScriptableObject
{
    [SerializeField, Header("產生器類別")]
    GenerClass generClass;
    [SerializeField, Header("產生物件")]
    private GameObject generObject;
    [SerializeField, Header("指定產生距離")]
    public float generDistance;
}
