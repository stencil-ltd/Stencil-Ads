using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using Ads;

[InitializeOnLoad]
public class StencilAdPlist
{
    static StencilAdPlist()
    {}

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

        if (buildTarget == BuildTarget.iOS) {
   
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
   
            // Get root
            PlistElementDict rootDict = plist.root;
            rootDict.SetString("GADApplicationIdentifier", AdSettings.Instance.AppId.Ios);

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}