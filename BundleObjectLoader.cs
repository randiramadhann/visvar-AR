using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class BundleObjectLoader : MonoBehaviour {
	public string assetName = "d_mal_shophouse";
	private string uriHead = "https://firebasestorage.googleapis.com/v0/b/ar-archilogy-235e4.appspot.com/o/";
	private string uriTail = "?alt=media";
	private string localPath;

	private void Start() {
		localPath = Path.Combine(Application.persistentDataPath, assetName);
		if(!File.Exists(localPath)) {
			Debug.Log("Model not found locally, downloading...");
			StartCoroutine(StartDownload());
		}
		else {
			Debug.Log("Model found locally, loading...");
			StartCoroutine(StartReadLocal());
		}
	}

	private IEnumerator StartReadLocal() {
		AssetBundleCreateRequest lAssetRequest = AssetBundle.LoadFromFileAsync(localPath);
		yield return lAssetRequest;

		AssetBundle lAsset = lAssetRequest.assetBundle;
		if(lAsset == null) {
			Debug.LogError("Failed to load AssetBundle");
			yield break;
		}

		StartCoroutine(AssetLoader(lAsset, assetName));
	}

	private IEnumerator StartDownload() {
		string uri = uriHead + assetName + uriTail;
		UnityWebRequest webRequest = UnityWebRequest.Get(uri);
		yield return webRequest.SendWebRequest();

		byte[] assetBundleData = webRequest.downloadHandler.data;
		AssetBundleCreateRequest localAssetBundleRequest = AssetBundle.LoadFromMemoryAsync(assetBundleData);
		yield return localAssetBundleRequest;

		AssetBundle localAssetBundle = localAssetBundleRequest.assetBundle;

		if(localAssetBundle == null) {
			Debug.LogError("Failed to load AssetBundle");
			yield break;
		}
		File.WriteAllBytes(localPath, webRequest.downloadHandler.data);

		StartCoroutine(AssetLoader(localAssetBundle, assetName));
	}

	private IEnumerator AssetLoader(AssetBundle bundle, string name) {
		AssetBundleRequest assetRequest = bundle.LoadAssetAsync<GameObject>(name);
		yield return assetRequest;

		GameObject asset = assetRequest.asset as GameObject;
		GameObject displayedAsset = Instantiate(asset);
		displayedAsset.name = assetName;
		bundle.Unload(false);
	}
}
