using UnityEngine;

/// <summary>
/// 网球落地冲击修复器 - 修复实际网球落点圆环无法显示的问题
/// </summary>
public class TennisBallImpactFix : MonoBehaviour
{
    [Header("修复设置")]
    [Tooltip("是否启用强制检测模式")]
    public bool enableForceDetection = true;

    [Tooltip("检测范围（距离场地中心的最大距离）")]
    public float detectionRange = 20f;

    [Tooltip("强制检测的高度阈值")]
    public float forceHeightThreshold = 1.0f;

    [Tooltip("强制检测的速度阈值")]
    public float forceSpeedThreshold = 0.5f;

    private BounceImpactMarker impactMarker;

    void Start()
    {
        Debug.Log("=== Tennis Ball Impact Fix Started ===");
        Debug.Log("Press Shift+F8 to enable force detection mode");
        Debug.Log("Press Shift+F9 to reset tennis ball positions");

        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker system not found!");
        }
        else
        {
            Debug.Log("✅ BounceImpactMarker system found");
        }
    }

    void Update()
    {
        if (enableForceDetection)
        {
            MonitorTennisBallsForced();
        }

        // 快捷键控制
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F8))
        {
            ToggleForceDetection();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F9))
        {
            ResetTennisBallPositions();
        }
    }

    /// <summary>
    /// 强制监控网球落地
    /// </summary>
    void MonitorTennisBallsForced()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 pos = obj.transform.position;
                    Vector3 vel = rb.velocity;
                    float speed = vel.magnitude;

                    // 检查是否在合理范围内
                    float distanceFromCenter = Vector3.Distance(pos, Vector3.zero);

                    if (distanceFromCenter > detectionRange)
                    {
                        Debug.LogWarning($"⚠️ Ball {obj.name} is too far from field center: {distanceFromCenter:F1}m");
                        Debug.LogWarning($"   Position: {pos}");
                        continue;
                    }

                    // 使用更宽松的检测条件
                    bool heightCondition = pos.y <= forceHeightThreshold;
                    bool speedCondition = speed > forceSpeedThreshold;
                    bool isMovingDown = vel.y < 0;

                    if (heightCondition && speedCondition && isMovingDown)
                    {
                        Debug.Log($"🎾 Force detection triggered for {obj.name}:");
                        Debug.Log($"   Position: {pos}");
                        Debug.Log($"   Velocity: {vel}");
                        Debug.Log($"   Speed: {speed:F2}m/s");

                        // 强制创建冲击标记
                        ForceCreateImpactMarker(pos, speed, vel);

                        // 将球标记为已处理（通过反射访问私有字段）
                        MarkBallAsProcessed(obj);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 强制创建冲击标记
    /// </summary>
    void ForceCreateImpactMarker(Vector3 ballPosition, float speed, Vector3 velocity)
    {
        if (impactMarker == null) return;

        // 计算地面冲击点
        Vector3 impactPoint = new Vector3(ballPosition.x, 0.01f, ballPosition.z);

        Debug.Log($"🎯 Force creating impact marker:");
        Debug.Log($"   Ball position: {ballPosition}");
        Debug.Log($"   Impact point: {impactPoint}");
        Debug.Log($"   Speed: {speed:F2}m/s");

        // 使用反射调用私有方法
        var method = impactMarker.GetType().GetMethod("CreateImpactMarker",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(impactMarker, new object[] { impactPoint, speed, velocity });
            Debug.Log($"✅ Force impact marker created successfully!");
        }
        else
        {
            Debug.LogError("❌ Could not find CreateImpactMarker method");
        }
    }

    /// <summary>
    /// 将球标记为已处理
    /// </summary>
    void MarkBallAsProcessed(GameObject ball)
    {
        if (impactMarker == null) return;

        // 使用反射访问私有字段
        var field = impactMarker.GetType().GetField("markedBalls",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            var markedBalls = field.GetValue(impactMarker) as System.Collections.Generic.Dictionary<GameObject, bool>;
            if (markedBalls != null)
            {
                markedBalls[ball] = true;
                Debug.Log($"🔒 Ball {ball.name} marked as processed");
            }
        }
    }

    /// <summary>
    /// 切换强制检测模式
    /// </summary>
    void ToggleForceDetection()
    {
        enableForceDetection = !enableForceDetection;
        Debug.Log($"Force detection mode: {(enableForceDetection ? "ENABLED" : "DISABLED")}");

        if (enableForceDetection)
        {
            Debug.Log("🔍 Now using relaxed detection conditions:");
            Debug.Log($"   Height threshold: ≤{forceHeightThreshold}m");
            Debug.Log($"   Speed threshold: >{forceSpeedThreshold}m/s");
            Debug.Log($"   Detection range: {detectionRange}m from center");
        }
    }

    /// <summary>
    /// 重置网球位置到合理范围
    /// </summary>
    void ResetTennisBallPositions()
    {
        Debug.Log("=== Resetting Tennis Ball Positions ===");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int resetCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                Vector3 currentPos = obj.transform.position;
                float distanceFromCenter = Vector3.Distance(currentPos, Vector3.zero);

                if (distanceFromCenter > detectionRange)
                {
                    // 重置到场地中心附近的随机位置
                    Vector3 newPos = new Vector3(
                        Random.Range(-2f, 2f),
                        Random.Range(1f, 3f),
                        Random.Range(-2f, 2f)
                    );

                    obj.transform.position = newPos;

                    // 重置速度
                    Rigidbody rb = obj.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }

                    Debug.Log($"🔄 Reset {obj.name}: {currentPos} → {newPos}");
                    resetCount++;

                    // 清除已标记状态
                    ClearBallMarkedStatus(obj);
                }
            }
        }

        Debug.Log($"✅ Reset {resetCount} tennis balls to field area");
    }

    /// <summary>
    /// 清除球的已标记状态
    /// </summary>
    void ClearBallMarkedStatus(GameObject ball)
    {
        if (impactMarker == null) return;

        var field = impactMarker.GetType().GetField("markedBalls",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            var markedBalls = field.GetValue(impactMarker) as System.Collections.Generic.Dictionary<GameObject, bool>;
            if (markedBalls != null && markedBalls.ContainsKey(ball))
            {
                markedBalls.Remove(ball);
                Debug.Log($"🧹 Cleared marked status for {ball.name}");
            }
        }
    }

    /// <summary>
    /// 创建测试网球在合理位置
    /// </summary>
    public void CreateTestTennisBall()
    {
        Debug.Log("=== Creating Test Tennis Ball ===");

        // 在场地中心上方创建测试网球
        Vector3 testPos = new Vector3(0, 3f, 0);

        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "TennisBall_Test";
        testBall.transform.position = testPos;
        testBall.transform.localScale = Vector3.one * 0.065f; // 网球大小

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f; // 网球质量
        rb.drag = 0.02f;
        rb.useGravity = true;

        // 设置材质
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.yellow;
        renderer.material = mat;

        // 给一个初始速度
        rb.velocity = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));

        Debug.Log($"✅ Test tennis ball created at {testPos}");
        Debug.Log($"   Initial velocity: {rb.velocity}");

        // 10秒后销毁
        Destroy(testBall, 10f);
    }
}