using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
///  此程式是控制玩家相關的操作方法，建議放在玩家物件上。
///  Include basic move, audio control, collision detect and State Class.
/// </summary>
namespace solar_a
{
    [RequireComponent(typeof(SSRocket))]
    public class Rocket_Controll : MonoBehaviour
    {

        #region 屬性
        ManageCenter mgc;
        Rigidbody Rocket_Rig;
        AudioSource Rocket_sound;
        // 動畫控制
        Animator Rocket_ani;
	    private bool isBoost;
        #endregion
        #region #序列化屬性
        [SerializeField, Header("停用玩家操控")]
        private bool offControl;
        public bool CloseTheControl { set { offControl = value; rc_dtion.onStop(value); } get { return offControl; } }
        [SerializeField, Header("火箭的基本資訊\n燃料(X)    \t移動速度(Y)\t衝刺倍率(Z)")]
        public Vector3 RocketBasic;
        [SerializeField, Header("火箭能力調整")]
        private float unit_fuel;
        public float Unit_fuel { get { return unit_fuel; } set { unit_fuel = value; } }
        [SerializeField]
        public float fuel_overcapacity = 20;
        [SerializeField, Range(0.1f, 5.0f)]
        private float rush_time;
        [SerializeField, Range(1, 20)]
	    private int rush_counts;
	    public int RushCounts => rush_counts;
        [SerializeField, Header("×快速查看"), Tooltip("程式控制項目，設定也沒意義。")]
        private float fuel = 100;
        private float speed_v = 4f;
        private float speed_a;
        Vector3 boostDirect;
        [SerializeField]
        public List<AudioClip> rocket_Clip = new List<AudioClip>();
        [SerializeField, Header("火焰控制項")]
        private bool hasParticleFile;
        [SerializeField, HideInInspector]
        private ParticleSystem particle_fire;
	    private ParticleSystem particle_fire2;
        [SerializeField, Tooltip("盡量不要超過預設值太多"), HideInInspector]
        private Vector2 fireLenght_min = new Vector2(0.5f, 1f), fireLenght_max = new Vector2(1f, 3f), fireBoost = new Vector2(0f, 1f);
        [SerializeField, Tooltip("火焰最大音量"), Range(0.1f, 1f), HideInInspector]
        private float fire_volume = 0.6f;
	    [SerializeField, Header("火箭控制項"),Tooltip("火箭動畫關閉時間")]
	    private float aniCloseTime = 1;
        [SerializeField, Header("火箭狀態")]
        public RocketCondition rc_dtion = new RocketCondition() { state = RocketState.Stay };
        //
        #endregion

        // 取得火箭資訊的欄位
        public Vector3 RocketVarInfo => new Vector3(fuel, speed_v, speed_a);
        /// <summary>
        /// 存取火箭資訊方法：
        /// PutRocketSyn 可以修改當前火箭的狀態，能夠限制輸入值，進入中繼站時會讀取 RocketS1 的資料。
        /// RocketVarInfo 是唯讀欄位，取得目前變動的數值。
        /// RocketBasic 是火箭的基本素質，重新讀取或換場景時會讀取該資料，改造火箭時不能低於該數值。
        /// SetBasicInfo 直接修改火箭的基本素質，除了太空站要修改外盡量不要動到這裡。
        /// </summary>
	    public Vector3 SetBasicInfo(float x, float y, float z) => RocketBasic = new Vector3(x, y, z);
	    [Header("減速效果"), HideInInspector]
        public float Speed_slow; 

        #region 公用方法
        public void ReGetCOMPON() => SetComponent();
        public void StateOnVisable() => RocketNormalState();
        public void StateToOff(bool state = true) => RocketEffectState(-1, state);
        public void StateToShield(bool state = true) => RocketEffectState(0, state);
        public void StateToSpeedline(bool state = true) => RocketEffectState(1, state);
        public void StateToBorken(bool state = true) => RocketEffectState(2, state);
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
            return RocketVarInfo;
        }
        /// <summary>
        /// 切換運行狀態：關聲音、定在畫面上以及關閉移動控制。
        /// </summary>
        public IEnumerator ControlChange(bool on = false)
        {
            if (!on) while (Rocket_sound.isPlaying) { Rocket_sound.Stop(); yield return null; }
            else while (!Rocket_sound.isPlaying) { Rocket_sound.Play(); yield return null; }
            Rocket_Rig.isKinematic = !on;
            if (rc_dtion.IsCrashed) rc_dtion.onStop(true); //如果為損毀就優先處理
            else if (on) rc_dtion.onStay();
            else rc_dtion.onStop(true);
            enabled = on;
            yield return null;
        }
        #endregion
        //
        private void UpForce()
        {
            if (particle_fire) if (particle_fire.isPlaying) Rocket_Rig.AddForce(Vector3.up * Time.deltaTime * 10f);
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
            float speed_v = this.speed_v * (1-Speed_slow);
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
            #region 火焰控制
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
	            if (particle_fire2) {
	            	particle_fire2.transform.localScale = particle_fire.transform.localScale;
	            }
            }
            #endregion
        }
        /// <summary>
        /// 衝刺程序
        /// </summary>
        /// <returns>等待時間</returns>
        private IEnumerator StartBoost()
        {

            float horizon = Input.GetAxis("Horizontal") != 0 ? 1 : 0;
            float vertial = Input.GetAxis("Vertical") != 0 ? 1 : 0;
            int c = rush_counts;
            // 加速度控制
            //增加推力
            Rocket_sound.pitch = 1.5f;
            while (!isBoost)
            {
                rc_dtion.Previous();
                StateToSpeedline(true);
                yield return new WaitForSeconds(rush_time);
                if (!mgc.noExhauRush) rush_counts--;
                Rocket_sound.pitch = 1;
                StateToSpeedline(false);
            }
            yield return new WaitForSeconds(1);
            isBoost = false;
        }
        /// <summary>
        ///  點火控制
        /// </summary>
        private void ignix_fire()
        {
            if (!particle_fire) return;
            if (!rc_dtion.IsStay) particle_fire.Play();
            else
            {
                particle_fire.Pause();
                particle_fire.transform.localScale = new Vector2(1, 1);
                CancelInvoke("ignix_fire");
            }
        }
        /// <summary>
        /// 火箭的執行狀態變化，依照火箭上"Effect"物件的內容進行開關，以下內容僅記錄用：
        /// 0. 開啟護盾：保護罩特效。
        /// 1. 衝刺：速度線。
        /// 2. 銷毀：毀壞的外觀。
        /// </summary>
        /// <param name="idx">根據子物件的順序決定效果</param>
        /// <param name="open">為真的時候啟動物件</param>
        private void RocketEffectState(int idx, bool open)
        {
            //print("效果開關");
            GameObject status = transform.Find("Effect").gameObject;
            if (idx >= 0) status = status.transform.GetChild(idx).gameObject;
            else status = transform.Find("Effect").gameObject;
            if (idx == 1) isBoost = true;
            if (idx == 2)
            {
                transform.Find("Normal").gameObject.SetActive(!open);
                enabled = !open;
                Rocket_sound.enabled = !open;
                if (!open)
                {
                    rc_dtion.onStay();
                    transform.position = new Vector3(0, -11, 0);
                }
            }
            status.SetActive(open);
        }
        private void RocketNormalState()
        {
            GameObject normal = transform.Find("Normal").gameObject;
            normal.SetActive(!normal.activeSelf);
        }
        #region 音效
        /// <summary>
        /// 火箭音效設定：
        /// 0. 碰到補給品
        /// 1. 碰到敵人或自爆
        /// 2. 強化性音效
        //public void ADOClipControl(AudioClip acp, float vol = 1) => Rocket_sound.PlayOneShot(acp, vol);
        /// </summary>
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
        #endregion
        private void CloseAnimator() => Rocket_ani.enabled = false;


        private void Awake()
        {
            fuel = RocketBasic.x;
            speed_v = RocketBasic.y;
            speed_a = RocketBasic.z;
            SetComponent();
        }
        /// <summary>
        /// 取得火箭上的元件
        /// </summary>
        private void SetComponent()
        {
            if (hasParticleFile) particle_fire = particle_fire != null ? particle_fire : GetComponentInChildren<ParticleSystem>();
	        if (hasParticleFile) if(name.Contains("Cargo")) particle_fire2 = GetComponentsInChildren<ParticleSystem>()[1];
            Rocket_Rig = GetComponent<Rigidbody>();
            Rocket_sound = GetComponent<AudioSource>();
            Rocket_ani = GetComponent<Animator>();
        }
        #region 事件
        private void Start()
        {
            mgc = ManageCenter.mgCenter;
	        // 檢查暫存器中是否有資料，如果有就讀取 => 如果從中繼站出來的火箭需要重讀資料。
	        RocketBasic = StaticSharp.Rocket_BASIC != Vector3.zero ? StaticSharp.Rocket_BASIC : RocketBasic;
            if (StaticSharp.Rocket_INFO == Vector3.zero) StaticSharp.Rocket_INFO = RocketBasic;
            else
            { 
	            fuel = StaticSharp.Rocket_INFO.x;
                speed_v = StaticSharp.Rocket_INFO.y;
	            speed_a = StaticSharp.Rocket_INFO.z;
	            fuel = fuel > 0 ? fuel: RocketBasic.x;
	            speed_v = speed_v > 0 ? speed_v: RocketBasic.y;
	            speed_a = speed_a > 0 ? speed_a: RocketBasic.z;
            }
	        if (StaticSharp.Rocket_POS != Vector3.zero) transform.position = StaticSharp.Rocket_POS;
	        if (StaticSharp.Rocket_rushCount != -1)  {
	        	rush_counts = StaticSharp.Rocket_rushCount;
	        	StaticSharp.Rocket_rushCount = -1;
	        }
            // 重新設定火箭控制器的資料
            ManageCenter.rocket_ctl = GetComponent<Rocket_Controll>();
            // 如果動畫內容是開啟的狀態，則在一定時間後關閉
	        if (Rocket_ani) if (Rocket_ani.isActiveAndEnabled && aniCloseTime > 0) Invoke("CloseAnimator", aniCloseTime);
            // 其他項目
            if (offControl) CloseTheControl = offControl;
        }

        // 火箭狀態方法
        private void Controller() => MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Force"));
        private bool InputMove() => Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        private bool InputBoost() => Input.GetAxisRaw("Force") != 0;
        private void Booster() => StartCoroutine(StartBoost());
        void Update()
        {
	        //print(rc_dtion.state);
	        if (!mgc) mgc = FindObjectOfType<ManageCenter>();
            switch (rc_dtion.state)
            {
                case RocketState.Stay:
                    UpForce();
                    //if (!Rocket_Rig.isKinematic) StartCoroutine(ControlChange(true));
                    if (InputMove()) rc_dtion.Next();
                    break;
                case RocketState.Move:
                    //print("移動狀態");
                    if (!InputMove()) rc_dtion.Previous();
	                if(Input.anyKeyDown) if (InputBoost() && rush_counts > 0 && !isBoost) rc_dtion.Next();
                    break;
                case RocketState.Boost:
                    //print(InputBoost());                    
                    if (!mgc) rc_dtion.Previous();
                    else Booster();
                    break;
                case RocketState.Crashed:
                    //print("損毀處理後進入停止狀態");
                    StartCoroutine(ControlChange(false));

                    break;
                case RocketState.Stop:
                    break;
                default:
                    print("狀態失控");
                    break;
            }
            //if (StaticSharp.Conditions == State.Finish) rc_dtion.onStop();
            // 火箭正常狀態下的行動。
            if (!rc_dtion.IsStop)
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
                if (particle_fire) Invoke("ignix_fire", 0.1f);
            }
            else
            {
                Rocket_Rig.velocity = Vector3.zero;
            }
        }
        private void OnTriggerEnter(Collider other)
	    {
		    ColEvent(other.gameObject, transform.position);
            //print($"(Rocket_Controll)發生碰撞的位置:{other.transform.position}");
            //print($"(Rocket_Controll)飛船所在的位置:{transform.position}, N:{other.tag}");            
        }
        private void OnCollisionEnter(Collision collision)
	    {
		    //print(collision.transform.name);
	        ColEvent(collision.gameObject, transform.position);
        }
        
	    private void ColEvent(GameObject gob, Vector3 pos){
	    	int idx = ColliderSystem.CollisionPlayerEvent(gob);
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
        public void onStop(bool isStop) => state = (isStop) ? RocketState.Stop : 0;
        public int GetState() => (int)(state);
        public void SetState(int idx) => state = (RocketState)idx;

        #endregion
    }
}