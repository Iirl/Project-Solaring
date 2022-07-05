using UnityEngine;

/// <summary>
///  ���{���O����a�������ާ@��k
/// </summary>
namespace solar_a
{
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
        [SerializeField, Header("���K���"), Range(0.5f, 8f)]
        float fire_min_x = 0.5f;
        [SerializeField, Range(0.5f, 8f)]
        float fire_min_y = 1f, fire_max_x = 1f, fire_max_y = 3f;
        [SerializeField, Range(0, 5f)]
        float fire_boost_x = 0, fire_boost_y = 1;
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
}