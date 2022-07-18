using UnityEngine;
using UnityEditor;
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
        GameObject MainObject;
        [SerializeField, Header("生成物件"), Tooltip("放入指定的物件，最好是有 Prefabs 過的檔案")]
        Object Generate;
        [SerializeField, Tooltip("指定物件位置")]
        Vector3 Generate_pos = Vector3.zero;
        [SerializeField, Tooltip("指定物件旋轉")]
        Quaternion Generate_rot = Quaternion.identity;
        [SerializeField, Tooltip("指定物件生成半徑，僅在隨機生成套用")]
        Vector3 Generate_posRaidus = new Vector3(20, 10, 20);
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
                Object cloned = Instantiate(OBTarget, Create_v3, Create_r3, Parent.transform);
                OBCloned = cloned;
                return cloned;
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

        #endregion


        public void ReadList()
        {
            int count = 0;
            foreach (GameObject item in gener_list)
            {
                if (item != null) print(item.name);
                count++;
            }
        }
        public GameObject ReadList(int target)
        {
            int count = 0;
            GameObject idx = null;
            foreach (GameObject item in gener_list)
            {
                if (item != null && count == target) idx = item;
                count++;
            }
            return idx;
        }
        /// <summary>
        /// 自動刪除指定的子類別，物件生成時自動判定是否超過生成上限。
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
        public void Destroys(int idx)
        {
            if (gener_list.Count > 0)
            {
                GameObject t = ReadList(idx);
                gener_list.RemoveAt(idx);
                Destroy(t);
            }
        }
        public void Destroys()
        {
            if (gener_list.Count < Generate_limit)
            {
                gener_list.RemoveAt(0);                
            }
        }
        #region 固定物件方法
        /// <summary>
        /// 根據面板屬性產生物件。
        /// </summary>
        public void Static_gen()
        {
            Generater generob = new Generater(MainObject, Generate, Generate_pos, Generate_rot);
            gener_list.Add(generob.Generates());
            Destroys(generob.GetParent());
        }

        /// <summary>
        /// 根據面板指定生成位置。
        /// </summary>
        /// <param name="locY">加上目前場景的位置</param>
        /// <param name="x">指定 x 軸座標位移</param>
        /// <param name="y">指定 y 軸座標位移</param>
        public void Static_gen(float locY, float x, float y)
        {
            Generater sgen;
            if (Generate != null && MainObject != null)
            {
                sgen = new Generater(MainObject, Generate);
                //sgen.ObjectMessegeInfo();
                sgen.Create_v3 = Generate_pos;
                sgen.Create_v3.y += locY + y;
                sgen.Create_v3.x += x;
                sgen.Create_r3 = Generate_rot;
                gener_list.Add(sgen.Generates());
                Destroys(sgen.GetParent());
            }
        }
        #endregion

        #region 隨機產生方法
        /// <summary>
        /// 將物件隨機生成在畫面中
        /// </summary>
        /// <param name="locY">目前空間的Y軸</param>
        /// <param name="isRotated">物件是否隨機旋轉</param>
        /// <returns>回傳為生成物件，用作執行下一個動作使用。</returns>
        public int Random_gen(float locY, bool isRotated)
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
                return sgen.OBCloned.GetInstanceID();
            }
            return -1;
        }

        /// <summary>
        /// 物件中的物件生成
        /// </summary>
        /// <param name="PA"></param>
        /// <param name="TG"></param>
        public void Random_Metro(int PAID, GameObject TG)
        {

            // 子物件計算：父物件到子物件的ID距離=2+4+10=16(0),20(1),24(2),28(3),32(4)。
            int sub_count = 16, i =1;
            int sub_max = sub_count + (4 * 4);
            GameObject PA = null;
            while (sub_count <= sub_max)
            {
                try
                {
                    // 轉換ID到父物件

                    PA = ((Transform)EditorUtility.InstanceIDToObject(PAID - 2)).gameObject;
                    PA = PA.transform.GetChild(i).gameObject;
                    // 子物件

                }
                catch (System.Exception)
                {
                    break;
                }

                // 生成物件
                Generater sgen = new(PA, TG, PA.transform.position, PA.transform.rotation * Quaternion.AngleAxis(30, Vector3.right)) ;
                gener_list.Add(sgen.Generates());
                if (gener_list.Count > Generate_limit )Destroys(0);
                sub_count += 4; i++;
            }
        }
        #endregion

    }


}