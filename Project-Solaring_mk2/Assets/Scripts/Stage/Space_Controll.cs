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

        [SerializeField, Header("旋轉設定")]
        private Vector3 spaceOffset;
        [SerializeField, Tooltip("旋轉速度"), Range(0f, 50f)]
        private float spine = 2.0f;
        #region 欄位
        private bool rot_left, rot_right, rotate;  //空間是否旋轉
        private float Coordinate = 360;
        #endregion
        private SpacetState spstate;
        public bool isRotate { get { return spstate == SpacetState.Rotate; } }
        private void SPNext() => spstate = spstate < SpacetState.Rotate? spstate+1: 0;
        #region 90度固定旋轉空間
        /// <summary>
        /// 1. 方向(轉向)判定
        /// </summary>
        private void DirectCheck(bool rL, bool rR)
        {
            if (spstate == 0)
            {
                rot_right = rR;
                rot_left = rL;
                //print("檢查是否旋轉");
                if ((rot_right || rot_left) && !rotate)
                {
                    SPNext();
                    Coordinate = PositionCheck(rot_left);
                    //print("1: "+spstate);
                }
            }
            else print("還在旋轉");
        }
        /// <summary>
        /// 2. 位置判定
        /// </summary>
        /// <param name="direct">轉向指引，若為真則+90度；假則是-90度</param>
        /// <returns></returns>
        private float PositionCheck(bool direct)
        {
            if (isRotate) return -1;
            if (spstate == SpacetState.Setting )
            {
                rotate = true;
                //print("取得目標座標");
                float y_axis = Mathf.Floor(transform.eulerAngles.y); // 目前Y軸軸心位置
                int quadrant = (Int32.Parse(y_axis.ToString()) / 90);  //所在象限
                if (quadrant == 0 && !direct) quadrant = 4;
                float next_axis = (direct) ? 90 * (quadrant + 1) : 90 * (quadrant - 1); // 下一個Y軸座標
                if (quadrant == 3 && direct) next_axis %= 360;
                //print($"當前象限:{quadrant}, 下一個座標{next_axis}");
                //print($"Y_Now: {y_axis}");
                SPNext();
                //print($"取得後狀態{spstate}");
                StartCoroutine(Spine());
                return Mathf.Round(next_axis);
            }
            //print("2: "+spstate);
            return 0;
        }
        /// <summary>
        /// 3. 旋轉程式，採調用法呼叫程式。
        /// </summary>
        private IEnumerator Spine()
        {            
            //print($"當前Y軸:目標軸 {transform.eulerAngles.y}:{Coordinate}");
            //print($"{Distane2axis}");
            //print("4:" + spstate);
            float execute = 0;
            yield return new WaitForSeconds(0.1f);
            while (isRotate)
            {
                float y_axis = Mathf.Floor(transform.eulerAngles.y); // 目前Y軸軸心位置
                float Distane2axis = Mathf.Abs(Mathf.DeltaAngle(y_axis, Coordinate)); // 到達下一個Y軸座標的距離            
                Quaternion target = Quaternion.Euler(0, Coordinate, 0);
                //Distane2axis = Mathf.Clamp(Distane2axis, 0, 90);
                StopCheck();
                // 旋轉曲線: x^0-1
                float upper = (Mathf.Abs(Distane2axis)) / 90;
                float iSpine = Mathf.Pow(spine, upper) * Time.deltaTime +1;
                // 收束方法
                if (execute > (360f/spine)) transform.rotation = target;
                else if (Distane2axis < 5 ) iSpine /= 4f;
                else if (Distane2axis < 10) iSpine /= 2f;
                iSpine = Mathf.Clamp(iSpine, 0.005f, 0.1f);
                // 執行旋轉函式
                transform.rotation = Quaternion.Lerp(transform.rotation, target, iSpine);
                //if (rot_left) transform.Rotate(Vector3.up * iSpine);
                //else if (rot_right) //transform.Rotate(Vector3.down * iSpine);
                //if (iSpine == Mathf.Infinity) print(upper);
                if (transform.rotation == target) yield return new WaitForSeconds(0.5f);
                else yield return null;
                execute++;
                
            }
        }
        /// <summary>
        /// 4. 停止判定，採定點判斷，若無法準確到定點則可能無限旋轉。
        /// </summary>
        private void StopCheck()
        {
            //print("4:" + spstate);
            float y_axis = Mathf.Floor(transform.eulerAngles.y); // 目前Y軸軸心位置
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

        #region 事件觸發

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
                    switch (spstate)
                    {
                        case SpacetState.Stay:
                            // 旋轉空間
                            //print(spstate);
                            bool keyLeft = Input.GetAxisRaw("Left_Spine") == 1;
                            bool keyRight = Input.GetAxisRaw("Right_Spine") == 1;
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


#region 筆記
/*
    迴轉流程：
    1. 按鍵判定：左、右鍵偵測
        按鍵測定的構思：按下按鍵後，僅只是啟動轉向判定，真正執行旋轉的判定應該要在位置判定之後，才不會按鍵影響旋轉。
        誤區：
        (1) 旋轉判定只能執行一次：如果放在更新事件中，會重複呼叫旋轉程式而出現不必要的錯誤。
        (2) 將位置判定放在更新事件中或重複執行：如果會動到定點，則好不容易設定的定點就會跑掉。
        (3) 切換左右轉怎麼換？

    2. 位置判定：目前在哪個座標上
        這邊只有一個重點：「過軸轉象限。」
        也許有更好的寫法，但如果使用角度計算的話，在0度要-90度的時候就要先將0轉成360度。
        但這樣改也花費不少時間。

    3. 旋轉判定：一旦開始旋轉，沒有到目標點不會停
        旋轉方法：
        這次直接做旋轉的模式，至於甚麼時候旋轉，甚麼時候停止則不再這個程式中決定。
        需要用到的變數主要有 Y軸目前的位置 與 下一個座標點的位置。
        尤其下一個座標位置不能隨程式執行而變動，不然才剛抵達就切到下一個座標，就會轉不停。
        速度曲線：
        決定 iSpine 的大小除了手動調整 Spine 之外，重要的是 iSpine 的變化。
        目前先做線性的設計，之後再觀察如何設定其他樣式。
        可以確定的是，如果線性無法完美停止，其他方程式自然容易暴衝。

    4. 定點判定：旋轉是否抵達定點，抵達後結束程式
        到達定點的做法：
        (1) 讀取定點的位置
        (2) 取距離公式:Mathf.DeltaAngle
        (3) 如果小於一定值，有兩個做法，影響的層級不同
            A. 停止   B. 速度遞減
            目前採 B.
        (4) 回傳停止訊號 or 指定條件停止
            採速度遞減的關係，所以可以指定條件停止

    PS: 函式跟變數的差異？
        函式可以傳入參數，也可以回傳參數，乍看之下其實跟變數相同，但是最大的差別就在函式只要呼叫就會執行內部的程式碼，變數只有在設定欄位時
        才會執行內容。


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
