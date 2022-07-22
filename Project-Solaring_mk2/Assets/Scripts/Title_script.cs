using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 處理標題有關的程式，每個功能建議用一個區域框起來。
/// </summary>
namespace solar_a
{
    public class Title_script : MonoBehaviour
    {
        #region 屬性
        [SerializeField, Header("場景處理系統")]
        ManageScene SceneSTG;
        [SerializeField, Header("Object Obtain")]
        private AudioClip acp_down;
        [SerializeField]
        private AudioSource audioSrc;
        [SerializeField]
        private GameObject btn_str, btn_opt, btn_open;
        [SerializeField]
        private RectTransform bg_space, bg_earth;
        [SerializeField, Header("Property Adjust")]
        private float bg_move_speed = 0.5f;
        // 其他屬性(欄位)
        private bool bg_move = true, reload_scene = false;

        #endregion

        #region 基本功能
        /// <summary>
        /// 調整選單的動畫
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
        /// 選單事件：第一層->第二層
        /// </summary>
        public void Menu1to2()
        {
            audioSrc.PlayOneShot(acp_down);
            btn_open.SetActive(false);
            if (!btn_str.activeSelf) btn_str.SetActive(true);
            if (!btn_opt.activeSelf) btn_opt.SetActive(true);
        }
        public void Menu2_Start()
        { 
            SceneSTG.LoadScenes(1);
        }
        #endregion

        #region 觸發事件
        private void Awake()
        {
            audioSrc = GameObject.Find("Audio Source").GetComponent<AudioSource>();
            // 直接讀取按鈕
            btn_str.SetActive(false);
            btn_opt.SetActive(false);

        }

        private void Start()
        {

        }

        private void FixedUpdate()
        {
            if (bg_space != null && bg_move) Move2Center(bg_space, transform.position.y / 2);
            if (bg_earth != null && bg_move) Move2Center(bg_earth, transform.position.y / 4);
        }
        private void Update()
        {
            if (Input.GetAxisRaw("Bbutton") > 0 && reload_scene) SceneSTG.ReloadCurrentScene(); // 重讀場景
            if (Input.anyKeyDown && btn_open.activeSelf) Menu1to2(); // 鍵盤觸發進入下一層選單事件

        }


        #endregion
    }
}