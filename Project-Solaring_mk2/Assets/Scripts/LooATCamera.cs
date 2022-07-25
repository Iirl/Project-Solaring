using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooATCamera : MonoBehaviour
{
    [SerializeField, Header("固定物件角度")]
    Vector3 angle = new(90, 45, 0);
    Transform target;


    private void Awake()
    {
        target = GameObject.Find("Main Camera").transform;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target);
            transform.Rotate(angle);
        }
    }
}
