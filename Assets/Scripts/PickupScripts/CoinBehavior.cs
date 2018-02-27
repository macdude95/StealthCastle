using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : ThrowableBehavior {

	public GameObject soundRingPrefab;
	private GameObject soundRing;

	void Awake() {
		soundRing = Instantiate(soundRingPrefab,
								transform.position,
								new Quaternion());
		soundRing.SetActive(false);
	}

	void FixedUpdate() {
		if (isBeingThrown) {
			airTime++;
			SetUsable(false);
		}

		if (ThrownForMaxTime()) {
			PutThrowableOnGround();
			SoundRing();
			SetUsable(true);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		PutThrowableOnGround();
		SoundRing();
		SetUsable(true);
	}

	private void SoundRing() {
		soundRing.transform.position = this.transform.position;
		soundRing.transform.localScale = new Vector3(0, 0, 0f);

		SpriteRenderer sr = soundRing.GetComponent<SpriteRenderer>();
		sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);

		soundRing.SetActive(true);
	}
}
