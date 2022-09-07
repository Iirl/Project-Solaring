using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{
    public class noDestoryInChangeScene : ManageScene
    {
        private bool isHaving;
        private string preLoadName;
        private void Awake()
        {
            preLoadName = name;
        }

        private void Start()
        {
            int i;
            i = PlayerPrefs.GetInt(preLoadName);
            

            isHaving = i == 1;
            print(isHaving);
            if (!isHaving)
            {

                PlayerPrefs.SetInt(preLoadName, 1);
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
