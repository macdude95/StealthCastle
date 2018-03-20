using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* DogController.cs
 * Created by Michael Cantrell
 */

public class DogController : MonoBehaviour, IRespawnable {

    public const int STATE_WANDER = 0, STATE_GO_TO_NOISE = 1, STATE_STAND_AND_BARK = 2;
    public ForceMode2D fMode;
    public float runSpeed = 300;
    public float wanderSpeed = 30;
    public Transform wanderNodes;
    public GameObject soundRingPrefab;
    public float ringStartScale = 0f;

    //state machine states
    private Transform noiseLocation;
    private int state;
    private GameObject[] soundRingPool;
    private int ringCount = 3;
    private int currentRing = 0;
    private DogPathFinding pathFinding;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private float speed;
    private Transform wanderTarget;
    private int barkingFrames = 0;
    private const string sourceName = "Dog";

    //Respawnable
    private Vector3 spawnPosition;
    private bool isActiveOnSpawn;

    private void Start()
    {
        state = STATE_WANDER;
        speed = wanderSpeed;
        pathFinding = GetComponent<DogPathFinding>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        wanderTarget = wanderNodes.GetChild(0);
        pathFinding.target = wanderTarget;
        soundRingPool = new GameObject[ringCount];
        for (int i = 0; i < ringCount; i++)
        {
            soundRingPool[i] =
                Instantiate(soundRingPrefab, this.transform.position, Quaternion.identity);
            soundRingPool[i].SetActive(false);
            soundRingPool[i].GetComponent<SoundRingController>().source = sourceName;
        }

        StartCoroutine(ChooseNewWanderTarget());
        StartCoroutine(Bark());

        //Respawnable
        spawnPosition = transform.position;
        isActiveOnSpawn = gameObject.activeSelf;
    }

    IEnumerator ChooseNewWanderTarget()
    {
        wanderTarget = wanderNodes.GetChild(Random.Range(0, wanderNodes.childCount));
        yield return new WaitForSeconds(Random.Range(3,8));
        StartCoroutine(ChooseNewWanderTarget());
    }

    IEnumerator Bark() {
        if (state == STATE_STAND_AND_BARK)
        {
            audioSource.Play();
            soundRingPool[currentRing].transform.position = this.transform.position;
            soundRingPool[currentRing].transform.localScale = new Vector3(ringStartScale, ringStartScale, 0f);
            SpriteRenderer sr = soundRingPool[currentRing].GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
            soundRingPool[currentRing].SetActive(true);
            currentRing = (currentRing + 1) % ringCount;

        }
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        StartCoroutine(Bark());
    }

    // Update is called once per frame
    void Update () {
        if (pathFinding.pathIsEnded && state == STATE_GO_TO_NOISE) {
            state = STATE_STAND_AND_BARK;
        }
        switch (state) {
            case STATE_WANDER:
                pathFinding.target = wanderTarget;
                speed = wanderSpeed;
                break;
            case STATE_GO_TO_NOISE:
                pathFinding.target = noiseLocation;
                speed = runSpeed;
                break;
            case STATE_STAND_AND_BARK:
                pathFinding.target = gameObject.transform;
                barkingFrames++;
                speed = 0;
                break;
            default:
                break;
        }
        if(barkingFrames > 200) {
            state = STATE_WANDER;
        }

        SetDir();
	}

    private void FixedUpdate()
    {
        Vector2 dir = pathFinding.ShortTermDirectionToTarget();
        dir *= speed * Time.fixedDeltaTime;

        rb.AddForce(dir, ForceMode2D.Impulse);
    }

    private void SetDir()
    {
        float horizontal = rb.velocity.x, vertical = rb.velocity.y;
        if (rb.velocity.magnitude < 3)
        {
            animator.SetBool("IS_MOVING", false);
            return;
        }
        if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical))
        {
            if (horizontal > 0)
            {
                animator.SetInteger("DIR", 1);//right
            }
            else
            {
                animator.SetInteger("DIR", 3);//left
            }
        }
        else
        {
            if (vertical > 0)
            {
                animator.SetInteger("DIR", 0);//up
            }
            else
            {
                animator.SetInteger("DIR", 2);//down
            }
        }
        animator.SetBool("IS_MOVING", true);
    }

    //called when a player is in direct LOS
    public void PlayerInVision(GameObject player)
    {
        state = STATE_STAND_AND_BARK;
        barkingFrames = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "SoundRing" && state == STATE_WANDER && other.GetComponent<SoundRingController>().source != sourceName)
        {
            //the guard just heard the player
            state = STATE_GO_TO_NOISE;
            noiseLocation = other.gameObject.transform;
            barkingFrames = 0;
        }
    }

    /* Respawn
    * Created by Michael Cantrell
    * Resets this class's attributes to their original states
    */
    public void Respawn() {
        state = STATE_WANDER;
        speed = wanderSpeed;
        pathFinding.target = wanderTarget;
        rb.velocity = Vector2.zero;
        transform.position = spawnPosition;
        gameObject.SetActive(isActiveOnSpawn);
        audioSource.Stop();
    }

}
