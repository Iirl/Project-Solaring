using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
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
        static public int UI_fuel = 100;
        static public float UI_moveDistane = 0;
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
        [SerializeField, Header("�V������t��")]
        AudioMixer adM;
        #region ���a�վ\��� (Private Feild)
        

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
            //    ss_ctl.transform.position += Vector3.up * unit / 2; // ��������1
            UI_moveDistane += unit;
            if (rocket_ctl.RocketS1.x > 0) rocket_ctl.PutRocketSyn(rocket_ctl.Unit_fuel * Time.deltaTime * ss_ctl.speed);   // �U���ܤ�
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
        /*
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
        */
        /// <summary>
        /// �M�������ơC
        /// �Ҧ�����q�e���W�������n�g�L�o�Ө禡�C
        /// </summary>
        /// <param name="obj"></param>
        public void ObjectDestory(GameObject obj)
        {
            GenerateSystem objGS = obj.transform.GetComponentInParent<GenerateSystem>();
            if (!objGS) { Destroy(obj); return; }
            //print(objGS.name);
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
        private IEnumerator PauseFadeEffect(bool visable = true)
        {
            canvas_select = pauseUI.GetComponent<CanvasGroup>();
            switch (visable)
            {
                case true:
                    while (canvas_select.alpha < 1)
                    {
                        canvas_select.alpha += 0.1f;
                        yield return new WaitForSeconds(fadeSpeed.x);
                    }
                    Time.timeScale = 0.05f;
                    condition.Next();
                    CanvasCtrl(canvas_select, visable);
                    break;
                default:
                    while (canvas_select.alpha > 0)
                    {
                        canvas_select.alpha -= 0.1f;
                        yield return new WaitForSeconds(fadeSpeed.y);

                    }
                    Time.timeScale = 1f;
                    condition.Previous();
                    CanvasCtrl(canvas_select, visable);
                    break;
            }

            GameState();



        }
        #endregion

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
            // ����UI - ���ʪ��g�k�G�����o������m�A�M��A�N��m�e��UI�̡C
            /*Vector3 stage_pos = GetStagePOS();
            UI_moveDistane = (int)stage_pos.y;  //*/
            // ���b MoveAction ��
            // �U��UI
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
            // �Хܤ�rUI���e
            if (ui_Dist != null) ui_Dist.text = $"{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"{UI_fuel}";
        }
        /// <summary>
        /// �Ȱ����}��
        /// </summary>
        public void show_Menu()
        {
            //print($"{StaticSharp.Conditions} & {condition.isPause}");
            if (menus != null)
            {
                canvas_select = menus;
                if (condition.isEnd)
                {
                    StartCoroutine(PauseFadeEffect(true));
                }
                else if (!condition.isPause)
                {
                    // ��{
                    condition.Next();
                    StartCoroutine(PauseFadeEffect(true));

                }
                else
                {
                    // �H�X
                    condition.Previous();
                    StartCoroutine(PauseFadeEffect(false));

                }
            }
        }
        /// <summary>
        /// �C�����A�B�z���p(�ݭn�QInvoke)
        /// �ھڥثe�����A�����C���i�檬�p
        /// END = �����C��
        /// Running ���� = �}�_�Ȱ����
        /// </summary>
        private void GameState()
        {
            if (condition.GetState() == "End")
            {// GameOver
                bool isEnd = true;
                transform.Find("UI_Pause").transform.Find("Btn_Back_en").gameObject.SetActive(!isEnd);
                mEnd.enabled = isEnd;
                ss_ctl.enabled = !isEnd;
                condition.Finish();
                rocket_ctl.ControlChange(!isEnd);
            }
            else if (StaticSharp.Conditions != State.Running)
            {// �p�G���O���檬�A�A�h�Ȱ��Ŷ��A�éI�s�Ȱ����C
                space_ctl.enabled = !space_ctl.enabled;
                rocket_ctl.ControlChange(!condition.isPause);
            } else
            {
                space_ctl.enabled = !space_ctl.enabled;
                rocket_ctl.ControlChange(!condition.isPause);
            }
            if (pauseUI != null) pauseUI.SetActive(true);
            CancelInvoke("GameState");
        }

        /// <summary>
        /// �i�J�C�������t��
        /// �Y���H�U���p�h�I�s���{���G
        /// 1. �S���U��
        /// 2. ���������
        /// </summary>
        private void StateEnd()
        {
            //CheckGame(true, 5f); //�����C�����󤧤@
            show_Menu();
        }

        private void Awake()
        {
            UI_moveDistane = 0;
        }
        private void Start()
        {
            //print($"�ثe�����s�����G{PlayerPrefs.GetInt(ss_mag.sceneID)}");
        }
        private void Update()
        {

            switch (StaticSharp.Conditions)
            {
                case State.Running:
                    show_UI();
                    MoveAction();
                    break;
                case State.Loading:
                    break;
                case State.Pause:
                    break;
                case State.End:
                    StateEnd();
                    break;
                default:
                    break;
            }
            //print(StaticSharp.Conditions);  //���A���ˬd
            // �����J����
            StaticSharp.SpecialistKeyInput(Input.GetKey(KeyCode.LeftControl),
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
