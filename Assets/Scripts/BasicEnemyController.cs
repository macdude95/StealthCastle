using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BasicEnemyController : MonoBehaviour {

    //state machine states
    public static readonly int STATE_PATHING = 0, STATE_ALERT = 1, STATE_HUNTING = 2;


    //pathfinding controller
    public GameObject pathNode;
    private AIPath pathController;
    private Seeker seeker;


    public void Start()
    {
        pathController = this.GetComponent<AIPath>();
        InitPath();
    }

    public void FixedUpdate()
    {
        if(!pathController.pathPending && pathController.reachedEndOfPath)
        {
            UpdateDestination();
        }
    }

    //update destination based on current state
    private void UpdateDestination()
    {
        pathNode = pathNode.GetComponent<PathNodeController>().getNextNode();
        pathController.destination = pathNode.transform.position;
        pathController.SearchPath();
    }

    private void InitPath()
    {
        //move to self, to kick off pathfinding
        pathController.destination = pathNode.transform.position;
        pathController.SearchPath();
    }

}
