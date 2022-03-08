using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship: MonoBehaviour
{
    public string faction = "Player";

    ShipHullMesh hull;
    ShipTowerMesh tower;
    public ShipInstruction instructions;
    public TMPro.TMP_Text nameLabel;
    public HexCell cell { get; private set; }
    float orientation = 0f;
    public string Name { get; private set; }
    public bool IsPlayer { get; private set; }

    void Start() {
        Refresh();
    }
    
    void Refresh() {
        hull.Triangulate(new HexCell[1] { cell });
        tower.Triangulate(new HexCell[1] { cell });
    }

    internal void forward() {
        Vector3 pos = Vector3.zero;
        pos.x += HexMetrics.getFactor();
        pos.z += 0.5f;
        transform.position += Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * pos;
        //Refresh();
    }

    internal void backward() {
        Vector3 pos = Vector3.zero;
        pos.x += HexMetrics.getFactor();
        pos.z += 0.5f;
        transform.position -= Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * pos;
        //Refresh();
    }

    internal void right() {
        transform.Rotate(Vector3.up, 60);
    }

    internal void left() {
        transform.Rotate(Vector3.up, -60);
    }

    public void CreateShip(int i, int off, string name, bool isPlayer) {
        this.Name = name;
        this.IsPlayer = isPlayer;
        nameLabel.text = name;
        CreateShip(i, off);
    }

    public void CreateShip(int i, int off) {
        int x = i * 3;
        int z = off * 3;
        float scale = 0.1f;
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f) * scale;
        position.y = -14.95f;
        position.z = z * (HexMetrics.outerRadius * 1.5f) * scale;
        transform.position = position;

        cell = new HexCell();
        cell.localPosition = Vector3.zero;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        hull = GetComponentInChildren<ShipHullMesh>();
        tower = GetComponentInChildren<ShipTowerMesh>();
    }
}
