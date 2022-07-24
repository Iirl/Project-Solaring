using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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
        List<GameObject> MainObject = new List<GameObject>();
        [SerializeField, Header("生成物件"), Tooltip("放入指定的物件，最好是有 Prefabs 過的檔案")]
        List<Object> Generate = new List<Object>();
        [SerializeField, Tooltip("指定物件位置")]
        Vector3 Generate_pos = Vector3.zero;
        [SerializeField, Tooltip("指定物件旋轉")]
        Quaternion Generate_rot = Quaternion.identity;
        [SerializeField, Tooltip("指定物件生成半徑，僅在隨機生成套用")]
        Vector3 Generate_posRaidus = new Vector3(20, 10, 20);
        [SerializeField, Header("指定物件生成數量上限")]
        int Generate_limit = 10;
        public ObjectArray gener_list = new();

        #region 產生器類別 class Generater
        /// <summary>
        /// Instantiate 方法的加強版類別
        /// </summary>
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
            public Vector3 Create_v3 = Vector3.zero;
            public Quaternion Create_r3 = Quaternion.identity;
            private GameObject Parent;
            public Object OBTarget, OBCloned;

            /// <summary>
            /// 製作物件時會自動指定主物件、生成物以及位置和旋轉狀態
            /// </summary>
            /// <param name="parent">主物件，要在哪個物件上生成</param>
            /// <param name="target">目標物件，甚麼 Object 會被生成</param>
            public Generater(GameObject parent, Object target)
            {
                OBTarget = target;
                Parent = parent;
            }
            /// <summary>
            /// 製作物件時手動指定主物件、生成物以及位置和旋轉狀態
            /// </summary>
            /// <param name="parent">主物件，要在哪個物件上生成</param>
            /// <param name="target">目標物件，甚麼 Object 會被生成</param>
            /// <param name="pos">目標物件的三維座標</param>
            /// <param name="rot">目標物件的旋轉座標，輸入0表示套用預設值</param>
            public Generater(GameObject parent, Object target, Vector3 pos, Quaternion rot)
            {
                OBTarget = target;
                Parent = parent;
                Create_v3 = pos;
                Create_r3 = rot;

            }

            /// <summary>
            /// 自動產生物件，會根據既有欄位決定生成的內容
            /// </summary>
            public Object Generates()
            {
                if (OBTarget != null && Parent != null)
                {
                    Object cloned = Instantiate(OBTarget, Create_v3, Create_r3, Parent.transform);
                    OBCloned = cloned;
                    return cloned;
                }
                return null;
            }


            /// <summary>
            /// 物件產生的訊息，除錯用。
            /// </summary>
            public void ObjectMessegeInfo()
            {
                Object target = OBTarget;
                Object parent = Parent;
                print($"The target is {target.name} that will be generate in {Create_v3}, {Create_r3}");
                print($"Will be generate in the {parent.name}.");

            }
            public GameObject GetParent()
            {
                return Parent;
            }

        }


        #endregion
        #region 陣列清單類別 ObjectArray
        /// <summary>
        /// 修改將物件對清單操作時的一些基本資訊
        /// </summary>
        public class ObjectArray : ArrayList
        {
            /// <summary>
            /// 追加第二維陣列
            /// 相關資訊如下：
            /// 0   InstanceID
            /// 1   Parent Object self
            /// 2   Object'name
            /// 3   Position
            /// 4   Rotation
            /// 5   Scale
            /// </summary>
            /// <param name="o"></param>
            /// <returns></returns>
            public override int Add(object o)
            {
                Object uo = (Object)o;
                Transform uot = FindObjectOfType<Transform>();
                // 加入 Object 為一陣列。
                ArrayList newlist = new();
                newlist.Add(uo.GetInstanceID());
                newlist.Add(uo);
                newlist.Add(uo.name);
                newlist.Add(uot.localPosition);
                newlist.Add(uot.localRotation);
                newlist.Add(uot.localScale);

                int i = base.Add(newlist);
                return i;
            }
            public override void RemoveAt(int i)
            {
                ArrayList al = (ArrayList)this[i];
                if (al != null)
                {
                }

                base.RemoveAt(i);

            }
            /// <summary>
            /// 測試用讀取清單函數
            /// </summary>
            public void ReadList()
            {
                foreach (ArrayList item in this)
                {
                    if (item != null) print(item);
                    foreach (var item2 in item) print(item2);
                }
            }
            public int ReadList(int i)
            {
                if (i <= this.Count)
                {
                    ArrayList item = (ArrayList)this[i];
                    if (item != null) return (int)item[0];
                }
                return -1;
            }
            public int FindKeys(int id)
            {
                int key = 0;
                foreach (ArrayList item in this)
                {
                    int j = (int)item[0];
                    if (j == id) break;
                    key++;
                }
                if (key == this.Count) return -1;
                return key;
            }
        }
        #endregion

        #endregion

        public void r()
        {
            // 測試讀取陣列清單的內容
            gener_list.ReadList();
            print(gener_list.Count);
        }
        /// <summary>
        /// 自動刪除指定的子類別，物件生成時自動判定是否超過生成上限。
        /// </summary>
        /// <param name="target">觸發銷毀的物件</param>
        public void Destroys(GameObject target)
        {
            // 先讀取ID，然後找到清單中相同ID，刪除該清單編號。
            int id = target.GetInstanceID();
            int key = gener_list.FindKeys(id);
            if (key != -1) gener_list.RemoveAt(key);
            Destroy(target);
        }
        /// <summary>
        /// 清空物件系統
        /// </summary>
        /// <param name="clear">第二重保護，True 才會啟動清除</param>
        public void Destroys(bool clear)
        {
            if (!clear) return;
            int max = gener_list.Count;
            for (int i = 0; i < max; i++)
            {
                ArrayList al = (ArrayList)gener_list[0];
                Object obj = (Object)al[1];
                Destroy(obj);
                gener_list.RemoveAt(0);
            }
        }

        /// <summary>
        /// 若場上有未清除的物件，執行這段程式消除。
        /// 1. 超過生成上限。
        /// 2. 清單未存放資料，但已存在於場上。
        /// 3. 超過畫面一定距離。
        /// </summary>
        /// <param name="i"></param>
        public void DestroysOnBug(int i)
        {
            if (gener_list.Count > 0 && gener_list.Count > Generate_limit)
            {
                Object obj = EditorUtility.InstanceIDToObject(gener_list.ReadList(0));
                gener_list.RemoveAt(0);
                Destroy(obj);
            }
            else if (gener_list.Count < 1 && MainObject[i].transform.childCount > 0)
            {
                int max = MainObject[i].transform.childCount;
                for (int bug_i = 0; bug_i < max; bug_i++) Destroy(MainObject[i].transform.GetChild(bug_i).gameObject);

            }

            for (int bug_i = 0; bug_i < MainObject[i].transform.childCount; bug_i++)
            {
                GameObject child_gob = MainObject[i].transform.GetChild(bug_i).gameObject;
                float dis = Vector3.Distance(child_gob.transform.position, MainObject[i].transform.position);
                if (dis > 50)
                {
                    try { gener_list.RemoveAt(gener_list.FindKeys(child_gob.GetInstanceID())); }
                    catch (System.Exception) { }
                    Destroy(child_gob);
                }
            }

        }
        /// <summary>
        /// 初始生成函數，如果甚麼都不傳入的話，至少要傳入目前Y的位置，才會在畫面上看到。
        /// </summary>
        /// <param name="worldOffset">目前場景的座標</param>
        /// <param name="i">指定生成列表的物件</param>
        /// <param name="isPos">是否隨機位置</param>
        /// <param name="isRoate">是否隨機旋轉</param>
        /// <returns></returns>
        private Generater Generator_EMP(Vector3 worldOffset, int i = 0, bool isPos = true, bool isRoate = false)
        {
            DestroysOnBug(i);
            //show
            Vector3 random_v3 = new(Random.Range(-Generate_posRaidus.x, Generate_posRaidus.x),
                Random.Range(0f, Generate_posRaidus.y),
                Random.Range(-Generate_posRaidus.z, Generate_posRaidus.z)
            );
            Generater generob = new(MainObject[i], Generate[i]);
            generob.Create_v3 = (isPos) ? random_v3 + worldOffset : Generate_pos + worldOffset; // 物件生成位置是否隨機，預設為是。
            generob.Create_r3 = (isRoate) ? Random.rotation : Generate_rot;                     // 物件生成方向是否隨機，預設為否。
            gener_list.Add(generob.Generates());               // 加入生成列表。
                                                               //Destroys(generob.GetParent());
            return generob;

        }
        #region 固定物件方法
        /// <summary>
        /// 簡易產生物件方法。
        /// </summary>
        public void Static_gen(float locY)
        {
            Vector3 stage = new Vector3(0, locY, 0);
            Generator_EMP(stage);
        }

        /// <summary>
        /// 根據面板指定生成位置。
        /// </summary>
        /// <param name="locY">加上目前場景的位置</param>
        /// <param name="i">生成物件的編號</param>
        /// <param name="x">指定 x 軸座標位移</param>
        /// <param name="y">指定 y 軸座標位移</param>
        public void Static_gen(float locY, int i, float x = 0, float y = 0, bool rot = false)
        {
            Generater sgen;
            Vector3 stage = new Vector3(0, locY, 0);
            if (x != 0 || y != 0) Generate_pos = new Vector3(x, y, 0);
            if (rot) Generate_rot = new Quaternion(x, y, 0, 0);
            if (Generate.Count > 0 && MainObject.Count > 0)
            {
                sgen = Generator_EMP(stage, i, false);
                //sgen.ObjectMessegeInfo();
            }
        }
        #endregion

        #region 隨機產生方法
        /// <summary>
        /// 將物件隨機生成在畫面中
        /// </summary>
        /// <param name="locY">目前空間的Y軸</param>
        /// <param name="isRotated">物件是否隨機旋轉</param>
        /// <param name="i">生成物件編號</param>
        /// <returns>回傳為生成物件，用作執行下一個動作使用。</returns>
        public int Random_gen(float locY, bool isRotated, int i)
        {
            Generater sgen;
            Vector3 stage = new Vector3(0, locY, 0);
            if (Generate.Count > 0 && MainObject.Count > 0)
            {
                sgen = Generator_EMP(stage, i, true, isRotated);
                if (sgen != null) return sgen.OBCloned.GetInstanceID();

            }

            return -1;
        }
        /// <summary>
        /// 物件中的物件生成
        /// </summary>
        /// <param name="PAID">父物件的ID</param>
        /// <param name="TG">要生成的物件</param>
        public void Random_Metro(int PAID, List<Object> TG)
        {
            //print($"id:{PAID}, GTB:{TG}");
            // 子物件計算：父物件到子物件的ID距離=2+10+10=22
            int sub_count = 22, i = 1;
            int sub_max = sub_count + (4 * 6);
            GameObject PA = null;
            while (sub_count <= sub_max)
            {
                int rnd = Random.Range(0, 3);
                try
                {
                    // 轉換ID到父物件

                    PA = ((Transform)EditorUtility.InstanceIDToObject(PAID - 2)).gameObject;
                    PA = PA.transform.GetChild(i).gameObject;
                    // 子物件

                }
                catch (System.Exception)
                {
                    print("Fial");
                    break;
                }

                // 生成物件
                Generater sgen = new(PA, TG[rnd], PA.transform.position, PA.transform.rotation * Quaternion.AngleAxis(30, Vector3.right));
                sgen.Generates();
                sub_count += 4; i++;
            }
        }
        #endregion

    }


}

#region 筆記
/*
  物件的銷毀：
    1. 不可以直接用 Destory 移除物件，這樣生成上限就沒辦法控制。
    2. 物件銷毀最好搭配 RemoveAt 使用。
    3. 清除順序：物件->清單->?


*/
#endregion