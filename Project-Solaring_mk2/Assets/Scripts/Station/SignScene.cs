using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace solar_a
{
    /// <summary>
    /// ���w���ʳ���
    /// �w����w���s���[����� InputField ���e������
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
            btnExit = GameObject.Find("btn_EXIT").GetComponent<Button>();
            jumpText = GetComponent<TMP_InputField>();
            //print(Convert.ToInt32(jumpText.text));
            btnExit.onClick.AddListener(() => {mgs.LoadScenes(Convert.ToInt32(jumpText.text)); });
        }

    }
}