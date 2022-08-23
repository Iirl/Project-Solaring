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
        [SerializeField, Header("連續呼叫")]
        private bool continues;
        [SerializeField, Header("指定距離開始產生物件")]
        private float generDestan =0;

        // 取得中央控制類別
        ManageCenter mgc;
        Object_Generator.Generater obGenerate;
        Object_Generator.ObjectArray gener_list = new();
        bool preLoadInvoke;
        /// <summary>
        /// 普通生成：只判斷是否旋轉物件。
        /// </summary>
        /// <param name="rotate"></param>
        public void NormalGenerate(bool rotate = false) => Static_gen(generData.grtRandomRoation);
        /// <summary>
        /// 子物件生成
        /// </summary>
        private void SubObjGenerate() => Random_gen(generData.grtRandomRoation);
        private void StaticPointGen() => Static_gen(generData.grtRandomRoation, false);
        public void Test()
        {
            
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
        public void Destroys(GameObject target, bool destTime = false)
        {
            // print(target.name);
            // 先讀取ID，然後找到清單中相同ID，刪除該清單編號。
            int id = target.transform.GetInstanceID();
            int key = gener_list.FindKeys(id);
            //print(key);
            if (key != -1)
            {
                // 移除清單內容
                gener_list.RemoveAt(key);
                // 執行物件刪除
                if (destTime) Destroy(target, 1f);
                else Destroy(target);
            }
            else
            {
                if (destTime) Destroy(target, 1f);
                else Destroy(target);
            }
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
                gener_list.RemoveAt(0);
            }
        }

        /// <summary>
        /// 若場上有未清除的物件，執行這段程式消除。
        /// 1. 超過生成上限。
        /// 2. 清單未存放資料，但已存在於場上。(修改成超過生成限制的半數)
        /// 3. 超過畫面一定距離。
        /// </summary>
        private void DestroysOnBug(Vector3 w_dist)
        {
            if (gener_list.Count > 0 && gener_list.Count > generData.grtLimit)
            {
                Object obj = gener_list.GetObject(1);
                gener_list.RemoveAt(0);
                Destroy(obj);
            }
            else if (gener_list.Count < 1 && transform.childCount > generData.grtLimit)
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
        private GameObject Generator_EMP(Vector3 worldOffset, bool isRoate = false ,bool random=true)
        {
            if (generData.grtObject == null) return null;
            DestroysOnBug(worldOffset);
            //show
            //Vector3 st_border = mgc.GetStageBorder();
            Vector3 random_v3 = new(Random.Range(-generData.grtPos.x, generData.grtPos.x),
                Random.Range(0f, generData.grtPos.y + generData.grtOffset),
                Random.Range(-generData.grtPos.z, generData.grtPos.z)
            );
            obGenerate = new(gameObject, generData.grtObject);                      // 在指定的位置[M]產生指定的物件[G]
            obGenerate.Create_v3 += (random) ? random_v3 + worldOffset : generData.grtPos + worldOffset;                // 物件生成的位置，會依據設定的位置改變。
            obGenerate.Create_r3 = (isRoate) ? Random.rotation : generData.grtRot;  // 物件生成方向是否隨機，預設為否。
            obGenerate.destoryTime = generData.grtdestTime;
            Object created = obGenerate.Generates();            
            gener_list.Add(created);                                          // 加入生成列表。
                                                                              //Destroys(generob.GetParent());
                                                                              //generob.ObjectMessegeInfo();

            // 若有設定銷毀時間，則加上銷毀的計時。            
            //Destroys(created.GetComponent<Transform>().gameObject, true);
            return created.GetComponent<Transform>().gameObject;
        }
        #endregion
        #region 物件產生方法的類型：定點、指定、隨機及帶有子物件生成。
        /// <summary>
        /// 簡易產生物件方法。
        /// </summary>
        private void Static_gen(bool isRot) => Generator_EMP(new Vector3(0, mgc.GetStagePOS().y, 0),isRot);
        private void Static_gen(bool isRot, bool isRnd) => Generator_EMP(new Vector3(0, mgc.GetStagePOS().y, 0), isRot, isRnd);
        private void Static_gen(float locY, bool isRotate) => Generator_EMP(new Vector3(0, locY, 0), isRotate);

        /// <summary>
        /// 將物件隨機生成在畫面中
        /// </summary>
        /// <param name="locY">目前空間的Y軸</param>
        /// <param name="isRotated">物件是否隨機旋轉</param>
        /// <returns>回傳為生成物件，用作執行下一個動作使用。</returns>
        private int Random_gen(bool isRotated)
        {
            Vector3 stage = new Vector3(0, mgc.GetStagePOS().y, 0);
            GameObject parentOB = Generator_EMP(stage, isRotated);
            Random_Metro(parentOB, generData.grtSubObject);
            return -1;
        }
        /// <summary>
        /// 物件中的物件生成
        /// </summary>
        /// <param name="parent">父物件的ID</param>
        /// <param name="TG">要生成的物件</param>
        private void Random_Metro(GameObject parent, List<Object> TG)
        {
            // 
            parent = parent.transform.GetChild(0).gameObject;
            int sub_count = 0;
            int sub_max = parent.transform.childCount;
            while (sub_count < sub_max)
            {
                int rnd = Random.Range(0, TG.Count);


                GameObject PAOB = parent.transform.GetChild(0).gameObject; ;
                try
                {
                    // 取得子物件的父物件
                    PAOB = parent.transform.GetChild(sub_count).gameObject;
                    // 子物件
                }
                catch (System.Exception)
                {
                    print("SubgenID Fail");
                    break;
                }

                // 生成物件
                if (Random.value < generData.grtProb)
                {
                    Object_Generator.Generater sgen = new(PAOB, TG[rnd], PAOB.transform.position, PAOB.transform.rotation * Quaternion.AngleAxis(30, Vector3.right));
                    sgen.Generates();
                }
                sub_count++;
            }
        }

        #endregion
        /// <summary>
        /// 切換生成內容系統
        /// 會根據類別決定產生的方法
        /// </summary>
        private void SwitchState()
        {
            if (transform.childCount < generData.grtLimit)
            {
                switch (generData.grtClass)
                {
                    case GenerClass.Normal:
                        NormalGenerate();
                        break;
                    case GenerClass.Meteorite:
                        SubObjGenerate();
                        break;
                    case GenerClass.StaticPoint:
                        StaticPointGen();
                        break;
                    default:
                        break;
                }
            }
            //print("呼叫次數");
        }

        private IEnumerator IntervalGenerate()
        {
            while (ManageCenter.UI_moveDistane < generDestan && !preLoadInvoke) yield return null;          // 距離指定
            if (continues) InvokeRepeating("SwitchState", generData.grtIntervalTime, generData.grtWaitTime);// 持續與一次性
            else Invoke("SwitchState", generData.grtIntervalTime);           
           
        }
        private void Awake()
        {
            mgc = FindObjectOfType<ManageCenter>();
        }
        private void Start()
        {
            preLoadInvoke = IsInvoking();
            StartCoroutine(IntervalGenerate());

        }
        private void Update()
        {
            //AutoGenerate();
        }
    }

}

