using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/*Created By: Alex Hua
 * Class is to manage the title screen functionality.*/
public class GameControllerIntroduction : MonoBehaviour {

	public GameObject eventSystem;
	public GameObject title_panel;
	public GameObject controls_panel;
	public GameObject title_text;
	public GameObject gate;
	public GameObject player;
	public GameObject fade;
	public Vector3 dest_scale;  //Final scaling of the player model
	public float time;

	private Vector3 orig_scale; //Original scaling of player model
	private bool scale;

	void Start() {
		eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);	//Destroy the current selected GameObject for eventsystem
		eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(title_panel.transform.GetChild(0).gameObject);	//Set the new GameObject for eventsystem
		orig_scale = player.transform.localScale;
		title_text.SetActive (true);
		controls_panel.SetActive (false);
		title_panel.SetActive (true);
		gate.SetActive (true);
		player.SetActive (true);
	}

	/*Function to load next scene*/
	public void SceneLoader(string SceneName) {
		gate.GetComponent<Animator>().SetBool("StartGame", true);
		player.GetComponent<Animator>().SetInteger ("DIR", 0);
		player.GetComponent<Animator>().SetBool("IS_MOVING", true);
		title_panel.SetActive(false);
		StartCoroutine(ScaleOverTime());
		StartCoroutine(FinishLast(SceneName));
	}

	/*Function to change Panels to go to and from Controls Menu*/
	public void ChangePanels(bool BoolState) {
		if (BoolState == true) {
			title_text.SetActive (false);
			title_panel.SetActive(false);
			controls_panel.SetActive(true);
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);	//Destroy the currernt selected GameObject for eventsystem
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(controls_panel.transform.GetChild(1).gameObject);	//Set the new GameObject for eventsystem
		} else {
			controls_panel.SetActive(false);
			title_panel.SetActive(true);
			title_text.SetActive (true);
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);	//Destroy the current selected GameObject for eventsystem
			eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(title_panel.transform.GetChild(0).gameObject);	//Set the new GameObject for eventsystem
		}
	}

    /*IEnumerator function to handle fading and then load next scene when finished*/
	IEnumerator Fading(string SceneName) {
		fade.GetComponent<Animator>().SetBool("Fade", true);
		yield return new WaitUntil(()=>fade.GetComponent<Image>().color.a == 1);
		SceneManager.LoadScene(SceneName);
	}

    /*IEnumerator function to scale the player model down to the destination scale*/
	IEnumerator ScaleOverTime() {
		scale = true;
		float current_time = 0.0f;
		while (current_time <= time) {
			player.transform.localScale = Vector3.Lerp (orig_scale, dest_scale, current_time / time);
			current_time += Time.deltaTime;
			yield return null;
		}
		scale = false;
	}

    /*IEnumerator function to track when the player scaling is done and then fade out*/
	IEnumerator FinishLast(string SceneName) {
		while(scale == true) {
			yield return new WaitForSeconds(.1f);	//Keep looping until coroutine is finished
		}
		StartCoroutine(Fading(SceneName));
	}
}
