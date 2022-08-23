using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace solar_a
{
    public class GenerateSystem : MonoBehaviour
    {
        [SerializeField, Header("������")]
        GeneratorData generData;
        [SerializeField, Header("�s��I�s")]
        private bool continues;
        [SerializeField, Header("���w�Z���}�l���ͪ���")]
        private float generDestan =0;

        // ���o�����������O
        ManageCenter mgc;
        Object_Generator.Generater obGenerate;
        Object_Generator.ObjectArray gener_list = new();
        bool preLoadInvoke;
        /// <summary>
        /// ���q�ͦ��G�u�P�_�O�_���ફ��C
        /// </summary>
        /// <param name="rotate"></param>
        public void NormalGenerate(bool rotate = false) => Static_gen(generData.grtRandomRoation);
        /// <summary>
        /// �l����ͦ�
        /// </summary>
        private void SubObjGenerate() => Random_gen(generData.grtRandomRoation);
        private void StaticPointGen() => Static_gen(generData.grtRandomRoation, false);
        public void Test()
        {
            
        }

        #region �����ˬd�t��

        public void _ReadOBJList()
        {
            // ����Ū���}�C�M�檺���e
            gener_list.ReadList();
            print(gener_list.Count);
        }
        /// <summary>
        /// �۰ʧR�����w���l���O�A����ͦ��ɦ۰ʧP�w�O�_�W�L�ͦ��W���C
        /// </summary>
        /// <param name="target">Ĳ�o�P��������</param>
        public void Destroys(GameObject target, bool destTime = false)
        {
            // print(target.name);
            // ��Ū��ID�A�M����M�椤�ۦPID�A�R���ӲM��s���C
            int id = target.transform.GetInstanceID();
            int key = gener_list.FindKeys(id);
            //print(key);
            if (key != -1)
            {
                // �����M�椺�e
                gener_list.RemoveAt(key);
                // ���檫��R��
                if (destTime) Destroy(target, 1f);
                else Destroy(target);
            }
            else
            {
                if (destTime) Destroy(target, 1f);
                else Destroy(target);
            }
        }
        /// <summary>
        /// �M�Ū���t��
        /// </summary>
        /// <param name="clear">�ĤG���O�@�ATrue �~�|�ҰʲM��</param>
        public void Destroys(bool clear)
        {
            if (!clear) return;
            int max = gener_list.Count;
            for (int i = 0; i < max; i++)
            {
                ArrayList al = (ArrayList)gener_list[0];
                Object obj = (Object)al[1];
                gener_list.RemoveAt(0);
            }
        }

        /// <summary>
        /// �Y���W�����M��������A����o�q�{�������C
        /// 1. �W�L�ͦ��W���C
        /// 2. �M�楼�s���ơA���w�s�b����W�C(�ק令�W�L�ͦ�����b��)
        /// 3. �W�L�e���@�w�Z���C
        /// </summary>
        private void DestroysOnBug(Vector3 w_dist)
        {
            if (gener_list.Count > 0 && gener_list.Count > generData.grtLimit)
            {
                Object obj = gener_list.GetObject(1);
                gener_list.RemoveAt(0);
                Destroy(obj);
            }
            else if (gener_list.Count < 1 && transform.childCount > generData.grtLimit)
            {
                int max = transform.childCount;
                for (int bug_i = 0; bug_i < max; bug_i++) Destroy(transform.GetChild(bug_i).gameObject);
            }
            //// �����P�w�e���y�����;��~�P�A�ҥH�p�G�n�M�������P���ʡA�n�O�o���w��������m...�C
            for (int bug_i = 0; bug_i < transform.childCount; bug_i++)
            {
                GameObject child_gob = transform.GetChild(bug_i).gameObject;
                float dis = Vector3.Distance(child_gob.transform.position, w_dist);
                if (dis > 100)
                {
                    try { gener_list.RemoveAt(gener_list.FindKeys(child_gob.GetInstanceID())); }
                    catch (System.Exception) { }
                    Destroy(child_gob);
                }
            }

        }
        /// <summary>
        /// ����ͦ��`�t�ΡC
        /// �p�G�ƻ򳣤��ǤJ���ܡA�ܤ֭n�ǤJ�ثeY����m�A�~�|�b�e���W�ݨ�C
        /// </summary>
        /// <param name="worldOffset">�ثe�������y��</param>
        /// <param name="i">���w�ͦ��C������</param>
        /// <param name="isPos">�O�_�H����m</param>
        /// <param name="isRoate">�O�_�H������</param>
        /// <returns></returns>
        private GameObject Generator_EMP(Vector3 worldOffset, bool isRoate = false ,bool random=true)
        {
            if (generData.grtObject == null) return null;
            DestroysOnBug(worldOffset);
            //show
            //Vector3 st_border = mgc.GetStageBorder();
            Vector3 random_v3 = new(Random.Range(-generData.grtPos.x, generData.grtPos.x),
                Random.Range(0f, generData.grtPos.y + generData.grtOffset),
                Random.Range(-generData.grtPos.z, generData.grtPos.z)
            );
            obGenerate = new(gameObject, generData.grtObject);                      // �b���w����m[M]���ͫ��w������[G]
            obGenerate.Create_v3 += (random) ? random_v3 + worldOffset : generData.grtPos + worldOffset;                // ����ͦ�����m�A�|�̾ڳ]�w����m���ܡC
            obGenerate.Create_r3 = (isRoate) ? Random.rotation : generData.grtRot;  // ����ͦ���V�O�_�H���A�w�]���_�C
            obGenerate.destoryTime = generData.grtdestTime;
            Object created = obGenerate.Generates();            
            gener_list.Add(created);                                          // �[�J�ͦ��C��C
                                                                              //Destroys(generob.GetParent());
                                                                              //generob.ObjectMessegeInfo();

            // �Y���]�w�P���ɶ��A�h�[�W�P�����p�ɡC            
            //Destroys(created.GetComponent<Transform>().gameObject, true);
            return created.GetComponent<Transform>().gameObject;
        }
        #endregion
        #region ���󲣥ͤ�k�������G�w�I�B���w�B�H���αa���l����ͦ��C
        /// <summary>
        /// ²�����ͪ����k�C
        /// </summary>
        private void Static_gen(bool isRot) => Generator_EMP(new Vector3(0, mgc.GetStagePOS().y, 0),isRot);
        private void Static_gen(bool isRot, bool isRnd) => Generator_EMP(new Vector3(0, mgc.GetStagePOS().y, 0), isRot, isRnd);
        private void Static_gen(float locY, bool isRotate) => Generator_EMP(new Vector3(0, locY, 0), isRotate);

        /// <summary>
        /// �N�����H���ͦ��b�e����
        /// </summary>
        /// <param name="locY">�ثe�Ŷ���Y�b</param>
        /// <param name="isRotated">����O�_�H������</param>
        /// <returns>�^�Ǭ��ͦ�����A�Χ@����U�@�Ӱʧ@�ϥΡC</returns>
        private int Random_gen(bool isRotated)
        {
            Vector3 stage = new Vector3(0, mgc.GetStagePOS().y, 0);
            GameObject parentOB = Generator_EMP(stage, isRotated);
            Random_Metro(parentOB, generData.grtSubObject);
            return -1;
        }
        /// <summary>
        /// ���󤤪�����ͦ�
        /// </summary>
        /// <param name="parent">������ID</param>
        /// <param name="TG">�n�ͦ�������</param>
        private void Random_Metro(GameObject parent, List<Object> TG)
        {
            // 
            parent = parent.transform.GetChild(0).gameObject;
            int sub_count = 0;
            int sub_max = parent.transform.childCount;
            while (sub_count < sub_max)
            {
                int rnd = Random.Range(0, TG.Count);


                GameObject PAOB = parent.transform.GetChild(0).gameObject; ;
                try
                {
                    // ���o�l���󪺤�����
                    PAOB = parent.transform.GetChild(sub_count).gameObject;
                    // �l����
                }
                catch (System.Exception)
                {
                    print("SubgenID Fail");
                    break;
                }

                // �ͦ�����
                if (Random.value < generData.grtProb)
                {
                    Object_Generator.Generater sgen = new(PAOB, TG[rnd], PAOB.transform.position, PAOB.transform.rotation * Quaternion.AngleAxis(30, Vector3.right));
                    sgen.Generates();
                }
                sub_count++;
            }
        }

        #endregion
        /// <summary>
        /// �����ͦ����e�t��
        /// �|�ھ����O�M�w���ͪ���k
        /// </summary>
        private void SwitchState()
        {
            if (transform.childCount < generData.grtLimit)
            {
                switch (generData.grtClass)
                {
                    case GenerClass.Normal:
                        NormalGenerate();
                        break;
                    case GenerClass.Meteorite:
                        SubObjGenerate();
                        break;
                    case GenerClass.StaticPoint:
                        StaticPointGen();
                        break;
                    default:
                        break;
                }
            }
            //print("�I�s����");
        }

        private IEnumerator IntervalGenerate()
        {
            while (ManageCenter.UI_moveDistane < generDestan && !preLoadInvoke) yield return null;          // �Z�����w
            if (continues) InvokeRepeating("SwitchState", generData.grtIntervalTime, generData.grtWaitTime);// ����P�@����
            else Invoke("SwitchState", generData.grtIntervalTime);           
           
        }
        private void Awake()
        {
            mgc = FindObjectOfType<ManageCenter>();
        }
        private void Start()
        {
            preLoadInvoke = IsInvoking();
            StartCoroutine(IntervalGenerate());

        }
        private void Update()
        {
            //AutoGenerate();
        }
    }

}

