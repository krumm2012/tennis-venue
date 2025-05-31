using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 自动设置UI管理器脚本 - 增强版
/// 用于快速创建和配置TennisVenueUIManager
/// 包含脚本编译问题诊断和修复功能
/// </summary>
public class AutoSetupUIManager : MonoBehaviour
{
    [Header("自动设置选项")]
    public bool autoCreateOnStart = true;
    public bool setupStatusMonitor = true;
    public bool setupIntegrationTest = true;
    public bool setupFeatureSummary = true;

    [Header("诊断选项")]
    public bool enableScriptDiagnosis = true;
    public bool showDetailedLogs = true;

    void Start()
    {
        if (enableScriptDiagnosis)
        {
            DiagnoseScriptCompilation();
        }

        if (autoCreateOnStart)
        {
            SetupUIManager();
        }
    }

    /// <summary>
    /// 诊断脚本编译问题
    /// </summary>
    public void DiagnoseScriptCompilation()
    {
        Debug.Log("🔍 ===== Script Compilation Diagnosis =====");

        // 检查TennisVenueUIManager类型是否可用
        System.Type uiManagerType = GetTypeByName("TennisVenueUIManager");
        if (uiManagerType == null)
        {
            Debug.LogError("❌ TennisVenueUIManager type not found!");
            Debug.LogWarning("🔧 Attempting automatic fix...");
            FixScriptCompilation();
        }
        else
        {
            Debug.Log("✅ TennisVenueUIManager type is available");

            // 检查是否为MonoBehaviour
            if (typeof(MonoBehaviour).IsAssignableFrom(uiManagerType))
            {
                Debug.Log("✅ TennisVenueUIManager inherits from MonoBehaviour");
            }
            else
            {
                Debug.LogError("❌ TennisVenueUIManager does not inherit from MonoBehaviour");
            }
        }

        // 检查其他UI类型
        CheckUIComponentTypes();

        Debug.Log("============================================");
    }

    /// <summary>
    /// 检查UI组件类型
    /// </summary>
    void CheckUIComponentTypes()
    {
        string[] componentNames = {
            "UIStatusMonitor",
            "UIIntegrationTest",
            "UIFeatureSummary"
        };

        foreach (string componentName in componentNames)
        {
            System.Type type = GetTypeByName(componentName);
            if (type != null)
            {
                Debug.Log($"✅ {componentName} type available");
            }
            else
            {
                Debug.LogWarning($"⚠️ {componentName} type not found");
            }
        }
    }

    /// <summary>
    /// 通过名称获取类型
    /// </summary>
    System.Type GetTypeByName(string typeName)
    {
        // 尝试从当前程序集获取
        System.Type type = System.Type.GetType(typeName);
        if (type != null) return type;

        // 从所有程序集中查找
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName);
            if (type != null) return type;
        }

        return null;
    }

    /// <summary>
    /// 修复脚本编译问题
    /// </summary>
    public void FixScriptCompilation()
    {
        Debug.Log("🔧 Attempting to fix script compilation...");

#if UNITY_EDITOR
        try
        {
            // 1. 强制刷新资源数据库
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log("✅ Asset database refreshed");

            // 2. 重新导入TennisVenueUIManager脚本
            string scriptPath = "Assets/Scripts/TennisVenueUIManager.cs";
            if (System.IO.File.Exists(scriptPath))
            {
                UnityEditor.AssetDatabase.ImportAsset(scriptPath, UnityEditor.ImportAssetOptions.ForceUpdate);
                Debug.Log("✅ TennisVenueUIManager.cs reimported");
            }
            else
            {
                Debug.LogError($"❌ Script file not found: {scriptPath}");
            }

            // 3. 请求脚本重新编译
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            Debug.Log("✅ Script recompilation requested");

            // 4. 延迟重试
            Invoke(nameof(RetrySetupAfterCompilation), 3f);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Fix attempt failed: {e.Message}");
            ShowManualFixInstructions();
        }
#else
        Debug.LogWarning("⚠️ Script compilation fix only available in Editor mode");
        ShowManualFixInstructions();
#endif
    }

    /// <summary>
    /// 编译后重试设置
    /// </summary>
    void RetrySetupAfterCompilation()
    {
        Debug.Log("🔄 Retrying setup after compilation...");
        DiagnoseScriptCompilation();

        // 如果类型现在可用，尝试设置
        if (GetTypeByName("TennisVenueUIManager") != null)
        {
            SetupUIManager();
        }
    }

    /// <summary>
    /// 设置UI管理器系统
    /// </summary>
    [ContextMenu("Setup UI Manager")]
    public void SetupUIManager()
    {
        Debug.Log("🚀 Starting UI Manager setup...");

        // 首先检查是否已存在
        TennisVenueUIManager existingUIManager = FindObjectOfType<TennisVenueUIManager>();
        if (existingUIManager != null)
        {
            Debug.Log("✅ TennisVenueUIManager already exists!");
            return;
        }

        // 尝试创建UI管理器
        bool success = CreateUIManagerWithFallback();

        if (success)
        {
            // 创建其他UI组件
            if (setupStatusMonitor) CreateComponentSafely<UIStatusMonitor>("UI Status Monitor");
            if (setupIntegrationTest) CreateComponentSafely<UIIntegrationTest>("UI Integration Tester");
            if (setupFeatureSummary) CreateComponentSafely<UIFeatureSummary>("UI Feature Summary");

            Debug.Log("🎉 UI Manager system setup completed!");
            ShowSuccessMessage();
        }
        else
        {
            Debug.LogError("❌ Failed to create UI Manager");
            ShowManualFixInstructions();
        }
    }

    /// <summary>
    /// 使用多种方法创建UI管理器
    /// </summary>
    bool CreateUIManagerWithFallback()
    {
        // 方法1: 直接通过类型添加组件
        try
        {
            GameObject uiManagerObj = new GameObject("Tennis Venue UI Manager");
            var component = uiManagerObj.AddComponent<TennisVenueUIManager>();

            if (component != null)
            {
                Debug.Log("✅ Method 1 Success: Component added via type");
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ Method 1 Failed: {e.Message}");
        }

        // 方法2: 通过字符串添加组件
        try
        {
            GameObject uiManagerObj2 = new GameObject("Tennis Venue UI Manager");
            var component = uiManagerObj2.AddComponent<TennisVenueUIManager>() as TennisVenueUIManager;

            if (component != null)
            {
                Debug.Log("✅ Method 2 Success: Component added via string");
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ Method 2 Failed: {e.Message}");
        }

        // 方法3: 通过反射添加组件
        try
        {
            System.Type uiManagerType = GetTypeByName("TennisVenueUIManager");
            if (uiManagerType != null)
            {
                GameObject uiManagerObj3 = new GameObject("Tennis Venue UI Manager");
                var component = uiManagerObj3.AddComponent(uiManagerType) as TennisVenueUIManager;

                if (component != null)
                {
                    Debug.Log("✅ Method 3 Success: Component added via reflection");
                    return true;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ Method 3 Failed: {e.Message}");
        }

        // 方法4: 创建空GameObject供手动添加
        try
        {
            GameObject manualObj = new GameObject("Tennis Venue UI Manager (MANUAL SETUP REQUIRED)");
            manualObj.tag = "Untagged"; // 确保对象被创建
            Debug.LogWarning("⚠️ Created empty GameObject - please manually add TennisVenueUIManager script");
            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ All methods failed: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 安全地创建组件
    /// </summary>
    bool CreateComponentSafely<T>(string objectName) where T : MonoBehaviour
    {
        try
        {
            if (FindObjectOfType<T>() == null)
            {
                GameObject obj = new GameObject(objectName);
                obj.AddComponent<T>();
                Debug.Log($"✅ {typeof(T).Name} created successfully");
                return true;
            }
            else
            {
                Debug.Log($"✅ {typeof(T).Name} already exists");
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ Failed to create {typeof(T).Name}: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 显示成功消息
    /// </summary>
    void ShowSuccessMessage()
    {
        Debug.Log("🎊 ===== Setup Completed Successfully! =====");
        Debug.Log("📱 UI panels are now available:");
        Debug.Log("   • Control Panel (top-left)");
        Debug.Log("   • View Control Panel (top-right)");
        Debug.Log("   • Function Panel (bottom-left)");
        Debug.Log("   • Debug Panel (bottom-right)");
        Debug.Log("⌨️ Keyboard shortcuts:");
        Debug.Log("   • F1: Auto-play toggle");
        Debug.Log("   • F2: Settings panel");
        Debug.Log("   • F3: Help panel");
        Debug.Log("   • F4: Status monitor");
        Debug.Log("   • F5: Run tests");
        Debug.Log("=========================================");
    }

    /// <summary>
    /// 显示手动修复说明
    /// </summary>
    void ShowManualFixInstructions()
    {
        Debug.LogWarning("🔧 ===== Manual Fix Instructions =====");
        Debug.LogWarning("If automatic setup failed, try these solutions:");
        Debug.LogWarning("1. 🔄 Press Shift+F10 to force recompile");
        Debug.LogWarning("2. 📁 Check if TennisVenueUIManager.cs exists");
        Debug.LogWarning("3. 🔍 Look for compilation errors in Console");
        Debug.LogWarning("4. 🛠️ In Unity: Assets → Reimport All");
        Debug.LogWarning("5. 🔄 Restart Unity Editor");
        Debug.LogWarning("6. 📋 Manual steps:");
        Debug.LogWarning("   - Create empty GameObject");
        Debug.LogWarning("   - Add Component → Scripts → TennisVenueUIManager");
        Debug.LogWarning("7. 🆘 If still failing, check README troubleshooting section");
        Debug.LogWarning("====================================");
    }

    /// <summary>
    /// 检查系统状态
    /// </summary>
    [ContextMenu("Check System Status")]
    public void CheckSystemStatus()
    {
        Debug.Log("📊 ===== UI System Status =====");

        // 检查UI组件
        Debug.Log($"🎮 UI Manager: {CheckComponent<TennisVenueUIManager>()}");
        Debug.Log($"📊 Status Monitor: {CheckComponent<UIStatusMonitor>()}");
        Debug.Log($"🧪 Integration Test: {CheckComponent<UIIntegrationTest>()}");
        Debug.Log($"📋 Feature Summary: {CheckComponent<UIFeatureSummary>()}");

        // 检查游戏组件
        Debug.Log($"🎾 Ball Launcher: {CheckComponent<BallLauncher>()}");
        Debug.Log($"📷 Camera Controller: {CheckComponent<CameraController>()}");

        // 检查场景对象
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"🖼️ Canvas count: {canvases.Length}");

        Debug.Log("=============================");
    }

    /// <summary>
    /// 检查组件状态
    /// </summary>
    string CheckComponent<T>() where T : MonoBehaviour
    {
        T component = FindObjectOfType<T>();
        return component != null ? "✅ Found" : "❌ Missing";
    }

    void Update()
    {
        // F10: 设置UI管理器
        if (Input.GetKeyDown(KeyCode.F10))
        {
            SetupUIManager();
        }

        // F11: 检查系统状态
        if (Input.GetKeyDown(KeyCode.F11))
        {
            CheckSystemStatus();
        }

        // Shift+F10: 诊断并修复脚本编译
        if (Input.GetKeyDown(KeyCode.F10) && Input.GetKey(KeyCode.LeftShift))
        {
            DiagnoseScriptCompilation();
            FixScriptCompilation();
        }

        // Ctrl+F10: 强制重新设置所有组件
        if (Input.GetKeyDown(KeyCode.F10) && Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("🔄 Force recreating all UI components...");
            SetupUIManager();
        }
    }

    void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, 0.8f);
        GUI.Label(new Rect(10, 10, 500, 120),
                  "🔧 Auto Setup UI Manager (Enhanced)\n" +
                  "F10: Setup UI Manager\n" +
                  "F11: Check System Status\n" +
                  "Shift+F10: Diagnose & Fix Scripts\n" +
                  "Ctrl+F10: Force Recreate All");
    }
}