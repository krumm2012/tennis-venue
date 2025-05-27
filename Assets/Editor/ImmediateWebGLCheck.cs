[InitializeOnLoad]
public class ImmediateWebGLCheck
{
    static ImmediateWebGLCheck()
    {
        UnityEngine.Debug.Log("🔍 检查WebGL支持状态...");

        bool webglSupported = UnityEditor.BuildPipeline.IsBuildTargetSupported(
            UnityEditor.BuildTargetGroup.WebGL,
            UnityEditor.BuildTarget.WebGL
        );

        if (webglSupported)
        {
            UnityEngine.Debug.Log("✅ WebGL Build Support已安装！");
            UnityEngine.Debug.Log("🚀 准备执行WebGL构建...");

            // 延迟执行构建
            UnityEditor.EditorApplication.delayCall += ExecuteBuild;
        }
        else
        {
            UnityEngine.Debug.LogError("❌ WebGL Build Support未安装！");
            UnityEngine.Debug.LogWarning("请按照以下步骤安装:");
            UnityEngine.Debug.LogWarning("1. 打开Unity Hub");
            UnityEngine.Debug.LogWarning("2. 找到Unity 2022.3.57f1c2");
            UnityEngine.Debug.LogWarning("3. 点击设置齿轮 → 添加模块");
            UnityEngine.Debug.LogWarning("4. 勾选WebGL Build Support");
            UnityEngine.Debug.LogWarning("5. 点击安装");
        }
    }

    static void ExecuteBuild()
    {
        try
        {
            UnityEngine.Debug.Log("🌐 开始WebGL构建...");

            var buildOptions = new UnityEditor.BuildPlayerOptions();
            buildOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
            buildOptions.locationPathName = "WebGL-Build";
            buildOptions.target = UnityEditor.BuildTarget.WebGL;
            buildOptions.options = UnityEditor.BuildOptions.None;

            var report = UnityEditor.BuildPipeline.BuildPlayer(buildOptions);

            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                UnityEngine.Debug.Log("🎉 WebGL构建成功！");
                UnityEngine.Debug.Log($"📁 路径: {System.IO.Path.GetFullPath("WebGL-Build")}");
                UnityEngine.Debug.Log("🖥️ 测试: cd WebGL-Build && python -m http.server 8000");
                UnityEngine.Debug.Log("🌐 访问: http://localhost:8000");
            }
            else
            {
                UnityEngine.Debug.LogError($"❌ 构建失败: {report.summary.result}");
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"💥 构建异常: {e.Message}");
        }
    }
}