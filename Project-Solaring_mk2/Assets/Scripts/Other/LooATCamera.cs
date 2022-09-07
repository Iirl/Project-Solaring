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
        target = Camera.main.transform;
    }

    private void Update()
    {
        if (target != null)
        {
            transform.LookAt(target);
            transform.Rotate(angle);
        } else
        {
            try
            {
                target = Camera.main.transform;
            }
            catch (System.Exception)
            {
                print("攝影機消失，或不存在!");
            }
        }
    }
}
