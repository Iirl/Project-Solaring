﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace solar_a
{
	/// <summary>
	/// 指定移動場景
	/// 針對指定按鈕附加轉跳自 InputField 內容的場景
	/// 該按鍵不需要額外附加轉場事件，否則會有衝突
    /// </summary>
    public class SignScene : MonoBehaviour
    {
        Button btnExit;
        ManageScene mgs;
        [SerializeField]
        TMP_InputField jumpText;
        private void Awake()
        {
            mgs = FindObjectOfType<ManageScene>();
            btnExit = GameObject.Find("btn_EXIT_JUMP").GetComponent<Button>();
            jumpText = GetComponent<TMP_InputField>();
            //print(Convert.ToInt32(jumpText.text));
            btnExit.onClick.AddListener(() => {mgs.LoadScenes(Convert.ToInt32(jumpText.text)); });
        }
    }
}