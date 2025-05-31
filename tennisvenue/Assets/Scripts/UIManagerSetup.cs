using UnityEngine;

/// <summary>
/// UI管理器自动设置脚本
/// 确保TennisVenueUIManager正确集成到场景中
/// </summary>
public class UIManagerSetup : MonoBehaviour
{
    [Header("自动设置选项")]
    public bool autoSetupOnStart = true;
    public bool replaceExistingUI = false;

    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupUIManager();
        }
    }

    /// <summary>
    /// 设置UI管理器
    /// </summary>
    [ContextMenu("Setup UI Manager")]
    public void SetupUIManager()
    {
        Debug.Log("🔧 Setting up Tennis Venue UI Manager...");

        // 检查是否已存在UI管理器
        TennisVenueUIManager existingManager = FindObjectOfType<TennisVenueUIManager>();
        if (existingManager != null)
        {
            if (replaceExistingUI)
            {
                Debug.Log("⚠️ Replacing existing UI Manager");
                DestroyImmediate(existingManager.gameObject);
            }
            else
            {
                Debug.Log("✅ UI Manager already exists, skipping setup");
                return;
            }
        }

        // 创建UI管理器对象
        GameObject uiManagerObj = new GameObject("Tennis Venue UI Manager");
        TennisVenueUIManager uiManager = uiManagerObj.AddComponent<TennisVenueUIManager>();

        // 自动查找和连接系统组件
        ConnectSystemComponents(uiManager);

        Debug.Log("✅ Tennis Venue UI Manager setup completed");

        // 显示设置信息
        ShowSetupInfo();
    }

    /// <summary>
    /// 连接系统组件
    /// </summary>
    void ConnectSystemComponents(TennisVenueUIManager uiManager)
    {
        Debug.Log("🔗 Connecting system components...");

        // 查找并连接各个系统组件
        uiManager.ballLauncher = FindObjectOfType<BallLauncher>();
        uiManager.cameraController = FindObjectOfType<CameraController>();
        uiManager.flightTimeTracker = FindObjectOfType<FlightTimeTracker>();
        uiManager.landingPointTracker = FindObjectOfType<LandingPointTracker>();
        uiManager.bounceImpactMarker = FindObjectOfType<BounceImpactMarker>();
        uiManager.trajectoryDragController = FindObjectOfType<TrajectoryDragController>();
        uiManager.airResistanceSystem = FindObjectOfType<AirResistanceSystem>();

        // 报告连接状态
        Debug.Log($"Ball Launcher: {(uiManager.ballLauncher != null ? "✅ Connected" : "❌ Not found")}");
        Debug.Log($"Camera Controller: {(uiManager.cameraController != null ? "✅ Connected" : "❌ Not found")}");
        Debug.Log($"Flight Time Tracker: {(uiManager.flightTimeTracker != null ? "✅ Connected" : "❌ Not found")}");
        Debug.Log($"Landing Point Tracker: {(uiManager.landingPointTracker != null ? "✅ Connected" : "❌ Not found")}");
        Debug.Log($"Bounce Impact Marker: {(uiManager.bounceImpactMarker != null ? "✅ Connected" : "❌ Not found")}");
        Debug.Log($"Trajectory Drag Controller: {(uiManager.trajectoryDragController != null ? "✅ Connected" : "❌ Not found")}");
        Debug.Log($"Air Resistance System: {(uiManager.airResistanceSystem != null ? "✅ Connected" : "❌ Not found")}");
    }

    /// <summary>
    /// 显示设置信息
    /// </summary>
    void ShowSetupInfo()
    {
        Debug.Log("=== Tennis Venue UI Manager Setup Info ===");
        Debug.Log("📱 UI Layout:");
        Debug.Log("  • Control Panel (左上): Launch Ball, Reset, Clear Balls");
        Debug.Log("  • View Control Panel (右上): 6个视角切换按钮");
        Debug.Log("  • Function Panel (左下): 功能控制按钮");
        Debug.Log("  • Debug Panel (右下): 调试和诊断按钮");
        Debug.Log("");
        Debug.Log("🎮 Features:");
        Debug.Log("  • 分组布局的UI界面");
        Debug.Log("  • 智能状态管理（绿色=启用，红色=禁用）");
        Debug.Log("  • 快捷键提示显示");
        Debug.Log("  • 自动组件连接");
        Debug.Log("  • 兼容现有键盘快捷键");
        Debug.Log("");
        Debug.Log("⌨️ 快捷键兼容:");
        Debug.Log("  • 所有原有快捷键继续有效");
        Debug.Log("  • UI按钮提供相同功能的点击操作");
        Debug.Log("  • I键显示系统状态（保持不变）");
        Debug.Log("==========================================");
    }

    /// <summary>
    /// 手动触发设置（用于测试）
    /// </summary>
    void Update()
    {
        // F12键手动触发UI设置
        if (Input.GetKeyDown(KeyCode.F12))
        {
            SetupUIManager();
        }
    }
}