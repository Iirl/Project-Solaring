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
    [SerializeField, Header("�U��"), Range(100, 200)]
    float fuel = 100;
    [SerializeField, Header("���ʳt��"), Range(2.0f, 12f)]
    float speed_v = 4f;
    [SerializeField, Header("���ʥ[�t��"), Range(0.2f, 1.5f)]
    float speed_a = 0.2f;
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
    private void MoveControll(float horizon, float vertial, bool boost)
    {
        if (Mathf.Abs(horizon) < 0.005f) horizon = 0;
        if (Mathf.Abs(vertial) < 0.005f) vertial = 0;
        float xSpeed = (speed_v * horizon) * Time.deltaTime;
        float ySpeed = (speed_v * vertial) * Time.deltaTime;
        float aSpeed = speed_a * 1000;
        // ���ʱ���
        Vector3 v3 = new Vector3(xSpeed, ySpeed, 0);
        rb_Rocket.velocity += v3;
        // �[�t�ױ���
        Vector3 r3 = new Vector3(horizon * aSpeed, vertial * aSpeed, 0);
        if (boost) rb_Rocket.AddForce(r3);
        // ½��^�_
        rb_Rocket.transform.Rotate(transform.rotation.x, transform.rotation.y, 0);
        //print($"H:{horizon}; V:{vertial}; Fire{particle_fire.isPlaying}");        
        //print($"H+V:{Mathf.Abs(horizon) + Mathf.Abs(vertial)};");
        // ���K����
        if (particle_fire != null)
        {
            float xFire = Mathf.Abs(Mathf.Pow(1f, horizon)) - 0.5f;
            float yFire = 0;
            if (vertial > 0) yFire = Mathf.Pow(4, vertial) - 1;
            else if (vertial < 0) yFire = Mathf.Abs(Mathf.Pow(1, vertial));
            if (boost) yFire += 1;
            particle_fire.transform.localScale = new Vector2(xFire, yFire);
            if (particle_fire.transform.localScale.x < 0.6f && particle_fire.transform.localScale.y < 1) ignix_fire(false);
            else ignix_fire(true);
        }

        //print(particle_fire.isPlaying);

    }
    /// <summary>
    ///  �I������
    /// </summary>
    /// <param name="isFire">�P�_�O�_�Ұʤ���</param>
    private void ignix_fire(bool isFire)
    {
        if (isFire)
        {
            //particle_fire.gameObject.SetActive(true);
            particle_fire.Play();
        }
        else
        {
            //particle_fire.gameObject.SetActive(false);
            particle_fire.Pause();
        }
    }

    #endregion

    #region �ƥ�
    private void Awake()
    {
        particle_fire = GetComponentInChildren<ParticleSystem>();
        rb_Rocket = GetComponent<Rigidbody>();
        ignix_fire(true);
    }
    void Start()
    {

    }
    /// <summary>
    /// ��s�ƥ�
    /// </summary>
    void Update()
    {
        MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetKey(KeyCode.Space));

    }
    private void FixedUpdate()
    {
        UpForce();
    }

    #endregion
    #region ##
    #endregion
}
