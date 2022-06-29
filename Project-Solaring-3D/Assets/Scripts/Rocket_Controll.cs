using UnityEngine;

/// <summary>
///  此程式是控制玩家相關的操作方法
/// </summary>
public class Rocket_Controll : MonoBehaviour
{

    #region 屬性
    ParticleSystem particle_fire;
    Rigidbody rb_Rocket;
    #endregion
    #region #序列化屬性
    [SerializeField, Header("燃料"),Range(100,200)]
    float fuel=100;
    [SerializeField, Header("移動速度"), Range(0.1f,2f)]
    float speed_v= 0.8f;
    [SerializeField, Header("移動加速度"), Range(0.2f,1.5f)]
    float speed_a=0.2f;
    #endregion

    #region 方法
    private void UpForce()
    {
        if (particle_fire.isPlaying) rb_Rocket.AddForce(Vector3.up * Time.deltaTime * 10f);
    }
    /// <summary>
    /// 移動控制
    /// </summary>
    /// <param name="horizon">水平移動向量</param>
    /// <param name="vertial">垂直移動向量</param>
    private void MoveControll(float horizon, float vertial)
    {
        float aSpeed = ( Input.GetKeyDown(KeyCode.F)) ? speed_a : 0;
        if (Mathf.Abs(horizon) < 0.005f) horizon = 0;
        if (Mathf.Abs(vertial) < 0.005f) vertial = 0;
        float xSpeed = (speed_v * horizon) * Time.deltaTime;
        float ySpeed = (speed_v * vertial) * Time.deltaTime;
        Vector3 v3 = new Vector3(xSpeed , ySpeed, 0);
        rb_Rocket.transform.Rotate(transform.rotation.x, transform.rotation.y, 0);
        rb_Rocket.velocity += v3;

        //print($"H:{horizon}; V:{vertial}; Fire{particle_fire.isPlaying}");        
        //print($"H+V:{Mathf.Abs(horizon) + Mathf.Abs(vertial)};");        
        //if (Mathf.Abs(horizon) == 0 && Mathf.Abs(vertial) == 0) ignix_fire(false);
        particle_fire.transform.localScale = new Vector2(Mathf.Abs(Mathf.Pow(1.2f, horizon))-0.5f, Mathf.Abs(Mathf.Pow(4,vertial)) - 1);
        //else  ignix_fire(true);

    }
    /// <summary>
    ///  點火控制
    /// </summary>
    /// <param name="isFire">判斷是否啟動引擎</param>
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

    #region 事件
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
    /// 更新事件
    /// </summary>
    void Update()
    {
        MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
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
