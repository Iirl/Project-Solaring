using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace solar_a
{
    /// <summary>
    /// 系統設定混音器，用來控制混音器的音量。
    /// </summary>
    public class MixerControll : MonoBehaviour
    {
        [SerializeField]
        AudioMixer audioMixer;
        [SerializeField]
        private Slider slider;
        private void MainVolume(float vol)
        {
            audioMixer.SetFloat("MainVolume", vol);
            StaticSharp._VOLUME = vol;
            slider.value = vol;
        }
        public void VolumeResset() => MainVolume(0);
        public void VolumeSlider() => MainVolume(slider.value);
    }
}
