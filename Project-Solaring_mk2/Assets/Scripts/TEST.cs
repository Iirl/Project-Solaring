using Unity.VisualScripting;
using UnityEngine;

public class TEST : MonoBehaviour
{

    private void Start()
    {
        AnimationEvent evt;
        evt = new AnimationEvent();
        PrintEvent(evt);
    }

    public void PrintEvent(object evt)
    {
        Debug.Log(evt);
    }
}
