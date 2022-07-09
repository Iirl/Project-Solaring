using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Cinemachine;

namespace solar_a
{
    public class SceneStage_Control : MonoBehaviour
    {
        #region 屬性
        BoxCollider Stage_boxBorder;
        RectTransform Space_Rect;
        [SerializeField, Header("場景資訊"), Tooltip("場景大小(Read Only)")]
        private Vector3 stage_container;
        [SerializeField, Tooltip("場景位址(Read Only)")]
        public Vector3 stage_position;

        [SerializeField, Header("場景相關可調變數"), Tooltip("移動速度"), Space]
        public float Space_speed = 10;
        [SerializeField, Header("判定區域顏色")]
        private Color box_color = Color.cyan;
        [SerializeField, Header("判定區域大小")]
        private Vector3 box_range = Vector3.zero;
        [SerializeField, Header("判定區域位移")]
        private Vector3 box_offset = Vector3.zero;
        Vector3 nbox_range;

        CinemachineVirtualCamera cinemachine;
        #endregion

        #region 方法
        /// <summary>
        /// 畫面移動
        /// </summary>
        private void _auto_move()
        {
            if (Space_Rect.position.x != 0) Space_Rect.position = new Vector2(0, Space_Rect.position.y);
            Space_Rect.position += Vector3.up * Time.deltaTime * Space_speed;
            stage_position = transform.position;
        }
        /// <summary>
        /// 場景邊緣判定
        /// </summary>
        /// <param name="colliders">碰撞區域</param>
        /// <param name="i">編號，沒有特別意義，只有程式內部會說明用途</param>
        private void _borderVelocity(Collider[] colliders, int i)
        {
            foreach (Collider col in colliders)
            {                
                if (col.tag.Contains("Player"))
                {                    
                    Rigidbody c_rig = col.GetComponent<Rigidbody>();
                    // 根據編號來決定反彈的方向：
                    /// 0 往下
                    /// 1 往上
                    /// 2 往右
                    /// 3 往左
                    switch (i) {  
                        case 0: c_rig.velocity += Vector3.down; break;
                        case 1: c_rig.velocity += Vector3.up; break;
                        case 2: c_rig.velocity += Vector3.right; break;
                        case 3: c_rig.velocity += Vector3.left; break; 
                        default: break;
                    }
                } else  if (col.tag.Contains("Enemy"))
                {
                    Destroy(col.gameObject,2);
                }
            } 
        }
        #endregion

        #region 全域控制方法
        /// <summary>
        /// 取得畫面邊界：程式啟動時或者調整大小時要呼叫此函數調整邊界。
        /// </summary>
        public Vector3 GetBoxborder()
        {
            Camera cv_mc = Stage_boxBorder.GetComponentInChildren<Camera>();
            stage_container.x = Mathf.Round(cv_mc.aspect * cinemachine.m_Lens.OrthographicSize * 2);     //width
            stage_container.y = Mathf.Round((1 / cv_mc.aspect) * stage_container.x); //heigh
            stage_container.z = stage_container.x + 2;
            Stage_boxBorder.size = stage_container;
            Space_Rect.sizeDelta = stage_container;
            return stage_container;
        }

        #endregion

        #region 事件
        private void Awake()
        {
            cinemachine = GetComponentInChildren<CinemachineVirtualCamera>();
            Stage_boxBorder = GetComponent<BoxCollider>();
            Space_Rect = GetComponent<RectTransform>();
            GetBoxborder();
            //box_range = stage_container; // 若要改成手動大小，註解掉這行。
            // 左右牆壁判定
            nbox_range = new Vector3(box_range.y, box_range.x, box_range.z);
        }
        private void Update()
        {
            // 場景移動
            _auto_move();
        }
        private void FixedUpdate()
        {
            // 場景邊緣動作
            _borderVelocity(Physics.OverlapBox(stage_position + box_offset, box_range, Quaternion.identity), 0);
            _borderVelocity(Physics.OverlapBox(stage_position - (Vector3.up * box_offset.y * 0.85f), box_range, Quaternion.identity), 1);
            _borderVelocity(Physics.OverlapBox(stage_position + Vector3.left * box_offset.x, nbox_range, Quaternion.identity), 2);
            _borderVelocity(Physics.OverlapBox(stage_position - (Vector3.left * box_offset.x * 1.1f), nbox_range, Quaternion.identity), 3);

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = box_color;
            Gizmos.DrawCube(stage_position + Vector3.up * box_offset.y, box_range);
            Gizmos.DrawCube(stage_position - (Vector3.up * box_offset.y * 0.85f), box_range);
            // 左右判定
            Vector3 nbox_range = new Vector3 (box_range.y, box_range.x, box_range.z);
            Gizmos.DrawCube(stage_position + Vector3.left * box_offset.x, nbox_range);
            Gizmos.DrawCube(stage_position - (Vector3.left * box_offset.x * 1.1f), nbox_range);
        }
        #endregion
    }
}