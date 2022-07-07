using UnityEngine;

/// <summary>
/// 此為在區域空間中產生物件的程式
/// </summary>
namespace solar_a
{
    public class Object_Generator : MonoBehaviour
    {

        #region 屬性
        //SceneStage_Control Scene_ctl;

        [SerializeField, Header("生成位置")]
        Object MainObject;
        [SerializeField, Header("生成物件"), Tooltip("放入指定的物件，最好是有 Prefabs 過的檔案")]
        Object Generate;
        [SerializeField, Tooltip("指定物件位置")]
        Vector3 Generate_pos = Vector3.zero;
        [SerializeField, Tooltip("指定物件旋轉")]
        Quaternion Generate_rot = Quaternion.identity;
        [SerializeField, Header("指定物件生成數量上限")]
        int Generate_limit =10;

        #region 產生器類別
        public class Generater
        {
            private float x { get; set; }
            private float y { get; set; }
            private float z { get; set; }
            private float xr { get; set; }
            private float yr { get; set; }
            private float zr { get; set; }
            public float X_pos_gen { get { return x; } set { x = value; Create_v3 = new Vector3(x, y, z); } }
            public float Y_pos_gen { get { return y; } set { y = value; Create_v3 = new Vector3(x, y, z); } }
            public float Z_pos_gen { get { return z; } set { z = value; Create_v3 = new Vector3(x, y, z); } }
            public float X_rot_gen { get { return xr; } set { xr = value; Create_r3 = new Quaternion(xr, yr, zr, 0); } }
            public float Y_rot_gen { get { return yr; } set { yr = value; Create_r3 = new Quaternion(xr, yr, zr, 0); } }
            public float Z_rot_gen { get { return zr; } set { zr = value; Create_r3 = new Quaternion(xr, yr, zr, 0); } }
            public Vector3 Create_v3 { get; set; }
            public Quaternion Create_r3 { get; set; }
            private GameObject Parent, Target;
            private Object Ob_Parent, Ob_Target;

            /// <summary>
            /// 製作物件時會自動指定主物件、生成物以及位置和旋轉狀態
            /// </summary>
            /// <param name="parent">主物件，要在哪個地方生成</param>
            /// <param name="target">目標物件，甚麼 Object 會被生成</param>
            public Generater(Object parent, Object target)
            {
                Ob_Target = target;
                Ob_Parent = parent;
                Parent = GameObject.Find(parent.name);
                Target = GameObject.Find(target.name);
                Create_v3 = new Vector3(Random.Range(-20,20), Random.Range(9, 14));
                Create_r3 = Quaternion.identity;

            }
            public Generater(Object parent, Object target, Vector3 pos, Quaternion rot)
            {
                Ob_Target = target;
                Ob_Parent = parent;
                Parent = GameObject.Find(parent.name);
                Target = GameObject.Find(target.name);
                Create_v3 = pos;
                Create_r3 = rot;

            }

            /// <summary>
            /// 自動產生物件，會根據既有欄位決定生成的內容
            /// </summary>
            public void Generates()
            {
                Object target = Ob_Target;
                Transform parent_trs = Parent.transform;
                Instantiate(target, Create_v3, Create_r3, parent_trs);
            }

            /// <summary>
            /// 物件產生的訊息，除錯用。
            /// </summary>
            public void Print_ObjectMessege()
            {
                Object target = Ob_Target;
                Object parent = Ob_Parent;
                print($"The target is {target.name} that will be generate in {Create_v3}, {Create_r3}");
                print($"Will be generate in the {parent.name}.");
            }
            public GameObject GetParent()
            {
                return Parent;
            }
            public GameObject GetTarget()
            {
                return Target;
            }

        }


        #endregion

        #endregion

        #region 固定物件方法

        public void Static_gen()
        {
            Generater sgen;
            if (Generate != null && MainObject != null)
            {
                sgen = new Generater(MainObject, Generate);
                sgen.Print_ObjectMessege();
                sgen.Generates();
                sgen.Create_v3 = Generate_pos;
                sgen.Create_r3 = Generate_rot;
            }
            int totals = GameObject.Find(MainObject.name).transform.childCount;
            print(totals);
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
}