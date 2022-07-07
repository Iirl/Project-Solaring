using UnityEngine;
using System.Collections;

/// <summary>
/// �����b�ϰ�Ŷ������ͪ��󪺵{��
/// </summary>
namespace solar_a
{
    public class Object_Generator : MonoBehaviour
    {

        #region �ݩ�
        //SceneStage_Control Scene_ctl;

        [SerializeField, Header("�ͦ���m")]
        Object MainObject;
        [SerializeField, Header("�ͦ�����"), Tooltip("��J���w������A�̦n�O�� Prefabs �L���ɮ�")]
        Object Generate;
        [SerializeField, Tooltip("���w�����m")]
        Vector3 Generate_pos = Vector3.zero;
        [SerializeField, Tooltip("���w�������")]
        Quaternion Generate_rot = Quaternion.identity;
        [SerializeField, Header("���w����ͦ��ƶq�W��")]
        int Generate_limit = 10;
        ArrayList gener_list = new ArrayList();

        #region ���;����O class Generater
        public class Generater
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
            public Vector3 Create_v3 = new(Random.Range(-20, 20), Random.Range(8.0f, 13.2f), Random.Range(-20, 20));
            public Quaternion Create_r3 = Quaternion.identity;
            private GameObject Parent, Target;
            private Object OBParent, OBTarget;
            public Object Ob_Parent
            {
                get { return OBParent; }
                set { OBParent = value; Parent = GameObject.Find(value.name); }
            }
            public Object Ob_Target
            {
                get { return OBTarget; }
                set { OBTarget = value; Target = GameObject.Find(value.name); }
            }

            /// <summary>
            /// �s�@����ɷ|�۰ʫ��w�D����B�ͦ����H�Φ�m�M���બ�A
            /// </summary>
            /// <param name="parent">�D����A�n�b���Ӫ���W�ͦ�</param>
            /// <param name="target">�ؼЪ���A�ƻ� Object �|�Q�ͦ�</param>
            public Generater(Object parent, Object target)
            {
                Ob_Target = target;
                Ob_Parent = parent;
            }
            /// <summary>
            /// �s�@����ɤ�ʫ��w�D����B�ͦ����H�Φ�m�M���બ�A
            /// </summary>
            /// <param name="parent">�D����A�n�b���Ӫ���W�ͦ�</param>
            /// <param name="target">�ؼЪ���A�ƻ� Object �|�Q�ͦ�</param>
            /// <param name="pos">�ؼЪ��󪺤T���y��</param>
            /// <param name="rot">�ؼЪ��󪺱���y��</param>
            public Generater(Object parent, Object target, Vector3 pos, Quaternion rot)
            {
                Ob_Target = target;
                Ob_Parent = parent;
                Parent = GameObject.Find(parent.name);
                Target = GameObject.Find(target.name);
                Create_v3 = pos;
                Create_r3 = rot;

            }

            /// <summary>
            /// �۰ʲ��ͪ���A�|�ھڬJ�����M�w�ͦ������e
            /// </summary>
            public Object Generates()
            {
                Object target = Ob_Target;
                Transform parent_trs = Parent.transform;
                Object cloned = Instantiate(target, Create_v3, Create_r3, parent_trs);
                return cloned;
            }


            /// <summary>
            /// ���󲣥ͪ��T���A�����ΡC
            /// </summary>
            public void ObjectMessegeInfo()
            {
                Object target = Ob_Target;
                Object parent = Ob_Parent;
                print($"The target is {target.name} that will be generate in {Create_v3}, {Create_r3}");
                print($"Will be generate in the {parent.name}.");

            }
            public GameObject GetParent()
            {
                return Parent.gameObject;
            }
            public GameObject GetTarget()
            {
                return Target;
            }

        }


        #endregion
        #endregion

        #region �T�w�����k
        private void readList()
        {
            foreach (GameObject item in gener_list)
            {
                if (item != null ) print(item.name);
            }
        }
        /// <summary>
        /// �ھڭ��O�ݩʲ��ͪ���C
        /// </summary>
        public void Static_gen()
        {
            Generater sgen = new Generater(MainObject, Generate, Generate_pos , Generate_rot);
            sgen.Generates();
            Destroys(sgen.GetParent());
        }

        /// <summary>
        /// �ھڭ��O���w�ͦ���m�C
        /// </summary>
        /// <param name="locY">�[�W�ثe��������m</param>
        public void Static_gen(float locY)
        {
            Generater sgen;
            Object sgen_o;
            if (Generate != null && MainObject != null)
            {
                sgen = new Generater(MainObject, Generate);
                //sgen.ObjectMessegeInfo();
                sgen.Create_v3 = Generate_pos;
                sgen.Create_v3.y += locY;
                sgen.Create_r3 = Generate_rot;
                sgen_o = sgen.Generates();
                gener_list.Add(sgen_o);
                Destroys(sgen.GetParent());
            }
            readList();
        }
        /// <summary>
        /// �R�����w���l���O
        /// </summary>
        /// <param name="target">������W��</param>
        public void Destroys(GameObject target)
        {
            if (gener_list.Count > 0 && gener_list.Count > Generate_limit)
            {
                GameObject t = target.transform.GetChild(0).gameObject;
                gener_list.RemoveAt(0);
                Destroy(t);
            }
        }
        #endregion

        #region �H�����ͤ�k

        #endregion

        public void Random_gen(float locY, bool isRotated)
        {
            Generater sgen;
            Object sgen_o;
            if (Generate != null && MainObject != null)
            {
                sgen = new Generater(MainObject, Generate);
                sgen.Create_v3.y += locY;
                if (isRotated) sgen.Create_r3 = Random.rotation;
                sgen_o = sgen.Generates();
                gener_list.Add(sgen_o);
                Destroys(sgen.GetParent());
            }
            print(gener_list.Count);
        }


        #region �ɵ��~��k

        #endregion

        #region �ƥ�
        #endregion
    }


}