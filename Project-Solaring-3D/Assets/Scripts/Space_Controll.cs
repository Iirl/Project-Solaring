using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// ���{����b����s�շ��A�Ψӽվ㪫��Ŷ������ʡC
/// </summary>
namespace solar_a
{
    public class Space_Controll : MonoBehaviour
    {
        #region �ܼ�
        static bool rotated, rot_left, rot_right;  //�Ŷ��O�_����
        [SerializeField, Header("����]�w"), Tooltip("���I�G���D���n�A�_�h���קK�]�w�p���I�C\n" +
            "�p�󪽨��d�򪺸ܥ��k��V�쩳�|����F�j�󪽨��d��h�^�Φ��@�Ӱj��A�ثe�y�L�������ӬO�S���D�A" +
            "�����T�w�O�_�Y�Ǽƭȷ|��BUG�A���ԷV�]�w�C")]
        private float rot_angle = 0;
        [SerializeField, Tooltip("����V����(L)"), Range(5f, 90f)]
        private float Left_angle = 45;
        [SerializeField, Tooltip("�k��V����(R)"), Range(0f, 90f)]
        private float Right_angle = 45;
        [SerializeField, Tooltip("����t��"), Range(0f, 10f)]
        float spine = 2.0f;
        [SerializeField, Tooltip("�������")]
        float spine_log = 10;
        #endregion

        #region ���

        #endregion

        #region �Ŷ���k
        /// <summary>
        ///  �����ӪŶ� +-X��
        /// </summary>
        /// �Ѽƽбq�ݩʭ��O�վ�A����V
        private void _Spine()
        {
            float center_loc = Mathf.Floor(transform.eulerAngles.y); // Y�b�b�ߦ�m
            float left_loc = rot_angle > Left_angle ? rot_angle - Left_angle : 360 + rot_angle - Left_angle; // Y�b�f�ۦ�m
            float right_loc = rot_angle + Right_angle < 360 ? rot_angle + Right_angle : 360 - rot_angle + Right_angle; // Y�b���ۦ�m
            // �P�_�O�_�������
            if (center_loc >= right_loc && center_loc < 180) rot_right = false;
            if (center_loc <= left_loc && center_loc > 180) rot_left = false;
            if (rotated)  FadeSpine(center_loc, left_loc, right_loc);
        }
        private void _Spine(int vect)
        {
            // 1 �k��  -1 ����
            if (vect != 0)
            {
                rotated = true;
                if (vect == 1)
                {
                    rot_right = true;
                    rot_left = false;
                }
                else if (vect == -1)
                {
                    rot_left = true;
                    rot_right = false;
                }
            }
        }
        /// <summary>
        /// ���ܳt�פ�k
        /// </summary>
        /// <param name="c">�ثe�b�ߪ�����</param>
        /// <param name="l">����ɨ���</param>
        /// <param name="r">�k��ɨ���</param>
        private void FadeSpine(float c, float l, float r)
        {
            float r_angle = (Mathf.DeltaAngle(c, r));
            float l_angle = (Mathf.DeltaAngle(c, l));
            float i_spine = spine, i = 1;
            float pos = l_angle + r_angle; // ���o�ثe��m
            int r_spine = PosFix(pos, l, r);
            switch (r_spine)
            {
                case 0: i = pos; break;
                case 2: i = r_angle; break;
                case -2: i = l_angle; break;
                default: break;
            }
            /// ����t�׽վ�G
            /// �ثe�ϥ�Log��ƽվ���t�A�Y�n���ܲ��ʦ��u�Цb���ץ�
            i_spine *= Mathf.Log(Mathf.Abs(i + 1), spine_log);
            /// ----
            if (i_spine <= 0) i_spine = 1f;
            if (rot_left) transform.Rotate(Vector2.down * i_spine);
            else if (rot_right) transform.Rotate(Vector2.up * i_spine);
            //print(r_angle);
            //print($"POS:{pos}, spine:{i_spine}"); // �ˬd��m�A��t
            //print($"{rotated} L{rot_left} R{rot_right}"); // �ˬd����}��

        }
        /// <summary>
        /// �ΨӮե����ʮy�СA�аѾ\_Spine()���
        /// </summary>
        /// <param name="p">�ثe��m</param>
        /// <param name="l">���ۦ�m</param>
        /// <param name="r">�k�ۦ�m</param>
        private int PosFix(float p, float l, float r)
        {
            p = Mathf.Floor(p);
            int n = 0;
            if (p >= -spine&& p <= spine)
            {
                rotated = false;
                n = 0;
            }
            else if (p >= Left_angle + Right_angle) // + 
            {
                //print("����ɽu");
                rotated = false;
                n = -1;
            }
            else if (p <= -Left_angle - Right_angle) // -
            {
                //print("�k��ɽu");
                rotated = false;
                n = 1;
            }
            else if (p > 0 && p < Left_angle) // Left spinning
            {
                n = -2;
            }
            else if (p < 0 && 360 + p > Right_angle) // Right spinning
            {
                n = 2;
            }

            return n;
        }
        #endregion

        #region �ƥ�Ĳ�o

        private void Awake()
        {
            rotated = false; rot_left = false; rot_right = false;
            //���z�Ŷ��Ѽ�
            Physics.gravity = new Vector3(0, -0.1F, 0);
            transform.Rotate(new Vector3(0, rot_angle, 0));
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {

            // ����Ŷ�
            if (Input.GetAxisRaw("Left_Spine") == 1) _Spine(-1);
            else if (Input.GetAxisRaw("Right_Spine") == 1) _Spine(1);
            _Spine();

        }
        #endregion
    }
}