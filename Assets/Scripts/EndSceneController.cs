using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneController : MonoBehaviour {

    public Animator kingAnimator;
    public Collider2D inRangeTrigger;
    public Collider2D playerBoxCollider;
    public string endCreditsSceneName;
	
	// Update is called once per frame
	void Update () {
        if (playerBoxCollider.IsTouching(inRangeTrigger) && Input.GetKeyDown(KeyCode.P)) {
            endGame();
        }
	}

    private void endGame() {
        kingAnimator.SetTrigger("DIE");
        GameController.instance.LoadNewLevel(endCreditsSceneName);
    }
}
