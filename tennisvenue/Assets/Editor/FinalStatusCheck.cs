using UnityEngine;
using UnityEditor;

public class FinalStatusCheck
{
    [MenuItem("Tools/Final Status Check")]
    public static void CheckFinalStatus()
    {
        Debug.Log("=== 🎾 Tennis Venue WebGL 最终状态检查 ===");

        // 1. 检查WebGL支持
        bool webglSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        Debug.Log($"WebGL支持: {(webglSupported ? "✅ 已安装" : "❌ 未安装")}");

        // 2. 检查当前构建目标
        Debug.Log($"当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");

        // 3. 检查编译状态
        Debug.Log($"编译状态: {(EditorApplication.isCompiling ? "🔄 编译中" : "✅ 编译完成")}");

        // 4. 检查场景
        string[] scenes = EditorBuildSettings.scenes.Length > 0 ? 
            System.Array.ConvertAll(EditorBuildSettings.scenes, scene => scene.path) :
            new string[] { "Assets/Scenes/SampleScene.unity" };
        
        Debug.Log($"构建场景: {string.Join(", ", scenes)}");

        // 5. 检查关键组件
        BallLauncher launcher = Object.FindObjectOfType<BallLauncher>();
        Debug.Log($"BallLauncher组件: {(launcher != null ? "✅ 找到" : "❌ 未找到")}");

        if (launcher != null)
        {
            Debug.Log($"  - ballPrefab: {(launcher.ballPrefab != null ? "✅" : "❌")}");
            Debug.Log($"  - launchPoint: {(launcher.launchPoint != null ? "✅" : "❌")}");
            Debug.Log($"  - mainCamera: {(launcher.mainCamera != null ? "✅" : "❌")}");
        }

        // 6. 总结状态
        if (webglSupported && !EditorApplication.isCompiling)
        {
            Debug.Log("🎉 所有检查通过！可以开始WebGL构建了！");
            Debug.Log("📋 下一步操作:");
            Debug.Log("   1. 使用 Tools → Start WebGL Build 开始构建");
            Debug.Log("   2. 或者使用 File → Build Settings → Build 手动构建");
            Debug.Log("   3. 构建完成后使用本地服务器测试");
        }
        else
        {
            Debug.LogWarning("⚠️ 还有问题需要解决");
        }

        Debug.Log("=== 检查完成 ===");
    }

    [MenuItem("Tools/Start WebGL Build Now")]
    public static void StartWebGLBuildNow()
    {
        Debug.Log("🚀 开始WebGL构建...");

        // 检查WebGL支持
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL))
        {
            Debug.LogError("❌ WebGL不支持，请先安装WebGL Build Support模块");
            return;
        }

        // 检查编译状态
        if (EditorApplication.isCompiling)
        {
            Debug.LogWarning("⚠️ 正在编译中，请等待编译完成后再构建");
            return;
        }

        try
        {
            // 配置构建选项
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            
            // 获取构建场景
            if (EditorBuildSettings.scenes.Length > 0)
            {
                buildOptions.scenes = System.Array.ConvertAll(EditorBuildSettings.scenes, scene => scene.path);
            }
            else
            {
                buildOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
            }
            
            buildOptions.locationPathName = "WebGL-Build";
            buildOptions.target = BuildTarget.WebGL;
            buildOptions.options = BuildOptions.None;

            Debug.Log("📋 构建配置:");
            Debug.Log($"   场景: {string.Join(", ", buildOptions.scenes)}");
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

                Debug.Log("📖 测试说明:");
                Debug.Log("1. 打开终端，进入项目根目录");
                Debug.Log("2. cd WebGL-Build");
                Debug.Log("3. python -m http.server 8000");
                Debug.Log("4. 浏览器访问: http://localhost:8000");
                
                Debug.Log("🌐 部署说明:");
                Debug.Log("- 将WebGL-Build文件夹内容上传到Web服务器");
                Debug.Log("- 支持GitHub Pages、Netlify、Vercel等平台");
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