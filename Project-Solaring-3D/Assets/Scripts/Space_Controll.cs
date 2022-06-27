using UnityEngine;
using System.Collections;

public class Space_Controll : MonoBehaviour
{
    #region 空間變數
    [SerializeField] 
    private bool rotated = false;  //空間是否旋轉
    #endregion


    #region 空間方法
    /// <summary>
    ///  選轉整個空間 +-90度
    /// </summary>
    public void _Spine()
    {
        float y_loc = Mathf.Floor(transform.rotation.y);
        //if (Mathf.Abs(y_loc) < 3) y_loc = 0;
        y_loc = (y_loc == 0) ?  -90 :  90;
        rotated = !rotated;
        transform.Rotate(0,y_loc,0);

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
        //物理空間參數
        Physics.gravity = new Vector3(0, -0.1F, 0);
    }

    private void Start()
    {
    }

    #endregion
}
