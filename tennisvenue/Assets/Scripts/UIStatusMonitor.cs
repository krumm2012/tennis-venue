using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UI状态监控器
/// 实时显示系统状态信息，包括性能、功能状态等
/// </summary>
public class UIStatusMonitor : MonoBehaviour
{
    [Header("监控配置")]
    public bool enableMonitoring = true;
    public bool showPerformanceInfo = true;
    public bool showSystemStatus = true;
    public float updateInterval = 1f;

    [Header("UI组件")]
    public Canvas statusCanvas;
    public GameObject statusPanel;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI performanceText;

    // 系统引用
    private TennisVenueUIManager uiManager;
    private BallLauncher ballLauncher;
    private CameraController cameraController;
    private TrajectoryDragController trajectoryDragController;

    // 性能监控
    private float frameRate;
    private int ballCount;
    private float lastUpdateTime;

    // 状态信息
    private Dictionary<string, string> systemStatus = new Dictionary<string, string>();

    void Start()
    {
        // 首先确保TennisBall标签存在
        CreateTennisBallTagIfNeeded();

        InitializeStatusMonitor();
        FindSystemComponents();
        CreateStatusUI();
    }

    /// <summary>
    /// 如果需要，创建TennisBall标签
    /// </summary>
    void CreateTennisBallTagIfNeeded()
    {
        #if UNITY_EDITOR
        // 检查标签是否存在
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
        bool tagExists = false;

        foreach (string tag in tags)
        {
            if (tag == "TennisBall")
            {
                tagExists = true;
                break;
            }
        }

        // 如果标签不存在，创建它
        if (!tagExists)
        {
            UnityEditorInternal.InternalEditorUtility.AddTag("TennisBall");
            Debug.Log("✅ TennisBall tag created automatically");
        }
        #endif
    }

    /// <summary>
    /// 初始化状态监控器
    /// </summary>
    void InitializeStatusMonitor()
    {
        if (!enableMonitoring) return;

        lastUpdateTime = Time.time;
        Debug.Log("📊 UI Status Monitor initialized");
    }

    /// <summary>
    /// 查找系统组件
    /// </summary>
    void FindSystemComponents()
    {
        uiManager = FindObjectOfType<TennisVenueUIManager>();
        ballLauncher = FindObjectOfType<BallLauncher>();
        cameraController = FindObjectOfType<CameraController>();
        trajectoryDragController = FindObjectOfType<TrajectoryDragController>();
    }

    /// <summary>
    /// 创建状态UI
    /// </summary>
    void CreateStatusUI()
    {
        if (!enableMonitoring) return;

        // 创建状态Canvas
        if (statusCanvas == null)
        {
            GameObject canvasObj = new GameObject("Status Monitor Canvas");
            statusCanvas = canvasObj.AddComponent<Canvas>();
            statusCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            statusCanvas.sortingOrder = 200; // 确保在其他UI之上

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 创建状态面板
        CreateStatusPanel();
    }

    /// <summary>
    /// 创建状态面板
    /// </summary>
    void CreateStatusPanel()
    {
        // 主状态面板
        GameObject panel = new GameObject("Status Panel");
        panel.transform.SetParent(statusCanvas.transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(10, -10);
        panelRect.sizeDelta = new Vector2(300, 200);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        statusPanel = panel;

        // 创建状态文本
        CreateStatusText();
        CreatePerformanceText();
    }

    /// <summary>
    /// 创建状态文本
    /// </summary>
    void CreateStatusText()
    {
        GameObject textObj = new GameObject("Status Text");
        textObj.transform.SetParent(statusPanel.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 60);
        textRect.offsetMax = new Vector2(-10, -10);

        statusText = textObj.AddComponent<TextMeshProUGUI>();
        statusText.text = "System Status";
        statusText.fontSize = 12;
        statusText.color = Color.white;
        statusText.alignment = TextAlignmentOptions.TopLeft;
    }

    /// <summary>
    /// 创建性能文本
    /// </summary>
    void CreatePerformanceText()
    {
        GameObject perfObj = new GameObject("Performance Text");
        perfObj.transform.SetParent(statusPanel.transform, false);

        RectTransform perfRect = perfObj.AddComponent<RectTransform>();
        perfRect.anchorMin = Vector2.zero;
        perfRect.anchorMax = new Vector2(1, 0);
        perfRect.offsetMin = new Vector2(10, 10);
        perfRect.offsetMax = new Vector2(-10, 50);

        performanceText = perfObj.AddComponent<TextMeshProUGUI>();
        performanceText.text = "Performance";
        performanceText.fontSize = 10;
        performanceText.color = Color.cyan;
        performanceText.alignment = TextAlignmentOptions.TopLeft;
    }

    void Update()
    {
        if (!enableMonitoring) return;

        // 定期更新状态信息
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateSystemStatus();
            UpdatePerformanceInfo();
            UpdateUI();
            lastUpdateTime = Time.time;
        }

        // 快捷键控制
        HandleInput();
    }

    /// <summary>
    /// 处理输入
    /// </summary>
    void HandleInput()
    {
        // F4键切换状态监控显示
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ToggleStatusMonitor();
        }

        // Shift+F4切换性能信息显示
        if (Input.GetKeyDown(KeyCode.F4) && Input.GetKey(KeyCode.LeftShift))
        {
            showPerformanceInfo = !showPerformanceInfo;
            Debug.Log($"📊 Performance info: {(showPerformanceInfo ? "ON" : "OFF")}");
        }
    }

    /// <summary>
    /// 切换状态监控显示
    /// </summary>
    void ToggleStatusMonitor()
    {
        enableMonitoring = !enableMonitoring;

        if (statusPanel != null)
        {
            statusPanel.SetActive(enableMonitoring);
        }

        Debug.Log($"📊 Status monitor: {(enableMonitoring ? "ON" : "OFF")}");
    }

    /// <summary>
    /// 更新系统状态
    /// </summary>
    void UpdateSystemStatus()
    {
        systemStatus.Clear();

        // UI管理器状态
        systemStatus["UI Manager"] = uiManager != null ? "✅ Active" : "❌ Missing";

        // 发球机状态
        if (ballLauncher != null)
        {
            systemStatus["Ball Launcher"] = "✅ Active";

            // 通过Slider获取当前值，如果Slider不存在则显示默认信息
            float currentAngle = ballLauncher.angleSlider != null ? ballLauncher.angleSlider.value : 0f;
            float currentSpeed = ballLauncher.speedSlider != null ? ballLauncher.speedSlider.value : 0f;

            systemStatus["Launch Angle"] = $"{currentAngle:F1}°";
            systemStatus["Launch Speed"] = $"{currentSpeed:F1}";
        }
        else
        {
            systemStatus["Ball Launcher"] = "❌ Missing";
        }

        // 摄像机状态
        if (cameraController != null)
        {
            systemStatus["Camera"] = "✅ Active";
            systemStatus["Current View"] = GetCurrentViewName();
        }
        else
        {
            systemStatus["Camera"] = "❌ Missing";
        }

        // 轨迹拖动状态
        if (trajectoryDragController != null)
        {
            systemStatus["Trajectory Drag"] = trajectoryDragController.enabled ? "✅ Enabled" : "⏸️ Disabled";
        }
        else
        {
            systemStatus["Trajectory Drag"] = "❌ Missing";
        }

        // 网球数量 - 使用安全的计数方法
        ballCount = CountTennisBalls();
        systemStatus["Tennis Balls"] = $"{ballCount} in scene";
    }

    /// <summary>
    /// 安全地计算网球数量
    /// </summary>
    int CountTennisBalls()
    {
        int count = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall") || obj.name.Contains("Ball"))
            {
                // 确保对象有物理组件，更可能是真实的网球
                if (obj.GetComponent<Rigidbody>() != null || obj.GetComponent<Collider>() != null)
                {
                    count++;
                }
            }
        }

        return count;
    }

    /// <summary>
    /// 获取当前视角名称
    /// </summary>
    string GetCurrentViewName()
    {
        if (cameraController == null) return "Unknown";

        string[] viewNames = { "Default", "Top", "Side", "Close", "Panorama", "Back" };
        int currentPreset = cameraController.CurrentPresetIndex;

        if (currentPreset >= 0 && currentPreset < viewNames.Length)
        {
            return viewNames[currentPreset];
        }

        return "Custom";
    }

    /// <summary>
    /// 更新性能信息
    /// </summary>
    void UpdatePerformanceInfo()
    {
        if (!showPerformanceInfo) return;

        // 计算帧率
        frameRate = 1.0f / Time.deltaTime;
    }

    /// <summary>
    /// 更新UI显示
    /// </summary>
    void UpdateUI()
    {
        if (statusText != null && showSystemStatus)
        {
            string statusInfo = "🎾 Tennis Venue Status\n";
            statusInfo += "━━━━━━━━━━━━━━━━━━━━\n";

            foreach (var kvp in systemStatus)
            {
                statusInfo += $"{kvp.Key}: {kvp.Value}\n";
            }

            statusText.text = statusInfo;
        }

        if (performanceText != null && showPerformanceInfo)
        {
            string perfInfo = $"📊 Performance\n";
            perfInfo += $"FPS: {frameRate:F1}\n";
            perfInfo += $"Time: {Time.time:F1}s\n";
            perfInfo += $"Objects: {FindObjectsOfType<GameObject>().Length}";

            performanceText.text = perfInfo;
        }
    }

    /// <summary>
    /// 获取详细系统报告
    /// </summary>
    public string GetDetailedSystemReport()
    {
        string report = "=== Tennis Venue Detailed System Report ===\n";
        report += $"Generated at: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";

        // 系统状态
        report += "🎮 System Status:\n";
        foreach (var kvp in systemStatus)
        {
            report += $"  • {kvp.Key}: {kvp.Value}\n";
        }

        // 性能信息
        report += $"\n📊 Performance Metrics:\n";
        report += $"  • Frame Rate: {frameRate:F2} FPS\n";
        report += $"  • Game Time: {Time.time:F2} seconds\n";
        report += $"  • Total GameObjects: {FindObjectsOfType<GameObject>().Length}\n";
        report += $"  • Tennis Balls: {ballCount}\n";

        // 内存信息
        report += $"\n💾 Memory Usage:\n";
        report += $"  • Total Memory: {System.GC.GetTotalMemory(false) / 1024 / 1024:F2} MB\n";

        report += "\n==========================================";

        return report;
    }

    /// <summary>
    /// 导出系统报告到控制台
    /// </summary>
    [ContextMenu("Export System Report")]
    public void ExportSystemReport()
    {
        string report = GetDetailedSystemReport();
        Debug.Log(report);
    }

    void OnGUI()
    {
        if (!enableMonitoring) return;

        // 在屏幕右下角显示快捷键提示
        GUI.color = new Color(1, 1, 1, 0.7f);
        GUI.Label(new Rect(Screen.width - 200, Screen.height - 60, 190, 50),
                  "F4: Toggle Status Monitor\nShift+F4: Toggle Performance");
    }
}