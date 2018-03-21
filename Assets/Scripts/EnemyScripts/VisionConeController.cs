using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// [Mostly?] Written by Daniel Anderson
/// </summary>
public class VisionConeController : MonoBehaviour {

    public string seenDisguiseType;
    [UnityEngine.Range(0, 2)]
    public float numPiRadiansOfCircle = 0.25f;
    public int numOfRays = 50;
    public float lightRange = 50.0f;
    [UnityEngine.Range(1, 10)]
    public int raycastFrameDelay = 1;
    private Mesh coneMesh;
    public bool ignoresDisguises;
    public bool isRanged;


    public void CheckVision(GameObject player, string curDisguise) {


        if (curDisguise != null && !ignoresDisguises && !curDisguise.Equals(seenDisguiseType))
            return;

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
				Debug.DrawRay(transform.parent.position, direction, Color.red, 0.3F);
                seen = true;
            }
            else {
                Debug.DrawRay(transform.parent.position, direction, Color.blue, 0.3F);
            }
        }
        if (seen) {
            //this whole thing is an awful hack but the real solution is even worse
            if (ignoresDisguises)
            {
                this.transform.parent.GetComponent<DogController>().PlayerInVision(player);
            }
            else if(isRanged)
            {
                this.transform.parent.GetComponent<RangedEnemyController>().PlayerInVision(player, player.GetComponent<PlayerController>());
            }
            else
            {
                this.transform.parent.GetComponent<BasicEnemyController>().PlayerInVision(player, player.GetComponent<PlayerController>());
            }
        }            
    }

	public void RotateVision(Vector3 target) {
		var dir = target - transform.position;
		var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		Quaternion rot = Quaternion.AngleAxis(angle + 90, Vector3.forward);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, 0.1f);
	}

    private void Update()
    {
        if (Time.frameCount % raycastFrameDelay == 0)
        {
            UpdateDynamicVisionCone();
        }
    }

    /* UpdateDynamicVisionCone
    * Created by Michael Cantrell
    * Is called every "raycastFrameDelay" frames in order to dynamically create
    * the mesh that is the dynamically shaping vision cone
    */
    void UpdateDynamicVisionCone() {
        List<Vector3> vertices = new List<Vector3>();

        vertices.Add(transform.InverseTransformPoint(transform.position));
        for (int i = 0; i < numOfRays; i++)
        {
            float angle = i * (numPiRadiansOfCircle * Mathf.PI) / (numOfRays - 1);
            angle += (transform.localRotation.eulerAngles.z - 90) * Mathf.Deg2Rad - (numPiRadiansOfCircle / 2 * Mathf.PI);
            Vector2 lDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            string[] layers = { "Walls", "Enemy" };
            RaycastHit2D hit = (Physics2D.Raycast(transform.position, lDirection, lightRange, LayerMask.GetMask(layers)));
            if (hit)
            {
                Debug.DrawLine(transform.position, hit.point);
                vertices.Add(transform.InverseTransformPoint(new Vector3(hit.point.x, hit.point.y, 0)));
            }
            else
            {
                Debug.DrawRay(transform.position, lDirection);
                vertices.Add(transform.InverseTransformPoint(new Vector3(transform.position.x + lDirection.x * lightRange, transform.position.y + lDirection.y * lightRange, 0)));
            }
        }
        if (vertices.Count > 2)
        {
            drawTriangles(vertices);
        }
    }

    /* drawTriangles
    * Created by Michael Cantrell
    * Take the verticies calculated using raycasts and draw triangles to 
    * create the mesh
    */
    private void drawTriangles(List<Vector3> vertices)
    {
        List<int> triangles = new List<int>();

        for (int i = 1; i < numOfRays; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i);
        }

        coneMesh.Clear();
        coneMesh.vertices = vertices.ToArray();
        coneMesh.uv = System.Array.ConvertAll<Vector3, Vector2>(coneMesh.vertices, getV3fromV2);
        coneMesh.triangles = triangles.ToArray();
    }

    /* getV3fromV2
    * Created by Michael Cantrell
    * Turn a Vector3 into a Vector2 by disregaurding the z component
    */
    private static Vector2 getV3fromV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

    void Start()
    {
        coneMesh = GetComponent<MeshFilter>().mesh;
        var render = GetComponent<MeshRenderer>();
        render.sortingLayerName = "Entity";
    }

    /* OnDrawGizmos
    * Created by Michael Cantrell
    * uncomment this line if you want to see vision cone in scene window
    */
    //void OnDrawGizmos()
    //{
    //    Start();
    //    Update();
    //}
}
