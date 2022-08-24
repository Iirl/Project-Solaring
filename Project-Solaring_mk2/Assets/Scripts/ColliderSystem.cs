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
    }

    [System.Serializable]
    private class BlockGameObject
    {
        public string label;
        public Genertic species;
        public int plus;

    }
    #region �I���ƥ�
    /// <summary>
    /// �I���ƥ�B�z�W�h�G
    /// �p�G�I��ĤH�N�����A�åB�ͦ��S�ġC
    /// �p�G�I�����N�ˬd�n���檺�ʧ@�C
    /// </summary>
    /// <param name="hitObj"></param>
    static public int CollisionPlayerEvent(GameObject hitObj)
    {
        //print(hitObj.name);
        int i = -1;
        if (hitObj.tag.Contains("Enemy"))
        {
            i = 1; //�����C���B�z
            
            ManageCenter.rocket_ctl.SendMessage("ADOClipControl", 1);
            StaticSharp.Conditions = State.End;
            if (collSys) collSys.SendMessage("ExploderEvent", hitObj.name);
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
            print("���I�A���");
        }
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
                    }
                }
            }
            tmpvar = false;
        }
    }
    private void ExploderEvent(string name)
    {
        foreach (var e in effects)
        {
            if (e.label.Contains(name)) { }
        }
    }
    #endregion

    private void Awake()
    {
        collSys = GetComponent<ColliderSystem>();
    }
}
