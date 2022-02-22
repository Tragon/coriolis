using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid: MonoBehaviour
{
	public int width = 6;
	public int height = 6;
	public int shipsFriend = 2;
	public int shipsEnemy = 2;
	public bool enableLabel = false;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;
	public GameObject shipPrefabEnemy;
	public GameObject shipPrefabFriend;

	Canvas gridCanvas;
	HexMesh hexMesh;
	HexCell[] cells;
	Ship[] ships;

	int activeShip = 0;

	void Awake() {
		int offX = width / 2;
		int offY = width / 2;
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();
		cells = new HexCell[height * width];
		ships = new Ship[shipsEnemy + shipsFriend];

		for(int z = 0, i = 0; z < height; z++) {
			for(int x = 0; x < width; x++) {
				CreateCell(x - offX, z - offY, i++);
			}
		}

		for(int i = 0; i < shipsEnemy; i++) {
			GameObject shipObj = Instantiate<GameObject>(shipPrefabEnemy);
			Ship ship = ships[i] = shipObj.GetComponentInChildren<Ship>();
			ship.CreateShip(i, 0);
		}

		for(int i = 0; i < shipsFriend; i++) {
			GameObject shipObj = Instantiate<GameObject>(shipPrefabFriend);
			Ship ship = ships[i + shipsEnemy] = shipObj.GetComponentInChildren<Ship>();
			ship.CreateShip(i, 1);
		}
	}

	void Start() {
		hexMesh.Triangulate(cells);
	}
	void OnGUI() {
		string[] shipNames = new string[shipsFriend + shipsEnemy];
		for(int i = 0; i < (shipsFriend + shipsEnemy); i++) {
			shipNames[i] = "Ship " + i;
		}
		activeShip = GUILayout.SelectionGrid(activeShip, shipNames, 1);
		if(GUILayout.Button("Forward")) {
			ships[activeShip].forward();
		}
		if(GUILayout.Button("Right")) {
			ships[activeShip].right();
		}
		if(GUILayout.Button("Left")) {
			ships[activeShip].left();
		}
	}

	void CreateCell(int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = new HexCell();// Instantiate<HexCell>(cellPrefab);
		//cell.transform.SetParent(transform, false);
		//cell.transform.localPosition = position;
		cell.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		if(enableLabel) {
			Text label = Instantiate<Text>(cellLabelPrefab);
			label.rectTransform.SetParent(gridCanvas.transform, false);
			label.rectTransform.anchoredPosition =
				new Vector2(position.x, position.z);
			label.text = cell.coordinates.ToStringOnSeparateLines();
		}
	}

	/*void CreateShipE(int i) {
		CreateShip(i, shipCellsE, 0);
	}

	void CreateShipF(int i) {
		CreateShip(i, shipCellsF, 1);
	}

	void CreateShip(int i, HexCell[] cells, int off) {
		int x = i * 3;
		int z = off * 3;
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);
		HexCell sc = cells[i] = new HexCell();
		sc.localPosition = position;
		sc.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
	}*/
}
