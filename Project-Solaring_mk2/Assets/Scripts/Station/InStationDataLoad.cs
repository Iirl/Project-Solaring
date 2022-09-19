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
public class InStationDataLoad : MonoBehaviour
{
    EffectPlayer efPlayer;
    [SerializeField]
    TextMeshProUGUI Value_engery, Value_speed, Value_acceler, ValuePoint;
    [SerializeField, Header("分配點數")]
    private int point = 3;
    private int pointUsed = 0;
    [SerializeField, Header("機體底限標示"), Tooltip("是否自動限制")]
    private bool autoLimit;
    [SerializeField, Tooltip("燃料底限")]
    private float fuelLimit = 100;
    [SerializeField, Tooltip("速度底限")]
    private float speedLimit = 1;
    [SerializeField, Tooltip("加速度底限")]
    private float aspeedLimit = 0.1f;
    [SerializeField, Header("原始顏色")]
    private Color nor_color = Color.green;
    [SerializeField, Header("配置顏色")]
    private Color distrubtion_color = Color.yellow;
    [SerializeField, Header("警示顏色")]
    private Color alert_color = Color.red;
    [SerializeField, Header("警示值")]
    private Vector3 alert_fva;
    private Vector3 local_info;

    #region 事件
    private void Awake()
    {
        efPlayer = transform.Find("AudioBox").GetComponent<EffectPlayer>();
        // 資料讀取處理
        local_info = StaticSharp.Rocket_BASIC;
        StaticSharp.Rocket_INFO.x = 100;
        ValuePoint.text = point.ToString();
    }

    private void Start()
    {
        if (autoLimit) LimitChange();  // 自動讀取限制
        ShowRocketInfo();
    }
    private void Update()
    {
        // 按鍵輸入偵測
        if (Input.GetKey(KeyCode.LeftShift)) if (Input.GetKeyUp(KeyCode.P)) PointPlus(1);
    }
    #endregion

    public void BtnFuel(float count) => OnBtnValue(0, local_info.x, count);
    public void BtnSpd(float count) => OnBtnValue(1, local_info.y, count);
    public void BtnAce(float count) => OnBtnValue(2, local_info.z, count);
    public void PointPlus(int plus) => OnPointValue(plus);
    #region 處理方法
    public void SaveChanged()
    {
        efPlayer.setNumber = 0;
        efPlayer.enabled = true;
        StaticSharp.Rocket_BASIC = local_info;
    }
    /// <summary>
    /// 面板數值變動程式
    /// </summary>
    /// <param name="v">輸入三維向量</param>
    /// <param name="i">輸入變化量</param>
    public void ChangeRocketInfo(Vector3 v, float i = 0)
    {
        bool? plus = i > 0 ? true : i<0 ?false:null;
        //print($"{plus} & {point}"); // 檢查數值是否有問題
        if (plus == null) return;
        if (plus == true) if(point == 0) return;
        else if (i < 0 && pointUsed == 0) return;
        //else if (local_info.x * local_info.y * local_info.z <= 0 && !plus) return;
        local_info += v;
        OnPointValue((bool)plus);
        ShowRocketInfo();
    }
    public void ShowRocketInfo()
    {
        Value_engery.text = local_info.x.ToString();
        Value_speed.text = local_info.y.ToString();
        Value_acceler.text = local_info.z.ToString();
        // TextColor Change
        switch (local_info.x.CompareTo(StaticSharp.Rocket_BASIC.x))
        {
            case 0: //相等，原始顏色
                Value_engery.color = nor_color;
                break;
            case 1: //增值，配置顏色
                Value_engery.color = distrubtion_color;
                break;
            case -1://低於基本值，警示顏色
                Value_engery.color = alert_color;
                break;
        }
        switch (local_info.y.CompareTo(StaticSharp.Rocket_BASIC.y))
        {
            case 0: //相等，原始顏色
                Value_speed.color = nor_color;
                break;
            case 1: //增值，配置顏色
                Value_speed.color = distrubtion_color;
                break;
            case -1://低於基本值，警示顏色
                Value_speed.color = alert_color;
                break;
        }
        switch (local_info.z.CompareTo(StaticSharp.Rocket_BASIC.z))
        {
            case 0: //相等，原始顏色
                Value_acceler.color = nor_color;
                break;
            case 1: //增值，配置顏色
                Value_acceler.color = distrubtion_color;
                break;
            case -1://低於基本值，警示顏色
                Value_acceler.color = alert_color;
                break;
        }
        if (local_info.x < alert_fva.x) Value_engery.color = alert_color;
        if (local_info.y < alert_fva.y) Value_speed.color = alert_color;
        if (local_info.z < alert_fva.z) Value_acceler.color = alert_color;

    }
    /// <summary>
    /// 設定調整各項能力的按鈕功能
    /// </summary>
    /// <param name="idx">指定能力種類</param>
    /// <param name="nowInfo">取得現在的數值(預設應填入最大值)</param>
    /// <param name="count">增加的數量</param>
    private void OnBtnValue(int idx, float nowInfo, float count)
    {
        Vector3 v3 = Vector3.zero;
        float limit=0;
        if (idx == 0) limit = fuelLimit;
        else if (idx == 1) limit = speedLimit;
        else if (idx == 2) limit = aspeedLimit;
        if (nowInfo + count < limit) return;
        if (idx == 0) v3.x += count;
        else if (idx == 1) v3.y += count;
        else if (idx == 2) v3.z += count;
        ChangeRocketInfo(v3, count);
        // 音效處理內容
        efPlayer.enabled = false;
        if (count >0) efPlayer.setNumber = 6;
        else efPlayer.setNumber = 7;
        if (efPlayer) if (!efPlayer.enabled) efPlayer.enabled = true;
    }
    private void OnPointValue(int i)
    {
        if (point >= 9) return;
        point += i;
        ValuePoint.text = point.ToString();
    }
    private void OnPointValue(bool plus)
    {
        if (plus) { point--; pointUsed++; }
        else { point++; pointUsed--; }
        ValuePoint.text = point.ToString();
    }
    private void LimitChange()
    {
        fuelLimit = local_info.x;
        speedLimit = local_info.y;
        aspeedLimit = local_info.z;
        alert_fva = new Vector3(fuelLimit, speedLimit, aspeedLimit);
    }
    #endregion
}
