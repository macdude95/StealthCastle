using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Created by Brian Egana
 * This script provides behavior for a throwable coin. When the coin hits a
 * surface, or is thrown far enough that it "hits the ground", it makes a
 * sound that attracts any enemies nearby.
 */
public class CoinBehavior : ThrowableBehavior {

    public AudioClip coinToss;
    public AudioClip coinHitConcrete;

	public GameObject soundRingPrefab;
	private GameObject soundRing;
    private AudioSource audioSource;

	void Awake() {
		soundRing = Instantiate(soundRingPrefab,
								transform.position,
								new Quaternion());
		soundRing.SetActive(false);
        audioSource = GetComponent<AudioSource>();
	}

	void FixedUpdate() {
		if (isBeingThrown) {
            audioSource.PlayOneShot(coinToss);
			airTime++;
			SetUsable(false);
		}

		if (ThrownForMaxTime()) {
            audioSource.PlayOneShot(coinHitConcrete);
			PutThrowableOnGround();
			SoundRing();
			SetUsable(true);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		int WALL_LAYER = 9;
		if (collision.gameObject.layer == WALL_LAYER ||
			!collision.gameObject.CompareTag("Enemy")) {
            audioSource.PlayOneShot(coinHitConcrete);
		}
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
