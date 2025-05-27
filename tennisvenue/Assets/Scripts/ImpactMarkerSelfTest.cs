using UnityEngine;
using System.Collections;

/// <summary>
/// 第一落地点圆环标识自测工具
/// 全面测试反弹冲击标记系统的各项功能
/// </summary>
public class ImpactMarkerSelfTest : MonoBehaviour
{
    [Header("自测设置")]
    [Tooltip("是否启用自动测试")]
    public bool enableAutoTest = true;

    [Tooltip("测试间隔时间（秒）")]
    public float testInterval = 3f;

    [Tooltip("测试网球数量")]
    public int testBallCount = 5;

    private BounceImpactMarker impactMarker;
    private int testPhase = 0;
    private float lastTestTime = 0f;

    void Start()
    {
        Debug.Log("=== 第一落地点圆环标识自测工具启动 ===");
        Debug.Log("快捷键说明:");
        Debug.Log("  F11: 开始完整自测流程");
        Debug.Log("  F12: 创建单个测试网球");
        Debug.Log("  Ctrl+F11: 测试不同速度的圆环");
        Debug.Log("  Ctrl+F12: 测试圆环可见性");
        Debug.Log("  Alt+F11: 重置所有测试");

        // 查找冲击标记系统
        impactMarker = FindObjectOfType<BounceImpactMarker>();
        if (impactMarker == null)
        {
            Debug.LogError("❌ 未找到BounceImpactMarker系统！");
            Debug.LogError("   请确保场景中存在BounceImpactMarker组件");
        }
        else
        {
            Debug.Log("✅ BounceImpactMarker系统已找到");
            CheckSystemStatus();
        }
    }

    void Update()
    {
        // 自动测试流程
        if (enableAutoTest && Time.time - lastTestTime > testInterval)
        {
            RunAutoTest();
            lastTestTime = Time.time;
        }

        // 快捷键控制
        HandleKeyboardInput();
    }

    /// <summary>
    /// 处理键盘输入
    /// </summary>
    void HandleKeyboardInput()
    {
        // F11: 完整自测流程
        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                TestDifferentSpeeds();
            }
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                ResetAllTests();
            }
            else
            {
                StartCompleteTest();
            }
        }

        // F12: 单个测试网球
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                TestRingVisibility();
            }
            else
            {
                CreateSingleTestBall();
            }
        }
    }

    /// <summary>
    /// 检查系统状态
    /// </summary>
    void CheckSystemStatus()
    {
        if (impactMarker == null) return;

        Debug.Log("🔍 系统状态检查:");
        Debug.Log($"   启用冲击标记: {impactMarker.enableImpactMarkers}");
        Debug.Log($"   基础圆环大小: {impactMarker.baseRingSize}");
        Debug.Log($"   标记生命周期: {impactMarker.markerLifetime}秒");
        Debug.Log($"   启用发光效果: {impactMarker.enableGlow}");
        Debug.Log($"   启用渐变消失: {impactMarker.enableFadeOut}");

        // 检查速度阈值
        Debug.Log("   速度阈值设置:");
        Debug.Log($"     低速阈值: {impactMarker.lowSpeedThreshold}m/s");
        Debug.Log($"     中速阈值: {impactMarker.mediumSpeedThreshold}m/s");
        Debug.Log($"     高速阈值: {impactMarker.highSpeedThreshold}m/s");

        // 检查当前活动标记数量
        int activeMarkers = impactMarker.GetActiveMarkerCount();
        Debug.Log($"   当前活动标记数: {activeMarkers}");
    }

    /// <summary>
    /// 开始完整测试流程
    /// </summary>
    void StartCompleteTest()
    {
        Debug.Log("=== 开始完整自测流程 ===");

        // 第一阶段：系统检查
        Debug.Log("📋 第一阶段：系统状态检查");
        CheckSystemStatus();

        // 第二阶段：创建测试网球
        Debug.Log("🎾 第二阶段：创建测试网球");
        StartCoroutine(CreateTestBallsSequence());
    }

    /// <summary>
    /// 创建测试网球序列
    /// </summary>
    IEnumerator CreateTestBallsSequence()
    {
        for (int i = 0; i < testBallCount; i++)
        {
            Vector3 startPos = new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(2f, 4f),
                Random.Range(-1f, 1f)
            );

            CreateTestBallAtPosition(startPos, $"TestBall_{i + 1}");

            Debug.Log($"✅ 创建测试网球 {i + 1}/{testBallCount} 在位置 {startPos}");

            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("🎯 所有测试网球已创建，等待落地检测...");

        // 开始监控测试结果
        StartCoroutine(MonitorTestResults());
    }

    /// <summary>
    /// 在指定位置创建测试网球
    /// </summary>
    void CreateTestBallAtPosition(Vector3 position, string ballName)
    {
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = $"TennisBall_{ballName}";
        testBall.transform.position = position;
        testBall.transform.localScale = Vector3.one * 0.065f; // 标准网球大小

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f; // 标准网球质量
        rb.drag = 0.02f;
        rb.useGravity = true;

        // 设置随机初始速度
        Vector3 randomVelocity = new Vector3(
            Random.Range(-3f, 3f),
            Random.Range(-1f, 1f),
            Random.Range(-3f, 3f)
        );
        rb.velocity = randomVelocity;

        // 设置材质
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 0.2f);
        renderer.material = mat;

        // 10秒后自动销毁
        Destroy(testBall, 10f);

        Debug.Log($"🎾 创建测试网球: {ballName}");
        Debug.Log($"   位置: {position}");
        Debug.Log($"   初始速度: {randomVelocity}");
    }

    /// <summary>
    /// 监控测试结果
    /// </summary>
    IEnumerator MonitorTestResults()
    {
        float monitorTime = 0f;
        int initialMarkerCount = impactMarker != null ? impactMarker.GetActiveMarkerCount() : 0;

        while (monitorTime < 8f) // 监控8秒
        {
            yield return new WaitForSeconds(1f);
            monitorTime += 1f;

            if (impactMarker != null)
            {
                int currentMarkerCount = impactMarker.GetActiveMarkerCount();
                int newMarkers = currentMarkerCount - initialMarkerCount;

                Debug.Log($"⏱️ 监控时间: {monitorTime}s - 新增圆环标记: {newMarkers}");

                // 检查是否有网球仍在空中
                int ballsInAir = CountBallsInAir();
                Debug.Log($"   空中网球数量: {ballsInAir}");
            }
        }

        Debug.Log("📊 测试结果总结:");
        SummarizeTestResults();
    }

    /// <summary>
    /// 统计空中网球数量
    /// </summary>
    int CountBallsInAir()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall_TestBall"))
            {
                if (obj.transform.position.y > 0.5f)
                {
                    count++;
                }
            }
        }

        return count;
    }

    /// <summary>
    /// 总结测试结果
    /// </summary>
    void SummarizeTestResults()
    {
        if (impactMarker == null)
        {
            Debug.LogError("❌ 无法总结测试结果：BounceImpactMarker系统未找到");
            return;
        }

        int totalMarkers = impactMarker.GetActiveMarkerCount();

        Debug.Log("=== 测试结果总结 ===");
        Debug.Log($"✅ 当前活动圆环标记数量: {totalMarkers}");
        Debug.Log($"✅ 预期标记数量: {testBallCount}");

        if (totalMarkers >= testBallCount * 0.8f) // 80%成功率
        {
            Debug.Log("🎉 测试成功！圆环标记系统工作正常");
        }
        else if (totalMarkers > 0)
        {
            Debug.LogWarning("⚠️ 部分测试成功，可能存在检测遗漏");
        }
        else
        {
            Debug.LogError("❌ 测试失败！未检测到任何圆环标记");
            Debug.LogError("   请检查BounceImpactMarker系统设置");
        }

        // 检查现有标记
        CheckExistingMarkers();
    }

    /// <summary>
    /// 检查现有标记
    /// </summary>
    void CheckExistingMarkers()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int markerCount = 0;

        Debug.Log("🔍 检查场景中的圆环标记:");

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring"))
            {
                markerCount++;
                Debug.Log($"   找到标记: {obj.name} 位置: {obj.transform.position}");

                // 检查标记组件
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Debug.Log($"     材质: {renderer.material.name}");
                    Debug.Log($"     颜色: {renderer.material.color}");
                }
            }
        }

        Debug.Log($"📊 场景中总共找到 {markerCount} 个标记对象");
    }

    /// <summary>
    /// 创建单个测试网球
    /// </summary>
    void CreateSingleTestBall()
    {
        Debug.Log("🎾 创建单个测试网球");

        Vector3 testPos = new Vector3(0, 3f, 0);
        CreateTestBallAtPosition(testPos, "Single");

        Debug.Log("✅ 单个测试网球已创建，观察是否产生圆环标记");
    }

    /// <summary>
    /// 测试不同速度的圆环
    /// </summary>
    void TestDifferentSpeeds()
    {
        Debug.Log("🌈 测试不同速度的圆环标记");

        StartCoroutine(CreateSpeedTestSequence());
    }

    /// <summary>
    /// 创建速度测试序列
    /// </summary>
    IEnumerator CreateSpeedTestSequence()
    {
        float[] testSpeeds = { 3f, 7f, 12f, 18f }; // 低、中、高、极高速
        string[] speedNames = { "Low", "Medium", "High", "VeryHigh" };

        for (int i = 0; i < testSpeeds.Length; i++)
        {
            Vector3 startPos = new Vector3(i * 1.5f - 2f, 3f, 0);

            GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            testBall.name = $"TennisBall_Speed_{speedNames[i]}";
            testBall.transform.position = startPos;
            testBall.transform.localScale = Vector3.one * 0.065f;

            Rigidbody rb = testBall.AddComponent<Rigidbody>();
            rb.mass = 0.057f;
            rb.drag = 0.02f;
            rb.useGravity = true;
            rb.velocity = Vector3.down * testSpeeds[i];

            // 设置不同颜色
            Renderer renderer = testBall.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.HSVToRGB(i * 0.25f, 1f, 1f);
            renderer.material = mat;

            Destroy(testBall, 8f);

            Debug.Log($"🎯 创建{speedNames[i]}测试球 (速度: {testSpeeds[i]}m/s)");

            yield return new WaitForSeconds(1f);
        }

        Debug.Log("🌈 不同速度测试球已全部创建");
    }

    /// <summary>
    /// 测试圆环可见性
    /// </summary>
    void TestRingVisibility()
    {
        Debug.Log("👁️ 测试圆环可见性");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker系统未找到");
            return;
        }

        // 直接创建可见的测试圆环
        Vector3[] testPositions = {
            new Vector3(0, 0.05f, 2),      // 前方
            new Vector3(-2, 0.05f, 0),     // 左侧
            new Vector3(2, 0.05f, 0),      // 右侧
            new Vector3(0, 0.05f, -2)      // 后方
        };

        for (int i = 0; i < testPositions.Length; i++)
        {
            // 使用反射调用CreateImpactMarker方法
            var method = impactMarker.GetType().GetMethod("CreateImpactMarker",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                float testSpeed = 5f + i * 3f; // 不同速度
                Vector3 testVelocity = Vector3.down * testSpeed;

                method.Invoke(impactMarker, new object[] { testPositions[i], testSpeed, testVelocity });

                Debug.Log($"✅ 创建可见性测试圆环 {i + 1} - 位置: {testPositions[i]}, 速度: {testSpeed}m/s");
            }
        }

        Debug.Log("👁️ 可见性测试圆环已创建，请检查场景中的圆环标记");
    }

    /// <summary>
    /// 重置所有测试
    /// </summary>
    void ResetAllTests()
    {
        Debug.Log("🔄 重置所有测试");

        // 清除所有测试网球
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int removedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall_Test") || obj.name.Contains("TennisBall_Speed"))
            {
                Destroy(obj);
                removedCount++;
            }
        }

        Debug.Log($"🧹 已清除 {removedCount} 个测试网球");

        // 清除所有圆环标记
        if (impactMarker != null)
        {
            // 使用反射调用ClearAllMarkers方法
            var method = impactMarker.GetType().GetMethod("ClearAllMarkers",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(impactMarker, null);
                Debug.Log("🧹 已清除所有圆环标记");
            }
        }

        // 重置测试状态
        testPhase = 0;
        lastTestTime = 0f;

        Debug.Log("✅ 测试重置完成");
    }

    /// <summary>
    /// 自动测试流程
    /// </summary>
    void RunAutoTest()
    {
        if (!enableAutoTest) return;

        switch (testPhase)
        {
            case 0:
                Debug.Log("🔄 自动测试阶段 1: 系统检查");
                CheckSystemStatus();
                testPhase++;
                break;

            case 1:
                Debug.Log("🔄 自动测试阶段 2: 创建测试网球");
                CreateSingleTestBall();
                testPhase++;
                break;

            case 2:
                Debug.Log("🔄 自动测试阶段 3: 检查结果");
                CheckExistingMarkers();
                testPhase = 0; // 重置循环
                break;
        }
    }
}