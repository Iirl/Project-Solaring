using solar_a;
using UnityEngine;

/// <summary>
/// Debug function.
/// �����ε{���A��ݭn��
/// </summary>
public class DBG : MonoBehaviour
{
    ManageCenter mgc;
    //
    [SerializeField, Header("���|���`")]
    public bool noDead;
    [SerializeField, Header("���ӿU��")]
    public bool noFuel;
    [SerializeField, Header("���ӽĨ�")]
    public bool noRush;
    [SerializeField, Header("������I")]
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
