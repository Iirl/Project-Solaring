using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        #region �޲z����t��
        [SerializeField, Header("�t�Υ\���`��"), Tooltip("����t��")]
        Space_Controll space_ctl;
        [SerializeField, Tooltip("���b����t��")]
        Rocket_Controll rocket_ctl;
        [SerializeField, Tooltip("��������t��")]
        SceneStage_Control ss_ctl;
        [SerializeField, Tooltip("�����t��")]
        ManageScene ss_mag;
        [SerializeField, Tooltip("�����޲z")]
        ManageEnd mEnd;
        #endregion
        /// <summary>
        /// ���;�����
        /// </summary>
        //("�w�]���;����O�A�Ы��w�@�Ӳ��;�����")]
        Object_Generator gener_class;
        [SerializeField, Tooltip("�]�w���;�����")]
        private string[] objectName = { "�ɵ��~", "�k��" };
        [SerializeField, Tooltip("�]�w�h�ֶZ�����ͤ@������")]
        private int[] objectDistance = { 50, 30 };
        [SerializeField, Header("�]�w���;����O"),Tooltip("0:�@��ͦ�, 1:�k�����O")]
        private GenerClass[] objectClass;
        /// <summary>
        /// GameUI Interface.
        /// </summary>
        [SerializeField, Header("����UI����"), Tooltip("UI �Z�����")]
        TMP_Text ui_Dist;
        [SerializeField, Tooltip("UI �U�Ƥ�r")]
        TMP_Text ui_fuel;
        [SerializeField, Tooltip("UI �U�Ʊ�")]
        Image ui_fuelbar;
        [SerializeField]
        GameObject[] EnergyPlus;
        [SerializeField, Tooltip("UI ����(Read Only)")]
        public int UI_moveDistane = 0, UI_fuel = 100;
        public int MoveDistance { get { return UI_moveDistane; } }
        /// <summary>
        /// ���e������
        /// </summary>
        [SerializeField, Header("�Ȱ����")]
        GameObject pauseUI;
        [SerializeField, Tooltip("�Ȱ����e���t��")]
        CanvasGroup menus;
        [Tooltip("�{�������ܥثe���e���A�i�H���γ]�w�C")]
        public CanvasGroup canvas_select;
        [SerializeField, Header("�e���H�Ƴt��")]
        Vector2 fadeSpeed = Vector2.zero + Vector2.one * 0.01f;
        #region ���a�վ\��� (Private Feild)
        private bool isGen = false;

        #endregion


        public void X_PowerMode()
        {
            StaticSharp.isPowerfullMode = !StaticSharp.isPowerfullMode;
            transform.Find("AudioBox").GetComponent<AudioSource>().Play();
        }
        /// <summary>
        /// �@�Τ�k (Public Method)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetStagePOS()
        {
            return ss_ctl.transform.position;
        }
        public Vector3 GetStageBorder()
        {
            return ss_ctl.GetBoxborder();
        }
        #region ���b�������
        // Time.deltaTime * Mathf.Abs(ss_ctl.Space_speed) * 0.25f;

        /// <summary>
        /// ���b�P�������ʡA�t�ת��ֺC���|�v�T��F�Z���A�V�֥i�H�V����F�ت��C
        /// </summary>
        public void MoveAction()
        {
            float unit = Time.deltaTime * ss_ctl.speed; // ���Z���A�ϥ� deltaTime �i�H������s�W�v�����~�C
            //if (!rocket_ctl.rc_dtion.IsStay) 
                ss_ctl.transform.position += Vector3.up * unit / 2; // ��������
            if (rocket_ctl.RocketS1.x > 0) rocket_ctl.PutRocketSyn(rocket_ctl.Unit_fuel * Time.deltaTime);   // �U���ܤ�
            //else rocket_ctl.PutRocketSyn(0, rocket_ctl.GetBasicInfo().y / 2);               // �U�ƥκɡA�����g�@

        }
        /// <summary>
        /// �U�ƸɥR��ơA��J�w�ȼW�[�U�ơC
        /// </summary>
        /// <param name="f">���w�n�ɪ���</param>
        public void FuelReplen(int f)
        {
            float nowFuel = rocket_ctl.RocketS1.x;
            rocket_ctl.PutRocketSyn(f, rocket_ctl.RocketBasic.y);
            rocket_ctl.ADOClipControl(0);
            if (StaticSharp.Conditions == State.End && nowFuel > 0) CancelInvoke("GameState");
        }
        #endregion
        #region ���ͪ���
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
            Vector3 st_border = ss_ctl.GetBoxborder();
            if (gener_class != null) gener_class.Static_gen(GetStagePOS().y, i, Random.Range(-st_border.x / 2, st_border.x / 2), Random.Range(st_border.y, st_border.y * 2), rotate);
        }
        /// <summary>
        /// ���ͪ��a�l���󪺵{���C
        /// �|���Τ@��ͦ����覡�ͦ������A���o�Ӫ���ID�A�̦��ͦ��l����C
        /// ���p�G�ϥιw�m�����ܡA����|�ͦ��b�l����W�C
        /// </summary>
        public void MeteoGenerate()
        {
            AsignGenerate(1);
            int obj = Random.Range(0, 3);
            int Gid = gener_class.Random_gen(GetStagePOS().y + 40, false, obj); // Fist: Generate SubObject.
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
            GenerateSystem objGS =  obj.transform.GetComponentInParent<GenerateSystem>();
            if (!objGS) { Destroy(obj); return; }
            objGS.Destroys(obj);
            //gener_class.Destroys(obj);
        }
        #endregion
        #region �� �� �� ��
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
        // �i�J���~��
        public void InToStation()
        {
            StaticSharp.Rocket_INFO = rocket_ctl.RocketS1;
            StaticSharp.Rocket_BASIC = rocket_ctl.RocketBasic;
            ss_mag.SaveLeveInform();
            ss_mag.LoadScenes("Station");
        }

        ///////////// ����ܤƬ���
        ///
        public void GetState()
        {
            print(condition.GetState());
        }
        /// <summary>
        /// ���H�J�ʵe
        /// </summary>
        public void FadeIn()
        {
            if (canvas_select.alpha < 1)
            {
                canvas_select.alpha += 0.1f;
            }
            else
            {
                condition.Next();
                GameState();
                CanvasCtrl(canvas_select, true);
                CancelInvoke("FadeIn");
                Time.timeScale = 0.05f;
            }
        }
        /// <summary>
        /// ���H�X�ʵe
        /// </summary>
        public void FadeOut()
        {
            if (canvas_select.alpha > 0)
            {
                canvas_select.alpha -= 0.1f;
            }
            else
            {
                condition.Previous();
                GameState();
                Time.timeScale = 1f;
                CanvasCtrl(canvas_select);
                CancelInvoke("FadeOut");
            }

        }
        #endregion
        /// <summary>
        /// �C�������P�w�t��
        /// 1. �S���U��
        /// 2. ���������
        /// </summary>
        /// <param name="end"></param>
        public void CheckGame(bool end = false, float times = 0.2f)
        {
            if (end && ((int)StaticSharp.Conditions) != 4)
            {
                // �ˬd�O�_���b�Ȱ��A�Y�O���ܱj����Ȱ�
                if (menus.alpha != 0 && menus != null)
                {
                    CanvasCtrl(menus);
                }
                condition.Dead();
                Invoke("GameState", times);
            }

        }
        StaticSharp.GameCondition condition = new StaticSharp.GameCondition();
        #region ���a�����k�Ψƥ�
        /// <summary>
        /// �e���s�ն}��
        /// </summary>
        /// <param name="cvs"></param>
        /// <param name="on"></param>
        private void CanvasCtrl(CanvasGroup cvs, bool on = false)
        {

            cvs.alpha = on ? 1 : 0;
            cvs.interactable = on;
            cvs.blocksRaycasts = on;
        }
        /// <summary>
        /// �]�wUI������
        /// </summary>
        private void show_UI()
        {
            Vector3 stage_pos = GetStagePOS();
            UI_moveDistane = (int)stage_pos.y;
            UI_fuel = (int)rocket_ctl.RocketS1.x;
            if (UI_fuel <= 100) ui_fuelbar.fillAmount = UI_fuel / 100f;
            else
            {   // �W�L 100 �������ή檬������
                ui_fuelbar.fillAmount = 1;
                int level = (int)((UI_fuel - 100) / rocket_ctl.fuel_overcapacity);
                if (EnergyPlus.Length >= level)
                {
                    int count = 0;
                    foreach (GameObject g in EnergyPlus)
                    {
                        g.SetActive(level > count);
                        count++;
                    }
                }
            }
            if (UI_fuel <= 0 && (int)StaticSharp.Conditions != 3) { CheckGame(true, 5f); }//�����C�����󤧤@
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
                if (StaticSharp.Conditions != State.Pause)
                {
                    // ��{
                    condition.Next();
                    InvokeRepeating("FadeIn", 0, fadeSpeed.x);
                }
                else
                {
                    // �H�X
                    condition.Previous();
                    InvokeRepeating("FadeOut", 0, fadeSpeed.y);
                }
            }
        }
        /// <summary>
        /// �C�����A�B�z���p
        /// �ھڥثe�����A�����C���i�檬�p
        /// END = �����C��
        /// Running ���� = �}�_�Ȱ����
        /// </summary>
        private void GameState()
        {
            bool isEnd = false;
            if (condition.GetState() == "End")
            {// GameOver
                transform.Find("UI_Pause").transform.Find("Btn_Back_en").gameObject.SetActive(false);
                isEnd = true;
                mEnd.enabled = true;
                ss_ctl.enabled = false;
                rocket_ctl.enabled = false;
            }
            else if (StaticSharp.Conditions != State.Running)
            {// �p�G���O���檬�A�A�h�Ȱ��Ŷ��A�éI�s�Ȱ����C
                space_ctl.enabled = !space_ctl.enabled;
            }
            if (pauseUI != null) pauseUI.SetActive(true);
            rocket_ctl.ControlChange(!isEnd);
            CancelInvoke("GameState");
        }
        /// <summary>
        /// ����ͦ���ơA�קK�P�@�ɶ����j�q�ͦ��C
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private IEnumerator PathAutoGenerObject(int idx = 0)
        {
            //���������w���O
            switch (objectClass[idx])
            {
                case GenerClass.Normal:AsignGenerate(0);
                    break;
                case GenerClass.Meteorite:AsignGenerate(1);
                    break;
                default:
                    break;
            }
            //AsignGenerate((int)gener_class); 
            isGen = true;
            switch (idx)
            {
                case 0: AutoGenerate(0, true); break;
                case 1: MeteoGenerate(); break;
                default: break;
            }
            yield return new WaitForSeconds(1);

            while (isGen) yield return isGen = (UI_moveDistane % objectDistance[idx] != idx) ? false : true;

        }
        

        private void Start()
        {
            //print($"�ثe�����s�����G{PlayerPrefs.GetInt(ss_mag.sceneID)}");
        }
        private void Update()
        {
            //print(StaticSharp.Conditions);  //���A���ˬd
            show_UI();
            StaticSharp.SpecialistKeyInput(Input.GetKey(KeyCode.LeftControl),
                Input.GetKey(KeyCode.LeftAlt),
                Input.GetKey(KeyCode.LeftShift)
                );
            //// ---���|�۰ʥͦ�����---
            ///
            if (!isGen) for (int i = 0; i < objectName.Length; i++)
                {
                    if (UI_moveDistane % objectDistance[i] == i && !isGen)
                    {
                        StartCoroutine(PathAutoGenerObject(i));
                        break;
                    }
                }
            
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
