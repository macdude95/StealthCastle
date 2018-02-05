using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float runSpeed = 200;
	public float walkSpeed = 100;	
	public float slowWalk = 20;
	public float slowRun = 50;
	public int framesBetweenRings = 30;
	public bool gadget01 = false;
	public bool gadget02 = false;
	
    private float speed = 4;
	private int framesSinceLastRing = 0;

    private Animator animationController;
    private Rigidbody2D rb;

    public GameObject soundRingPrefab;
    private GameObject[] soundRingPool;
    private int ringCount = 6;
    private int currentRing = 0;

	public bool isDisguised = false;

    private void Awake() {
        soundRingPool = new GameObject[ringCount];
        for(int i = 0; i < ringCount; i++) {
            soundRingPool[i] = (GameObject) Instantiate(soundRingPrefab, this.transform.position, Quaternion.identity);
            soundRingPool[i].SetActive(false);
        }
    }

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<Animator>();
    }

    private void Update() {
		SetDir();
		SetSpeed();
    }

	private void SoundRings() {
		if (framesSinceLastRing < framesBetweenRings) {
			framesSinceLastRing++;
			return;
		}
		framesSinceLastRing = 0;
        if (currentRing >= ringCount)
            currentRing = 0;

        soundRingPool[currentRing].transform.position = this.transform.position;
        soundRingPool[currentRing].transform.localScale = new Vector3(.2f, .2f, 0f);
        SpriteRenderer sr = soundRingPool[currentRing].GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
        soundRingPool[currentRing].SetActive(true);
        currentRing++;
	}

	private void SetSpeed() {
		if (Input.GetButton ("Run")) {
			speed = runSpeed;
		} else {
			speed = walkSpeed;
		}
	}
		
    //sets animation direction
    private void SetDir() {
        float horizontal = rb.velocity.x, vertical = rb.velocity.y;
        if (horizontal == 0 && vertical == 0) {
            animationController.SetBool("IS_MOVING", false);
			framesSinceLastRing = 0;
            return;
        }
        if (horizontal >= vertical) {
            if (horizontal > 0) {
                animationController.SetInteger("DIR", 1);//right
            }
            else {
                animationController.SetInteger("DIR", 2);//left
            }
        }
        else {
            if (vertical > 0) {
                animationController.SetInteger("DIR", 0);//up
            }
            else {
                animationController.SetInteger("DIR", 3);//down
            }
        }
		animationController.SetBool("IS_MOVING", true);
		if (speed == runSpeed) {
			SoundRings();
		}
    }

	void FixedUpdate() {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
    }

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "BasicTrap") {
			(collision.gameObject.transform.GetChild(0)).SendMessage("ActivateTrap");
		}
		else if (collision.gameObject.tag == "Door") {
			LeverAnimation.instanceLever.ChangeLeverAnimation();
			DoorAnimation.instanceDoor.ChangeDoorStatus();
		}
		else if (collision.gameObject.tag == "SpiderWeb") {
			if (gadget01 == true) {

			}
			else {
				walkSpeed = slowWalk;
				runSpeed = slowRun;
			}
		}
		else if (collision.gameObject.tag == "Gadget01") {
			gadget01 = true;
		}
		else if (collision.gameObject.tag == "Gadget02") {
			gadget02 = true;
		}
    }

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.gameObject.tag == "VisionDetector" && !isDisguised) {
			collision.gameObject.SendMessage("CheckVision", this.gameObject);
		}
		else if (collision.gameObject.CompareTag("Disguise") &&
				 Input.GetButtonDown("Submit")) {
			isDisguised = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject.tag == "SpiderWeb") {
			walkSpeed = 100;
			runSpeed = 200;
		}
	}
}
