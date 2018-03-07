using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* IntroSceneController.cs
 * Created by Michael Cantrell
 */

public class IntroSceneController : MonoBehaviour {

    public Animator goodKingAnimator;
    public Text dialogueText;
    public static IntroSceneController instance;
    public GameObject lockCharacterObject;
    public Animator evilKingAnimator;
    public GameObject evilGuard1;
    public GameObject evilGuard2;
    public Collider2D PlayerCollider;

    private int dialogueIndex = 0;
    private static readonly string pressSpaceToAdvance = "\n(Press Space To Advance)";
    private static readonly string goodKingName = "King Balthazar";
    private static readonly string[] dialogue = { 
        goodKingName + ": Hark!", 
        goodKingName + ": I venture off to meetheth\nwith the evil king to facilitateth peace!" + pressSpaceToAdvance, 
        goodKingName + ": We both know how\nthis encounter might endeth up." + pressSpaceToAdvance, 
        goodKingName + ": Remember thy mission\nif something goeth wrong." + pressSpaceToAdvance, 
        goodKingName + ": Thee wilt doeth whatever it takes\nto protecteth our beloved kingdom." + pressSpaceToAdvance, 
        goodKingName + ": Taketh the path to thy left in order to stayeth safe." + pressSpaceToAdvance, 
        "",
        "???: *screams*"
    };

    private BoxCollider2D moveEnemiesCollider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

	void Start () {
        // don't let the player leave until the cutscene is finished
        NextDialogue();
        moveEnemiesCollider = GetComponent<BoxCollider2D>();
	}
	
	void Update () {
        goodKingAnimator.ResetTrigger("NEXT");
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (dialogueIndex == 6)
            {
                goodKingAnimator.SetTrigger("NEXT");
                lockCharacterObject.SetActive(false);
            }
            else if (dialogueIndex < 6)
            {
                NextDialogue();
            }

        }
        if (moveEnemiesCollider.IsTouching(PlayerCollider)) {
            EnemiesLeave();
        }
	}

    public void NextDialogue() {
        if (dialogueIndex >= dialogue.Length) {
            dialogueText.text = "";
        } else if (dialogue[dialogueIndex].Equals("")) {
            dialogueText.text = "";
            dialogueIndex++;
        } else {
            dialogueText.text = dialogue[dialogueIndex++];
        }
    }

    private void EnemiesLeave() {
        dialogueText.text = "";
        evilKingAnimator.SetTrigger("MOVE");
        evilGuard1.GetComponent<Animator>().SetTrigger("MOVE");
        evilGuard2.GetComponent<Animator>().SetTrigger("MOVE");
        evilGuard1.GetComponent<Rigidbody2D>().velocity = new Vector2(0,-100);
        evilGuard2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -100);
    }
}
