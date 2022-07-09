using UnityEngine;
using System.Collections;

/// <summary>
/// 此為在區域空間中產生物件的程式
/// </summary>
namespace solar_a
{
    [AddComponentMenu("Transform/物件產生器")]
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
        [SerializeField, Tooltip("指定物件生成半徑，僅在隨機生成套用")]
        Vector3 Generate_posRaidus = new Vector3(20,10,20);
        [SerializeField, Header("指定物件生成數量上限")]
        int Generate_limit = 10;
        ArrayList gener_list = new ArrayList();

        #region 產生器類別 class Generater
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
            public Vector3 Create_v3 = Vector3.one;
            public Quaternion Create_r3 = Quaternion.identity;
            private GameObject Parent, Target;
            private Object OBParent, OBTarget;
            public Object Ob_Parent
            {
                get { return OBParent; }
                set { OBParent = value; Parent = GameObject.Find(value.name); }
            }
            public Object Ob_Target
            {
                get { return OBTarget; }
                set { OBTarget = value; Target = GameObject.Find(value.name); }
            }

            /// <summary>
            /// 製作物件時會自動指定主物件、生成物以及位置和旋轉狀態
            /// </summary>
            /// <param name="parent">主物件，要在哪個物件上生成</param>
            /// <param name="target">目標物件，甚麼 Object 會被生成</param>
            public Generater(Object parent, Object target)
            {
                Ob_Target = target;
                Ob_Parent = parent;
            }
            /// <summary>
            /// 製作物件時手動指定主物件、生成物以及位置和旋轉狀態
            /// </summary>
            /// <param name="parent">主物件，要在哪個物件上生成</param>
            /// <param name="target">目標物件，甚麼 Object 會被生成</param>
            /// <param name="pos">目標物件的三維座標</param>
            /// <param name="rot">目標物件的旋轉座標，輸入0表示套用預設值</param>
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
            public Object Generates()
            {
                Object target = Ob_Target;
                Transform parent_trs = Parent.transform;
                Object cloned = Instantiate(target, Create_v3, Create_r3, parent_trs);
                return cloned;
            }


            /// <summary>
            /// 物件產生的訊息，除錯用。
            /// </summary>
            public void ObjectMessegeInfo()
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


        private void ReadList()
        {
            foreach (GameObject item in gener_list)
            {
                if (item != null) print(item.name);
            }
        }
        /// <summary>
        /// 刪除指定的子類別
        /// </summary>
        /// <param name="target">父物件名稱</param>
        public void Destroys(GameObject target)
        {
            if (gener_list.Count > 0 && gener_list.Count > Generate_limit)
            {
                GameObject t = target.transform.GetChild(0).gameObject;
                gener_list.RemoveAt(0);
                Destroy(t);
            }
        }

        #region 固定物件方法
        /// <summary>
        /// 根據面板屬性產生物件。
        /// </summary>
        public void Static_gen()
        {
            Generater generob = new Generater(MainObject, Generate, Generate_pos , Generate_rot);
            gener_list.Add(generob.Generates());
            Destroys(generob.GetParent());
        }

        /// <summary>
        /// 根據面板指定生成位置。
        /// </summary>
        /// <param name="locY">加上目前場景的位置</param>
        /// <param name="x">指定 x 軸座標位置</param>
        /// <param name="y">指定 y 軸座標位置</param>
        public void Static_gen(float locY, float x, float y)
        {
            Generater sgen;
            if (Generate != null && MainObject != null)
            {
                sgen = new Generater(MainObject, Generate);
                //sgen.ObjectMessegeInfo();
                sgen.Create_v3.y += locY;
                sgen.Create_v3 = Generate_pos;
                sgen.Create_r3 = Generate_rot;
                gener_list.Add(sgen.Generates());
                Destroys(sgen.GetParent());
            }
        }
        #endregion

        #region 隨機產生方法
        public void Random_gen(float locY, bool isRotated)
        {
            Generater sgen;
            Vector3 random_v3 = new(Random.Range(-Generate_posRaidus.x, Generate_posRaidus.x),
                Random.Range(0f, Generate_posRaidus.y), 
                Random.Range(-Generate_posRaidus.z, Generate_posRaidus.z)
            );
            if (Generate != null && MainObject != null)
            {
                sgen = new Generater(MainObject, Generate, random_v3, Generate_rot);
                sgen.Create_v3.y += locY;
                if (isRotated) sgen.Create_r3 = Random.rotation;
                gener_list.Add(sgen.Generates());
                Destroys(sgen.GetParent());
            }
        }
        #endregion

        #region 事件
        #endregion
    }


}