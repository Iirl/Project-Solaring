using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class imageFloat : MonoBehaviour
{
    RectTransform rect;
    [SerializeField, Header("循環播放")]
    private bool recycleHeight;
    [SerializeField]
    private bool recycleWidth;
    private bool isRecycle;
    [SerializeField, Header("縮放大小")]
    private float height = 1;
    [SerializeField]
    private float width = 1;
    [SerializeField, Header("縮放時間"),Range(0.01f,1)]
    private float flashTime = 0.02f;
    //
    private IEnumerator ShakeEffect()
    {
        if (isRecycle) yield break;
        float fh;
        float fw;
        isRecycle = true;
        while (isRecycle)
        {
            fh = recycleHeight ? UnityEngine.Random.value * height :1;
            fw = recycleWidth ? UnityEngine.Random.value * width : 1;
            //rect.transform.localScale = new Vector3(fw, fh, 1);
            rect.transform.localScale = Vector3.Lerp(rect.transform.localScale, new Vector3(fw, fh, 1), 10f);
            yield return new WaitForSeconds(flashTime);
        }
        isRecycle = false;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (!isRecycle) StartCoroutine(ShakeEffect());
    }
}
