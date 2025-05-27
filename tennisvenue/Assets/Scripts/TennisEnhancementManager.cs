using UnityEngine;
using TMPro;

/// <summary>
/// 网球系统增强管理器 - 统一管理所有新功能
/// </summary>
public class TennisEnhancementManager : MonoBehaviour
{
    [Header("系统组件")]
    public FloorBounceSystem floorBounceSystem;
    public FlightTimeTracker flightTimeTracker;
    public PlayerModel playerModel;
    public AirResistanceSystem airResistanceSystem;
    public LandingPointTracker landingPointTracker;
    public LandingPointDebugger landingPointDebugger;
    public BounceImpactMarker bounceImpactMarker;

    [Header("信息显示")]
    public TextMeshProUGUI systemInfoText;
    public TextMeshProUGUI systemStatusText;

    void Start()
    {
        InitializeAllSystems();
        SetupSystemStatusUI();

        Debug.Log("网球增强系统管理器已启动");
    }

    /// <summary>
    /// 初始化所有系统
    /// </summary>
    void InitializeAllSystems()
    {
        // 查找或创建地面反弹系统
        if (floorBounceSystem == null)
        {
            floorBounceSystem = FindObjectOfType<FloorBounceSystem>();
            if (floorBounceSystem == null)
            {
                GameObject systemObj = new GameObject("FloorBounceSystem");
                floorBounceSystem = systemObj.AddComponent<FloorBounceSystem>();
            }
        }

        // 查找或创建飞行时间追踪器
        if (flightTimeTracker == null)
        {
            flightTimeTracker = FindObjectOfType<FlightTimeTracker>();
            if (flightTimeTracker == null)
            {
                GameObject systemObj = new GameObject("FlightTimeTracker");
                flightTimeTracker = systemObj.AddComponent<FlightTimeTracker>();
            }
        }

        // 查找或创建人物模型
        if (playerModel == null)
        {
            playerModel = FindObjectOfType<PlayerModel>();
            if (playerModel == null)
            {
                GameObject systemObj = new GameObject("PlayerModel");
                playerModel = systemObj.AddComponent<PlayerModel>();
            }
        }

        // 查找或创建空气阻力系统
        if (airResistanceSystem == null)
        {
            airResistanceSystem = FindObjectOfType<AirResistanceSystem>();
            if (airResistanceSystem == null)
            {
                GameObject systemObj = new GameObject("AirResistanceSystem");
                airResistanceSystem = systemObj.AddComponent<AirResistanceSystem>();
            }
        }

        // 查找或创建落点追踪系统
        if (landingPointTracker == null)
        {
            landingPointTracker = FindObjectOfType<LandingPointTracker>();
            if (landingPointTracker == null)
            {
                GameObject systemObj = new GameObject("LandingPointTracker");
                landingPointTracker = systemObj.AddComponent<LandingPointTracker>();
                Debug.Log("✅ 自动创建落点追踪系统");
            }
        }

        // 查找或创建落点调试器
        if (landingPointDebugger == null)
        {
            landingPointDebugger = FindObjectOfType<LandingPointDebugger>();
            if (landingPointDebugger == null)
            {
                GameObject systemObj = new GameObject("LandingPointDebugger");
                landingPointDebugger = systemObj.AddComponent<LandingPointDebugger>();
                Debug.Log("✅ 自动创建落点调试器");
            }
        }

        // 查找或创建反弹冲击标记系统
        if (bounceImpactMarker == null)
        {
            bounceImpactMarker = FindObjectOfType<BounceImpactMarker>();
            if (bounceImpactMarker == null)
            {
                GameObject systemObj = new GameObject("BounceImpactMarker");
                bounceImpactMarker = systemObj.AddComponent<BounceImpactMarker>();
                Debug.Log("✅ 自动创建反弹冲击标记系统");
            }
        }

        Debug.Log("所有增强系统初始化完成");
    }

    void CreateSystemInfoUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject infoObj = new GameObject("SystemInfoText");
        infoObj.transform.SetParent(canvas.transform, false);
        infoObj.layer = 5;

        systemInfoText = infoObj.AddComponent<TextMeshProUGUI>();
        systemInfoText.text = "Enhanced features loaded";
        systemInfoText.fontSize = 14;
        systemInfoText.color = Color.white;

        RectTransform rect = systemInfoText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -390f);
        rect.sizeDelta = new Vector2(300, 120);

        UpdateSystemInfoDisplay();
    }

    void UpdateSystemInfoDisplay()
    {
        if (systemInfoText != null)
        {
            string info = "Tennis System Enhanced Features:\n" +
                         "• Polyurethane Floor Bounce (6mm)\n" +
                         "• Real-time Flight Time Display\n" +
                         "• 175cm Player Model Swing Test\n" +
                         "• Optimal Hit Height Analysis\n" +
                         "• Air Resistance Analysis\n" +
                         "• Landing Point Coordinate Tracking\n" +
                         "• Impact Ring Markers (Speed-based)";
            systemInfoText.text = info;
        }
    }

    void LogSystemInfo()
    {
        Debug.Log("=== System Feature Description ===");
        Debug.Log("1. Floor Bounce: 6mm polyurethane floor, bounce coefficient 0.75");
        Debug.Log("2. Flight Time: Real-time display of current and last flight time");
        Debug.Log("3. Player Model: 175cm height, test optimal hit height");
        Debug.Log("4. Landing Point Tracking: Record landing coordinates, display accuracy analysis");
        Debug.Log("5. Impact Ring Markers: Speed-based circular markers at first bounce");
        Debug.Log("6. Control Instructions:");
        Debug.Log("   - Space/Left Click: Launch ball");
        Debug.Log("   - P: Manual swing trigger");
        Debug.Log("   - R: Reset flight time timer");
        Debug.Log("   - H: Show hit height analysis/air resistance details");
        Debug.Log("   - L: Clear landing history");
        Debug.Log("   - M: Toggle landing point markers");
        Debug.Log("   - F3: Toggle impact ring markers");
        Debug.Log("   - F4: Clear all impact markers");
        Debug.Log("   - F5: Create test impact marker");
        Debug.Log("   - U: Toggle air resistance info display");
        Debug.Log("   - T: Run air resistance test data");
    }

    void Update()
    {
        // 按I键显示/刷新系统状态
        if (Input.GetKeyDown(KeyCode.I))
        {
            RefreshSystemStatus();
        }
    }

    /// <summary>
    /// 刷新系统状态显示
    /// </summary>
    void RefreshSystemStatus()
    {
        if (systemStatusText != null)
        {
            systemStatusText.text = GetSystemStatusInfo();
            Debug.Log("System status refreshed");
        }
        else
        {
            SetupSystemStatusUI(); // 如果UI不存在，重新创建
        }
    }

    void ShowSystemStatus()
    {
        Debug.Log("=== System Status Check ===");
        Debug.Log($"Floor Bounce System: {(floorBounceSystem != null ? "Normal" : "Not Found")}");
        Debug.Log($"Flight Time Tracking: {(flightTimeTracker != null ? "Normal" : "Not Found")}");
        Debug.Log($"Player Model System: {(playerModel != null ? "Normal" : "Not Found")}");
        Debug.Log($"Air Resistance System: {(airResistanceSystem != null ? "Normal" : "Not Found")}");
        Debug.Log($"Landing Point Tracking: {(landingPointTracker != null ? "Normal" : "Not Found")}");
        Debug.Log($"Impact Ring Markers: {(bounceImpactMarker != null ? "Normal" : "Not Found")}");

        if (flightTimeTracker != null)
        {
            Debug.Log($"Current tracking status: {(flightTimeTracker.isTrackingFlight ? "Tracking" : "Standby")}");
        }

        if (landingPointTracker != null)
        {
            Vector3 lastLanding = landingPointTracker.GetLastLandingPoint();
            if (lastLanding != Vector3.zero)
            {
                Debug.Log($"Last landing point: ({lastLanding.x:F2}, {lastLanding.y:F2}, {lastLanding.z:F2})");
            }

            var history = landingPointTracker.GetLandingHistory();
            Debug.Log($"Landing history count: {history.Count}");
        }

        if (bounceImpactMarker != null)
        {
            Debug.Log($"Impact markers enabled: {bounceImpactMarker.enableImpactMarkers}");
            Debug.Log($"Active impact markers: {bounceImpactMarker.GetActiveMarkerCount()}");
        }
    }

    /// <summary>
    /// 创建系统信息UI显示
    /// </summary>
    void SetupSystemStatusUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // 创建系统状态信息显示对象
        GameObject statusObj = new GameObject("SystemStatusText");
        statusObj.transform.SetParent(canvas.transform, false);
        statusObj.layer = 5; // UI layer

        systemStatusText = statusObj.AddComponent<TextMeshProUGUI>();
        systemStatusText.text = GetSystemStatusInfo();
        systemStatusText.fontSize = 11;
        systemStatusText.color = Color.white;

        // 设置位置（右上角）
        RectTransform statusRect = systemStatusText.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(1, 1);
        statusRect.anchorMax = new Vector2(1, 1);
        statusRect.anchoredPosition = new Vector2(-20, -20f);
        statusRect.sizeDelta = new Vector2(300, 180);

        Debug.Log("System status UI created");
    }

    /// <summary>
    /// 获取系统状态信息
    /// </summary>
    string GetSystemStatusInfo()
    {
        string status = "=== Tennis System Status ===\n";
        status += $"Floor Bounce: {(floorBounceSystem != null ? "✅ Normal" : "❌ Not Loaded")}\n";
        status += $"Flight Time Tracking: {(flightTimeTracker != null ? "✅ Normal" : "❌ Not Loaded")}\n";
        status += $"Player Model: {(playerModel != null ? "✅ Normal" : "❌ Not Loaded")}\n";
        status += $"Air Resistance: {(airResistanceSystem != null ? "✅ Normal" : "❌ Not Loaded")}\n";
        status += $"Landing Point Tracking: {(landingPointTracker != null ? "✅ Normal" : "❌ Not Loaded")}\n";
        status += $"Landing Debugger: {(landingPointDebugger != null ? "✅ Normal" : "❌ Not Loaded")}\n";
        status += $"Impact Ring Markers: {(bounceImpactMarker != null ? "✅ Normal" : "❌ Not Loaded")}\n";

        if (landingPointTracker != null)
        {
            status += $"  Marker Creation: {(landingPointTracker.createLandingMarkers ? "Enabled" : "Disabled")}\n";
            status += $"  History Records: {landingPointTracker.GetLandingHistory().Count}/{landingPointTracker.maxLandingHistory}\n";
        }

        if (bounceImpactMarker != null)
        {
            status += $"  Impact Markers: {(bounceImpactMarker.enableImpactMarkers ? "Enabled" : "Disabled")}\n";
            status += $"  Active Rings: {bounceImpactMarker.GetActiveMarkerCount()}\n";
            status += $"  Ring Lifetime: {bounceImpactMarker.markerLifetime}s\n";
        }

        status += "\nPress I to refresh status";
        return status;
    }
}