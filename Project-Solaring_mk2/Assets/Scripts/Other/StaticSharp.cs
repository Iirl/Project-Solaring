using System.Collections;
using solar_a;
using UnityEngine;

public class StaticSharp
{
    // 檔案存取資訊
    static public int _LANG_ID;
    static public int _LEVEL;
    static public int _SCORE;
    static public float _VOLUME;
    static public Vector3 _RECORDPOS;
    // 遊戲內資訊
    static public State Conditions;
    static public Vector3 Rocket_BASIC;
    static public Vector3 Rocket_INFO;
    static public Vector3 Rocket_POS;
	static public int Rocket_rushCount = -1;
	static public bool isFirstPlay;
    static public bool isChangeScene;
    static public bool isDialogEvent;
    static public float DistanceRecord; // 紀錄飛行距離，將該值傳給下一關使用。
    // Secret
    static public int _SecretSCORE;
    static public bool isProtected;
    
    #region Shotkey, 快捷鍵設定
    static private void TestBoard(SSRocket d)
    {
        //print("Debug");
        //顯示隱藏面板
        if (d) d.ShowDebug(!d.isShowed);
        if (d) return; 
        CanvasGroup testOB = Object.FindObjectOfType<TestObject>().GetComponent<CanvasGroup>();
        if (testOB) testOB.CanvansFadeControl(!testOB.interactable);

    }
    /// <summary>
    /// 特殊指令，當玩家輸入快速鍵的時候會出現的封弊功能。
    /// </summary>
    static public void SpecialistKeyInput(bool isCtrl, bool isAtl, bool isLS)
    {
        SSRocket SSR = Object.FindObjectOfType<SSRocket>();
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
                if (kAtl && kO) TestBoard(SSR);
	            if (kLS) Debug.Log("C+S button");
	            // 黑洞跳關
	            else if (kM) ManageCenter.mgCenter.X_BlockHole(); //Debug.Log("M button");
                //else if (kO) Debug.Log("O button");
                else if (kP) Debug.Log("P button");
                else if (kQ) Debug.Log("Q button");
            }
            else if (isAtl)
            {
                //SSR.SendMessage("GeneratorBlock"); //產生OBJ物件
                if (kLS) Debug.Log("A+S button");
                else if (kN) ManageCenter.rocket_ctl.StateOnVisable();	// 飛碟隱形
                else if (kR) ManageCenter.mgCenter.ObjectDestoryAll();	// 清空並重新生成
                else if (kP) ManageCenter.mgCenter.X_PowerMode();		// 無敵模式
            }
            else if (isLS)
            {
	            if (kC) ManageCenter.rocket_SSR.ChangeToCargo();		// 切換成貨機
                else if (kU) ManageCenter.rocket_SSR.ChangeToUFO();		//切換成幽浮
            }
        }

    }
    #endregion

    /// <summary>
    /// 遊戲狀態機
    /// 目前先設定為：執行中、讀取、暫停及結束遊戲。
    /// ※與之前的寫法相比，不需要個別設定布林值，狀態變化只要下要變的函數就可以，而且順序可以固定或指定。
    /// </summary>
    public class GameCondition
    {
        private State state { get { return Conditions; } set { Conditions = value; } }
        public bool isPause { get { return Conditions == State.Pause; } }
	    public bool isLoading { get { return Conditions == State.Loading; } }
        public bool isEnd  { get { return Conditions == State.End; } }

        public GameCondition()
        {
            state = State.Running;
        }

        public string GetState() => state.ToString();
        public int GetStateID() => (int)state;
        public void Run() => state = 0;
        public void Next()
        {
            if (state < State.End - 1) state++;
        }
        public void Previous()
        {
            if (state != 0) state--;
        }
        public void Dead() => state = State.End;
        public void Finish() => state = State.Finish;
    }
}

public enum State { Running, Loading, Pause, End, Finish }
public enum GenerClass { Normal, Meteorite, StaticPoint, RootObject, PrevRocord }
public enum RocketState { Stay, Move, Boost, Crashed, Stop }
public enum RocketACondition { Protect,FullGage ,FullRush , Other}
public enum RocketDCondition { Slowdown }
public enum RocketPreviews { Basic, Outfit1, Outfit2}
public enum SpacetState { Stay, Setting, Rotate, Stop }



// 自定義延伸方法
static class Extension
{
    /// <summary>
    /// 淡入出畫布控制系統
    /// </summary>
    /// <param name="cvsgp">畫布元件</param>
    /// <param name="isIN">是否淡入，否則淡出</param>
    static public void CanvansFadeControl(this CanvasGroup cvsgp, bool isIN = false)
    {
        cvsgp.alpha = isIN ? 1 : 0;
        cvsgp.interactable = isIN;
        cvsgp.blocksRaycasts = isIN;
    }
    /// <summary>
    /// 畫布淡入出效果偕同程序
    /// </summary>
    /// <param name="cvsgp">畫布群組</param>
    /// <param name="visable">真為淡入，假為淡出</param>
    /// <param name="fadeSpeed">效果速度</param>
    /// <returns></returns>
    static public IEnumerator FadeEffect(this CanvasGroup cvsgp, bool visable = true, float fadeSpeed=0.1f)
    {
        switch (visable)
        {
            case true:
                while (cvsgp.alpha < 1)
                {
                    cvsgp.alpha += 0.1f;
                    yield return new WaitForSeconds(fadeSpeed);
                }
                cvsgp.CanvansFadeControl(visable);
                break;
            default:
                while (cvsgp.alpha > 0)
                {
                    cvsgp.alpha -= 0.1f;
                    yield return new WaitForSeconds(fadeSpeed);
                }
                cvsgp.CanvansFadeControl(visable);
                break;
        }
    }

    /// <summary>
    /// 觸發銷毀物件事件，直接讀取中控系統的銷毀事件。
    /// </summary>
    /// <param name="hitObj">觸發的物件</param>
    /// <param name="hasDest">是否有銷毀等待時間</param>
    static public void StageColliderEvent(this Collider col, bool hasDest = true)
    {
        //Debug.Log("場景邊緣觸發");
        if (!col.tag.Contains("Player")) ManageCenter.mgCenter.ObjectDestory(col.gameObject, hasDest);
    }
}