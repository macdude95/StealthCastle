using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerIntroduction : MonoBehaviour {

	public GameObject eventSystem;
	public GameObject title_panel;
	public GameObject controls_panel;

	void Start() {
		StartCoroutine(Fading.instance.FadeToZeroAlpha());
	}

	void FixedUpdate() {
		CheckInputs();
	}

	public void CheckInputs() {
		if (Input.GetAxis("Vertical") != 0) {
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (title_panel.transform.GetChild (0).gameObject);
		} else {
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
		}
	}

	/*Function to load next scene*/
	public void SceneLoader(string SceneName) {
		StartCoroutine(Fading.instance.FadeToFullAlpha());
		StartCoroutine(Fading.instance.DoLast(SceneName));
	}

	/*Function to change Panels to go to and from Controls Menu*/
	public void ChangePanels(bool BoolState) {
		if (BoolState == true) {
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);	//Destroy the currernt selected GameObject for eventsystem
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(controls_panel.transform.GetChild(1).gameObject);	//Set the new GameObject for eventsystem
			title_panel.SetActive(false);
			controls_panel.SetActive(true);
		} else {
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);	//Destroy the current selected GameObject for eventsystem
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(title_panel.transform.GetChild(0).gameObject);	//Set the new GameObject for eventsystem
			controls_panel.SetActive(false);
			title_panel.SetActive(true);
		}
	}
}
