using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 7;
	public float smoothMoveTime = 0.1f;
	public float turnSpeed = 8;

	private float angle;
	private float smoothInputMagnitude;
	private float smoothMoveVelocity;
	private Vector2 velocity;

	private Rigidbody2D rigidBody;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		Vector3 inputDirection = new Vector3 (-Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"), 0).normalized;
		float inputMagnitude = inputDirection.magnitude;
		smoothInputMagnitude = Mathf.SmoothDamp (smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

		float targetAngle = Mathf.Atan2 (inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
		angle = Mathf.LerpAngle (angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

		velocity = transform.up * speed * smoothInputMagnitude;
	}


	void FixedUpdate()
	{
		rigidBody.MoveRotation (angle);
		rigidBody.MovePosition (rigidBody.position + velocity * Time.deltaTime);
	}

	private void OnTriggerStay2D(Collider2D other)
	{

		if (other.gameObject.tag == "VisionDetector")
		{
			other.gameObject.SendMessage("CheckVision", this.gameObject);
		}


	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "BasicTrap")
		{
			(collision.gameObject.transform.GetChild(0)).SendMessage("ActivateTrap");
		}
	}

}
