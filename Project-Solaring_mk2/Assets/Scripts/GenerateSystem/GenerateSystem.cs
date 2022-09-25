using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace solar_a
{
    /// <summary>
    /// 物件產生系統，掛上此系統的物件，會根據資料產生物件。
    /// </summary>
    public class GenerateSystem : MonoBehaviour
    {
        [SerializeField, Header("物件資料")]
        GeneratorData generData;
        [SerializeField, Header("連續呼叫")]
        private bool continues;
        [SerializeField, Header("指定距離開始產生物件(x~y)"), Tooltip("如果為零表示不限制")]
        private Vector2 generDestan;

        // 取得中央控制類別
        ManageCenter mgc;
        Object_Generator.Generater obGenerate;
        public Object_Generator.ObjectArray gener_list;
        bool preLoadInvoke;
        float timeCounter;
        public void RealseTheObject(GameObject obj) => TakeMethod(obj);
        //普通生成：只判斷是否旋轉物件。
        public void NormalGenerate(bool rotate = false) => Random_gen(generData.grtRandomRoation);
        // 子物件生成
        private void SubObjGenerate() => Generator_Random(generData.grtRandomRoation);
        private void StaticPointGen() => Static_gen(generData.grtPos, generData.grtRandomRoation);
        private void PreviousRecordPostition() => Static_gen(generData.grtPos, generData.grtRandomRoation);
        // 根節點生成
        private void Root_StaticPoint() => Static_gen(generData.grtPos, generData.grtRandomRoation);

        #region 物件檢查系統

        public void _ReadOBJList()
        {
            // 測試讀取陣列清單的內容
            gener_list.ReadList();
            print(gener_list.Count);
        }
        /// <summary>
        /// 自動刪除指定的子類別，物件生成時自動判定是否超過生成上限。
        /// 中控取得銷毀方法的地方。
        /// </summary>
        /// <param name="target">觸發銷毀的物件</param>
        public void ReleaseObj(GameObject target, bool destTime = false)
        {
            // print(target.name);
            // 先讀取ID，然後找到清單中相同ID，刪除該清單編號。
            int id = target.transform.GetInstanceID();
            int key = gener_list.FindKeys(id);
            if (key != -1) gener_list.ReleaseAt(key);
        }
        /// <summary>
        /// 清空物件系統
        /// </summary>
        /// <param name="clear">第二重保護，True 才會啟動清除</param>
        public void RenewObjAll(bool clear)
        {
            if (!clear) return;
            gener_list.RemoveAll();
        }
        private void TakeMethod(GameObject obj, bool? isOff = true)
        {
            if (!obj) return;
            if (isOff == null) obGenerate.GenerateOffClear(obj);
            else if (isOff == true) { obGenerate.GenerateOff(obj);}
        }
        /// <summary>
        /// 物件生成總系統。
        /// 如果甚麼都不傳入的話，至少要傳入目前Y的位置，才會在畫面上看到。
        /// </summary>
        /// <param name="worldOffset">目前場景的座標</param>
        /// <param name="isRoate">是否隨機旋轉</param>
        /// <param name="randPos">是否隨機位置</param>
        /// <returns></returns>
        private GameObject Generator_EMP(Vector3 worldOffset, bool isRoate = false, bool randPos = true)
        {
            if (generData.grtObject == null) return null;
            //DestroysOnBug(worldOffset);
            worldOffset += transform.position;
            worldOffset.y += generData.grtOffset; // Offset 會調整Y軸座標
            Vector3 random_v3 = new(Random.Range(-generData.grtPos.x, generData.grtPos.x),
                Random.Range(0f, generData.grtPos.y),
                Random.Range(-generData.grtPos.z / 2, generData.grtPos.z / 2)
            );
            //print(random_v3);                     // 在指定的位置[M]產生指定的物件[G]
            obGenerate.Create_v3 = (randPos) ? random_v3 + worldOffset : generData.grtPos + worldOffset;                // 物件生成的位置，會依據設定的位置改變。
            obGenerate.Create_r3 = (isRoate) ? Random.rotation : generData.grtRot;  // 物件生成方向是否隨機，預設為否。
            GameObject created =null;
            try
            {
                created = obGenerate.GenerateOut();
                created.GetComponent<Simple_move>().releaseObj = RealseTheObject;
                gener_list.Add(created);
                if (generData.grtdestTime > 0) StartCoroutine(RealseTimer(created, generData.grtdestTime));

            }
            catch (System.Exception)
            {
                print("物件池物件清空或遺失，重新生成喔~");
                obGenerate.GenerateOffClear(created);
            }
            return created;
        }
        /// <summary>
        /// 將物件隨機生成在畫面中
        /// </summary>
        /// <param name="locY">目前空間的Y軸</param>
        /// <param name="isRotated">物件是否隨機旋轉</param>
        /// <returns>回傳為生成物件，用作執行下一個動作使用。</returns>
        private int Generator_Random(bool isRotated)
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
        private void Random_Metro(GameObject parent, List<GameObject> TG)
        {
            // 
            try
            {
                parent = parent.transform.GetChild(0).gameObject;
            }
            catch (System.Exception) { return; }
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
                    Object_Generator.Generater sgen = new(PAOB, TG[rnd]);
                    sgen.Create_v3 = PAOB.transform.position;
                    sgen.Create_r3 = PAOB.transform.rotation * Quaternion.AngleAxis(30, Vector3.right);
                    sgen.Generates(); // 子物件不須回收
                }
                sub_count++;
            }
        }
        /// <summary>
        /// 釋放計時器，在物件生成時就先給予計時器，時間到之後就會自動回收。
        /// </summary>
        private IEnumerator RealseTimer(GameObject obj, float time)
        {
            //print($"{name}會在{time}秒後銷毀");
            yield return new WaitForSeconds(time);
            RealseTheObject(obj);
        }
        #endregion
        #region 物件產生方法的應用：定點、指定、隨機及帶有子物件生成。
        /// <summary>
        /// 簡易產生物件方法。
        /// </summary>
        private void Random_gen(bool isRot) => Generator_EMP(new Vector3(0, mgc.GetStagePOS().y, 0), isRot);
        private void Static_gen(bool isRot, bool isRnd) => Generator_EMP(new Vector3(0, mgc.GetStagePOS().y, 0), isRot, isRnd);
        private void Static_gen(Vector3 setPos, bool isRotate) => Generator_EMP(Vector3.up * mgc.GetStageBorder().y, isRotate, false);

        #endregion

        private void Awake()
        {
            mgc = FindObjectOfType<ManageCenter>();
            gener_list = new();
            if (GenerClass.RootObject != generData.grtClass) obGenerate = new(gameObject, generData.grtObject, generData.grtLimit);
            else obGenerate = new(gameObject, generData.grtObject, Vector3.zero, Quaternion.identity, generData.grtLimit);
        }
        private void Start()
        {
            preLoadInvoke = IsInvoking();
            if (mgc.GetLevel() > 0) generDestan.x += mgc.stInfo[mgc.GetLevel() - 1].finishDistane;
            generDestan.y = generDestan.y != 0 ? Mathf.Clamp(generDestan.y, generDestan.x, generDestan.y) : 0;
            if (mgc) StartCoroutine(IntervalGenerate());
            // 資料輸入
            obGenerate.destoryTime = generData.grtdestTime;

        }
        private void Update()
        {
            //AutoGenerate();
            timeCounter += Time.deltaTime;
        }
        /// <summary>
        /// 切換生成內容系統
        /// 會根據類別決定產生的方法
        /// </summary>
        private void SwitchState()
        {
            if (timeCounter < generData.grtWaitTime) return;
            else timeCounter = 0;
            //print($"物件池數量監視器\n<color=green>啟用</color>:{activeCount}<color=red>未啟用</color>:{inActiveCount}<color=yellow>總數:{CountAll}</color>");
            if (obGenerate.countAct < generData.grtLimit)
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
                    case GenerClass.RootObject:
                        Root_StaticPoint();
                        break;
                    case GenerClass.PrevRocord:
                        // 設定前一次的失敗的位置
                        generData.grtPos = StaticSharp._RECORDPOS;
                        if (generData.grtPos == Vector3.zero) break;
                        print($"Borken rocket will be generate at {generData.grtPos}.");
                        PreviousRecordPostition();
                        break;
                    default:
                        break;
                }
            }
            else if (obGenerate.countAll > generData.grtLimit || obGenerate.countAct < 0) TakeMethod(generData.grtObject, null);
            //else if (inActiveCount > 0) obGenerate.GenerateOut();
            if (ManageCenter.UI_moveDistane > generDestan.y && generDestan.y != 0)
            {// 超過指定距離停止產生
                print($"結束產生{gameObject.name}");
                CancelInvoke("SwitchState");
                Destroy(gameObject, 30); //設定銷毀是避免物件留在場上
            }
            //print("呼叫次數");
        }

        private IEnumerator IntervalGenerate()
        {
            if (generData.grtClass == GenerClass.PrevRocord) generDestan.x = StaticSharp._SCORE;
            while (ManageCenter.UI_moveDistane < generDestan.x && !preLoadInvoke) yield return null;// 距離指定
            // 開始呼叫
            if (continues) InvokeRepeating("SwitchState", 0, generData.grtIntervalTime);            // 持續與一次性
            else Invoke("SwitchState", generData.grtIntervalTime);

        }
    }

}

