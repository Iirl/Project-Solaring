using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


/// <summary>
/// �B�z���D�������{���A�C�ӥ\���ĳ�Τ@�Ӱϰ�ذ_�ӡC
/// </summary>
namespace solar_a
{
    public class Title_script : MonoBehaviour
    {
        #region �ݩ�
        [SerializeField, Header("�����B�z�t��")]
        ManageScene SceneSTG;
        [SerializeField, Header("���񭵮�")]
        private List<AudioClip> sound_effect;
        [SerializeField]
        private AudioSource audioSrc;
        [SerializeField, Header("�����s����")]
        private List<GameObject> menuButton;
        [SerializeField, Header("Property Adjust")]
        private float bg_move_speed = 0.5f;
        // ��L�ݩ�(���)
        private bool bg_move = true, reload_scene = false;

        #endregion

        #region �򥻥\��
        /// <summary>
        /// �վ��檺�ʵe
        /// </summary>
        /// <param name="rts">�ǤJ�� Rect ����</param>
        /// <param name="end">���ʵ����I</param>
        private void Move2Center(RectTransform rts, float end)
        {
            float y = rts.position.y;
            if (Mathf.Ceil(y) != Mathf.Ceil(end) && !bg_move)
            {
                rts.Translate(new Vector2(0, end - y) * Time.deltaTime * bg_move_speed);
            }
            else
            {
                bg_move = false;
                reload_scene = true;
            }

        }
        /// <summary>
        /// ��歵�ĮĪG
        /// ���ĵ��ѡG
        /// </summary>
        public void MenuSound(int i)
        {
            if (sound_effect[i] != null) audioSrc.PlayOneShot(sound_effect[i]);
        }
        #endregion

        #region Ĳ�o�ƥ�
        private void Awake()
        {
            bg_move = false;
            audioSrc = GameObject.Find("Audio Source").GetComponent<AudioSource>();
            // ����Ū�����s


        }

        private void Start()
        {

        }

        private void FixedUpdate()
        {

        }
        private void Update()
        {
            if (Input.GetAxisRaw("Menu") > 0 && reload_scene) SceneSTG.ReloadCurrentScene(); // ��Ū����

        }


        #endregion
    }
}