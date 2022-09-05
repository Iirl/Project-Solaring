using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace solar_a
{
    public class MixerControll : MonoBehaviour
    {
        [SerializeField]
        AudioMixer audioMixer;
        float vol;

        private void Awake()
        {
            audioMixer.GetFloat("MainVolume", out vol);
        }
        private void Update()
        {
            if (vol != StaticSharp._VOLUME)
            {
                audioMixer.SetFloat("MainVolume", StaticSharp._VOLUME);
                audioMixer.GetFloat("MainVolume", out vol);
            }
        }
    }
}
