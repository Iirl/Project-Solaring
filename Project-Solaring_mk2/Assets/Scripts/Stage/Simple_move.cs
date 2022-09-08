using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 自動追蹤系統:
    /// 直線追蹤、定向移動。
    /// Put on the object, it will trace the player's position.
    /// </summary>
    public class Simple_move : MonoBehaviour
    {
        public enum MoveMethod { Straight, Track, Direction, Hold }
        #region 面板控制屬性

        #endregion
        [Header("移動的目標"), Tooltip("To get the target the syntax, and auto set the information.")]
        GameObject target;
        private Vector3 target_v3;
        private Vector3 direct;
        /// <summary>
        /// 在面板顯示設定
        /// </summary>
        [SerializeField, Header("移動方式")]
        public MoveMethod moveMethod;
        [SerializeField, Header("移動速度"), Tooltip("Please test it until you want's speed.")]
        protected float Orispeed = 1.2f;
        [SerializeField, Tooltip("Set random speed to set every object have different speed.")]
        protected bool randomSpd = true;
        [SerializeField, Header("直線移動方向"),HideInInspector]
        protected Vector3 straightV3;
        [SerializeField, Header("停止追蹤距離"), Tooltip("If your Screen size less than 12, recommend to fix it."), HideInInspector]
        protected float stopTracert = 12;
        private float dist;
        //
        private AudioSource[] audios;

        /// <summary>
        /// 持續移動方法：依據 direct 的方向移動
        /// </summary>
        private void ContinueMove() => transform.Translate(-direct * Orispeed * Time.deltaTime, Space.World);
        /// <summary>
        /// 直線移動方法：在面板中設定移動方向
        /// </summary>
        private void StraightMove() => transform.Translate(straightV3 * Orispeed * Time.deltaTime, Space.World);
        /// <summary>
        /// 直線移動方法：依據 dist 和 target 的參數決定移動。
        /// * 停止追蹤：設定當 dist 小於一定值之後就不更新 direct 和 target資訊。
        /// </summary>
        private void TransMove()
        {
            if (target == null) target = GameObject.FindWithTag("Player");
            transform.LookAt(target.transform.position);
            dist = Vector3.Distance(transform.position, target_v3);
            float speed = Orispeed / dist * Time.deltaTime;
            if (dist > stopTracert)
            {
                target_v3 = target.transform.position;
                direct = (transform.position - target_v3).normalized;

            }
            transform.position = Vector3.Lerp(transform.position, target_v3, speed);
            if (dist < 1) moveMethod = MoveMethod.Direction;
        }
        #region 物件啟動事件
        /// <summary>
        /// 將物件的聲音執行或停止
        /// </summary>
        /// <param name="isStop">是否停止</param>
        /// <returns></returns>
        public IEnumerator Mute(bool isStop = true)
        {
            if (audios.Length > 0)
            {
                foreach (AudioSource audio in audios) if (isStop) audio.Stop(); else audio.Play();
            }
            yield return null;
        }

        private void Awake()
        {
            try
            {
                audios = GetComponents<AudioSource>();
                target = GameObject.FindWithTag("Player");

            }
            catch (System.Exception) { }
        }
        void Start()
        {
            target_v3 = target ? target.transform.position : straightV3;                // 設定目標的座標
            direct = (transform.position - target_v3).normalized; // 設定目標的方向
            if (randomSpd) Orispeed = Random.Range(Orispeed, Orispeed * 2);
        }

        // Update is called once per frame
        void Update()
        {
            switch (moveMethod)
            {
                case MoveMethod.Straight:
                    StraightMove();
                    break;
                case MoveMethod.Track:
                    TransMove();
                    break;
                case MoveMethod.Direction:
                    ContinueMove();
                    break;
                case MoveMethod.Hold:
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //print("碰到物件");
            if (other.tag.Contains("Player")) if (!ManageCenter.space_ctl.isRotate) enabled = false;
        }
        #endregion


    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Simple_move))]
    public class ListMoveEditor : Editor
    {
        SerializedProperty spMethod;
        SerializedProperty spStraight;
        SerializedProperty spStopTracert;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            spMethod = serializedObject.FindProperty("moveMethod");
            spStraight = serializedObject.FindProperty("straightV3");
            spStopTracert = serializedObject.FindProperty("stopTracert");

            if (spMethod.enumValueFlag == 0)
            {
                EditorGUILayout.PropertyField(spStraight);

            }
            else if (spMethod.enumValueFlag == 1)
            {

            }
            else if (spMethod.enumValueFlag == 2)
            {
                EditorGUILayout.PropertyField(spStopTracert);
            }
            else if (spMethod.enumValueFlag == 3)
            {

            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}