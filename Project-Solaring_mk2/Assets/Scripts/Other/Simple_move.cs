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
        enum MoveMethod { Straight, Track, Direction, Hold }
        #region ���O�����ݩ�

        #endregion
        [Header("���ʪ��ؼ�"), Tooltip("To get the target the syntax, and auto set the information.")]
        GameObject target;
        private Vector3 target_v3;
        private Vector3 direct;
        /// <summary>
        /// �b���O��ܳ]�w
        /// </summary>
        [SerializeField, Header("���ʤ覡")]
        private MoveMethod moveMethod;
        [SerializeField, Header("���u���ʤ�V")]
        private Vector3 straightV3;
        [SerializeField, Header("���ʳt��"), Tooltip("Please test it until you want's speed.")]
        private float Orispeed = 1.2f;
        [SerializeField, Tooltip("Set random speed to set every object have different speed.")]
        private bool randomSpd = true;
        [SerializeField, Header("����l�ܶZ��"), Tooltip("If your Screen size less than 12, recommend to fix it.")]
        private float stopTracert = 12;
        private float dist;
        //
        private AudioSource[] audios;

        /// <summary>
        /// ���򲾰ʤ�k�G�̾� direct ����V����
        /// </summary>
        private void ContinueMove() => transform.Translate(-direct * Orispeed * Time.deltaTime, Space.World);
        /// <summary>
        /// ���u���ʤ�k�G�b���O���]�w���ʤ�V
        /// </summary>
        private void StraightMove() => transform.Translate(straightV3* Orispeed * Time.deltaTime, Space.World);
        /// <summary>
        /// ���u���ʤ�k�G�̾� dist �M target ���ѼƨM�w���ʡC
        /// * ����l�ܡG�]�w�� dist �p��@�w�Ȥ���N����s direct �M target��T�C
        /// </summary>
        private void TransMove()
        {
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
        #region ����Ұʨƥ�
        public IEnumerator Mute(bool isMute = true)
        {
            if (audios.Length > 0)
            {
                foreach (AudioSource audio in audios) if (isMute) audio.Stop(); else audio.Play();
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
            target_v3 = target ? target.transform.position: Vector3.zero;                // �]�w�ؼЪ��y��
            direct = (transform.position - target_v3).normalized; // �]�w�ؼЪ���V
            if (randomSpd) Orispeed = Random.Range(Orispeed, Orispeed*2);
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
            //print("�I�쪫��");
            if (other.tag.Contains("Player")) enabled = false;
        }
        #endregion
    }
}