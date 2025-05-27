using UnityEngine;
using UnityEditor;

public class WebGLSupportChecker
{
    [MenuItem("Tennis Venue/Check WebGL Support")]
    public static void CheckWebGLSupport()
    {
        Debug.Log("🔍 检查WebGL支持状态...");
        
        // 检查WebGL是否可用
        bool webglSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        
        if (webglSupported)
        {
            Debug.Log("✅ WebGL Build Support已安装并可用！");
            Debug.Log($"📱 当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");
            
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                Debug.Log("🔄 切换到WebGL构建目标...");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
                Debug.Log("✅ 已切换到WebGL构建目标");
            }
            else
            {
                Debug.Log("✅ 当前已是WebGL构建目标");
            }
            
            Debug.Log("🚀 准备执行WebGL构建...");
            ExecuteWebGLBuild();
        }
        else
        {
            Debug.LogError("❌ WebGL Build Support未安装！");
            ShowInstallInstructions();
        }
    }
    
    private static void ExecuteWebGLBuild()
    {
        Debug.Log("🌐 开始WebGL构建...");
        
        try
        {
            // 配置Player Settings
            PlayerSettings.companyName = "Tennis Venue Studio";
            PlayerSettings.productName = "Tennis Venue Simulator";
            PlayerSettings.bundleVersion = "1.0.0";
            
            // WebGL特定设置
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.memorySize = 512;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.decompressionFallback = true;
            
            // 设置构建选项
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
            buildOptions.locationPathName = "WebGL-Build";
            buildOptions.target = BuildTarget.WebGL;
            buildOptions.options = BuildOptions.None;
            
            Debug.Log("📋 构建配置:");
            Debug.Log($"   目标平台: {buildOptions.target}");
            Debug.Log($"   输出路径: {buildOptions.locationPathName}");
            Debug.Log($"   场景: {buildOptions.scenes[0]}");
            
            // 执行构建
            var report = BuildPipeline.BuildPlayer(buildOptions);
            
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log("🎉 WebGL构建成功完成！");
                Debug.Log($"📁 构建路径: {System.IO.Path.GetFullPath(buildOptions.locationPathName)}");
                Debug.Log($"📦 构建大小: {report.summary.totalSize / (1024 * 1024):F2} MB");
                Debug.Log($"⏱️ 构建时间: {report.summary.totalTime}");
                
                ShowUsageInstructions();
            }
            else
            {
                Debug.LogError($"❌ WebGL构建失败: {report.summary.result}");
                Debug.LogError($"错误数量: {report.summary.totalErrors}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"💥 构建异常: {e.Message}");
        }
    }
    
    private static void ShowInstallInstructions()
    {
        Debug.LogWarning("📋 安装WebGL Build Support的步骤:");
        Debug.LogWarning("1. 打开Unity Hub");
        Debug.LogWarning("2. 找到Unity 2022.3.57f1c2版本");
        Debug.LogWarning("3. 点击版本右侧的设置齿轮图标");
        Debug.LogWarning("4. 选择'添加模块'");
        Debug.LogWarning("5. 勾选'WebGL Build Support'");
        Debug.LogWarning("6. 点击'安装'");
        Debug.LogWarning("7. 安装完成后重新打开Unity项目");
        Debug.LogWarning("8. 再次运行此检查");
    }
    
    private static void ShowUsageInstructions()
    {
        Debug.Log("📖 WebGL构建完成！使用指南:");
        Debug.Log("");
        Debug.Log("🖥️ 本地测试:");
        Debug.Log("1. 打开终端/命令行");
        Debug.Log("2. cd WebGL-Build");
        Debug.Log("3. python -m http.server 8000");
        Debug.Log("4. 浏览器访问: http://localhost:8000");
        Debug.Log("");
        Debug.Log("🌐 部署选项:");
        Debug.Log("• GitHub Pages: 上传WebGL-Build文件夹内容");
        Debug.Log("• Netlify: 拖拽WebGL-Build文件夹到netlify.com");
        Debug.Log("• 自己的服务器: 上传所有文件");
        Debug.Log("");
        Debug.Log("🎮 游戏控制:");
        Debug.Log("• 空格键/鼠标左键: 发射网球");
        Debug.Log("• WASD: 摄像机移动");
        Debug.Log("• R/T/F/C/V: 切换视角");
    }
}