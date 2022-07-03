using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class System_function : MonoBehaviour
{
    #region 屬性
    BoxCollider _boxBorder;
    [SerializeField]
    TMP_Text ui_Dist, ui_fuel;
    [SerializeField]
    private int UI_moveDistane = 0, UI_fuel=100;
    [SerializeField, Header("取得元件")]
    GameObject Border;
    #endregion
    #region 方法
    /// <summary>
    /// 設定UI的提示
    /// </summary>
    private void _show_UI()
    {
        UI_moveDistane = (int)(Border.transform.position.y);
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
        _boxBorder.size = new Vector3(w+1, h, h+2);

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
        _boxBorder = GameObject.Find("Border").GetComponent<BoxCollider>();
        Box_border();
    }
    private void Update()
    {
        _show_UI();
    }
    private void FixedUpdate()
    {
        
    }
    #endregion
}
