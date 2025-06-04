using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FlightTimeTracker : MonoBehaviour
{
    [Header("UI显示组件")]
    public TextMeshProUGUI flightTimeText;
    public TextMeshProUGUI totalFlightText;
    public TextMeshProUGUI debugInfoText;    // 新增调试信息显示
    public TextMeshProUGUI ballStatusText;   // 新增网球状态显示

    [Header("调试设置")]
    public bool enableDebugMode = true;      // 启用调试模式
    public bool showDetailedInfo = true;     // 显示详细信息
    public bool logToConsole = true;         // 输出到控制台

    [Header("追踪设置")]
    public float minTrackingSpeed = 0.3f;    // 最小追踪速度
    public float maxTrackingTime = 15f;      // 最大追踪时间
    public float minTrackingHeight = -2f;    // 最小追踪高度

    public bool isTrackingFlight = false;
    private float flightStartTime;
    private GameObject currentBall;
    private List<float> flightHistory = new List<float>();  // 飞行历史记录

    void Start()
    {
        InitializeFlightTimeUI();
        StartCoroutine(MonitorBallLaunch());

        if (enableDebugMode)
        {
            Debug.Log("🚀 Flight Time Tracker initialized with DEBUG MODE enabled");
            Debug.Log("   ⏱️ Enhanced features: Detailed tracking, Statistics, Debug UI");
            Debug.Log("   🔧 Debug controls: F10 toggle debug mode, F11 clear history, F12 show stats");
        }
    }

    void Update()
    {
        // 新增调试控制快捷键
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

    void InitializeFlightTimeUI()
    {
        if (logToConsole)
            Debug.Log("🔧 初始化增强版飞行时间追踪系统");

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found! Cannot create flight time UI.");
            return;
        }

        // 创建实时飞行时间显示（增强版）
        GameObject textObj = new GameObject("FlightTimeText");
        textObj.transform.SetParent(canvas.transform, false);
        textObj.layer = 5;

        flightTimeText = textObj.AddComponent<TextMeshProUGUI>();
        flightTimeText.text = "🕐 飞行时间: 0.00s";
        flightTimeText.fontSize = 18;
        flightTimeText.color = Color.yellow;
        flightTimeText.fontStyle = FontStyles.Bold;  // 加粗显示

        RectTransform rectTransform = flightTimeText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(20, -140f);
        rectTransform.sizeDelta = new Vector2(280, 30);

        // 创建总飞行时间显示（增强版）
        GameObject totalObj = new GameObject("TotalFlightText");
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

        // 创建调试信息显示（新增）
        if (enableDebugMode)
        {
            CreateDebugInfoDisplay(canvas);
            CreateBallStatusDisplay(canvas);
        }

        if (logToConsole)
            Debug.Log("✅ 增强版飞行时间UI已创建");
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
                UpdateFlightTimeDisplay();
                CheckBallStatus();
            }

            yield return new WaitForSeconds(0.1f);  // 降低更新频率以提高性能
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
                        StartTrackingNewBall(obj);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 开始追踪新的网球
    /// </summary>
    void StartTrackingNewBall(GameObject ball)
    {
        currentBall = ball;
        isTrackingFlight = true;
        flightStartTime = Time.time;

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

    void UpdateFlightTimeDisplay()
    {
        if (currentBall == null) return;

        float currentTime = Time.time - flightStartTime;
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();

        // 更新飞行时间显示
        if (flightTimeText != null)
        {
            flightTimeText.text = $"🕐 飞行时间: {currentTime:F2}s";
        }

        // 更新调试信息（新增）
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

    string GetBallStatusDescription(Vector3 position, Vector3 velocity)
    {
        if (velocity.y > 2f)
            return "上升阶段 ↗️";
        else if (velocity.y > -2f)
            return "平飞阶段 ➡️";
        else if (position.y > 0.5f)
            return "下降阶段 ↘️";
        else
            return "即将落地 ⬇️";
    }

    void CheckBallStatus()
    {
        if (currentBall == null)
        {
            StopFlightTimeTracking();
            return;
        }

        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float speed = rb.velocity.magnitude;
            float height = currentBall.transform.position.y;
            float flightTime = Time.time - flightStartTime;

            // 增强的停止条件检测
            if (speed < minTrackingSpeed || height < minTrackingHeight || flightTime > maxTrackingTime)
            {
                StopFlightTimeTracking();
            }
        }
    }

    void StopFlightTimeTracking()
    {
        if (isTrackingFlight)
        {
            float totalFlightTime = Time.time - flightStartTime;

            // 记录到历史
            flightHistory.Add(totalFlightTime);

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

            if (enableDebugMode)
            {
                if (debugInfoText != null)
                    debugInfoText.text = $"🔧 调试信息: 飞行结束，总计{flightHistory.Count}次飞行";

                if (ballStatusText != null)
                    ballStatusText.text = "⚾ 网球状态: 已落地 🎯";
            }

            if (logToConsole)
            {
                Debug.Log($"✅ 飞行结束，总时间: {totalFlightTime:F2}s");
                Debug.Log($"📈 历史记录: 共追踪{flightHistory.Count}次飞行");
            }
        }
    }

    // 新增调试功能方法
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

        foreach (float time in flightHistory)
        {
            totalTime += time;
            if (time > maxTime) maxTime = time;
            if (time < minTime) minTime = time;
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

    // 新增GUI调试面板
    void OnGUI()
    {
        if (!enableDebugMode) return;

        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 120));
        GUILayout.Label("🚀 飞行追踪调试", GUI.skin.box);
        GUILayout.Label($"追踪状态: {(isTrackingFlight ? "进行中" : "待机")}");
        GUILayout.Label($"历史记录: {flightHistory.Count} 次");

        if (GUILayout.Button("清空历史 (F11)"))
        {
            ClearFlightHistory();
        }

        if (GUILayout.Button("显示统计 (F12)"))
        {
            ShowFlightStatistics();
        }

        GUILayout.EndArea();
    }

    /// <summary>
    /// 公共方法：开始飞行时间追踪（供BallLauncher调用）
    /// </summary>
    public void StartFlightTimeTracking()
    {
        // 立即查找并开始追踪最新发射的球
        CheckForNewBalls();
    }
}