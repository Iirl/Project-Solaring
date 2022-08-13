using System;
using System.Collections;
using System.Collections.Generic;
using solar_a;
using UnityEngine;
using TMPro;

/// <summary>
/// 太空船數值變化系統。
/// The space boat's value will changed in the station.
/// </summary>
public class DataLoad : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Value_engery, Value_speed, Value_acceler;
    [SerializeField, Header("分配點數")]
    private int point = 3;
    [SerializeField, Header("正常顏色")]
    private Color nor_color = Color.yellow;
    [SerializeField, Header("警示顏色")]
    private Color alert_color = Color.red;
    [SerializeField, Header("警示值")]
    private Vector3 alert_fva;
    private Vector3 local_info;

    #region 事件
    private void Awake()
    {
        local_info = StaticSharp.Rocket_BASIC;
    }

    private void Start()
    {
        ChangeRocketInfo(Vector3.zero);
    }
    #endregion
    public void SaveChanged()
    {
        StaticSharp.Rocket_BASIC = local_info;
    }
    /// <summary>
    /// 面板數值變動程式
    /// </summary>
    /// <param name="v">輸入三維向量</param>
    /// <param name="i">輸入變化量</param>
    public void ChangeRocketInfo(Vector3 v, int i=0)
    {
        bool plus = i > 0 ? true : false;
        //print($"{plus} & {point}"); // 檢查數值是否有問題
        if (plus && point == 0) return;
        else if (local_info.x * local_info.y * local_info.z <= 0 && !plus) return;
        local_info += v;
        if (plus) point--;
        else point++;
        Value_engery.text = local_info.x.ToString();
        Value_speed.text = local_info.y.ToString();
        Value_acceler.text = local_info.z.ToString();
        if (v == Vector3.zero) return;
        // TextColor Change
        if (float.Parse(Value_engery.text) < alert_fva.x) Value_engery.color = alert_color;
        else Value_engery.color = nor_color;
        if (float.Parse(Value_speed.text) < alert_fva.y) Value_speed.color = alert_color;
        else Value_speed.color = nor_color;
        if (float.Parse(Value_acceler.text) < alert_fva.z) Value_acceler.color = alert_color;
        else Value_acceler.color = nor_color;
    }

    public void BtnFuel(int count)
    {
        Vector3 v3 = Vector3.zero;
        v3.x += count;
        ChangeRocketInfo(v3, count);
    }
    public void BtnSpd(int count)
    {
        Vector3 v3 = Vector3.zero;
        v3.y += count;
        ChangeRocketInfo(v3, count);
    }
    public void BtnAce(int count)
    {
        Vector3 v3 = Vector3.zero;
        v3.z += count;
        ChangeRocketInfo(v3, count);
    }
}
