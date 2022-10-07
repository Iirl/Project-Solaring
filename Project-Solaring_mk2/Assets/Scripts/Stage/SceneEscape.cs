using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 場景快速鍵
    /// </summary>
    public class SceneEscape : ManageScene
	{
		private bool first;
        // 滿足特定條件時，從此程式呼叫下一關程式。
        // 主要是之前是以中控中心控制關卡的移轉，所以沒有特定由子類別轉場。
        // 沒有特別的情況下中控中心還在的話仍然由中控中心轉場。
        public IEnumerator LoadNextScenesEmtor()
        {
            while (!StaticSharp.isChangeScene) yield return new WaitForSeconds(0.1f);
	        StaticSharp.isChangeScene =false;
	        Time.timeScale = 1;
	        if (first) LoadScenes(11); //第一次執行會進入的關卡
	        else SceneChageEvent(true);
            yield return null;
        }
        private void Start()
        {
            //print("引導場的手動轉場");
	        if (GetScenesName().Contains("Intro")) StartCoroutine(LoadNextScenesEmtor());
	        first = (StaticSharp.isFirstPlay);
        }

        private void Update()
	    {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
	            StaticSharp.isChangeScene =false;
	            if (first) LoadScenes(11); //第一次執行會進入的關卡
	            else SceneChageEvent(true);
            }
        }
    }
}