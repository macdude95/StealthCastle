using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BasicEnemyController : MonoBehaviour {

    //state machine states
    public static readonly int STATE_PATHING = 0, STATE_ALERT = 1, STATE_HUNTING = 2;


    //pathfinding controller
    public GameObject nextNode;

    private GameObject lastNode;
    private AIPath pathController;
    private Seeker seeker;

    private int state;

    public void Start()
    {
        pathController = this.GetComponent<AIPath>();
        //move to self, to kick off pathfinding
        pathController.destination = nextNode.transform.position;
        pathController.SearchPath();
        state = BasicEnemyController.STATE_PATHING;
    }

    public void FixedUpdate()
    {
        if(!pathController.pathPending && pathController.reachedEndOfPath)
        {
            ArrivedAtDestination();
        }
    }

    private void ArrivedAtDestination()
    {
        lastNode = nextNode;
        if (state == BasicEnemyController.STATE_PATHING)
        {
            nextNode = lastNode.GetComponent<PathNodeController>().getNextNode();
            UpdateDestination(nextNode.transform.position);
        }
        else if (state == BasicEnemyController.STATE_HUNTING)
        {
            state = STATE_PATHING;
            pathController.maxSpeed = 2;
            UpdateDestination(lastNode.transform.position);
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
        pathController.maxSpeed = 4;
        UpdateDestination(player.transform.position);
    }
}
