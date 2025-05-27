using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class WebGLStatusCheck
{
    static WebGLStatusCheck()
    {
        EditorApplication.delayCall += CheckWebGLStatus;
    }

    static void CheckWebGLStatus()
    {
        Debug.Log("🔍 检查WebGL构建状态...");

        // 检查WebGL支持
        bool webglSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        Debug.Log($"WebGL支持: {(webglSupported ? "✅ 已安装" : "❌ 未安装")}");

        // 检查当前构建目标
        Debug.Log($"当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");

        // 检查编译状态
        Debug.Log($"编译状态: {(EditorApplication.isCompiling ? "🔄 编译中" : "✅ 编译完成")}");

        if (webglSupported && !EditorApplication.isCompiling)
        {
            Debug.Log("🚀 WebGL构建环境准备就绪！");
            Debug.Log("💡 可以执行WebGL构建了");
        }
        else if (!webglSupported)
        {
            Debug.LogWarning("⚠️ 需要安装WebGL Build Support模块");
            Debug.LogWarning("📋 安装步骤:");
            Debug.LogWarning("1. 打开Unity Hub");
            Debug.LogWarning("2. 找到Unity 2022.3.57f1c2");
            Debug.LogWarning("3. 点击设置齿轮 → 添加模块");
            Debug.LogWarning("4. 勾选WebGL Build Support");
            Debug.LogWarning("5. 点击安装");
        }
    }
}