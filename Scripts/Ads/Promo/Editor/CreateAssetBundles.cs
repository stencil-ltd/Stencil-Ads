using System.IO;
using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        const string dir = "AssetBundles";
        const BuildAssetBundleOptions opts = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        
        var ios = $"{dir}/ios";
        if (!Directory.Exists(ios)) Directory.CreateDirectory(ios);
        BuildPipeline.BuildAssetBundles(ios, opts, BuildTarget.iOS);
        
        var android = $"{dir}/android";
        if (!Directory.Exists(android)) Directory.CreateDirectory(android);
        BuildPipeline.BuildAssetBundles(android, opts, BuildTarget.Android);
    }
}