using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Created by Brian Egana
 * This script provides behavior for caltrops. When the caltrops are thrown,
 * the caltrops will collide with anything that is not the player. If the
 * caltrops hits an enemy, the enemy will be permanently slowed and the
 * caltrops will disappear.
 */
public class CaltropsBehavior : ThrowableBehavior {

	public AudioClip setCaltrops;
	private AudioSource audioSource;

	void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	void FixedUpdate() {
		if (isBeingThrown) {
			airTime++;
			SetUsable(false);

			if (ThrownForMaxTime()) {
				audioSource.PlayOneShot(setCaltrops);
				PutThrowableOnGround();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		rb2D.velocity = Vector2.zero;
		GameObject entity = collision.gameObject;
		if (entity.CompareTag("Player") && !isBeingThrown) {
			PutThrowableOnGround();
			SetUsable(true);
		}
		else if (entity.CompareTag("Enemy")) {
			entity.SendMessage("SlowEnemy");
			isBeingThrown = false;
			gameObject.SetActive(false);
		}
		else {
			audioSource.PlayOneShot(setCaltrops);
			PutThrowableOnGround();
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
	GameObject entity = collision.gameObject;
		if (entity.CompareTag("Player") && !isBeingThrown) {
			SetUsable(false);
		}
	}
}
