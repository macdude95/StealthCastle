using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody2D rb;
    public int speed;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
      
        if (other.gameObject.tag == "VisionDetector")
        {
            other.gameObject.SendMessage("CheckVision", this.gameObject);
        }
       
    }

}
