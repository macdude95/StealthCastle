using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float runSpeed = 10;
	public float walkSpeed = 4;	
	public GameObject particlePrefab;
	public int numberOfParticles = 20;
	public int numberOfFramesUntilNextSoundWave = 10;

    private float speed = 4;
	private int numberOfFramesSinceLastParticle = 0;

    private Animator animationController;
    private Rigidbody2D rb;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<Animator>();
    }

    private void Update()
	{
		SetDir();
		RunOrWalk ();
    }

	private void SoundParticles() {
		GameObject particle;
		Rigidbody2D rb;
		Vector2 dirToFly;

		if (this.numberOfFramesSinceLastParticle < this.numberOfFramesUntilNextSoundWave) {
			this.numberOfFramesSinceLastParticle++;
			return;
		}
		this.numberOfFramesSinceLastParticle = 0;

		for (int i = 0; i < numberOfParticles; i++) {
			float angle = i * Mathf.PI * 2 / numberOfParticles;
			Vector2 pos = new Vector2 (Mathf.Cos (angle) + transform.position.x, Mathf.Sin (angle) + transform.position.y) * 1f;
			particle = Instantiate (particlePrefab, pos, Quaternion.identity);
			dirToFly = particle.transform.position - this.transform.position;
			rb = particle.GetComponent<Rigidbody2D> ();
			rb.velocity = dirToFly;
		}
	}

	private void RunOrWalk() {
		if (Input.GetButton ("Run")) {
			speed = runSpeed;
			SoundParticles ();
		} else {
			speed = walkSpeed;
		}
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
