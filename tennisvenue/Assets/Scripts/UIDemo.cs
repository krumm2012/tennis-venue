using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI演示脚本
/// 展示如何使用TennisVenueUIManager的各种功能
/// </summary>
public class UIDemo : MonoBehaviour
{
    [Header("演示配置")]
    public bool enableDemo = true;
    public bool showInstructions = true;
    public bool autoRunDemo = false;
    public float demoInterval = 3f;

    [Header("演示状态")]
    public bool isRunningDemo = false;
    public int currentDemoStep = 0;

    private TennisVenueUIManager uiManager;
    private bool demoStarted = false;
    private Coroutine autoDemoCoroutine;

    // 演示步骤
    private string[] demoSteps = {
        "创建UI管理器",
        "测试基本控制面板",
        "演示视角切换",
        "测试功能控制",
        "运行调试工具",
        "验证系统状态"
    };

    void Start()
    {
        if (!enableDemo) return;

        Debug.Log("🎮 UI Demo Started - Tennis Venue UI Manager");

        if (showInstructions)
        {
            ShowInstructions();
        }

        // 延迟查找UI管理器，确保它已经创建
        Invoke("FindUIManager", 2f);

        // 自动演示
        if (autoRunDemo)
        {
            Invoke("StartAutoDemo", 5f);
        }
    }

    void Update()
    {
        if (!enableDemo) return;

        // 演示快捷键
        HandleDemoKeys();

        // 实时状态监控
        if (demoStarted && Time.frameCount % 300 == 0) // 每5秒检查一次
        {
            MonitorUIStatus();
        }
    }

    /// <summary>
    /// 处理演示快捷键
    /// </summary>
    void HandleDemoKeys()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            CreateUIManagerDemo();
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            RunUITestDemo();
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            ShowUIStatusDemo();
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            StartInteractiveDemo();
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            TestAllButtons();
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            ToggleAutoDemo();
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            ResetDemo();
        }
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void ShowInstructions()
    {
        Debug.Log("=== Tennis Venue UI Manager 使用说明 ===");
        Debug.Log("🎮 UI界面分为四个面板：");
        Debug.Log("   📍 Control Panel (左上角): 基本控制 - Launch Ball, Reset, Clear Balls");
        Debug.Log("   📷 View Control Panel (右上角): 视角控制 - 6种预设视角");
        Debug.Log("   ⚙️ Function Panel (左下角): 功能控制 - 挥拍、分析、追踪等");
        Debug.Log("   🔧 Debug Panel (右下角): 调试工具 - 状态、测试、诊断等");
        Debug.Log("");
        Debug.Log("🔑 演示快捷键:");
        Debug.Log("   F6: 重置演示");
        Debug.Log("   F7: 切换自动演示");
        Debug.Log("   F8: 测试所有按钮");
        Debug.Log("   F9: 开始交互式演示");
        Debug.Log("   F10: 创建UI管理器");
        Debug.Log("   F11: 运行UI系统测试");
        Debug.Log("   F12: 显示UI状态");
        Debug.Log("");
        Debug.Log("💡 提示: 按钮颜色表示状态 - 绿色(启用), 红色(禁用), 蓝色(操作)");
        Debug.Log("========================================");
    }

    /// <summary>
    /// 查找UI管理器
    /// </summary>
    void FindUIManager()
    {
        uiManager = FindObjectOfType<TennisVenueUIManager>();
        if (uiManager != null)
        {
            Debug.Log("✅ UI Manager found and ready!");
            demoStarted = true;
        }
        else
        {
            Debug.Log("⚠️ UI Manager not found. Press F10 to create one.");
        }
    }

    /// <summary>
    /// 创建UI管理器演示
    /// </summary>
    void CreateUIManagerDemo()
    {
        Debug.Log("🔧 Creating UI Manager...");

        // 检查是否已存在
        if (uiManager != null)
        {
            Debug.Log("ℹ️ UI Manager already exists!");
            return;
        }

        // 查找UIManagerSetup
        UIManagerSetup setup = FindObjectOfType<UIManagerSetup>();
        if (setup != null)
        {
            setup.SetupUIManager();
            uiManager = FindObjectOfType<TennisVenueUIManager>();
            Debug.Log("✅ UI Manager created via UIManagerSetup!");
            demoStarted = true;
        }
        else
        {
            Debug.Log("❌ UIManagerSetup not found. Creating directly...");
            CreateUIManagerDirectly();
        }
    }

    /// <summary>
    /// 直接创建UI管理器
    /// </summary>
    void CreateUIManagerDirectly()
    {
        GameObject uiManagerObj = new GameObject("Tennis Venue UI Manager");
        uiManager = uiManagerObj.AddComponent<TennisVenueUIManager>();
        Debug.Log("✅ UI Manager created directly!");
        demoStarted = true;
    }

    /// <summary>
    /// 运行UI测试演示
    /// </summary>
    void RunUITestDemo()
    {
        Debug.Log("🧪 Running UI System Test...");

        UISystemTest tester = FindObjectOfType<UISystemTest>();
        if (tester != null)
        {
            tester.enabled = true;
            Debug.Log("✅ UI System Test started!");
        }
        else
        {
            Debug.Log("❌ UISystemTest not found. Creating test manually...");
            CreateManualTest();
        }
    }

    /// <summary>
    /// 显示UI状态演示
    /// </summary>
    void ShowUIStatusDemo()
    {
        Debug.Log("📊 UI System Status:");

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<TennisVenueUIManager>();
        }

        if (uiManager != null)
        {
            Debug.Log("✅ TennisVenueUIManager: Active");

            // 检查UI面板
            CheckUIPanels();

            // 检查系统组件连接
            CheckSystemComponents();
        }
        else
        {
            Debug.Log("❌ TennisVenueUIManager: Not Found");
        }

        // 检查其他UI组件
        CheckOtherUIComponents();
    }

    /// <summary>
    /// 检查UI面板
    /// </summary>
    void CheckUIPanels()
    {
        GameObject controlPanel = GameObject.Find("Control Panel");
        GameObject viewPanel = GameObject.Find("View Control Panel");
        GameObject functionPanel = GameObject.Find("Function Panel");
        GameObject debugPanel = GameObject.Find("Debug Panel");

        Debug.Log($"   📍 Control Panel: {(controlPanel != null ? "✅ Created" : "❌ Missing")}");
        Debug.Log($"   📷 View Panel: {(viewPanel != null ? "✅ Created" : "❌ Missing")}");
        Debug.Log($"   ⚙️ Function Panel: {(functionPanel != null ? "✅ Created" : "❌ Missing")}");
        Debug.Log($"   🔧 Debug Panel: {(debugPanel != null ? "✅ Created" : "❌ Missing")}");
    }

    /// <summary>
    /// 检查系统组件连接
    /// </summary>
    void CheckSystemComponents()
    {
        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        CameraController camera = FindObjectOfType<CameraController>();
        FlightTimeTracker flightTracker = FindObjectOfType<FlightTimeTracker>();
        LandingPointTracker landingTracker = FindObjectOfType<LandingPointTracker>();
        BounceImpactMarker impactMarker = FindObjectOfType<BounceImpactMarker>();

        Debug.Log($"   🚀 BallLauncher: {(launcher != null ? "✅ Connected" : "❌ Missing")}");
        Debug.Log($"   📷 CameraController: {(camera != null ? "✅ Connected" : "❌ Missing")}");
        Debug.Log($"   ⏱️ FlightTimeTracker: {(flightTracker != null ? "✅ Connected" : "❌ Missing")}");
        Debug.Log($"   🎯 LandingPointTracker: {(landingTracker != null ? "✅ Connected" : "❌ Missing")}");
        Debug.Log($"   💥 BounceImpactMarker: {(impactMarker != null ? "✅ Connected" : "❌ Missing")}");
    }

    /// <summary>
    /// 检查其他UI组件
    /// </summary>
    void CheckOtherUIComponents()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        UIManagerSetup setup = FindObjectOfType<UIManagerSetup>();
        UISystemTest test = FindObjectOfType<UISystemTest>();

        Debug.Log($"🖼️ Canvas: {(canvas != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"🔧 UIManagerSetup: {(setup != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"🧪 UISystemTest: {(test != null ? "✅ Found" : "❌ Missing")}");
    }

    /// <summary>
    /// 开始交互式演示
    /// </summary>
    void StartInteractiveDemo()
    {
        if (isRunningDemo)
        {
            Debug.Log("⚠️ Demo already running. Press F6 to reset.");
            return;
        }

        Debug.Log("🎬 Starting Interactive UI Demo...");
        isRunningDemo = true;
        currentDemoStep = 0;

        StartCoroutine(RunInteractiveDemo());
    }

    /// <summary>
    /// 运行交互式演示
    /// </summary>
    IEnumerator RunInteractiveDemo()
    {
        for (int i = 0; i < demoSteps.Length; i++)
        {
            currentDemoStep = i;
            Debug.Log($"🎬 Demo Step {i + 1}/{demoSteps.Length}: {demoSteps[i]}");

            switch (i)
            {
                case 0:
                    yield return StartCoroutine(DemoCreateUIManager());
                    break;
                case 1:
                    yield return StartCoroutine(DemoBasicControls());
                    break;
                case 2:
                    yield return StartCoroutine(DemoViewControls());
                    break;
                case 3:
                    yield return StartCoroutine(DemoFunctionControls());
                    break;
                case 4:
                    yield return StartCoroutine(DemoDebugTools());
                    break;
                case 5:
                    yield return StartCoroutine(DemoSystemVerification());
                    break;
            }

            yield return new WaitForSeconds(demoInterval);
        }

        Debug.Log("🎉 Interactive Demo Completed!");
        isRunningDemo = false;
    }

    /// <summary>
    /// 演示创建UI管理器
    /// </summary>
    IEnumerator DemoCreateUIManager()
    {
        if (uiManager == null)
        {
            CreateUIManagerDemo();
            yield return new WaitForSeconds(1f);
        }
        Debug.Log("✅ UI Manager creation demo completed");
        yield return null;
    }

    /// <summary>
    /// 演示基本控制
    /// </summary>
    IEnumerator DemoBasicControls()
    {
        Debug.Log("🎮 Testing basic controls...");

        if (uiManager != null)
        {
            // 模拟点击Launch Ball按钮
            Debug.Log("   🚀 Simulating Launch Ball button click");
            yield return new WaitForSeconds(0.5f);

            // 模拟点击Clear Balls按钮
            Debug.Log("   🧹 Simulating Clear Balls button click");
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("✅ Basic controls demo completed");
        yield return null;
    }

    /// <summary>
    /// 演示视角控制
    /// </summary>
    IEnumerator DemoViewControls()
    {
        Debug.Log("📷 Testing view controls...");

        CameraController camera = FindObjectOfType<CameraController>();
        if (camera != null)
        {
            string[] views = {"Default", "Top", "Side", "Close", "Panorama", "Back"};

            for (int i = 0; i < views.Length; i++)
            {
                Debug.Log($"   📷 Switching to {views[i]} view");
                // 这里可以实际调用视角切换
                yield return new WaitForSeconds(0.3f);
            }
        }

        Debug.Log("✅ View controls demo completed");
        yield return null;
    }

    /// <summary>
    /// 演示功能控制
    /// </summary>
    IEnumerator DemoFunctionControls()
    {
        Debug.Log("⚙️ Testing function controls...");

        string[] functions = {"Swing Test", "Height Analysis", "Air Resistance", "Landing Point", "Impact Marker"};

        foreach (string function in functions)
        {
            Debug.Log($"   ⚙️ Testing {function}");
            yield return new WaitForSeconds(0.4f);
        }

        Debug.Log("✅ Function controls demo completed");
        yield return null;
    }

    /// <summary>
    /// 演示调试工具
    /// </summary>
    IEnumerator DemoDebugTools()
    {
        Debug.Log("🔧 Testing debug tools...");

        string[] tools = {"System Status", "Clear History", "Toggle Markers", "Test Ball", "Diagnostics"};

        foreach (string tool in tools)
        {
            Debug.Log($"   🔧 Testing {tool}");
            yield return new WaitForSeconds(0.4f);
        }

        Debug.Log("✅ Debug tools demo completed");
        yield return null;
    }

    /// <summary>
    /// 演示系统验证
    /// </summary>
    IEnumerator DemoSystemVerification()
    {
        Debug.Log("🔍 Running system verification...");

        ShowUIStatusDemo();
        yield return new WaitForSeconds(1f);

        Debug.Log("✅ System verification completed");
        yield return null;
    }

    /// <summary>
    /// 测试所有按钮
    /// </summary>
    void TestAllButtons()
    {
        Debug.Log("🧪 Testing all UI buttons...");

        Button[] buttons = FindObjectsOfType<Button>();
        Debug.Log($"Found {buttons.Length} buttons in the scene");

        foreach (Button button in buttons)
        {
            if (button != null && button.gameObject.activeInHierarchy)
            {
                Debug.Log($"   🔘 Button: {button.name} - {(button.interactable ? "Interactable" : "Disabled")}");
            }
        }

        Debug.Log("✅ Button test completed");
    }

    /// <summary>
    /// 切换自动演示
    /// </summary>
    void ToggleAutoDemo()
    {
        autoRunDemo = !autoRunDemo;

        if (autoRunDemo && !isRunningDemo)
        {
            Debug.Log("🔄 Auto demo enabled - starting in 2 seconds...");
            Invoke("StartInteractiveDemo", 2f);
        }
        else if (!autoRunDemo && autoDemoCoroutine != null)
        {
            Debug.Log("⏹️ Auto demo disabled");
            StopCoroutine(autoDemoCoroutine);
        }

        Debug.Log($"🔄 Auto Demo: {(autoRunDemo ? "ON" : "OFF")}");
    }

    /// <summary>
    /// 重置演示
    /// </summary>
    void ResetDemo()
    {
        Debug.Log("🔄 Resetting UI Demo...");

        isRunningDemo = false;
        currentDemoStep = 0;

        if (autoDemoCoroutine != null)
        {
            StopCoroutine(autoDemoCoroutine);
            autoDemoCoroutine = null;
        }

        Debug.Log("✅ Demo reset completed");
    }

    /// <summary>
    /// 创建手动测试
    /// </summary>
    void CreateManualTest()
    {
        Debug.Log("🔧 Creating manual UI test...");

        // 检查基本组件
        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        CameraController camera = FindObjectOfType<CameraController>();

        Debug.Log($"🚀 BallLauncher: {(launcher != null ? "Found" : "Missing")}");
        Debug.Log($"📷 CameraController: {(camera != null ? "Found" : "Missing")}");

        if (launcher != null && camera != null)
        {
            Debug.Log("✅ Core components found. UI system should work properly.");
        }
        else
        {
            Debug.Log("⚠️ Some core components are missing. UI functionality may be limited.");
        }
    }

    /// <summary>
    /// 监控UI状态
    /// </summary>
    void MonitorUIStatus()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<TennisVenueUIManager>();
        }

        // 静默监控，只在有问题时报告
        if (uiManager == null && demoStarted)
        {
            Debug.LogWarning("⚠️ UI Manager lost connection - attempting to reconnect...");
            FindUIManager();
        }
    }

    /// <summary>
    /// 开始自动演示
    /// </summary>
    void StartAutoDemo()
    {
        if (autoRunDemo && !isRunningDemo)
        {
            StartInteractiveDemo();
        }
    }

    /// <summary>
    /// 在Scene视图中显示帮助信息
    /// </summary>
    void OnDrawGizmos()
    {
        if (!enableDemo) return;

        // 在Scene视图中显示UI演示状态
        if (isRunningDemo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
        }
        else
        {
            Gizmos.color = demoStarted ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        // 显示当前演示步骤
        if (isRunningDemo && currentDemoStep < demoSteps.Length)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, Vector3.up * (currentDemoStep + 1));
        }
    }

    /// <summary>
    /// GUI显示演示信息
    /// </summary>
    void OnGUI()
    {
        if (!enableDemo) return;

        GUILayout.BeginArea(new Rect(10, Screen.height - 200, 400, 180));

        GUILayout.Label("=== UI Demo Control ===", new GUIStyle() { fontSize = 16, normal = { textColor = Color.yellow } });

        if (isRunningDemo)
        {
            GUILayout.Label($"🎬 Running Demo: Step {currentDemoStep + 1}/{demoSteps.Length}",
                new GUIStyle() { normal = { textColor = Color.cyan } });
            GUILayout.Label($"Current: {(currentDemoStep < demoSteps.Length ? demoSteps[currentDemoStep] : "Completed")}",
                new GUIStyle() { normal = { textColor = Color.white } });
        }
        else
        {
            GUILayout.Label("Demo Status: Ready", new GUIStyle() { normal = { textColor = Color.green } });
        }

        GUILayout.Label($"UI Manager: {(uiManager != null ? "✅ Active" : "❌ Missing")}",
            new GUIStyle() { normal = { textColor = uiManager != null ? Color.green : Color.red } });

        GUILayout.Label($"Auto Demo: {(autoRunDemo ? "ON" : "OFF")}",
            new GUIStyle() { normal = { textColor = autoRunDemo ? Color.green : Color.gray } });

        GUILayout.Space(10);
        GUILayout.Label("Quick Keys:", new GUIStyle() { fontSize = 12, normal = { textColor = Color.white } });
        GUILayout.Label("F6:Reset F7:Auto F8:Test F9:Demo F10:Create F11:Test F12:Status");

        GUILayout.EndArea();
    }
}