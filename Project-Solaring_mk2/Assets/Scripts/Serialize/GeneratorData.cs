using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Object",menuName ="Objector/Object Generator")]
public class GeneratorData :ScriptableObject
{
    [SerializeField, Header("���;����O")]
    GenerClass generClass;
    [SerializeField, Header("���ͪ���")]
    private GameObject generObject;
    [SerializeField, Header("���w���ͶZ��")]
    public float generDistance;
}
