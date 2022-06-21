using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;


using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class CustomBuild 
{

    
    static int MajorVerion = 1;// 대규모 업데이트 
    static int MinorVerion = 1;// 기존버전에 새로운 기능 추가
    static int BuildVerion = 1;// 여기는 빌드 할때 마다 증가한다
    static int VerionCode = 10;// 그냥 증가

    // 여기에 실제 기입을 해야 됨
    static string LOCALIP = "192.168.0.8";
    static string SERVERIP2 = "blockaircraft.com";
    static string SERVERIP = "118.27.107.133";






    static void MakeGameFile(bool bRelease)
    {
        // 기본적인 config 파일을 만든다
        // 서버 IP  버전등을 만든다 

        CWJSon kjSon = new CWJSon();

        if (bRelease)
        {
            kjSon.Add("ServerIP", SERVERIP);
            kjSon.Add("ServerIP2", SERVERIP2);
            kjSon.Add("Release", 1);// 릴리즈
        }
        else
        {
            kjSon.Add("ServerIP", LOCALIP);
            kjSon.Add("ServerIP2", LOCALIP);
            kjSon.Add("Release",0);// 내부테스트
        }

        
        kjSon.SaveFile("Config");
    }


    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }
    [MenuItem("Build/Android/대규모업데이트")]
    public static void IncreaseMajor()
    {
        MinorVerion = EditorPrefs.GetInt("MinorVerion");
        MajorVerion = EditorPrefs.GetInt("MajorVerion");
        MajorVerion++;
        MinorVerion = 0;
        BuildVerion = 0;
        EditorPrefs.SetInt("MajorVerion", MajorVerion);
        EditorPrefs.SetInt("MinorVerion", MinorVerion);
        EditorPrefs.SetInt("BuildVerion", BuildVerion);
        AndroidBuild();
    }

    [MenuItem("Build/Android/기능추가")]
    public static void IncreaseMinor()
    {


        MinorVerion = EditorPrefs.GetInt("MinorVerion");
        MajorVerion = EditorPrefs.GetInt("MajorVerion");

        MinorVerion++;
        BuildVerion = 0;
        EditorPrefs.SetInt("MinorVerion", MinorVerion);
        EditorPrefs.SetInt("BuildVerion", BuildVerion);
        AndroidBuild();
    }

    static void MakeVersion()
    {


        BuildVerion = EditorPrefs.GetInt("BuildVerion");
        BuildVerion++;
        EditorPrefs.SetInt("BuildVerion", BuildVerion);


        EditorUserBuildSettings.buildAppBundle = true;

        VerionCode = EditorPrefs.GetInt("VerionCode");
        VerionCode++;
        if (VerionCode < 400)
        {
            VerionCode = 400;
        }
        EditorPrefs.SetInt("VerionCode", VerionCode);


        PlayerSettings.bundleVersion = string.Format("{0}.{1}.{2}", MajorVerion, MinorVerion, BuildVerion);

        if(PlayerSettings.Android.bundleVersionCode>= VerionCode)
        {
            VerionCode = PlayerSettings.Android.bundleVersionCode + 1;
        }
        PlayerSettings.Android.bundleVersionCode = VerionCode;
    }
    static void MakeBuild()
    {

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = new[] {  "Assets/SpaceBlock/Scenes/GLogin.unity", "Assets/SpaceBlock/Scenes/RGame.unity" };


        buildPlayerOptions.locationPathName = string.Format("Build/Blockaircraft_{0}_{1}_{2}.aab", MajorVerion, MinorVerion, BuildVerion);
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;



        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }

    }
    public static void AndroidBuild()
    {
        MakeVersion();
        MakeGameFile(true);
        MakeBuild();
    }

    [MenuItem("Build/Android/버그수정")]
    public static void DebugBuild()
    {
        MinorVerion = EditorPrefs.GetInt("MinorVerion");
        MajorVerion = EditorPrefs.GetInt("MajorVerion");

        AndroidBuild();
    }


    [MenuItem("Build/Android/Test")]
    public static void AndroidBuildTest()
    {
        MakeVersion();
        MakeGameFile(false);
        EditorUserBuildSettings.buildAppBundle = false;
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        
        buildPlayerOptions.scenes = new[] { "Assets/SpaceBlock/Scenes/GLogin.unity", "Assets/SpaceBlock/Scenes/RGame.unity" };


        //buildPlayerOptions.locationPathName = "Build/test.apk";
        //PlayerSettings.bundleVersion = string.Format("{0}.{1}.{2}", MajorVerion, MinorVerion, BuildVerion);
        //Z:\HDD1\Data\공유폴더\Build
        //buildPlayerOptions.locationPathName = string.Format("Build/test_{0}_{1}_{2}.apk", MajorVerion, MinorVerion, BuildVerion);
        buildPlayerOptions.locationPathName = string.Format("Z:/HDD1/Data/공유폴더/Build/test_{0}_{1}_{2}.apk", MajorVerion, MinorVerion, BuildVerion);

        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }



    //[MenuItem("Build/Build PC")]
    [MenuItem("Build/PC/Release")]
    public static void PCBuildRelase()
    {
        MakeGameFile(true);
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();


        buildPlayerOptions.scenes = new[] { "Assets/SpaceBlock/Scenes/GLogin.unity", "Assets/SpaceBlock/Scenes/RGame.unity" };

        buildPlayerOptions.locationPathName = "PCBuild/test.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    [MenuItem("Build/PC/Test")]
    public static void PCBuildTest()
    {
        MakeGameFile(false);
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/SpaceBlock/Scenes/GLogin.unity", "Assets/SpaceBlock/Scenes/RGame.unity" };


        buildPlayerOptions.locationPathName = "PCBuild/test.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

}
