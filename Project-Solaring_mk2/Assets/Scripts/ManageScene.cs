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
        public string sceneID = "SceneID";

        /// <summary>
        /// 取得目前場景的編號。
        /// 輸入參數一可以取得目前場景的上限。
        /// </summary>
        /// <returns>回傳一個 int 的 index 。</returns>
        public int GetScenes(bool isMax = false)
        {
            if (!isMax) return SceneManager.GetActiveScene().buildIndex;
            else return SceneManager.sceneCountInBuildSettings;
        }
        /// <summary>
        /// 重讀場景
        /// </summary>
        public void ReloadCurrentScene()
        {
            NormalProcessFunction();
            SceneManager.LoadScene(GetScenes());
        }
        /// <summary>
        /// 儲存場景的資訊
        /// 當場景轉換時，紀錄要保留的資訊。
        /// </summary>
        public void SaveLeveInform()
        {
            PlayerPrefs.SetInt(sceneID, GetScenes());
        }
        /// <summary>
        /// 根據目前的指標移動到下一關:
        /// 目前只能指定達到關卡上限之後回到標題。
        /// 應該要在最後加入結束場景，避免直接跳回標題。
        /// </summary>
        public void LoadScenes()
        {
            NormalProcessFunction();
            int idx = PlayerPrefs.GetInt(sceneID);
            idx = (GetScenes(true) == idx+1) ?0:idx+1;
            SceneManager.LoadScene(idx);
        }
        /// <summary>
        /// 載入指定編號的場景
        /// </summary>
        /// <param name="idx">請輸入場景編號</param>
        public void LoadScenes(int idx)
        {
            NormalProcessFunction();
            SceneManager.LoadScene(idx);
        }
        public void LoadScenes(string sname)
        {
            NormalProcessFunction();
            SceneManager.LoadScene(sname);
        }
        /// <summary>
        /// 讀取前一個或下一個場景
        /// </summary>
        public void LoadScenesPreOrder(bool next)
        {
            int now = GetScenes();
            int nexts = now + 1;
            int prevs = now - 1;
            if (next)
            {
                if (nexts == SceneManager.sceneCount) return;
                LoadScenes(now + 1);
            }
            else
            {
                if (prevs < 0) return;
                LoadScenes(now - 1);
            }
            NormalProcessFunction();
        }
        /// <summary>
        /// 結束程式函數
        /// </summary>
        public void Quit()
        {
            Application.Quit();
        }

        /// <summary>
        /// 所有場景執行會用到的通用函數
        /// </summary>
        private void NormalProcessFunction()
        {
            if (Time.timeScale != 1) Time.timeScale = 1;
            CancelInvoke();
        }

        private void Start()
        {
            //print($"原場景編號為：{PlayerPrefs.GetInt(sceneID)}");
        }
    }
}
