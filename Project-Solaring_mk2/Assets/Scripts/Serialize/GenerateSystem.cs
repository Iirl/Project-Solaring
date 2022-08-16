using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace solar_a
{
    public class GenerateSystem : MonoBehaviour
    {
        [SerializeField, Header("物件資料")]
        GeneratorData generData;

        // 取得中央控制類別
        ManageCenter mgc;
        Object_Generator.Generater obGenerate;
        Object_Generator.ObjectArray gener_list = new();

        public void AutoGenerate(bool rotate = false)
        {
            Vector3 st_border = mgc.GetStageBorder();
            Static_gen(st_border.y);
        }
        public void Test()
        {
            Vector3 st_border = mgc.GetStageBorder();
            int id = Random_gen(st_border.y / 4, true);
            print(id);
            //gener_list.ReadList();

        }

        #region 物件檢查系統

        public void _ReadOBJList()
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
        /// 2. 清單未存放資料，但已存在於場上。(修改成超過生成限制的半數)
        /// 3. 超過畫面一定距離。
        /// </summary>
        public void DestroysOnBug(Vector3 w_dist)
        {
            if (gener_list.Count > 0 && gener_list.Count > generData.grtLimit)
            {
                Object obj = Resources.InstanceIDToObject(gener_list.ReadList(0));
                gener_list.RemoveAt(0);
                Destroy(obj);
            }
            else if (gener_list.Count < 1 && transform.childCount > generData.grtLimit / 2)
            {
                int max = transform.childCount;
                for (int bug_i = 0; bug_i < max; bug_i++) Destroy(transform.GetChild(bug_i).gameObject);
            }
            //// 此條判定容易造成產生器誤判，所以如果要和場景不同移動，要記得指定場景的位置...。
            for (int bug_i = 0; bug_i < transform.childCount; bug_i++)
            {
                GameObject child_gob = transform.GetChild(bug_i).gameObject;
                float dis = Vector3.Distance(child_gob.transform.position, w_dist);
                if (dis > 100)
                {
                    try { gener_list.RemoveAt(gener_list.FindKeys(child_gob.GetInstanceID())); }
                    catch (System.Exception) { }
                    Destroy(child_gob);
                }
            }

        }
        /// <summary>
        /// 物件生成總系統。
        /// 如果甚麼都不傳入的話，至少要傳入目前Y的位置，才會在畫面上看到。
        /// </summary>
        /// <param name="worldOffset">目前場景的座標</param>
        /// <param name="i">指定生成列表的物件</param>
        /// <param name="isPos">是否隨機位置</param>
        /// <param name="isRoate">是否隨機旋轉</param>
        /// <returns></returns>
        private GameObject Generator_EMP(Vector3 worldOffset, bool isRoate = false)
        {
            if (generData.grtObject == null) return null;
            DestroysOnBug(worldOffset);
            //show
            //Vector3 st_border = mgc.GetStageBorder();
            Vector3 random_v3 = new(Random.Range(-generData.grtPos.x, generData.grtPos.x),
                Random.Range(0f, generData.grtPos.y),
                Random.Range(-generData.grtPos.z, generData.grtPos.z)
            );

            obGenerate = new(gameObject, generData.grtObject);                      // 在指定的位置[M]產生指定的物件[G]
            obGenerate.Create_v3 = random_v3 + worldOffset;                         // 物件生成的位置，會依據設定的位置改變。
            obGenerate.Create_r3 = (isRoate) ? Random.rotation : generData.grtRot;  // 物件生成方向是否隨機，預設為否。
            gener_list.Add(obGenerate.Generates());               // 加入生成列表。
                                                                  //Destroys(generob.GetParent());
                                                                  //generob.ObjectMessegeInfo();
            return gener_list.GetGameObject(gener_list.Count-1);
        }
        #endregion
        #region 物件產生方法的類型：定點、指定、隨機及帶有子物件生成。
        /// <summary>
        /// 簡易產生物件方法。
        /// </summary>
        public void Static_gen(float locY)
        {
            Vector3 stage = new Vector3(0, locY, 0);
            Generator_EMP(stage);
        }


        /// <summary>
        /// 將物件隨機生成在畫面中
        /// </summary>
        /// <param name="locY">目前空間的Y軸</param>
        /// <param name="isRotated">物件是否隨機旋轉</param>
        /// <returns>回傳為生成物件，用作執行下一個動作使用。</returns>
        public int Random_gen(float locY, bool isRotated)
        {
            Vector3 stage = new Vector3(0, locY, 0);
            Generator_EMP(stage, isRotated);

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
            // 子物件計算：sub_count為父物件到子物件的ID距離(每多一個元件數值就會改變...)
            int sub_count = 22, i = 1;
            int sub_max = sub_count + (4 * 6);
            GameObject PA = null;
            while (sub_count <= sub_max)
            {
                int rnd = Random.Range(0, TG.Count);
                try
                {
                    // 轉換ID到父物件
                    PA = ((Transform)Resources.InstanceIDToObject(PAID - 2)).gameObject;
                    PA = PA.transform.GetChild(i).gameObject;
                    // 子物件
                }
                catch (System.Exception)
                {
                    print("SubgenID Fail");
                    break;
                }

                // 生成物件
                Object_Generator.Generater sgen = new(PA, TG[rnd], PA.transform.position, PA.transform.rotation * Quaternion.AngleAxis(30, Vector3.right));
                sgen.Generates();
                sub_count += 4; i++;
            }
        }

        #endregion
        private void SwitchState()
        {
            switch (generData.grtClass)
            {
                case GenerClass.Normal:
                    break;
                case GenerClass.Meteorite:
                    break;
                default:
                    break;
            }
        }

        private void Awake()
        {
            mgc = FindObjectOfType<ManageCenter>();
        }
        private void Update()
        {
            //AutoGenerate();
        }
    }

}

