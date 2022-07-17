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

        public void AutoGenerate()
        {
            if (gener_class != null) gener_class.Random_gen(ss_ctl.transform.position.y, false);
        }
        public void AutoGenerate(bool rotate)
        {
            if (gener_class != null) gener_class.Random_gen(ss_ctl.transform.position.y, rotate);
        }
        /// <summary>
        /// �����w�]�����;����O�A�]�t�ɵ��~���͡C
        /// </summary>
        /// <param name="i">���w���;����e�A�ثe�������;��p�U�G
        /// 0. �w�]�A���� UFO �C
        /// 1. ���� �c�l �C
        /// 2. ���� �~�l �C
        /// </param>
        public void AsignGenerate(int i)
        {
            string name;
            switch (i)
            {
                case 0: name = "UFOGenerator"; break;
                case 1: name = "BOXGenerator"; break;
                case 2: name = "BottleGenerator"; break;
                case 11: name = "meteorite01Gen"; break;
                case 12: name = "meteorite02Gen"; break;
                case 13: name = "meteorite03Gen"; break;
                case 14: name = "meteorite04Gen"; break;
                default: name = null; break;
            }
            gener_class = GameObject.Find(name).GetComponent<Object_Generator>();
            print($"�u{name}�v was Selected.");
        }
        /// <summary>
        /// ���ͪ��a�l���󪺵{���C
        /// �|���Τ@��ͦ����覡�ͦ������A���o�Ӫ���ID�A�̦��ͦ��l����C
        /// ���p�G�ϥιw�m�����ܡA����|�ͦ��b�l����W�C
        /// </summary>
        /// <param name="tg">���ؤl����n�Q�ͦ�</param>
        public void MeteoGenerate(GameObject tg)
        {
            int Gid = gener_class.Random_gen(ss_ctl.transform.position.y, false);
            gener_class.Random_Metro(Gid, tg);
        }
        public void MeteoGenerate(int tgi)
        {
            int Gid = gener_class.Random_gen(ss_ctl.transform.position.y, false);
            GameObject tg = ((Transform)EditorUtility.InstanceIDToObject(tgi)).gameObject;
            ////
            string[] subBlocks = { "BlueCrystalPack01", "RedCrystalPack01"};
            ////
            gener_class.Random_Metro(Gid, tg);
        }
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
        public void LoadNowOB()
        {
            gener_class.ReadList();
        }
        public void DeleteOB(int i)
        {
            gener_class.Destroys(i);
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
        #endregion

        #region ���a�����k�Ψƥ�
        /// <summary>
        /// �]�wUI������
        /// </summary>
        private void _show_UI()
        {
            UI_moveDistane = (int)ss_ctl.stage_position.y;
            UI_fuel = (int)rocket_ctl.GetRocketInfo().x;
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
