using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Cinemachine;
using Unity.VisualScripting;

namespace solar_a
{
    /// <summary>
    /// 場景控制系統，邊界判定及場景內容的變化。
    /// Box Collider limit to Rocket can't over the border.
    /// </summary>
    public class SceneStage_Control : MonoBehaviour
    {
        #region 屬性
        [SerializeField, Header("場景資訊"), Tooltip("場景大小(Read Only)")]
        private Vector3 stage_container;
        [SerializeField, Tooltip("場景位址(Read Only)")]
        private Vector3 stage_position;
        [SerializeField, Header("場景相關可調變數"), Tooltip("判定區域顏色")]
        private Color box_color = Color.cyan;
        [SerializeField, Header("判定區域大小")]
        private Vector3 box_range = Vector3.zero;
        [SerializeField, Header("判定區域位移")]
        private Vector3 box_offset = Vector3.zero;
        [SerializeField,Header("限制火箭移動(Clamp)區域微調")]
        private Vector2 minBorder;
        [SerializeField]
        private Vector2 maxBorder;
        Vector3 nbox_range;
        [SerializeField, Header("場景移動速度")]
        public float speed;
        public float finishDistane;
        [SerializeField, Header("其他標籤觸發消除物件")]
        private string[] includeTag;
        [SerializeField, Header("重力調整")]
        private Vector3 gravity3 = new(0, -9.8f, 0);
        Camera MainCam;
        CinemachineVirtualCamera cinemachine;
        BoxCollider Stage_boxBorder;
        RectTransform Space_Rect;
        #endregion

        #region 全域控制方法
        /// <summary>
        /// 取得畫面邊界：程式啟動時或者調整大小時要呼叫此函數調整邊界。
        /// </summary>
        public Vector3 GetBoxborder()
        {
            stage_container.x = Mathf.Round(MainCam.aspect * cinemachine.m_Lens.OrthographicSize * 2);     //width
            stage_container.y = Mathf.Round((1 / MainCam.aspect) * stage_container.x) - 1; //heigh
            stage_container.z = stage_container.x;
            Stage_boxBorder.size = stage_container;
            Space_Rect.sizeDelta = stage_container;
            return stage_container;
        }

        #endregion
        #region 方法
        /// <summary>
        /// 畫面移動
        /// </summary>
        private void _auto_move()
        {
        }
        /// <summary>
        /// 場景邊緣判定，玩家：回推；物件：破壞。
        /// </summary>
        /// <param name="colliders">碰撞區域</param>
        /// <param name="i">編號，沒有特別意義，只有程式內部會說明用途</param>
        private void _borderVelocity(Collider[] colliders, int i)
        {
            foreach (Collider col in colliders)
            {
                if (col.tag.Contains("Player"))
                {
                    col.transform.position = new Vector3(
                        Mathf.Clamp(col.transform.position.x, -stage_container.x/2+ minBorder.x, stage_container.x/2 + maxBorder.x),
                        Mathf.Clamp(col.transform.position.y , -stage_container.y / 2+ minBorder.y + transform.position.y, stage_container.y / 2 + transform.position.y + + maxBorder.y)
                        );
                }
                else if (col.tag.Contains("Enemy") || col.tag.Contains("Block"))
                {
                    if (i == 1) ColliderSystem.StageColliderEvent(col.gameObject);
                }
            }
            //print($"透過 {colliders[0].tag}");

        }
        #endregion

        private void OnTriggerExit(Collider other)
        {
            ColliderSystem.StageColliderEvent(other.gameObject);
            if (includeTag.Length > 0)
            {
                foreach (var e in includeTag)
                    if (other.tag.Contains(e))
                        ManageCenter.mgCenter.ObjectDestory(other.gameObject, true);
            }
            //print(other.tag);
        }
        #region 事件
        private void Awake()
        {
            MainCam = Camera.main;
            cinemachine = cinemachine ?? FindObjectOfType<CinemachineVirtualCamera>().GetComponentInChildren<CinemachineVirtualCamera>();
            Stage_boxBorder = GetComponent<BoxCollider>();
            Space_Rect = GetComponent<RectTransform>();
        }
        private void Start()
        {
            Physics.gravity = gravity3;
            Application.targetFrameRate = 120;
            GetBoxborder();
            //box_range = stage_container; // 若要改成手動大小，註解掉這行。
            // 左右牆壁判定
            nbox_range = new Vector3(box_range.y, box_range.x / 2, box_range.z);

        }
        private void FixedUpdate()
        {
            // 場景邊緣動作
            _borderVelocity(Physics.OverlapBox(stage_position + Vector3.up * box_offset.y, box_range, Quaternion.identity), 0);
            _borderVelocity(Physics.OverlapBox(stage_position - (Vector3.up * box_offset.y * 0.85f), box_range, Quaternion.identity), 1);
            _borderVelocity(Physics.OverlapBox(stage_position + Vector3.left * box_offset.x, nbox_range, Quaternion.identity), 2);
            _borderVelocity(Physics.OverlapBox(stage_position - (Vector3.left * box_offset.x * 1.1f), nbox_range, Quaternion.identity), 3);

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = box_color;
            Vector3 nbox_range = new Vector3(box_range.y, box_range.x/2, box_range.z); // 讓編輯器模式下也能看到邊界
            Gizmos.DrawCube(stage_position + Vector3.up * box_offset.y, box_range);             // 0上邊界
            Gizmos.DrawCube(stage_position - (Vector3.up * box_offset.y * 0.85f), box_range);   // 1下邊界
            // 左右判定
            Gizmos.DrawCube(stage_position + Vector3.left * box_offset.x, nbox_range);          // 2右邊界
            Gizmos.DrawCube(stage_position - (Vector3.left * box_offset.x * 1.1f), nbox_range); // 3左邊界
        }
        #endregion
    }
}

/*
 * 原本防止超過邊界寫法，會有回彈的問題。
    Rigidbody c_rig = col.GetComponent<Rigidbody>();
    Vector3 bound = c_rig.velocity;
    // 根據編號來決定反彈的方向：
    /// 0 往下
    /// 1 往上
    /// 2 往右
    /// 3 往左
    switch (i)
    {
        case 0: bound.y = -0.1f; break;
        case 1: bound.y = 0.1f; break;
        case 2: bound.x = 0.1f; break;
        case 3: bound.x = -0.1f; break;
        default: break;
    }
    c_rig.velocity = bound;
 * 
 * 
 * */