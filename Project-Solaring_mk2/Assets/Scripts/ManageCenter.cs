using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace solar_a
{
    /// <summary>
    /// ��汱��t�ΡA�եΨ�L�t�Ϊ��\��A����P�@�P��k�C
    /// ���t������b���w�s�b����W������W
    /// �ثe��b�G GameManage_UI
    /// �|�N�i���ƽեΪ���k��󦹳B�A�b��L����W�즳����k�p���ݭn���ƨϥΤ]�|��b�o�C
    /// </summary>
    public class ManageCenter : MonoBehaviour
    {
        [SerializeField, Header("�t�Υ\���`��"), Tooltip("����t��")]
        Space_Controll space_ctl;
        [SerializeField, Tooltip("���b����t��")]
        Rocket_Controll rocket_ctl;
        [SerializeField, Tooltip("��������t��")]
        SceneStage_Control ss_ctl;
        [SerializeField, Tooltip("�����t��")]
        ManageScene ss_mag;
        [SerializeField, Tooltip("�w�]���;����O�A�Ы��w�@�Ӳ��;�����")]
        Object_Generator gener_class;
        [SerializeField, Tooltip("�����޲z")]
        ManageEnd mEnd;
        [SerializeField, Header("����UI����"), Tooltip("UI �Z�����")]
        TMP_Text ui_Dist;
        [SerializeField, Tooltip("UI �U�����")]
        TMP_Text ui_fuel;
        [SerializeField, Tooltip("UI ����(Read Only)")]
        private int UI_moveDistane = 0, UI_fuel = 100;
        public int MoveDistance { get { return UI_moveDistane; } }
        private bool isEnd = false;

        #region �@����� (Public Feild)
        public CanvasGroup canvas_select;
        #endregion

        #region �@�Τ�k (Public Method)

        ///////////// ���b�������
        // Time.deltaTime * Mathf.Abs(ss_ctl.Space_speed) * 0.25f;

        /// <summary>
        /// ���b�P�������ʡA�t�ת��ֺC���|�v�T��F�Z���A�V�֥i�H�V����F�ت��C
        /// </summary>
        public void MoveAction()
        {
            float unit = Time.deltaTime * rocket_ctl.RocketS1.y; // ���Z���A�ϥ� deltaTime �i�H������s�W�v�����~�C
            ss_ctl.transform.position += Vector3.up * unit; // ��������
            if (rocket_ctl.RocketS1.x > 0) rocket_ctl.PutRocketSyn(unit * 0.2f); // �U���ܤ�
            else rocket_ctl.PutRocketSyn(0, rocket_ctl.GetBasicSPD() / 2);         // �U�ƥκɡA�����g�@

        }
        /// <summary>
        /// �U�ƸɥR�t��
        /// </summary>
        /// <param name="f">���w�n�ɪ���</param>
        public void FuelReplen(int f)
        {
            if (isEnd && rocket_ctl.RocketS1.x > 0) { CancelInvoke(); return; }
            rocket_ctl.PutRocketSyn(-f, rocket_ctl.GetBasicSPD());
        }

        ///////////// ���ͪ���

        public void TestGener(int i)
        {
            AutoGenerate(i, false);
        }

        /// <summary>
        /// �����w�]�����;����O�A�]�t�ɵ��~���͡C
        /// �o�̬O�]�w�n�ϥΦ�����O�C
        /// �ѩ󴫭Ӽg�k�A�o�̼ȮɪŸm
        /// </summary>
        /// <param name="i">���w���;����e�A�ثe�������;��p�U�G
        /// 0. �w�]�A���� �@�몫�����O �C
        /// 1. ���� �k�����O �C
        /// 2. ���� �ľ����O �C
        /// </param>
        public void AsignGenerate(int i)
        {
            string name = "";
            switch (i)
            {
                case 0: name = "ObjectGenerator"; break;
                case 1: name = "ObjectGenerator2"; break;
                default: break;
            }
            gener_class = GameObject.Find(name).GetComponent<Object_Generator>();
        }
        /// <summary>
        /// �եΦ۰ʲ��͸ɵ��~�t��
        /// </summary>
        /// <param name="i">�ɵ��~���O</param>
        /// <param name="rotate">�O�_�H���ͦ���V</param>
        public void AutoGenerate(int i, bool rotate = false)
        {
            AsignGenerate(0); //������ �ɫ~���O
            if (gener_class != null) gener_class.Static_gen(ss_ctl.transform.position.y, Random.Range(0, 3), Random.value * 15, Random.value * 7);
        }
        /// <summary>
        /// ���ͪ��a�l���󪺵{���C
        /// �|���Τ@��ͦ����覡�ͦ������A���o�Ӫ���ID�A�̦��ͦ��l����C
        /// ���p�G�ϥιw�m�����ܡA����|�ͦ��b�l����W�C
        /// </summary>
        public void MeteoGenerate()
        {
            AsignGenerate(1); //������ �k�����O
            int obj = Random.Range(0, 3);
            int Gid = gener_class.Random_gen(ss_ctl.transform.position.y, false, obj); // Fist: Generate SubObject.
            // Second: Load Insub Prefabs.
            List<Object> pfabs = new()
            {
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/Empty.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_BlueCrystal01.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_BlueCrystal02.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_PurpCrystal01.prefab", typeof(Object))
            };

            // Third: Input on the subobject.
            gener_class.Random_Metro(Gid, pfabs);
        }
        /// <summary>
        /// �M������t�ΡC
        /// �Ҧ�����q�e���W�������n�g�L�o�Ө禡�C
        /// </summary>
        /// <param name="obj"></param>
        public void ObjectDestory(GameObject obj)
        {
            gener_class.Destroys(obj);
        }
        /////////////////////�� ��//////////////////////////////
        /// <summary>
        /// �w�����ͪ����k�A�|�̷ӥثe��������m����C
        /// i �N��ثe�������s���A�i�H�b�o�̫��w���������ӮɡA�Φ�ؤ覡�ͦ���ت���C
        /// </summary>
        public void PreOrderGen()
        {
            int i = ss_mag.LoadScenes();
            switch (i)
            {
                case 0: break;
                default: break;
            }
        }

        public void test()
        {
            gener_class.Destroys(true);
        }
        ///////////// ����ܤƬ���
        /// <summary>
        /// �H�J�ʵe
        /// </summary>
        public void FadeIn()
        {
            if (canvas_select.alpha < 1)
            {
                canvas_select.alpha += 0.1f;
            }
            else
            {
                canvas_select.alpha = 1;
                canvas_select.interactable = true;
                canvas_select.blocksRaycasts = true;
                CancelInvoke("FadeIn");
            }

        }
        /// <summary>
        /// �H�X�ʵe
        /// </summary>
        public void FadeOut()
        {
            if (canvas_select.alpha > 0)
            {
                canvas_select.alpha -= 0.1f;
            }
            else
            {
                canvas_select.alpha = 0;
                canvas_select.interactable = false;
                canvas_select.blocksRaycasts = false;
                CancelInvoke("FadeOut");
            }

        }

        /// <summary>
        /// �C�������P�w�t��
        /// 1. �S���U��
        /// 2. ���������
        /// </summary>
        /// <param name="end"></param>
        public void CheckGame(bool end = false, float times=0.2f)
        {
            if (end && !isEnd)
            {
                Invoke("GameOver", times);
            }

        }
        #endregion

        #region ���a�����k�Ψƥ�
        /// <summary>
        /// �]�wUI������
        /// </summary>
        private void show_UI()
        {
            UI_moveDistane = (int)ss_ctl.stage_position.y;
            UI_fuel = (int)rocket_ctl.RocketS1.x;
            if (UI_fuel <= 0 && !isEnd) { CheckGame(true, 5f); }//�����C�����󤧤@
            if (ui_Dist != null) ui_Dist.text = $"Distance:{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"Fuel:{UI_fuel}";
        }
        /// <summary>
        /// �C�������B�z���p
        /// </summary>
        private void GameOver()
        {
            mEnd.enabled = true;
            rocket_ctl.ControlChange();
            rocket_ctl.enabled = false;
            ss_ctl.enabled = false;
            isEnd = true;
            CancelInvoke();
        }
        /// <summary>
        /// �S����O�A���a��J�ֳt�䪺�ɭԷ|�X�{���ʹ��\��C
        /// </summary>
        private void SpecialistKeyInput(bool isCtrl, bool isAtl, bool isLS)
        {
            if (!isCtrl && !isAtl && !isLS) return;
            else
            {
                bool kCtrl = Input.GetKey(KeyCode.LeftControl);
                bool kAtl = Input.GetKey(KeyCode.LeftAlt);
                bool kLS = Input.GetKey(KeyCode.LeftShift);
                bool kB = Input.GetKeyDown(KeyCode.B);
                bool kC = Input.GetKeyDown(KeyCode.C);
                bool kN = Input.GetKeyDown(KeyCode.N);
                bool kM = Input.GetKeyDown(KeyCode.M);
                bool kO = Input.GetKeyDown(KeyCode.O);
                bool kP = Input.GetKeyDown(KeyCode.P);
                bool kQ = Input.GetKeyDown(KeyCode.Q);
                bool kR = Input.GetKeyDown(KeyCode.R);
                bool kU = Input.GetKeyDown(KeyCode.U);
                if (isCtrl)
                {
                    if (kAtl && kO)
                    {
                        //print("Debug");
                        //GameObject testOB = ((Transform)Resources.InstanceIDToObject(27318)).gameObject;
                        CanvasGroup testOB = GameObject.Find("TestObject").GetComponent<CanvasGroup>();
                        testOB.alpha = testOB.alpha != 0 ? 0:1;
                        testOB.interactable = !testOB.interactable;
                        testOB.blocksRaycasts = !testOB.blocksRaycasts;

                    }
                    else if (kB) print("B button");
                    else if (kM) print("M button");
                    else if (kO) print("O button");
                    else if (kP) print("P button");
                    else if (kQ) print("Q button");
                }
                else if (isAtl)
                {
                    if (kN) print("N button");
                    if (kR) print("R button");
                }
                else if (isLS)
                {
                    if (kC) print("C button");
                    if (kU) print("U button");
                }
            }

        }
        private void Update()
        {
            show_UI();
            SpecialistKeyInput(Input.GetKey(KeyCode.LeftControl),
                Input.GetKey(KeyCode.LeftAlt),
                Input.GetKey(KeyCode.LeftShift)
                );
        }
        #endregion

        #region �m�J����
        /*Ctrl+b �}�Ҷ¬}�H�������G�ݭn�¬}�ͦ����ʵe�ιϤ�
        Shift+c ���b�����f���G�ݭn�@�[�f��
        Shift+u ���b������
        Alt+n ���b����
        Alt+r ���s�ͦ��ɵ��P��ê��*/

        #endregion
    }
}
