﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Mostly?] Written by Daniel Anderson
/// </summary>
public class PlayerController : MonoBehaviour, IRespawnable {

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

	public GameObject soundRingPrefab;
    private GameObject[] soundRingPool;
    private int ringCount = 6;
    private int currentRing = 0;

    private Sprite savePlayerSprite;
    private bool usingBox = false;
    private bool isSprinting = false;
    private bool isSlowed = false;

    private GameObject interactable = null;
    private bool canPickUp = true;

    //Respawnable
    private Vector3 spawnPosition;
    private bool isActiveOnSpawn;
	public GameObject slowIndicator;

	//initial items carried over from previous level
	private GameObject startItem;
	private PickUpController startPickUpController;

    //sounds
    public AudioClip loudStep, throwObject, useBox, releaseBox, webCut, itemPickup, webEnter, gemPickup, deathSound;

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

        //Respawnable
        spawnPosition = transform.position;
        isActiveOnSpawn = gameObject.activeSelf;
		slowIndicator.SetActive(false);

		startItem = GameController.instance.currItem;
		if (startItem != null) {
			startPickUpController =
				startItem.GetComponent<PickUpController>();
			if (startPickUpController.itemIsDisguise) {
				DisguiseInfoContainer disguiseInfo =
					startItem.GetComponent<DisguiseInfoContainer>();
				currentDisguise = disguiseInfo.disguiseName;
				disguiseScript.DonDisguise(startItem);
			}
		}
		else {
			startPickUpController = null;
		}
    }

    void Update() {
		if (!isDead) {
            CheckInputs();
            SetDir();
			SetSpeed();
		}
        /*Haungs cheat button code*/
		if (Input.GetButtonDown ("HaungsCheat")) {
            GameObject finishTriggerObject = GameObject.Find("FinishTrigger");
            if (finishTriggerObject != null) {
                GameController.instance.LoadNewLevel(finishTriggerObject.GetComponent<FinishLevel>().SceneName);
            }

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
    void CheckInputs() {
        if (animControl.GetBool("IS_CHANGING"))
            return;

		if (Input.GetButtonDown ("Interaction") && (usingBox == false)) {
			//something to pick up
			if (interactable != null) {
				PickUpGadget (interactable);
			}
            //something to drop
            else if (GameController.instance.currItem != null) {
				DropOldGadget (GameController.instance.currItem);
			}
		} else if (Input.GetButtonDown ("UseItem")) {
			UseBoxDisguise();
		} else if (Input.GetButtonUp ("UseItem")) {
			StopBoxDisguise();
		}
    }

    public void DidFinishDying() {
        GameController.instance.PlayerDied();
	}

    public void KillPlayer() {
        isDead = true;
        if (usingBox) {
            GetComponent<SpriteRenderer>().sprite = savePlayerSprite;
            animControl.enabled = true;
        }
        disguiseScript.SetAnimControlToOrig();
        animControl.SetBool("IS_DEAD", true);

        rb.velocity = Vector2.zero;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().sortingLayerName = "Environment";
        audioSource.PlayOneShot(deathSound);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Arrow")) {
            KillPlayer();
            collision.gameObject.GetComponent<ArrowController>().HitTarget();
        }
    }

	/*
	 * Created by Mitchell Keller & 
	 * Checks to see if player has collided with any outside triggers
	 */

    private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("BasicTrap")) {
			(collision.gameObject.transform.GetChild(0)).SendMessage("ActivateTrap");
		}
		else if (collision.gameObject.CompareTag("SpiderWeb")) {
			if (!GameController.instance.GetItemName().Equals("WebCutter")) {
                if (!isSlowed)
                    audioSource.PlayOneShot(webEnter);
                isSlowed = true;
				slowIndicator.SetActive(true);
			}
            else {
                audioSource.PlayOneShot(webCut);
            }
		}
		else if (collision.gameObject.CompareTag("Enemy") && !usingBox &&
			(collision.gameObject.GetComponent<BasicEnemyController>() != null)) {
			KillPlayer();
		}
		else if (collision.gameObject.CompareTag("Gem")) {
			ScoreScript.instance.score += 100;
            audioSource.PlayOneShot(gemPickup);
        }
		else if (collision.gameObject.CompareTag("Gem2")) {
			ScoreScript.instance.score += 500;
            audioSource.PlayOneShot(gemPickup);
        }
		else if (collision.gameObject.CompareTag("Gem3")) {
			ScoreScript.instance.score += 1000;
            audioSource.PlayOneShot(gemPickup);
        }
		else if (collision.gameObject.CompareTag("Gem4")) {
			ScoreScript.instance.score += 2500;
            audioSource.PlayOneShot(gemPickup);
        }
		else if (collision.gameObject.CompareTag("Gem5")) {
			ScoreScript.instance.score += 5000;
            audioSource.PlayOneShot(gemPickup);
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
		if (collision.gameObject.CompareTag("VisionDetector")) {

			//if disguised see if can see through
			collision.gameObject.GetComponent<VisionConeController>().CheckVision(this.gameObject,currentDisguise);
		}
        if(collision.gameObject.CompareTag("Gadget") && interactable == null)
        {
            collision.gameObject.GetComponent<PickUpController>().pickupReady(true);
            interactable = collision.gameObject;
        }
    }

	/* 
	 * Created by Mitchell Keller
	 * Checks to see if the player has collided with a 
	 * gadget of spiderweb
	 */

	private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "SpiderWeb") {
            isSlowed = false;
			slowIndicator.SetActive(false);
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
            currentDisguise = null;
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
        audioSource.PlayOneShot(gemPickup);
    }

    /* SoundRings
    * Created by Michael Cantrell
    * Every "framesBetweenRings", this method creates a new SoundRing
    * and plays the associated sound
    */
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

	/* SetSpeed
	 * Created by Mitchell Keller & 
	 * Checks and adjusts speed if player is 
	 * sprinting or slowed
	 */

    //do not touch this unless you're adding a brand new interaction that HAS to behave differently
	private void SetSpeed() {
        isSprinting = (Input.GetAxis("Run") > 0);

        //check immediately for immobilization
        if (usingBox) {
			speed = 0;
			return;
		}
        if (isSprinting) {
            if (isSlowed)
                speed = slowRun;
            else
                speed = runSpeed;
        }
        else {
            if (isSlowed)
                speed = slowWalk;
            else
                speed = walkSpeed;
        }
	}

    public bool UsingBox() {
        return usingBox;
    }

    //sets animation controls
    private void SetDir() {
        float horizontal = rb.velocity.x, vertical = rb.velocity.y;
        if (horizontal == 0 && vertical == 0) {
            animControl.SetBool("IS_MOVING", false);
			framesSinceLastRing = 0;
            return;
        }
        if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical)) {
            if (horizontal > 0) {
                animControl.SetInteger("DIR", 1); //right
            }
            else {
                animControl.SetInteger("DIR", 3); //left
            }
        }
        else {
            if (vertical > 0) {
                animControl.SetInteger("DIR", 0); //up
            }
            else {
                animControl.SetInteger("DIR", 2); //down
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

    /*Created by: Alex Hua
     * Purpose of function is to use the box disguise 
     * when the player presses the key with the item.
     * Function handles the changing of sprites*/
    private void UseBoxDisguise() {
		if (GameController.instance.GetItemName () == "BoxDisguise") {
			usingBox = true;
			this.GetComponent<Animator> ().enabled = false;
			this.GetComponent<SpriteRenderer> ().sprite = GameController.instance.currItem.GetComponent<SpriteRenderer> ().sprite;
            audioSource.Stop();
            audioSource.PlayOneShot(useBox);
        }
	}
    /*Created by: Alex Hua
     * Purpose of function is to stop using the box disguise
     * when the player releases the key that uses the box
     * when they have it*/
	private void StopBoxDisguise() {
		if (GameController.instance.GetItemName () == "BoxDisguise") {
            usingBox = false;
            GetComponent<SpriteRenderer>().sprite = savePlayerSprite;
            this.GetComponent<Animator>().enabled = true;
            audioSource.Stop();
            audioSource.PlayOneShot(releaseBox);
        }
    }

    public void PlayFootstep()
    {
        if(!isSprinting)
            audioSource.PlayOneShot(loudStep, .3f);
    }

    /* Respawn
    * Created by Michael Cantrell
    * Resets this class's attributes to their original states
    */
    public void Respawn() {
        transform.position = spawnPosition;
        gameObject.SetActive(isActiveOnSpawn);

        isDead = false;
        usingBox = false;
        isSprinting = false;
        isSlowed = false;

        interactable = null;
        canPickUp = true;

		if (startItem != null) {
			Debug.Assert(startPickUpController != null);
			if (startPickUpController.itemIsDisguise) {
				DisguiseInfoContainer disguiseInfo =
					startItem.GetComponent<DisguiseInfoContainer>();
				currentDisguise = disguiseInfo.disguiseName;
				disguiseScript.DonDisguise(startItem);
			}
		}
		else {
			currentDisguise = null;
			disguiseScript.SetAnimControlToOrig();
		}
        animControl.SetBool("IS_DEAD", false);
        audioSource.Stop();

        GetComponent<BoxCollider2D>().enabled = true;
        rb.velocity = Vector2.zero;
        GetComponent<SpriteRenderer>().sortingLayerName = "Player";

        if (interactable != null) {
            interactable.GetComponent<PickUpController>().pickupReady(false);
            interactable = null;
        }

		slowIndicator.SetActive(false);
	}
}
