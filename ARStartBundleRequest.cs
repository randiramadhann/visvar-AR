using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class ARStartBundleRequest : MonoBehaviour {
	public static ARStartBundleRequest instance;
	public string[] assetNames;
	public Dictionary<string, float> assetProgress = new Dictionary<string, float>();
	private string uriHead = "https://firebasestorage.googleapis.com/v0/b/ar-archilogy-235e4.appspot.com/o/";
	private string uriTail = "?alt=media";

	private void Awake() {
		if(instance == null) {
			instance = this;
		}
		else if(instance != this) {
			Destroy(gameObject);
		}
	}

	private void Start() {
		foreach(string assetName in assetNames) {
			assetProgress.Add(assetName, 0f);

			string localPath = Path.Combine(Application.persistentDataPath, assetName);
			if(!File.Exists(localPath)) {
				Debug.Log("Model "+assetName+" not found locally, downloading...");
				StartCoroutine(StartDownload(localPath, assetName));
			}
			else {
				Debug.Log("Model " + assetName + " found locally, loading...");
				assetProgress[assetName] = 1f;
				StartCoroutine(StartReadLocal(localPath, assetName));
			}
		}
	}

	private IEnumerator StartReadLocal(string localPath, string name) {
		AssetBundleCreateRequest lAssetRequest = AssetBundle.LoadFromFileAsync(localPath);
		yield return lAssetRequest;

		AssetBundle lAsset = lAssetRequest.assetBundle;
		if(lAsset == null) {
			Debug.LogError("Failed to load AssetBundle");
			yield break;
		}

		StartCoroutine(AssetLoader(lAsset, name));
	}

	private IEnumerator StartDownload(string localPath, string name) {
		string uri = uriHead + name + uriTail;
		UnityWebRequest webRequest = UnityWebRequest.Get(uri);

		UnityWebRequestAsyncOperation webOps = webRequest.SendWebRequest();

		while(!webOps.isDone) {
			assetProgress[name] = webRequest.downloadProgress;
			yield return null;
		}
		assetProgress[name] = 1f;

		byte[] assetBundleData = webRequest.downloadHandler.data;
		AssetBundleCreateRequest localAssetBundleRequest = AssetBundle.LoadFromMemoryAsync(assetBundleData);
		yield return localAssetBundleRequest;

		AssetBundle localAssetBundle = localAssetBundleRequest.assetBundle;

		if(localAssetBundle == null) {
			Debug.LogError("Failed to load AssetBundle");
			yield break;
		}
		File.WriteAllBytes(localPath, webRequest.downloadHandler.data);

		StartCoroutine(AssetLoader(localAssetBundle, name));
	}

	private IEnumerator AssetLoader(AssetBundle bundle, string name) {
		AssetBundleRequest assetRequest = bundle.LoadAssetAsync<GameObject>(name);
		yield return assetRequest;

		GameObject asset = assetRequest.asset as GameObject;
		GameObject displayedAsset = Instantiate(asset);
		displayedAsset.name = name;
		displayedAsset.SetActive(false);

		if(name == "f_mal_shophouse" || name == "f_mal_tic" || name == "f_mal_cohouse" || name == "f_mal_senthir" || name == "f_wb_cohouse" || name == "f_wb_homestay" || name == "f_wb_museum" || name == "f_wb_rest") {
			displayedAsset.transform.Rotate(0f, 180f, 0f, Space.Self);
		}
		else if(name == "f_wb_social") {
			displayedAsset.transform.Rotate(0f, 90f, 0f, Space.Self);
		}

		BuildingController.instance.spawnedRumah.Add(name, displayedAsset);

		bundle.Unload(false);
	}
}
