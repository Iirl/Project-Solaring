using UnityEngine;

/// <summary>
///  ���{���O����a�������ާ@��k
/// </summary>
public class Rocket_Controll : MonoBehaviour
{

    #region �ݩ�
    ParticleSystem particle_fire;
    Rigidbody rb_Rocket;
    #endregion
    #region #�ǦC���ݩ�
    [SerializeField, Header("�U��"),Range(100,200)]
    float fuel=100;
    [SerializeField, Header("���ʳt��"), Range(8,20)]
    float speed_v=8;
    [SerializeField, Header("���ʥ[�t��"), Range(0.2f,1.5f)]
    float speed_a=0.2f;
    #endregion

    #region ��k
    private void UpForce()
    {
        if (particle_fire.isPlaying) rb_Rocket.AddForce(Vector3.up * Time.deltaTime * 10f);
    }
    /// <summary>
    /// ���ʱ���
    /// </summary>
    /// <param name="horizon">�������ʦV�q</param>
    /// <param name="vertial">�������ʦV�q</param>
    private void MoveControll(float horizon, float vertial)
    {
        float aSpeed = ( Input.GetKeyDown(KeyCode.F)) ? speed_a : 0;
        if (Mathf.Abs(horizon) < 0.005f) horizon = 0;
        if (Mathf.Abs(vertial) < 0.005f || !particle_fire.isPlaying) vertial = 0;
        float xSpeed = (speed_v * horizon) * Time.deltaTime;
        float ySpeed = (speed_v * vertial) * Time.deltaTime;
        Vector3 v3 = new Vector3(xSpeed , ySpeed, 0);
        rb_Rocket.velocity += v3;


    }
    /// <summary>
    ///  �I������
    /// </summary>
    /// <param name="isFire">�P�_�O�_�Ұʤ���</param>
    private void ignix_fire(bool isFire)
    {
        if (isFire) particle_fire.Stop();
        else particle_fire.Play();
    }
    private void VisiableWall()
    {
    }

    #endregion

    #region �ƥ�
    private void Awake()
    {
        particle_fire = GetComponentInChildren<ParticleSystem>();
        rb_Rocket = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }
    /// <summary>
    /// ��s�ƥ�
    /// </summary>
    void Update()
    {
        MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.Space)) ignix_fire(particle_fire.isPlaying);
        //VisiableWall();
    }
    private void FixedUpdate()
    {
        UpForce();
    }

    #endregion
    #region ##
    #endregion
}
