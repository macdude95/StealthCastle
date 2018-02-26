using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinScript : MonoBehaviour {

	public float throwMultiplier = 1f;
	public float throwPower = 0f;
	public float maxPower = 17.5f;

	public Slider coinAimSlider;
	private RectTransform aimRectTransform;

	private GameObject coin;
	private Rigidbody2D coinRb;
	private CircleCollider2D coinCollider2D;

	private BoxCollider2D playerCollider2D;
	private Animator playerAnim;

	void Start() {
		playerAnim = GetComponent<Animator>();
		playerCollider2D = gameObject.GetComponent<BoxCollider2D>();

		aimRectTransform = coinAimSlider.GetComponent<RectTransform>();
		coinAimSlider.value = 0f;
		coinAimSlider.enabled = false;
	}

	void FixedUpdate() {
		if (GameController.instance.GetItemName().Equals("ThrowableCoin")) {
			int dirInt = playerAnim.GetInteger("DIR");
			coinAimSlider.enabled = true;

			if (coin == null) {
				coin = GameController.instance.currItem;
				coinRb = coin.GetComponent<Rigidbody2D>();
				coinCollider2D = coin.GetComponent<CircleCollider2D>();
			}

			SetAimDirection(dirInt);
			if (Input.GetButton("UseItem")) {
				throwPower += 0.25f;
				coinAimSlider.value = throwPower;
			}
			if (Input.GetButtonUp("UseItem") || throwPower >= maxPower) {
				Vector2 coinThrowVec = DirIntToVector(dirInt);
				SetAimDirection(dirInt);

				ThrowCoin(coinThrowVec, throwPower * throwMultiplier);
				throwPower = 0;
				coinAimSlider.value = throwPower;
			}
		}

		if (coin != null && coinCollider2D.isTrigger) {
			Physics2D.IgnoreCollision(coinCollider2D, playerCollider2D, false);
			coin = null;
		}
	}

	private void SetAimDirection(int dirInt) {
		int UP = 0;
		int RIGHT = 1;
		int LEFT = 3;

		if (dirInt == UP) {
			aimRectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		else if (dirInt == RIGHT) {
			aimRectTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
		}
		else if (dirInt == LEFT) {
			aimRectTransform.localEulerAngles = new Vector3(0f, 0f, 90f);
		}
		else {
			aimRectTransform.localEulerAngles = new Vector3(0f, 0f, 180f);
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
