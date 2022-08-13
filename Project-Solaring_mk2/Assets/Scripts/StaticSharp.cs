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
    /// �C�����A��
    /// �ثe���]�w���G���椤�BŪ���B�Ȱ��ε����C���C
    /// ���P���e���g�k�ۤ�A���ݭn�ӧO�]�w���L�ȡA���A�ܤƥu�n�U�n�ܪ���ƴN�i�H�A�ӥB���ǥi�H�T�w�Ϋ��w�C
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
