using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneController : MonoBehaviour {

    public Animator kingAnimator;
    public Collider2D inRangeTrigger;
    public Collider2D playerBoxCollider;
    public Text pressSpaceToKill;
    public string endCreditsSceneName;
    private bool gameEnded = false;

	private void Awake()
	{
        pressSpaceToKill.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
        if (playerBoxCollider.IsTouching(inRangeTrigger) && !gameEnded){
            pressSpaceToKill.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space)) 
            {

                gameEnded = true;
                StartCoroutine(endGame());
            }
        } else {
            pressSpaceToKill.gameObject.SetActive(false);
        }
	}

    IEnumerator endGame() {
        kingAnimator.SetTrigger("DIE");
        yield return new WaitForSeconds(2);
        GameController.instance.LoadNewLevel(endCreditsSceneName);
    }
}
