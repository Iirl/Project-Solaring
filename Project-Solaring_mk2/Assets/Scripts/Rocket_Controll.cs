using UnityEngine;

/// <summary>
///  ���{���O����a�������ާ@��k�A��ĳ��b���a����W�C
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
        [SerializeField, Header("�U��"), Range(50, 200), Tooltip("�C������0.25�U��")]
        float fuel = 100;
        [SerializeField, Header("���ʳt��"), Range(2.0f, 12f)]
        float speed_v = 4f;
        float orgspd_v = 4f;
        [SerializeField, Header("���ʥ[�t��"), Range(0.2f, 1.5f)]
        float speed_a = 0.2f;
        float orgspd_a = 0.2f;
        public Vector3 RocketS1 { get { return new Vector3(fuel, speed_v, speed_a); } }
        [SerializeField, Header("���K���")]
        Vector2 fireLenght_min = new Vector2(0.5f, 1f);
        [SerializeField, Tooltip("�ɶq���n�W�L�w�]�ȤӦh")]
        Vector2 fireLenght_max = new Vector2(1f, 3f), fireBoost = new Vector2(0f, 1f);
        [SerializeField, Header("���K�̤j���q"), Range(0.1f, 1f)]
        float fire_volume = 0.6f;
        [SerializeField, Header("���b�j�p")]
        Vector3 rocketBox = new Vector3(1f, 3f, 0);
        [SerializeField, Tooltip("���b�첾")]
        Vector3 rocketOffset = new Vector3(0, -1f, 0);
        [SerializeField, Tooltip("���b�C��")]
        Color rocketColor = Color.white;
        [SerializeField, Header("��s���")]
        public bool isControl = true;
        //
        ParticleSystem particle_fire;
        Rigidbody Rocket_Rig;
        AudioSource Rocket_sound;
        private bool isMove;
        #endregion

        #region ���Τ�k
        /// <summary>
        /// �s�����b��T��k�C
        /// </summary>
        /// <returns>x=�U�ơFy=�t�סFz=�[�t��</returns>
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
        /// �����B�檬�A�G���n���B�w�b�e���W�H���������ʱ���C
        /// </summary>
        public bool ControlChange()
        {
            if (Rocket_sound.isPlaying) Rocket_sound.Stop(); else Rocket_sound.Play();
            Rocket_Rig.isKinematic = !Rocket_Rig.isKinematic;
            isControl = !isControl;
            return isControl;
        }
        #endregion

        #region ��k
        private void UpForce()
        {
            if (particle_fire.isPlaying) Rocket_Rig.AddForce(Vector3.up * Time.deltaTime * 10f);
        }
        /// <summary>
        /// �����ˬd
        /// </summary>
        private void MoveCheck()
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) isMove = true;
            else isMove = false;

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
            Rocket_Rig.velocity += v3;
            // �[�t�ױ���
            Vector3 r3 = new Vector3(horizon * aSpeed, vertial * aSpeed, 0);
            if (boost) Rocket_Rig.AddForce(r3);
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
                if (boost) yFire += fireBoost.y;
                if (boost) xFire += fireBoost.x;
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
        /// ���b���ĲH�J
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

        /////////////////////////////////////////////
        /// �I���ϰ�
        /// 
        private void InvokEnd()
        {
            mgCenter.CheckGame(true);
        }
        #endregion


        #region �ƥ�
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
        /// ��s�ƥ�
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
            print($"(Rocket_Controll)�o�͸I������m:{other.transform.position}");
            print($"(Rocket_Controll)����Ҧb����m:{transform.position}, N:{other.tag}");
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