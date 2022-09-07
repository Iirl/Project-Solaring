using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooATCamera : MonoBehaviour
{
    [SerializeField, Header("�T�w���󨤫�")]
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
                print("��v�������A�Τ��s�b!");
            }
        }
    }
}
