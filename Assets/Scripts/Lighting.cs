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

		Mesh mesh = triangleOject.GetComponent<MeshFilter>().mesh;
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


	// Find intersection of RAY & SEGMENT
//	private Vector3 getIntersection(Vector2 rayA, Vector2 rayB, Vector2 segmentA, Vector2 segmentB){
//		// RAY in parametric: Point + Delta*T1
//		float r_px = rayA.x;
//		float r_py = rayA.y;
//		float r_dx = rayB.x-rayA.x;
//		float r_dy = rayB.y-rayA.y;
//		// SEGMENT in parametric: Point + Delta*T2
//		float s_px = segmentA.x;
//		float s_py = segmentA.y;
//		float s_dx = segmentB.x-segmentA.x;
//		float s_dy = segmentB.y-segmentA.y;
//		// Are they parallel? If so, no intersect
//		float r_mag = Mathf.Sqrt(r_dx*r_dx+r_dy*r_dy);
//		float s_mag = Mathf.Sqrt(s_dx*s_dx+s_dy*s_dy);
//		if(r_dx/r_mag==s_dx/s_mag && r_dy/r_mag==s_dy/s_mag){
//			// Unit vectors are the same.
//			return Vector3.zero;
//		}
//		// SOLVE FOR T1 & T2
//		// r_px+r_dx*T1 = s_px+s_dx*T2 && r_py+r_dy*T1 = s_py+s_dy*T2
//		// ==> T1 = (s_px+s_dx*T2-r_px)/r_dx = (s_py+s_dy*T2-r_py)/r_dy
//		// ==> s_px*r_dy + s_dx*T2*r_dy - r_px*r_dy = s_py*r_dx + s_dy*T2*r_dx - r_py*r_dx
//		// ==> T2 = (r_dx*(s_py-r_py) + r_dy*(r_px-s_px))/(s_dx*r_dy - s_dy*r_dx)
//		float T2 = (r_dx*(s_py-r_py) + r_dy*(r_px-s_px))/(s_dx*r_dy - s_dy*r_dx);
//		float T1 = (s_px+s_dx*T2-r_px)/r_dx;
//		// Must be within parametic whatevers for RAY/SEGMENT
//		if(T1<0) return Vector3.zero;
//		if(T2<0 || T2>1) return Vector3.zero;
//		// Return the POINT OF INTERSECTION
//		return new Vector3(r_px+r_dx*T1,r_py+r_dy*T1, T1);
//	}
//	function getSightPolygon(float sightX, float sightY){
//		// Get all unique points
//		var points = (function(segments){
//			var a = [];
//			segments.forEach(function(seg){
//				a.push(seg.a,seg.b);
//			});
//			return a;
//		})(segments);
//		var uniquePoints = (function(points){
//			var set = {};
//			return points.filter(function(p){
//				var key = p.x+","+p.y;
//				if(key in set){
//					return false;
//				}else{
//					set[key]=true;
//					return true;
//				}
//			});
//		})(points);
//		// Get all angles
//		var uniqueAngles = [];
//		for(var j=0;j<uniquePoints.length;j++){
//			var uniquePoint = uniquePoints[j];
//			var angle = Math.atan2(uniquePoint.y-sightY,uniquePoint.x-sightX);
//			uniquePoint.angle = angle;
//			uniqueAngles.push(angle-0.00001,angle,angle+0.00001);
//		}
//		// RAYS IN ALL DIRECTIONS
//		var intersects = [];
//		for(var j=0;j<uniqueAngles.length;j++){
//			var angle = uniqueAngles[j];
//			// Calculate dx & dy from angle
//			var dx = Math.cos(angle);
//			var dy = Math.sin(angle);
//			// Ray from center of screen to mouse
//			var ray = {
//				a:{x:sightX,y:sightY},
//				b:{x:sightX+dx,y:sightY+dy}
//			};
//			// Find CLOSEST intersection
//			var closestIntersect = null;
//			for(var i=0;i<segments.length;i++){
//				var intersect = getIntersection(ray,segments[i]);
//				if(intersect.equals(Vector3.zero) continue;
//				if(!closestIntersect || intersect.param<closestIntersect.param){
//					closestIntersect=intersect;
//				}
//			}
//			// Intersect angle
//			if(!closestIntersect) continue;
//			closestIntersect.angle = angle;
//			// Add to list of intersects
//			intersects.push(closestIntersect);
//		}
//		// Sort intersects by angle
//		intersects = intersects.sort(function(a,b){
//			return a.angle-b.angle;
//		});
//		// Polygon is intersects, in order of angle
//		return intersects;
//	}
//
//	///////////////////////////////////////////////////////
//	// DRAWING
//	var canvas = document.getElementById("canvas");
//	var ctx = canvas.getContext("2d");
//	var foreground = new Image();
//	function draw(){
//		// Clear canvas
//		ctx.clearRect(0,0,canvas.width,canvas.height);
//		// Sight Polygons
//		var fuzzyRadius = 10;
//		var polygons = [
//			getSightPolygon(20,Mouse.y),
//			getSightPolygon(820,360-Mouse.y)
//		];
//		for(var angle=0;angle<Math.PI*2;angle+=(Math.PI*2)/10){
//			var dx = Math.cos(angle)*fuzzyRadius;
//			var dy = Math.sin(angle)*fuzzyRadius;
//			polygons.push(
//				getSightPolygon(20+dx,Mouse.y+dy),
//				getSightPolygon(820+dx,360-Mouse.y+dy)
//			);
//		};
//		// DRAW AS A GIANT POLYGON
//		for(var i=2;i<polygons.length;i++){
//			drawPolygon(polygons[i],ctx,"rgba(255,255,255,0.2)");
//		}
//		drawPolygon(polygons[0],ctx,"#fff");
//		drawPolygon(polygons[1],ctx,"#fff");
//		// Masked Foreground
//		ctx.globalCompositeOperation = "source-in";
//		ctx.drawImage(foreground,0,0);
//		ctx.globalCompositeOperation = "source-over";
//		// Draw dots
//		ctx.fillStyle = "#fff";
//		ctx.beginPath();
//		ctx.arc(20, Mouse.y, 2, 0, 2*Math.PI, false);
//		ctx.fill();
//		ctx.beginPath();
//		ctx.arc(820, 360-Mouse.y, 2, 0, 2*Math.PI, false);
//		ctx.fill();
//		for(var angle=0;angle<Math.PI*2;angle+=(Math.PI*2)/10){
//			var dx = Math.cos(angle)*fuzzyRadius;
//			var dy = Math.sin(angle)*fuzzyRadius;
//			ctx.beginPath();
//			ctx.arc(20+dx, Mouse.y+dy, 2, 0, 2*Math.PI, false);
//			ctx.fill();
//			ctx.beginPath();
//			ctx.arc(820+dx, 360-Mouse.y+dy, 2, 0, 2*Math.PI, false);
//			ctx.fill();
//		}
//	}
//	function drawPolygon(polygon,ctx,fillStyle){
//		ctx.fillStyle = fillStyle;
//		ctx.beginPath();
//		ctx.moveTo(polygon[0].x,polygon[0].y);
//		for(var i=1;i<polygon.length;i++){
//			var intersect = polygon[i];
//			ctx.lineTo(intersect.x,intersect.y);
//		}
//		ctx.fill();
//	}
//	// LINE SEGMENTS
//	var segments = [
//		// Border
//		{a:{x:0,y:0}, b:{x:840,y:0}},
//		{a:{x:840,y:0}, b:{x:840,y:360}},
//		{a:{x:840,y:360}, b:{x:0,y:360}},
//		{a:{x:0,y:360}, b:{x:0,y:0}},
//		// Polygon #1
//		{a:{x:100,y:150}, b:{x:120,y:50}},
//		{a:{x:120,y:50}, b:{x:200,y:80}},
//		{a:{x:200,y:80}, b:{x:140,y:210}},
//		{a:{x:140,y:210}, b:{x:100,y:150}},
//		// Polygon #2
//		{a:{x:100,y:200}, b:{x:120,y:250}},
//		{a:{x:120,y:250}, b:{x:60,y:300}},
//		{a:{x:60,y:300}, b:{x:100,y:200}},
//		// Polygon #3
//		{a:{x:200,y:260}, b:{x:220,y:150}},
//		{a:{x:220,y:150}, b:{x:300,y:200}},
//		{a:{x:300,y:200}, b:{x:350,y:320}},
//		{a:{x:350,y:320}, b:{x:200,y:260}},
//		// Polygon #4
//		{a:{x:540,y:60}, b:{x:560,y:40}},
//		{a:{x:560,y:40}, b:{x:570,y:70}},
//		{a:{x:570,y:70}, b:{x:540,y:60}},
//		// Polygon #5
//		{a:{x:650,y:190}, b:{x:760,y:170}},
//		{a:{x:760,y:170}, b:{x:740,y:270}},
//		{a:{x:740,y:270}, b:{x:630,y:290}},
//		{a:{x:630,y:290}, b:{x:650,y:190}},
//		// Polygon #6
//		{a:{x:600,y:95}, b:{x:780,y:50}},
//		{a:{x:780,y:50}, b:{x:680,y:150}},
//		{a:{x:680,y:150}, b:{x:600,y:95}}
//	];
//	// DRAW LOOP
//	window.requestAnimationFrame = window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.msRequestAnimationFrame;
//	var updateCanvas = true;
//	function drawLoop(){
//		requestAnimationFrame(drawLoop);
//		if(updateCanvas){
//			draw();
//			updateCanvas = false;
//		}
//	}
//	window.onload = function(){
//		foreground.onload = function(){
//			drawLoop();
//		};
//		foreground.src = "foreground.png";
//	};
//	// MOUSE	
//	var Mouse = {
//		x: canvas.width/2,
//		y: canvas.height/2
//	};
//	canvas.onmousemove = function(event){	
//		Mouse.x = event.clientX;
//		Mouse.y = event.clientY;
//		updateCanvas = true;
//	};
}
