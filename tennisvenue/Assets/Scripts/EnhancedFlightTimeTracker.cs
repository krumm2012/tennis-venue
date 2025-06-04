using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 增强版飞行时间追踪器 - 支持详细调试信息和多项数据显示
/// </summary>
public class EnhancedFlightTimeTracker : MonoBehaviour
{
    [Header("UI显示组件")]
    public TextMeshProUGUI flightTimeText;
    public TextMeshProUGUI totalFlightText;
    public TextMeshProUGUI debugInfoText;
    public TextMeshProUGUI ballStatusText;

    [Header("调试设置")]
    public bool enableDebugMode = true;
    public bool showDetailedInfo = true;
    public bool logToConsole = true;

    [Header("追踪设置")]
    public float minTrackingSpeed = 0.3f;      // 最小追踪速度
    public float maxTrackingTime = 15f;        // 最大追踪时间
    public float minTrackingHeight = -2f;      // 最小追踪高度
    public float updateInterval = 0.1f;        // UI更新间隔

    // 状态管理
    public bool isTrackingFlight = false;
    private float flightStartTime;
    private GameObject currentBall;
    private List<FlightData> flightHistory = new List<FlightData>();

    // 飞行数据结构
    [System.Serializable]
    public class FlightData
    {
        public float duration;
        public float maxHeight;
        public float distance;
        public Vector3 startPos;
        public Vector3 endPos;
        public string timestamp;
    }

    void Start()
    {
        InitializeEnhancedUI();
        StartCoroutine(MonitorBallLaunch());

        if (enableDebugMode)
        {
            Debug.Log("🚀 Enhanced Flight Time Tracker initialized with debug mode enabled");
            Debug.Log("   ⏱️ Features: Detailed tracking, Extended UI, Statistics");
            Debug.Log("   🔧 Debug controls: F10 toggle debug mode, F11 clear history, F12 show stats");
        }
    }

    void Update()
    {
        // 调试控制快捷键
        if (Input.GetKeyDown(KeyCode.F10))
        {
            ToggleDebugMode();
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            ClearFlightHistory();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            ShowFlightStatistics();
        }
    }

    /// <summary>
    /// 初始化增强版UI界面
    /// </summary>
    void InitializeEnhancedUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found! Cannot create flight time UI.");
            return;
        }

        // 创建实时飞行时间显示
        CreateFlightTimeDisplay(canvas);

        // 创建总飞行时间显示
        CreateTotalFlightDisplay(canvas);

        // 创建调试信息显示
        if (enableDebugMode)
        {
            CreateDebugInfoDisplay(canvas);
            CreateBallStatusDisplay(canvas);
        }

        Debug.Log("✅ Enhanced Flight Time UI created successfully");
    }

    void CreateFlightTimeDisplay(Canvas canvas)
    {
        GameObject textObj = new GameObject("EnhancedFlightTimeText");
        textObj.transform.SetParent(canvas.transform, false);
        textObj.layer = 5;

        flightTimeText = textObj.AddComponent<TextMeshProUGUI>();
        flightTimeText.text = "🕐 飞行时间: 0.00s";
        flightTimeText.fontSize = 18;
        flightTimeText.color = Color.yellow;
        flightTimeText.fontStyle = FontStyles.Bold;

        RectTransform rectTransform = flightTimeText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(20, -140f);
        rectTransform.sizeDelta = new Vector2(280, 30);
    }

    void CreateTotalFlightDisplay(Canvas canvas)
    {
        GameObject totalObj = new GameObject("EnhancedTotalFlightText");
        totalObj.transform.SetParent(canvas.transform, false);
        totalObj.layer = 5;

        totalFlightText = totalObj.AddComponent<TextMeshProUGUI>();
        totalFlightText.text = "📊 上次飞行: --";
        totalFlightText.fontSize = 16;
        totalFlightText.color = Color.cyan;

        RectTransform totalRect = totalFlightText.GetComponent<RectTransform>();
        totalRect.anchorMin = new Vector2(0, 1);
        totalRect.anchorMax = new Vector2(0, 1);
        totalRect.anchoredPosition = new Vector2(20, -170f);
        totalRect.sizeDelta = new Vector2(350, 30);
    }

    void CreateDebugInfoDisplay(Canvas canvas)
    {
        GameObject debugObj = new GameObject("FlightDebugInfo");
        debugObj.transform.SetParent(canvas.transform, false);
        debugObj.layer = 5;

        debugInfoText = debugObj.AddComponent<TextMeshProUGUI>();
        debugInfoText.text = "🔧 调试信息: 准备就绪";
        debugInfoText.fontSize = 14;
        debugInfoText.color = Color.green;

        RectTransform debugRect = debugInfoText.GetComponent<RectTransform>();
        debugRect.anchorMin = new Vector2(0, 1);
        debugRect.anchorMax = new Vector2(0, 1);
        debugRect.anchoredPosition = new Vector2(20, -200f);
        debugRect.sizeDelta = new Vector2(400, 30);
    }

    void CreateBallStatusDisplay(Canvas canvas)
    {
        GameObject statusObj = new GameObject("BallStatusInfo");
        statusObj.transform.SetParent(canvas.transform, false);
        statusObj.layer = 5;

        ballStatusText = statusObj.AddComponent<TextMeshProUGUI>();
        ballStatusText.text = "⚾ 网球状态: 待发射";
        ballStatusText.fontSize = 14;
        ballStatusText.color = Color.white;

        RectTransform statusRect = ballStatusText.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 1);
        statusRect.anchorMax = new Vector2(0, 1);
        statusRect.anchoredPosition = new Vector2(20, -230f);
        statusRect.sizeDelta = new Vector2(400, 30);
    }

    /// <summary>
    /// 监控网球发射
    /// </summary>
    IEnumerator MonitorBallLaunch()
    {
        while (true)
        {
            // 移除鼠标监听，只监控是否有新的网球出现
            // 这样就不会和BallLauncher产生冲突
            if (!isTrackingFlight)
            {
                // 检查是否有新的运动中的网球
                CheckForNewBalls();
            }

            if (isTrackingFlight)
            {
                UpdateFlightDisplay();
                CheckBallStatus();
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    /// <summary>
    /// 检查是否有新的运动中的网球
    /// </summary>
    void CheckForNewBalls()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null && rb.velocity.magnitude > minTrackingSpeed)
                {
                    // 确保这是一个新的球（不是之前已经追踪过的）
                    if (currentBall == null || obj != currentBall)
                    {
                        StartEnhancedFlightTracking(obj);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 开始增强版飞行追踪
    /// </summary>
    public void StartEnhancedFlightTracking()
    {
        CheckForNewBalls();
    }

    /// <summary>
    /// 开始追踪指定的网球
    /// </summary>
    void StartEnhancedFlightTracking(GameObject ball)
    {
        currentBall = ball;
        isTrackingFlight = true;
        flightStartTime = Time.time;

        // 更新UI状态
        if (flightTimeText != null)
            flightTimeText.color = Color.green;

        if (logToConsole)
        {
            Debug.Log($"🚀 开始追踪网球飞行: {ball.name}");
            Debug.Log($"   📍 起始位置: {ball.transform.position}");

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log($"   🏃 初始速度: {rb.velocity.magnitude:F2} m/s");
            }
        }
    }

    /// <summary>
    /// 更新飞行显示
    /// </summary>
    void UpdateFlightDisplay()
    {
        if (currentBall == null) return;

        float currentTime = Time.time - flightStartTime;
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();

        // 更新飞行时间
        if (flightTimeText != null)
        {
            flightTimeText.text = $"🕐 飞行时间: {currentTime:F2}s";
        }

        // 更新详细信息
        if (enableDebugMode && showDetailedInfo && rb != null)
        {
            Vector3 position = currentBall.transform.position;
            Vector3 velocity = rb.velocity;

            if (debugInfoText != null)
            {
                debugInfoText.text = $"🔧 高度: {position.y:F2}m | 速度: {velocity.magnitude:F2}m/s | 距离: {Vector3.Distance(position, Vector3.zero):F2}m";
            }

            if (ballStatusText != null)
            {
                string status = GetBallStatusDescription(position, velocity);
                ballStatusText.text = $"⚾ 网球状态: {status}";
            }
        }
    }

    /// <summary>
    /// 获取网球状态描述
    /// </summary>
    string GetBallStatusDescription(Vector3 position, Vector3 velocity)
    {
        if (velocity.y > 2f)
            return "上升阶段";
        else if (velocity.y > -2f)
            return "平飞阶段";
        else if (position.y > 0.5f)
            return "下降阶段";
        else
            return "即将落地";
    }

    /// <summary>
    /// 检查网球状态
    /// </summary>
    void CheckBallStatus()
    {
        if (currentBall == null)
        {
            StopFlightTracking();
            return;
        }

        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb == null) return;

        float speed = rb.velocity.magnitude;
        float height = currentBall.transform.position.y;
        float flightTime = Time.time - flightStartTime;

        // 检查停止条件
        bool shouldStop = speed < minTrackingSpeed ||
                         height < minTrackingHeight ||
                         flightTime > maxTrackingTime;

        if (shouldStop)
        {
            StopFlightTracking();
        }
    }

    /// <summary>
    /// 停止飞行追踪
    /// </summary>
    void StopFlightTracking()
    {
        if (!isTrackingFlight) return;

        float totalFlightTime = Time.time - flightStartTime;

        // 记录飞行数据
        FlightData flightData = new FlightData
        {
            duration = totalFlightTime,
            timestamp = System.DateTime.Now.ToString("HH:mm:ss")
        };

        if (currentBall != null)
        {
            flightData.endPos = currentBall.transform.position;
        }

        flightHistory.Add(flightData);

        // 更新UI
        isTrackingFlight = false;

        if (flightTimeText != null)
        {
            flightTimeText.text = "🕐 飞行时间: 0.00s";
            flightTimeText.color = Color.yellow;
        }

        if (totalFlightText != null)
        {
            totalFlightText.text = $"📊 上次飞行: {totalFlightTime:F2}s (共{flightHistory.Count}次)";
        }

        if (debugInfoText != null && enableDebugMode)
        {
            debugInfoText.text = $"🔧 调试信息: 飞行结束，总计{flightHistory.Count}次飞行";
        }

        if (ballStatusText != null)
        {
            ballStatusText.text = "⚾ 网球状态: 已落地";
        }

        if (logToConsole)
        {
            Debug.Log($"✅ 飞行结束，总时间: {totalFlightTime:F2}s");
            Debug.Log($"📈 历史记录: 共追踪{flightHistory.Count}次飞行");
        }

        currentBall = null;
    }

    /// <summary>
    /// 切换调试模式
    /// </summary>
    public void ToggleDebugMode()
    {
        enableDebugMode = !enableDebugMode;

        if (debugInfoText != null)
            debugInfoText.gameObject.SetActive(enableDebugMode);
        if (ballStatusText != null)
            ballStatusText.gameObject.SetActive(enableDebugMode);

        Debug.Log($"🔧 调试模式: {(enableDebugMode ? "已启用" : "已关闭")}");

        if (enableDebugMode)
        {
            Debug.Log("   F10: 切换调试模式");
            Debug.Log("   F11: 清空飞行历史");
            Debug.Log("   F12: 显示飞行统计");
        }
    }

    /// <summary>
    /// 清空飞行历史
    /// </summary>
    public void ClearFlightHistory()
    {
        int count = flightHistory.Count;
        flightHistory.Clear();

        if (totalFlightText != null)
            totalFlightText.text = "📊 上次飞行: --";

        if (debugInfoText != null && enableDebugMode)
            debugInfoText.text = "🔧 调试信息: 历史记录已清空";

        Debug.Log($"🗑️ 已清空{count}条飞行历史记录");
    }

    /// <summary>
    /// 显示飞行统计
    /// </summary>
    public void ShowFlightStatistics()
    {
        if (flightHistory.Count == 0)
        {
            Debug.Log("📊 飞行统计: 暂无数据");
            return;
        }

        float totalTime = 0f;
        float maxTime = 0f;
        float minTime = float.MaxValue;

        foreach (var flight in flightHistory)
        {
            totalTime += flight.duration;
            if (flight.duration > maxTime) maxTime = flight.duration;
            if (flight.duration < minTime) minTime = flight.duration;
        }

        float avgTime = totalTime / flightHistory.Count;

        Debug.Log("📊 ===== 飞行统计报告 =====");
        Debug.Log($"   🎯 总飞行次数: {flightHistory.Count}");
        Debug.Log($"   ⏱️ 平均飞行时间: {avgTime:F2}s");
        Debug.Log($"   🔼 最长飞行时间: {maxTime:F2}s");
        Debug.Log($"   🔽 最短飞行时间: {minTime:F2}s");
        Debug.Log($"   ⏰ 总累计时间: {totalTime:F2}s");
        Debug.Log("==========================");
    }

    /// <summary>
    /// 外部调用接口：开始追踪
    /// </summary>
    public void StartFlightTimeTracking()
    {
        StartEnhancedFlightTracking();
    }

    void OnGUI()
    {
        if (!enableDebugMode) return;

        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 150));
        GUILayout.Label("🚀 飞行追踪调试", GUI.skin.box);
        GUILayout.Label($"追踪状态: {(isTrackingFlight ? "进行中" : "待机")}");
        GUILayout.Label($"历史记录: {flightHistory.Count} 次");
        GUILayout.Label($"更新间隔: {updateInterval:F2}s");

        if (GUILayout.Button("清空历史"))
        {
            ClearFlightHistory();
        }

        if (GUILayout.Button("显示统计"))
        {
            ShowFlightStatistics();
        }

        GUILayout.EndArea();
    }
}