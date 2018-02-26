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
	private Animator animControl;
    private AudioSource audioSource;

	//used when guard attacks player while player is disguised
	private DisguiseScript disguiseScript;
	private string currentDisguise = null;

	public AudioClip loudStep;
	public GameObject soundRingPrefab;
    private GameObject[] soundRingPool;
    private int ringCount = 6;
    private int currentRing = 0;

    public AudioSource a_door;
    public GameObject doorOpenText;

    private Sprite savePlayerSprite;
    private bool usingBox = false;
    private bool isSprinting = false;
    private bool isSlowed = false;

    private GameObject interactable = null;
    private bool canPickUp = true;

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
        animControl = GetComponent<Animator>();
        savePlayerSprite = GetComponent<SpriteRenderer>().sprite;
		disguiseScript = GetComponent<DisguiseScript>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
		if (!isDead) {
            CheckUsedBoxDisguise();
            CheckInputs();
            SetDir();
			SetSpeed();
		}
    }

	void FixedUpdate() {
		if (!isDead) {
			rb.velocity =
				new Vector2(Input.GetAxis("Horizontal") * speed,
							Input.GetAxis("Vertical") * speed);
		}
	}

    //handle any inputs here
    void CheckInputs()
    {
        if (animControl.GetBool("IS_CHANGING"))
            return;

        if(Input.GetButtonDown("Interaction"))
        {
            //something to pick up
            if(interactable != null)
            {
                PickUpGadget(interactable);
            }
            //something to drop
            else if(GameController.instance.currItem != null)
            {
                DropOldGadget(GameController.instance.currItem);
            }
        }
        else if(Input.GetButtonDown("UseItem"))
        {
            
        }
    }

	public void ResetScene() {
		GameController.instance.ResetScene();
	}

    public void KillPlayer() {
        isDead = true;
        if (usingBox) {
            GetComponent<SpriteRenderer>().sprite = savePlayerSprite;
            this.GetComponent<Animator>().enabled = true;
        }
        disguiseScript.SetAnimControlToOrig();
        animControl.SetBool("IS_DEAD", true);

        rb.velocity = Vector2.zero;
        GetComponent<BoxCollider2D>().enabled = false;
    }

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("BasicTrap")) {
			(collision.gameObject.transform.GetChild(0)).SendMessage("ActivateTrap");
		}
		else if (collision.gameObject.CompareTag("SpiderWeb")) {
			if (!GameController.instance.GetItemName().Equals("WebCutter")) {
				isSlowed = true;
			}
		}
		else if (collision.gameObject.CompareTag("Enemy") && !usingBox) {
			KillPlayer();
		}
		else if (collision.gameObject.CompareTag("Finish")) {
			SceneManager.LoadScene("Playtest01");
		}
		else if (collision.gameObject.CompareTag("Gem")) {
			GameController.instance.score++;
			GameController.instance.DisplayScore();
		}
        else if (collision.gameObject.CompareTag("Gadget"))
        {
            if(interactable != null)
            {
                interactable.GetComponent<PickUpController>().pickupReady(false);
                interactable = null;
            }

            collision.gameObject.GetComponent<PickUpController>().pickupReady(true);
            interactable = collision.gameObject;
        }
    }

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("VisionDetector") &&
			CanBeSeen(collision.gameObject.GetComponent<VisionConeController>().seenDisguiseType)) {

			//if disguised see if can see through
			collision.gameObject.SendMessage("CheckVision", this.gameObject);
		}
        if(collision.gameObject.CompareTag("Gadget") && interactable == null)
        {
            collision.gameObject.GetComponent<PickUpController>().pickupReady(true);
            interactable = collision.gameObject;
        }
    }

	private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "SpiderWeb") {
            isSlowed = false;
        }
        else if (collision.gameObject.CompareTag("Gadget"))
        {
            collision.gameObject.GetComponent<PickUpController>().pickupReady(false);
            interactable = null;
        }
    }

	private void DropOldGadget(GameObject oldItem) {
		PickUpController oldItemController =
				oldItem.GetComponent<PickUpController>();

		oldItemController.DropItem(this.transform.position);
		if (oldItemController.itemIsDisguise) {
			/*
			 * Assuming that the player is disguised, setting the
			 * 'IS_CHANGING' boolean to true will animate the player changing
			 * back into normal. This will activate an event trigger in the
			 * respective animation that will restore the player's original
			 * animation controller.
			 */
			animControl.SetBool("IS_CHANGING", true);
		}
        oldItem.SetActive(true);
        GameController.instance.SetPlayerItem(null);
	}

	private void PickUpGadget(GameObject newGadget) {
		GameObject oldGadget = GameController.instance.currItem;
		if (oldGadget != null) {
			DropOldGadget(oldGadget);
		}

		GameObject newItem = newGadget;
		PickUpController newItemController =
			newItem.GetComponent<PickUpController>();

		if (newItemController.itemIsDisguise) {
			currentDisguise =
				newItem.GetComponent<DisguiseInfoContainer>().disguiseName;
            this.GetComponent<DisguiseScript>().DonDisguise(newGadget);
        }
        GameController.instance.SetPlayerItem(newGadget);
        newGadget.SetActive(false);
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

    public bool UsingBox() {
        return usingBox;
    }

    //sets animation direction
    private void SetDir() {
        float horizontal = rb.velocity.x, vertical = rb.velocity.y;
        if (horizontal == 0 && vertical == 0) {
            animControl.SetBool("IS_MOVING", false);
			framesSinceLastRing = 0;
            return;
        }
        if (horizontal >= vertical) {
            if (horizontal > 0) {
                animControl.SetInteger("DIR", 1); //right
            }
            else {
                animControl.SetInteger("DIR", 2); //left
            }
        }
        else {
            if (vertical > 0) {
                animControl.SetInteger("DIR", 0); //up
            }
            else {
                animControl.SetInteger("DIR", 3); //down
            }
        }
		animControl.SetBool("IS_MOVING", true);
		if (speed == runSpeed &&
			!GameController.instance.GetItemName().Equals("SilentBoots")) {
			SoundRings();
		}
    }

    private bool CanBeSeen(string disguiseType) {
        if (usingBox) return false;
        return ((currentDisguise != null) ? currentDisguise.Equals(disguiseType) : true);
    }

    private void CheckUsedBoxDisguise() {
        if (GameController.instance.GetItemName() == "BoxDisguise" && Input.GetButton("Interaction")) {
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
