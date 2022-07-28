using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


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
        [SerializeField, Header("播放音效")]
        private List<AudioClip> sound_effect;
        [SerializeField]
        private AudioSource audioSrc;
        [SerializeField, Header("選單按鈕控制")]
        private List<GameObject> menuButton;
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
            if (Mathf.Ceil(y) != Mathf.Ceil(end) && !bg_move)
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
        /// 選單音效效果
        /// 音效註解：
        /// </summary>
        public void MenuSound(int i)
        {
            if (sound_effect[i] != null) audioSrc.PlayOneShot(sound_effect[i]);
        }
        #endregion

        #region 觸發事件
        private void Awake()
        {
            bg_move = false;
            audioSrc = GameObject.Find("Audio Source").GetComponent<AudioSource>();
            // 直接讀取按鈕


        }

        private void Start()
        {

        }

        private void FixedUpdate()
        {

        }
        private void Update()
        {
            if (Input.GetAxisRaw("Menu") > 0 && reload_scene) SceneSTG.ReloadCurrentScene(); // 重讀場景

        }


        #endregion
    }
}