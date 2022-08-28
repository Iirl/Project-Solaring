using System.Collections;
using UnityEngine;
namespace solar_a
{
    // 掛在物件上會自動抓取 AudioSource 並且依照設定內容撥放音效
    public class AudioPlay : MonoBehaviour
    {
        [SerializeField, Header("Audio Souse")]
        AudioSource ads;
        [SerializeField, Header("播放長度")]
        private float playTime;
        [SerializeField, Header("播放間隔")]
        private float playInterval;
        private bool isDoing;

        private void Awake()
        {
            ads = ads ?? GetComponent<AudioSource>();
        }
        private void Update()
        {
            if (!isDoing) StartCoroutine(PlaySound());
            // 一旦遊戲狀態中止就會停止撥放
            if (StaticSharp.Conditions == (State)4 && !isDoing)
            {
                isDoing = true;
                enabled = false;
            }
        }

        private IEnumerator PlaySound()
        {
            isDoing = true;
            ads.Play();
            yield return new WaitForSeconds(playTime);
            ads.Stop();
            yield return new WaitForSeconds(playInterval);
            isDoing = false;
        }
    }
}

