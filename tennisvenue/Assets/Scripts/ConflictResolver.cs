using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 冲突解决器 - 自动检测和解决重复发射问题
/// </summary>
public class ConflictResolver : MonoBehaviour
{
    [Header("自动解决配置")]
    public bool autoResolveOnStart = true;
    public bool disableSimpleTennisUI = true;
    public bool forceDisableAutoPlay = true;
    public bool disableTestScripts = true;

    void Start()
    {
        Debug.Log("=== 冲突解决器启动 ===");
        
        if (autoResolveOnStart)
        {
            Invoke("ResolveAllConflicts", 1f); // 延迟1秒执行，确保所有脚本都已初始化
        }

        Debug.Log("快捷键:");
        Debug.Log("  Ctrl+Shift+F9: 手动解决所有冲突");
        Debug.Log("  Ctrl+Alt+F9: 禁用所有测试脚本");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F9))
        {
            ResolveAllConflicts();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F9))
        {
            DisableAllTestScripts();
        }
    }

    /// <summary>
    /// 解决所有冲突
    /// </summary>
    [ContextMenu("解决所有冲突")]
    public void ResolveAllConflicts()
    {
        Debug.Log("=== 开始解决冲突 ===");

        int resolvedCount = 0;

        // 1. 禁用SimpleTennisUI
        if (disableSimpleTennisUI)
        {
            resolvedCount += DisableSimpleTennisUI();
        }

        // 2. 强制关闭自动播放
        if (forceDisableAutoPlay)
        {
            resolvedCount += ForceDisableAutoPlay();
        }

        // 3. 禁用测试脚本
        if (disableTestScripts)
        {
            resolvedCount += DisableTestScripts();
        }

        // 4. 检查并报告当前状态
        CheckCurrentStatus();

        Debug.Log($"✅ 冲突解决完成，共处理 {resolvedCount} 个问题");
    }

    /// <summary>
    /// 禁用SimpleTennisUI
    /// </summary>
    int DisableSimpleTennisUI()
    {
        int count = 0;
        SimpleTennisUI[] simpleUIs = FindObjectsOfType<SimpleTennisUI>();
        
        foreach (var ui in simpleUIs)
        {
            if (ui.enabled)
            {
                ui.enabled = false;
                count++;
                Debug.Log($"🔧 已禁用 SimpleTennisUI: {ui.gameObject.name}");
            }
        }

        if (count > 0)
        {
            Debug.Log($"⚠️ 禁用了 {count} 个 SimpleTennisUI 实例 (它们使用SendMessage会导致重复发射)");
        }

        return count;
    }

    /// <summary>
    /// 强制关闭自动播放
    /// </summary>
    int ForceDisableAutoPlay()
    {
        int count = 0;
        TennisVenueUIManager[] uiManagers = FindObjectsOfType<TennisVenueUIManager>();
        
        foreach (var ui in uiManagers)
        {
            var autoPlayField = ui.GetType().GetField("isAutoPlayMode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var coroutineField = ui.GetType().GetField("autoPlayCoroutine", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (autoPlayField != null)
            {
                bool isAutoPlay = (bool)autoPlayField.GetValue(ui);
                
                if (isAutoPlay)
                {
                    // 强制关闭自动播放
                    autoPlayField.SetValue(ui, false);
                    
                    // 停止协程
                    object coroutine = coroutineField?.GetValue(ui);
                    if (coroutine != null)
                    {
                        ui.StopAllCoroutines();
                        coroutineField.SetValue(ui, null);
                    }

                    count++;
                    Debug.Log($"🔧 已强制关闭 {ui.gameObject.name} 的自动播放模式");
                }
            }
        }

        if (count > 0)
        {
            Debug.Log($"⚠️ 强制关闭了 {count} 个自动播放功能 (它们会导致重复发射)");
        }

        return count;
    }

    /// <summary>
    /// 禁用测试脚本
    /// </summary>
    int DisableTestScripts()
    {
        int count = 0;

        // 禁用QuickBallTest中的空格键监听
        QuickBallTest[] ballTests = FindObjectsOfType<QuickBallTest>();
        foreach (var test in ballTests)
        {
            if (test.enabled)
            {
                // 使用反射设置enableSpaceKey为false
                var field = test.GetType().GetField("enableSpaceKey", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(test, false);
                    count++;
                    Debug.Log($"🔧 已禁用 QuickBallTest 的空格键监听: {test.gameObject.name}");
                }
            }
        }

        // 禁用SimpleImpactTest中的空格键监听
        SimpleImpactTest[] impactTests = FindObjectsOfType<SimpleImpactTest>();
        foreach (var test in impactTests)
        {
            if (test.enabled)
            {
                var field = test.GetType().GetField("enableSpaceKey", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(test, false);
                    count++;
                    Debug.Log($"🔧 已禁用 SimpleImpactTest 的空格键监听: {test.gameObject.name}");
                }
            }
        }

        return count;
    }

    /// <summary>
    /// 禁用所有测试脚本
    /// </summary>
    int DisableAllTestScripts()
    {
        int count = 0;

        // 禁用所有测试相关脚本
        string[] testScriptTypes = {
            "QuickBallTest", "SimpleImpactTest", "LauncherDiagnostic", 
            "QuickLauncherFix", "SimpleLauncherFix", "LauncherFixTest",
            "LandingPointTestDemo", "CompileTestHelper"
        };

        foreach (string scriptType in testScriptTypes)
        {
            MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script.GetType().Name == scriptType && script.enabled)
                {
                    script.enabled = false;
                    count++;
                    Debug.Log($"🔧 已禁用测试脚本: {scriptType} ({script.gameObject.name})");
                }
            }
        }

        if (count > 0)
        {
            Debug.Log($"⚠️ 禁用了 {count} 个测试脚本以避免冲突");
        }

        return count;
    }

    /// <summary>
    /// 检查当前状态
    /// </summary>
    void CheckCurrentStatus()
    {
        Debug.Log("=== 当前系统状态 ===");

        // 检查BallLauncher
        BallLauncher[] launchers = FindObjectsOfType<BallLauncher>();
        Debug.Log($"BallLauncher: {launchers.Length} 个 {(launchers.Length == 1 ? "✅" : "⚠️")}");

        // 检查UI管理器
        TennisVenueUIManager[] uiManagers = FindObjectsOfType<TennisVenueUIManager>();
        Debug.Log($"TennisVenueUIManager: {uiManagers.Length} 个");

        foreach (var ui in uiManagers)
        {
            var autoPlayField = ui.GetType().GetField("isAutoPlayMode", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (autoPlayField != null)
            {
                bool isAutoPlay = (bool)autoPlayField.GetValue(ui);
                Debug.Log($"   {ui.gameObject.name} 自动播放: {(isAutoPlay ? "❌ 启用" : "✅ 禁用")}");
            }
        }

        // 检查SimpleTennisUI
        SimpleTennisUI[] simpleUIs = FindObjectsOfType<SimpleTennisUI>();
        int enabledSimpleUI = 0;
        foreach (var ui in simpleUIs)
        {
            if (ui.enabled) enabledSimpleUI++;
        }
        Debug.Log($"SimpleTennisUI: {simpleUIs.Length} 个 (启用: {enabledSimpleUI}) {(enabledSimpleUI == 0 ? "✅" : "⚠️")}");

        // 检查活跃的测试脚本
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        int activeTestScripts = 0;
        foreach (var script in allScripts)
        {
            if (script.enabled && (script.GetType().Name.Contains("Test") || script.GetType().Name.Contains("Diagnostic")))
            {
                activeTestScripts++;
            }
        }
        Debug.Log($"活跃测试脚本: {activeTestScripts} 个 {(activeTestScripts == 0 ? "✅" : "⚠️")}");

        Debug.Log("===================");
    }

    /// <summary>
    /// 应急重置 - 清理所有可能的冲突
    /// </summary>
    [ContextMenu("应急重置")]
    public void EmergencyReset()
    {
        Debug.Log("=== 执行应急重置 ===");

        // 停止所有协程
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (var script in allScripts)
        {
            script.StopAllCoroutines();
        }

        // 禁用所有非核心脚本
        DisableAllTestScripts();
        DisableSimpleTennisUI();
        ForceDisableAutoPlay();

        Debug.Log("✅ 应急重置完成 - 只保留核心BallLauncher功能");
    }
}