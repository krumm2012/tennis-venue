using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

/// <summary>
/// UI事件诊断工具 - 检查按钮事件监听器的重复注册问题
/// </summary>
public class UIEventDiagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== UI事件诊断工具启动 ===");
        Debug.Log("快捷键:");
        Debug.Log("  Ctrl+Shift+F12: 检查所有按钮事件监听器");
        Debug.Log("  Alt+F12: 检查LaunchBall按钮的具体监听器");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F12))
        {
            DiagnoseAllButtonEvents();
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F12))
        {
            DiagnoseLaunchButtonSpecifically();
        }
    }

    /// <summary>
    /// 诊断所有按钮事件
    /// </summary>
    void DiagnoseAllButtonEvents()
    {
        Debug.Log("=== 诊断所有按钮事件监听器 ===");

        Button[] allButtons = FindObjectsOfType<Button>();
        Debug.Log($"🔍 找到 {allButtons.Length} 个按钮");

        foreach (Button button in allButtons)
        {
            DiagnoseButton(button);
        }
    }

    /// <summary>
    /// 专门诊断LaunchBall按钮
    /// </summary>
    void DiagnoseLaunchButtonSpecifically()
    {
        Debug.Log("=== 专门诊断LaunchBall按钮 ===");

        // 查找TennisVenueUIManager
        TennisVenueUIManager uiManager = FindObjectOfType<TennisVenueUIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("❌ 未找到TennisVenueUIManager");
            return;
        }

        // 使用反射获取launchButton字段
        FieldInfo launchButtonField = uiManager.GetType().GetField("launchButton", 
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        if (launchButtonField != null)
        {
            Button launchButton = (Button)launchButtonField.GetValue(uiManager);
            
            if (launchButton != null)
            {
                Debug.Log($"✅ 找到LaunchBall按钮: {launchButton.gameObject.name}");
                DiagnoseButton(launchButton, true);
            }
            else
            {
                Debug.LogWarning("❌ LaunchBall按钮引用为null");
            }
        }
        else
        {
            Debug.LogWarning("❌ 未找到launchButton字段");
            
            // 尝试通过名称查找
            Button[] allButtons = FindObjectsOfType<Button>();
            foreach (Button button in allButtons)
            {
                if (button.gameObject.name.Contains("Launch") || 
                    button.gameObject.name.Contains("🚀"))
                {
                    Debug.Log($"🔍 通过名称找到可能的LaunchBall按钮: {button.gameObject.name}");
                    DiagnoseButton(button, true);
                }
            }
        }
    }

    /// <summary>
    /// 诊断单个按钮
    /// </summary>
    void DiagnoseButton(Button button, bool detailed = false)
    {
        if (button == null) return;

        string buttonName = button.gameObject.name;
        
        // 获取按钮的onClick事件
        var onClickEvent = button.onClick;
        
        // 使用反射获取事件监听器数量
        FieldInfo persistentCallsField = onClickEvent.GetType().GetField("m_PersistentCalls", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (persistentCallsField != null)
        {
            var persistentCalls = persistentCallsField.GetValue(onClickEvent);
            
            // 获取调用列表
            PropertyInfo callsProperty = persistentCalls.GetType().GetProperty("m_Calls", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (callsProperty != null)
            {
                var calls = callsProperty.GetValue(persistentCalls) as System.Collections.IList;
                
                if (calls != null)
                {
                    int callCount = calls.Count;
                    
                    string logLevel = callCount > 1 ? "⚠️" : "✅";
                    Debug.Log($"{logLevel} 按钮 \"{buttonName}\": {callCount} 个持久监听器");
                    
                    if (detailed && callCount > 0)
                    {
                        Debug.Log($"   详细监听器信息:");
                        for (int i = 0; i < calls.Count; i++)
                        {
                            var call = calls[i];
                            
                            // 获取目标对象和方法名
                            FieldInfo targetField = call.GetType().GetField("m_Target", 
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            FieldInfo methodNameField = call.GetType().GetField("m_MethodName", 
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            
                            if (targetField != null && methodNameField != null)
                            {
                                var target = targetField.GetValue(call);
                                var methodName = methodNameField.GetValue(call);
                                
                                string targetName = target != null ? target.GetType().Name : "null";
                                Debug.Log($"     {i + 1}. 目标: {targetName}, 方法: {methodName}");
                            }
                        }
                    }
                    
                    if (callCount > 1)
                    {
                        Debug.LogWarning($"⚠️ 按钮 \"{buttonName}\" 有 {callCount} 个监听器，可能导致重复调用！");
                    }
                }
            }
        }
        
        // 检查运行时监听器
        FieldInfo runtimeCallsField = onClickEvent.GetType().GetField("m_Calls", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (runtimeCallsField != null)
        {
            var runtimeCalls = runtimeCallsField.GetValue(onClickEvent);
            
            // 尝试获取运行时调用数量
            MethodInfo getInvocationListMethod = runtimeCalls.GetType().GetMethod("GetInvocationList");
            if (getInvocationListMethod != null)
            {
                try
                {
                    var invocationList = getInvocationListMethod.Invoke(runtimeCalls, null) as System.Delegate[];
                    if (invocationList != null && invocationList.Length > 0)
                    {
                        Debug.Log($"   运行时监听器: {invocationList.Length} 个");
                        
                        if (detailed)
                        {
                            for (int i = 0; i < invocationList.Length; i++)
                            {
                                var del = invocationList[i];
                                Debug.Log($"     运行时 {i + 1}. 目标: {del.Target?.GetType().Name}, 方法: {del.Method.Name}");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // 忽略反射错误
                }
            }
        }
        
        // 检查按钮是否可交互
        if (!button.interactable)
        {
            Debug.Log($"   ⚠️ 按钮 \"{buttonName}\" 不可交互");
        }
        
        // 检查按钮是否启用
        if (!button.gameObject.activeInHierarchy)
        {
            Debug.Log($"   ⚠️ 按钮 \"{buttonName}\" 未激活");
        }
    }

    /// <summary>
    /// 模拟点击LaunchBall按钮并监控调用
    /// </summary>
    [ContextMenu("模拟点击LaunchBall按钮")]
    public void SimulateLaunchButtonClick()
    {
        Debug.Log("=== 模拟点击LaunchBall按钮 ===");
        
        TennisVenueUIManager uiManager = FindObjectOfType<TennisVenueUIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("❌ 未找到TennisVenueUIManager");
            return;
        }

        FieldInfo launchButtonField = uiManager.GetType().GetField("launchButton", 
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        if (launchButtonField != null)
        {
            Button launchButton = (Button)launchButtonField.GetValue(uiManager);
            
            if (launchButton != null && launchButton.interactable)
            {
                Debug.Log("🖱️ 模拟点击LaunchBall按钮...");
                
                // 记录点击前的状态
                int ballCountBefore = CountTennisBalls();
                Debug.Log($"   点击前网球数量: {ballCountBefore}");
                
                // 模拟点击
                launchButton.onClick.Invoke();
                
                // 等待一帧后检查结果
                StartCoroutine(CheckResultAfterClick(ballCountBefore));
            }
            else
            {
                Debug.LogWarning("❌ LaunchBall按钮不可用");
            }
        }
    }

    /// <summary>
    /// 点击后检查结果
    /// </summary>
    System.Collections.IEnumerator CheckResultAfterClick(int ballCountBefore)
    {
        yield return new WaitForEndOfFrame();
        
        int ballCountAfter = CountTennisBalls();
        int ballsCreated = ballCountAfter - ballCountBefore;
        
        Debug.Log($"   点击后网球数量: {ballCountAfter}");
        Debug.Log($"   创建的网球数量: {ballsCreated}");
        
        if (ballsCreated == 1)
        {
            Debug.Log("✅ 正常：创建了1个网球");
        }
        else if (ballsCreated > 1)
        {
            Debug.LogError($"❌ 异常：创建了{ballsCreated}个网球！存在重复发射问题！");
        }
        else if (ballsCreated == 0)
        {
            Debug.LogWarning("⚠️ 异常：没有创建网球，可能发射失败");
        }
    }

    /// <summary>
    /// 计算场景中的网球数量
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
}