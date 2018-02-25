using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Seeker))]
public class DogPathFinding : MonoBehaviour {

	public Transform target;

	public float pathUpdateDelay = 1;

	private Seeker seeker;
	private Rigidbody2D rb;

	public Path path;

	[HideInInspector]
	public bool pathIsEnded = false;

	public float nextWaypointDistance = 3;

	private int currentWaypoint = 0;
	private bool searchingForPlayer = false;

	void Awake() {
		seeker = GetComponent<Seeker> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void OnEnable() {
		StartPathing ();
	}

	void StartPathing() {
		if (target == null) {
			if (!searchingForPlayer) {
				searchingForPlayer = true;
				StartCoroutine (SearchForPlayer ());
			}
			return;
		}
		seeker.StartPath (transform.position, target.position, OnPathComplete);
		StartCoroutine(UpdatePath());
	}

	IEnumerator SearchForPlayer () {
		GameObject sResult = GameObject.FindGameObjectWithTag ("Player");
		if (sResult == null) {
			yield return new WaitForSeconds (0.5f);
			StartCoroutine (SearchForPlayer ());
		} else {
			target = sResult.transform;
			searchingForPlayer = false;
			StartCoroutine (UpdatePath ());
			yield return null;
		}
	}

	IEnumerator UpdatePath() {
		yield return new WaitForSeconds (pathUpdateDelay);
		if (target == null) {
			if (!searchingForPlayer) {
				searchingForPlayer = true;
				StartCoroutine (SearchForPlayer ());
			}
			yield return null;
		}
		// Start a new path to the target position, return the result to the OnPathComplete method
		seeker.StartPath (transform.position, target.position, OnPathComplete);
		StartCoroutine (UpdatePath ());
	}

	public void OnPathComplete(Path p) {
//		Debug.Log ("We got a path. Did it have an error? " + p.error);
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
	}

	void FixedUpdate() {
		if (target == null) {
			if (!searchingForPlayer) {
				searchingForPlayer = true;
				StartCoroutine (SearchForPlayer ());
			}
			return;
		}

		if (path == null)
			return;

		if (currentWaypoint >= path.vectorPath.Count) {
			if (pathIsEnded)
				return;
			Debug.Log ("End of path reached.");
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;

		float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
		if (dist < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
	}

    public Vector2 ShortTermDirectionToTarget() {
        if (path == null || currentWaypoint >= path.vectorPath.Count) {
            return Vector2.zero;
        }
        return (path.vectorPath[currentWaypoint] - transform.position).normalized;
    }
 }
