using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// ����I�쪱�a�L�צp�󳣷|�����C��
    /// When object trigger the player, the game will end
    /// </summary>
    public class noVincible : MonoBehaviour
    { 
        private void OnTriggerEnter(Collider other)
        {
            //print("�I�쪫��");
            if (other.tag.Contains("Player")) StaticSharp.Conditions = State.End;
        }
        
    }
}