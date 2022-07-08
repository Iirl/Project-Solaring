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
        [SerializeField, Header("����]�w"), Tooltip("���I")]
        private float rot_angle = 0;
        [SerializeField, Tooltip("����V����(L)"), Range(5f, 90f)]
        private float Left_angle = 45;
        [SerializeField, Tooltip("�k��V����(R)"), Range(0f, 90f)]
        private float Right_angle = 45;
        [SerializeField, Tooltip("����t��"), Range(0f, 10f)]
        float spine = 2.0f; //����t��
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

            if (rotated)
            {
                FadeSpine(center_loc, left_loc, right_loc);
                if (rot_left) { }//transform.Rotate(eulers: Vector2.down * spine);
                else if (rot_right) { } //transform.Rotate(eulers: Vector2.up * spine);
                                        // ���n�I�A����׼Ʒ����M�k��׼Ʒ����έ��I�C
                //if (center_loc == rot_angle) rotated = false;
                //else if (center_loc > left_loc - spine && center_loc < left_loc + spine) { rotated = false; }
                //else if (center_loc > right_loc - spine && center_loc < right_loc + spine) { rotated = false; }

            }
            //print($"Name:= {y_loc}  / rot{rot_angle}");
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

        private void FadeSpine(float c, float l, float r)
        {
            float r_angle = (Mathf.DeltaAngle(c, r));
            float l_angle = (Mathf.DeltaAngle(c, l));
            float i_spine = spine;
            float pos = l_angle + r_angle; // ���o�ثe��m
            if (pos == rot_angle)
            {
                rotated = false;
            } else if (pos > 0 && pos < Left_angle)
            {

            } else if (pos < 0 && 360 + pos > Right_angle)
            {
            }
            else if (pos >= Left_angle + Right_angle) // +
            {
                print("����");
                rotated = false;
            }
            else if (pos <= -Left_angle - Right_angle)
            { // -
                print("�k��");
                rotated = false;
            }
            //print(-l_angle - r_angle);
            //print(pos);
            //print(Mathf.Pow(Left_angle-l_angle, 1));
            //i_spine = spine * (l_angle - 1) / Left_angle;
            //i_spine = spine * (r_angle + 1) / Right_angle;
            print($"{rotated} L{rot_left} R{rot_right}");

            if (Mathf.Abs(i_spine) < 0.5f) i_spine = 0.5f;

            if (rot_left)
            {
                transform.Rotate(Vector2.down * i_spine);
            }

            else if (rot_right)
            {
                transform.Rotate(Vector2.up * i_spine);
            }

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