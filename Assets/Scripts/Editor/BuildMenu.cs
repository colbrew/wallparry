using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

// Output the build size or a failure depending on BuildPlayer.

public class BuildMenu : MonoBehaviour
{
    [MenuItem("Build/Build and Run iOS")]
    public static void iOSBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/TestLevel.unity"};
        buildPlayerOptions.locationPathName = "Builds/iOSBuild";
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.AutoRunPlayer;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize / 1024000 + " megabytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    [MenuItem("Build/Build and Run Android")]
    public static void AndroidBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/TestLevel.unity" };
        buildPlayerOptions.locationPathName = "Builds/AndroidBuild";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.AutoRunPlayer;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize / 1024000 + " megabytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}