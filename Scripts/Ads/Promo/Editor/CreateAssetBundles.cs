using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        const string dir = "AssetBundles";
        
        var ios = $"{dir}/ios";
        if (!Directory.Exists(ios)) Directory.CreateDirectory(ios);
        var manifest = BuildPipeline.BuildAssetBundles(ios, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.iOS);
        Debug.Log($"iOS Bundle - {manifest.GetAssetBundleHash("stencilcrosspromo")}");
        
        var android = $"{dir}/android";
        if (!Directory.Exists(android)) Directory.CreateDirectory(android);
        manifest = BuildPipeline.BuildAssetBundles(android, BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.Android);
        Debug.Log($"Android Bundle - {manifest.GetAssetBundleHash("stencilcrosspromo")}");
    }
}