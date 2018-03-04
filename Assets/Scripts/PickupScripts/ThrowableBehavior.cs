using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Created by Brian Egana
 * This script provides default initialization for throwable items. The script
 * is designed so that if another throwable item is added, a script can
 * inherit this class and implement custom behavior.
 */
public class ThrowableBehavior : MonoBehaviour {

	public bool isBeingThrown = false;
	public float airTime = 0f;
	public float maxAirTime = 50f;

	protected Rigidbody2D rb2D;
	private CircleCollider2D circCol;

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		rb2D.velocity = Vector2.zero;

		circCol = GetComponent<CircleCollider2D>();
		SetUsable(true);
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
