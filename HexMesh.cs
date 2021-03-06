using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh: MonoBehaviour
{
	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;

	void Awake() {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		hexMesh.name = "Hex Mesh";
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
		Vector3 center = cell.localPosition;
		AddTriangle(center, 0, 6);
	}

	Vector3 scaleBase = new Vector3(HexMetrics.radFactor * HexMetrics.scaler / 3f, 0f, 0f);

	void AddTriangle(Vector3 c, int a, int b) {
		for(int i = a; i < b; i++) {
			Vector3 rot = Quaternion.AngleAxis(60 * (i-1), Vector3.up) * scaleBase;
			AddTriangle(
				c + rot,
				c + rot + HexMetrics.corners[i % 6],
				c + rot + HexMetrics.corners[(i + 1) % 6]
			);
		}
	}
	Vector3 scaler = new Vector3(0.9f, 0.9f, 0.9f);

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
