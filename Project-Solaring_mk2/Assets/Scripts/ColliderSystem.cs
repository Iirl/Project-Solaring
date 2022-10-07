using solar_a;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 全域碰撞偵測系統與中央系統元件。
/// </summary>
public class ColliderSystem : MonoBehaviour
{
    #region 屬性
	static ManageCenter mgc;
	static ManageDisco mgDsco;
	static Space_Controll spc;
    static ColliderSystem collSys;
    public enum Genertic { Supply, Protect, UnlimitFuel, UnlimitRush, Other }
    #endregion
    [SerializeField, Header("撞擊特效"), NonReorderable]
    EffectGameObject[] effects;
    [SerializeField, Header("物品效果")]
    BlockGameObject[] blocks;
    //
    private bool tmpvar;

    [System.Serializable]
    private class EffectGameObject
    {
        public string label;
        public GameObject ExplodeObj;
        public int ExplodeTime = 1;
    }

    [System.Serializable]
    private class BlockGameObject
    {
        public string label;
        public Genertic species;
        public int plus;
        [Header("聲音控制")]
        public AudioClip adClip;
        [Range(0, 1)]
        public float adVol;

    }
    #region 碰撞事件
    static bool loadScene;
    /// <summary>
    /// 碰撞事件處理規則：
    /// 如果碰到敵人就消滅，並且生成特效。
    /// 如果碰到方塊就檢查要執行的動作。
    /// </summary>
    /// <param name="hitObj"></param>
    static public int CollisionPlayerEvent(GameObject hitObj)
	{
		if (!mgc || !mgDsco || !spc)GetCMP();
        if (!hitObj) return -1;
        //print(hitObj.name);
        int i = 0;
        if (hitObj.tag.Contains("Enemy"))
        {
            if (spc.isRotate) return -1;
            i = 1; //結束遊戲處理
	        StaticSharp._SecretSCORE++;
            mgDsco.OneShotEffect(ManageCenter.rocket_ctl.rocket_Clip[1]);
            mgc.ObjectDestory(hitObj, false);
	        if (!mgc.protect) StaticSharp.Conditions = State.End;
            //print(hitObj.name);
            //Destroy(hitObj);
        }
        else if (hitObj.tag.Contains("Block"))
        {
            i = 2; //當火箭碰到補品時
            if (collSys) collSys.SendMessage("BolckEvent", hitObj.name);
            mgc.ObjectDestory(hitObj, false);
        }
        else if (hitObj.tag.Contains("Respawn"))
        {
            i = 3; // 碰到太空站
            mgc.InToStation();
        }
        else if (hitObj.tag.Contains("Finish"))
        {
            i = 4; // 進入終點
            if (!loadScene)
            {
                loadScene = true;
                StaticSharp.isChangeScene = true;
                print("終點，轉場");
                loadScene = false;
            }
        }
        else if (hitObj.tag.Contains("Sticky"))
        {
            i = 5; // 減速、黏酌
        }

        if (collSys) collSys.SendMessage("ExploderEvent", hitObj);
        return i;
    }
    /// <summary>
    /// 方塊碰撞事件
    /// </summary>
    /// <param name="name"></param>
    private void BolckEvent(string name)
	{
        if (!tmpvar && name.Length > 0)
        {
            tmpvar = true;
            //print($"查詢是否匹配 {name}");
            foreach (var block in blocks)
            {
                if (name.ToLower().Contains(block.label.ToLower()))
                {
                    switch (block.species)
                    {
                        case Genertic.Supply:
                            ManageCenter.mgCenter.FuelReplen(block.plus);
                            break;
                        case Genertic.Protect:
                            //print("保護效果");
                            ManageCenter.rocket_SSR.StateControaller(0, block.plus);
                            break;
                        case Genertic.UnlimitFuel:
                            ManageCenter.rocket_SSR.StateControaller(1, block.plus);
                            break;
                        case Genertic.UnlimitRush:
                            ManageCenter.rocket_SSR.StateControaller(2, block.plus);
                            break;
                        case Genertic.Other:
                            break;
                        default:
                            break;
                    }
                    //print(block.label);
                    if (block.adClip && ManageCenter.mgDsko)
                    {
                        ManageCenter.mgDsko.EffectVolumeAdjust(block.adVol);
                        ManageCenter.mgDsko.OneShotEffect(block.adClip);
                    }
                    break;
                }
            }
        }
        tmpvar = false;

    }
    /// <summary>
    /// 碰撞產生爆炸特效事件
    /// </summary>
    /// <param name="gob">發生碰撞的位置</param>
    private void ExploderEvent(GameObject gob)
    {
        if (!gob) return;
        foreach (var e in effects)
        {
            if (gob.name.ToLower().Contains(e.label.ToLower()))
            {
                //print($"爆炸特效 {e.label} 產生");
                Destroy(Instantiate(e.ExplodeObj, transform), e.ExplodeTime);
            }
        }
    }
    /// <summary>
    /// 轉場事件:當中控器不存在時使用
    /// </summary>
    #endregion
	static private void GetCMP(){
		mgc = ManageCenter.mgCenter ? ManageCenter.mgCenter : FindObjectOfType<ManageCenter>();
		mgDsco =  ManageCenter.mgDsko? ManageCenter.mgDsko : FindObjectOfType<ManageDisco>();
		spc = ManageCenter.space_ctl ? ManageCenter.space_ctl : FindObjectOfType<Space_Controll>();
	}
    private void Awake()
    {
        collSys = GetComponent<ColliderSystem>();
    }
}
