using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour {

	public GameObject soundRingPrefab;

	private GameObject soundRing;
	private Rigidbody2D rb2D;
	private CircleCollider2D circCol;

	void Awake() {
		soundRing = Instantiate(soundRingPrefab,
								transform.position,
								new Quaternion());
		soundRing.SetActive(false);
	}

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		circCol = GetComponent<CircleCollider2D>();
	}

	void FixedUpdate() {
		if (rb2D.velocity == Vector2.zero) {
			circCol.isTrigger = true;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		SoundRing();
		rb2D.velocity = Vector2.zero;
		circCol.isTrigger = true;
	}

	private void SoundRing() {
		soundRing.transform.position = this.transform.position;
		soundRing.transform.localScale = new Vector3(0, 0, 0f);
		SpriteRenderer sr = soundRing.GetComponent<SpriteRenderer>();
		sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
		soundRing.SetActive(true);
	}
}
