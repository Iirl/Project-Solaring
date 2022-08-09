using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// �۰ʰl�ܨt��:
    /// ���u�l�ܡB�w�V����
    /// Put on the object, it will trace the player's position.
    /// </summary>
    public class Simple_move : MonoBehaviour
    {

        #region ���O�����ݩ�

        #endregion
        [Header("���ʪ��ؼ�"),Tooltip("To get the target the syntax, and auto set the information.")]
        GameObject target;
        private Vector3 target_v3;
        private Vector3 direct;
        [SerializeField, Header("���ʳt��"), Tooltip("Please test it until you want's speed.")]
        private float Orispeed = 1.2f;
        [SerializeField, Header("����l�ܶZ��"),Tooltip("If your Screen size less than 12, recommend to fix it.")]
        private float stopTracert= 12;
        private float dist; 
        [Header("Check The Move System")]
        private bool isEnd;
        /// <summary>
        /// ���򲾰ʤ�k�G�̾� direct ����V����
        /// </summary>
        private void ContinueMove()
        {
            transform.Translate(-direct* Orispeed * Time.deltaTime, Space.World);
        }
        /// <summary>
        /// ���u���ʤ�k�G�̾� dist �M target ���ѼƨM�w���ʡC
        /// * ����l�ܡG�]�w�� dist �p��@�w�Ȥ���N����s direct �M target��T�C
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
        #region ����Ұʨƥ�
        void Start()
        {
            target = GameObject.FindWithTag("Player");
            //target = GameObject.Find("Comet");
            //target_v3 = target.transform.position;                // �]�w�ؼЪ��y��
            //direct = (transform.position - target_v3).normalized; // �]�w�ؼЪ���V
        }

        // Update is called once per frame
        void Update()
        {
            if (isEnd) ContinueMove();
            else TransMove();
        }

        private void OnTriggerEnter(Collider other)
        {
            //print("�I�쪫��");
        }
        #endregion
    }
}