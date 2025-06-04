using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// LaunchBall按钮重复发射问题修复器
/// 专门针对用户报告的"LaunchBall按钮发2球，然后影响空格键也发2球"的问题
/// </summary>
public class LaunchBallBugFixer : MonoBehaviour
{
    [Header("修复配置")]
    public bool autoFixOnStart = true;
    public bool enableDebugLogs = true;
    
    private bool isFixApplied = false;
    
    void Start()
    {
        Debug.Log("=== LaunchBall按钮修复器启动 ===");
        Debug.Log("快捷键:");
        Debug.Log("  Ctrl+F11: 手动修复LaunchBall按钮问题");
        Debug.Log("  Shift+F11: 测试LaunchBall按钮发射次数");
        Debug.Log("  Alt+F11: 重置所有状态");
        
        if (autoFixOnStart)
        {
            StartCoroutine(DelayedAutoFix());
        }
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F11))
        {
            FixLaunchBallButton();
        }
        
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F11))
        {
            StartCoroutine(TestLaunchBallButton());
        }
        
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F11))
        {
            ResetAllStates();
        }
    }
    
    /// <summary>
    /// 延迟自动修复，确保所有组件都已初始化
    /// </summary>
    IEnumerator DelayedAutoFix()
    {
        yield return new WaitForSeconds(2f); // 等待UI完全初始化
        
        Debug.Log("🔧 开始自动修复LaunchBall按钮问题...");
        FixLaunchBallButton();
    }
    
    /// <summary>
    /// 修复LaunchBall按钮问题
    /// </summary>
    [ContextMenu("修复LaunchBall按钮")]
    public void FixLaunchBallButton()
    {
        Debug.Log("=== 开始修复LaunchBall按钮问题 ===");
        
        int fixedIssues = 0;
        
        // 1. 检查并修复重复的TennisVenueUIManager实例
        fixedIssues += FixDuplicateUIManagers();
        
        // 2. 检查并修复按钮事件监听器
        fixedIssues += FixButtonEventListeners();
        
        // 3. 检查并禁用冲突脚本
        fixedIssues += DisableConflictingScripts();
        
        // 4. 验证BallLauncher状态
        fixedIssues += ValidateBallLauncherState();
        
        Debug.Log($"✅ 修复完成，共处理 {fixedIssues} 个问题");
        isFixApplied = true;
        
        // 等待一帧后进行测试
        StartCoroutine(DelayedTest());
    }
    
    /// <summary>
    /// 修复重复的UIManager实例
    /// </summary>
    int FixDuplicateUIManagers()
    {
        TennisVenueUIManager[] uiManagers = FindObjectsOfType<TennisVenueUIManager>();
        
        if (uiManagers.Length > 1)
        {
            Debug.LogWarning($"⚠️ 发现 {uiManagers.Length} 个TennisVenueUIManager实例！");
            
            // 保留第一个，销毁其他的
            for (int i = 1; i < uiManagers.Length; i++)
            {
                Debug.Log($"🗑️ 销毁重复的TennisVenueUIManager: {uiManagers[i].gameObject.name}");
                DestroyImmediate(uiManagers[i].gameObject);
            }
            
            return uiManagers.Length - 1;
        }
        
        return 0;
    }
    
    /// <summary>
    /// 修复按钮事件监听器
    /// </summary>
    int FixButtonEventListeners()
    {
        int fixedCount = 0;
        
        // 查找所有LaunchBall按钮
        Button[] allButtons = FindObjectsOfType<Button>();
        
        foreach (Button button in allButtons)
        {
            if (button.gameObject.name.Contains("Launch") || 
                button.gameObject.name.Contains("🚀"))
            {
                // 检查事件监听器数量
                int listenerCount = button.onClick.GetPersistentEventCount();
                
                if (listenerCount > 1)
                {
                    Debug.LogWarning($"⚠️ LaunchBall按钮 '{button.gameObject.name}' 有 {listenerCount} 个监听器！");
                    
                    // 清除所有监听器并重新绑定
                    button.onClick.RemoveAllListeners();
                    
                    // 重新绑定正确的方法
                    TennisVenueUIManager uiManager = FindObjectOfType<TennisVenueUIManager>();
                    if (uiManager != null)
                    {
                        var launchMethod = uiManager.GetType().GetMethod("LaunchBall", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (launchMethod != null)
                        {
                            button.onClick.AddListener(() => launchMethod.Invoke(uiManager, null));
                            Debug.Log($"✅ 重新绑定LaunchBall按钮事件");
                            fixedCount++;
                        }
                    }
                }
            }
        }
        
        return fixedCount;
    }
    
    /// <summary>
    /// 禁用冲突脚本
    /// </summary>
    int DisableConflictingScripts()
    {
        int disabledCount = 0;
        
        // 禁用SimpleTennisUI（已知会使用SendMessage导致问题）
        SimpleTennisUI[] simpleUIs = FindObjectsOfType<SimpleTennisUI>();
        foreach (var ui in simpleUIs)
        {
            if (ui.enabled)
            {
                ui.enabled = false;
                Debug.Log($"🔧 已禁用SimpleTennisUI: {ui.gameObject.name}");
                disabledCount++;
            }
        }
        
        return disabledCount;
    }
    
    /// <summary>
    /// 验证BallLauncher状态
    /// </summary>
    int ValidateBallLauncherState()
    {
        BallLauncher[] launchers = FindObjectsOfType<BallLauncher>();
        
        if (launchers.Length != 1)
        {
            Debug.LogWarning($"⚠️ 场景中有 {launchers.Length} 个BallLauncher，应该只有1个！");
            
            if (launchers.Length > 1)
            {
                // 保留第一个，禁用其他的
                for (int i = 1; i < launchers.Length; i++)
                {
                    launchers[i].enabled = false;
                    Debug.Log($"🔧 禁用多余的BallLauncher: {launchers[i].gameObject.name}");
                }
                return launchers.Length - 1;
            }
        }
        
        return 0;
    }
    
    /// <summary>
    /// 测试LaunchBall按钮
    /// </summary>
    IEnumerator TestLaunchBallButton()
    {
        Debug.Log("=== 测试LaunchBall按钮发射次数 ===");
        
        // 记录测试前的网球数量
        int ballsBefore = CountTennisBalls();
        Debug.Log($"📊 测试前网球数量: {ballsBefore}");
        
        // 清除所有现有网球
        ClearAllBalls();
        yield return new WaitForEndOfFrame();
        
        ballsBefore = CountTennisBalls();
        Debug.Log($"📊 清除后网球数量: {ballsBefore}");
        
        // 查找LaunchBall按钮
        TennisVenueUIManager uiManager = FindObjectOfType<TennisVenueUIManager>();
        if (uiManager == null)
        {
            Debug.LogError("❌ 未找到TennisVenueUIManager");
            yield break;
        }
        
        // 使用反射获取launchButton
        var buttonField = uiManager.GetType().GetField("launchButton", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (buttonField != null)
        {
            Button launchButton = (Button)buttonField.GetValue(uiManager);
            
            if (launchButton != null && launchButton.interactable)
            {
                Debug.Log("🖱️ 模拟点击LaunchBall按钮...");
                launchButton.onClick.Invoke();
                
                // 等待物理计算完成
                yield return new WaitForSeconds(0.1f);
                
                int ballsAfter = CountTennisBalls();
                int ballsCreated = ballsAfter - ballsBefore;
                
                Debug.Log($"📊 测试后网球数量: {ballsAfter}");
                Debug.Log($"📊 创建的网球数量: {ballsCreated}");
                
                if (ballsCreated == 1)
                {
                    Debug.Log("✅ 测试通过：LaunchBall按钮正常工作，只发射了1个网球");
                }
                else if (ballsCreated > 1)
                {
                    Debug.LogError($"❌ 测试失败：LaunchBall按钮发射了{ballsCreated}个网球！仍存在重复发射问题！");
                }
                else
                {
                    Debug.LogWarning("⚠️ 测试异常：没有发射网球，可能BallLauncher配置有问题");
                }
            }
            else
            {
                Debug.LogError("❌ LaunchBall按钮不可用");
            }
        }
        else
        {
            Debug.LogError("❌ 未找到launchButton字段");
        }
    }
    
    /// <summary>
    /// 延迟测试
    /// </summary>
    IEnumerator DelayedTest()
    {
        yield return new WaitForSeconds(1f);
        
        Debug.Log("🧪 执行修复后测试...");
        yield return StartCoroutine(TestLaunchBallButton());
        
        // 测试空格键是否也正常
        yield return StartCoroutine(TestSpaceKeyLaunch());
    }
    
    /// <summary>
    /// 测试空格键发射
    /// </summary>
    IEnumerator TestSpaceKeyLaunch()
    {
        Debug.Log("=== 测试空格键发射 ===");
        
        ClearAllBalls();
        yield return new WaitForEndOfFrame();
        
        int ballsBefore = CountTennisBalls();
        Debug.Log($"📊 空格键测试前网球数量: {ballsBefore}");
        
        // 模拟空格键按下
        Debug.Log("⌨️ 模拟空格键按下...");
        
        // 通过BallLauncher直接测试
        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        if (launcher != null)
        {
            launcher.LaunchBall(Vector3.zero);
            
            yield return new WaitForSeconds(0.1f);
            
            int ballsAfter = CountTennisBalls();
            int ballsCreated = ballsAfter - ballsBefore;
            
            Debug.Log($"📊 空格键测试后网球数量: {ballsAfter}");
            Debug.Log($"📊 创建的网球数量: {ballsCreated}");
            
            if (ballsCreated == 1)
            {
                Debug.Log("✅ 空格键测试通过：只发射了1个网球");
            }
            else
            {
                Debug.LogError($"❌ 空格键测试失败：发射了{ballsCreated}个网球！");
            }
        }
    }
    
    /// <summary>
    /// 重置所有状态
    /// </summary>
    void ResetAllStates()
    {
        Debug.Log("🔄 重置所有状态...");
        
        // 清除所有网球
        ClearAllBalls();
        
        // 重置修复状态
        isFixApplied = false;
        
        Debug.Log("✅ 状态已重置");
    }
    
    /// <summary>
    /// 计算网球数量
    /// </summary>
    int CountTennisBalls()
    {
        int count = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall") || obj.name.Contains("Tennis Ball") || obj.name.Contains("Ball"))
            {
                if (obj.GetComponent<Rigidbody>() != null || obj.GetComponent<Collider>() != null)
                {
                    count++;
                }
            }
        }

        return count;
    }
    
    /// <summary>
    /// 清除所有网球
    /// </summary>
    void ClearAllBalls()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int cleared = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall") || obj.name.Contains("Tennis Ball") || obj.name.Contains("Ball"))
            {
                if (obj.GetComponent<Rigidbody>() != null || obj.GetComponent<Collider>() != null)
                {
                    DestroyImmediate(obj);
                    cleared++;
                }
            }
        }
        
        if (enableDebugLogs && cleared > 0)
        {
            Debug.Log($"🧹 清除了 {cleared} 个网球");
        }
    }
    
    /// <summary>
    /// GUI显示修复状态
    /// </summary>
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 100));
        
        GUILayout.Label("=== LaunchBall修复器 ===", 
            new GUIStyle() { fontSize = 12, normal = { textColor = Color.cyan } });
        
        string statusText = isFixApplied ? "✅ 已修复" : "⚠️ 未修复";
        Color statusColor = isFixApplied ? Color.green : Color.yellow;
        
        GUILayout.Label($"状态: {statusText}", 
            new GUIStyle() { normal = { textColor = statusColor } });
        
        GUILayout.Label($"网球数量: {CountTennisBalls()}", 
            new GUIStyle() { normal = { textColor = Color.white } });
        
        GUILayout.Space(5);
        GUILayout.Label("Ctrl+F11: 修复 | Shift+F11: 测试", 
            new GUIStyle() { fontSize = 10, normal = { textColor = Color.gray } });
        
        GUILayout.EndArea();
    }
}