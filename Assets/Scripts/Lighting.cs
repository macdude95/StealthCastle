using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lighting : MonoBehaviour {

	public GameObject myLight;
	public GameObject triangleOject;
	public Material mat;
	public int numOfRays = 50;
	public float lightRange = 100.0f;

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;

		List<Vector3>  vertices = new List<Vector3> ();

		vertices.Add(myLight.transform.position);

		for (int i = 0; i < numOfRays; i++) {
			float angle = i * (2.0f * Mathf.PI) / numOfRays;
			Vector2 lDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));


			RaycastHit2D hit = (Physics2D.Raycast (myLight.transform.position, lDirection, lightRange));

			if (hit) {
				Gizmos.DrawLine (myLight.transform.position, hit.point);
				vertices.Add (new Vector3(hit.point.x, hit.point.y, -1));
			} else {
				// try to make this never happen... drawing can mess up if it does
				Gizmos.DrawRay(myLight.transform.position, lDirection*lightRange);
			}
		}
		if (vertices.Count > 2) {
			drawTriangles (vertices);
		}
	}

	private void drawTriangles(List<Vector3> vertices) {
		List<int> triangles = new List<int> ();

		for (int i = 1; i < numOfRays; i++) {
			triangles.Add (0);
			triangles.Add (i+1);
			triangles.Add (i);
		}
		triangles.Add (0);
		triangles.Add (1);
		triangles.Add (numOfRays);

		Mesh mesh = triangleOject.GetComponent<MeshFilter>().sharedMesh;
		mesh.Clear();
		mesh.vertices = vertices.ToArray ();
		mesh.uv = System.Array.ConvertAll<Vector3, Vector2> (mesh.vertices, getV3fromV2);
		mesh.triangles = triangles.ToArray();
		triangleOject.GetComponent<MeshRenderer> ().material = mat;
	}

	private static Vector2 getV3fromV2 (Vector3 v3)
	{
		return new Vector2 (v3.x, v3.y);
	}

	public void Start() {
		float textureXSize = 1.0f/mat.mainTexture.texelSize.x * transform.localScale.x;
		float textureYSize = 1.0f/mat.mainTexture.texelSize.y * transform.localScale.y;
		mat.SetTextureScale("_MainTex", new Vector2(100f/textureXSize, 100f/textureYSize));
	}

}
