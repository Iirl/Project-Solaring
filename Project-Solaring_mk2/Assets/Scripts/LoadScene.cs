using UnityEngine;
using UnityEngine.SceneManagement;

namespace solar_a
{
    public class LoadScene : MonoBehaviour
    {
       public void NextScene(int SceneID)
        {
            SceneManager.LoadScene(0);
        }


    }

}