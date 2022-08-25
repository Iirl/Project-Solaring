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
    EffectGameObject[] effects;
    [SerializeField, Header("物品效果"), NonReorderable]
    BlockGameObject[] blocks;
    private readonly object locker = new object();
    private bool tmpvar;

    [System.Serializable]
    private class EffectGameObject
    {
        public string label;
        public GameObject ExplodeObj;
        public int ExplodeTime=1;
    }

    [System.Serializable]
    private class BlockGameObject
    {
        public string label;
        public Genertic species;
        public int plus;
        [Header("聲音控制")]
        public AudioClip adClip;
        public int adVol;

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
        //print(hitObj.name);
        int i = -1;
        if (hitObj.tag.Contains("Enemy"))
        {
            i = 1; //結束遊戲處理
            if (ManageCenter.space_ctl.isRotate) return -1;
            ManageCenter.rocket_ctl.SendMessage("ADOClipControl", 1);
            StaticSharp.Conditions = State.End;
        }
        else if (hitObj.tag.Contains("Block"))
        {
            i = 2; //當火箭碰到補品時
            ManageCenter.mgCenter.ObjectDestory(hitObj, false);
            if (collSys) collSys.SendMessage("BolckEvent", hitObj.name);
        }
        else if (hitObj.tag.Contains("Respawn"))
        {
            i = 3; // 碰到太空站
            ManageCenter.mgCenter.InToStation();
        }
        else if (hitObj.tag.Contains("Finish"))
        {
            i = 4; // 進入終點
            print("終點，轉場");
        }
        if (collSys) collSys.SendMessage("ExploderEvent", hitObj);
        return i;
    }
    static public void StageColliderEvent(GameObject hitObj, bool hasDest=true)
    {
        //print("場景邊緣觸發");
        if (!hitObj.tag.Contains("Player"))ManageCenter.mgCenter.ObjectDestory(hitObj, hasDest);
    }
    /// <summary>
    /// 方塊碰撞事件
    /// </summary>
    /// <param name="name"></param>
    private void BolckEvent(string name)
    {
        lock (locker)
        {
            if (!tmpvar)
            {
                tmpvar = true;
                //print($"查詢是否匹配 {name}");
                foreach (var block in blocks)
                {
                    if (name.ToLower().Contains(block.label.ToLower()) && block.species == Genertic.Supply)
                    {
                        //print(block.label);
                        ManageCenter.mgCenter.FuelReplen(block.plus);
                        if (block.adClip) ManageCenter.rocket_ctl.ADOClipControl(block.adClip,block.adVol);
                    }
                }
            }
            tmpvar = false;
        }
    }
    private void ExploderEvent(GameObject gob)
    {
        foreach (var e in effects)
        {
            if (gob.name.ToLower().Contains(e.label.ToLower())) {
                print($"爆炸特效 {e.label} 產生");
                Destroy(Instantiate(e.ExplodeObj,transform),e.ExplodeTime);
            }
        }
    }
    #endregion

    private void Awake()
    {
        collSys = GetComponent<ColliderSystem>();
    }
}
