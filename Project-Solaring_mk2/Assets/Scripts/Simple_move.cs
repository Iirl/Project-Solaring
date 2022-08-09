using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 自動追蹤系統:
    /// 直線追蹤、定向移動
    /// Put on the object, it will trace the player's position.
    /// </summary>
    public class Simple_move : MonoBehaviour
    {

        #region 面板控制屬性

        #endregion
        [Header("移動的目標"),Tooltip("To get the target the syntax, and auto set the information.")]
        GameObject target;
        private Vector3 target_v3;
        private Vector3 direct;
        [SerializeField, Header("移動速度"), Tooltip("Please test it until you want's speed.")]
        private float Orispeed = 1.2f;
        [SerializeField, Header("停止追蹤距離"),Tooltip("If your Screen size less than 12, recommend to fix it.")]
        private float stopTracert= 12;
        private float dist; 
        [Header("Check The Move System")]
        private bool isEnd;
        /// <summary>
        /// 持續移動方法：依據 direct 的方向移動
        /// </summary>
        private void ContinueMove()
        {
            transform.Translate(-direct* Orispeed * Time.deltaTime, Space.World);
        }
        /// <summary>
        /// 直線移動方法：依據 dist 和 target 的參數決定移動。
        /// * 停止追蹤：設定當 dist 小於一定值之後就不更新 direct 和 target資訊。
        /// </summary>
        private void TransMove()
        {
            transform.LookAt(target.transform.position);
            dist = Vector3.Distance(transform.position, target_v3);
            float speed = Orispeed/ dist * Time.deltaTime;
            if (dist > stopTracert)
            {
                target_v3 = target.transform.position;
                direct = (transform.position - target_v3).normalized;

            }
            transform.position = Vector3.Lerp( transform.position, target_v3, speed);
            if (dist < 1) isEnd = true;
        }
        #region 物件啟動事件
        void Start()
        {
            target = GameObject.FindWithTag("Player");
            //target = GameObject.Find("Comet");
            //target_v3 = target.transform.position;                // 設定目標的座標
            //direct = (transform.position - target_v3).normalized; // 設定目標的方向
        }

        // Update is called once per frame
        void Update()
        {
            if (isEnd) ContinueMove();
            else TransMove();
        }

        private void OnTriggerEnter(Collider other)
        {
            //print("碰到物件");
        }
        #endregion
    }
}