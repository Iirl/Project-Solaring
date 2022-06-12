using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space_move : MonoBehaviour
{
    Rigidbody2D rigid_ssp = null;
    Animator ani_ssp = null;
    const float speed = 0.005f;
    // Start is called before the first frame update
    void Start()
    {
        rigid_ssp = GetComponent<Rigidbody2D>();
        ani_ssp = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horiz = Input.GetAxis("Horizontal");
        float verti = Input.GetAxis("Vertical");
        if ( horiz != 0f || verti != 0f)
        {
            moveed(horiz, verti);
            
        } else { floated(); }
    }

    void floated()
    {
        rigid_ssp.velocity += new Vector2(0.0f, 0.00015f) ;
        
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
}
