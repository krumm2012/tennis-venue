using UnityEngine;

/// <summary>
/// 反弹冲击标记测试脚本 - 验证圆环标记系统功能
/// </summary>
public class ImpactMarkerTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool autoRunTest = true;
    public float testInterval = 3f;

    private BounceImpactMarker impactMarker;
    private float lastTestTime;

    void Start()
    {
        // 查找反弹冲击标记系统
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (impactMarker == null)
        {
            Debug.LogWarning("BounceImpactMarker system not found!");
            return;
        }

        Debug.Log("=== Impact Marker Test Started ===");
        Debug.Log("Press F6 to run manual test");
        Debug.Log("Press F7 to test different speed ranges");
        Debug.Log("Press F8 to test marker cleanup");

        if (autoRunTest)
        {
            Debug.Log("Auto test will run every 3 seconds");
        }
    }

    void Update()
    {
        // 自动测试
        if (autoRunTest && Time.time - lastTestTime > testInterval)
        {
            RunRandomSpeedTest();
            lastTestTime = Time.time;
        }

        // 手动测试快捷键
        if (Input.GetKeyDown(KeyCode.F6))
        {
            RunManualTest();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            TestDifferentSpeedRanges();
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            TestMarkerCleanup();
        }
    }

    /// <summary>
    /// 运行随机速度测试
    /// </summary>
    void RunRandomSpeedTest()
    {
        if (impactMarker == null) return;

        // 随机生成测试位置和速度
        Vector3 testPosition = new Vector3(
            Random.Range(-3f, 3f),
            0.01f,
            Random.Range(0f, 5f)
        );

        float testSpeed = Random.Range(3f, 20f);

        // 手动创建冲击标记进行测试
        impactMarker.GetType().GetMethod("CreateImpactMarker",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(impactMarker, new object[] { testPosition, testSpeed, Vector3.down * testSpeed });

        Debug.Log($"🧪 Auto test: Speed {testSpeed:F1}m/s at {testPosition}");
    }

    /// <summary>
    /// 运行手动测试
    /// </summary>
    void RunManualTest()
    {
        Debug.Log("=== Running Manual Impact Marker Test ===");

        if (impactMarker == null)
        {
            Debug.LogError("BounceImpactMarker system not found!");
            return;
        }

        // 创建测试网球
        CreateTestTennisBall();

        Debug.Log("Test tennis ball created - watch for impact marker when it lands");
    }

    /// <summary>
    /// 测试不同速度范围
    /// </summary>
    void TestDifferentSpeedRanges()
    {
        Debug.Log("=== Testing Different Speed Ranges ===");

        if (impactMarker == null) return;

        // 测试低速（绿色）
        CreateTestMarker(new Vector3(-2, 0.01f, 1), 3f, "Low Speed (Green)");

        // 测试中速（黄色）
        CreateTestMarker(new Vector3(-1, 0.01f, 1), 7f, "Medium Speed (Yellow)");

        // 测试高速（红色）
        CreateTestMarker(new Vector3(0, 0.01f, 1), 12f, "High Speed (Red)");

        // 测试极高速（紫色）
        CreateTestMarker(new Vector3(1, 0.01f, 1), 18f, "Extreme Speed (Magenta)");

        Debug.Log("Speed range test completed - check the different colored rings");
    }

    /// <summary>
    /// 测试标记清理功能
    /// </summary>
    void TestMarkerCleanup()
    {
        Debug.Log("=== Testing Marker Cleanup ===");

        if (impactMarker == null) return;

        int markerCount = impactMarker.GetActiveMarkerCount();
        Debug.Log($"Current active markers: {markerCount}");

        if (markerCount > 0)
        {
            impactMarker.ClearAllImpactMarkers();
            Debug.Log("All markers cleared");
        }
        else
        {
            Debug.Log("No markers to clear");
        }
    }

    /// <summary>
    /// 创建测试网球
    /// </summary>
    void CreateTestTennisBall()
    {
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "TennisBall_Test";
        testBall.transform.position = new Vector3(0, 3f, 0);
        testBall.transform.localScale = Vector3.one * 0.067f;

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.02f;
        rb.angularDrag = 0.02f;

        // 设置材质
        Collider collider = testBall.GetComponent<Collider>();
        PhysicMaterial ballMat = new PhysicMaterial("TestBall");
        ballMat.bounciness = 0.85f;
        ballMat.dynamicFriction = 0.6f;
        ballMat.staticFriction = 0.6f;
        ballMat.frictionCombine = PhysicMaterialCombine.Average;
        ballMat.bounceCombine = PhysicMaterialCombine.Maximum;
        collider.material = ballMat;

        // 设置橙色材质便于识别
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material orangeMat = new Material(Shader.Find("Standard"));
        orangeMat.color = Color.magenta;
        renderer.material = orangeMat;

        // 给球一个初始速度
        Vector3 initialVelocity = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(-1f, 1f),
            Random.Range(2f, 5f)
        );
        rb.velocity = initialVelocity;

        // 5秒后自动销毁
        Destroy(testBall, 5f);

        Debug.Log($"Test ball created with velocity: {initialVelocity}");
    }

    /// <summary>
    /// 创建测试标记
    /// </summary>
    void CreateTestMarker(Vector3 position, float speed, string description)
    {
        if (impactMarker == null) return;

        // 使用反射调用私有方法
        var method = impactMarker.GetType().GetMethod("CreateImpactMarker",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(impactMarker, new object[] { position, speed, Vector3.down * speed });
            Debug.Log($"✅ {description} marker created at {position}");
        }
        else
        {
            Debug.LogError("Could not find CreateImpactMarker method");
        }
    }

    /// <summary>
    /// 显示系统状态
    /// </summary>
    [ContextMenu("Show System Status")]
    public void ShowSystemStatus()
    {
        if (impactMarker != null)
        {
            Debug.Log("=== Impact Marker System Status ===");
            Debug.Log(impactMarker.GetSystemStatus());
        }
        else
        {
            Debug.LogWarning("BounceImpactMarker system not found");
        }
    }
}