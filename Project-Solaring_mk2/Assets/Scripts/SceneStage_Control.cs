using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Cinemachine;

namespace solar_a
{
    /// <summary>
    /// 場景控制系統，邊界判定及場景內容的變化
    /// </summary>
    public class SceneStage_Control : MonoBehaviour
    {
        #region 屬性
        [SerializeField, Header("中控系統")]
        ManageCenter mgCenter;
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
        Vector3 nbox_range;
        [SerializeField, Header("重力調整")]
        private Vector3 gravity3 = new(0,-9.8f,0);

        CinemachineVirtualCamera cinemachine;
        BoxCollider Stage_boxBorder;
        RectTransform Space_Rect;
        #endregion

        #region 方法
        /// <summary>
        /// 畫面移動
        /// </summary>
        private void _auto_move()
        {
            if (Space_Rect.position.x != 0) Space_Rect.position = new Vector2(0, Space_Rect.position.y);
            mgCenter.MoveAction();
            stage_position = transform.position; // 場景資訊
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
                    Rigidbody c_rig = col.GetComponent<Rigidbody>();
                    Vector3 bound = c_rig.velocity;
                    // 根據編號來決定反彈的方向：
                    /// 0 往下
                    /// 1 往上
                    /// 2 往右
                    /// 3 往左
                    switch (i) {  
                        case 0: bound.y = -0.1f; break;
                        case 1: bound.y = 0.1f; break;
                        case 2: bound.x = 0.1f; break;
                        case 3: bound.x = -0.1f; break; 
                        default: break;
                    }
                    c_rig.velocity = bound;
                } else  if (col.tag.Contains("Enemy") || col.tag.Contains("Block"))
                {
                    if (i!=0) mgCenter.ObjectDestory(col.gameObject);
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
            Physics.gravity = gravity3;
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
            _borderVelocity(Physics.OverlapBox(stage_position - (Vector3.up * box_offset.y * 1f), box_range, Quaternion.identity), 1);
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
        private void OnTriggerExit(Collider other)
        {
            if (other.tag.Contains("Block") || other.tag.Contains("Enemy")) mgCenter.ObjectDestory(other.gameObject);
            if (other.tag.Contains("Player"))
            {
                // 第二層防止衝破大氣層保險
                Vector3 check = (other.transform.position);
                if (check.x > stage_container.x / 2-2) check.x = stage_container.x /2 -2 + transform.position.x;
                else if (check.x < -stage_container.x / 2+2) check.x = -stage_container.x / 2+2 + transform.position.x;
                if (check.y < -stage_container.y/2+1 + transform.position.y) check.y = transform.position.y - stage_container.y / 2;
                else if (check.y > stage_container.y/2 + transform.position.y) check.y = stage_container.y / 2 + transform.position.y;
                other.transform.position = check;
            }
        }
        #endregion
    }
}