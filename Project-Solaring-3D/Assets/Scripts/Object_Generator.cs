using UnityEngine;

/// <summary>
/// �����b�ϰ�Ŷ������ͪ��󪺵{��
/// </summary>
public class Object_Generator : MonoBehaviour
{

    #region �ݩ�
    Object targetOb;
    GameObject parentGob, targetGob;
    #endregion

    #region �T�w�����k
    public void generateOb()
    {
        targetOb = Resources.Load("Grass");
        parentGob = GameObject.Find("GameObject");
        Instantiate(
            targetOb,
            parentGob.transform.position,
            parentGob.transform.rotation,
            parentGob.transform
       );
    }

    public void destoryOb()
    {
        parentGob = GameObject.Find("GameObject");
        if (parentGob.transform.childCount != 0)
        {  //�p�G�l����w�姹�N���A�R��
            targetGob = parentGob.transform.GetChild(0).gameObject;
            Destroy(targetGob);
        }
    }
    #endregion

    #region �H�����ͤ�k

    #endregion

    #region �ɵ��~��k

    #endregion

    #region �ƥ�

    #endregion
}
