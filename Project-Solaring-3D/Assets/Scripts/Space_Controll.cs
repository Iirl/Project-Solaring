using UnityEngine;
using System.Collections;

/// <summary>
/// ���{����b����s�շ��A�Ψӽվ㪫��Ŷ������ʡC
/// </summary>
public class Space_Controll : MonoBehaviour
{
    #region �Ŷ��ܼ�
    static bool rotated, rot_left, rot_right;  //�Ŷ��O�_����
    [SerializeField, Header("���I")]
    private float rot_angle=0;
    [SerializeField, Header("����V����(L)"), Range(180,359)]
    private float Left_angle = 315;
    [SerializeField, Header("�k��V����(R)"), Range(0,179)]
    private float Right_angle = 45;
    #endregion


    #region �Ŷ���k
    /// <summary>
    ///  �����ӪŶ� +-X��
    /// </summary>
    /// �Ѽƽбq�ݩʭ��O�վ�A����V
    public void _Spine()
    {
        float y_loc = Mathf.Floor(transform.eulerAngles.y);
        float spine = 2f; //����t��
        if (y_loc >= Right_angle && y_loc < Right_angle + spine *2 ) rot_right = false;
        if (y_loc >= Left_angle && y_loc < Left_angle + spine*2) rot_left = false;
        if (rotated)
        {
            if (rot_left) transform.Rotate(eulers: Vector3.down * spine);
            else if (rot_right) transform.Rotate(eulers: Vector3.up * spine);
            // ���n�I�A����׼Ʒ����M�k��׼Ʒ����έ��I�C
            if (y_loc > rot_angle - spine && y_loc < rot_angle + spine) rotated = false;
            else if (y_loc == Right_angle) rotated = false;
            else if (y_loc == Left_angle) rotated = false;
        }
        //print($"Name:= {y_loc}  / rot{rot_angle}");
    }
    public void _Spine(int vect)
    {
        // 1 �k��  -1 ����
        if (vect != 0)
        {
            rotated = true;
            if (vect == 1)
            {
                rot_right = true;
                rot_left = false;
            }
            else if (vect == -1)
            {
                rot_left = true;
                rot_right = false;
            }
        }
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
        rotated = false; rot_left = false; rot_right = false;
        //���z�Ŷ��Ѽ�
        Physics.gravity = new Vector3(0, -0.1F, 0);
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        // ����Ŷ�
        if (Input.GetAxisRaw("Left_Spine") == 1) _Spine(-1);
        else if (Input.GetAxisRaw("Right_Spine") == 1) _Spine(1);
        _Spine();
    }
    #endregion
}
