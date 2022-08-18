using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }

    private void Update()
    {
        if(!isDoing) StartCoroutine(PlaySound());
        if (StaticSharp.Conditions == (State)4) Close();
    }

    private void Close()
    {
        isDoing = true;
        enabled = false;
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
