using UnityEngine;
using UnityEngine.SceneManagement;

namespace solar_a
{
    /// <summary>
    /// �����޲z�t��
    /// �u�n�B�z�����ƥ󪺤�k�Ш̦����O�B�z�C
    /// </summary>
    public class ManageScene : MonoBehaviour
    {
        /// <summary>
        /// ���o�ثe�������s��
        /// </summary>
        /// <returns>�^�Ǥ@�� int �� index �C</returns>
        public int GetScenes()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
        /// <summary>
        /// ��Ū����
        /// </summary>
        public void ReloadCurrentScene()
        {
            if (Time.timeScale != 1) Time.timeScale = 1;
            SceneManager.LoadScene(GetScenes());
        }        
        /// <summary>
        /// ���J���w�s��������
        /// </summary>
        /// <param name="idx">�п�J�����s��</param>
        public void LoadScenes(int idx)
        {
            if (Time.timeScale != 1) Time.timeScale = 1;
            SceneManager.LoadScene(idx);
        }
    }
}