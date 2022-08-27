using System.Collections;
using solar_a;
using UnityEngine;

public class StaticSharp
{
    static public int _LANG_ID;
    //
    static public State Conditions;
    static public Vector3 Rocket_BASIC;
    static public Vector3 Rocket_INFO;
    static public bool isChangeScene;
    static public bool isDialogEvent;
    //
    
    #region Shotkey, 快捷鍵設定
    static private void TestBoard(DBG d)
    {
        //print("Debug");
        //顯示除錯面板        
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
        DBG Dbg = Object.FindObjectOfType<DBG>();
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
                if (kAtl && kO) TestBoard(Dbg);
                if (kLS) Debug.Log("C+S button");
                else if (kB) Debug.Log("B button");
                else if (kM) Debug.Log("M button");
                else if (kO) Debug.Log("O button");
                else if (kP) Debug.Log("P button");
                else if (kQ) Debug.Log("Q button");
            }
            else if (isAtl)
            {
                if (kLS) Debug.Log("A+S button");
                else if (kN) Debug.Log("N button");
                else if (kR) Dbg.SendMessage("GeneratorBlock"); //產生OBJ物件
            }
            else if (isLS)
            {
                if (kC) Debug.Log("C button");
                else if (kU) Debug.Log("U button");
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
public enum GenerClass { Normal, Meteorite, StaticPoint }
public enum RocketState { Stay, Move, Boost, Crashed, Stop }
public enum SpacetState { Stay, Setting, Rotate, Stop }




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

}