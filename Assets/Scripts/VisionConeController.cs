using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeController : MonoBehaviour {

    public string seenDisguiseType;

    private void CheckVision(GameObject player) {
        bool seen = false;
        Vector3 direction;
        Vector2 scale = player.GetComponent<BoxCollider2D>().size;
        List<Vector3> points = new List<Vector3>();

        points.Add(player.transform.position);
        //add points
        points.Add(new Vector3(player.transform.position.x + (scale.x) * .5f, player.transform.position.y + (scale.y) * .5f)); //top right
        points.Add(new Vector3(player.transform.position.x - (scale.x) * .5f, player.transform.position.y + (scale.y) * .5f)); //top left
        points.Add(new Vector3(player.transform.position.x + (scale.x) * .5f, player.transform.position.y - (scale.y) * .5f)); //bottom right
        points.Add(new Vector3(player.transform.position.x - (scale.x) * .5f, player.transform.position.y - (scale.y) * .5f)); //bottom left

        foreach (Vector3 target in points) {
			direction = (target - transform.parent.position);
			RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, direction);
            if (hit.collider != null && hit.collider.gameObject.tag == "Player") {
				//Debug.DrawRay(transform.parent.position, direction, Color.red, 0.3F);
                seen = true;
            }
            else {
                //Debug.DrawRay(transform.parent.position, direction, Color.blue, 0.3F);
            }
        }
        if (seen) {
            this.SendMessageUpwards("PlayerInVision", player);
            RotateVision(player.transform.position);
        }            
    }

	public void RotateVision(Vector3 target) {
		var dir = target - transform.position;
		var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		//transform.localRotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
		Quaternion rot = Quaternion.AngleAxis(angle + 90, Vector3.forward);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, 0.1f);
	}
}
