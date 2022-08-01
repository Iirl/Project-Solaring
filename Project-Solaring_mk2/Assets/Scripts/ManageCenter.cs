using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        [SerializeField, Tooltip("�Ȱ����")]
        CanvasGroup menus;
        [SerializeField, Tooltip("�����޲z")]
        ManageEnd mEnd;
        [SerializeField, Header("�w�]���;����O�A�Ы��w�@�Ӳ��;�����")]
        Object_Generator gener_class;
        [SerializeField, Tooltip("�]�w���;�����")]
        private string[] objectName = { "�ɵ��~", "�k��"};

        /// <summary>
        /// GameUI Interface.
        /// </summary>
        [SerializeField, Header("����UI����"), Tooltip("UI �Z�����")]
        TMP_Text ui_Dist;
        [SerializeField, Tooltip("UI �U�����")]
        TMP_Text ui_fuel;
        [SerializeField, Tooltip("UI ����(Read Only)")]
        public int UI_moveDistane = 0, UI_fuel = 100;
        public int MoveDistance { get { return UI_moveDistane; } }
        private bool uiLoad = false;
        private bool isEnd = false, isPause = false;
        private bool isGen_sup = false, isGen_mto = false;

        #region �@����� (Public Feild)
        public CanvasGroup canvas_select;
        #endregion

        #region �@�Τ�k (Public Method)

        public Vector3 GetStagePOS()
        {
            return ss_ctl.transform.position;
        }
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
        /// �U�ƸɥR��ơA��J�w�ȼW�[�U�ơC
        /// </summary>
        /// <param name="f">���w�n�ɪ���</param>
        public void FuelReplen(int f)
        {
            if (isEnd && rocket_ctl.RocketS1.x > 0) { CancelInvoke("GameOver"); return; }
            rocket_ctl.PutRocketSyn(-f, rocket_ctl.GetBasicSPD());
            rocket_ctl.ADOClipControl(0);
        }

        ///////////// ���ͪ���

        /// <summary>
        /// ����ͦ��t��
        /// ���w�Z���H���ͦ�����
        /// </summary>
        /// <param name="i"></param>
        public void GenerAuto()
        {
            if (!isGen_sup)
            {
                AutoGenerate(0, true);
                isGen_sup = true;
            }
            if (!isGen_mto)
            {
                MeteoGenerate();
                isGen_mto = true;
            }

            CancelInvoke("GenerAuto");
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

            try { gener_class = GameObject.Find(objectName[i]).GetComponent<Object_Generator>(); }
            catch { print("���O���󤣦s�b�A�Э��s�]�w"); }
            
        }
        /// <summary>
        /// �եΦ۰ʲ��͸ɵ��~
        /// </summary>
        /// <param name="i">�ɵ��~���O</param>
        /// <param name="rotate">�O�_�H���ͦ���V</param>
        public void AutoGenerate(int i, bool rotate = false)
        {
            AsignGenerate(0); //������ �ɫ~���O
            Vector3 st_border = ss_ctl.GetBoxborder();
            if (gener_class != null) gener_class.Static_gen(ss_ctl.transform.position.y, i, Random.Range(-st_border.x/2, st_border.x / 2), Random.Range(st_border.y, st_border.y*2), rotate);
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
#if UNITY_EDITOR
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/Empty.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_BlueCrystal01.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_BlueCrystal02.prefab", typeof(Object)),
                AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Crystal/GP_PurpCrystal01.prefab", typeof(Object))
#endif
            };

            // Third: Input on the subobject.
            gener_class.Random_Metro(Gid, pfabs);
        }
        /// <summary>
        /// �M�������ơC
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
            int i = ss_mag.GetScenes();
            switch (i)
            {
                case 0: break;
                default: break;
            }
        }

        public void test()
        {
            // Change Asign Object to List String.
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
                uiLoad = true;
            }
            else
            {
                uiLoad = false;
                CanvasCtrl(canvas_select,true);
                CancelInvoke("FadeIn");
                Time.timeScale = 0.05f;
            }
        }
        /// <summary>
        /// �H�X�ʵe
        /// </summary>
        public void FadeOut()
        {
            if (canvas_select.alpha > 0)
            {
                canvas_select.alpha -= 0.01f;
                uiLoad = true;
            }
            else
            {
                uiLoad = false;
                CanvasCtrl(canvas_select);
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
                if (menus.alpha != 0 && menus != null)
                {
                    isPause = false;
                    CanvasCtrl(menus);
                }
                Invoke("GameOver", times);
            }

        }
        #endregion

        #region ���a�����k�Ψƥ�
        /// <summary>
        /// �e���s�ն}��
        /// </summary>
        /// <param name="cvs"></param>
        /// <param name="on"></param>
        private void CanvasCtrl(CanvasGroup cvs, bool on = false)
        {
            if (on)
            {
                cvs.alpha = 1;
                cvs.interactable = true;
                cvs.blocksRaycasts = true;

            }
            else
            {
                cvs.alpha = 0;
                cvs.interactable = false;
                cvs.blocksRaycasts = false;

            }

        }
        /// <summary>
        /// �]�wUI������
        /// </summary>
        private void show_UI()
        {
            Vector3 stage_pos = GetStagePOS();
            UI_moveDistane = (int)stage_pos.y;
            UI_fuel = (int)rocket_ctl.RocketS1.x;
            if (UI_fuel <= 0 && !isEnd) { CheckGame(true, 5f); }//�����C�����󤧤@
            if (ui_Dist != null) ui_Dist.text = $"{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"{UI_fuel}";
        }
        /// <summary>
        /// �Ȱ����}��
        /// </summary>
        public void show_Menu()
        {
            if (menus != null)
            {
                canvas_select = menus;
                if (!isPause)
                {
                    // ��{
                    isPause = true;
                    InvokeRepeating("FadeIn", 0, 0.05f);
                }
                else
                {
                    // �H�X
                    Time.timeScale = 1f;
                    InvokeRepeating("FadeOut", 0, 0.001f);
                    isPause = false;
                }
                rocket_ctl.ControlChange();
                //ss_ctl.enabled = !ss_ctl.enabled;
                space_ctl.enabled = !space_ctl.enabled;
            }
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
                    if (kLS) print("C+S button");
                    else if (kB) print("B button");
                    else if (kM) print("M button");
                    else if (kO) print("O button");
                    else if (kP) print("P button");
                    else if (kQ) print("Q button");
                }
                else if (isAtl)
                {
                    if (kLS) print("A+S button");
                    else if (kN) print("N button");
                    else if (kR) print("R button");
                }
                else if (isLS)
                {
                    if (kC) print("C button");
                    else if (kU) print("U button");
                }
            }

        }
        private void Update()
        {
            show_UI();
            if (Input.GetAxisRaw("Menu") == 1 && !uiLoad && !isEnd) show_Menu();
            SpecialistKeyInput(Input.GetKey(KeyCode.LeftControl),
                Input.GetKey(KeyCode.LeftAlt),
                Input.GetKey(KeyCode.LeftShift)
                );
            //// ---���|�۰ʥͦ�����---
            if (UI_moveDistane % 20 == 0 && !isGen_sup) Invoke("GenerAuto", 1);  // �H���ͦ��ɫ~
            else isGen_sup = false;
            if (UI_moveDistane % 30 == 1 && !isGen_mto) Invoke("GenerAuto", 1);  // �H���ͦ���ê��
            else isGen_mto = false;
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
