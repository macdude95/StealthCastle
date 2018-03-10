using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* LetterSceneController.cs
 * Created by Michael Cantrell
 */

public class LetterSceneController : MonoBehaviour {
    
    public Image fadeInOutImage;
    public Animator fadeInOutAnimator;
    public string firstLevelName;
    public AudioClip letterOpeningSound;

	// Use this for initialization
	void Start () {
        fadeInOutImage.gameObject.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(letterOpeningSound);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
        {
            StartCoroutine(FadeAndCompletion(() => {
                SceneManager.LoadScene(firstLevelName);
            }));
        }
	}

    /* FadeAndCompletion
    * Created by Michael Cantrell
    * A coroutine that fades the scene to black and then
    * performs a specified action after the fade is complete
    */
    private IEnumerator FadeAndCompletion(System.Action onFadeComplete)
    {
        fadeInOutAnimator.SetBool("FADE", true);
        yield return new WaitUntil(() => Mathf.Approximately(fadeInOutImage.color.a, 1));
        onFadeComplete();
    }
}
