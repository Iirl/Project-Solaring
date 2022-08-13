using UnityEngine;
using System.Collections.Generic;

/// <summary>
///  ���{���O����a�������ާ@��k�A��ĳ��b���a����W�C
///  Include basic move, audio control, collision detect and State Class.
/// </summary>
namespace solar_a
{
    public class Rocket_Controll : MonoBehaviour
    {

        #region �ݩ�
        [SerializeField, Header("�����t��")]
        ManageCenter mgCenter;
        #endregion
        #region #�ǦC���ݩ�
        [SerializeField, Header("���b���򥻸�T\n�U��(X)\n���ʳt��(Y)\n���ʥ[�t��(Z)")]
        public Vector3 RocketBasic;
        [SerializeField, Header("�����ӿU��")]
        float unit_fuel;
        public float Unit_fuel { get { return unit_fuel; } set { unit_fuel = value; } }
        [SerializeField]
        float fuel = 100;
        [SerializeField]
        float speed_v = 4f;
        [SerializeField]
        float speed_a = 0.2f;
        [SerializeField]
        public float fuel_overcapacity = 20;
        public Vector3 RocketS1 { get { return new Vector3(fuel, speed_v, speed_a); } }
        [SerializeField, Header("���K���")]
        Vector2 fireLenght_min = new Vector2(0.5f, 1f);
        [SerializeField, Tooltip("�ɶq���n�W�L�w�]�ȤӦh")]
        Vector2 fireLenght_max = new Vector2(1f, 3f), fireBoost = new Vector2(0f, 1f);
        [SerializeField, Header("���K�̤j���q"), Range(0.1f, 1f)]
        float fire_volume = 0.6f;
        [SerializeField]
        List<AudioClip> rocket_Clip = new List<AudioClip>();
        [SerializeField, Header("���b�j�p")]
        Vector3 rocketBox = new Vector3(1f, 3f, 0);
        [SerializeField, Tooltip("���b�첾")]
        Vector3 rocketOffset = new Vector3(0, -1f, 0);
        [SerializeField, Tooltip("���b�C��")]
        Color rocketColor = Color.white;
        [SerializeField, Header("��s���")]
        //public bool isControl = true;
        //public bool isMove;
        public RocketCondition rc_dtion = new RocketCondition() { state = RocketCondition.State.Stay };
        //
        ParticleSystem particle_fire;
        Rigidbody Rocket_Rig;
        AudioSource Rocket_sound;
        #endregion

        #region ���Τ�k
        /// <summary>
        /// �s�����b��T��k�C
        /// </summary>
        /// <returns>x=�U�ơFy=�t�סFz=�[�t��</returns>
        public Vector3 PutRocketSyn(float x, float y = -1, float z = -1)
        {
            fuel += x;
            if (fuel > RocketBasic.x) fuel = RocketBasic.x; // �U�ƭ���b�̤j��
            speed_v = y >= 0 ? y : speed_v;
            speed_a = z >= 0 ? z : speed_a;
            return RocketS1;
        }
        public Vector3 SetBasicInfo(float x, float y , float z)
        {
            return RocketBasic = new Vector3(x,y,z);
        }
        /// <summary>
        /// �����B�檬�A�G���n���B�w�b�e���W�H���������ʱ���C
        /// </summary>
        public bool ControlChange(bool on=false)
        {
            if (!on) Rocket_sound.Stop(); else Rocket_sound.Play();
            Rocket_Rig.isKinematic = !on;
            rc_dtion.ControlChange();
            return rc_dtion.IsStop;
        }
        #endregion

        #region ��k
        private void UpForce()
        {
            if (particle_fire.isPlaying) Rocket_Rig.AddForce(Vector3.up * Time.deltaTime * 10f);
        }
        /// <summary>
        /// ���ʱ���
        /// </summary>
        /// <param name="horizon">�������ʦV�q</param>
        /// <param name="vertial">�������ʦV�q</param>
        private void MoveControll(float horizon, float vertial, float boost)
        {
            if (Mathf.Abs(horizon) < 0.005f) horizon = 0;
            if (Mathf.Abs(vertial) < 0.005f) vertial = 0;
            float xSpeed = (speed_v * horizon) * Time.deltaTime;
            float ySpeed = (speed_v * vertial) * Time.deltaTime;
            float aSpeed = speed_a * 1000;
            // ���ʱ���
            Vector3 v3 = new Vector3(xSpeed, ySpeed, 0);
            Rocket_Rig.velocity += v3;
            // �[�t�ױ���
            Vector3 r3 = new Vector3(horizon * aSpeed, vertial * aSpeed, 0);
            if (boost > 0) Rocket_Rig.AddForce(r3);
            if (boost > 0) PutRocketSyn(-(speed_a) * 0.25f * Time.deltaTime);
            // ½��^�_
            Rocket_Rig.transform.Rotate(transform.rotation.x, transform.rotation.y, 0);
            //print($"H:{horizon}; V:{vertial}; Fire{particle_fire.isPlaying}");        
            //print($"H+V:{Mathf.Abs(horizon) + Mathf.Abs(vertial)};");
            // ���K����
            if (particle_fire != null)
            {
                float x_var = Mathf.Abs(horizon);
                float y_var = Mathf.Abs(vertial);
                //// set Fire X axis lenght.��b����
                float xFire = fireLenght_min.x - 0.1f;
                if (horizon != 0) xFire = Mathf.Pow(fireLenght_max.x, x_var);
                //else if (horizon < 0) xFire = Mathf.Pow(fire_min_x, x_var);
                //// set Fire Y axis lenght.�a�b����
                float yFire = fireLenght_min.y - 0.1f;
                if (vertial > 0) yFire += Mathf.Pow(fireLenght_max.y, y_var);
                else if (vertial < 0) yFire += Mathf.Abs(Mathf.Pow(fireLenght_min.y, y_var));
                //// set Fire Boost.�Ĩ����
                if (boost > 0) yFire += fireBoost.y;
                if (boost > 0) xFire += fireBoost.x;
                //// set Horizon Move yFire.
                if (yFire < fireLenght_min.y && x_var > 0) yFire += Mathf.Abs(Mathf.Pow(fireLenght_min.y, x_var));

                particle_fire.transform.localScale = new Vector2(xFire, yFire);
            }

            //print(particle_fire.isPlaying);
        }
        /// <summary>
        ///  �I������
        /// </summary>
        private void ignix_fire()
        {
            if (!rc_dtion.IsStay)
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
        /// ���b���ĲH�J
        /// </summary>
        private void SoundFadeIn()
        {
            float v = Rocket_sound.volume;
            if (v <= fire_volume)
            {
                Rocket_sound.volume += 0.05f;
            }
            else { CancelInvoke("SoundFadIn"); }
        }
        /// <summary>
        /// ���b���ĲH�X
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
        /// ���b���ĳ]�w�G
        /// 0. �I��ɵ��~
        /// 1. �I��ĤH�Φ��z
        /// 2. �j�Ʃʭ���
        /// </summary>
        public void ADOClipControl(int i)
        {
            Rocket_sound.volume = 1;
            Rocket_sound.PlayOneShot(rocket_Clip[i], 1f);
        }
        /////////////////////////////////////////////
        /// �I���ϰ�
         
        /// <summary>
        /// ����b�I�쪫��ɪ��B�z
        /// </summary>
        /// <param name="other">���o����</param>
        private void CollisionEvent(GameObject other)
        {
            if (other.tag.Contains("Enemy"))
            {
                if (StaticSharp.isPowerfullMode)
                {
                    return;
                }
                //�����C���B�z
                ADOClipControl(1);
                mgCenter.CheckGame(true);
            }
            else if (other.tag.Contains("Block"))
            {                
                int addFuel = 0;
                //����b�I��ɫ~��
                if (other.name.Contains("Box") || other.name.Contains("box")) addFuel = 10;
                if (other.name.Contains("Bottle")) addFuel = 5;
                mgCenter.ObjectDestory(other);
                mgCenter.FuelReplen(addFuel);
            }
            else if (other.tag.Contains("Respawn"))
            {
                mgCenter.InToStation();
            }
        }
        #endregion

        private void Awake()
        {            
            RocketBasic = StaticSharp.Rocket_BASIC != Vector3.zero ? StaticSharp.Rocket_BASIC: RocketBasic;
            fuel = RocketBasic.x;
            speed_v = RocketBasic.y;
            speed_a = RocketBasic.z;
            particle_fire = GetComponentInChildren<ParticleSystem>();
            Rocket_Rig = GetComponent<Rigidbody>();
            Rocket_sound = GetComponent<AudioSource>();
        }
        #region �ƥ�
        void Update()
        {
            if (!rc_dtion.IsStop)
            {
                MoveControll(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Force"));
                Rocket_sound.pitch = Input.GetAxis("Force") > 0 ? 1.5f : 1;
                int h = (int)Input.GetAxisRaw("Horizontal");
                int v = (int)Input.GetAxisRaw("Vertical");
                if ((h!=0 ||v !=0))
                {                    
                    if (Rocket_sound.volume < fire_volume) Invoke("SoundFadeIn", 0.2f);
                    rc_dtion.onMove();
                } else  if (h ==0 && v == 0)
                {
                    if (Rocket_sound.volume > 0.1f) Invoke("SoundFadeOut", 0.2f);
                    rc_dtion.onStay();
                } else { 
                }
                Invoke("ignix_fire", 0.1f);
            }
            else
            {
                Rocket_Rig.velocity = Vector3.zero;
            }

        }
        private void FixedUpdate()
        {

            if (!rc_dtion.IsStop)
            {
                UpForce();
            }
            //print($"State: {rc_dtion.state}, IsStop {rc_dtion.IsStop}, IsStay {rc_dtion.IsStay}");
            //print(rc_dtion.GetState());
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = rocketColor;
            Gizmos.DrawCube(transform.position + rocketOffset, rocketBox);

        }
        private void OnTriggerEnter(Collider other)
        {
            //print($"(Rocket_Controll)�o�͸I������m:{other.transform.position}");
            //print($"(Rocket_Controll)����Ҧb����m:{transform.position}, N:{other.tag}");
            CollisionEvent(other.gameObject);
        }
        private void OnCollisionEnter(Collision collision)
        {
            CollisionEvent(collision.gameObject);
            
        }
        #endregion
        #region ##
        #endregion
    }

    #region ���b���A
    public class RocketCondition
    {
        public enum State { Stay, Move, Float, Crashed, Stop }
        public State state = State.Stop;
        public bool IsStay { get { return isStay; } }
        private bool isStay = true;
        public bool IsCrashed { get { return isCrashed; } }
        private bool isCrashed = false;
        public bool IsStop { get { return isStop; } }
        private bool isStop = false;

        public void Next()
        {
            if (state < State.Crashed - 1) state++;
            boolChange();
        }
        public void Previous()
        {
            if (state != 0) state--;
            boolChange();
        }
        public void Dead()
        {
            state = State.Crashed;
            boolChange();
        }
        public void onMove()
        {
            state = State.Move;
            boolChange();
        }
        public void onStay()
        {
            state = State.Stay;
            boolChange();
        }
        public bool ControlChange()
        {
            return isStop = !isStop;
        }
        public int GetState()
        {
            return (int)(state);
        }

        private void boolChange()
        {
            isStay = (int)state == 0 ? true : false;
            isCrashed = (int)state == 3 ? true : false;
            isStop = (int)state == 4 ? true : false;
        }
    }
    #endregion
}