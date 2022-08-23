using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Cinemachine;
using Unity.VisualScripting;

namespace solar_a
{
    /// <summary>
    /// ��������t�ΡA��ɧP�w�γ������e���ܤơC
    /// Box Collider limit to Rocket can't over the border.
    /// </summary>
    public class SceneStage_Control : MonoBehaviour
    {
        #region �ݩ�
        [SerializeField, Header("������T"), Tooltip("�����j�p(Read Only)")]
        private Vector3 stage_container;
        [SerializeField, Tooltip("������}(Read Only)")]
        private Vector3 stage_position;
        [SerializeField, Header("���������i���ܼ�"), Tooltip("�P�w�ϰ��C��")]
        private Color box_color = Color.cyan;
        [SerializeField, Header("�P�w�ϰ�j�p")]
        private Vector3 box_range = Vector3.zero;
        [SerializeField, Header("�P�w�ϰ�첾")]
        private Vector3 box_offset = Vector3.zero;
        [SerializeField,Header("������b����(Clamp)�ϰ�L��")]
        private Vector2 minBorder;
        [SerializeField]
        private Vector2 maxBorder;
        Vector3 nbox_range;
        [SerializeField, Header("�������ʳt��")]
        public float speed;
        public float finishDistane;
        [SerializeField, Header("��L����Ĳ�o��������")]
        private string[] includeTag;
        [SerializeField, Header("���O�վ�")]
        private Vector3 gravity3 = new(0, -9.8f, 0);
        Camera MainCam;
        CinemachineVirtualCamera cinemachine;
        BoxCollider Stage_boxBorder;
        RectTransform Space_Rect;
        #endregion

        #region ���챱���k
        /// <summary>
        /// ���o�e����ɡG�{���ҰʮɩΪ̽վ�j�p�ɭn�I�s����ƽվ���ɡC
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
        #region ��k
        /// <summary>
        /// �e������
        /// </summary>
        private void _auto_move()
        {
        }
        /// <summary>
        /// ������t�P�w�A���a�G�^���F����G�}�a�C
        /// </summary>
        /// <param name="colliders">�I���ϰ�</param>
        /// <param name="i">�s���A�S���S�O�N�q�A�u���{�������|�����γ~</param>
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
            //print($"�z�L {colliders[0].tag}");

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
        #region �ƥ�
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
            //box_range = stage_container; // �Y�n�令��ʤj�p�A���ѱ��o��C
            // ���k����P�w
            nbox_range = new Vector3(box_range.y, box_range.x / 2, box_range.z);

        }
        private void FixedUpdate()
        {
            // ������t�ʧ@
            _borderVelocity(Physics.OverlapBox(stage_position + Vector3.up * box_offset.y, box_range, Quaternion.identity), 0);
            _borderVelocity(Physics.OverlapBox(stage_position - (Vector3.up * box_offset.y * 0.85f), box_range, Quaternion.identity), 1);
            _borderVelocity(Physics.OverlapBox(stage_position + Vector3.left * box_offset.x, nbox_range, Quaternion.identity), 2);
            _borderVelocity(Physics.OverlapBox(stage_position - (Vector3.left * box_offset.x * 1.1f), nbox_range, Quaternion.identity), 3);

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = box_color;
            Vector3 nbox_range = new Vector3(box_range.y, box_range.x/2, box_range.z); // ���s�边�Ҧ��U�]��ݨ����
            Gizmos.DrawCube(stage_position + Vector3.up * box_offset.y, box_range);             // 0�W���
            Gizmos.DrawCube(stage_position - (Vector3.up * box_offset.y * 0.85f), box_range);   // 1�U���
            // ���k�P�w
            Gizmos.DrawCube(stage_position + Vector3.left * box_offset.x, nbox_range);          // 2�k���
            Gizmos.DrawCube(stage_position - (Vector3.left * box_offset.x * 1.1f), nbox_range); // 3�����
        }
        #endregion
    }
}

/*
 * �쥻����W�L��ɼg�k�A�|���^�u�����D�C
    Rigidbody c_rig = col.GetComponent<Rigidbody>();
    Vector3 bound = c_rig.velocity;
    // �ھڽs���ӨM�w�ϼu����V�G
    /// 0 ���U
    /// 1 ���W
    /// 2 ���k
    /// 3 ����
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