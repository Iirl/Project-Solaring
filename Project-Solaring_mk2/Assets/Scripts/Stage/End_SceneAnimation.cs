using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{

	public class End_SceneAnimation : ManageScene
	{
		#region 欄位
		[SerializeField]
		private Animator endWhite;
		private float endTime => endWhite.GetCurrentAnimatorStateInfo(0).length;
		private float deltaTime =0;
		
		#endregion
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			deltaTime+=Time	.deltaTime;
			if (deltaTime >= endTime || Input.GetKeyDown(KeyCode.Escape)) {
				deltaTime = 0;
				ReloadToMenuAndClear();
			}
		}
	}
}