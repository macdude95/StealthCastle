using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerIntroduction : MonoBehaviour {

	public GameObject title_panel;
	public GameObject controls_panel;

	public void SceneLoader(int SceneIndex) {
		SceneManager.LoadScene(SceneIndex);
	}
	public void ChangePanels(bool BoolState) {
		Debug.Log ("sdlgs");
		if (BoolState == true) {
			title_panel.SetActive(false);
			controls_panel.SetActive(true);
		} else {
			controls_panel.SetActive(false);
			title_panel.SetActive(true);
		}
	}
}
