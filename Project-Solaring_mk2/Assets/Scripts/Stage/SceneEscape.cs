using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// �����ֳt��
    /// </summary>
    public class SceneEscape : ManageScene
    {

        private void Update()
        {
            if( Input.GetKeyDown(KeyCode.Escape)) StartCoroutine(LoadScenesPreOrder(true));
        }
    }
}