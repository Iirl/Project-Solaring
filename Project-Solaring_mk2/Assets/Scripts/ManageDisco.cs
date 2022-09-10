using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace solar_a
{
    /// <summary>
    /// 聲音控制中心
    /// 1. 調整音量
    /// 透過 *Adjust(float) 改變*的音量。
    /// 2. 播放音效
    /// 透過 OneShot*(AuidoClip) 來播放一次聲音，*為聲道。
    /// </summary>
    public class ManageDisco : MonoBehaviour
    {
        [SerializeField, Header("混音控制系統")]
        private AudioMixer MainMixer;
        [SerializeField, Range(-80, 0)]
        private float mstMixerVolume;
        private string mstName = "MainVolume";
        [SerializeField, Header("音樂聲道")]
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

        private IEnumerator MixerVolAdjust()
        {
            bool on = true;
            while (on)
            {
                MainMixer.SetFloat(mstName, mstMixerVolume);
                if (StaticSharp._VOLUME != mstMixerVolume) StaticSharp._VOLUME = mstMixerVolume;
                yield return new WaitForSeconds(0.5f);
                on = false;
            }
        }
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
        public bool MusicIsPlaying => MusicPlayer.isPlaying;
        public bool EffectIsPlaying => EffectAudio.isPlaying;
        public void OneShotMusic(AudioClip audioClip) => PlayClips(audioClip, 0);
        public void OneShotEffect(AudioClip audioClip) => PlayClips(audioClip, 1);
        public void ChangePlayMusic(AudioClip audioClip) => ChangeMusic(audioClip);

        private void Awake()
        {
            mstMixerVolume = StaticSharp._VOLUME;
        }
        private void Start()
        {
            StartCoroutine(MixerVolAdjust());            
        }
    }
}