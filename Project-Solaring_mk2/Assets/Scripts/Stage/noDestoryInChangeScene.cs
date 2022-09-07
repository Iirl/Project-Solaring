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
        [SerializeField]
        private bool isHaving;
        static private List<string> preLoadName = new List<string>();
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
    }
}
