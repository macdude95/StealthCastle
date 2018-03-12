using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fading : MonoBehaviour {

	public static Fading instance;	//Fading script instance
	public Image FaderImage;		//Image to fade
	public float fadeSpeed = .8f;	//How fast to fade

	private bool fading = false;	//Boolean to do something after IEnumerator functions are done

	/*Create the instance of Fading*/
	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
	}
		
	/*Function to fade to black*/
	public IEnumerator FadeToFullAlpha() {
		fading = true;
		FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, 0);	//Set the alpha
		//Continue looping as long as the alpha is not max
		while (FaderImage.color.a < 1.0f) {
			FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, FaderImage.color.a + (Time.deltaTime / fadeSpeed));	//Set the fade
			yield return null;
		}
		fading = false;
	}

	/*Function to fade to nothing (clear)*/
	public IEnumerator FadeToZeroAlpha() {
		fading = true;
		FaderImage.color = new Color (FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, 1);
		while(FaderImage.color.a > 0.0f) {
			FaderImage.color = new Color(FaderImage.color.r, FaderImage.color.g, FaderImage.color.b, FaderImage.color.a - (Time.deltaTime / fadeSpeed));	//Set the fade
			yield return null;
		}
		fading = false;
	}

	/*Function to load scene after fading is finished*/
	public IEnumerator DoLast(string SceneName) {
		while(fading == true) {
			yield return new WaitForSeconds(.1f);	//Keep looping until coroutine is finished
		}
		SceneManager.LoadScene (SceneName);	//Load the next scene
	}
}
