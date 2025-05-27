using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class BuildTargetChecker
{
    static BuildTargetChecker()
    {
        EditorApplication.delayCall += CheckBuildTargets;
    }
    
    private static void CheckBuildTargets()
    {
        Debug.Log("🔍 检查可用的构建目标...");
        
        // 检查当前构建目标
        Debug.Log($"📱 当前构建目标: {EditorUserBuildSettings.activeBuildTarget}");
        
        // 检查WebGL是否可用
        bool webglSupported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        Debug.Log($"🌐 WebGL支持状态: {(webglSupported ? "✅ 支持" : "❌ 不支持")}");
        
        if (!webglSupported)
        {
            Debug.LogWarning("⚠️ WebGL构建目标不受支持！");
            Debug.LogWarning("可能的原因:");
            Debug.LogWarning("1. WebGL Build Support模块未安装");
            Debug.LogWarning("2. Unity版本不支持WebGL");
            Debug.LogWarning("3. 许可证问题");
            
            Debug.Log("📋 解决方案:");
            Debug.Log("1. 打开Unity Hub");
            Debug.Log("2. 找到Unity 2022.3.57f1c2");
            Debug.Log("3. 点击设置齿轮图标");
            Debug.Log("4. 选择'添加模块'");
            Debug.Log("5. 勾选'WebGL Build Support'");
            Debug.Log("6. 点击'安装'");
        }
        else
        {
            Debug.Log("✅ WebGL构建目标可用，可以进行WebGL构建");
            
            // 尝试切换到WebGL平台
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                Debug.Log("🔄 切换到WebGL构建目标...");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            }
        }
        
        // 列出所有支持的构建目标
        Debug.Log("📋 所有支持的构建目标:");
        foreach (BuildTarget target in System.Enum.GetValues(typeof(BuildTarget)))
        {
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
            if (group != BuildTargetGroup.Unknown)
            {
                bool supported = BuildPipeline.IsBuildTargetSupported(group, target);
                string status = supported ? "✅" : "❌";
                Debug.Log($"   {status} {target} ({group})");
            }
        }
    }
}