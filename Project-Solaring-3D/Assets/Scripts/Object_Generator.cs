using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �����b�ϰ�Ŷ������ͪ��󪺵{��
/// </summary>
namespace solar_a
{
    [AddComponentMenu("Transform/���󲣥;�")]
    public class Object_Generator : MonoBehaviour
    {

        #region �ݩ�
        //SceneStage_Control Scene_ctl;

        [SerializeField, Header("�ͦ���m")]
        List<GameObject> MainObject = new List<GameObject>();
        [SerializeField, Header("�ͦ�����"), Tooltip("��J���w������A�̦n�O�� Prefabs �L���ɮ�")]
        List<Object> Generate = new List<Object>();
        [SerializeField, Tooltip("���w�����m")]
        Vector3 Generate_pos = Vector3.zero;
        [SerializeField, Tooltip("���w�������")]
        Quaternion Generate_rot = Quaternion.identity;
        [SerializeField, Tooltip("���w����ͦ��b�|�A�Ȧb�H���ͦ��M��")]
        Vector3 Generate_posRaidus = new Vector3(20, 10, 20);
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
            public Vector3 Create_v3 = Vector3.zero;
            public Quaternion Create_r3 = Quaternion.identity;
            private GameObject Parent;
            public Object OBTarget, OBCloned;

            /// <summary>
            /// �s�@����ɷ|�۰ʫ��w�D����B�ͦ����H�Φ�m�M���બ�A
            /// </summary>
            /// <param name="parent">�D����A�n�b���Ӫ���W�ͦ�</param>
            /// <param name="target">�ؼЪ���A�ƻ� Object �|�Q�ͦ�</param>
            public Generater(GameObject parent, Object target)
            {
                OBTarget = target;
                Parent = parent;
            }
            /// <summary>
            /// �s�@����ɤ�ʫ��w�D����B�ͦ����H�Φ�m�M���બ�A
            /// </summary>
            /// <param name="parent">�D����A�n�b���Ӫ���W�ͦ�</param>
            /// <param name="target">�ؼЪ���A�ƻ� Object �|�Q�ͦ�</param>
            /// <param name="pos">�ؼЪ��󪺤T���y��</param>
            /// <param name="rot">�ؼЪ��󪺱���y�СA��J0��ܮM�ιw�]��</param>
            public Generater(GameObject parent, Object target, Vector3 pos, Quaternion rot)
            {
                OBTarget = target;
                Parent = parent;
                Create_v3 = pos;
                Create_r3 = rot;

            }

            /// <summary>
            /// �۰ʲ��ͪ���A�|�ھڬJ�����M�w�ͦ������e
            /// </summary>
            public Object Generates()
            {
                Object cloned = Instantiate(OBTarget, Create_v3, Create_r3, Parent.transform);
                OBCloned = cloned;
                return cloned;
            }


            /// <summary>
            /// ���󲣥ͪ��T���A�����ΡC
            /// </summary>
            public void ObjectMessegeInfo()
            {
                Object target = OBTarget;
                Object parent = Parent;
                print($"The target is {target.name} that will be generate in {Create_v3}, {Create_r3}");
                print($"Will be generate in the {parent.name}.");

            }
            public GameObject GetParent()
            {
                return Parent;
            }

        }


        #endregion

        #endregion


        public void ReadList()
        {
            int count = 0;
            foreach (GameObject item in gener_list)
            {
                if (item != null) print(item.name);
                count++;
            }
        }
        public GameObject ReadList(int target)
        {
            int count = 0;
            GameObject idx = null;
            foreach (GameObject item in gener_list)
            {
                if (item != null && count == target) idx = item;
                count++;
            }
            return idx;
        }
        /// <summary>
        /// �۰ʧR�����w���l���O�A����ͦ��ɦ۰ʧP�w�O�_�W�L�ͦ��W���C
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
        public void Destroys(int idx)
        {
            if (gener_list.Count > 0)
            {
                GameObject t = ReadList(idx);
                gener_list.RemoveAt(idx);
                Destroy(t);
            }
        }
        public void Destroys()
        {
            if (gener_list.Count < Generate_limit)
            {
                gener_list.RemoveAt(0);
            }
        }
        /// <summary>
        /// ��l�ͦ���ơA�p�G�ƻ򳣤��ǤJ���ܡA�ܤ֭n�ǤJ�ثeY����m�A�~�|�b�e���W�ݨ�C
        /// </summary>
        /// <param name="yWorld">�ثe�������y��</param>
        /// <param name="i"></param>
        /// <param name="isPos"></param>
        /// <param name="isRoate"></param>
        /// <returns></returns>
        private Generater Generator_EMP(Vector3 worldOffset, int i = 0, bool isPos = true, bool isRoate = false)
        {
            Vector3 random_v3 = new(Random.Range(-Generate_posRaidus.x, Generate_posRaidus.x),
                Random.Range(0f, Generate_posRaidus.y),
                Random.Range(-Generate_posRaidus.z, Generate_posRaidus.z)
            );
            Generater generob = new Generater(MainObject[i], Generate[i], Generate_pos, Generate_rot);
            generob.Create_v3 += worldOffset;                     // �վ��@�ɦ�m
            if (isPos) generob.Create_v3 = random_v3;          // ����ͦ���m�O�_�H���A�w�]���O�C
            if (isRoate) generob.Create_r3 = Random.rotation;  // ����ͦ���V�O�_�H���A�w�]���_�C
            gener_list.Add(generob.Generates());               // �[�J�ͦ��C��C
            Destroys(generob.GetParent());
            return generob;

        }
        #region �T�w�����k
        /// <summary>
        /// �ھڭ��O�ݩʲ��ͪ���C
        /// </summary>
        public void Static_gen()
        {
            Generator_EMP(transform.position);
        }

        /// <summary>
        /// �ھڭ��O���w�ͦ���m�C
        /// </summary>
        /// <param name="locY">�[�W�ثe��������m</param>
        /// <param name="x">���w x �b�y�Ц첾</param>
        /// <param name="y">���w y �b�y�Ц첾</param>
        public void Static_gen(float locY, float x, float y, int i)
        {
            Generater sgen;
            Vector3 stage = new Vector3(0, locY, 0);
            if (Generate.Count > 0 && MainObject.Count > 0)
            {
                sgen = Generator_EMP(stage, i);
                //sgen.ObjectMessegeInfo();
                sgen.Create_v3 = Generate_pos;
                sgen.Create_v3.y += locY + y;
                sgen.Create_v3.x += x;
                sgen.Create_r3 = Generate_rot;
            }
        }
        #endregion

        #region �H�����ͤ�k
        /// <summary>
        /// �N�����H���ͦ��b�e����
        /// </summary>
        /// <param name="locY">�ثe�Ŷ���Y�b</param>
        /// <param name="isRotated">����O�_�H������</param>
        /// <returns>�^�Ǭ��ͦ�����A�Χ@����U�@�Ӱʧ@�ϥΡC</returns>
        public int Random_gen(float locY, bool isRotated, int i)
        {
            Generater sgen;
            Vector3 stage = new Vector3(0, locY, 0);
            if (Generate.Count > 0 && MainObject.Count > 0)
            {
                sgen = Generator_EMP(stage, i, isRotated);
                return sgen.OBCloned.GetInstanceID();
            }
            return -1;
        }

        /// <summary>
        /// ���󤤪�����ͦ�
        /// </summary>
        /// <param name="PAID">������ID</param>
        /// <param name="TG">�n�ͦ�������</param>
        public void Random_Metro(int PAID, GameObject TG)
        {

            // �l����p��G�������l����ID�Z��=2+4+10=16(0),20(1),24(2),28(3),32(4)�C
            int sub_count = 16, i = 1;
            int sub_max = sub_count + (4 * 4);
            GameObject PA = null;
            while (sub_count <= sub_max)
            {
                try
                {
                    // �ഫID�������

                    PA = ((Transform)EditorUtility.InstanceIDToObject(PAID - 2)).gameObject;
                    PA = PA.transform.GetChild(i).gameObject;
                    // �l����

                }
                catch (System.Exception)
                {
                    break;
                }

                // �ͦ�����
                Generater sgen = new(PA, TG, PA.transform.position, PA.transform.rotation * Quaternion.AngleAxis(30, Vector3.right));
                sub_count += 4; i++;
            }
        }
        #endregion

    }


}