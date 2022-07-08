using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 此程式放在物件群組當中，用來調整物件空間的移動。
/// </summary>
namespace solar_a
{
    public class Space_Controll : MonoBehaviour
    {
        #region 變數
        static bool rotated, rot_left, rot_right;  //空間是否旋轉
        [SerializeField, Header("旋轉設定"), Tooltip("原點")]
        private float rot_angle = 0;
        [SerializeField, Tooltip("左轉向角度(L)"), Range(5f, 90f)]
        private float Left_angle = 45;
        [SerializeField, Tooltip("右轉向角度(R)"), Range(0f, 90f)]
        private float Right_angle = 45;
        [SerializeField, Tooltip("旋轉速度"), Range(0f, 10f)]
        float spine = 2.0f; //旋轉速度
        #endregion

        #region 欄位

        #endregion

        #region 空間方法
        /// <summary>
        ///  選轉整個空間 +-X度
        /// </summary>
        /// 參數請從屬性面板調整，左轉向
        private void _Spine()
        {
            float center_loc = Mathf.Floor(transform.eulerAngles.y); // Y軸軸心位置
            float left_loc = rot_angle > Left_angle ? rot_angle - Left_angle : 360 + rot_angle - Left_angle; // Y軸逆旋位置
            float right_loc = rot_angle + Right_angle < 360 ? rot_angle + Right_angle : 360 - rot_angle + Right_angle; // Y軸正旋位置
            // 判斷是否停止轉動
            if (center_loc >= right_loc && center_loc < 180) rot_right = false;
            if (center_loc <= left_loc && center_loc > 180) rot_left = false;

            if (rotated)
            {
                FadeSpine(center_loc, left_loc, right_loc);
                if (rot_left) { }//transform.Rotate(eulers: Vector2.down * spine);
                else if (rot_right) { } //transform.Rotate(eulers: Vector2.up * spine);
                                        // 停駐點，左轉度數極限和右轉度數極限及原點。
                //if (center_loc == rot_angle) rotated = false;
                //else if (center_loc > left_loc - spine && center_loc < left_loc + spine) { rotated = false; }
                //else if (center_loc > right_loc - spine && center_loc < right_loc + spine) { rotated = false; }

            }
            //print($"Name:= {y_loc}  / rot{rot_angle}");
        }
        private void _Spine(int vect)
        {
            // 1 右轉  -1 左轉
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
            float pos = l_angle + r_angle; // 取得目前位置
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
                print("左邊");
                rotated = false;
            }
            else if (pos <= -Left_angle - Right_angle)
            { // -
                print("右邊");
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

        #region 事件觸發

        private void Awake()
        {
            rotated = false; rot_left = false; rot_right = false;
            //物理空間參數
            Physics.gravity = new Vector3(0, -0.1F, 0);
            transform.Rotate(new Vector3(0, rot_angle, 0));
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {

            // 旋轉空間
            if (Input.GetAxisRaw("Left_Spine") == 1) _Spine(-1);
            else if (Input.GetAxisRaw("Right_Spine") == 1) _Spine(1);
            _Spine();

        }
        #endregion
    }
}