using solar_a;
using UnityEngine;

/// <summary>
/// Debug function.
/// 除錯用程式，把需要的
/// </summary>
public class DBG : MonoBehaviour
{
    ManageCenter mgc;
    //
    [SerializeField, Header("不會死亡")]
    public bool noDead;
    [SerializeField, Header("不耗燃料")]
    public bool noFuel;
    [SerializeField, Header("不耗衝刺")]
    public bool noRush;
    [SerializeField, Header("移到終點")]
    public bool toFinal;

    private void Awake()
    {
        mgc = FindObjectOfType<ManageCenter>();
    }
    private void Update()
    {
        mgc.noDead = noDead ? true: false;
        mgc.noExhauFuel = noFuel ? true: false;
        mgc.noExhauRush = noRush ? true: false;
        mgc.toFinDest = toFinal ? true: false;
    }
}
