using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Linq;

/// <summary>
/// Editor utility that builds the Unity project for WebGL platform.
/// This script can be executed from the Unity Editor menu under "Build/Build WebGL".
/// It gathers all enabled scenes from the build settings, sets the build path, and initiates the build process.
/// The build output will be located in the "webgl-dist" directory in the project root.
/// </summary>
public class WebGLBuilder
{
    /// <summary>
    /// Builds the Unity project for WebGL using the current build settings.
    /// This method is accessible from the Unity Editor menu under "Build/Build WebGL".
    /// Uses only the enabled scenes from the build settings and outputs to "webgl-dist".
    /// </summary>
    [MenuItem("Build/Build WebGL")]
    public static void BuildProject()
    {
        // Output directory for the WebGL build
        string buildPath = "webgl-dist";

        // Configure build options
        var options = new BuildPlayerOptions
        {
            // Only include scenes that are enabled in the build settings
            scenes = EditorBuildSettings.scenes
                     .Where(s => s.enabled)
                     .Select(s => s.path)
                     .ToArray(),
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            // Use default build options (no development build, compression, etc.)
            options = BuildOptions.None
        };

        // Execute the build and get the report
        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary; // Contains size, result, and other build information

        // Log appropriate message based on build result
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"WebGL build succeeded: {summary.totalSize} bytes");
        }
        else
        {
            Debug.LogError($"WebGL build failed: {summary.result}");
        }
    }
}