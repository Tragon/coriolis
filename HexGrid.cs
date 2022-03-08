using System;
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
	List<HexCell> cells = new List<HexCell>();
	List<Ship> ships = new List<Ship>();

	int activeShip = 0;

	bool directControls = false;

	void Awake() {
		int offX = width / 2;
		int offY = height / 2;
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		for(int z = 0, i = 0; z < height; z++) {
			for(int x = 0; x < width; x++) {
				CreateCell(x - offX, z - offY, i++);
			}
		}

		for(int i = 0; i < shipsEnemy; i++) {
			GameObject shipObj = Instantiate<GameObject>(shipPrefabEnemy);
			Ship ship = shipObj.GetComponentInChildren<Ship>();
			ship.CreateShip(i, 0, "Enemy Ship " + i, false);
			ships.Add(ship);
		}

		for(int i = 0; i < shipsFriend; i++) {
			GameObject shipObj = Instantiate<GameObject>(shipPrefabFriend);
			Ship ship = shipObj.GetComponentInChildren<Ship>();
			ship.CreateShip(i, 1, "Ship " + i, true);
			ships.Add(ship);
		}
	}

    public Ship AddShip(string name, bool isPlayer) {
		GameObject skin = isPlayer ? shipPrefabFriend : shipPrefabEnemy;
		GameObject shipObj = Instantiate<GameObject>(skin);
		Ship ship = shipObj.GetComponentInChildren<Ship>();
		ship.CreateShip(ships.FindAll(x => x.IsPlayer == isPlayer).Count, isPlayer ? 1 : 0, name, isPlayer);
		ships.Add(ship);
		return ship;
	}

    void Start() {
		hexMesh.Triangulate(cells.ToArray());
	}

	public void SetDirectControls(bool directControls) {
		this.directControls = directControls;

	}

	/*void OnGUI() {
		if(directControls) {
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
	}*/

	void CreateCell(int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = new HexCell();// Instantiate<HexCell>(cellPrefab);
		cells.Add(cell);
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
