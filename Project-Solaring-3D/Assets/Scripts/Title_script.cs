using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// �B�z���D�������{���A�C�ӥ\���ĳ�Τ@�Ӱϰ�ذ_�ӡC
/// </summary>
public class Title_script : MonoBehaviour
{
    #region �ݩ�
    [SerializeField, Header("Object Obtain")]
    private Camera cam;
    [SerializeField]
    private GameObject btn_str, btn_opt, btn_open;
    [SerializeField]
    private RectTransform bg_space, bg_earth, cvs_rect;
    private AudioSource audioSrc;
    [SerializeField]
    private AudioClip acp_down;
    [SerializeField, Header("Property Adjust")]
    private float bg_move_speed = 0.5f;
    // ��L�ݩ�(���)
    private bool bg_move = true, reload_scene = false;

    #endregion

    #region �򥻥\��
    /// <summary>
    /// �վ��檺������m
    /// </summary>
    /// <param name="rts">�ǤJ�� Rect ����</param>
    /// <param name="end">���ʵ����I</param>
    private void Move2Center(RectTransform rts, float end)
    {
        float y = rts.position.y;
        if (Mathf.Ceil(y) != Mathf.Ceil(end))
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
    /// ��Ū����
    /// </summary>
    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// ���ƥ�G�Ĥ@�h->�ĤG�h
    /// </summary>
    public void Menu1to2()
    {
        btn_str.SetActive(true);
        btn_opt.SetActive(true);
        btn_open.SetActive(false);
        audioSrc.PlayOneShot(acp_down);
    }

    #endregion

    #region Ĳ�o�ƥ�
    private void Awake()
    {
        audioSrc = GameObject.Find("Audio Source").GetComponent<AudioSource>();

    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        if (bg_space != null && bg_move) Move2Center(bg_space, cvs_rect.position.y / 2);
        if (bg_earth != null && bg_move) Move2Center(bg_earth, cvs_rect.position.y / 4);
    }
    private void Update()
    {
        if (Input.GetAxisRaw("Bbutton") > 0 && reload_scene) ReloadCurrentScene(); // ��Ū����
        if (Input.anyKeyDown && btn_open.activeSelf) Menu1to2(); // ��LĲ�o�i�J�U�@�h���ƥ�

    }


    #endregion
}
