using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {

	public Camera c;
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector2(c.ScreenToWorldPoint(Input.mousePosition).x, c.ScreenToWorldPoint(Input.mousePosition).y);
	}
}
