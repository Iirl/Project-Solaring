using UnityEngine;
/// <summary>
///  ����Ĳ�o�ƥ�A�ΨӰ�����ئ欰
/// </summary>
public class Trigger_Collider : MonoBehaviour
{
    #region Ĳ�o
    #endregion
    /// <summary>
    /// Ĳ�o�ƥ�
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        //print(other.gameObject.name);
        if (other.gameObject.tag != "Player") Destroy(other.gameObject, 1);
        if (other.gameObject.tag == "Player") Bound_Object(other.gameObject);
    }
    #region �I��
    #endregion
    #region ��k
    private void Bound_Object(GameObject gameob)
    {
        gameob.GetComponent<Rigidbody>().velocity =
        new Vector2(Input.GetAxis("Horizontal") * -5,
        Input.GetAxis("Vertical") * -5
        );
    }

    #endregion
}
