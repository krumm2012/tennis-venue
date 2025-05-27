using UnityEngine;
using UnityEditor;

public class DirectWebGLTest
{
    [MenuItem("Tools/Test WebGL Support")]
    public static void TestWebGLSupport()
    {
        Debug.Log("🔍 直接测试WebGL支持...");

        // 检查WebGL支持
        bool webglSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        Debug.Log($"WebGL支持状态: {(webglSupported ? "✅ 支持" : "❌ 不支持")}");

        // 检查当前构建目标
        Debug.Log($"当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");

        if (webglSupported)
        {
            Debug.Log("🎉 WebGL Build Support已安装！");
            Debug.Log("🚀 可以进行WebGL构建");

            // 尝试切换到WebGL平台
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                Debug.Log("🔄 切换到WebGL构建目标...");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            }
        }
        else
        {
            Debug.LogError("❌ WebGL Build Support未安装！");
            Debug.LogWarning("请通过Unity Hub安装WebGL Build Support模块");
        }
    }

    [MenuItem("Tools/Start WebGL Build")]
    public static void StartWebGLBuild()
    {
        Debug.Log("🌐 开始WebGL构建...");

        // 检查WebGL支持
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL))
        {
            Debug.LogError("❌ WebGL不支持，请先安装WebGL Build Support模块");
            return;
        }

        try
        {
            // 配置构建选项
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
            buildOptions.locationPathName = "WebGL-Build";
            buildOptions.target = BuildTarget.WebGL;
            buildOptions.options = BuildOptions.None;

            Debug.Log("📋 构建配置:");
            Debug.Log($"   场景: {buildOptions.scenes[0]}");
            Debug.Log($"   输出: {buildOptions.locationPathName}");
            Debug.Log($"   平台: {buildOptions.target}");

            // 执行构建
            var report = BuildPipeline.BuildPlayer(buildOptions);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log("🎉 WebGL构建成功！");
                Debug.Log($"📁 构建路径: {System.IO.Path.GetFullPath(buildOptions.locationPathName)}");
                Debug.Log($"📦 构建大小: {report.summary.totalSize / (1024 * 1024):F2} MB");
                Debug.Log($"⏱️ 构建时间: {report.summary.totalTime}");

                Debug.Log("📖 使用说明:");
                Debug.Log("1. cd WebGL-Build");
                Debug.Log("2. python -m http.server 8000");
                Debug.Log("3. 浏览器访问: http://localhost:8000");
            }
            else
            {
                Debug.LogError($"❌ WebGL构建失败: {report.summary.result}");
                Debug.LogError($"错误数量: {report.summary.totalErrors}");
                Debug.LogError($"警告数量: {report.summary.totalWarnings}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"💥 构建异常: {e.Message}");
        }
    }
}