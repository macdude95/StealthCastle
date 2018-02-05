using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;

public class BasicEnemyController : MonoBehaviour {

    //state machine states
    public static readonly int STATE_PATHING = 0, STATE_ALERT = 1, STATE_HUNTING = 2;


    //pathfinding controller
    public GameObject nextNode;
    public float huntingSpeedMult;
    private GameObject visionCone;
    private GameObject lastNode;
    private AIPath pathController;

    private Animator animationController;

    private int state;
    private float baseSpeed;

    public void Start()
    {
        visionCone = transform.GetChild(0).gameObject;
        animationController = this.GetComponent<Animator>();
        pathController = this.GetComponent<AIPath>();
        //move to self, to kick off pathfinding
		UpdateDestination(nextNode.transform.position);
        state = BasicEnemyController.STATE_PATHING;
        baseSpeed = pathController.maxSpeed;
    }

    public void Update()
    {
        if(!pathController.pathPending && pathController.reachedEndOfPath)
        {
            ArrivedAtDestination();
		}
		visionCone.SendMessage("RotateVision", pathController.steeringTarget);
        SetDir();
    }

    private void ArrivedAtDestination()
    {
        lastNode = nextNode;
        if (state == BasicEnemyController.STATE_PATHING)
        {
            nextNode = lastNode.GetComponent<PathNodeController>().getNextNode();
            UpdateDestination(nextNode.transform.position);
//            visionCone.SendMessage("RotateVision", nextNode.transform.position);
        }
        else if (state == BasicEnemyController.STATE_HUNTING)
        {
            state = STATE_PATHING;
            pathController.maxSpeed = baseSpeed;
            UpdateDestination(lastNode.transform.position);
//            visionCone.SendMessage("RotateVision", lastNode.transform.position);
        }
    }

    //update destination based on current state
    private void UpdateDestination(Vector3 newDestination)
    {
        pathController.destination = newDestination;
        pathController.SearchPath();

    }

    //called when a player is in direct LOS
    public void PlayerInVision(GameObject player)
    {
        state = BasicEnemyController.STATE_HUNTING;
        pathController.maxSpeed = baseSpeed * huntingSpeedMult;
        UpdateDestination(player.transform.position);
    }


    private void SetDir()
    {
        float horizontal = pathController.velocity.x, vertical = pathController.velocity.y;
        if (horizontal == 0 && vertical == 0)
        {
            animationController.SetBool("IS_MOVING", false);
            return;
        }
        if (horizontal >= vertical)
        {
            if (horizontal > 0)
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

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player")
			SceneManager.LoadScene ("Playtest01");
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "SoundRing") { 
			//the guard just heard the player
			state = BasicEnemyController.STATE_HUNTING;
			pathController.maxSpeed = baseSpeed*huntingSpeedMult;
			UpdateDestination(other.transform.position);
		}
	}
}
