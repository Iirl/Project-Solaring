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
    EffectGameObject[] effect;
    [SerializeField, Header("���~�ĪG"), NonReorderable]
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
    #region �I���ƥ�
    /// <summary>
    /// �I���ƥ�B�z�W�h�G
    /// �p�G�I��ĤH�N�����A�åB�ͦ��S�ġC
    /// �p�G�I�����N�ˬd�n���檺�ʧ@�C
    /// </summary>
    /// <param name="hitObj"></param>
    static public int CollisionPlayerEvent(GameObject hitObj)
    {
        int i = -1;
        if (hitObj.tag.Contains("Enemy"))
        {
            i = 1;
            //�����C���B�z
            ManageCenter.rocket_ctl.SendMessage("ADOClipControl", 1);
            StaticSharp.Conditions = State.End;
        }
        else if (hitObj.tag.Contains("Block"))
        {
            i = 2;
            ManageCenter.mgCenter.ObjectDestory(hitObj,false);
            if (collSys) collSys.SendMessage("BolckEvent", hitObj.name);            
            //����b�I��ɫ~��
            //print($"����^�_�~ {hitObj.name}");
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
            print("���I�A���");
        }
        return i;
    }
    /// <summary>
    /// ����I���ƥ�
    /// </summary>
    /// <param name="name"></param>
    private void BolckEvent(string name)
    {
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
    #endregion

    private void Awake()
    {
        collSys = GetComponent<ColliderSystem>();
    }
}
