using UnityEngine;

namespace solar_a
{
    public class ManageEnd : MonoBehaviour
    {
        [SerializeField, Header("¤¤±±¨t²Î")]
        ManageCenter mgCenter;


        private void Start()
        {
            mgCenter.canvas_select = GetComponent<CanvasGroup>();
            mgCenter.InvokeRepeating("FadeIn", 0, 0.1f);
        }
    }
}