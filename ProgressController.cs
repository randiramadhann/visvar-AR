using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressController : MonoBehaviour {

	private static ProgressController instance;
	public string assetName = "";
	public GameObject panelObject;
	public TMP_Text panelText;
	public TMP_Text panelPercent;
	public Slider panelSlider;

	public static void Status(string name) {
		bool pget = ARStartBundleRequest.instance.assetProgress.ContainsKey(name);
		if(pget) {
			instance.assetName = name;
			instance.panelText.text = "Sedang mengunduh model \"" + name + "\"";
			instance.panelObject.SetActive(true);
		}
	}

	private void Awake() {
		if(instance == null) {
			instance = this;
		}
		else if(instance != this) {
			Destroy(gameObject);
		}
	}

	private void Update() {
		ARStartBundleRequest.instance.assetProgress.TryGetValue(assetName, out float progress);
		panelSlider.value = progress;
		panelPercent.text = (Mathf.RoundToInt(progress * 100f)) + "%";
		if(Mathf.Approximately(1f, progress)) {
			panelObject.SetActive(false);
		}
	}
}
