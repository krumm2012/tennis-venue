using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

public class WebGLBuilder
{
    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        Debug.Log("🌐 开始WebGL构建流程...");
        
        // 1. 设置构建场景
        string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        
        // 2. 检查场景文件是否存在
        foreach (string scene in scenes)
        {
            if (!File.Exists(scene))
            {
                Debug.LogError($"❌ 场景文件不存在: {scene}");
                return;
            }
        }
        
        // 3. 设置WebGL构建选项
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = "WebGL-Build";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;
        
        Debug.Log("📋 构建设置:");
        Debug.Log($"   目标平台: WebGL");
        Debug.Log($"   输出路径: WebGL-Build");
        Debug.Log($"   场景数量: {scenes.Length}");
        
        // 4. 配置WebGL Player Settings
        ConfigureWebGLSettings();
        
        // 5. 开始构建
        Debug.Log("🚀 开始构建WebGL版本...");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        // 6. 检查构建结果
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("✅ WebGL构建成功完成！");
            Debug.Log($"📁 构建输出: {Path.GetFullPath("WebGL-Build")}");
            Debug.Log($"⏱️ 构建时间: {report.summary.totalTime}");
            Debug.Log($"📦 构建大小: {report.summary.totalSize} bytes");
            
            // 显示后续步骤提示
            ShowNextSteps();
        }
        else
        {
            Debug.LogError($"❌ WebGL构建失败: {report.summary.result}");
            if (report.summary.totalErrors > 0)
            {
                Debug.LogError($"错误数量: {report.summary.totalErrors}");
            }
        }
    }
    
    private static void ConfigureWebGLSettings()
    {
        Debug.Log("⚙️ 配置WebGL Player Settings...");
        
        // 基本设置
        PlayerSettings.companyName = "Tennis Venue Studio";
        PlayerSettings.productName = "Tennis Venue Simulator";
        PlayerSettings.bundleVersion = "1.0.0";
        
        // WebGL特定设置
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.memorySize = 512; // 512MB内存
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None; // 最佳性能
        PlayerSettings.WebGL.dataCaching = true; // 启用数据缓存
        PlayerSettings.WebGL.decompressionFallback = true; // 兼容性
        
        // 质量设置优化
        QualitySettings.SetQualityLevel(1); // Fast质量级别
        
        Debug.Log("✅ WebGL设置配置完成");
    }
    
    private static void ShowNextSteps()
    {
        Debug.Log("📖 后续步骤:");
        Debug.Log("1. 启动本地服务器测试:");
        Debug.Log("   cd WebGL-Build");
        Debug.Log("   python -m http.server 8000");
        Debug.Log("2. 浏览器访问: http://localhost:8000");
        Debug.Log("3. 部署到网络:");
        Debug.Log("   - GitHub Pages: 上传到GitHub仓库并启用Pages");
        Debug.Log("   - Netlify: 拖拽WebGL-Build文件夹到netlify.com");
    }
    
    [MenuItem("Build/Configure WebGL Settings Only")]
    public static void ConfigureWebGLOnly()
    {
        Debug.Log("⚙️ 仅配置WebGL设置（不构建）...");
        ConfigureWebGLSettings();
        Debug.Log("✅ WebGL设置配置完成，可以手动构建了");
    }
    
    [MenuItem("Build/Open Build Folder")]
    public static void OpenBuildFolder()
    {
        string buildPath = Path.GetFullPath("WebGL-Build");
        if (Directory.Exists(buildPath))
        {
            EditorUtility.RevealInFinder(buildPath);
            Debug.Log($"📁 打开构建文件夹: {buildPath}");
        }
        else
        {
            Debug.LogWarning("⚠️ 构建文件夹不存在，请先执行构建");
        }
    }
}