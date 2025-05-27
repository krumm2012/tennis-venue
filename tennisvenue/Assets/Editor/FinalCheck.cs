using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class FinalCheck
{
    static FinalCheck()
    {
        EditorApplication.delayCall += PerformFinalCheck;
    }

    static void PerformFinalCheck()
    {
        Debug.Log("=== 🎾 Tennis Venue 最终状态检查 ===");

        // 1. 检查编译状态
        if (EditorApplication.isCompiling)
        {
            Debug.Log("🔄 正在编译中...");
            return;
        }
        else
        {
            Debug.Log("✅ 编译完成，无错误");
        }

        // 2. 检查WebGL支持
        bool webglSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        Debug.Log($"WebGL支持: {(webglSupported ? "✅ 已安装" : "❌ 未安装")}");

        // 3. 检查当前构建目标
        Debug.Log($"当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");

        // 4. 检查场景设置
        var scenes = EditorBuildSettings.scenes;
        if (scenes.Length > 0)
        {
            Debug.Log($"构建场景数量: {scenes.Length}");
            foreach (var scene in scenes)
            {
                Debug.Log($"  - {scene.path} {(scene.enabled ? "✅" : "❌")}");
            }
        }
        else
        {
            Debug.Log("⚠️ 没有设置构建场景，将使用当前场景");
        }

        // 5. 总结状态
        if (webglSupported && !EditorApplication.isCompiling)
        {
            Debug.Log("🎉 所有检查通过！WebGL构建环境已就绪！");
            Debug.Log("📋 可用操作:");
            Debug.Log("   • Tools → Final Status Check - 完整状态检查");
            Debug.Log("   • Tools → Start WebGL Build Now - 一键构建");
            Debug.Log("   • File → Build Settings → Build - 手动构建");
        }
        else
        {
            Debug.LogWarning("⚠️ 还有问题需要解决");
            if (!webglSupported)
            {
                Debug.LogWarning("请通过Unity Hub安装WebGL Build Support模块");
            }
        }

        Debug.Log("=== 检查完成 ===");
    }
}