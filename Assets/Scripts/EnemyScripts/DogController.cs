using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour {

    public const int STATE_PATHING = 0, STATE_HEARD_PLAYER = 1, STATE_SEES_PLAYER = 2;
    public ForceMode2D fMode;
    public float runSpeed = 300;
    public float wanderSpeed = 30;
    public Transform wanderNodes;
    //public Transform player;
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

    private void Start()
    {
        state = STATE_PATHING;
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
        }

        StartCoroutine(ChooseNewWanderTarget());
        StartCoroutine(Bark());
    }

    IEnumerator ChooseNewWanderTarget()
    {
        wanderTarget = wanderNodes.GetChild(Random.Range(0, wanderNodes.childCount));
        yield return new WaitForSeconds(Random.Range(3,8));
        StartCoroutine(ChooseNewWanderTarget());
    }

    IEnumerator Bark() {
        if (state == STATE_SEES_PLAYER)
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
        switch (state) {
            case STATE_PATHING:
                pathFinding.target = wanderTarget;
                speed = wanderSpeed;
                break;
            case STATE_HEARD_PLAYER:
                pathFinding.target = noiseLocation;
                speed = runSpeed;
                break;
            case STATE_SEES_PLAYER:
                pathFinding.target = gameObject.transform;
                barkingFrames++;
                speed = 0;
                break;
            default:
                break;
        }
        if(barkingFrames > 200) {
            state = STATE_PATHING;
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
        if (horizontal >= vertical)
        {
            if (horizontal > 0)
            {
                animator.SetInteger("DIR", 1);//right
            }
            else
            {
                animator.SetInteger("DIR", 2);//left
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
                animator.SetInteger("DIR", 3);//down
            }
        }
        animator.SetBool("IS_MOVING", true);
    }

    //called when a player is in direct LOS
    public void PlayerInVision(GameObject player)
    {
        state = STATE_SEES_PLAYER;
        barkingFrames = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "SoundRing" && state == STATE_PATHING)
        {
            //the guard just heard the player
            state = STATE_HEARD_PLAYER;
            noiseLocation = other.gameObject.transform;
            barkingFrames = 0;
        }
    }
}
