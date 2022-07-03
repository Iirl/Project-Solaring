using UnityEngine;
using System.Collections;

/// <summary>
/// 此程式放在物件群組當中，用來調整物件空間的移動。
/// </summary>
public class Space_Controll : MonoBehaviour
{
    #region 空間變數
    static bool rotated, rot_left, rot_right;  //空間是否旋轉
    [SerializeField, Header("旋轉設定") ,Tooltip("原點")]
    private float rot_angle=0;
    [SerializeField, Tooltip("左轉向角度(L)"), Range(180,359)]
    private float Left_angle = 315;
    [SerializeField, Tooltip("右轉向角度(R)"), Range(0,179)]
    private float Right_angle = 45;
    #endregion
    #region 整體空間(Border)範圍
    [SerializeField, Header("整體空間定義")]
    GameObject SPACE;
    RectTransform Space_RT;
    [SerializeField, Space]
    private float Space_speed = 10;

    #endregion

    #region 空間方法
    /// <summary>
    /// 畫面移動
    /// </summary>
    private void _auto_move()
    {
        if (Space_RT.position.x != 0) Space_RT.position = new Vector2(0,Space_RT.position.y);        
        Space_RT.position += Vector3.up * Time.deltaTime * Space_speed;
    }
    /// <summary>
    ///  選轉整個空間 +-X度
    /// </summary>
    /// 參數請從屬性面板調整，左轉向
    public void _Spine()
    {
        float center_loc = Mathf.Floor(transform.eulerAngles.y);
        //print (transform.localEulerAngles);
        float spine = 2f; //旋轉速度
        if (center_loc >= Right_angle && center_loc < Right_angle + spine * 2) rot_right = false;        
        if (center_loc >= Left_angle && center_loc < Left_angle + spine * 2)   rot_left  = false;

        if (rotated)
        {
            if (rot_left) transform.Rotate(eulers: Vector2.down * spine);
            else if (rot_right) transform.Rotate(eulers: Vector2.up * spine);
            // 停駐點，左轉度數極限和右轉度數極限及原點。
            if (center_loc > rot_angle - spine && center_loc < rot_angle + spine) rotated = false;
            else if (center_loc > Left_angle - spine && center_loc < Left_angle + spine) { rotated = false; }
            else if (center_loc > Right_angle - spine && center_loc < Right_angle + spine) { rotated = false; }
        }
        //print($"Name:= {y_loc}  / rot{rot_angle}");
    }
    public void _Spine(int vect)
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
    public void _2DSpine()
    {

    }
    /// <summary>
    ///  2維座標判定
    /// </summary>
    public void _location()
    {

    }

    #endregion

    #region 事件觸發

    private void Awake()
    {
        rotated = false; rot_left = false; rot_right = false;
        Space_RT = SPACE.GetComponent<RectTransform>();
        //物理空間參數
        Physics.gravity = new Vector3(0, -0.1F, 0);
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        // 場景移動
        _auto_move();

        // 旋轉空間
        if (Input.GetAxisRaw("Left_Spine") == 1) _Spine(-1);
        else if (Input.GetAxisRaw("Right_Spine") == 1) _Spine(1);
        _Spine();

    }
    #endregion
}
