using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaltropsBehavior : ThrowableBehavior {

	private void OnCollisionEnter2D(Collision2D collision) {
		GameObject entity = collision.gameObject;
		if (entity.CompareTag("Player") && !isBeingThrown) {
			PutThrowableOnGround();
			SetUsable(true);
		}
		if (entity.CompareTag("Enemy")) {
			BasicEnemyController enemyScript =
				entity.GetComponent<BasicEnemyController>();

			enemyScript.SlowEnemy();
			isBeingThrown = false;
			gameObject.SetActive(false);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
	GameObject entity = collision.gameObject;
		if (entity.CompareTag("Player") && !isBeingThrown) {
			SetUsable(false);
		}
	}
}
