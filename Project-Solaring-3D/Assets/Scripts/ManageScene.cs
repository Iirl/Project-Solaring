using UnityEngine;
using UnityEngine.SceneManagement;

namespace solar_a
{
    /// <summary>
    /// 場景管理系統
    /// 只要處理場景事件的方法請依此類別處理。
    /// </summary>
    public class ManageScene : MonoBehaviour
    {
        /// <summary>
        /// 重讀場景
        /// </summary>
        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        /// <summary>
        /// 取得目前場景的編號
        /// </summary>
        /// <returns>回傳一個 int 的 index 。</returns>
        public int LoadScenes()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
        /// <summary>
        /// 載入指定編號的場景
        /// </summary>
        /// <param name="idx">請輸入場景編號</param>
        public void LoadScenes(int idx)
        {
            SceneManager.LoadScene(idx);
        }
    }
}