using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Start_menu : MonoBehaviour
{
    private Button button = null;
    // Start is called before the first frame update
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        string click_name = transform.name;
        if (click_name == "start")
        {
            Debug.Log("Start the games.");

        } else if (click_name == "end")
        {
            Debug.Log("Exit the games.");
            Application.Quit();
        }
    }
}
