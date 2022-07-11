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
        [SerializeField, Header("燃料"), Range(100, 200), Tooltip("每單位消耗0.25燃料")]
        float fuel = 100;
        [SerializeField, Header("移動速度"), Range(2.0f, 12f)]
        float speed_v = 4f;
        [SerializeField, Header("移動加速度"), Range(0.2f, 1.5f)]
        float speed_a = 0.2f;
        [SerializeField, Header("火焰控制項"), Range(0.5f, 8f)]
        float fire_min_x = 0.5f;
        [SerializeField, Range(0.5f, 8f)]
        float fire_min_y = 1f, fire_max_x = 1f, fire_max_y = 3f;
        [SerializeField, Range(0, 5f)]
        float fire_boost_x = 0, fire_boost_y = 1;
        [SerializeField, Tooltip("火焰音量"), Range(0.1f, 1f),Space]
        float fire_volume = 0.6f;
        [SerializeField, Header("更新控制項")]
        public bool isControl = true;
        //
        ParticleSystem particle_fire;
        Rigidbody Rocket_Rig;
        AudioSource Rocket_sound;
        private bool isMove;
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
            if (Input.GetAxisRaw("Horizontal") !=0  || Input.GetAxisRaw("Vertical") !=0) isMove = true;
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
                float xFire = fire_min_x - 0.1f;
                if (horizon != 0) xFire = Mathf.Pow(fire_max_x, x_var);
                //else if (horizon < 0) xFire = Mathf.Pow(fire_min_x, x_var);

                float yFire = fire_min_y - 0.1f;
                if (vertial > 0) yFire = Mathf.Pow(fire_max_y, y_var);
                else if (vertial < 0) yFire = Mathf.Abs(Mathf.Pow(fire_min_y, y_var));

                if (boost) yFire += fire_boost_y;
                if (boost) xFire += fire_boost_x;


                particle_fire.transform.localScale = new Vector2(xFire, yFire);

                if (particle_fire.transform.localScale.x < fire_min_x && particle_fire.transform.localScale.y < fire_min_y) ignix_fire(false);
                else ignix_fire(true);
            }

            //print(particle_fire.isPlaying);
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

        #endregion

        #region 取得資訊
        /// <summary>
        /// 調用火箭資訊方法
        /// </summary>
        /// <returns>x=燃料；y=速度；z=加速度</returns>
        public Vector3 GetRocketInfo()
        {
            return new Vector3(fuel, speed_v, speed_a);
        }

        #endregion

        #region 事件
        private void Awake()
        {
            particle_fire = GetComponentInChildren<ParticleSystem>();
            Rocket_Rig = GetComponent<Rigidbody>();
            Rocket_sound = GetComponent<AudioSource>();
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
            if (isControl)
            {
                MoveCheck();
                if (isMove)
                {
                    bool boost = Input.GetKey(KeyCode.Space);
                    if (boost) Rocket_sound.pitch = 1.5f;
                    else Rocket_sound.pitch = 1;
                    if (Rocket_sound.volume < fire_volume) Invoke("SoundFadeIn", 0.2f);
                    MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), boost);
                }
                else
                {
                    if (Rocket_sound.volume > 0.1f) Invoke("SoundFadeOut", 0.2f);
                }
            }
        }
        private void FixedUpdate()
        {

            if (isControl)
            {
                UpForce();
                fuel = mgCenter.fuelChange(fuel);
            }
        }

        #endregion
        #region ##
        #endregion
    }
}