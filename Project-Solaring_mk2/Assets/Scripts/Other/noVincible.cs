using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 當物件碰到玩家無論如何都會結束遊戲
    /// When object trigger the player, the game will end
    /// </summary>
    public class noVincible : MonoBehaviour
    { 
        private void OnTriggerEnter(Collider other)
        {
            //print("碰到物件");
            if (other.tag.Contains("Player")) StaticSharp.Conditions = State.End;
        }
        
    }
}