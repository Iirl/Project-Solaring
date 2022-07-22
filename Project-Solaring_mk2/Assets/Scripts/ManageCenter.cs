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

        #region �@����� (Public Feild)
        public CanvasGroup canvas_select;
        #endregion

        #region �@�Τ�k (Public Method)

        ///////////// ���b�������
        /// <summary>
        /// �U���ܤ�
        /// </summary>
        /// <param name="f">��J�ثe�U��</param>
        /// <returns></returns>
        public float fuelChange(float f)
        {
            f -= Time.deltaTime * Mathf.Abs(ss_ctl.Space_speed) * 0.25f;
            return f;
        }

        ///////////// ���ͪ���

        public void AutoGenerate(int i)
        {
            if (gener_class != null) gener_class.Static_gen(ss_ctl.transform.position.y, i);
        }
        public void AutoGenerate(bool rotate)
        {
            if (gener_class != null) gener_class.Random_gen(ss_ctl.transform.position.y, rotate, 0);
        }
        /// <summary>
        /// �����w�]�����;����O�A�]�t�ɵ��~���͡C
        /// �o�̬O�]�w�n�ϥΦ�����O�C
        /// �ѩ󴫭Ӽg�k�A�o�̼ȮɪŸm
        /// </summary>
        /// <param name="i">���w���;����e�A�ثe�������;��p�U�G
        /// 0. �w�]�A���� UFO �C
        /// 1. ���� �c�l �C
        /// 2. ���� �~�l �C
        /// </param>
        public void AsignGenerate(int i)
        {
            string name = "";
            switch (i)
            {
                case 0: name = "ObjectGenerator"; break;
                case 1: name = "ObjectGenerator2"; break;
                default:break;
            }
            gener_class = GameObject.Find(name).GetComponent<Object_Generator>();
        }
        /// <summary>
        /// ���ͪ��a�l���󪺵{���C
        /// �|���Τ@��ͦ����覡�ͦ������A���o�Ӫ���ID�A�̦��ͦ��l����C
        /// ���p�G�ϥιw�m�����ܡA����|�ͦ��b�l����W�C
        /// </summary>
        /// <param name="tg">���ؤl����n�Q�ͦ�</param>
        public void MeteoGenerate()
        {
            int obj = Random.Range(0, 3);
            int Gid = gener_class.Random_gen(ss_ctl.transform.position.y, false,obj); // Fist: Generate SubObject.
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
            gener_class.r();
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
        /// �C�������B�z���p
        /// </summary>
        public void GameOver()
        {
            mEnd.enabled = true;
            rocket_ctl.enabled = false;
            rocket_ctl.GetComponent<Rigidbody>().isKinematic = true;
            ss_ctl.enabled = false;
        }
        #endregion

        #region ���a�����k�Ψƥ�
        /// <summary>
        /// �]�wUI������
        /// </summary>
        private void _show_UI()
        {
            UI_moveDistane = (int)ss_ctl.stage_position.y;
            UI_fuel = (int)rocket_ctl.GetRocketInfo().x;
            if (UI_fuel <= 0) GameOver(); //�����C�����󤧤@
            if (ui_Dist != null) ui_Dist.text = $"Distance:{UI_moveDistane}";
            if (ui_fuel != null) ui_fuel.text = $"Fuel:{UI_fuel}";
        }
        private void Update()
        {
            _show_UI();
        }
        #endregion

    }
}
