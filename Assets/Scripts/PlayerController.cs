using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float runSpeed = 75;
	public float walkSpeed = 45;	
	public float slowWalk = 10;
	public float slowRun = 30;

	private bool isDead = false;

	public int framesBetweenRings = 20;
	public float ringStartScale = 0f;
	
    private float speed = 4;
	private int framesSinceLastRing = 0;

	private Rigidbody2D rb;
	private Animator animationController;
    private AudioSource audioSource;
    public AudioClip loudStep;

	//used when guard attacks player while player is disguised
	private EquipDisguise disguiseScript;

    public GameObject soundRingPrefab;
    private GameObject[] soundRingPool;
    private int ringCount = 6;
    private int currentRing = 0;

	private string currentDisguise = null;

    public AudioSource a_door;
    public GameObject doorOpenText;

    private Sprite savePlayerSprite;
    private bool usingBox = false;
    private bool isSprinting = false;
    private bool isSlowed = false;

    void Awake() {
        soundRingPool = new GameObject[ringCount];
        for(int i = 0; i < ringCount; i++) {
            soundRingPool[i] =
				Instantiate(soundRingPrefab, this.transform.position, Quaternion.identity);
            soundRingPool[i].SetActive(false);
        }
    }

    void Start() {
		rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<Animator>();
        savePlayerSprite = GetComponent<SpriteRenderer>().sprite;
		disguiseScript = GetComponent<EquipDisguise>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
		if (!isDead) {
            CheckUsedBoxDisguise();

            //should always be the last two calls
            SetDir();
			SetSpeed();
		}
    }

	void FixedUpdate() {
		if (!isDead) {
			rb.velocity =
				new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
		}
	}

	public void ResetScene() {
		GameController.instance.ResetScene();
	}

    public void KillPlayer()
    {
        isDead = true;
        if (usingBox)
        {
            GetComponent<SpriteRenderer>().sprite = savePlayerSprite;
            this.GetComponent<Animator>().enabled = true;
        }
        disguiseScript.SetAnimControlToOrig();
        animationController.SetBool("IS_DEAD", true);

        rb.velocity = Vector2.zero;
        GetComponent<BoxCollider2D>().enabled = false;
    }

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "BasicTrap") {
			(collision.gameObject.transform.GetChild(0)).SendMessage("ActivateTrap");
		}
		else if (collision.gameObject.tag == "SpiderWeb") {
			if (!GameController.instance.getItemName().Equals("WebCutter")) {
                isSlowed = true;
			}
		}
		else if (collision.gameObject.CompareTag("Gadget")) {
			GameObject oldItem = GameController.instance.currItem;
			if (oldItem != null) {
				oldItem.SetActive(true);
				oldItem.GetComponent<PickUpController>().DropItem(this.transform.position,
					new Vector2(this.rb.velocity.x, this.rb.velocity.y));
			}
			GameController.instance.SetPlayerItem(collision.gameObject);
		}
		else if (collision.gameObject.CompareTag("Disguise")) {
			currentDisguise =
				collision.gameObject.GetComponent<DisguiseInformationContainer>().disguiseName;
		}
		else if (collision.gameObject.CompareTag("Enemy") && !usingBox) {
            KillPlayer();
		}
		else if (collision.gameObject.CompareTag("Finish")) {
			SceneManager.LoadScene("Playtest01");
		}
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("VisionDetector") &&
			CanBeSeen(collision.gameObject.GetComponent<VisionConeController>().seenDisguiseType)) {

			//if disguised see if can see through
			collision.gameObject.SendMessage("CheckVision", this.gameObject);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject.tag == "SpiderWeb") {
            isSlowed = false;
        }
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
		soundRingPool[currentRing].transform.localScale = new Vector3(ringStartScale, ringStartScale, 0f);
        SpriteRenderer sr = soundRingPool[currentRing].GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
        soundRingPool[currentRing].SetActive(true);
        currentRing++;
        audioSource.PlayOneShot(loudStep);
	}

    //do not touch this unless you're adding a brand new interaction that HAS to behave differently
	private void SetSpeed() {
        isSprinting = (Input.GetAxis("Run") > 0);

        //check immediately for immobilization
        if (usingBox)
            { speed = 0; return; }
        if(isSprinting)
        {
            if (isSlowed)
                speed = slowRun;
            else
                speed = runSpeed;
        }
        else
        {
            if (isSlowed)
                speed = slowWalk;
            else
                speed = walkSpeed;
        }
	}

    public bool UsingBox()
    {
        return usingBox;
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
		if (speed == runSpeed && !GameController.instance.getItemName().Equals("HastyBoots")) {
			SoundRings();
		}
    }

   /* void DoorPlayerOpen() {
        if (rb.IsTouching(GameObject.FindGameObjectWithTag("Door").GetComponent<BoxCollider2D>())) {
            doorOpenText.SetActive(true);
            if (Input.GetKeyDown("space")) {
                a_door.Play();
                LeverAnimation.instanceLever.ChangeLeverAnimation();
                DoorAnimation.instanceDoor.ChangeDoorStatus();
            }
        }
        else {
            doorOpenText.SetActive(false);
        }
    }*/

    private bool CanBeSeen(string disguiseType) {
        if (usingBox) return false;
        return ((currentDisguise != null) ? currentDisguise.Equals(disguiseType) : true);
    }

    private void CheckUsedBoxDisguise() {
        if (isDead) return;

        if (GameController.instance.getItemName() == "BoxDisguise" && Input.GetAxis("Interaction") > 0) {
            usingBox = true;
            this.GetComponent<Animator>().enabled = false;
            this.GetComponent<SpriteRenderer>().sprite = GameController.instance.currItem.GetComponent<SpriteRenderer>().sprite;
        }
		else {
            usingBox = false;
            GetComponent<SpriteRenderer>().sprite = savePlayerSprite;
            this.GetComponent<Animator>().enabled = true;
        }
    }
}
