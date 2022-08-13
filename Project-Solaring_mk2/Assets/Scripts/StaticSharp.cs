using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSharp
{
    static public State Conditions;
    static public Vector3 Rocket_BASIC;
    static public Vector3 Rocket_INFO;
    static public bool isPowerfullMode=false;


    public enum State { Running, Loading, Pause, End }
    /// <summary>
    /// 遊戲狀態機
    /// 目前先設定為：執行中、讀取、暫停及結束遊戲。
    /// ※與之前的寫法相比，不需要個別設定布林值，狀態變化只要下要變的函數就可以，而且順序可以固定或指定。
    /// </summary>
    public class GameCondition
    {
        private State state { get { return Conditions; } set { Conditions = value; } }

        public void Next()
        {
            if (state < State.End - 1) state++;
        }
        public void Previous()
        {
            if (state != 0) state--;
        }
        public void Dead()
        {
            state = State.End;
        }
        public string GetState()
        {
            return state.ToString();
        }
    }
}
