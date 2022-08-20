using System.Collections;
using System.Collections.Generic;
using solar_a;
using UnityEngine;

public class StaticSharp
{
    static public State Conditions;
    static public Vector3 Rocket_BASIC;
    static public Vector3 Rocket_INFO;
    static public bool isPowerfullMode=false;


    #region Shotkey, 快捷鍵設定
    static private void test(int idx = 0)
    {
        //print("Debug");
        //顯示除錯面板
        CanvasGroup testOB = Object.FindObjectOfType<TestObject>().GetComponent<CanvasGroup>();
        if (!testOB) return;
        testOB.alpha = testOB.alpha != 0 ? 0 : 1;
        testOB.interactable = !testOB.interactable;
        testOB.blocksRaycasts = !testOB.blocksRaycasts;
    }

    /// <summary>
    /// 特殊指令，當玩家輸入快速鍵的時候會出現的封弊功能。
    /// </summary>
    static public void SpecialistKeyInput(bool isCtrl, bool isAtl, bool isLS)
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
                    test();
                }
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
                else if (kR) Debug.Log("R button");
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
        public string GetState() => state.ToString();
        public int GetStateID() => (int) state;
    }
}

public enum State { Running, Loading, Pause, End, Finish }
public enum GenerClass { Normal, Meteorite }

