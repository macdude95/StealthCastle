using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* IntroSceneController.cs
 * Created by Michael Cantrell
 */

public class IntroSceneController : MonoBehaviour {

    public Animator goodKingAnimator;
    public GameObject finishTrigger;
    public Text dialogueText;
    public static IntroSceneController instance;
    public GameObject lockCharacterObject;

    private int dialogueIndex = 0;
    private static readonly string[] dialogue = { 
        "Good King: I'm about to go and meet with the bad king.", 
        "",
        "???: *screams*", 
        "Corrects", 
        "Wrongs" };

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
        //finishTrigger.SetActive(false);
        NextDialogue();
	}
	
	void Update () {
        goodKingAnimator.ResetTrigger("NEXT");
        if (Input.GetKeyDown(KeyCode.Space)) {
            goodKingAnimator.SetTrigger("NEXT");

        }
        if (dialogueIndex > 1)
        {
            lockCharacterObject.SetActive(false);
        }
	}

    public void NextDialogue() {
        if (dialogueIndex >= dialogue.Length) {
            dialogueText.text = "";
        } else if (dialogue[dialogueIndex].Equals("")) {
            dialogueText.text = "";
            dialogueIndex++;
        } else {
            dialogueText.text = dialogue[dialogueIndex++] + "\n(Press Space To Advance)";
        }
    }
}
