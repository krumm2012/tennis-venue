using UnityEngine;

/// <summary>
/// 网球落地冲击诊断器 - 专门诊断为什么实际网球落点圆环无法显示
/// </summary>
public class TennisBallImpactDiagnostic : MonoBehaviour
{
    [Header("诊断设置")]
    [Tooltip("是否启用详细日志")]
    public bool enableVerboseLogging = true;

    [Tooltip("检查频率（每N帧检查一次）")]
    public int checkFrequency = 30;

    private BounceImpactMarker impactMarker;
    private int frameCount = 0;

    void Start()
    {
        Debug.Log("=== Tennis Ball Impact Diagnostic Started ===");
        Debug.Log("Press Shift+F5 to run detailed diagnostic");
        Debug.Log("Press Shift+F6 to force create impact at tennis ball position");
        Debug.Log("Press Shift+F7 to check impact marker system status");

        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker system not found!");
        }
        else
        {
            Debug.Log("✅ BounceImpactMarker system found and active");
        }
    }

    void Update()
    {
        frameCount++;

        // 定期检查网球状态
        if (enableVerboseLogging && frameCount % checkFrequency == 0)
        {
            CheckTennisBallStatus();
        }

        // 快捷键控制
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F5))
        {
            RunDetailedDiagnostic();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F6))
        {
            ForceCreateImpactAtTennisBall();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F7))
        {
            CheckImpactMarkerSystemStatus();
        }
    }

    /// <summary>
    /// 检查网球状态
    /// </summary>
    void CheckTennisBallStatus()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int tennisBallCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                tennisBallCount++;
                Rigidbody rb = obj.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Vector3 pos = obj.transform.position;
                    Vector3 vel = rb.velocity;
                    float speed = vel.magnitude;

                    // 检查是否满足冲击条件
                    bool heightCondition = pos.y <= 0.5f;
                    bool velocityCondition = vel.y < -0.5f;
                    bool speedCondition = speed > 1.5f;

                    if (heightCondition || velocityCondition || speedCondition)
                    {
                        Debug.Log($"🎾 Ball {obj.name} status:");
                        Debug.Log($"   Position: {pos} (Height: {pos.y:F3}m)");
                        Debug.Log($"   Velocity: {vel} (Speed: {speed:F2}m/s)");
                        Debug.Log($"   Conditions: Height({heightCondition}) Velocity({velocityCondition}) Speed({speedCondition})");

                        // 检查是否应该触发冲击标记
                        if (heightCondition && velocityCondition && speedCondition)
                        {
                            Debug.LogWarning($"⚠️ Ball {obj.name} meets ALL impact conditions but no marker created!");
                            Debug.LogWarning($"   This suggests the impact detection logic may have issues");
                        }
                    }
                }
            }
        }

        if (tennisBallCount == 0)
        {
            Debug.Log("📊 No tennis balls found in scene");
        }
    }

    /// <summary>
    /// 运行详细诊断
    /// </summary>
    void RunDetailedDiagnostic()
    {
        Debug.Log("=== Running Detailed Tennis Ball Impact Diagnostic ===");

        // 1. 检查BounceImpactMarker系统状态
        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker system not found!");
            return;
        }

        Debug.Log($"✅ BounceImpactMarker system status:");
        Debug.Log($"   Enabled: {impactMarker.enableImpactMarkers}");
        Debug.Log($"   Active markers: {impactMarker.GetActiveMarkerCount()}");

        // 2. 检查所有网球
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int tennisBallCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                tennisBallCount++;
                AnalyzeTennisBall(obj);
            }
        }

        Debug.Log($"📊 Total tennis balls analyzed: {tennisBallCount}");

        // 3. 检查地面设置
        CheckGroundSetup();

        // 4. 检查现有圆环标记
        CheckExistingImpactMarkers();

        Debug.Log("=== Diagnostic Complete ===");
    }

    /// <summary>
    /// 分析单个网球
    /// </summary>
    void AnalyzeTennisBall(GameObject ball)
    {
        Debug.Log($"🔍 Analyzing tennis ball: {ball.name}");

        // 检查组件
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        Collider col = ball.GetComponent<Collider>();

        Debug.Log($"   Has Rigidbody: {rb != null}");
        Debug.Log($"   Has Collider: {col != null}");

        if (rb != null)
        {
            Vector3 pos = ball.transform.position;
            Vector3 vel = rb.velocity;

            Debug.Log($"   Position: {pos}");
            Debug.Log($"   Velocity: {vel}");
            Debug.Log($"   Speed: {vel.magnitude:F2}m/s");
            Debug.Log($"   Is Kinematic: {rb.isKinematic}");
            Debug.Log($"   Use Gravity: {rb.useGravity}");
            Debug.Log($"   Mass: {rb.mass}");
            Debug.Log($"   Drag: {rb.drag}");

            // 检查是否满足冲击条件
            bool heightCondition = pos.y <= 0.5f;
            bool velocityCondition = vel.y < -0.5f;
            bool speedCondition = vel.magnitude > 1.5f;

            Debug.Log($"   Impact conditions:");
            Debug.Log($"     Height ≤ 0.5m: {heightCondition} (current: {pos.y:F3}m)");
            Debug.Log($"     Velocity Y < -0.5: {velocityCondition} (current: {vel.y:F2})");
            Debug.Log($"     Speed > 1.5m/s: {speedCondition} (current: {vel.magnitude:F2})");

            bool shouldCreateImpact = heightCondition && velocityCondition && speedCondition;
            Debug.Log($"   Should create impact: {shouldCreateImpact}");
        }
    }

    /// <summary>
    /// 检查地面设置
    /// </summary>
    void CheckGroundSetup()
    {
        Debug.Log("🏟️ Checking ground setup:");

        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Debug.Log($"   Floor found: {floor.name}");
            Debug.Log($"   Position: {floor.transform.position}");
            Debug.Log($"   Scale: {floor.transform.localScale}");

            Collider floorCollider = floor.GetComponent<Collider>();
            Debug.Log($"   Has Collider: {floorCollider != null}");

            if (floorCollider != null)
            {
                Debug.Log($"   Collider type: {floorCollider.GetType().Name}");
                Debug.Log($"   Is Trigger: {floorCollider.isTrigger}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Floor object not found!");
        }
    }

    /// <summary>
    /// 检查现有冲击标记
    /// </summary>
    void CheckExistingImpactMarkers()
    {
        Debug.Log("⭕ Checking existing impact markers:");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int markerCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("TestImpactMarker"))
            {
                markerCount++;
                Debug.Log($"   Found marker: {obj.name} at {obj.transform.position}");
            }
        }

        Debug.Log($"   Total markers found: {markerCount}");
    }

    /// <summary>
    /// 强制在网球位置创建冲击标记
    /// </summary>
    void ForceCreateImpactAtTennisBall()
    {
        Debug.Log("=== Force Creating Impact at Tennis Ball Position ===");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundTennisBall = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                foundTennisBall = true;
                Vector3 ballPos = obj.transform.position;
                Vector3 impactPos = new Vector3(ballPos.x, 0.01f, ballPos.z); // 强制设置到地面高度

                Debug.Log($"Creating forced impact marker for {obj.name}");
                Debug.Log($"Ball position: {ballPos}");
                Debug.Log($"Impact position: {impactPos}");

                // 直接调用BounceImpactMarker的CreateImpactMarker方法
                if (impactMarker != null)
                {
                    // 使用反射调用私有方法
                    var method = impactMarker.GetType().GetMethod("CreateImpactMarker",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (method != null)
                    {
                        Rigidbody rb = obj.GetComponent<Rigidbody>();
                        float speed = rb != null ? rb.velocity.magnitude : 10f;
                        Vector3 velocity = rb != null ? rb.velocity : Vector3.down * speed;

                        method.Invoke(impactMarker, new object[] { impactPos, speed, velocity });
                        Debug.Log($"✅ Forced impact marker created with speed: {speed:F2}m/s");
                    }
                    else
                    {
                        Debug.LogError("❌ Could not find CreateImpactMarker method");
                    }
                }

                break; // 只处理第一个找到的网球
            }
        }

        if (!foundTennisBall)
        {
            Debug.LogWarning("⚠️ No tennis balls found to create impact marker");
        }
    }

    /// <summary>
    /// 检查冲击标记系统状态
    /// </summary>
    void CheckImpactMarkerSystemStatus()
    {
        Debug.Log("=== Impact Marker System Status ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker system not found!");
            return;
        }

        Debug.Log($"System Status: {impactMarker.GetSystemStatus()}");
        Debug.Log($"Enable Impact Markers: {impactMarker.enableImpactMarkers}");
        Debug.Log($"Base Ring Size: {impactMarker.baseRingSize}");
        Debug.Log($"Marker Lifetime: {impactMarker.markerLifetime}s");
        Debug.Log($"Enable Glow: {impactMarker.enableGlow}");
        Debug.Log($"Enable Fade Out: {impactMarker.enableFadeOut}");

        // 检查速度阈值
        Debug.Log($"Speed Thresholds:");
        Debug.Log($"   Low Speed: <{impactMarker.lowSpeedThreshold}m/s");
        Debug.Log($"   Medium Speed: <{impactMarker.mediumSpeedThreshold}m/s");
        Debug.Log($"   High Speed: <{impactMarker.highSpeedThreshold}m/s");
    }
}