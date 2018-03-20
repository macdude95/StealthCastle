using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* SoundRingController.cs
 * Created by Michael Cantrell
 */

public class SoundRingController : MonoBehaviour {

	public float endScale = 0.5f;
	public float smoothTime = 0.3F;
	public float fadeSpeed = 50;
    public string source = "";

	private float velocity;

    void OnEnable () {
		StartCoroutine (FadeAndDestroy ());
	}

	IEnumerator FadeAndDestroy() {
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		for (float f = sr.color.a; f >= 0; f -= 1/fadeSpeed) {
			sr.color = new Color (sr.color.r, sr.color.g, sr.color.b, f);
			yield return null;
		}
        this.gameObject.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(false);
	}

	void Update() {
		float newScale = Mathf.SmoothDamp (transform.localScale.x, endScale, ref velocity, smoothTime);
		transform.localScale = new Vector2 (newScale, newScale);
	}
}
