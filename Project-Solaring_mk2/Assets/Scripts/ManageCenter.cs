using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using Unity.VisualScripting;
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
        [Header("�����t��")]
        static public ManageCenter mgCenter;
        [SerializeField, Header("�U�{���޲z�t��"),Tooltip("�����t��")]
        private ManageScene MgScene;
        static public ManageScene mgScene;
        [SerializeField, Tooltip("�����޲z")]
        private ManageEnd MgEnd;
        static public ManageEnd mgEnd;
        [SerializeField, Tooltip("���b����t��")]
        private Rocket_Controll Rocket_CTL;
        static public Rocket_Controll rocket_ctl;
        [SerializeField,Tooltip("��������t��")]
        private SceneStage_Control SS_CTL;
        static public SceneStage_Control ss_ctl;
        [Tooltip("�Ŷ��Ŧܨt��")]
        static public Space_Controll space_ctl;
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
        [SerializeField, Tooltip("�κɿU�ƫ᪺�ä�ɶ�"),Range(0,10)]
        private float fuelExhaustedTime=5;
        /// <summary>
        /// ���e������
        /// </summary>
        [SerializeField, Header("�Ȱ����")]
        GameObject pauseUI;
        [SerializeField, Tooltip("�Ȱ����e���t��")]
        CanvasGroup pauseMenus;
        [SerializeField, Header("�e���H�Ƴt��")]
        Vector2 fadeSpeed = Vector2.zero + Vector2.one * 0.01f;
        [SerializeField, Header("�V������t��")]
        AudioMixer adM;
        #region ���a�վ\��� (Private Feild)
        [Tooltip("�{�������ܥثe���e���A�i�H���γ]�w�C")]
        private CanvasGroup canvas_select;
        private AudioSource AudioBox;
        private BoxCollider FinishBox;
        [HideInInspector]
        public bool noExhauFuel, noExhauRush, noDead, toFinDest;
        #endregion


        public void X_PowerMode()
        {
            
            noDead = !noDead;
            transform.Find("AudioBox").GetComponent<AudioSource>().Play();
        }
        /// <summary>
        /// �@�Τ�k (Public Method)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetStagePOS() => ss_ctl.transform.position;
        public Vector3 GetStageBorder() => ss_ctl.GetBoxborder();
        public void SetRockBasicInfo(float x, float y=0, float z=0) {
            x = x != 0 ? x: rocket_ctl.RocketBasic.x;
            y = y != 0 ? y: rocket_ctl.RocketBasic.y;
            z = z != 0 ? z: rocket_ctl.RocketBasic.z;
            rocket_ctl.SetBasicInfo(x,y,z);
                }
        //�����\��
        public void GetState() => print(condition.GetState());

        #region ���b����P�p�Ƭ���
        private IEnumerator DeathCounter(float counter=0)
        {            
            for (int i=0; i< counter;i++) yield return new WaitForSeconds(1);
            if(rocket_ctl.RocketS1.x <1) condition.Dead();
            yield return null;
        }
        /// <summary>
        /// ���b�P�������ʡC
        /// �ª��G�վ������ Y�b�ƭȡC
        /// �s���G�����ק�UI�ƭȡC
        /// </summary>
        public void MoveAction()
        {
            //if (!rocket_ctl.rc_dtion.IsStay) 
            //    ss_ctl.transform.position += Vector3.up * unit / 2; // ��������1
            float unit = Time.deltaTime * ss_ctl.speed; // ���Z���A�ϥ� deltaTime �i�H������s�W�v�����~�C            
            if (toFinDest) UI_moveDistane = ss_ctl.finishDistane;
            else UI_moveDistane += unit;
            if (UI_moveDistane >= ss_ctl.finishDistane)
            {
                UI_moveDistane = ss_ctl.finishDistane;
                FinishBox.enabled = true;
            }

            float fueldown = rocket_ctl.Unit_fuel * Time.deltaTime * ss_ctl.speed;
            //print(rocket_ctl.rc_dtion.IsBoost);
            //if (rocket_ctl.rc_dtion.IsBoost)  // fueldown -= (rocket_ctl.RocketS1.z * rocket_ctl.Unit_fuel) * Time.deltaTime;
            if (rocket_ctl.RocketS1.x > 0 && !noExhauFuel) rocket_ctl.PutRocketSyn(fueldown);   // �U���ܤ�
            else if (!noExhauFuel) StartCoroutine(DeathCounter(fuelExhaustedTime));   // �U�ƥκɡA���`�˼ơC

        }
        /// <summary>
        /// �U�ƸɥR��ơA��J�w�ȼW�[�U�ơC
        /// </summary>
        /// <param name="f">���w�n�ɪ���</param>
        public void FuelReplen(int f)
        {
            float nowFuel = rocket_ctl.RocketS1.x;
            rocket_ctl.PutRocketSyn(f);
            rocket_ctl.ADOClipControl(0);
            if (StaticSharp.Conditions == State.End && nowFuel > 0) CancelInvoke("GameState");
        }
        #endregion
        #region ����q�Ψ��
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
        /// <param name="obj">�I���ϰ�^�Ǫ�����</param>
        GenerateSystem objGS;
        public void ObjectDestory(GameObject obj, bool hasDesTime=false)
        {
            objGS = obj.transform.GetComponentInParent<GenerateSystem>();
            //print($"�W��: {objGS.name} hasdes:{hasDesTime}");  // ���լO�_��Ū���쪫��AŪ����h�����P���קK���~�C
            if (!objGS) { Destroy(obj); return; }
            objGS.Destroys(obj, hasDesTime);
            //gener_class.Destroys(obj);
        }
        #endregion
        #region �� �� �� ��
        /// <summary>
        /// �i�J���~��
        /// �x�s���b�����
        /// </summary>
        public void InToStation()
        {
            StaticSharp.Rocket_INFO = rocket_ctl.RocketS1;
            StaticSharp.Rocket_BASIC = rocket_ctl.RocketBasic;
            mgScene.SaveLeveInform();
            mgScene.LoadScenes("Station");            
        }

        ///////////// ����ܤƬ���
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
                    condition.Next();
                    canvas_select.CanvansFadeControl(visable);
                    break;
                default:
                    while (canvas_select.alpha > 0)
                    {
                        canvas_select.alpha -= 0.1f;
                        yield return new WaitForSeconds(fadeSpeed.y);
                    }
                    condition.Previous();
                    canvas_select.CanvansFadeControl(visable);
                    break;
            }
            GameState();
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
            if (UI_fuel <= 100)
            {
                ui_fuelbar.fillAmount = UI_fuel / 100f;
                if(EnergyPlus[0].activeSelf) EnergyPlus[0].SetActive(false);
            }
            else
            {   // �W�L 100 �������ή檬������
                ui_fuelbar.fillAmount = 1;
                int level = (int)((UI_fuel - 100) / rocket_ctl.fuel_overcapacity) + 1;
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
            if (ui_Dist != null) ui_Dist.text = $"{UI_moveDistane.ToString("0")}";
            if (ui_fuel != null) ui_fuel.text = $"{UI_fuel}";
        }
        /// <summary>
        /// �Ȱ����}��
        /// </summary>
        public void show_Menu()
        {
            //print($"{StaticSharp.Conditions} & {condition.isPause}");
            if (pauseMenus != null)
            {
                canvas_select = pauseMenus;
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
        #endregion
        /// <summary>
        /// �C�����A�B�z���p(�ݭn�QInvoke)
        /// �ھڥثe�����A�����C���i�檬�p
        /// END = �����C��
        /// Running ���� = �}�_�Ȱ����
        /// </summary>
        private void GameState()
        {
            if (pauseUI != null && !pauseUI.activeSelf) pauseUI.SetActive(true);
            if (condition.GetState() == "End")
            {   // GameOver
                bool isEnd = true;
                pauseUI.transform.Find("Btn_Back_en").gameObject.SetActive(!isEnd);
                mgEnd.enabled = isEnd;
                ss_ctl.enabled = !isEnd;
                // ��������
                StartCoroutine(rocket_ctl.ControlChange(!isEnd));
                Simple_move[] simple_s = FindObjectsOfType<Simple_move>();
                foreach (Simple_move simple in simple_s) StartCoroutine(simple.Mute());
                condition.Finish();
            }
            else if (StaticSharp.Conditions != State.Running)
            {// �p�G���O���檬�A�A�h�Ȱ��Ŷ��A�éI�s�Ȱ����C
                StartCoroutine(rocket_ctl.ControlChange(false));
            }
            else StartCoroutine(rocket_ctl.ControlChange(true));
            CancelInvoke("GameState");
        }
        StaticSharp.GameCondition condition = new StaticSharp.GameCondition();
        #region ���a�����k�Ψƥ�
        /// <summary>
        /// �i�J�C�������t�ΡA�Y���H�U���p�h�I�s���{���G
        /// 1. �S���U��
        /// 2. ���������
        /// </summary>
        private void StateEnd() => StartCoroutine(PauseFadeEffect(true));

        #endregion

        private void Awake()
        {
            mgCenter = GetComponent<ManageCenter>();
            FinishBox = GameObject.Find("NextStage").GetComponent<BoxCollider>();
        }
        private void Start()
        {
            if (pauseMenus == null) pauseMenus = pauseUI.GetComponent<CanvasGroup>();
            if (MgEnd) mgEnd = MgEnd;
            else mgEnd = FindObjectOfType<ManageEnd>();
            mgScene = MgScene ?? FindObjectOfType<ManageScene>();
            ss_ctl = SS_CTL ?? FindObjectOfType<SceneStage_Control>();
            rocket_ctl = Rocket_CTL ?? FindObjectOfType<Rocket_Controll>();
            
            //print($"�ثe�����s�����G{PlayerPrefs.GetInt(ss_mag.sceneID)}");
            UI_moveDistane = 0;
        }
        private void Update()
        {

            switch (StaticSharp.Conditions)
            {
                case State.Running:
                    if (Time.timeScale != 1) Time.timeScale = 1;
                    show_UI();
                    if (UI_moveDistane <= ss_ctl.finishDistane) MoveAction();
                    break;
                case State.Pause:
                    if (Time.timeScale > 0.1f) Time.timeScale = 0.05f;
                    break;
                case State.End:
                    if (noDead)
                    {
                        condition.Run();
                        break;
                    }
                    StateEnd();
                    break;
                case State.Finish:
                    if (Time.timeScale > 0.1f) Time.timeScale = 0f;
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
            if (Input.GetKeyDown(KeyCode.Escape)) show_Menu();
        }

        #region �m�J����
        /*Ctrl+b �}�Ҷ¬}�H�������G�ݭn�¬}�ͦ����ʵe�ιϤ�
        Shift+c ���b�����f���G�ݭn�@�[�f��
        Shift+u ���b������
        Alt+n ���b����
        Alt+r ���s�ͦ��ɵ��P��ê��*/

        #endregion
    }


}
