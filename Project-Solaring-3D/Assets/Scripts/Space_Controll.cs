using UnityEngine;
using System.Collections;

public class Space_Controll : MonoBehaviour
{
    #region �Ŷ��ܼ�
    [SerializeField] 
    private bool rotated = false;  //�Ŷ��O�_����
    #endregion


    #region �Ŷ���k
    /// <summary>
    ///  �����ӪŶ� +-90��
    /// </summary>
    public void _Spine()
    {
        float y_loc = Mathf.Floor(transform.rotation.y);
        //if (Mathf.Abs(y_loc) < 3) y_loc = 0;
        y_loc = (y_loc == 0) ?  -90 :  90;
        rotated = !rotated;
        transform.Rotate(0,y_loc,0);

    }
    /// <summary>
    ///  2���y�ЧP�w
    /// </summary>
    public void _location()
    {

    }

    #endregion

    #region �ƥ�Ĳ�o

    private void Awake()
    {
        //���z�Ŷ��Ѽ�
        Physics.gravity = new Vector3(0, -0.1F, 0);
    }

    private void Start()
    {
    }

    #endregion
}
