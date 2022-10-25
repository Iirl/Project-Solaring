using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 過場不破壞物件，會偵測場上是否有存在過的物件，若有則刪除；否則加入不破壞行列。
    /// </summary>
    public class noDestoryInChangeScene : ManageScene
    {
        [SerializeField, Header("重複檢查"), HideInInspector]
        private bool isHaving;
        static private List<string> preLoadName = new List<string>();
	    [SerializeField, Header("背景物件調整")]
	    private int starLevel;
	    private int nowLevel => ManageCenter.mgScene.GetScenes();
        //
        public void DestoryOnStageObject() => ClearObject();

        private void ClearObject()
        {
            preLoadName.Clear();
            Destroy(gameObject);
        }

        private void Awake()
        {
	        if (starLevel > 0)gameObject.name = gameObject.name + starLevel;
            if (preLoadName.Count > 0) foreach (string name in preLoadName) if (name == this.name) isHaving = true;
            if (isHaving) Destroy(gameObject);
            else
            {
                preLoadName.Add(name);
                DontDestroyOnLoad(gameObject);
            }
	        starLevel +=  1;
        }
        private void Update()
	    {
            if (starLevel > 0)
            {
	            if( nowLevel != starLevel && nowLevel != starLevel + 1) Destroy(gameObject);
            }
		    
	    }
    }
}
