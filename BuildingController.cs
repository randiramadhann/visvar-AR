using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using GoogleARCore;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour {
	public static BuildingController instance;
	public bool levelMode = false;

	public GameObject FitToScanOverlay;

	public Dictionary<string, GameObject> spawnedRumah = new Dictionary<string, GameObject>();

	private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

	public void Awake() {
		if(instance == null) {
			instance = this;
		}
		else if(instance != this) {
			Destroy(gameObject);
		}
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
	}

	public void Update() {
		if(Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}

		if(Session.Status != SessionStatus.Tracking) {
			Screen.sleepTimeout = SleepTimeout.SystemSetting;
		}
		else {
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

		foreach(var image in m_TempAugmentedImages) {
			bool canFindObject = spawnedRumah.TryGetValue(image.Name, out GameObject rumahVisualizer);

			if(!canFindObject) {
				if(image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking) {
					ProgressController.Status(image.Name);
				}
				continue;
			}

			if(image.TrackingState == TrackingState.Tracking) {
				if(!rumahVisualizer.activeSelf) {
					Anchor anchor = image.CreateAnchor(image.CenterPose);
					rumahVisualizer.SetActive(true);
				}
				rumahVisualizer.transform.position = image.CenterPose.position;
				rumahVisualizer.transform.rotation = image.CenterPose.rotation;
			}
			else if(image.TrackingState == TrackingState.Stopped) {
				if(rumahVisualizer.activeSelf) {
					rumahVisualizer.SetActive(false);
				}
			}
		}

		bool showScan = true;
		foreach(var visualizer in spawnedRumah.Values) {
			if(visualizer.activeSelf) {
				showScan = false;
			}
		}

		FitToScanOverlay.SetActive(showScan);
	}

	public void IncreaseFloor() {
		foreach(var visualizer in spawnedRumah.Values) {
			if(visualizer.activeSelf) {
				FloorController fc = visualizer.GetComponent<FloorController>();
				if(fc != null) {
					fc.IncreaseFloor();
				}
			}
		}
	}

	public void ToggleMode() {
		levelMode = !levelMode;
	}

	public void DecreaseFloor() {
		foreach(var visualizer in spawnedRumah.Values) {
			if(visualizer.activeSelf) {
				FloorController fc = visualizer.GetComponent<FloorController>();
				if(fc != null) {
					fc.DecreaseFloor();
				}
			}
		}
	}
}
