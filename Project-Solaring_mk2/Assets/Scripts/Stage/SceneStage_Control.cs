using UnityEngine;
using Cinemachine;

namespace solar_a
{
    /// <summary>
    /// 場景控制系統，邊界判定及場景內容的變化。
    /// Box Collider limit to Rocket can't over the border.
    /// </summary>
    public class SceneStage_Control : MonoBehaviour
    {
        #region 屬性
        [SerializeField, Header("場景資訊(Read Only)"), Tooltip("場景大小")]
        private Vector3 stage_boxCollider;
        [SerializeField, Header("場景碰撞器微調")]
        private Vector3 stage_boxOffset = Vector3.forward *5;
        [SerializeField, Header("使用Gizmos大小")]
        private bool stageInGizmos;
        [SerializeField, Header("手繪物理碰撞區域"), Tooltip("判定區域顏色")]
        private Color box_color = Color.cyan;
        [SerializeField, Tooltip("場景位址")]
        private Vector3 box_position = Vector3.one * -1;
        [SerializeField, Tooltip("判定區域位移")]
        private Vector3 box_offset = Vector3.zero;
        [SerializeField, Tooltip("判定區域大小")]
        private Vector3 box_range = Vector3.zero;
        [SerializeField, Header("限制火箭移動(Clamp)區域微調")]
        private Vector2 minBorder;
        [SerializeField]
        private Vector2 maxBorder;
        Vector3 nbox_range;
        [SerializeField, Header("其他標籤觸發消除物件")]
        private string[] includeTags = { "Enemy", "Block" };
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
            stage_boxCollider.x = Mathf.Round(MainCam.aspect * cinemachine.m_Lens.OrthographicSize * 2) + stage_boxOffset.x;     //width
            stage_boxCollider.y = Mathf.Round((1 / MainCam.aspect) * stage_boxCollider.x) + stage_boxOffset.y; //heigh
            stage_boxCollider.z = stage_boxCollider.x + stage_boxOffset.z;
            if (stageInGizmos)
            {
                stage_boxCollider.x = box_range.x;
                stage_boxCollider.z = box_range.z;
            }
            Stage_boxBorder.size = stage_boxCollider;
            Space_Rect.sizeDelta = stage_boxCollider;
            return stage_boxCollider;
        }

        #endregion
        #region 方法
        /// <summary>
        /// 場景邊緣判定，玩家：回推；物件：碰撞系統=>延遲破壞。
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
                        Mathf.Clamp(col.transform.position.x, -stage_boxCollider.x / 2 + minBorder.x, stage_boxCollider.x / 2 + maxBorder.x),
                        Mathf.Clamp(col.transform.position.y, -stage_boxCollider.y / 2 + minBorder.y + transform.position.y, stage_boxCollider.y / 2 + transform.position.y + +maxBorder.y)
                        );
                }
            }
            //print($"透過 {colliders[0].tag}");

        }
        #endregion

        private void OnTriggerExit(Collider other)
        {
            if (other.name.Contains("Player")) { 
                Collider[] colliders = { other };
                _borderVelocity(colliders, 0);
            }
            // 常見狀態交由碰撞系統處理，額外物件標籤則直接延遲銷毀。
            if (includeTags.Length > 0)
            {
                foreach (var e in includeTags)
                    if (other.tag.Contains(e))
                        other.StageColliderEvent();
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
            _borderVelocity(Physics.OverlapBox(box_position + Vector3.up * box_offset.y, box_range, Quaternion.identity), 0);
            _borderVelocity(Physics.OverlapBox(box_position - (Vector3.up * box_offset.y * 0.85f), box_range, Quaternion.identity), 1);
            _borderVelocity(Physics.OverlapBox(box_position + Vector3.left * box_offset.x, nbox_range, Quaternion.identity), 2);
            _borderVelocity(Physics.OverlapBox(box_position - (Vector3.left * box_offset.x * 1.1f), nbox_range, Quaternion.identity), 3);

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = box_color;
            Vector3 nbox_range = new Vector3(box_range.y, box_range.x / 2, box_range.z); // 讓編輯器模式下也能看到邊界
            Gizmos.DrawCube(box_position + Vector3.up * box_offset.y, box_range);             // 0上邊界
            Gizmos.DrawCube(box_position - (Vector3.up * box_offset.y * 0.85f), box_range);   // 1下邊界
            // 左右判定
            Gizmos.DrawCube(box_position + Vector3.left * box_offset.x, nbox_range);          // 2右邊界
            Gizmos.DrawCube(box_position - (Vector3.left * box_offset.x * 1.1f), nbox_range); // 3左邊界
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