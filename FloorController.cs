using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour {
	public int totalfloor = 3;
	private int currentfloor = 3;
	private List<Transform> floors;
	private bool levelMode = false;

	private void Start() {
		currentfloor = totalfloor;
		floors = new List<Transform>();
		foreach(Transform t in transform) {
			floors.Add(t);
		}
	}

	private void Update() {
		if(BuildingController.instance != null) {
			if(levelMode != BuildingController.instance.levelMode) {
				levelMode = BuildingController.instance.levelMode;
				SetFloor(currentfloor);
			}
		}
	}

	public void IncreaseFloor() {
		currentfloor++;
		currentfloor = Mathf.Max(1, currentfloor);
		currentfloor = Mathf.Min(totalfloor, currentfloor);
		SetFloor(currentfloor);
	}

	public void DecreaseFloor() {
		currentfloor--;
		currentfloor = Mathf.Max(1, currentfloor);
		currentfloor = Mathf.Min(totalfloor, currentfloor);
		SetFloor(currentfloor);
	}

	private void SetFloor(int f) {
		for(int i = 0; i < f - 1; i++) {
			floors[i].gameObject.SetActive(!levelMode);
		}

		floors[f - 1].gameObject.SetActive(true);

		for(int i = f; i < totalfloor; i++) {
			floors[i].gameObject.SetActive(false);
		}
	}
}
