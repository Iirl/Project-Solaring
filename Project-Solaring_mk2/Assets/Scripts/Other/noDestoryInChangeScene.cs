using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noDestoryInChangeScene : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
