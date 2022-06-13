using UnityEngine;

/// <summary>
/// 飛行船移動
/// </summary>
/// 
namespace IIrl { 
    public class Space_move : MonoBehaviour
    {
    
        #region 資料：系統保存資料
        [SerializeField, Header("移動速度"), Range(0.001f, 0.01f) ]    
        private float speed = 0.005f;
        private Rigidbody2D rigid_ssp;
        //private Animator ani_ssp;
        #endregion

        #region 功能：主要程式執行的區域
        private void Awake() {
            rigid_ssp = GetComponent<Rigidbody2D>();
            //ani_ssp = GetComponent<Animator>();
            Physics2D.gravity = new Vector2(0,0);
        
        }
        void moveed(float h, float v)
        {
            float xSpeed;
            float ySpeed;
            if (h > 0)
            {
                xSpeed = 2 * speed;
            }
            else if (h < 0)
            {
                xSpeed = -2 * speed;
            }
            else { xSpeed = 0.0f; }
            if (v > 0)
            {
                ySpeed = 1.2f * speed;
            }
            else if (v < 0)
            {
                ySpeed = -1.2f * speed;
            }
            else { ySpeed = 0.0f; }
            
            rigid_ssp.velocity += new Vector2(xSpeed, ySpeed);

        }

        private void VisableWall()
        {
            GameObject line = GameObject.Find("_border");
            PolygonCollider2D line_Coll = line.GetComponent<PolygonCollider2D>();
            float bnd_xMax = line_Coll.bounds.center.x + line_Coll.bounds.extents.x - 0.5f;
            float bnd_yMax = line_Coll.bounds.center.y + line_Coll.bounds.extents.y - 0.5f;
            float bnd_xMin = line_Coll.bounds.center.x - line_Coll.bounds.extents.x + 0.5f;
            float bnd_yMin = line_Coll.bounds.center.y - line_Coll.bounds.extents.y + 0.5f;
            float bnd_xclamp = Mathf.Clamp(rigid_ssp.position.x, bnd_xMin, bnd_xMax);
            float bnd_yclamp = Mathf.Clamp(rigid_ssp.position.y, bnd_yMin, bnd_yMax);
            float fix_x = 0f;
            float fix_y = 0f;
            
            if (bnd_xclamp >= bnd_xMax) fix_x = -0.1f;
            else if (bnd_xclamp <= bnd_xMin) fix_x = 0.1f;
            else fix_x = rigid_ssp.velocity.x;
            if (bnd_yclamp >= bnd_yMax) fix_y = -0.2f;
            else if (bnd_yclamp <= bnd_yMin) fix_y = 0.2f;
            else fix_y = rigid_ssp.velocity.y;

            rigid_ssp.velocity = new Vector2(fix_x, fix_y);
            rigid_ssp.position = new Vector2(bnd_xclamp, bnd_yclamp);
        }

        #endregion

        #region 事件：程式的入口點
        private void Start()
        {
        }
        private void Update()
        {
            float horiz = Input.GetAxis("Horizontal");
            float verti = Input.GetAxis("Vertical");
            VisableWall();
            if ( horiz != 0f || verti != 0f)
            {
                moveed(horiz, verti);
            
            }
        }
        #endregion

    }
}