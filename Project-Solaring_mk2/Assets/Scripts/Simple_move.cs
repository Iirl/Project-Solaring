using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{
    public class Simple_move : MonoBehaviour
    {

        #region 屬性
        [SerializeField, Header("中控系統")]
        ManageCenter mgCenter;

        #endregion

        private void CircleMove()
        {
            transform.Rotate(Vector3.right * Time.deltaTime);
            transform.position += Vector3.up * Time.deltaTime;
        }

        private void TransMove()
        {
            Vector3 v3 = new Vector3(transform.position.x,transform.position.y,transform.position.z);
            Vector3 w3 = mgCenter.GetStagePOS();
            if (v3.x > w3.x *1.5) v3 -= Vector3.right;
             if (v3.x < w3.x / 2) v3 += Vector3.right;
             if (v3.y > w3.y * 1.5) v3 -= Vector3.up;
             if (v3.y < w3.y / 2) v3 += Vector3.up;
            transform.position = v3;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            TransMove();
        }
    }
}