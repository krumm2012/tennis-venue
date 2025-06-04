using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 场景对象诊断工具 - 检查所有加载的脚本和可能的冲突
/// </summary>
public class SceneObjectDiagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 场景对象诊断工具启动 ===");
        Debug.Log("快捷键:");
        Debug.Log("  F9键: 检查所有加载的脚本");
        Debug.Log("  Ctrl+F9: 检查可能的发射冲突");
        Debug.Log("  Shift+F9: 检查自动播放状态");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                CheckLaunchConflicts();
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                CheckAutoPlayStatus();
            }
            else
            {
                DiagnoseAllScripts();
            }
        }
    }

    /// <summary>
    /// 诊断所有脚本
    /// </summary>
    void DiagnoseAllScripts()
    {
        Debug.Log("=== 场景中的所有脚本组件 ===");

        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        Dictionary<string, int> scriptCounts = new Dictionary<string, int>();

        foreach (MonoBehaviour script in allScripts)
        {
            string scriptName = script.GetType().Name;
            if (!scriptCounts.ContainsKey(scriptName))
            {
                scriptCounts[scriptName] = 0;
            }
            scriptCounts[scriptName]++;
        }

        Debug.Log($"📊 总共找到 {allScripts.Length} 个脚本实例");
        Debug.Log("📝 脚本分类统计:");

        foreach (var kvp in scriptCounts)
        {
            string status = "";
            
            // 特别标记可能有冲突的脚本
            if (kvp.Key.Contains("Launcher") || kvp.Key.Contains("Ball") || 
                kvp.Key.Contains("UI") || kvp.Key.Contains("Test"))
            {
                status = " ⚠️";
            }

            Debug.Log($"   {kvp.Key}: {kvp.Value} 个实例{status}");
        }

        // 检查特定的关键脚本
        CheckKeyScripts();
    }

    /// <summary>
    /// 检查关键脚本
    /// </summary>
    void CheckKeyScripts()
    {
        Debug.Log("🔍 关键脚本检查:");

        // 检查BallLauncher
        BallLauncher[] ballLaunchers = FindObjectsOfType<BallLauncher>();
        Debug.Log($"   BallLauncher: {ballLaunchers.Length} 个 {(ballLaunchers.Length == 1 ? "✅" : "⚠️")}");

        // 检查UI管理器
        TennisVenueUIManager[] uiManagers = FindObjectsOfType<TennisVenueUIManager>();
        Debug.Log($"   TennisVenueUIManager: {uiManagers.Length} 个 {(uiManagers.Length <= 1 ? "✅" : "⚠️")}");

        // 检查简单UI
        SimpleTennisUI[] simpleUIs = FindObjectsOfType<SimpleTennisUI>();
        Debug.Log($"   SimpleTennisUI: {simpleUIs.Length} 个 {(simpleUIs.Length == 0 ? "✅" : "⚠️")}");

        // 检查测试脚本
        QuickBallTest[] ballTests = FindObjectsOfType<QuickBallTest>();
        Debug.Log($"   QuickBallTest: {ballTests.Length} 个");

        SimpleImpactTest[] impactTests = FindObjectsOfType<SimpleImpactTest>();
        Debug.Log($"   SimpleImpactTest: {impactTests.Length} 个");

        // 检查诊断脚本
        LauncherDiagnostic[] diagnostics = FindObjectsOfType<LauncherDiagnostic>();
        Debug.Log($"   LauncherDiagnostic: {diagnostics.Length} 个");

        QuickLauncherFix[] fixes = FindObjectsOfType<QuickLauncherFix>();
        Debug.Log($"   QuickLauncherFix: {fixes.Length} 个");
    }

    /// <summary>
    /// 检查发射冲突
    /// </summary>
    void CheckLaunchConflicts()
    {
        Debug.Log("=== 检查发射冲突 ===");

        // 检查所有可能调用LaunchBall的脚本
        List<string> conflictScripts = new List<string>();

        // 检查TennisVenueUIManager
        TennisVenueUIManager[] uiManagers = FindObjectsOfType<TennisVenueUIManager>();
        foreach (var ui in uiManagers)
        {
            if (ui.enabled)
            {
                conflictScripts.Add($"TennisVenueUIManager ({ui.gameObject.name})");
                
                // 检查自动播放状态
                var autoPlayField = ui.GetType().GetField("isAutoPlayMode", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (autoPlayField != null)
                {
                    bool isAutoPlay = (bool)autoPlayField.GetValue(ui);
                    if (isAutoPlay)
                    {
                        Debug.LogWarning("⚠️ 自动播放模式已启用！这会导致重复发射！");
                    }
                }
            }
        }

        // 检查SimpleTennisUI
        SimpleTennisUI[] simpleUIs = FindObjectsOfType<SimpleTennisUI>();
        foreach (var ui in simpleUIs)
        {
            if (ui.enabled)
            {
                conflictScripts.Add($"SimpleTennisUI ({ui.gameObject.name}) - 使用SendMessage!");
                Debug.LogWarning("⚠️ SimpleTennisUI使用SendMessage调用Update，可能导致重复发射！");
            }
        }

        // 检查测试脚本
        QuickBallTest[] ballTests = FindObjectsOfType<QuickBallTest>();
        foreach (var test in ballTests)
        {
            if (test.enabled)
            {
                conflictScripts.Add($"QuickBallTest ({test.gameObject.name})");
            }
        }

        Debug.Log($"📊 发现 {conflictScripts.Count} 个可能的冲突脚本:");
        foreach (string script in conflictScripts)
        {
            Debug.Log($"   ⚠️ {script}");
        }

        if (conflictScripts.Count == 0)
        {
            Debug.Log("✅ 未发现明显的发射冲突");
        }
    }

    /// <summary>
    /// 检查自动播放状态
    /// </summary>
    void CheckAutoPlayStatus()
    {
        Debug.Log("=== 检查自动播放状态 ===");

        TennisVenueUIManager[] uiManagers = FindObjectsOfType<TennisVenueUIManager>();
        
        if (uiManagers.Length == 0)
        {
            Debug.Log("未找到TennisVenueUIManager");
            return;
        }

        foreach (var ui in uiManagers)
        {
            Debug.Log($"检查 {ui.gameObject.name}:");

            // 使用反射检查私有字段
            var autoPlayField = ui.GetType().GetField("isAutoPlayMode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var intervalField = ui.GetType().GetField("autoPlayInterval", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var coroutineField = ui.GetType().GetField("autoPlayCoroutine", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (autoPlayField != null)
            {
                bool isAutoPlay = (bool)autoPlayField.GetValue(ui);
                float interval = intervalField != null ? (float)intervalField.GetValue(ui) : 0f;
                object coroutine = coroutineField?.GetValue(ui);

                Debug.Log($"   自动播放模式: {(isAutoPlay ? "✅ 启用" : "❌ 禁用")}");
                Debug.Log($"   播放间隔: {interval} 秒");
                Debug.Log($"   协程状态: {(coroutine != null ? "✅ 运行中" : "❌ 未运行")}");

                if (isAutoPlay && coroutine != null)
                {
                    Debug.LogWarning("⚠️ 自动播放正在运行！这会导致重复发射网球！");
                    Debug.LogWarning("   解决方案: 在UI中找到Auto Play按钮并关闭它");
                }
            }
        }
    }

    /// <summary>
    /// 强制关闭自动播放
    /// </summary>
    [ContextMenu("强制关闭自动播放")]
    public void ForceDisableAutoPlay()
    {
        Debug.Log("=== 强制关闭自动播放 ===");

        TennisVenueUIManager[] uiManagers = FindObjectsOfType<TennisVenueUIManager>();
        
        foreach (var ui in uiManagers)
        {
            var toggleMethod = ui.GetType().GetMethod("ToggleAutoPlay", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var autoPlayField = ui.GetType().GetField("isAutoPlayMode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (autoPlayField != null)
            {
                bool isAutoPlay = (bool)autoPlayField.GetValue(ui);
                
                if (isAutoPlay)
                {
                    if (toggleMethod != null)
                    {
                        toggleMethod.Invoke(ui, null);
                        Debug.Log($"✅ 已关闭 {ui.gameObject.name} 的自动播放模式");
                    }
                    else
                    {
                        // 直接设置字段
                        autoPlayField.SetValue(ui, false);
                        Debug.Log($"🔧 直接关闭了 {ui.gameObject.name} 的自动播放字段");
                    }
                }
                else
                {
                    Debug.Log($"ℹ️ {ui.gameObject.name} 的自动播放已经是关闭状态");
                }
            }
        }
    }
}