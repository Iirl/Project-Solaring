using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class System_function : MonoBehaviour
{
    #region 屬性
    RectTransform scene_move_rt;
    BoxCollider _boxBorder;
    [SerializeField]
    TMP_Text ui_Dist, ui_fuel;
    [SerializeField]
    private int UI_moveDistane = 0, UI_fuel=100;    
    #endregion
    #region 方法
    /// <summary>
    /// 畫面移動
    /// </summary>
    private void _auto_move()
    {
        scene_move_rt.position += Vector3.up * Time.deltaTime;
    } 
    /// <summary>
    /// 設定UI的提示
    /// </summary>
    private void _show_UI()
    {
        UI_moveDistane = (int)( scene_move_rt.position.y);
        if (ui_Dist != null) ui_Dist.text = $"Distance:{UI_moveDistane}";
        if (ui_fuel != null) ui_fuel.text = $"Fuel:{UI_fuel}";
    }

    /// <summary>
    /// 程式啟動時或者調整大小時要呼叫此函數調整邊界。
    /// </summary>
    private void Box_border()
    {
        float w = _boxBorder.GetComponentInChildren<Camera>().aspect * 10;     //width
        float h = (1 / _boxBorder.GetComponentInChildren<Camera>().aspect) * w; //heigh
        _boxBorder.size = new Vector3(w+2, h+2, w+h/2);

    }
    #endregion

    #region 全域控制方法
    /// <summary>
    /// 重讀場景
    /// </summary>
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region 事件
    private void Awake()
    {
        scene_move_rt = GameObject.Find("Border").GetComponent<RectTransform>();
        _boxBorder = GameObject.Find("Border").GetComponent<BoxCollider>();
        Box_border();
    }
    private void Update()
    {
        _auto_move();
        _show_UI();
    }
    private void FixedUpdate()
    {
        
    }
    #endregion
}
