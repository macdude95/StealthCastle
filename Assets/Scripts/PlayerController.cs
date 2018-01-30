using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 8;

    private Animator animationController;
    private Rigidbody2D rb;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<Animator>();
    }

    private void Update()
    {
        
    }



    private void SetDir(){
        float horizontal = rb.velocity.x, vertical = rb.velocity.y;
        if (horizontal == 0 && vertical == 0)
        {
            animationController.SetBool("IS_MOVING", false);
            return;
        }
        if (horizontal >= vertical)
        {
            if(horizontal > 0)
            {
                animationController.SetInteger("DIR", 1);//right
            }
            else
            {
                animationController.SetInteger("DIR", 2);//left
            }
        }
        else
        {
            if (vertical > 0)
            {
                animationController.SetInteger("DIR", 0);//up
            }
            else
            {
                animationController.SetInteger("DIR", 3);//down
            }
        }
        animationController.SetBool("IS_MOVING", true);
    }


	void FixedUpdate()
	{
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
        SetDir();
    }

	private void OnTriggerStay2D(Collider2D other)
	{

		if (other.gameObject.tag == "VisionDetector")
		{
			other.gameObject.SendMessage("CheckVision", this.gameObject);
		}


	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "BasicTrap")
		{
			(collision.gameObject.transform.GetChild(0)).SendMessage("ActivateTrap");
		}
	}

}
