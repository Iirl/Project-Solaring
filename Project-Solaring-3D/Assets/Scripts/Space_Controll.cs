using UnityEngine;
using System.Collections;

/// <summary>
/// ���{����b����s�շ��A�Ψӽվ㪫��Ŷ������ʡC
/// </summary>
public class Space_Controll : MonoBehaviour
{
    #region �Ŷ��ܼ�
    static bool rotated, rot_left, rot_right;  //�Ŷ��O�_����
    [SerializeField, Header("����]�w") ,Tooltip("���I")]
    private float rot_angle=0;
    [SerializeField, Tooltip("����V����(L)"), Range(180,359)]
    private float Left_angle = 315;
    [SerializeField, Tooltip("�k��V����(R)"), Range(0,179)]
    private float Right_angle = 45;
    #endregion
    #region ����Ŷ�(Border)�d��
    [SerializeField, Header("����Ŷ��w�q")]
    GameObject SPACE;
    RectTransform Space_RT;
    [SerializeField, Space]
    private float Space_speed = 10;

    #endregion

    #region �Ŷ���k
    /// <summary>
    /// �e������
    /// </summary>
    private void _auto_move()
    {
        if (Space_RT.position.x != 0) Space_RT.position = new Vector2(0,Space_RT.position.y);        
        Space_RT.position += Vector3.up * Time.deltaTime * Space_speed;
    }
    /// <summary>
    ///  �����ӪŶ� +-X��
    /// </summary>
    /// �Ѽƽбq�ݩʭ��O�վ�A����V
    public void _Spine()
    {
        float center_loc = Mathf.Floor(transform.eulerAngles.y);
        //print (transform.localEulerAngles);
        float spine = 2f; //����t��
        if (center_loc >= Right_angle && center_loc < Right_angle + spine * 2) rot_right = false;        
        if (center_loc >= Left_angle && center_loc < Left_angle + spine * 2)   rot_left  = false;

        if (rotated)
        {
            if (rot_left) transform.Rotate(eulers: Vector2.down * spine);
            else if (rot_right) transform.Rotate(eulers: Vector2.up * spine);
            // ���n�I�A����׼Ʒ����M�k��׼Ʒ����έ��I�C
            if (center_loc > rot_angle - spine && center_loc < rot_angle + spine) rotated = false;
            else if (center_loc > Left_angle - spine && center_loc < Left_angle + spine) { rotated = false; }
            else if (center_loc > Right_angle - spine && center_loc < Right_angle + spine) { rotated = false; }
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
    public void _2DSpine()
    {

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
        Space_RT = SPACE.GetComponent<RectTransform>();
        //���z�Ŷ��Ѽ�
        Physics.gravity = new Vector3(0, -0.1F, 0);
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        // ��������
        _auto_move();

        // ����Ŷ�
        if (Input.GetAxisRaw("Left_Spine") == 1) _Spine(-1);
        else if (Input.GetAxisRaw("Right_Spine") == 1) _Spine(1);
        _Spine();

    }
    #endregion
}
