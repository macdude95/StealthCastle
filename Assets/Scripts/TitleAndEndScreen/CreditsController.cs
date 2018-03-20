using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour {

	void Start() {
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (0).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (1).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (2).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (3).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (4).gameObject.SetActive (false);
		GameController.instance.transform.GetChild (0).gameObject.transform.GetChild (5).gameObject.SetActive (false);
	}

	public void ChangeScene() {
		GameController.instance.LoadNewLevel ("TitleScreen");
	}
}
