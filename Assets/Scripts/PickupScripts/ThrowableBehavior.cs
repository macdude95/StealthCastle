using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableBehavior : MonoBehaviour {

	public bool isBeingThrown = false;
	public float airTime = 0f;
	public float maxAirTime = 50f;

	private Rigidbody2D rb2D;
	private CircleCollider2D circCol;

	void Start() {
		airTime = 0f;

		rb2D = GetComponent<Rigidbody2D>();
		rb2D.velocity = Vector2.zero;

		circCol = GetComponent<CircleCollider2D>();
		SetUsable(true);
	}

	void FixedUpdate() {
		if (isBeingThrown) {
			airTime++;
			SetUsable(false);

			if (ThrownForMaxTime()) {
				PutThrowableOnGround();
			}
		}
	}

	protected void SetUsable(bool usable) {
		circCol.isTrigger = usable;
	}

	protected bool ThrownForMaxTime() {
		return airTime >= maxAirTime;
	}

	protected void PutThrowableOnGround() {
		rb2D.velocity = Vector2.zero;
		airTime = 0f;
		isBeingThrown = false;
	}
}
