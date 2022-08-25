using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
///  此程式是控制玩家相關的操作方法，建議放在玩家物件上。
///  Include basic move, audio control, collision detect and State Class.
/// </summary>
namespace solar_a
{
    public class Rocket_Controll : MonoBehaviour
    {

        #region 屬性
        ParticleSystem particle_fire;
        Rigidbody Rocket_Rig;
        AudioSource Rocket_sound;
        private bool isBoost;
        #endregion
        #region #序列化屬性
        [SerializeField, Header("火箭的基本資訊\n燃料(X)\n移動速度(Y)\n衝刺倍率(Z)")]
        public Vector3 RocketBasic;
        [SerializeField, Header("單位消耗燃料")]
        private float unit_fuel;
        public float Unit_fuel { get { return unit_fuel; } set { unit_fuel = value; } }
        [SerializeField]
        private float fuel = 100;
        private float speed_v = 4f;
        private float speed_a;
        [SerializeField, Range(0.1f, 20)]
        private float rush_time;
        [SerializeField, Range(1, 20)]
        private int rush_counts;
        Vector3 boostDirect;
        [SerializeField]
        public float fuel_overcapacity = 20;
        // 取得火箭資訊的欄位
        public Vector3 RocketS1 { get { return new Vector3(fuel, speed_v, speed_a); } }
        [SerializeField, Header("火焰控制項")]
        Vector2 fireLenght_min = new Vector2(0.5f, 1f);
        [SerializeField, Tooltip("盡量不要超過預設值太多")]
        Vector2 fireLenght_max = new Vector2(1f, 3f), fireBoost = new Vector2(0f, 1f);
        [SerializeField, Header("火焰最大音量"), Range(0.1f, 1f)]
        float fire_volume = 0.6f;
        [SerializeField]
        List<AudioClip> rocket_Clip = new List<AudioClip>();
        [SerializeField, Header("火箭大小")]
        Vector3 rocketBox = new Vector3(1f, 3f, 0);
        [SerializeField, Tooltip("火箭位移")]
        Vector3 rocketOffset = new Vector3(0, -1f, 0);
        [SerializeField, Tooltip("火箭顏色")]
        Color rocketColor = Color.white;
        [SerializeField, Header("更新控制項")]
        //public bool isControl = true;
        //public bool isMove;
        public RocketCondition rc_dtion = new RocketCondition() { state = RocketState.Stay };
        //
        #endregion

        /// <summary>
        /// 存取火箭資訊方法：
        /// PutRocketSyn 可以修改當前火箭的狀態，能夠限制輸入值，進入中繼站時會讀取 RocketS1 的資料。
        /// RocketS1 是唯讀欄位，不可以直接修改該數值。
        /// RocketBasic 是火箭的基本素質，重新讀取或換場景時會讀取該資料，改造火箭時不能低於該數值。
        /// SetBasicInfo 直接修改火箭的基本素質。
        /// </summary>
        public Vector3 SetBasicInfo(float x, float y, float z) => RocketBasic = new Vector3(x, y, z);

        #region 公用方法
        /// <summary>
        /// 火箭資料變動。
        /// </summary>
        /// <returns>x=燃料；y=速度；z=加速時間</returns>
        public Vector3 PutRocketSyn(float x, float y = -1, float z = -1)
        {
            fuel += x;
            if (fuel > RocketBasic.x) fuel = RocketBasic.x; // 燃料限制在最大值
            else if (fuel < 0) fuel = 0;
            speed_v = y >= 0 ? y : speed_v;
            speed_a = z >= 0 ? z : speed_a;
            return RocketS1;
        }

        /// <summary>
        /// 切換運行狀態：關聲音、定在畫面上以及關閉移動控制。
        /// </summary>
        public IEnumerator ControlChange(bool on = false)
        {
            if (!on) while (Rocket_sound.isPlaying) { Rocket_sound.Stop(); yield return null; }
            else while (!Rocket_sound.isPlaying) { Rocket_sound.Play(); yield return null; }
            Rocket_Rig.isKinematic = !on;
            if (on) rc_dtion.onStay();
            else rc_dtion.onStop();
            enabled = on;
            yield return null;
        }
        #endregion

        private void UpForce()
        {
            if (particle_fire.isPlaying) Rocket_Rig.AddForce(Vector3.up * Time.deltaTime * 10f);
            Rocket_Rig.AddForce(-Rocket_Rig.velocity, ForceMode.Acceleration);

        }
        /// <summary>
        /// 移動控制
        /// </summary>
        /// <param name="horizon">水平移動向量</param>
        /// <param name="vertial">垂直移動向量</param>
        private void MoveControll(float horizon, float vertial, float boost)
        {
            if (Mathf.Abs(horizon) < 0.005f) horizon = 0;
            if (Mathf.Abs(vertial) < 0.005f) vertial = 0;
            float xSpeed = (speed_v * horizon) * Time.deltaTime;
            float ySpeed = (speed_v * vertial) * Time.deltaTime;
            float aSpeed = speed_v * speed_a * Time.deltaTime;
            // 移動控制
            Vector3 v3 = new Vector3(xSpeed, ySpeed, 0);
            Rocket_Rig.velocity += v3;
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
                // 衝刺狀態
                if (rc_dtion.IsBoost)
                {
                    boostDirect = new Vector3(horizon * aSpeed, vertial * aSpeed, 0);
                    Rocket_Rig.velocity += (boostDirect);
                    xFire += fireBoost.x;
                    yFire += fireBoost.y;
                }
                //// set Horizon Move yFire.
                if (yFire < fireLenght_min.y && x_var > 0) yFire += Mathf.Abs(Mathf.Pow(fireLenght_min.y, x_var));

                particle_fire.transform.localScale = new Vector2(xFire, yFire);
            }
            //print(particle_fire.isPlaying);
        }
        /// <summary>
        /// 衝刺程序
        /// </summary>
        /// <returns>等待時間</returns>
        private IEnumerator StartBoost()
        {
            isBoost = true;
            float horizon = Input.GetAxis("Horizontal") != 0 ? 1 : 0;
            float vertial = Input.GetAxis("Vertical") != 0 ? 1 : 0;
            int c = rush_counts;
            // 加速度控制
            //增加推力
            Rocket_sound.pitch = 1.5f;
            while (c == rush_counts)
            {
                yield return new WaitForSeconds(rush_time);
                isBoost = false;
                if (!ManageCenter.mgCenter.noExhauRush) rush_counts--;
                Rocket_sound.pitch = 1;
                rc_dtion.Previous();
            }
        }
        /// <summary>
        ///  點火控制
        /// </summary>
        private void ignix_fire()
        {
            if (!rc_dtion.IsStay) particle_fire.Play();
            else
            {
                particle_fire.Pause();
                particle_fire.transform.localScale = new Vector2(1, 1);
                CancelInvoke("ignix_fire");
            }
        }

        #region 音效
        /// <summary>
        /// 火箭音效淡入
        /// </summary>
        private void SoundFadeIn()
        {
            float v = Rocket_sound.volume;
            if (v <= fire_volume) Rocket_sound.volume += 0.05f;
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

        /// <summary>
        /// 火箭音效設定：
        /// 0. 碰到補給品
        /// 1. 碰到敵人或自爆
        /// 2. 強化性音效
        /// </summary>
        public void ADOClipControl(int i)
        {
            Rocket_sound.volume = 1;
            Rocket_sound.PlayOneShot(rocket_Clip[i], 1f);
        }
        public void ADOClipControl(AudioClip acp, float vol=1) => Rocket_sound.PlayOneShot(acp, vol);
        #endregion

        private void Awake()
        {
            RocketBasic = StaticSharp.Rocket_BASIC != Vector3.zero ? StaticSharp.Rocket_BASIC : RocketBasic;
            fuel = RocketBasic.x;
            speed_v = RocketBasic.y;
            speed_a = RocketBasic.z;
            particle_fire = GetComponentInChildren<ParticleSystem>();
            Rocket_Rig = GetComponent<Rigidbody>();
            Rocket_sound = GetComponent<AudioSource>();

        }
        #region 事件
        private void Controller() => MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Force"));
        private bool InputMove() => Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        private bool InputBoost() => Input.GetAxisRaw("Force") != 0;
        private void Booster() => StartCoroutine(StartBoost());
        void Update()
        {
            switch (rc_dtion.state)
            {
                case RocketState.Stay:
                    UpForce();
                    if (InputMove()) rc_dtion.Next();
                    break;
                case RocketState.Move:
                    if (!InputMove()) rc_dtion.Previous();
                    if (InputBoost() && rush_counts != 0) rc_dtion.Next();
                    break;
                case RocketState.Boost:
                    if (!isBoost) Booster();
                    break;
                case RocketState.Crashed:
                    break;
                case RocketState.Stop:
                    break;
                default:
                    break;
            }
            //print(InputMove());
            //if (StaticSharp.Conditions == State.Finish) rc_dtion.onStop();
            // 火箭正常狀態下的行動。
            if (!rc_dtion.IsStop || !rc_dtion.IsCrashed)
            {
                Controller();
                // 狀態控制
                //print(rc_dtion.state);
                if (!rc_dtion.IsStay)
                {
                    if (Rocket_sound.volume < fire_volume) Invoke("SoundFadeIn", 0.2f);
                }
                else
                {
                    if (Rocket_sound.volume > 0.1f) Invoke("SoundFadeOut", 0.2f);
                }
                Invoke("ignix_fire", 0.1f);
            }
            else Rocket_Rig.velocity = Vector3.zero;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = rocketColor;
            Gizmos.DrawCube(transform.position + rocketOffset, rocketBox);
        }
        private void OnTriggerEnter(Collider other)
        {
            //CollisionEvent(other.gameObject);
            int idx = ColliderSystem.CollisionPlayerEvent(other.gameObject);
            //print($"(Rocket_Controll)發生碰撞的位置:{other.transform.position}");
            //print($"(Rocket_Controll)飛船所在的位置:{transform.position}, N:{other.tag}");
        }
        private void OnCollisionEnter(Collision collision)
        {
            int idx = ColliderSystem.CollisionPlayerEvent(collision.gameObject);
            //CollisionEvent(collision.gameObject);
        }
        #endregion
        #region ##
        #endregion
    }

    #region 火箭狀態
    public class RocketCondition
    {
        public RocketState state = RocketState.Stop;
        public bool IsStay { get { return state == RocketState.Stay; } }
        public bool IsBoost { get { return state == RocketState.Boost; } }
        public bool IsCrashed { get { return state == RocketState.Crashed; } }
        public bool IsStop { get { return state == RocketState.Stop; } }
        //
        public void Next() => state = (state < RocketState.Crashed - 1) ? state + 1 : state;
        public void Previous() => state = (state != 0) ? state - 1 : state;
        public void Dead() => state = RocketState.Crashed;
        public void onMove() => state = RocketState.Move;
        public void onStay() => state = RocketState.Stay;
        public void onStop() => state = RocketState.Stop;
        public int GetState() => (int)(state);
        public void SetState(int idx) => state = (RocketState)idx;

        #endregion
    }
}