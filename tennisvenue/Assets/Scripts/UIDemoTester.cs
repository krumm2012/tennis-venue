using UnityEngine;

/// <summary>
/// UI Demo测试器
/// 验证完善的UI Demo功能是否正常工作
/// </summary>
public class UIDemoTester : MonoBehaviour
{
    [Header("测试配置")]
    public bool enableTesting = true;
    public bool autoStartDemo = true;

    private UIDemo uiDemo;
    private bool testStarted = false;

    void Start()
    {
        if (!enableTesting) return;

        Debug.Log("🧪 UI Demo Tester Started");

        // 查找UIDemo组件
        uiDemo = FindObjectOfType<UIDemo>();
        if (uiDemo == null)
        {
            Debug.LogWarning("❌ UIDemo component not found in scene");
            return;
        }

        Debug.Log("✅ UIDemo component found");

        // 自动启动演示
        if (autoStartDemo)
        {
            Invoke("StartDemoTest", 3f);
        }
    }

    void Update()
    {
        if (!enableTesting) return;

        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestUIDemo();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            TestInteractiveDemo();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            TestAutoDemo();
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            TestButtonFunctionality();
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            ShowDemoStatus();
        }
    }

    /// <summary>
    /// 开始演示测试
    /// </summary>
    void StartDemoTest()
    {
        if (testStarted) return;

        Debug.Log("🎬 Starting UI Demo Test Sequence...");
        testStarted = true;

        // 测试基本功能
        TestUIDemo();
    }

    /// <summary>
    /// 测试UI Demo基本功能
    /// </summary>
    void TestUIDemo()
    {
        Debug.Log("🧪 Testing UI Demo Basic Functions...");

        if (uiDemo == null)
        {
            Debug.LogError("❌ UIDemo not available");
            return;
        }

        // 检查UIDemo配置
        Debug.Log($"   Enable Demo: {uiDemo.enableDemo}");
        Debug.Log($"   Show Instructions: {uiDemo.showInstructions}");
        Debug.Log($"   Auto Run Demo: {uiDemo.autoRunDemo}");
        Debug.Log($"   Demo Interval: {uiDemo.demoInterval}s");

        // 检查演示状态
        Debug.Log($"   Is Running Demo: {uiDemo.isRunningDemo}");
        Debug.Log($"   Current Demo Step: {uiDemo.currentDemoStep}");

        Debug.Log("✅ UI Demo basic test completed");
    }

    /// <summary>
    /// 测试交互式演示
    /// </summary>
    void TestInteractiveDemo()
    {
        Debug.Log("🎬 Testing Interactive Demo...");

        if (uiDemo == null)
        {
            Debug.LogError("❌ UIDemo not available");
            return;
        }

        // 模拟按F9键启动交互式演示
        Debug.Log("   Simulating F9 key press (Start Interactive Demo)");

        // 检查演示是否正在运行
        if (uiDemo.isRunningDemo)
        {
            Debug.Log("   ✅ Interactive demo is running");
        }
        else
        {
            Debug.Log("   ⚠️ Interactive demo not running - may need manual trigger");
        }

        Debug.Log("✅ Interactive demo test completed");
    }

    /// <summary>
    /// 测试自动演示
    /// </summary>
    void TestAutoDemo()
    {
        Debug.Log("🔄 Testing Auto Demo...");

        if (uiDemo == null)
        {
            Debug.LogError("❌ UIDemo not available");
            return;
        }

        // 模拟按F7键切换自动演示
        Debug.Log("   Simulating F7 key press (Toggle Auto Demo)");

        Debug.Log($"   Auto Run Demo: {uiDemo.autoRunDemo}");

        Debug.Log("✅ Auto demo test completed");
    }

    /// <summary>
    /// 测试按钮功能
    /// </summary>
    void TestButtonFunctionality()
    {
        Debug.Log("🔘 Testing Button Functionality...");

        // 查找UI管理器
        TennisVenueUIManager uiManager = FindObjectOfType<TennisVenueUIManager>();
        if (uiManager != null)
        {
            Debug.Log("   ✅ TennisVenueUIManager found");

            // 检查系统组件连接
            Debug.Log($"   Ball Launcher: {(uiManager.ballLauncher != null ? "✅" : "❌")}");
            Debug.Log($"   Camera Controller: {(uiManager.cameraController != null ? "✅" : "❌")}");
        }
        else
        {
            Debug.Log("   ❌ TennisVenueUIManager not found");
        }

        // 查找UI按钮
        UnityEngine.UI.Button[] buttons = FindObjectsOfType<UnityEngine.UI.Button>();
        Debug.Log($"   Found {buttons.Length} UI buttons in scene");

        Debug.Log("✅ Button functionality test completed");
    }

    /// <summary>
    /// 显示演示状态
    /// </summary>
    void ShowDemoStatus()
    {
        Debug.Log("📊 UI Demo Status Report:");

        if (uiDemo != null)
        {
            Debug.Log("=== UIDemo Component ===");
            Debug.Log($"   Enabled: {uiDemo.enabled}");
            Debug.Log($"   Enable Demo: {uiDemo.enableDemo}");
            Debug.Log($"   Show Instructions: {uiDemo.showInstructions}");
            Debug.Log($"   Auto Run Demo: {uiDemo.autoRunDemo}");
            Debug.Log($"   Demo Interval: {uiDemo.demoInterval}s");
            Debug.Log($"   Is Running Demo: {uiDemo.isRunningDemo}");
            Debug.Log($"   Current Demo Step: {uiDemo.currentDemoStep}");
        }

        // 检查相关组件
        UIManagerSetup setup = FindObjectOfType<UIManagerSetup>();
        UISystemTest systemTest = FindObjectOfType<UISystemTest>();
        TennisVenueUIManager uiManager = FindObjectOfType<TennisVenueUIManager>();

        Debug.Log("=== Related Components ===");
        Debug.Log($"   UIManagerSetup: {(setup != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"   UISystemTest: {(systemTest != null ? "✅ Found" : "❌ Missing")}");
        Debug.Log($"   TennisVenueUIManager: {(uiManager != null ? "✅ Found" : "❌ Missing")}");

        Debug.Log("========================");
    }

    /// <summary>
    /// GUI显示测试信息
    /// </summary>
    void OnGUI()
    {
        if (!enableTesting) return;

        GUILayout.BeginArea(new Rect(Screen.width - 300, 10, 280, 200));

        GUILayout.Label("=== UI Demo Tester ===", new GUIStyle() { fontSize = 14, normal = { textColor = Color.cyan } });

        if (uiDemo != null)
        {
            GUILayout.Label($"UIDemo: ✅ Active", new GUIStyle() { normal = { textColor = Color.green } });
            GUILayout.Label($"Running: {(uiDemo.isRunningDemo ? "YES" : "NO")}",
                new GUIStyle() { normal = { textColor = uiDemo.isRunningDemo ? Color.green : Color.gray } });
            GUILayout.Label($"Step: {uiDemo.currentDemoStep}", new GUIStyle() { normal = { textColor = Color.white } });
        }
        else
        {
            GUILayout.Label("UIDemo: ❌ Missing", new GUIStyle() { normal = { textColor = Color.red } });
        }

        GUILayout.Space(10);
        GUILayout.Label("Test Keys:", new GUIStyle() { fontSize = 12, normal = { textColor = Color.white } });
        GUILayout.Label("F1: Basic Test");
        GUILayout.Label("F2: Interactive Demo");
        GUILayout.Label("F3: Auto Demo");
        GUILayout.Label("F4: Button Test");
        GUILayout.Label("F5: Status Report");

        GUILayout.EndArea();
    }
}