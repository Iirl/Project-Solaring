using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace solar_a
{
    /// <summary>
    /// 自定播放聲音系統：
    /// 1. 放上聲音資料
    /// 2. 指定列表號碼
    /// 直接使用本
    /// </summary>
    public class AudioSystem : MonoBehaviour
    {
        [SerializeField]
        protected DataAudio dataAdo;
        [SerializeField]
        protected int playNumber;
        [SerializeField]
        protected bool allowDulpic;
        //
        [SerializeField]
        protected bool playLoop;
        [SerializeField, Header("時間設定"), Range(0, 1000), HideInInspector]
        protected int playTimes;
	    [SerializeField, Range(0.01f, 300), HideInInspector]
        protected float waitTime = 0.5f;
        //
        protected ManageDisco mds;
        protected bool isPlayingSound;
        // 播放聲音方法，預設為播放音效，如要修改則覆寫此方法。
        protected virtual void SoundPlayer() {  }
        protected virtual IEnumerator SoundCheck() { yield break; }
        protected virtual void SoundOnStart() { }
	    protected virtual void SoundOnEnd() { enabled = false; }
	    // 共用私人
	    private IEnumerator player;
        // 公開使用方法
        public int setNumber { set { playNumber = value; } }
        public bool AllowDulpic { set { allowDulpic = value; } }
        public bool PlayLoop { set { playLoop = value; } }


        #region 事件整理
        /// <summary>
        /// 播放音效計時器
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayerTimer()
        {
            int count = 0;

	        SoundOnStart(); // 1
            yield return new WaitForSeconds(0.1f);
            do
            {
                count++;
                if (playTimes < count && playTimes > 0) break;
	            SoundPlayer(); // 2
                if (waitTime == 0) break;
                yield return new WaitForSeconds(waitTime);
	            if (!allowDulpic) yield return StartCoroutine(SoundCheck()); //3
            } while (playLoop);
	        SoundOnEnd(); // 4
        }


        private void OnEnable()
        {
	        if (mds) { player = PlayerTimer(); StartCoroutine(player); }
	        //print(player);
        }
        protected virtual void OnDisable()
        {
	        playLoop = false;
        	StopCoroutine(player);
        }
        private void Awake()
        {
	        mds = FindObjectOfType<ManageDisco>();
	        if(!dataAdo) print($"物件 {gameObject.name} 沒有聲音資料檔。");
        }
        #endregion
    }
}
