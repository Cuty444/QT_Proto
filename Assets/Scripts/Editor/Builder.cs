using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;

class Builder
{
    private const string ScriptingDefine = "AMPLIFY_SHADER_EDITOR";
    private const string TestScriptingDefine = "AMPLIFY_SHADER_EDITOR;Testing";
    
    private static readonly string BuildSettingPath = $"{Directory.GetCurrentDirectory()}/BuildSetting.config";

    
    [MenuItem("Build/테스트 모드 활성화", false, 1000)]
    public static void EnableTestMode()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, TestScriptingDefine);
    }
    
    [MenuItem("Build/테스트 모드 비활성화", false, 1001)]
    public static void DisableTestMode()
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, ScriptingDefine);
    }
    
    [MenuItem("Build/빌드하기", false, 10)]
    public static void Build()
    {
        var stream = new FileStream(BuildSettingPath,FileMode.Open);
        var reader = new StreamReader(stream);

        var dir = $"{reader.ReadLine()}/{PlayerSettings.productName}_{DateTime.Now.ToString("MMddHHmm")}/{PlayerSettings.productName}.exe";

        reader.Close();

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, ScriptingDefine);
        
        GenericBuild(FindEnabledEditorScenes(), dir, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows, BuildOptions.None);
    }
    
    [MenuItem("Build/테스트 빌드하기", false, 11)]
    public static void TestBuild()
    {
        var stream = new FileStream(BuildSettingPath,FileMode.Open);
        var reader = new StreamReader(stream);

        var dir = $"{reader.ReadLine()}/{PlayerSettings.productName}_Develop_{DateTime.Now.ToString("MMddHHmm")}/{PlayerSettings.productName}.exe";
        
        reader.Close();

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, TestScriptingDefine);
        
        GenericBuild(FindEnabledEditorScenes(), dir, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    public static void GenericBuild(string[] scenes, string target_dir, BuildTargetGroup build_group, BuildTarget build_target, BuildOptions build_options, bool isTesting = true)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_group, build_target);
        var res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);

        if (res.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + res.summary.totalSize + " bytes");
        }
        else if (res.summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    private static string[] FindEnabledEditorScenes()
    {
        var scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
            {
                continue;
            }

            scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }
}