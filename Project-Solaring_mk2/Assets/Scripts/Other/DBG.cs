using UnityEngine;

/// <summary>
/// Debug function.
/// �����ε{���A��ݭn��
/// </summary>
public class DBG : MonoBehaviour
{
    [SerializeField, Header("���|���`")]
    public bool noDead;


    private void Update()
    {
        if (noDead) if (StaticSharp.Conditions == State.End) StaticSharp.Conditions = 0;
    }
}
