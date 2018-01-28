using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		Camera c = Camera.current;
		transform.position = new Vector2(c.ScreenToWorldPoint(Input.mousePosition).x, c.ScreenToWorldPoint(Input.mousePosition).y);
	}
}
