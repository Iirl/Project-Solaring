using UnityEngine;
using UnityEngine.SceneManagement;

namespace solar_a
{
    /// <summary>
    /// 場景管理系統
    /// 只要處理場景事件的方法請依此類別處理。
    /// Function:
    /// - Take out the scene id
    /// - Reload scene
    /// - Load asign scene
    /// - Pre or Next scene load
    /// - Quit the application
    /// </summary>
    public class ManageScene : MonoBehaviour
    {
        /// <summary>
        /// 取得目前場景的編號。
        /// 輸入參數一可以取得目前場景的上限。
        /// </summary>
        /// <returns>回傳一個 int 的 index 。</returns>
        public int GetScenes(bool isMax=false)
        {
            if (!isMax) return SceneManager.GetActiveScene().buildIndex;
            else return SceneManager.sceneCount;
        }
        /// <summary>
        /// 重讀場景
        /// </summary>
        public void ReloadCurrentScene()
        {
            if (Time.timeScale != 1) Time.timeScale = 1;
            SceneManager.LoadScene(GetScenes());
        }        
        /// <summary>
        /// 載入指定編號的場景
        /// </summary>
        /// <param name="idx">請輸入場景編號</param>
        public void LoadScenes(int idx)
        {
            if (Time.timeScale != 1) Time.timeScale = 1;
            SceneManager.LoadScene(idx);
        }        
        /// <summary>
        /// 讀取前一個或下一個場景
        /// </summary>
        public void LoadScenesPreOrder(bool next)
        {
            int now = GetScenes();
            int nexts = now +1;
            int prevs = now -1;
            if (next) {
                if (nexts == SceneManager.sceneCount) return;
                LoadScenes(now +1);
            } else {
                if (prevs < 0) return;
                LoadScenes(now -1);
            }
        }
        /// <summary>
        /// 結束程式函數
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }
    }
}
