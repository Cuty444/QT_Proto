using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;

class Builder
{
    private static string BuildSettingPath = $"{Directory.GetCurrentDirectory()}/BuildSetting.config";

    [MenuItem("Build/빌드하기")]
    public static void Build()
    {
        var stream = new FileStream(BuildSettingPath,FileMode.Open);
        var reader = new StreamReader(stream);

        var dir = $"{reader.ReadLine()}/{PlayerSettings.productName}_{DateTime.Now.ToString("MMddHHmm")}/{PlayerSettings.productName}.exe";

        reader.Close();

        GenericBuild(FindEnabledEditorScenes(), dir, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    public static void GenericBuild(string[] scenes, string target_dir, BuildTargetGroup build_group, BuildTarget build_target, BuildOptions build_options)
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