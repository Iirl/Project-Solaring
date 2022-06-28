using UnityEngine;

/// <summary>
/// 此為在區域空間中產生物件的程式
/// </summary>
public class Object_Generator : MonoBehaviour
{

    #region 屬性
    Object targetOb;
    GameObject parentGob, targetGob;
    #endregion

    #region 固定物件方法
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
        {  //如果子物件已砍完就不再刪除
            targetGob = parentGob.transform.GetChild(0).gameObject;
            Destroy(targetGob);
        }
    }
    #endregion

    #region 隨機產生方法

    #endregion

    #region 補給品方法

    #endregion

    #region 事件

    #endregion
}
