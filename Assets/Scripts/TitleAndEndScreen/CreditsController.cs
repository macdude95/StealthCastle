using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Created By: Alex Hua
 * Purpose of this class is to roll the credits scene
 * in the appropriate way and remove any misc. text
 * The actual credits rolling is handled by the Animation/Animator.*/
public class CreditsController : MonoBehaviour {

	void Start() {
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (0).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (1).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (2).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (3).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (4).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (5).gameObject.SetActive (false);
	}

    /*Function to change the scene after the credits are finished*/
	public void ChangeScene() {
		GameController.instance.LoadNewLevel ("TitleScreen");
	}
}
