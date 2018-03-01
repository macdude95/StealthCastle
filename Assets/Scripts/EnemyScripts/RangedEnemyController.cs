using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : MonoBehaviour, Respawnable {

    //state machine states
    public static readonly int STATE_PATHING = 0, STATE_ALERT = 1, STATE_HUNTING = 2;
    public float maxStuckTime;

    public float arrowSpeed;

    //pathfinding controller
    public GameObject nextNode;
    public float huntingSpeedMult;
    private GameObject visionCone;
    private AIPath pathController;

    private Animator animationController;

    private int state;
    private float baseSpeed;
    private float stuckTimer = 0;

    public GameObject arrowProjectile;
    private ArrowController arrowController;
    private bool arrorwReady = true;
    private Vector3 arrowTargetPosition;

    //collider used for attacking the player
    private CircleCollider2D attackCollider;

    //Death Audio
    private AudioSource a_found;

    //Respawnable
    private Vector3 spawnPosition;
    private bool isActiveOnSpawn;
    private GameObject firstNode;

    void Start()
    {
        visionCone = transform.GetChild(0).gameObject;
        animationController = this.GetComponent<Animator>();
        pathController = this.GetComponent<AIPath>();

        UpdateDestination(nextNode.transform.position);
        state = BasicEnemyController.STATE_PATHING;
        baseSpeed = pathController.maxSpeed;
        a_found = GetComponent<AudioSource>();

        arrowController = arrowProjectile.GetComponent<ArrowController>();
        arrowController.SetParent(this.GetComponent<RangedEnemyController>());

        //Respawnable
        spawnPosition = transform.position;
        isActiveOnSpawn = gameObject.activeSelf;
        firstNode = nextNode;
    }

    void Update()
    {
        if (!pathController.pathPending && pathController.reachedEndOfPath)
        {
            //Debug.Log("Path Reached: " + pathController.reachedEndOfPath);
            ArrivedAtDestination();
        }
        CheckStuck();
        visionCone.SendMessage("RotateVision", pathController.steeringTarget);
        SetDir();
    }

    public void SlowEnemy()
    {
        float SLOW_MULTIPLIER = 0.5f;

        baseSpeed *= SLOW_MULTIPLIER;
        pathController.maxSpeed *= SLOW_MULTIPLIER;
    }

    //called when a player is in direct LOS
    public void PlayerInVision(GameObject player, PlayerController controller)
    {
        if (controller.UsingBox() || animationController.GetBool("IS_ATTACKING"))
            return;

        if(arrorwReady)
        {
            StartAttacking();
            arrowTargetPosition = player.transform.position;
            FireProjectile();
        }

        if(!animationController.GetBool("IS_ATTACKING"))
        {
            state = BasicEnemyController.STATE_HUNTING;
            pathController.maxSpeed = baseSpeed * huntingSpeedMult;
            pathController.slowdownDistance = 0;

            Vector3 playerPosition = player.transform.position;
            GraphNode nearestPlayerNode =
                AstarPath.active.GetNearest(playerPosition).node;
            playerPosition = (Vector3)nearestPlayerNode.position;

            UpdateDestination(playerPosition);
        }

    }

    public void StartAttacking()
    {
        arrorwReady = false;
        animationController.SetBool("IS_ATTACKING", true);
        pathController.maxSpeed = 0;
    }

    public void ArrowHit()
    {
        arrorwReady = true;
    }

    public void FireProjectile()
    {
        arrowProjectile.SetActive(true);
        arrowProjectile.transform.position = this.transform.position;
        Vector3 dir = Vector3.Normalize(arrowTargetPosition - this.transform.position) * arrowSpeed;
        arrowProjectile.GetComponent<Rigidbody2D>().velocity = dir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowProjectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void StopAttacking()
    {
        animationController.SetBool("IS_ATTACKING", false);
        pathController.maxSpeed = baseSpeed;
    }

    private void ArrivedAtDestination()
    {
        if (state == BasicEnemyController.STATE_PATHING)
        {
            nextNode = nextNode.GetComponent<PathNodeController>().getNextNode();
            UpdateDestination(nextNode.transform.position);
        }
        else if (state == BasicEnemyController.STATE_HUNTING ||
                 state == BasicEnemyController.STATE_ALERT)
        {
            state = STATE_PATHING;
            pathController.maxSpeed = baseSpeed;
            pathController.slowdownDistance = 64;
            UpdateDestination(nextNode.transform.position);
        }
    }

    //update destination based on current state
    private void UpdateDestination(Vector3 newDestination)
    {
        pathController.destination = newDestination;
        pathController.SearchPath();
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

    private void CheckStuck()
    {
        if (pathController.velocity.magnitude < .4)
        {
            stuckTimer++;
            if (stuckTimer > maxStuckTime)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject entity = collision.gameObject;
        if (entity.CompareTag("Gadget"))
        {
            UpdateDestination(entity.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SoundRing") &&
            state == BasicEnemyController.STATE_PATHING)
        {

            //the guard just heard a sound
            state = BasicEnemyController.STATE_ALERT;
            pathController.maxSpeed = baseSpeed * huntingSpeedMult;
            UpdateDestination(other.transform.position);
        }

        if (other.CompareTag("Player") &&
            (!(other.gameObject.GetComponent<PlayerController>()).UsingBox() ||
            state == STATE_HUNTING))
        {
            a_found.Play();
            animationController.SetBool("IS_ATTACKING", true);
            other.gameObject.GetComponent<PlayerController>().KillPlayer();
        }
    }

    public void Respawn()
    {
        transform.position = spawnPosition;
        gameObject.SetActive(isActiveOnSpawn);
        state = BasicEnemyController.STATE_PATHING;
        StopAttacking();
        nextNode = firstNode;
        UpdateDestination(nextNode.transform.position);

        //projectile stuff
        arrowProjectile.SetActive(false);
        arrorwReady = true;
    }
}
