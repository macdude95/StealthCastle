using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneController : MonoBehaviour {

    public Animator goodKingAnimator;

	void Start () {
		
	}
	
	void Update () {
        goodKingAnimator.ResetTrigger("NEXT");
        if (Input.GetKeyDown(KeyCode.Space)) {
            goodKingAnimator.SetTrigger("NEXT");
        }
	}
}
