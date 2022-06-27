using UnityEngine;

/// <summary>
///  此程式是控制玩家相關的操作方法
/// </summary>
public class Rocket_Controll : MonoBehaviour
{

    #region 屬性
    ParticleSystem particle_fire;
    Rigidbody rb_Rocket;
    BoxCollider _boxBorder;
    #endregion
    #region #序列化屬性
    [SerializeField, Header("燃料"),Range(100,200)]
    float fuel=100;
    [SerializeField, Header("移動速度"), Range(8,20)]
    float speed_v=8;
    [SerializeField, Header("移動加速度"), Range(0.2f,1.5f)]
    float speed_a=0.2f;
    #endregion

    #region 方法
    private void UpForce()
    {
        if (particle_fire.isPlaying) rb_Rocket.AddForce(Vector3.up * Time.deltaTime * 5f);
    }
    private void MoveControll(float horizon, float vertial)
    {
        float aSpeed = ( Input.GetKeyDown(KeyCode.F)) ? speed_a : 0;
        if (Mathf.Abs(horizon) < 0.005f) horizon = 0;
        if (Mathf.Abs(vertial) < 0.005f) vertial = 0;
        float xSpeed = (speed_v * horizon) * Time.deltaTime;
        float ySpeed = (speed_v * vertial) * Time.deltaTime;
        Vector3 v3 = new Vector3(xSpeed , ySpeed, 0);
        rb_Rocket.velocity += v3;


    }
    private void VisiableWall()
    {
    }

    /// <summary>
    /// 程式啟動時或者調整大小時要呼叫此函數調整邊界。
    /// </summary>
    private void Box_border()
    {
        float w = _boxBorder.GetComponentInChildren<Camera>().aspect * 10;     //width
        float h = (1 / _boxBorder.GetComponentInChildren<Camera>().aspect) * w; //heigh
        _boxBorder.size = new Vector2(w, h);

    }
    #endregion

    #region 事件
    private void Awake()
    {
        particle_fire = GetComponentInChildren<ParticleSystem>();
        rb_Rocket = GetComponent<Rigidbody>();
        _boxBorder = GameObject.Find("Border").GetComponent<BoxCollider>();
        Box_border();
    }
    void Start()
    {
        
    }
    void Update()
    {
        MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //VisiableWall();
    }
    private void FixedUpdate()
    {
        UpForce();
    }
    private void OnTriggerExit(Collider other)
    {
        print(other.gameObject.name);

    }
    #endregion
    #region ##
    #endregion
}
