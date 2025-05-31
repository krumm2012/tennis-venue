using UnityEngine;

/// <summary>
/// UI系统测试脚本
/// 验证TennisVenueUIManager是否正常工作
/// </summary>
public class UISystemTest : MonoBehaviour
{
    [Header("测试配置")]
    public bool enableTestMode = true;
    public bool autoCreateUI = true;

    private TennisVenueUIManager uiManager;
    private bool testCompleted = false;

    void Start()
    {
        if (!enableTestMode) return;

        Debug.Log("=== UI System Test Started ===");

        if (autoCreateUI)
        {
            CreateUIManager();
        }

        // 延迟测试，确保UI完全初始化
        Invoke("RunUITests", 1f);
    }

    /// <summary>
    /// 创建UI管理器
    /// </summary>
    void CreateUIManager()
    {
        // 检查是否已存在
        uiManager = FindObjectOfType<TennisVenueUIManager>();
        if (uiManager != null)
        {
            Debug.Log("✅ UI Manager already exists");
            return;
        }

        // 创建新的UI管理器
        GameObject uiManagerObj = new GameObject("Tennis Venue UI Manager");
        uiManager = uiManagerObj.AddComponent<TennisVenueUIManager>();

        Debug.Log("✅ UI Manager created successfully");
    }

    /// <summary>
    /// 运行UI测试
    /// </summary>
    void RunUITests()
    {
        if (testCompleted) return;

        Debug.Log("🧪 Running UI system tests...");

        // 测试1: 检查UI管理器是否存在
        TestUIManagerExists();

        // 测试2: 检查Canvas是否创建
        TestCanvasCreation();

        // 测试3: 检查UI面板是否创建
        TestUIPanelsCreation();

        // 测试4: 检查系统组件连接
        TestSystemComponentConnections();

        // 测试5: 测试按钮功能
        TestButtonFunctionality();

        testCompleted = true;
        Debug.Log("✅ UI system tests completed");
    }

    /// <summary>
    /// 测试UI管理器是否存在
    /// </summary>
    void TestUIManagerExists()
    {
        uiManager = FindObjectOfType<TennisVenueUIManager>();
        if (uiManager != null)
        {
            Debug.Log("✅ Test 1 PASSED: UI Manager exists");
        }
        else
        {
            Debug.LogError("❌ Test 1 FAILED: UI Manager not found");
        }
    }

    /// <summary>
    /// 测试Canvas创建
    /// </summary>
    void TestCanvasCreation()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        if (canvases.Length > 0)
        {
            Debug.Log($"✅ Test 2 PASSED: Found {canvases.Length} Canvas(es)");

            // 检查Canvas设置
            foreach (Canvas canvas in canvases)
            {
                Debug.Log($"  Canvas: {canvas.name}, RenderMode: {canvas.renderMode}");
            }
        }
        else
        {
            Debug.LogError("❌ Test 2 FAILED: No Canvas found");
        }
    }

    /// <summary>
    /// 测试UI面板创建
    /// </summary>
    void TestUIPanelsCreation()
    {
        // 查找UI面板
        GameObject controlPanel = GameObject.Find("Control Panel");
        GameObject viewPanel = GameObject.Find("View Control Panel");
        GameObject functionPanel = GameObject.Find("Function Panel");
        GameObject debugPanel = GameObject.Find("Debug Panel");

        int panelCount = 0;
        if (controlPanel != null) panelCount++;
        if (viewPanel != null) panelCount++;
        if (functionPanel != null) panelCount++;
        if (debugPanel != null) panelCount++;

        if (panelCount >= 4)
        {
            Debug.Log($"✅ Test 3 PASSED: Found {panelCount}/4 UI panels");
        }
        else
        {
            Debug.LogWarning($"⚠️ Test 3 PARTIAL: Found {panelCount}/4 UI panels");
        }
    }

    /// <summary>
    /// 测试系统组件连接
    /// </summary>
    void TestSystemComponentConnections()
    {
        if (uiManager == null) return;

        int connectedComponents = 0;

        // 使用反射检查组件连接
        var ballLauncherField = uiManager.GetType().GetField("ballLauncher");
        var cameraControllerField = uiManager.GetType().GetField("cameraController");

        if (ballLauncherField?.GetValue(uiManager) != null) connectedComponents++;
        if (cameraControllerField?.GetValue(uiManager) != null) connectedComponents++;

        // 直接查找组件
        if (FindObjectOfType<BallLauncher>() != null) connectedComponents++;
        if (FindObjectOfType<CameraController>() != null) connectedComponents++;

        Debug.Log($"✅ Test 4: Found {connectedComponents} system components");
    }

    /// <summary>
    /// 测试按钮功能
    /// </summary>
    void TestButtonFunctionality()
    {
        // 查找按钮
        UnityEngine.UI.Button[] buttons = FindObjectsOfType<UnityEngine.UI.Button>();

        if (buttons.Length > 0)
        {
            Debug.Log($"✅ Test 5 PASSED: Found {buttons.Length} UI buttons");

            // 列出按钮名称
            foreach (var button in buttons)
            {
                Debug.Log($"  Button: {button.name}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Test 5 FAILED: No UI buttons found");
        }
    }

    /// <summary>
    /// 手动触发测试
    /// </summary>
    void Update()
    {
        // F11键手动运行测试
        if (Input.GetKeyDown(KeyCode.F11))
        {
            testCompleted = false;
            RunUITests();
        }

        // F10键创建UI管理器
        if (Input.GetKeyDown(KeyCode.F10))
        {
            CreateUIManager();
        }
    }

    /// <summary>
    /// 显示测试说明
    /// </summary>
    void OnGUI()
    {
        if (!enableTestMode) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== UI System Test ===", new GUIStyle() { fontSize = 16, normal = { textColor = Color.yellow } });
        GUILayout.Label("F10: Create UI Manager");
        GUILayout.Label("F11: Run UI Tests");
        GUILayout.Label("F12: Setup UI Manager (if UIManagerSetup exists)");

        if (uiManager != null)
        {
            GUILayout.Label("✅ UI Manager: Active", new GUIStyle() { normal = { textColor = Color.green } });
        }
        else
        {
            GUILayout.Label("❌ UI Manager: Missing", new GUIStyle() { normal = { textColor = Color.red } });
        }

        GUILayout.EndArea();
    }
}