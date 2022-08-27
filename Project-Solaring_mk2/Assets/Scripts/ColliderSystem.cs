using solar_a;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ����I�������t�λP�����t�Τ���C
/// </summary>
public class ColliderSystem : MonoBehaviour
{
    #region �ݩ�
    static ColliderSystem collSys;
    public enum Genertic { Supply, Other }
    #endregion
    [SerializeField, Header("�����S��"), NonReorderable]
    EffectGameObject[] effects;
    [SerializeField, Header("���~�ĪG"), NonReorderable]
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
        [Header("�n������")]
        public AudioClip adClip;
        [Range(0,1)]
        public float adVol;

    }
    #region �I���ƥ�
    static bool loadScene;
    /// <summary>
    /// �I���ƥ�B�z�W�h�G
    /// �p�G�I��ĤH�N�����A�åB�ͦ��S�ġC
    /// �p�G�I�����N�ˬd�n���檺�ʧ@�C
    /// </summary>
    /// <param name="hitObj"></param>
    static public int CollisionPlayerEvent(GameObject hitObj)
    {
        if (!hitObj) return -1;
        //print(hitObj.name);
        int i = 0;
        if (hitObj.tag.Contains("Enemy"))
        {
            i = 1; //�����C���B�z
            if (ManageCenter.space_ctl.isRotate) return -1;
            ManageCenter.rocket_ctl.SendMessage("ADOClipControl", 1);
            StaticSharp.Conditions = State.End;
        }
        else if (hitObj.tag.Contains("Block"))
        {
            i = 2; //����b�I��ɫ~��
            ManageCenter.mgCenter.ObjectDestory(hitObj, false);
            if (collSys) collSys.SendMessage("BolckEvent", hitObj.name);
        }
        else if (hitObj.tag.Contains("Respawn"))
        {
            i = 3; // �I��Ӫů�
            ManageCenter.mgCenter.InToStation();
        }
        else if (hitObj.tag.Contains("Finish"))
        {
            i = 4; // �i�J���I
            if(!loadScene)
            {
                loadScene = true;
                StaticSharp.isChangeScene = true;
                print("���I�A���");
            }
        }
        if (collSys) collSys.SendMessage("ExploderEvent", hitObj);
        return i;
    }
    static public void StageColliderEvent(GameObject hitObj, bool hasDest=true)
    {
        //print("������tĲ�o");
        if (!hitObj.tag.Contains("Player"))ManageCenter.mgCenter.ObjectDestory(hitObj, hasDest);
    }
    /// <summary>
    /// ����I���ƥ�
    /// </summary>
    /// <param name="name"></param>
    private void BolckEvent(string name)
    {
        if (name == "") return;
        lock (locker)
        {
            if (!tmpvar)
            {
                tmpvar = true;
                //print($"�d�߬O�_�ǰt {name}");
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
    /// <summary>
    /// �I�������z���S�Ĩƥ�
    /// </summary>
    /// <param name="gob">�o�͸I������m</param>
    private void ExploderEvent(GameObject gob)
    {
        if (!gob) return;
        foreach (var e in effects)
        {
            if (gob.name.ToLower().Contains(e.label.ToLower())) {
                print($"�z���S�� {e.label} ����");
                Destroy(Instantiate(e.ExplodeObj,transform),e.ExplodeTime);
            }
        }
    }
    /// <summary>
    /// ����ƥ�
    /// </summary>
    //private void SceneChageEvent() => ManageCenter.StartC
    #endregion

    private void Awake()
    {
        collSys = GetComponent<ColliderSystem>();
    }
}
