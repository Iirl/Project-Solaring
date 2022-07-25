using UnityEngine;

/// <summary>
///  此程式是控制玩家相關的操作方法，建議放在玩家物件上。
/// </summary>
namespace solar_a
{
    public class Rocket_Controll : MonoBehaviour
    {

        #region 屬性
        [SerializeField, Header("中控系統")]
        ManageCenter mgCenter;
        #endregion
        #region #序列化屬性
        [SerializeField, Header("燃料"), Range(50, 200), Tooltip("每單位消耗0.25燃料")]
        float fuel = 100;
        [SerializeField, Header("移動速度"), Range(2.0f, 12f)]
        float speed_v = 4f;
        float orgspd_v = 4f;
        [SerializeField, Header("移動加速度"), Range(0.2f, 1.5f)]
        float speed_a = 0.2f;
        float orgspd_a = 0.2f;
        public Vector3 RocketS1 { get { return new Vector3(fuel, speed_v, speed_a); } }
        [SerializeField, Header("火焰控制項")]
        Vector2 fireLenght_min = new Vector2(0.5f, 1f);
        [SerializeField, Tooltip("盡量不要超過預設值太多")]
        Vector2 fireLenght_max = new Vector2(1f, 3f), fireBoost = new Vector2(0f, 1f);
        [SerializeField, Header("火焰最大音量"), Range(0.1f, 1f)]
        float fire_volume = 0.6f;
        [SerializeField, Header("火箭大小")]
        Vector3 rocketBox = new Vector3(1f, 3f, 0);
        [SerializeField, Tooltip("火箭位移")]
        Vector3 rocketOffset = new Vector3(0, -1f, 0);
        [SerializeField, Tooltip("火箭顏色")]
        Color rocketColor = Color.white;
        [SerializeField, Header("更新控制項")]
        public bool isControl = true;
        //
        ParticleSystem particle_fire;
        Rigidbody Rocket_Rig;
        AudioSource Rocket_sound;
        private bool isMove;
        #endregion

        #region 公用方法
        /// <summary>
        /// 存取火箭資訊方法。
        /// </summary>
        /// <returns>x=燃料；y=速度；z=加速度</returns>
        public Vector3 PutRocketSyn(float x, float y = -1, float z = -1)
        {
            fuel -= x;
            speed_v = y >= 0 ? y : speed_v;
            speed_a = z >= 0 ? z : speed_a;
            return RocketS1;
        }
        public float GetBasicSPD()
        {
            return orgspd_v;
        }
        public float GetBasicASPD()
        {
            return orgspd_a;
        }
        /// <summary>
        /// 切換運行狀態：關聲音、定在畫面上以及關閉移動控制。
        /// </summary>
        public bool ControlChange()
        {
            if (Rocket_sound.isPlaying) Rocket_sound.Stop(); else Rocket_sound.Play();
            Rocket_Rig.isKinematic = !Rocket_Rig.isKinematic;
            isControl = !isControl;
            return isControl;
        }
        #endregion

        #region 方法
        private void UpForce()
        {
            if (particle_fire.isPlaying) Rocket_Rig.AddForce(Vector3.up * Time.deltaTime * 10f);
        }
        /// <summary>
        /// 移動檢查
        /// </summary>
        private void MoveCheck()
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) isMove = true;
            else isMove = false;

        }
        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="horizon">水平移動向量</param>
        /// <param name="vertial">垂直移動向量</param>
        private void MoveControll(float horizon, float vertial, bool boost)
        {
            if (Mathf.Abs(horizon) < 0.005f) horizon = 0;
            if (Mathf.Abs(vertial) < 0.005f) vertial = 0;
            float xSpeed = (speed_v * horizon) * Time.deltaTime;
            float ySpeed = (speed_v * vertial) * Time.deltaTime;
            float aSpeed = speed_a * 1000;
            // 移動控制
            Vector3 v3 = new Vector3(xSpeed, ySpeed, 0);
            Rocket_Rig.velocity += v3;
            // 加速度控制
            Vector3 r3 = new Vector3(horizon * aSpeed, vertial * aSpeed, 0);
            if (boost) Rocket_Rig.AddForce(r3);
            // 翻轉回復
            Rocket_Rig.transform.Rotate(transform.rotation.x, transform.rotation.y, 0);
            //print($"H:{horizon}; V:{vertial}; Fire{particle_fire.isPlaying}");        
            //print($"H+V:{Mathf.Abs(horizon) + Mathf.Abs(vertial)};");
            // 火焰控制
            if (particle_fire != null)
            {
                float x_var = Mathf.Abs(horizon);
                float y_var = Mathf.Abs(vertial);
                //// set Fire X axis lenght.橫軸長度
                float xFire = fireLenght_min.x - 0.1f;
                if (horizon != 0) xFire = Mathf.Pow(fireLenght_max.x, x_var);
                //else if (horizon < 0) xFire = Mathf.Pow(fire_min_x, x_var);
                //// set Fire Y axis lenght.縱軸長度
                float yFire = fireLenght_min.y - 0.1f;
                if (vertial > 0) yFire += Mathf.Pow(fireLenght_max.y, y_var);
                else if (vertial < 0) yFire += Mathf.Abs(Mathf.Pow(fireLenght_min.y, y_var));
                //// set Fire Boost.衝刺長度
                if (boost) yFire += fireBoost.y;
                if (boost) xFire += fireBoost.x;
                //// set Horizon Move yFire.
                if (yFire < fireLenght_min.y && x_var > 0) yFire += Mathf.Abs(Mathf.Pow(fireLenght_min.y, x_var));

                particle_fire.transform.localScale = new Vector2(xFire, yFire);
            }

            //print(particle_fire.isPlaying);
        }
        /// <summary>
        ///  點火控制
        /// </summary>
        private void ignix_fire()
        {
            if (isMove)
            {
                particle_fire.Play();
            }
            else
            {
                particle_fire.Pause();
                particle_fire.transform.localScale = new Vector2(1, 1);
                CancelInvoke("ignix_fire");
            }

        }
        /// <summary>
        /// 火箭音效淡入
        /// </summary>
        private void SoundFadeIn()
        {
            float v = Rocket_sound.volume;
            if (v < fire_volume)
            {
                Rocket_sound.volume += 0.05f;
            }
            else { CancelInvoke("SoundFadIn"); }
        }
        /// <summary>
        /// 火箭音效淡出
        /// </summary>
        private void SoundFadeOut()
        {
            float v = Rocket_sound.volume;
            if (v >= 0.1f)
            {
                Rocket_sound.volume -= 0.05f;
            }
            else { CancelInvoke("SoundFadeOut"); }

        }

        /////////////////////////////////////////////
        /// 碰撞區域
        /// 
        private void InvokEnd()
        {
            mgCenter.CheckGame(true);
        }
        #endregion


        #region 事件
        private void Awake()
        {
            orgspd_v = speed_v; orgspd_a = speed_a;
            particle_fire = GetComponentInChildren<ParticleSystem>();
            Rocket_Rig = GetComponent<Rigidbody>();
            Rocket_sound = GetComponent<AudioSource>();
        }
        void Start()
        {

        }
        /// <summary>
        /// 更新事件
        /// </summary>
        void Update()
        {
            if (isControl)
            {
                MoveCheck();
                if (isMove)
                {
                    Rocket_sound.pitch = Input.GetKey(KeyCode.Space) ? 1.5f : 1;
                    if (Rocket_sound.volume < fire_volume) Invoke("SoundFadeIn", 0.2f);
                    MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetKey(KeyCode.Space));
                }
                else
                {
                    if (Rocket_sound.volume > 0.1f) Invoke("SoundFadeOut", 0.2f);
                }
                Invoke("ignix_fire", 0.1f);
            }
        }
        private void FixedUpdate()
        {

            if (isControl)
            {
                UpForce();
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = rocketColor;
            Gizmos.DrawCube(transform.position + rocketOffset, rocketBox);

        }
        private void OnTriggerEnter(Collider other)
        {
            print($"(Rocket_Controll)發生碰撞的位置:{other.transform.position}");
            print($"(Rocket_Controll)飛船所在的位置:{transform.position}, N:{other.tag}");
            if (other.tag.Contains("Enemy"))
            {
                InvokEnd();
            } else if (other.tag.Contains("Block"))
            {

            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag.Contains("Enemy"))
            {
                InvokEnd();
            }
            else if (collision.transform.tag.Contains("Block"))
            {

            }
        }
        #endregion
        #region ##
        #endregion
    }
}