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
        [SerializeField, Header("旋轉設定"), Tooltip("原點：除非必要，否則應避免設定小數點。\n" +
            "小於直角範圍的話左右轉向到底會停止；大於直角範圍則回形成一個迴圈，目前稍微測試應該是沒問題，" +
            "但不確定是否某些數值會有BUG，請謹慎設定。")]
        private float rot_angle = 0;
        [SerializeField, Tooltip("旋轉速度"), Range(0f, 10f)]
        float spine = 2.0f;
        #endregion

        #region 欄位
        float Coordinate;
        #endregion

        #region 90度固定旋轉空間
        private void DirectCheck()
        {

        }
        private void PositionCheck(int direct)
        {
            float y_axis = Mathf.Floor(transform.eulerAngles.y); // Y軸軸心位置
            int quadrant = Int32.Parse( y_axis.ToString()) / 90;  //所在象限
            float next_axis = (direct == 1)? 90 * (1 + quadrant): 90 * (quadrant); // 下一個Y軸座標
            float Distane2axis = (Mathf.DeltaAngle(y_axis, next_axis)); // 到達下一個Y軸座標的距離

            //print($"Now:{y_axis}, quadrant:{quadrant}");
            //print($"Next Y:{next_axis}, dist:{Distane2axis}");
        }
        private void Spine()
        {
            float y_axis = Mathf.Floor(transform.eulerAngles.y); // Y軸軸心位置
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
            if (Input.GetAxisRaw("Left_Spine") == 1) { rot_left = true; }
            else if (Input.GetAxisRaw("Right_Spine") == 1) { rot_right = true; }
            PositionCheck(-1);

        }
        #endregion
    }
}


#region 筆記
/*
    迴轉流程：
    1. 按鍵判定：左、右鍵偵測
        按鍵測定的構思：按下按鍵後，僅只是啟動轉向判定，真正執行旋轉的判定應該要在位置判定之後，才不會按鍵影響旋轉。

    2. 位置判定：目前在哪個座標上

    3. 旋轉判定：一旦開始旋轉，沒有到目標點不會停
        旋轉方法：
        這次直接做旋轉的模式，至於甚麼時候旋轉，甚麼時候停止則不再這個程式中決定。
        需要用到的變數主要有 Y軸目前的位置 與 下一個座標點的位置。
        尤其下一個座標位置不能隨程式執行而變動，不然才剛抵達就切到下一個座標，就會轉不停。

    4. 定點判定：旋轉是否抵達定點，抵達後結束程式
 


//// 下面是原本的寫法，想要寫成扇形旋轉，但沒有成功。
        #region 空間方法(捨棄)
        [SerializeField, Tooltip("左轉向角度(L)"), Range(5f, 90f)]
        private float Left_angle = 45;
        [SerializeField, Tooltip("右轉向角度(R)"), Range(0f, 90f)]
        private float Right_angle = 45;
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
            if (rotated)  FadeSpine(center_loc, left_loc, right_loc);
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
        /// <summary>
        /// 漸變速度方法
        /// </summary>
        /// <param name="c">目前軸心的角度</param>
        /// <param name="l">左邊界角度</param>
        /// <param name="r">右邊界角度</param>
        private void FadeSpine(float c, float l, float r)
        {
            float r_angle = (Mathf.DeltaAngle(c, r));
            float l_angle = (Mathf.DeltaAngle(c, l));
            float i_spine = spine, i = 1;
            float pos = l_angle + r_angle; // 取得目前位置
            int r_spine = PosFix(pos, l, r);
            switch (r_spine)
            {
                case 0: i = pos; break;
                case 2: i = r_angle; break;
                case -2: i = l_angle; break;
                default: break;
            }
            /// 選轉速度調整：
            /// 目前使用Log函數調整轉速，若要更變移動曲線請在此修正
            i_spine *= Mathf.Log(Mathf.Abs(i + 1), spine_log);
            /// ----
            if (i_spine <= 0) i_spine = 1f;
            if (rot_left) transform.Rotate(Vector2.down * i_spine);
            else if (rot_right) transform.Rotate(Vector2.up * i_spine);
            //print(r_angle);
            //print($"POS:{pos}, spine:{i_spine}"); // 檢查位置，轉速
            //print($"{rotated} L{rot_left} R{rot_right}"); // 檢查旋轉開關

        }
        /// <summary>
        /// 用來校正移動座標，請參閱_Spine()函數
        /// </summary>
        /// <param name="p">目前位置</param>
        /// <param name="l">左旋位置</param>
        /// <param name="r">右旋位置</param>
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
                //print("左邊界線");
                rotated = false;
                n = -1;
            }
            else if (p <= -Left_angle - Right_angle) // -
            {
                //print("右邊界線");
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
