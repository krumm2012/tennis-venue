using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

// [InitializeOnLoad] // 注释掉自动加载，避免干扰游戏模式
public class ImmediateWebGLBuild
{
    // 注释掉自动构建逻辑
    /*
    static ImmediateWebGLBuild()
    {
        // 延迟执行，确保Unity完全初始化
        EditorApplication.delayCall += ExecuteBuildOnce;
    }

    private static void ExecuteBuildOnce()
    {
        // 只执行一次构建
        if (buildExecuted) return;
        buildExecuted = true;

        Debug.Log("🌐 自动执行WebGL构建...");

        // 停止游戏模式（如果正在运行）
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            // 等待停止后再执行构建
            EditorApplication.delayCall += () => {
                ExecuteWebGLBuild();
            };
        }
        else
        {
            ExecuteWebGLBuild();
        }
    }
    */

    // 添加菜单项用于手动构建
    [MenuItem("Build/WebGL Build")]
    public static void ManualWebGLBuild()
    {
        Debug.Log("🌐 手动执行WebGL构建...");
        ExecuteWebGLBuild();
    }

    private static void ExecuteWebGLBuild()
    {
        try
        {
            Debug.Log("🚀 开始WebGL构建过程...");

            // 1. 配置Player Settings
            Debug.Log("⚙️ 配置Player Settings...");
            PlayerSettings.companyName = "Tennis Venue Studio";
            PlayerSettings.productName = "Tennis Venue Simulator";
            PlayerSettings.bundleVersion = "1.0.0";

            // WebGL特定设置
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.memorySize = 512;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.decompressionFallback = true;

            // 2. 设置构建选项
            Debug.Log("📋 设置构建选项...");
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
            buildOptions.locationPathName = "WebGL-Build";
            buildOptions.target = BuildTarget.WebGL;
            buildOptions.options = BuildOptions.None;

            Debug.Log($"🎯 构建目标: {buildOptions.target}");
            Debug.Log($"📁 输出路径: {buildOptions.locationPathName}");
            Debug.Log($"🎮 场景: {buildOptions.scenes[0]}");

            // 3. 执行构建
            Debug.Log("⚡ 开始WebGL构建...");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

            // 4. 检查构建结果
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("🎉 WebGL构建成功完成！");
                Debug.Log($"📁 构建路径: {Path.GetFullPath(buildOptions.locationPathName)}");
                Debug.Log($"📦 构建大小: {report.summary.totalSize / (1024 * 1024):F2} MB");
                Debug.Log($"⏱️ 构建时间: {report.summary.totalTime}");
                Debug.Log($"⚠️ 警告数量: {report.summary.totalWarnings}");

                ShowUsageInstructions();
            }
            else
            {
                Debug.LogError($"❌ WebGL构建失败！");
                Debug.LogError($"构建结果: {report.summary.result}");
                Debug.LogError($"错误数量: {report.summary.totalErrors}");
                Debug.LogError($"警告数量: {report.summary.totalWarnings}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"💥 构建过程中发生异常: {e.Message}");
            Debug.LogError($"异常详情: {e.StackTrace}");
        }
    }

    private static void ShowUsageInstructions()
    {
        Debug.Log("📖 WebGL构建完成！使用指南:");
        Debug.Log("");
        Debug.Log("🖥️ 本地测试步骤:");
        Debug.Log("1. 打开终端/命令行");
        Debug.Log("2. 进入项目目录: cd WebGL-Build");
        Debug.Log("3. 启动HTTP服务器: python -m http.server 8000");
        Debug.Log("4. 浏览器访问: http://localhost:8000");
        Debug.Log("");
        Debug.Log("🌐 部署到网络:");
        Debug.Log("• GitHub Pages: 上传WebGL-Build文件夹内容到GitHub仓库");
        Debug.Log("• Netlify: 拖拽WebGL-Build文件夹到netlify.com");
        Debug.Log("• 自己的服务器: 上传WebGL-Build文件夹内容");
        Debug.Log("");
        Debug.Log("🎮 游戏控制:");
        Debug.Log("• 空格键/鼠标左键: 发射网球");
        Debug.Log("• WASD: 摄像机移动");
        Debug.Log("• R/T/F/C/V: 切换视角");
        Debug.Log("• 滑块: 调整发球角度、速度、方向");
        Debug.Log("");
        Debug.Log("✅ Tennis Venue WebGL版本构建完成！");
    }
}