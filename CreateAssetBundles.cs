using UnityEngine;
using UnityEditor;
using System.IO;


public class CreateAssetBundles {
	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles() {
		BuildPipeline.BuildAssetBundles(Application.persistentDataPath + "/localdevassets", BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
	}
}
