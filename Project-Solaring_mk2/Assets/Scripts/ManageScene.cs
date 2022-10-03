using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using static Cinemachine.DocumentationSortingAttribute;

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
        [HideInInspector]
        public string sceneID = "SceneID";
        [SerializeField]
	    public int level1Scene = 2;
	    // 場景公開使用方法
        public int GetScenes(bool isMax = false) => GetActiveSceneOrBuild(isMax);
        public string GetScenesName() => GetActiveSceneName();
        public void ReloadCurrentScene() => SceneReload();
        public void ReloadToStart() => ClearInformation(true);
	    public void ReloadToMenuAndClear() => ClearInformation(true, 0);
        public void ReloadToAndClear() => ClearInformation();
        public void SaveLeveInform(int level) => SaveInformation(level);
        public void SceneChageEvent(bool isNext) => StartCoroutine(EnumerLoadScene(isNext));
        public void LoadScenes(int idx) => GenericScene(idx);
	    public void LoadScenes(string sname) => GenericScene(sname);
	    public void Quit() => Application.Quit(); // 結束程式函數
        /// <summary>
        /// 取得目前場景的編號。
        /// 輸入參數一可以取得目前場景的上限。
        /// </summary>
        /// <returns>回傳一個 int 的 index 。</returns>
        private int GetActiveSceneOrBuild(bool isMax = false)
        {
            if (!isMax) return SceneManager.GetActiveScene().buildIndex;
            else return SceneManager.sceneCountInBuildSettings;
        }
        private string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
        /// <summary>
        /// 重讀場景
        /// </summary>
        private void SceneReload()
        {
            ManageCenter.rocket_ctl.StateToBorken(false);
            SceneManager.LoadScene(GetScenes());
        }
        /// <summary>
        /// 儲存場景的資訊
        /// 當場景轉換時，紀錄要保留的資訊。
        /// </summary>
        private void SaveInformation(int level)
        {
            PlayerPrefs.SetInt(sceneID, GetScenes());
            StaticSharp.Rocket_INFO = ManageCenter.rocket_ctl.RocketVarInfo;         //火箭當前數值
            StaticSharp.Rocket_BASIC = ManageCenter.rocket_ctl.RocketBasic;     //火箭基本數值
            StaticSharp.Rocket_POS = ManageCenter.rocket_ctl.transform.position;     //火箭當前位置
            //StaticSharp.DistanceRecord = UI_moveDistane;   // 調整成目前距離
            StaticSharp._LEVEL = level;
            StaticSharp.DistanceRecord = ManageCenter.UI_moveDistane;     // 紀錄當前關卡的距離
        }
        /// <summary>
        /// 清除所有場上存在過的物件
        /// </summary>
        /// <param name="restart">真為重新載入場景</param>
	    private void ClearInformation(bool restart=false, int sceneid = -1)
        {
            PlayerPrefs.SetInt(sceneID, 2);
            StaticSharp.Rocket_INFO = Vector3.zero;
            StaticSharp.Rocket_BASIC = Vector3.zero;     //火箭基本數值
            StaticSharp.Rocket_POS = Vector3.zero;     //火箭當前位置
            StaticSharp._LEVEL = 1;
            StaticSharp.DistanceRecord = 0;     // 紀錄當前關卡的距離
	        // 淨空場景物件
	        try {
		        noDestoryInChangeScene[] nDICS = FindObjectsOfType<noDestoryInChangeScene>();
		        int len = nDICS.Length;
		        if (len > 0)
		        	for (int i = 0; i < len	; i++) nDICS[i].DestoryOnStageObject();
	        } catch (System.Exception e) {
	        	print("清除物件失敗");
	        }

            if (!restart) return;
            //若需要重新載入場景則執行以下段落
	        StaticSharp._SecretSCORE = 0;
	        if (sceneid >= 0) GenericScene(sceneid);
	        else GenericScene(level1Scene);
        }
        /// <summary>
        /// 根據目前的指標移動到下一關:
        /// 目前只能指定達到關卡上限之後回到標題。
        /// 應該要在最後加入結束場景，避免直接跳回標題。
        /// </summary>
        public void LoadScenes()
        {
            int idx = PlayerPrefs.GetInt(sceneID);
            float nextdist = ManageCenter.mgCenter.stInfo[idx-1].finishDistane;
            float inDist =  StaticSharp.DistanceRecord += 1000f;
            //print($"{inDist} -> {idx}:{nextdist}");
            if (inDist > nextdist) idx += 1;
            GenericScene(idx);
        }
        /// <summary>
        /// 載入指定編號的場景
        /// </summary>
        /// <param name="idx">請輸入場景編號</param>
        private void GenericScene<T> (T value) where T:IComparable
        {
            if (typeof(T) == typeof(int)) SceneManager.LoadScene(Convert.ToInt32(value),LoadSceneMode.Single);
            if (typeof(T) == typeof(string)) SceneManager.LoadScene(value.ToString(), LoadSceneMode.Single);
        }
        /// <summary>
        /// 讀取前一個或下一個場景
        /// </summary>
        private IEnumerator EnumerLoadScene(bool next=true)
        {
            if (StaticSharp.isChangeScene) yield break;
            while (StaticSharp.isDialogEvent) yield return new WaitForSeconds(1);
            StaticSharp.isChangeScene = false;
            int now = GetScenes();
            int nexts = now + 1, prevs = now - 1;
	        //print($"目前場景編號為：{now}, Next:{nexts}, Previous:{prevs}");
            if (GetScenes()+1 == GetScenes(true) - 1) ClearInformation(true);
            else if (next) LoadScenes(nexts);
            else if (prevs > 0) LoadScenes(prevs);
        }
    }
}
