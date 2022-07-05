using UnityEngine;

/// <summary>
/// �����b�ϰ�Ŷ������ͪ��󪺵{��
/// </summary>
namespace solar_a
{
    public class Object_Generator : MonoBehaviour
    {

        #region �ݩ�
        System_function sysFuntion;

        [SerializeField, Header("�ͦ���m")]
        Object MainObject;
        [SerializeField, Header("�ͦ�����"), Tooltip("��J���w������A�̦n�O�� Prefabs �L���ɮ�")]
        Object Generate;
        [SerializeField, Tooltip("���w�����m")]
        Vector3 Generate_pos = Vector3.zero;
        [SerializeField, Tooltip("���w�������")]
        Quaternion Generate_rot = Quaternion.identity;

        #region ���O
        class Generater
        {
            private float x { get; set; }
            private float y { get; set; }
            private float z { get; set; }
            private float xr { get; set; }
            private float yr { get; set; }
            private float zr { get; set; }
            public float X_pos_gen { get { return x; } set { x = value; Create_v3 = new Vector3(x, y, z); } }
            public float Y_pos_gen { get { return y; } set { y = value; Create_v3 = new Vector3(x, y, z); } }
            public float Z_pos_gen { get { return z; } set { z = value; Create_v3 = new Vector3(x, y, z); } }
            public float X_rot_gen { get { return xr; } set { xr = value; Create_r3 = new Quaternion(xr, yr, zr, 0); } }
            public float Y_rot_gen { get { return yr; } set { yr = value; Create_r3 = new Quaternion(xr, yr, zr, 0); } }
            public float Z_rot_gen { get { return zr; } set { zr = value; Create_r3 = new Quaternion(xr, yr, zr, 0); } }
            public Vector3 Create_v3 { get; set; }
            public Quaternion Create_r3 { get; set; }
            private GameObject Parent, Target;
            private Object Ob_Parent, Ob_Target;

            /// <summary>
            /// �s�@����ɷ|�۰ʫ��w�D����B�ͦ����H�Φ�m�M���બ�A
            /// </summary>
            /// <param name="parent">�D����A�n�b���Ӧa��ͦ�</param>
            /// <param name="target">�ؼЪ���A�ƻ� Object �|�Q�ͦ�</param>
            public Generater(Object parent, Object target)
            {
                Ob_Target = target;
                Ob_Parent = parent;
                Parent = GameObject.Find(parent.name);
                Target = GameObject.Find(target.name);
                Create_v3 = Parent.transform.position;
                Create_r3 = Parent.transform.rotation;

            }

            /// <summary>
            /// �۰ʲ��ͪ���A�|�ھڬJ�����M�w�ͦ������e
            /// </summary>
            public void Generates()
            {
                Object target = Ob_Target;
                Transform parent_trs = Parent.transform;
                Instantiate(target, Create_v3, Create_r3, parent_trs);
            }


            public void Print_ObjectMessege()
            {
                Object target = Ob_Target;
                Object parent = Ob_Parent;
                print($"The target is {target.name} that will be generate in {Create_v3}, {Create_r3}");
                print($"Will be generate in the {parent.name}.");
            }

        }


        #endregion

        #endregion

        #region �T�w�����k

        public void Static_gen()
        {
            string obj = "UFO_clone";
            //float gener_pos_x = Random.Range(-sysFuntion.stage_x, sysFuntion.stage_x);
            //float gener_pos_y = Random.Range(0, sysFuntion.stage_y);
            //////

            Generater sgen;
            if (Generate != null && MainObject != null)
            {
                sgen = new Generater(MainObject, Generate);
                sgen.Create_v3 = Generate_pos;
                sgen.Create_r3 = Generate_rot;
                sgen.Print_ObjectMessege();
                sgen.Generates();
            }


        }

        /// <summary>
        /// �R�����w���l���O
        /// </summary>
        /// <param name="Parent">������W��</param>
        public void Destroys(GameObject Parent)
        {
            if (Parent.transform.childCount != 0)
            {
                GameObject target = Parent.transform.GetChild(0).gameObject;
                Destroy(target);
            }
        }
        #endregion

        #region �H�����ͤ�k

        #endregion

        #region �ɵ��~��k

        #endregion

        #region �ƥ�
        #endregion
    }
}