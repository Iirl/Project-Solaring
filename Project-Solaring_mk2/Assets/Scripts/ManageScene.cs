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
        /// ��Ū����
        /// </summary>
        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        /// <summary>
        /// ���o�ثe�������s��
        /// </summary>
        /// <returns>�^�Ǥ@�� int �� index �C</returns>
        public int LoadScenes()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
        /// <summary>
        /// ���J���w�s��������
        /// </summary>
        /// <param name="idx">�п�J�����s��</param>
        public void LoadScenes(int idx)
        {
            SceneManager.LoadScene(idx);
        }
    }
}