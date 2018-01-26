using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeController : MonoBehaviour {

    // Use this for initialization
    private Vector3 offset;
	void Start () {
        offset = transform.position - transform.parent.position;
	}

	void FixedUpdate () {
        transform.position = transform.parent.position + offset;
	}

    private void CheckVision(GameObject player) {
        Vector3 direction;
        Vector2[] corners = player.GetComponent<PolygonCollider2D>().points;
        List<Vector3> points = new List<Vector3>();

        points.Add(player.transform.position);
        foreach(Vector2 corner in corners) {
            points.Add(new Vector3(corner[0] * player.transform.localScale.x, corner[1] * player.transform.localScale.y) + player.transform.position);
        }

        foreach(Vector3 target in points) {
            direction = (target - this.getRay2DOrigin());
            RaycastHit2D hit = Physics2D.Raycast(this.getRay2DOrigin(), direction);
            if (hit.collider != null && hit.collider.gameObject.tag == "Player") {
                Debug.DrawRay(this.getRay2DOrigin(), direction, Color.red, 1F);
            }
            else {
                //Debug.DrawRay(this.getRay2DOrigin(), direction, Color.blue, 1.5F);
            }
        }
    }

    private Vector3 getRay2DOrigin() {
        return transform.parent.position;
    }
}
