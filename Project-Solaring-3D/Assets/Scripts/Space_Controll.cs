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
        [SerializeField, Tooltip("����t��"), Range(0f, 10f)]
        float spine = 2.0f;
        #endregion

        #region ���
        float Coordinate;
        #endregion

        #region 90�שT�w����Ŷ�
        private void DirectCheck()
        {

        }
        private void PositionCheck(int direct)
        {
            float y_axis = Mathf.Floor(transform.eulerAngles.y); // Y�b�b�ߦ�m
            int quadrant = Int32.Parse( y_axis.ToString()) / 90;  //�Ҧb�H��
            float next_axis = (direct == 1)? 90 * (1 + quadrant): 90 * (quadrant); // �U�@��Y�b�y��
            float Distane2axis = (Mathf.DeltaAngle(y_axis, next_axis)); // ��F�U�@��Y�b�y�Ъ��Z��

            //print($"Now:{y_axis}, quadrant:{quadrant}");
            //print($"Next Y:{next_axis}, dist:{Distane2axis}");
        }
        private void Spine()
        {
            float y_axis = Mathf.Floor(transform.eulerAngles.y); // Y�b�b�ߦ�m
            float iSpine = Mathf.Pow(spine, Coordinate / y_axis);
            transform.Rotate(Vector2.up * iSpine);
            if (y_axis == Coordinate)
            {
                rotated = false;
                CancelInvoke("Spine");
            }
        }
        private void StopCheck()
        {

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
            if (Input.GetAxisRaw("Left_Spine") == 1) { rot_left = true; }
            else if (Input.GetAxisRaw("Right_Spine") == 1) { rot_right = true; }
            PositionCheck(-1);

        }
        #endregion
    }
}


#region ���O
/*
    �j��y�{�G
    1. ����P�w�G���B�k�䰻��
        ������w���c��G���U�����A�ȥu�O�Ұ���V�P�w�A�u��������઺�P�w���ӭn�b��m�P�w����A�~���|����v�T����C

    2. ��m�P�w�G�ثe�b���Ӯy�ФW

    3. ����P�w�G�@���}�l����A�S����ؼ��I���|��
        �����k�G
        �o�����������઺�Ҧ��A�ܩ�ƻ�ɭԱ���A�ƻ�ɭ԰���h���A�o�ӵ{�����M�w�C
        �ݭn�Ψ쪺�ܼƥD�n�� Y�b�ثe����m �P �U�@�Ӯy���I����m�C
        �ר�U�@�Ӯy�Ц�m�����H�{��������ܰʡA���M�~���F�N����U�@�Ӯy�СA�N�|�ण���C

    4. �w�I�P�w�G����O�_��F�w�I�A��F�ᵲ���{��
 


//// �U���O�쥻���g�k�A�Q�n�g�����α���A���S�����\�C
        #region �Ŷ���k(�˱�)
        [SerializeField, Tooltip("����V����(L)"), Range(5f, 90f)]
        private float Left_angle = 45;
        [SerializeField, Tooltip("�k��V����(R)"), Range(0f, 90f)]
        private float Right_angle = 45;
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
*/
#endregion
