using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 落点坐标追踪系统 - 检测并显示网球落地坐标
/// </summary>
public class LandingPointTracker : MonoBehaviour
{
    [Header("UI显示")]
    public TextMeshProUGUI currentLandingText;
    public TextMeshProUGUI landingHistoryText;

    [Header("落点检测参数")]
    [Tooltip("检测高度阈值 - 球低于此高度视为落地")]
    public float groundHeightThreshold = 0.2f;

    [Tooltip("速度阈值 - 球速度低于此值且接近地面时视为落地")]
    public float velocityThreshold = 1.0f;

    [Tooltip("保存的落点历史数量")]
    public int maxLandingHistory = 5;

    [Header("可视化")]
    [Tooltip("是否在落点创建标记")]
    public bool createLandingMarkers = true;

    [Tooltip("落点标记保持时间（秒）")]
    public float markerLifetime = 10f;

    private List<Vector3> landingHistory = new List<Vector3>();
    private Dictionary<GameObject, bool> trackedBalls = new Dictionary<GameObject, bool>();
    private GameObject currentBall;

    void Start()
    {
        InitializeLandingPointUI();
        Debug.Log("落点坐标追踪系统已初始化");
    }

    /// <summary>
    /// 初始化落点坐标UI
    /// </summary>
    void InitializeLandingPointUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // 创建当前落点坐标显示
        GameObject currentObj = new GameObject("CurrentLandingText");
        currentObj.transform.SetParent(canvas.transform, false);
        currentObj.layer = 5;

        currentLandingText = currentObj.AddComponent<TextMeshProUGUI>();
        currentLandingText.text = "Landing Point: Waiting...";
        currentLandingText.fontSize = 15;
        currentLandingText.color = Color.yellow;

        RectTransform currentRect = currentLandingText.GetComponent<RectTransform>();
        currentRect.anchorMin = new Vector2(0, 1);
        currentRect.anchorMax = new Vector2(0, 1);
        currentRect.anchoredPosition = new Vector2(20, -210f);
        currentRect.sizeDelta = new Vector2(300, 25);

        // 创建落点历史显示
        GameObject historyObj = new GameObject("LandingHistoryText");
        historyObj.transform.SetParent(canvas.transform, false);
        historyObj.layer = 5;

        landingHistoryText = historyObj.AddComponent<TextMeshProUGUI>();
        landingHistoryText.text = "Landing History:\n(No records)";
        landingHistoryText.fontSize = 12;
        landingHistoryText.color = Color.cyan;

        RectTransform historyRect = landingHistoryText.GetComponent<RectTransform>();
        historyRect.anchorMin = new Vector2(0, 1);
        historyRect.anchorMax = new Vector2(0, 1);
        historyRect.anchoredPosition = new Vector2(20, -280f);
        historyRect.sizeDelta = new Vector2(300, 100);

        Debug.Log("落点坐标UI已创建");
    }

    void Update()
    {
        // 监控所有网球对象
        MonitorTennisBalls();

        // 手动清除落点历史
        if (Input.GetKeyDown(KeyCode.L))
        {
            ClearLandingHistory();
        }

        // 切换落点标记显示
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleLandingMarkers();
        }

        // 手动测试创建落点标记（按N键）
        if (Input.GetKeyDown(KeyCode.N))
        {
            TestCreateLandingMarker();
        }

        // 手动强制记录当前位置为落点（按B键）
        if (Input.GetKeyDown(KeyCode.B))
        {
            ForceRecordLandingPoint();
        }
    }

    /// <summary>
    /// 监控所有网球对象
    /// </summary>
    void MonitorTennisBalls()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // 如果这个球还没有被追踪，开始追踪
                    if (!trackedBalls.ContainsKey(obj))
                    {
                        trackedBalls[obj] = false; // false表示尚未落地
                        currentBall = obj;
                        Debug.Log($"开始追踪新网球: {obj.name}");
                    }

                    // 检查已追踪的球是否落地（只检查未落地的球）
                    if (trackedBalls.ContainsKey(obj) && !trackedBalls[obj])
                    {
                        CheckBallLanding(obj);
                    }
                }
            }
        }

        // 清理已销毁的球对象
        CleanupDestroyedBalls();
    }

    /// <summary>
    /// 检查网球是否落地
    /// </summary>
    void CheckBallLanding(GameObject ball)
    {
        if (ball == null) return;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 position = ball.transform.position;
        float velocity = rb.velocity.magnitude;

        Debug.Log($"检查网球落地状态: {ball.name}");
        Debug.Log($"  位置: ({position.x:F2}, {position.y:F2}, {position.z:F2})");
        Debug.Log($"  速度: {velocity:F2}m/s");
        Debug.Log($"  高度阈值: {groundHeightThreshold}m, 速度阈值: {velocityThreshold}m/s");

        // 检测落地条件：
        // 1. 高度低于阈值
        // 2. 速度较慢（表示已经停下或接近停下）
        // 3. 或者检测到与地面碰撞
        bool hasLanded = false;
        Vector3 landingPosition = position;
        string landingReason = "";

        // 基本高度和速度检测
        if (position.y <= groundHeightThreshold && velocity <= velocityThreshold)
        {
            hasLanded = true;
            landingReason = "基本检测: 高度和速度都符合条件";
            Debug.Log($"✅ 球已落地 - {landingReason}");
        }

        // 使用射线检测确认是否接触地面
        if (!hasLanded && position.y <= groundHeightThreshold + 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, 1f))
            {
                Debug.Log($"射线检测到地面: {hit.collider.name} 距离: {hit.distance:F2}m");
                if (hit.collider.name.Contains("Floor") || hit.collider.name.Contains("Ground"))
                {
                    hasLanded = true;
                    landingPosition = hit.point; // 使用精确的接触点
                    landingReason = "射线检测: 检测到地面碰撞";
                    Debug.Log($"✅ 球已落地 - {landingReason}");
                }
            }
        }

        // 额外检测：如果球已经很低且几乎不动，强制认为已落地
        if (!hasLanded && position.y <= 0.15f && velocity <= 3.0f)
        {
            hasLanded = true;
            landingPosition = new Vector3(position.x, 0.05f, position.z); // 强制到地面
            landingReason = "强制检测: 球太低且速度慢";
            Debug.Log($"✅ 球已落地 - {landingReason}");
        }

        // 超级宽松检测：如果球的Y坐标很低，无论速度如何都认为落地
        if (!hasLanded && position.y <= 0.05f)
        {
            hasLanded = true;
            landingPosition = new Vector3(position.x, 0.05f, position.z);
            landingReason = "超级宽松检测: 球已接触地面";
            Debug.Log($"✅ 球已落地 - {landingReason}");
        }

        if (hasLanded)
        {
            Debug.Log($"🎯 记录落地点: {landingPosition} (原因: {landingReason})");
            RecordLandingPoint(landingPosition, ball);
            trackedBalls[ball] = true; // 标记为已落地
        }
        else
        {
            Debug.Log($"❌ 球未满足落地条件");
        }
    }

    /// <summary>
    /// 记录落点坐标
    /// </summary>
    void RecordLandingPoint(Vector3 landingPoint, GameObject ball)
    {
        // 添加到历史记录
        landingHistory.Add(landingPoint);

        // 限制历史记录数量
        if (landingHistory.Count > maxLandingHistory)
        {
            landingHistory.RemoveAt(0);
        }

        // 更新UI显示
        UpdateLandingPointDisplay(landingPoint);

        // 创建落点标记
        if (createLandingMarkers)
        {
            CreateLandingMarker(landingPoint);
        }

        // 输出到控制台
        Debug.Log($"网球落地坐标: ({landingPoint.x:F2}, {landingPoint.y:F2}, {landingPoint.z:F2})");

        // 计算与目标的距离（如果有目标的话）
        CalculateAccuracyInfo(landingPoint);
    }

    /// <summary>
    /// 更新落点坐标显示
    /// </summary>
    void UpdateLandingPointDisplay(Vector3 lastLanding)
    {
        if (currentLandingText != null)
        {
            currentLandingText.text = $"落点坐标: ({lastLanding.x:F2}, {lastLanding.y:F2}, {lastLanding.z:F2})";
            currentLandingText.color = Color.green;
        }

        if (landingHistoryText != null)
        {
            string historyText = "落点历史:\n";
            for (int i = landingHistory.Count - 1; i >= 0; i--)
            {
                Vector3 point = landingHistory[i];
                historyText += $"  {landingHistory.Count - i}. ({point.x:F1}, {point.y:F1}, {point.z:F1})\n";
            }
            landingHistoryText.text = historyText;
        }
    }

    /// <summary>
    /// 创建落点可视化标记
    /// </summary>
    void CreateLandingMarker(Vector3 position)
    {
        Debug.Log($"=== 开始创建落点标记 ===");
        Debug.Log($"标记位置: ({position.x:F2}, {position.y:F2}, {position.z:F2})");

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "LandingMarker";

        // 将标记稍微抬高，确保可见
        Vector3 markerPosition = position + Vector3.up * 0.1f;
        marker.transform.position = markerPosition;
        marker.transform.localScale = Vector3.one * 0.25f; // 增大标记尺寸

        // 设置更明显的红色材质
        Renderer renderer = marker.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material markerMaterial = new Material(Shader.Find("Standard"));
            markerMaterial.color = Color.red;
            markerMaterial.SetFloat("_Metallic", 0.0f);
            markerMaterial.SetFloat("_Smoothness", 0.8f);

            // 添加发光效果使其更明显
            markerMaterial.EnableKeyword("_EMISSION");
            markerMaterial.SetColor("_EmissionColor", Color.red * 0.8f);

            renderer.material = markerMaterial;
            Debug.Log("✅ 标记材质设置完成");
        }
        else
        {
            Debug.LogWarning("❌ 标记Renderer组件缺失");
        }

        // 移除碰撞器避免干扰
        Collider collider = marker.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
            Debug.Log("✅ 移除标记碰撞器");
        }

        // 设置自动销毁
        Destroy(marker, markerLifetime);

        Debug.Log($"✅ 落点标记创建完成: {marker.name} 位置: {markerPosition}");
        Debug.Log($"标记将在 {markerLifetime} 秒后自动销毁");

        // 验证标记是否真的存在于场景中
        GameObject testFind = GameObject.Find("LandingMarker");
        if (testFind != null)
        {
            Debug.Log("✅ 标记已成功添加到场景中");
        }
        else
        {
            Debug.LogWarning("❌ 标记创建后无法在场景中找到");
        }
    }

    /// <summary>
    /// 计算精度信息（与发球机前方中心点的距离）
    /// </summary>
    void CalculateAccuracyInfo(Vector3 landingPoint)
    {
        // 找到发球机
        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        if (launcher != null)
        {
            // 计算发球机前方的预期目标点（假设距离10米）
            Vector3 launcherPosition = launcher.transform.position;
            Vector3 targetDirection = launcher.transform.forward;
            Vector3 expectedTarget = launcherPosition + targetDirection * 10f;
            expectedTarget.y = 0; // 地面高度

            // 计算实际落点与预期目标的偏差
            landingPoint.y = 0; // 忽略高度差异
            float distance = Vector3.Distance(landingPoint, expectedTarget);

            Vector3 deviation = landingPoint - expectedTarget;

            Debug.Log($"落点精度分析:");
            Debug.Log($"  预期目标: ({expectedTarget.x:F2}, {expectedTarget.z:F2})");
            Debug.Log($"  实际落点: ({landingPoint.x:F2}, {landingPoint.z:F2})");
            Debug.Log($"  偏差距离: {distance:F2}m");
            Debug.Log($"  偏差向量: 横向{deviation.x:F2}m, 纵向{deviation.z:F2}m");
        }
    }

    /// <summary>
    /// 清除落点历史
    /// </summary>
    public void ClearLandingHistory()
    {
        landingHistory.Clear();
        if (currentLandingText != null)
        {
            currentLandingText.text = "Landing Point: Waiting...";
            currentLandingText.color = Color.yellow;
        }
        if (landingHistoryText != null)
        {
            landingHistoryText.text = "Landing History:\n(Cleared)";
        }

        // 清除所有标记
        GameObject[] markers = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject marker in markers)
        {
            if (marker.name == "LandingMarker")
            {
                Destroy(marker);
            }
        }

        Debug.Log("Landing history cleared");
    }

    /// <summary>
    /// 切换落点标记显示
    /// </summary>
    void ToggleLandingMarkers()
    {
        createLandingMarkers = !createLandingMarkers;
        Debug.Log($"落点标记显示: {(createLandingMarkers ? "开启" : "关闭")}");
    }

    /// <summary>
    /// 清理已销毁的球对象
    /// </summary>
    void CleanupDestroyedBalls()
    {
        List<GameObject> ballsToRemove = new List<GameObject>();

        foreach (var ball in trackedBalls.Keys)
        {
            if (ball == null)
            {
                ballsToRemove.Add(ball);
            }
        }

        foreach (var ball in ballsToRemove)
        {
            trackedBalls.Remove(ball);
        }
    }

    /// <summary>
    /// 获取最后一次落点坐标
    /// </summary>
    public Vector3 GetLastLandingPoint()
    {
        if (landingHistory.Count > 0)
        {
            return landingHistory[landingHistory.Count - 1];
        }
        return Vector3.zero;
    }

    /// <summary>
    /// 获取落点历史列表
    /// </summary>
    public List<Vector3> GetLandingHistory()
    {
        return new List<Vector3>(landingHistory);
    }

    /// <summary>
    /// 手动测试创建落点标记（按N键）
    /// </summary>
    void TestCreateLandingMarker()
    {
        Debug.Log("=== 手动测试创建落点标记 ===");

        // 在场地中央创建测试标记
        Vector3 testPosition = new Vector3(0, 0.05f, 5); // 场地中央偏前位置

        if (createLandingMarkers)
        {
            CreateLandingMarker(testPosition);
            Debug.Log($"测试标记已创建于: {testPosition}");
        }
        else
        {
            Debug.LogWarning("落点标记创建功能已禁用，请按M键启用后再试");
        }
    }

    /// <summary>
    /// 手动强制记录当前位置为落点（按B键）
    /// </summary>
    void ForceRecordLandingPoint()
    {
        Debug.Log("=== 手动强制记录落点 ===");

        // 查找场景中的网球
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundBall = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                foundBall = true;
                Vector3 ballPosition = obj.transform.position;
                Vector3 groundPosition = new Vector3(ballPosition.x, 0.05f, ballPosition.z);

                Debug.Log($"找到网球: {obj.name} 位置: {ballPosition}");
                Debug.Log($"强制记录落点: {groundPosition}");

                RecordLandingPoint(groundPosition, obj);

                // 标记为已落地
                if (!trackedBalls.ContainsKey(obj))
                {
                    trackedBalls[obj] = true;
                }
                else
                {
                    trackedBalls[obj] = true;
                }

                break;
            }
        }

        if (!foundBall)
        {
            // 如果没找到网球，在摄像机前方创建一个测试落点
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 testPosition = cam.transform.position + cam.transform.forward * 5f;
                testPosition.y = 0.05f; // 地面高度

                Debug.Log($"未找到网球，在摄像机前方创建测试落点: {testPosition}");
                RecordLandingPoint(testPosition, null);
            }
            else
            {
                // 默认位置
                Vector3 defaultPosition = new Vector3(0, 0.05f, 3);
                Debug.Log($"创建默认位置落点: {defaultPosition}");
                RecordLandingPoint(defaultPosition, null);
            }
        }
    }

    /// <summary>
    /// 公共方法：强制检查指定网球的落地状态（供调试器调用）
    /// </summary>
    public void ForceCheckBallLanding(GameObject ball)
    {
        if (ball != null)
        {
            Debug.Log($"=== 调试器强制检查网球落地状态 ===");
            CheckBallLanding(ball);
        }
    }

    /// <summary>
    /// 公共方法：手动记录落点（供调试器调用）
    /// </summary>
    public void ManualRecordLandingPoint(Vector3 position, GameObject ball = null)
    {
        Debug.Log($"=== 手动记录落点 ===");
        RecordLandingPoint(position, ball);
    }
}