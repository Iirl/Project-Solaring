using UnityEngine;
/// <summary>
///  全域觸發事件，用來偵測邊框行為
/// </summary>
public class Trigger_Collider : MonoBehaviour
{
    #region 觸發
    #endregion
    /// <summary>
    /// 觸發事件
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        //print(other.gameObject.name);
        if (other.gameObject.tag != "Player") Destroy(other.gameObject,1);
        if (other.gameObject.tag == "Player") other.GetComponent<Rigidbody>().velocity =
            new Vector2(Input.GetAxis("Horizontal")* -5, 
            Input.GetAxis("Vertical") * -5
            );
    }
    #region 碰撞
    #endregion
}
