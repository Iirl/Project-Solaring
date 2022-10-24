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
        [SerializeField]
        private int starLevel;
        //
        public void DestoryOnStageObject() => ClearObject();

        private void ClearObject()
        {
            preLoadName.Clear();
            Destroy(gameObject);
        }

        private void Awake()
        {
            if (preLoadName.Count > 0) foreach (string name in preLoadName) if (name == this.name) isHaving = true;
            if (isHaving) Destroy(gameObject);
            else
            {
                preLoadName.Add(name);
                DontDestroyOnLoad(gameObject);
            }
        }
        private void Update()
        {
            if (starLevel > 0)
            {
                //if( ManageCenter.mgScene.GetScenes() != starLevel && ManageCenter.mgScene.GetScenes() != starLevel - 1) Destroy(gameObject);
            }
        }
    }
}
