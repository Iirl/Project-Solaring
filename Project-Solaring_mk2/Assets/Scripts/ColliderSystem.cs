using solar_a;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 全域碰撞偵測系統與中央系統元件。
/// </summary>
public class ColliderSystem : MonoBehaviour
{
    #region 屬性
    static ColliderSystem collSys;
    public enum Genertic { Supply, Other }
    #endregion
    [SerializeField, Header("撞擊特效"), NonReorderable]
    EffectGameObject[] effect;
    [SerializeField, Header("物品效果"), NonReorderable]
    BlockGameObject[] blocks;

    [System.Serializable]
    private class EffectGameObject
    {
        public string label;
        public GameObject ExplodeObj;
    }

    [System.Serializable]
    private class BlockGameObject
    {
        public string label;
        public Genertic species;
        public int plus;

    }
    #region 碰撞事件
    /// <summary>
    /// 碰撞事件處理規則：
    /// 如果碰到敵人就消滅，並且生成特效。
    /// 如果碰到方塊就檢查要執行的動作。
    /// </summary>
    /// <param name="hitObj"></param>
    static public int CollisionPlayerEvent(GameObject hitObj)
    {
        int i = -1;
        if (hitObj.tag.Contains("Enemy"))
        {
            i = 1;
            //結束遊戲處理
            ManageCenter.rocket_ctl.SendMessage("ADOClipControl", 1);
            StaticSharp.Conditions = State.End;
        }
        else if (hitObj.tag.Contains("Block"))
        {
            i = 2;
            ManageCenter.mgCenter.ObjectDestory(hitObj,false);
            if (collSys) collSys.SendMessage("BolckEvent", hitObj.name);            
            //當火箭碰到補品時
            //print($"收到回復品 {hitObj.name}");
            /*
            int addFuel = 0;
            if (hitObj.name.Contains("Box") || hitObj.name.Contains("box")) addFuel = 10;
            if (hitObj.name.Contains("Bottle")) addFuel = 5;
            ManageCenter.mgCenter.FuelReplen(addFuel);*/
        }
        else if (hitObj.tag.Contains("Respawn"))
        {
            i = 3;
            ManageCenter.mgCenter.InToStation();
        }
        else if (hitObj.tag.Contains("Finish"))
        {
            i = 4;
            print("終點，轉場");
        }
        return i;
    }
    /// <summary>
    /// 方塊碰撞事件
    /// </summary>
    /// <param name="name"></param>
    private void BolckEvent(string name)
    {
        //print($"查詢是否匹配 {name}");
        foreach (var block in blocks)
        {
            if (name.ToLower().Contains(block.label.ToLower()) && block.species == Genertic.Supply)
            {
                //print(block.label);
                ManageCenter.mgCenter.FuelReplen(block.plus);
            }
        }
    }
    #endregion

    private void Awake()
    {
        collSys = GetComponent<ColliderSystem>();
    }
}
