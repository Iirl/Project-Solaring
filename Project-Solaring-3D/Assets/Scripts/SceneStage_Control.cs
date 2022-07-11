using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Cinemachine;

namespace solar_a
{
    public class SceneStage_Control : MonoBehaviour
    {
        #region �ݩ�
        BoxCollider Stage_boxBorder;
        RectTransform Space_Rect;
        [SerializeField, Header("������T"), Tooltip("�����j�p(Read Only)")]
        private Vector3 stage_container;
        [SerializeField, Tooltip("������}(Read Only)")]
        public Vector3 stage_position;

        [SerializeField, Header("���������i���ܼ�"), Tooltip("���ʳt��"), Space]
        public float Space_speed = 10;
        [SerializeField, Header("�P�w�ϰ��C��")]
        private Color box_color = Color.cyan;
        [SerializeField, Header("�P�w�ϰ�j�p")]
        private Vector3 box_range = Vector3.zero;
        [SerializeField, Header("�P�w�ϰ�첾")]
        private Vector3 box_offset = Vector3.zero;
        Vector3 nbox_range;

        CinemachineVirtualCamera cinemachine;
        #endregion

        #region ��k
        /// <summary>
        /// �e������
        /// </summary>
        private void _auto_move()
        {
            if (Space_Rect.position.x != 0) Space_Rect.position = new Vector2(0, Space_Rect.position.y);
            Space_Rect.position += Vector3.up * Time.deltaTime * Space_speed;
            stage_position = transform.position;
        }
        /// <summary>
        /// ������t�P�w
        /// </summary>
        /// <param name="colliders">�I���ϰ�</param>
        /// <param name="i">�s���A�S���S�O�N�q�A�u���{�������|�����γ~</param>
        private void _borderVelocity(Collider[] colliders, int i)
        {
            foreach (Collider col in colliders)
            {                
                if (col.tag.Contains("Player"))
                {                    
                    Rigidbody c_rig = col.GetComponent<Rigidbody>();
                    // �ھڽs���ӨM�w�ϼu����V�G
                    /// 0 ���U
                    /// 1 ���W
                    /// 2 ���k
                    /// 3 ����
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

        #region ���챱���k
        /// <summary>
        /// ���o�e����ɡG�{���ҰʮɩΪ̽վ�j�p�ɭn�I�s����ƽվ���ɡC
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

        #region �ƥ�
        private void Awake()
        {
            cinemachine = GetComponentInChildren<CinemachineVirtualCamera>();
            Stage_boxBorder = GetComponent<BoxCollider>();
            Space_Rect = GetComponent<RectTransform>();
            GetBoxborder();
            //box_range = stage_container; // �Y�n�令��ʤj�p�A���ѱ��o��C
            // ���k����P�w
            nbox_range = new Vector3(box_range.y, box_range.x, box_range.z);
        }
        private void Update()
        {
            // ��������
            _auto_move();
        }
        private void FixedUpdate()
        {
            // ������t�ʧ@
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
            // ���k�P�w
            Vector3 nbox_range = new Vector3 (box_range.y, box_range.x, box_range.z);
            Gizmos.DrawCube(stage_position + Vector3.left * box_offset.x, nbox_range);
            Gizmos.DrawCube(stage_position - (Vector3.left * box_offset.x * 1.1f), nbox_range);
        }
        #endregion
    }
}