using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace solar_a
{
    public enum EventMethod { PressKey, RocketOnOff, GOBOnOff }
    public class DialogSystem : MonoBehaviour
    {
        [SerializeField, Header("語言資料")]
        private ScriptData landata;
        [SerializeField, Header("資料欄位"), NonReorderable]
        private TextMeshProUGUI TextField;
        [SerializeField, Header("等待指標")]
        private GameObject textDelta;
        [SerializeField]
        private KeyCode KeyInput;
        [SerializeField, Header("文字效果時間"), Tooltip("等待時間"), Range(0f, 5f)]
        private float textWait=0.5f;
        [SerializeField, Tooltip("打字時間"),Range(0.01f,0.2f)]
        private float textTypeEffect=0.0625f;
        [SerializeField, Header("事件處理")]
        private List<EventData> eventData;
        private List<int> evKeyIDs ;
        //
        #region 內部資料欄位
	    private CanvasGroup dialogCVG;
	    private IEnumerator ItypeEffect;
	    TextMeshProUGUI langText;
	    bool wait=true, evDown=false;
	    int line;
        [System.Serializable]
        public class EventData
        {
            public string eventName;
            public int eventKeyID;
            public EventMethod method;
            public string[] values;
            [SerializeField, Header("方法: GameObject On Off")]
            public GameObject gameObject;
        }
        #endregion
		
	    /// <summary>
	    /// 重新讀取
	    /// </summary>
	    private void ReloadDialog(){
	    	int lid = StaticSharp._LANG_ID;
	    	StartCoroutine(dialogCVG.FadeEffect(false));
	    	StopCoroutine(ItypeEffect);
	    	ItypeEffect = TypeEffect(lid, line);
	    	FontAssetChange(lid);
	    	StartCoroutine(ItypeEffect);
	    }
	    private void FontAssetChange(int langID) => TextField.font = landata.Language[langID].font; //更換字型
        /// <summary>
        /// 打字機效果
        /// </summary>
        /// <param name="lang">輸入語言編號會切換資料語言</param>
        /// <returns></returns>
        private IEnumerator TypeEffect(int lang,int setLine = 0)
        {
            StaticSharp.isDialogEvent = true;
	        StartCoroutine(dialogCVG.FadeEffect(true));
	        textDelta.SetActive(false);
            bool skip = false;
            line = 0;
            float setTime = 0;
	        wait = true;
            foreach (var e in landata.Language[lang].datas)
            {
                TextField.text = "";
                foreach (var content in e)
                {
                	if (setLine != 0) if (line < setLine) {line++; continue;}
                    if(content.Equals('<')) skip = true;
                    else if(content.Equals('>')) skip = false;
                    TextField.text += content;
                    if (!skip) yield return new WaitForSeconds(textTypeEffect);
                    else if (Input.GetKeyUp("x")) { TextField.text = e; break; }
                }
	            //print("//等待使用者回應事件");
	            textDelta.SetActive(true);
                while (wait)
                {	                
                	if(eventData.Count>0 && !evDown) yield return StartCoroutine(EventDitect(line));
	                if (textWait > 0) {
		                setTime += Time.deltaTime;
	                	if (setTime > textWait) wait = false;
	                }
	                else if (Input.GetKey(KeyInput) || Input.GetKey(KeyInput.ToString().ToLower())) wait = false; 
	                yield return null;
                }
                // 段落結束處理
                textDelta.SetActive(false);
                yield return null;
	            wait = true;
	            evDown = false;
                setTime = 0;
                line++;
            }
            //文字輸出結束
            while (!Input.anyKey) yield return null;
            TextField.text = "";
            yield return StartCoroutine(dialogCVG.FadeEffect(false));
            StaticSharp.isDialogEvent = false;
        }
        /// <summary>
        /// 偵測事件系統
        /// </summary>
        /// <param name="idx">傳入執行文字行數</param>
        /// <returns></returns>
        private IEnumerator EventDitect(int idx)
        {
            int i = evKeyIDs.FindIndex(e => e.Equals(idx));
            if (i != -1)
            {
                textDelta.SetActive(false);
                switch (eventData[i].method)
                {
                    case EventMethod.PressKey:
                        //按鍵偵測
                        yield return StartCoroutine(PressKey(eventData[i].values));
                        wait = false;
                        break;
                    case EventMethod.RocketOnOff:
                        //print("火箭控制");
                        RocketOnOff(eventData[i].values[0].ToLower().Equals("on"));
                        break;
                    case EventMethod.GOBOnOff:
                        //print("物件開關控制");
                        GOBOnOff(eventData[i].gameObject,
                            eventData[i].values[0].ToLower().Equals("on"));
                        break;
                    default:
                        break;
                }
            }
	        evDown = true;	        
	        if (!textDelta.activeSelf) 
		        textDelta.SetActive(true);
            yield return null;
        }
        /// <summary>
        /// 方法事件：按下按鈕後繼續。
        /// 在 value 中設定的數值應為一個字元的字串，超過將會中止。
        /// </summary>
        /// <param name="key">讀入字串陣列</param>
        /// <returns></returns>
        private IEnumerator PressKey(string[] key)
        {
            if (key.Length < 1 || key[0].Length>1) yield break;
            bool keep = true;
            string tips = "Press";
            foreach (string s in key) tips += $" [{s.ToUpper()}] ";
            guiText =(tips);
            while (keep)
            {
                foreach (var e in key)                
                    if (Input.GetKey(e.ToLower())) keep = false;                
                yield return null;
                guiShow = keep;
            }
            //print("成功輸入並等待");
            yield return new WaitForSeconds(textWait);
        }
        /// <summary>
        /// 方法事件：開關火箭
        /// </summary>
        /// <param name="isOn">On為真表示可以控制火箭</param>
        private void RocketOnOff(bool isOn=true) => ManageCenter.rocket_ctl.CloseTheControl = !isOn;
        private void GOBOnOff(GameObject gobj, bool isOn = true) => gobj.SetActive(isOn);

        private void Awake()
        {
            dialogCVG = GetComponent<CanvasGroup>();
            evKeyIDs = new List<int>();
	        FontAssetChange(0);
        }
        private void Start()
	    {		    
		    StaticSharp.isDialogEvent = false;
	        if (!StaticSharp.isDialogEvent) {ItypeEffect = TypeEffect(StaticSharp._LANG_ID); StartCoroutine(ItypeEffect);}
            for (int i = 0; i < eventData.Count; i++) evKeyIDs.Add(eventData[i].eventKeyID);
            guiWindows.x = Screen.width / 3.33f;
	        guiWindows.y= Screen.height / 10;
        }

        #region GUI 顯現
        //[SerializeField]
        private bool guiShow;
        private string guiText;
        //[SerializeField]
        private Rect guiWindows = new Rect(0,0,270,60);
        private void OnGUI()
        {
            if (guiText !=null) guiWindows.width = guiText.Length*7+10;
            if (guiShow) guiWindows= GUI.Window(0,guiWindows,TipFunction,"Tips");
        }        
        private void TipFunction(int ID) {
            GUI.color = Color.white;            
            GUI.Label(new Rect(20,25,guiWindows.width, guiWindows.height-25),guiText);            
        }

        #endregion
    }
}