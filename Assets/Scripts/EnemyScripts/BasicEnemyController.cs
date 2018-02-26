using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;

public class BasicEnemyController : MonoBehaviour {

    //state machine states
    public static readonly int STATE_PATHING = 0, STATE_ALERT = 1, STATE_HUNTING = 2;
    public float maxStuckTime;

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
    private AudioSource a_found;

    void Start() {
        visionCone = transform.GetChild(0).gameObject;
        animationController = this.GetComponent<Animator>();
        pathController = this.GetComponent<AIPath>();

		UpdateDestination(nextNode.transform.position);
        state = BasicEnemyController.STATE_PATHING;
        baseSpeed = pathController.maxSpeed;
        a_found = GetComponent<AudioSource>();
    }

    void Update() {
        if (!pathController.pathPending && pathController.reachedEndOfPath) {
			//Debug.Log("Path Reached: " + pathController.reachedEndOfPath);
            ArrivedAtDestination();
		}
        checkStuck();
		visionCone.SendMessage("RotateVision", pathController.steeringTarget);
        SetDir();
    }

    private void checkStuck()
    {
        if(pathController.velocity.magnitude < .4)
        {
            stuckTimer++;
            if(stuckTimer > maxStuckTime)
            {
                ArrivedAtDestination();
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0;
        }
    }

	//called when a player is in direct LOS
	public void PlayerInVision(GameObject player, PlayerController controller) {
        if (controller.UsingBox())
            return;
		state = BasicEnemyController.STATE_HUNTING;
		pathController.maxSpeed = baseSpeed * huntingSpeedMult;
		pathController.slowdownDistance = 0;

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
			pathController.slowdownDistance = 64;
            UpdateDestination(nextNode.transform.position);
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && state == STATE_HUNTING)
        {
            a_found.Play();
            animationController.SetBool("IS_ATTACKING", true);
            collision.gameObject.GetComponent<PlayerController>().KillPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "SoundRing" &&
			state == BasicEnemyController.STATE_PATHING) { 
			//the guard just heard the player
            state = BasicEnemyController.STATE_ALERT;
			pathController.maxSpeed = baseSpeed * huntingSpeedMult;
			UpdateDestination(other.transform.position);
		}

        if (other.CompareTag("Player") && 
            (state == STATE_HUNTING || !((PlayerController)other.gameObject.GetComponent<PlayerController>()).UsingBox())) {
			a_found.Play();
			animationController.SetBool("IS_ATTACKING", true);
		}
	}
}
