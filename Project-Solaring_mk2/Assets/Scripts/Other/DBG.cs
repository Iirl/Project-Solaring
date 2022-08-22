using UnityEngine;

/// <summary>
/// Debug function.
/// 除錯用程式，把需要的
/// </summary>
public class DBG : MonoBehaviour
{
    [SerializeField, Header("不會死亡")]
    public bool noDead;


    private void Update()
    {
        if (noDead) if (StaticSharp.Conditions == State.End) StaticSharp.Conditions = 0;
    }
}
