using UnityEngine;
using UnityEditor;

public class QuickTest
{
    [MenuItem("Tools/Quick Test All Systems")]
    public static void TestAllSystems()
    {
        Debug.Log("=== 🎾 快速系统测试 ===");

        // 1. 编译状态
        Debug.Log($"编译状态: {(EditorApplication.isCompiling ? "🔄 编译中" : "✅ 完成")}");

        // 2. WebGL支持
        bool webglSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        Debug.Log($"WebGL支持: {(webglSupported ? "✅ 已安装" : "❌ 未安装")}");

        // 3. 构建目标
        Debug.Log($"构建目标: {EditorUserBuildSettings.activeBuildTarget}");

        // 4. 场景检查
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        Debug.Log($"当前场景: {activeScene.name} ({activeScene.path})");

        // 5. 关键组件检查
        var launcher = Object.FindObjectOfType<BallLauncher>();
        Debug.Log($"BallLauncher: {(launcher != null ? "✅ 找到" : "❌ 未找到")}");

        var camera = Camera.main;
        Debug.Log($"主摄像机: {(camera != null ? "✅ 找到" : "❌ 未找到")}");

        // 6. 总结
        if (webglSupported && !EditorApplication.isCompiling)
        {
            Debug.Log("🎉 所有系统正常！可以开始WebGL构建！");
        }
        else
        {
            Debug.LogWarning("⚠️ 发现问题，请检查上述状态");
        }

        Debug.Log("=== 测试完成 ===");
    }
}