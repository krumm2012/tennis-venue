using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

public class SimpleWebGLBuilder
{
    [MenuItem("Tennis Venue/Build WebGL")]
    public static void BuildWebGL()
    {
        Debug.Log("🌐 开始Tennis Venue WebGL构建...");
        
        // 配置WebGL设置
        ConfigureWebGLSettings();
        
        // 设置构建参数
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = "WebGL-Build";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;
        
        Debug.Log("🚀 开始构建...");
        
        // 执行构建
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        // 检查结果
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("✅ WebGL构建成功！");
            Debug.Log($"📁 输出路径: {Path.GetFullPath("WebGL-Build")}");
            Debug.Log($"📦 构建大小: {report.summary.totalSize / (1024 * 1024):F2} MB");
            Debug.Log($"⏱️ 构建时间: {report.summary.totalTime}");
            
            ShowInstructions();
        }
        else
        {
            Debug.LogError($"❌ 构建失败: {report.summary.result}");
        }
    }
    
    private static void ConfigureWebGLSettings()
    {
        Debug.Log("⚙️ 配置WebGL设置...");
        
        PlayerSettings.companyName = "Tennis Venue Studio";
        PlayerSettings.productName = "Tennis Venue Simulator";
        PlayerSettings.bundleVersion = "1.0.0";
        
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.memorySize = 512;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.decompressionFallback = true;
        
        Debug.Log("✅ WebGL设置配置完成");
    }
    
    private static void ShowInstructions()
    {
        Debug.Log("📖 构建完成！使用说明:");
        Debug.Log("1. 本地测试:");
        Debug.Log("   cd WebGL-Build");
        Debug.Log("   python -m http.server 8000");
        Debug.Log("   访问: http://localhost:8000");
        Debug.Log("2. 部署: 上传WebGL-Build文件夹到网络服务器");
    }
}