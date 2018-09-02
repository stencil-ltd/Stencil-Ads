using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        const string dir = "Assets/StreamingAssets/Bundles";
        const BuildAssetBundleOptions opts = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        
        var ios = $"{dir}/ios";
        if (!Directory.Exists(ios)) Directory.CreateDirectory(ios);
        var manifest = BuildPipeline.BuildAssetBundles(ios, opts, BuildTarget.iOS);
        Debug.Log($"iOS Bundle - {manifest.GetAssetBundleHash("stencilcrosspromo")}");
        
        var android = $"{dir}/android";
        if (!Directory.Exists(android)) Directory.CreateDirectory(android);
        manifest = BuildPipeline.BuildAssetBundles(android, opts, BuildTarget.Android);
        Debug.Log($"Android Bundle - {manifest.GetAssetBundleHash("stencilcrosspromo")}");
    }
}