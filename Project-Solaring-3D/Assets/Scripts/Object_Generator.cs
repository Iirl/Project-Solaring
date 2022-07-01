using UnityEngine;

/// <summary>
/// 此為在區域空間中產生物件的程式
/// </summary>
public class Object_Generator : MonoBehaviour
{

    #region 屬性

    #region 類別
    class Static_Generate
    {
        private float x { get; set; }
        private float y { get; set; }
        private float z { get; set; }
        private float xr { get; set; }
        private float yr { get; set; }
        private float zr { get; set; }
        public float X_pos_gen { get { return x; } set { x = value; Create_v3 =  new Vector3(x, y, z); } }
        public float Y_pos_gen { get { return y; } set { y = value; Create_v3 = new Vector3(x, y, z); } }
        public float Z_pos_gen { get { return z; } set { z = value; Create_v3 = new Vector3(x, y, z); } }
        public float X_rot_gen { get { return xr; } set { xr = value; Create_r3 = new Quaternion(xr, yr, zr,0); } }
        public float Y_rot_gen { get { return yr; } set { yr = value; Create_r3 = new Quaternion(xr, yr, zr,0); } }
        public float Z_rot_gen { get { return zr; } set { zr = value; Create_r3 = new Quaternion(xr, yr, zr,0); } }
        private Vector3 Create_v3 { get; set; }
        private Quaternion Create_r3 { get; set; }
        private GameObject Parent,Target;
        private Object Ob_Parent,Ob_Target;
        public Static_Generate(Object parent, Object target)
        {
            Ob_Target = target;
            Ob_Parent = parent;
            Parent = GameObject.Find(parent.name);
            Target = GameObject.Find(target.name);
            Create_v3 = Parent.transform.position;
            Create_r3 = Parent.transform.rotation;

        }

        public void Generates()
        {
            Object target = Ob_Target;
            Transform parent_trs = Parent.transform;
            Instantiate(target, Create_v3, Create_r3, parent_trs);
        }


        public void Print_ObjectMessege()
        {
            Object target = Ob_Target;
            Object parent = Ob_Parent;
            print($"The target is {target.name} that will be generate in {Create_v3}, {Create_r3}");
            print($"Will be generate in the {parent.name}.");
        }

    }


        #endregion

    #endregion

    #region 固定物件方法

    public void Static_gen()
    {
        string obj = "UFO_clone";
        float gener_pos_x = GameObject.Find("Border").transform.position.x;
        float gener_pos_y = GameObject.Find("Border").transform.position.y;
        //////
        Static_Generate sgen = new Static_Generate(this , Resources.Load(obj));
        sgen.X_pos_gen = gener_pos_x;
        sgen.Y_pos_gen = gener_pos_y;
        sgen.Print_ObjectMessege();
        sgen.Generates();
    }

    /// <summary>
    /// 刪除指定的子類別
    /// </summary>
    /// <param name="Parent">父物件名稱</param>
    public void Destroys(GameObject Parent)
    {
        if (Parent.transform.childCount != 0)
        {
            GameObject target = Parent.transform.GetChild(0).gameObject;
            Destroy(target);
        }
    }
    #endregion

    #region 隨機產生方法

    #endregion

    #region 補給品方法

    #endregion

    #region 事件

    #endregion
}
