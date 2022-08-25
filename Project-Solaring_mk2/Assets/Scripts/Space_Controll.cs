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

        [SerializeField, Header("����]�w")]
        private Vector3 spaceOffset;
        [SerializeField, Tooltip("����t��"), Range(0f, 50f)]
        private float spine = 2.0f;
        #region ���
        private bool rot_left, rot_right, rotate;  //�Ŷ��O�_����
        private float Coordinate = 360;
        #endregion
        private SpacetState spstate;
        public bool isRotate { get { return spstate == SpacetState.Rotate; } }
        private void SPNext() => spstate = spstate < SpacetState.Rotate? spstate+1: 0;
        #region 90�שT�w����Ŷ�
        /// <summary>
        /// 1. ��V(��V)�P�w
        /// </summary>
        private void DirectCheck(bool rL, bool rR)
        {
            if (spstate == 0)
            {
                rot_right = rR;
                rot_left = rL;
                //print("�ˬd�O�_����");
                if ((rot_right || rot_left) && !rotate)
                {
                    SPNext();
                    Coordinate = PositionCheck(rot_left);
                    //print("1: "+spstate);
                }
            }
            else print("�٦b����");
        }
        /// <summary>
        /// 2. ��m�P�w
        /// </summary>
        /// <param name="direct">��V���ޡA�Y���u�h+90�סF���h�O-90��</param>
        /// <returns></returns>
        private float PositionCheck(bool direct)
        {
            if (spstate == SpacetState.Setting )
            {
                rotate = true;
                //print("���o�ؼЮy��");
                float y_axis = Mathf.Floor(transform.eulerAngles.y); // �ثeY�b�b�ߦ�m
                int quadrant = (Int32.Parse(y_axis.ToString()) / 90);  //�Ҧb�H��
                if (quadrant == 0 && !direct) quadrant = 4;
                float next_axis = (direct) ? 90 * (quadrant + 1) : 90 * (quadrant - 1); // �U�@��Y�b�y��
                if (quadrant == 3 && direct) next_axis %= 360;
                //print($"��e�H��:{quadrant}, �U�@�Ӯy��{next_axis}");
                //print($"Y_Now: {y_axis}");
                SPNext();
                //print($"���o�᪬�A{spstate}");
                StartCoroutine(Spine());
                return Mathf.Round(next_axis);
            }
            //print("2: "+spstate);
            return 0;
        }
        /// <summary>
        /// 3. ����{���A�ĽեΪk�I�s�{���C
        /// </summary>
        private IEnumerator Spine()
        {            
            //print($"��eY�b:�ؼжb {transform.eulerAngles.y}:{Coordinate}");
            //print($"{Distane2axis}");
            //print("4:" + spstate);
            float execute = 0;
            yield return new WaitForSeconds(0.1f);
            while (isRotate)
            {
                float y_axis = Mathf.Floor(transform.eulerAngles.y); // �ثeY�b�b�ߦ�m
                float Distane2axis = Mathf.Abs(Mathf.DeltaAngle(y_axis, Coordinate)); // ��F�U�@��Y�b�y�Ъ��Z��            
                Quaternion target = Quaternion.Euler(0, Coordinate, 0);
                //Distane2axis = Mathf.Clamp(Distane2axis, 0, 90);
                StopCheck();
                // ���঱�u: x^0-1
                float upper = (Mathf.Abs(Distane2axis)) / 90;
                float iSpine = Mathf.Pow(spine, upper) * Time.deltaTime +1;
                // ������k
                if (execute > 300) transform.rotation = target;
                else if (Distane2axis < 5 ) iSpine /= 4f;
                else if (Distane2axis < 10) iSpine /= 2f;
                iSpine = Mathf.Clamp(iSpine, 0.005f, 0.1f);
                // �������禡                
                transform.rotation = Quaternion.Lerp(transform.rotation, target, iSpine);
                //if (rot_left) transform.Rotate(Vector3.up * iSpine);
                //else if (rot_right) //transform.Rotate(Vector3.down * iSpine);
                //if (iSpine == Mathf.Infinity) print(upper);
                yield return null;
                execute++;
                
            }
        }
        /// <summary>
        /// 4. ����P�w�A�ĩw�I�P�_�A�Y�L�k�ǽT��w�I�h�i��L������C
        /// </summary>
        private void StopCheck()
        {
            //print("4:" + spstate);
            float y_axis = Mathf.Floor(transform.eulerAngles.y); // �ثeY�b�b�ߦ�m
            bool stop = !isRotate;
            y_axis = (y_axis == -1) ? 0 : y_axis; 
            y_axis = (y_axis == 360) ? 0 : y_axis;
            switch (Mathf.Floor(y_axis))
            {
                case 0: stop = true; break;
                case 90: stop = true; break;
                case 180: stop = true; break;
                case 270: stop = true; break;
                default: stop = false; break;
            }

            if (stop && y_axis == Coordinate)
            {
                rot_right = false;
                rot_left = false;
                rotate = false;
                SPNext();
                //CancelInvoke("Spine");
            }

        }

        #endregion

        #region �ƥ�Ĳ�o

        private void Start()
        {
            rot_left = false; rot_right = false;
            transform.position += spaceOffset;
            spstate = 0;
        }

        private void FixedUpdate()
        {
            switch (StaticSharp.Conditions)
            {
                case State.Running:
                    // ����Ŷ�
                    //print(spstate);
                    bool keyLeft = Input.GetAxisRaw("Left_Spine") == 1;
                    bool keyRight = Input.GetAxisRaw("Right_Spine") == 1;
                    switch (spstate)
                    {
                        case SpacetState.Stay:
                            if (keyLeft || keyRight) DirectCheck(keyLeft, keyRight);
                            break;
                        case SpacetState.Rotate:

                            break;
                        case SpacetState.Stop:
                            break;
                        default:
                            break;
                    }
                    break;
                case State.Pause:
                    break;
                case State.End:
                    break;
                case State.Finish:
                    break;
                default:
                    break;
            }

        }
        #endregion
    }
}


#region ���O
/*
    �j��y�{�G
    1. ����P�w�G���B�k�䰻��
        ������w���c��G���U�����A�ȥu�O�Ұ���V�P�w�A�u��������઺�P�w���ӭn�b��m�P�w����A�~���|����v�T����C
        �~�ϡG
        (1) ����P�w�u�����@���G�p�G��b��s�ƥ󤤡A�|���ƩI�s����{���ӥX�{�����n�����~�C
        (2) �N��m�P�w��b��s�ƥ󤤩έ��ư���G�p�G�|�ʨ�w�I�A�h�n���e���]�w���w�I�N�|�]���C
        (3) �������k���򴫡H

    2. ��m�P�w�G�ثe�b���Ӯy�ФW
        �o��u���@�ӭ��I�G�u�L�b��H���C�v
        �]�\����n���g�k�A���p�G�ϥΨ��׭p�⪺�ܡA�b0�׭n-90�ת��ɭԴN�n���N0�ন360�סC
        ���o�˧�]��O���֮ɶ��C

    3. ����P�w�G�@���}�l����A�S����ؼ��I���|��
        �����k�G
        �o�����������઺�Ҧ��A�ܩ�ƻ�ɭԱ���A�ƻ�ɭ԰���h���A�o�ӵ{�����M�w�C
        �ݭn�Ψ쪺�ܼƥD�n�� Y�b�ثe����m �P �U�@�Ӯy���I����m�C
        �ר�U�@�Ӯy�Ц�m�����H�{��������ܰʡA���M�~���F�N����U�@�Ӯy�СA�N�|�ण���C
        �t�צ��u�G
        �M�w iSpine ���j�p���F��ʽվ� Spine ���~�A���n���O iSpine ���ܤơC
        �ثe�����u�ʪ��]�p�A����A�[��p��]�w��L�˦��C
        �i�H�T�w���O�A�p�G�u�ʵL�k��������A��L��{���۵M�e���ɽġC

    4. �w�I�P�w�G����O�_��F�w�I�A��F�ᵲ���{��
        ��F�w�I�����k�G
        (1) Ū���w�I����m
        (2) ���Z������:Mathf.DeltaAngle
        (3) �p�G�p��@�w�ȡA����Ӱ��k�A�v�T���h�Ť��P
            A. ����   B. �t�׻���
            �ثe�� B.
        (4) �^�ǰ���T�� or ���w���󰱤�
            �ĳt�׻�����Y�A�ҥH�i�H���w���󰱤�

    PS: �禡���ܼƪ��t���H
        �禡�i�H�ǤJ�ѼơA�]�i�H�^�ǰѼơA�E�ݤ��U�����ܼƬۦP�A���O�̤j���t�O�N�b�禡�u�n�I�s�N�|���椺�����{���X�A�ܼƥu���b�]�w����
        �~�|���椺�e�C


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
