using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeController : MonoBehaviour {




    private void CheckVision(GameObject player) {
        bool seen = false;
        Vector3 direction;
        Vector2[] corners = player.GetComponent<PolygonCollider2D>().points;
        List<Vector3> points = new List<Vector3>();

        points.Add(player.transform.position);
        foreach(Vector2 corner in corners) {
            points.Add(new Vector3(corner[0] * player.transform.localScale.x, corner[1] * player.transform.localScale.y) + player.transform.position);
        }

        foreach(Vector3 target in points) {
			direction = (target - transform.parent.position);
			RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, direction);
            if (hit.collider != null && hit.collider.gameObject.tag == "Player") {
				Debug.DrawRay(transform.parent.position, direction, Color.red, 0.3F);
                seen = true;
            }
            else {
                Debug.DrawRay(transform.parent.position, direction, Color.blue, 0.3F);
            }
        }
        if (seen)
        {
            this.SendMessageUpwards("PlayerInVision", player);
            rotateVision(player.transform.position);
        }            
    }

    public void rotateVision(Vector3 target)
    {
        var dir = target - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

}
