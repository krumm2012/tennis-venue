using UnityEngine;
using UnityEditor;

public class QuickWebGLTest
{
    [MenuItem("Tools/Quick WebGL Test")]
    public static void TestWebGL()
    {
        Debug.Log("🔍 快速WebGL测试...");

        // 检查WebGL支持
        bool webglSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        Debug.Log($"WebGL支持状态: {(webglSupported ? "✅ 支持" : "❌ 不支持")}");

        // 检查当前构建目标
        Debug.Log($"当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");

        if (webglSupported)
        {
            Debug.Log("✅ 可以进行WebGL构建！");
        }
        else
        {
            Debug.LogWarning("❌ 需要安装WebGL Build Support模块");
        }
    }
}