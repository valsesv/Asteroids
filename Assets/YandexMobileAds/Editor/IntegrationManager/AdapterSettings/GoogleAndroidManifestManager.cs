using UnityEngine;
using System.IO;
using System.Xml;

public static class AndroidManifestManager
{
    private const string ANDROID_NAMESPACE = "http://schemas.android.com/apk/res/android";
    private const string MANIFEST_PATH = "Assets/Plugins/Android/AndroidManifest.xml";
    
    public static void AddAppIDToManifest(string appID)
    {
        if (!File.Exists(MANIFEST_PATH))
        {
            CreateBaseManifestWithAppID(appID);
            Debug.Log("AndroidManifest.xml not found. A new file has been created with the base structure and your key.");
            return;
        }

        UpdateExistingManifest(appID);
    }

    private static void CreateBaseManifestWithAppID(string appID)
    {
        string baseManifest = 
@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android""
    xmlns:tools=""http://schemas.android.com/tools""
    package=""com.unity3d.player"">

    <application>
        <activity android:name=""com.unity3d.player.UnityPlayerActivity""
                  android:theme=""@style/UnityThemeSelector"">
            <intent-filter>
                <action android:name=""android.intent.action.MAIN"" />
                <category android:name=""android.intent.category.LAUNCHER"" />
            </intent-filter>
            <meta-data android:name=""unityplayer.UnityActivity"" android:value=""true"" />
        </activity>
    </application>
</manifest>";

        Directory.CreateDirectory(Path.GetDirectoryName(MANIFEST_PATH));
        File.WriteAllText(MANIFEST_PATH, baseManifest);

        UpdateExistingManifest(appID);
    }

    private static void UpdateExistingManifest(string appID)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(MANIFEST_PATH);

        XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
        nsMgr.AddNamespace("android", ANDROID_NAMESPACE);

        XmlNode applicationNode = doc.SelectSingleNode("/manifest/application");
        if (applicationNode == null)
        {
            Debug.LogWarning("No <application> node found in AndroidManifest.xml. A new one will be created.");
            applicationNode = doc.CreateElement("application");
            doc.DocumentElement.AppendChild(applicationNode);
        }

        XmlNode metaDataNode = applicationNode.SelectSingleNode("meta-data[@android:name='com.google.android.gms.ads.APPLICATION_ID']", nsMgr);

        if (metaDataNode == null)
        {
            XmlElement metaElement = doc.CreateElement("meta-data");
            metaElement.SetAttribute("name", ANDROID_NAMESPACE, "com.google.android.gms.ads.APPLICATION_ID");
            metaElement.SetAttribute("value", ANDROID_NAMESPACE, appID);
            applicationNode.AppendChild(metaElement);
            Debug.Log("Added new meta-data for AdMob APPLICATION_ID in AndroidManifest.xml.");
        }
        else
        {
            XmlElement metaElement = (XmlElement)metaDataNode;
            metaElement.SetAttribute("value", ANDROID_NAMESPACE, appID);
            Debug.Log("Updated existing meta-data for AdMob APPLICATION_ID in AndroidManifest.xml.");
        }

        doc.Save(MANIFEST_PATH);
        Debug.Log("AndroidManifest.xml successfully updated.");
    }
}
