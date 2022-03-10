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
    public TriangleCoordinate tcoord { get; private set; }
    public int orientation { get; private set; }
    public string Name { get; private set; }
    public bool IsPlayer { get; private set; }

    void Start() {
        Refresh();
    }
    
    void Refresh() {
        hull.Triangulate(new HexCell[1] { cell });
        tower.Triangulate(new HexCell[1] { cell });
    }

    internal void forwardS() {
        Vector3 pos = Vector3.zero;
        pos.x += HexMetrics.getFactor();
        pos.z += 0.5f;
        transform.position += Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * pos;
        tcoord = TriangleCoordinate.FromGlobalCoordinates(transform.position);
        if(instructions != null) {
            instructions.ResetInfo();
        }
        //Refresh();
    }

    internal void forward() {
        Vector3 newPos = transform.position;
        Vector3 pos = Vector3.zero;
        pos.x += HexMetrics.getFactor();
        pos.z += 0.5f;
        newPos += Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * pos;
        Debug.LogError(Name + " Pre:" + transform.position + " Post:" + newPos + " PreTC:" + tcoord + " PostTC:" + TriangleCoordinate.FromWorldCoordinates(newPos));
        tcoord = TriangleCoordinate.FromGlobalCoordinates(newPos);
        transform.position = tcoord.ToGlobalCoordinates() + new Vector3(0f, -14.95f, 0f);
        if(instructions != null) {
            instructions.ResetInfo();
        }
        //Refresh();
    }

    internal void backwardS() {
        Vector3 pos = Vector3.zero;
        pos.x += HexMetrics.getFactor();
        pos.z += 0.5f;
        transform.position -= Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * pos;
        tcoord = TriangleCoordinate.FromGlobalCoordinates(transform.position);
        if(instructions != null) {
            instructions.ResetInfo();
        }
        //Refresh();
    }

    internal void backward() {
        Vector3 newPos = transform.position;
        Vector3 pos = Vector3.zero;
        pos.x += HexMetrics.getFactor();
        pos.z += 0.5f;
        newPos -= Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * pos;
        tcoord = TriangleCoordinate.FromGlobalCoordinates(newPos);
        transform.position = tcoord.ToGlobalCoordinates() + new Vector3(0f, -14.95f, 0f);
        if(instructions != null) {
            instructions.ResetInfo();
        }
        //Refresh();
    }

    internal void right() {
        orientation = HexMetrics.mod(orientation + 60, 360);
        transform.rotation = Quaternion.Euler(0, orientation, 0);
        if(instructions != null) {
            instructions.ResetInfo();
        }
    }

    internal void left() {
        orientation = HexMetrics.mod(orientation - 60, 360);
        transform.rotation = Quaternion.Euler(0, orientation, 0);
        if(instructions != null) {
            instructions.ResetInfo();
        }
    }

    public void CreateShip(int i, int off, string name, bool isPlayer) {
        this.Name = name;
        this.IsPlayer = isPlayer;
        orientation = 0;
        nameLabel.text = name;
        CreateShip(i, off);
    }

    public void CreateShip(TriangleCoordinate pos, string name, bool isPlayer) {
        this.Name = name;
        this.IsPlayer = isPlayer;
        orientation = 0;
        nameLabel.text = name;
        CreateShip(pos);
    }

    public void CreateShip(TriangleCoordinate pos) {
        Vector3 position = pos.ToGlobalCoordinates();
        position.y = -14.95f;
        transform.position = position;

        cell = new HexCell();
        cell.localPosition = Vector3.zero;
        tcoord = TriangleCoordinate.FromGlobalCoordinates(transform.position);

        hull = GetComponentInChildren<ShipHullMesh>();
        tower = GetComponentInChildren<ShipTowerMesh>();
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
        tcoord = TriangleCoordinate.FromGlobalCoordinates(transform.position);

        hull = GetComponentInChildren<ShipHullMesh>();
        tower = GetComponentInChildren<ShipTowerMesh>();
    }
}
