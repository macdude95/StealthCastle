using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;
/// <summary>
/// [Mostly?] Written by Daniel Anderson
/// </summary>
public class BasicEnemyController : MonoBehaviour, IRespawnable {

    //state machine states
    public static readonly int STATE_PATHING = 0, STATE_ALERT = 1, STATE_HUNTING = 2;
    public float maxStuckTime;
    private bool canAttack = true;

    //pathfinding controller
    public GameObject nextNode;
    public float huntingSpeedMult;
    private GameObject visionCone;
    private AIPath pathController;

    private Animator animationController;

    private int state;
    private float baseSpeed;
    private float stuckTimer = 0;

	//collider used for attacking the player
	private CircleCollider2D attackCollider;

    //Death Audio
    private AudioSource audioSource;
    public AudioClip attentionLost, soundHeard, playerSeen;

    //Respawnable
    private Vector3 spawnPosition;
    private bool isActiveOnSpawn;
	private float initSpeed;
    private GameObject firstNode;
	public GameObject slowIndicator;

	void Start() {
        visionCone = transform.GetChild(0).gameObject;
        animationController = this.GetComponent<Animator>();
        pathController = this.GetComponent<AIPath>();

		UpdateDestination(nextNode.transform.position);
        state = BasicEnemyController.STATE_PATHING;
        baseSpeed = pathController.maxSpeed;
        audioSource = GetComponent<AudioSource>();

        //Respawnable
        spawnPosition = transform.position;
        isActiveOnSpawn = gameObject.activeSelf;
		initSpeed = baseSpeed;
		firstNode = nextNode;
		slowIndicator.SetActive(false);
	}

    void Update() {
        if (!pathController.pathPending && pathController.reachedEndOfPath) {
			//Debug.Log("Path Reached: " + pathController.reachedEndOfPath);
            ArrivedAtDestination();
		}
        CheckStuck();
		visionCone.SendMessage("RotateVision", pathController.steeringTarget);
        SetDir();
    }

	public void SlowEnemy() {
		float SLOW_MULTIPLIER = 0.5f;

		baseSpeed *= SLOW_MULTIPLIER;
		pathController.maxSpeed *= SLOW_MULTIPLIER;
		slowIndicator.SetActive(true);
	}

	//called when a player is in direct LOS
	public void PlayerInVision(GameObject player, PlayerController controller) {
        if (controller.UsingBox())
            return;

        if (state != BasicEnemyController.STATE_HUNTING)
            audioSource.PlayOneShot(playerSeen);

        BGMPlayer.instance.PlayActionMusic();
        state = BasicEnemyController.STATE_HUNTING;
		pathController.maxSpeed = baseSpeed * huntingSpeedMult;

		Vector3 playerPosition = player.transform.position;
		GraphNode nearestPlayerNode =
			AstarPath.active.GetNearest(playerPosition).node;
		playerPosition = (Vector3)nearestPlayerNode.position;

		UpdateDestination(playerPosition);
    }

	public void StopAttacking() {
		animationController.SetBool("IS_ATTACKING", false);
	}

	private void ArrivedAtDestination() {
        if (state == BasicEnemyController.STATE_PATHING) {
            nextNode = nextNode.GetComponent<PathNodeController>().getNextNode();
            UpdateDestination(nextNode.transform.position);
        }
        else if (state == BasicEnemyController.STATE_HUNTING ||
				 state == BasicEnemyController.STATE_ALERT) {
            state = STATE_PATHING;
            pathController.maxSpeed = baseSpeed;
            UpdateDestination(nextNode.transform.position);
            audioSource.PlayOneShot(attentionLost);
        }
    }

    //update destination based on current state
    private void UpdateDestination(Vector3 newDestination) {
        pathController.destination = newDestination;
        pathController.SearchPath();
    }

    private void SetDir() {
        float horizontal = pathController.velocity.x, vertical = pathController.velocity.y;
        if (horizontal == 0 && vertical == 0) {
            animationController.SetBool("IS_MOVING", false);
            return;
        }
        if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical)) {
            if (horizontal > 0) {
                animationController.SetInteger("DIR", 1);//right
            }
            else {
                animationController.SetInteger("DIR", 3);//left
            }
        }
        else {
            if (vertical > 0) {
                animationController.SetInteger("DIR", 0);//up
            }
            else {
                animationController.SetInteger("DIR", 2);//down
            }
        }
        animationController.SetBool("IS_MOVING", true);
    }

	private void CheckStuck() {
		if (pathController.velocity.magnitude < .4) {
			stuckTimer++;
			if (stuckTimer > maxStuckTime) {
				ArrivedAtDestination();
				stuckTimer = 0;
			}
		}
		else {
			stuckTimer = 0;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		GameObject entity = collision.gameObject;
		if (entity.CompareTag("Gadget")) {
			UpdateDestination(entity.transform.position);
		}
    }

    void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("SoundRing") &&
			state == BasicEnemyController.STATE_PATHING) { 

			//the guard just heard a sound
            state = BasicEnemyController.STATE_ALERT;
			pathController.maxSpeed = baseSpeed * huntingSpeedMult;
			UpdateDestination(other.transform.position);
            BGMPlayer.instance.PlayActionMusic();
            audioSource.PlayOneShot(soundHeard);
        }

        if (other.CompareTag("Player") &&
            (!(other.gameObject.GetComponent<PlayerController>()).UsingBox() ||
            state == STATE_HUNTING) && canAttack) {
            AttackPlayer(other.gameObject);
        }
	}

    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player") &&
            (!(other.gameObject.GetComponent<PlayerController>()).UsingBox() ||
            state == STATE_HUNTING) && canAttack)
        {
            AttackPlayer(other.gameObject);
        }
    }

    private void AttackPlayer(GameObject player) {
        audioSource.Play();
        animationController.SetBool("IS_ATTACKING", true);
        player.GetComponent<PlayerController>().KillPlayer();
        canAttack = false;
    }

    /* Respawn
    * Created by Michael Cantrell
    * Resets this class's attributes to their original states
    */
    public void Respawn() {
        transform.position = spawnPosition;
        gameObject.SetActive(isActiveOnSpawn);

        state = BasicEnemyController.STATE_PATHING;
		baseSpeed = initSpeed;
		pathController.maxSpeed = baseSpeed;
		slowIndicator.SetActive(false);

		StopAttacking();
        nextNode = firstNode;
        UpdateDestination(nextNode.transform.position);
        audioSource.Stop();
        canAttack = true;
    }
}
