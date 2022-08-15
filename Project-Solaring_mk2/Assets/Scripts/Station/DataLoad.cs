using System;
using System.Collections;
using System.Collections.Generic;
using solar_a;
using UnityEngine;
using TMPro;

/// <summary>
/// �ӪŲ�ƭ��ܤƨt�ΡC
/// The space boat's value will changed in the station.
/// </summary>
public class DataLoad : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Value_engery, Value_speed, Value_acceler;
    [SerializeField, Header("���t�I��")]
    private int point = 3;
    private int pointUsed = 0;
    [SerializeField, Header("���驳���Х�"), Tooltip("�U�Ʃ���")]
    private float fuelLimit = 100;
    [SerializeField, Tooltip("�t�ש���")]
    private float speedLimit = 1;
    [SerializeField, Tooltip("�[�t�ש���")]
    private float aspeedLimit = 0.1f;
    [SerializeField, Header("�t�m�C��")]
    private Color nor_color = Color.green;
    [SerializeField, Header("��l�C��")]
    private Color alert_color = Color.yellow;
    [SerializeField, Header("ĵ�ܭ�")]
    private Vector3 alert_fva;
    private Vector3 local_info;

    #region �ƥ�
    private void Awake()
    {
        local_info = StaticSharp.Rocket_BASIC;
    }

    private void Start()
    {
        LimitChange();  // �۰�Ū������
        ChangeRocketInfo(Vector3.zero);
    }
    #endregion
    public void SaveChanged()
    {
        StaticSharp.Rocket_BASIC = local_info;
    }
    /// <summary>
    /// ���O�ƭ��ܰʵ{��
    /// </summary>
    /// <param name="v">��J�T���V�q</param>
    /// <param name="i">��J�ܤƶq</param>
    public void ChangeRocketInfo(Vector3 v, int i=0)
    {
        bool plus = i > 0 ? true : false;
        //print($"{plus} & {point}"); // �ˬd�ƭȬO�_�����D
        print("A");
        if (plus && point == 0) return;
        else if (i < 0 && pointUsed == 0) return;
        else if (local_info.x * local_info.y * local_info.z <= 0 && !plus) return;
        print("B");
        local_info += v;
        if (plus) { point--; pointUsed++; }
        else if (i < 0) { point++; pointUsed--; }
        Value_engery.text = local_info.x.ToString();
        Value_speed.text = local_info.y.ToString();
        Value_acceler.text = local_info.z.ToString();
        if (v == Vector3.zero) return;
        // TextColor Change
        if (float.Parse(Value_engery.text) <= alert_fva.x) Value_engery.color = alert_color;
        else Value_engery.color = nor_color;
        if (float.Parse(Value_speed.text) <= alert_fva.y) Value_speed.color = alert_color;
        else Value_speed.color = nor_color;
        if (float.Parse(Value_acceler.text) <= alert_fva.z) Value_acceler.color = alert_color;
        else Value_acceler.color = nor_color;
    }

    public void BtnFuel(int count)
    {
        Vector3 v3 = Vector3.zero;
        if (local_info.x + count < fuelLimit) return;
        v3.x += count;
        ChangeRocketInfo(v3, count);
    }
    public void BtnSpd(int count)
    {
        Vector3 v3 = Vector3.zero;
        if (local_info.y + count < speedLimit) return;
        v3.y += count;
        ChangeRocketInfo(v3, count);
    }
    public void BtnAce(int count)
    {
        Vector3 v3 = Vector3.zero;
        if (local_info.z + count < aspeedLimit) return;
        v3.z += count;
        ChangeRocketInfo(v3, count);
    }

    private void LimitChange()
    {
        fuelLimit = local_info.x;
        speedLimit = local_info.y;
        aspeedLimit = local_info.z;
        alert_fva = new Vector3(fuelLimit, speedLimit, aspeedLimit);
    }
}
