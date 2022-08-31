using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace solar_a
{
    public class ManageDisco : MonoBehaviour
    {
        [SerializeField, Header("混音控制系統")]
        private AudioMixer MainMixer;
        [SerializeField, Range(-80, 0)]
        private float mstMixerVolume;
        private string mstName = "MainVolume";
        [SerializeField,Header("音樂聲道")]
        private AudioSource MusicPlayer; 
        [SerializeField, Range(0, 1)]
        private float micVolume;
        //private string micName = "micVol";
        [SerializeField, Header("效果音聲道")]
        private AudioSource EffectAudio;
        [SerializeField, Range(0, 1)]
        private float effVolume;
        //private string effName = "effectVol";
        //
        private float setTime=0.5f;
        private float varTime;        

        #region 播放音效方法
        /// <summary>
        /// 播放一次聲音
        /// 0: Music 聲道
        /// 1: Effect 聲道
        /// </summary>
        /// <param name="adc">輸入聲音</param>
        /// <param name="idx">選擇聲道</param>
        private void PlayClips(AudioClip adc, int idx)
        {
            switch (idx)
            {
                case 0: //musicPlayer
                    MusicPlayer.PlayOneShot(adc, micVolume);
                    break;
                case 1: //effect
                    EffectAudio.PlayOneShot(adc, effVolume);
                    break;
                default:
                break;
            }
        }
        /// <summary>
        /// 切換音樂
        /// </summary>
        /// <param name="adc">輸入音樂</param>
        private void ChangeMusic(AudioClip adc)
        {
            if (MusicPlayer.isPlaying) MusicPlayer.Stop();
            if (!MusicPlayer.loop) MusicPlayer.loop = !MusicPlayer.loop;
            MusicPlayer.clip = adc;
            MusicPlayer.Play();
        }

        #endregion
        //公用處理方法
        public void MasterVolumeAdjust(float vol) => mstMixerVolume = vol;
        public void MusicVolumeAdjust(float vol) => micVolume = vol;
        public void EffectVolumeAdjust(float vol) => effVolume = vol;
        public void OneShotMusic(AudioClip audioClip) => PlayClips(audioClip,0);
        public void OneShotEffect(AudioClip audioClip) => PlayClips(audioClip,1);
        public void ChangePlayMusic(AudioClip audioClip) => ChangeMusic(audioClip);



        private void Awake()
        {
            mstMixerVolume = StaticSharp._VOLUME;
        }

        private void Update()
        {            
            if (setTime > varTime) varTime += Time.deltaTime;
            else
            {
                varTime = 0;
                MainMixer.SetFloat(mstName, mstMixerVolume);
            }
            if (StaticSharp._VOLUME != mstMixerVolume) StaticSharp._VOLUME = mstMixerVolume;


        }




    }
}