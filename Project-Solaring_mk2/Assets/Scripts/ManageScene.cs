using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
        private void SceneChageEvent(bool isNext) => StartCoroutine(LoadScenesPreOrder(isNext));

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
        public void ReloadCurrentScene() => SceneManager.LoadScene(GetScenes());
        /// <summary>
        /// 儲存場景的資訊
        /// 當場景轉換時，紀錄要保留的資訊。
        /// </summary>
        public void SaveLeveInform(){
            PlayerPrefs.SetInt(sceneID, GetScenes());

            StaticSharp.Rocket_INFO = ManageCenter.rocket_ctl.RocketS1;         //火箭當前數值
            StaticSharp.Rocket_BASIC = ManageCenter.rocket_ctl.RocketBasic;     //火箭基本數值
            //StaticSharp.DistanceRecord = UI_moveDistane;   // 調整成目前距離
            StaticSharp.DistanceRecord = ManageCenter.ss_ctl.finishDistane;     // 紀錄當前關卡的距離
        }
        /// <summary>
        /// 根據目前的指標移動到下一關:
        /// 目前只能指定達到關卡上限之後回到標題。
        /// 應該要在最後加入結束場景，避免直接跳回標題。
        /// </summary>
        public void LoadScenes()
        {
            int idx = PlayerPrefs.GetInt(sceneID);
            idx = (GetScenes(true) == idx+1) ?0:idx+1;
            SceneManager.LoadScene(idx);
        }
        /// <summary>
        /// 載入指定編號的場景
        /// </summary>
        /// <param name="idx">請輸入場景編號</param>
        public void LoadScenes(int idx) => SceneManager.LoadScene(idx);
        public void LoadScenes(string sname)=> SceneManager.LoadScene(sname);
        /// <summary>
        /// 讀取前一個或下一個場景
        /// </summary>
        public IEnumerator LoadScenesPreOrder(bool next)
        {
            while (StaticSharp.isDialogEvent) yield return new WaitForSeconds(1);
            StaticSharp.isChangeScene = false;
            int now = GetScenes();
            int nexts = now+1, prevs = now-1;
            //print($"目前場景編號為：{now}, Next:{nexts}, Previous:{prevs}");
            if (next && nexts  != GetScenes(true)) LoadScenes(nexts);
            else if (prevs >0) LoadScenes(prevs);
        }
        /// <summary>
        /// 結束程式函數
        /// </summary>
        public void Quit() => Application.Quit();

        private void Start()
        {
            //print($"原場景編號為：{PlayerPrefs.GetInt(sceneID)}");
            //print($"最大場景數量為：{GetScenes(true)}");
        }
    }
}
