using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 處理標題有關的程式，每個功能建議用一個區域框起來。
/// </summary>
public class Title_script : MonoBehaviour
{
    #region 屬性
    [SerializeField, Header("Object Obtain")]
    private Camera cam;
    [SerializeField]
    private GameObject btn_str, btn_opt, btn_open;
    [SerializeField]
    private RectTransform bg_space, bg_earth, cvs_rect;
    private AudioSource audioSrc;
    [SerializeField]
    private AudioClip acp_down;
    [SerializeField, Header("Property Adjust")]
    private float bg_move_speed = 0.5f;
    // 其他屬性(欄位)
    private bool bg_move = true, reload_scene = false;

    #endregion

    #region 基本功能
    /// <summary>
    /// 調整選單的垂直位置
    /// </summary>
    /// <param name="rts">傳入的 Rect 元件</param>
    /// <param name="end">移動結束點</param>
    private void Move2Center(RectTransform rts, float end)
    {
        float y = rts.position.y;
        if (Mathf.Ceil(y) != Mathf.Ceil(end))
        {
            rts.Translate(new Vector2(0, end - y) * Time.deltaTime * bg_move_speed);
        }
        else
        {
            bg_move = false;
            reload_scene = true;
        }

    }
    /// <summary>
    /// 重讀場景
    /// </summary>
    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// 選單事件：第一層->第二層
    /// </summary>
    public void Menu1to2()
    {
        btn_str.SetActive(true);
        btn_opt.SetActive(true);
        btn_open.SetActive(false);
        audioSrc.PlayOneShot(acp_down);
    }

    #endregion

    #region 觸發事件
    private void Awake()
    {
        audioSrc = GameObject.Find("Audio Source").GetComponent<AudioSource>();

    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        if (bg_space != null && bg_move) Move2Center(bg_space, cvs_rect.position.y / 2);
        if (bg_earth != null && bg_move) Move2Center(bg_earth, cvs_rect.position.y / 4);
    }
    private void Update()
    {
        if (Input.GetAxisRaw("Bbutton") > 0 && reload_scene) ReloadCurrentScene(); // 重讀場景
        if (Input.anyKeyDown && btn_open.activeSelf) Menu1to2(); // 鍵盤觸發進入下一層選單事件

    }


    #endregion
}
