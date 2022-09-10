using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{

    /// <summary>
    /// 直接從中控台控制開關。
    /// </summary>
    public class FinishArea : ManageScene
    {
        [SerializeField]
        private BoxCollider FinishBox;
        static public bool finishState;

        private void Awake()
        {
            FinishBox = GetComponent<BoxCollider>();
            finishState = true;
        }
        private void Start()
        {
            FinishBox.enabled = true;
        }

    }

}