using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour {

	public float throwMultiplier = 1f;
	public float throwPower = 0f;

	private GameObject coin;
	private Rigidbody2D coinRb;
	private CircleCollider2D coinCollider2D;

	private BoxCollider2D playerCollider2D;
	private Animator playerAnim;

	void Start() {
		playerAnim = GetComponent<Animator>();
		playerCollider2D = gameObject.GetComponent<BoxCollider2D>();
	}

	void FixedUpdate() {
		if (GameController.instance.GetItemName().Equals("ThrowableCoin")) {
			if (coin == null) {
				coin = GameController.instance.currItem;
				coinRb = coin.GetComponent<Rigidbody2D>();
				coinCollider2D = coin.GetComponent<CircleCollider2D>();
			}

			if (Input.GetButton("Interaction")) {
				throwPower++;
			}
			if (Input.GetButtonUp("Interaction")) {
				int dirInt = playerAnim.GetInteger("DIR");
				Vector2 coinThrowVec = DirIntToVector(dirInt);

				ThrowCoin(coinThrowVec, throwPower * throwMultiplier);
				throwPower = 0;
			}
		}

		if (coin != null && coinCollider2D.isTrigger) {
			Physics2D.IgnoreCollision(coinCollider2D, playerCollider2D, false);
		}
	}

	private Vector2 DirIntToVector(int dirInt) {
		int UP = 0;
		int RIGHT = 1;
		int LEFT = 3;

		if (dirInt == UP) {
			return Vector2.up;
		}
		else if (dirInt == RIGHT) {
			return Vector2.right;
		}
		else if (dirInt == LEFT) {
			return Vector2.left;
		}
		else {
			return Vector2.down;
		}
	}

	private void ThrowCoin(Vector2 dir, float power) {
		coin.transform.position = this.transform.position;
		coinCollider2D.isTrigger = false;
		Physics2D.IgnoreCollision(coinCollider2D, playerCollider2D, true);

		coin.SetActive(true);
		coinRb.AddForce(dir * power, ForceMode2D.Impulse);
		GameController.instance.SetPlayerItem(null);
	}
}
