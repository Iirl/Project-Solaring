using UnityEngine;


/// <summary>
/// �B�z���D�������{���A�C�ӥ\���ĳ�Τ@�Ӱϰ�ذ_�ӡC
/// </summary>
public class Title_script : MonoBehaviour
{
    #region �ݩ�
    [SerializeField, Header("Object Obtain")]
    private Camera cam;
    [SerializeField]
    private RectTransform bg_space, bg_earth, cvs_rect;
    [SerializeField, Header("Property Adjust")]
    private float bg_move_speed = 0.01f;
    private bool bg_move = true;
    
    #endregion

    #region �򥻥\��

    private void Move2Center(RectTransform rts, float end) {
        float y = rts.position.y;
        if (Mathf.Ceil(y) != Mathf.Ceil(end)) rts.Translate(new Vector2(0,end-y)*Time.deltaTime);

    }

    #endregion

    #region Ĳ�o�ƥ�
    private void Awake()
    {
        
    }

    private void Start()
    {
        

    }

    private void FixedUpdate()
    {
        if (bg_space != null && bg_move) Move2Center(bg_space, cvs_rect.position.y/2);
        if (bg_earth != null && bg_move) Move2Center(bg_earth, cvs_rect.position.y/4);
    }
    #endregion
}
