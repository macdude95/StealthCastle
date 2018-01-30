using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeController : MonoBehaviour {

	BasicEnemyController enemyScript;

	public void Start() {
		enemyScript = GetComponentInParent<BasicEnemyController>();
	}

	public void CheckVision(GameObject player) {
		bool playerWithinVision = false;
		Vector2 enemyPosition = transform.parent.position;
		Vector2 playerPosition = player.transform.position;
		Vector2 direction = (playerPosition - enemyPosition);

		RaycastHit2D hit = Physics2D.Raycast(playerPosition, direction);
		playerWithinVision = hit.collider != null &&
							 hit.collider.gameObject.tag == "Player";

		if (playerWithinVision) {
			Debug.DrawRay(enemyPosition, direction, Color.red, 1.5F);
			transform.parent.position =
				Vector2.MoveTowards(enemyPosition,
									playerPosition,
									enemyScript.speed);
        }
    }
}
