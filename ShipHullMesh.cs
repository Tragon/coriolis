using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ShipHullMesh : MonoBehaviour
{
	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;

	void Awake() {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Ship Hull Mesh";
		vertices = new List<Vector3>();
		triangles = new List<int>();
	}

	public void Triangulate(HexCell[] cells) {
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		for(int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();
	}

	void Triangulate(HexCell cell) {
		//Vector3 center = cell.transform.localPosition;
		//cockpit
		Vector3 center = cell.localPosition;
		AddTriangle(center, 0, 6);
		// left flank
		center = HexMetrics.move(center, -1, 1);
		AddTriangle(center, 1, 3);
		// front left
		center = HexMetrics.move(center, 1, 0);
		AddTriangle(center, 1, 5);
		// front top
		center = HexMetrics.move(center, 1, 0);
		AddTriangle(center, 3, 5);
		// front right
		center = HexMetrics.move(center, 0, -1);
		AddTriangle(center, 3, 7);
		// right flank
		center = HexMetrics.move(center, 0, -1);
		AddTriangle(center, 5, 7);
	}

	void AddTriangle(Vector3 c, int a, int b) {
		for(int i = a; i < b; i++) {
			AddTriangle(
				c,
				c + HexMetrics.cornersShip[i % 6],
				c + HexMetrics.cornersShip[(i + 1) % 6]
			);
		}
	}

	void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
}
