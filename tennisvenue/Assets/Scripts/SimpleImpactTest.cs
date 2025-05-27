using UnityEngine;

/// <summary>
/// 简单冲击标记测试 - 专门诊断ImpactMarker_Ring不显示的问题
/// </summary>
public class SimpleImpactTest : MonoBehaviour
{
    [Header("测试设置")]
    [Tooltip("测试网球掉落高度")]
    public float dropHeight = 3f;

    [Tooltip("是否启用详细诊断")]
    public bool enableDetailedDiagnostic = true;

    private BounceImpactMarker impactMarker;

    void Start()
    {
        Debug.Log("=== Simple Impact Test - ImpactMarker_Ring 诊断 ===");
        Debug.Log("快捷键说明:");
        Debug.Log("  空格键: 创建测试网球（场地中央掉落）");
        Debug.Log("  Enter键: 检查系统状态和标记统计");
        Debug.Log("  Delete键: 清除所有测试网球");
        Debug.Log("  F9: 运行完整诊断");
        Debug.Log("  F10: 强制创建可见圆环");

        // 查找冲击标记系统
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (impactMarker == null)
        {
            Debug.LogError("❌ 未找到BounceImpactMarker系统！");
            Debug.LogError("   请确保场景中存在BounceImpactMarkerSystem对象");
        }
        else
        {
            Debug.Log("✅ BounceImpactMarker系统已找到");
            CheckInitialSystemStatus();
        }
    }

    void Update()
    {
        // 快捷键控制
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateTestTennisBall();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckSystemStatusAndMarkers();
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            ClearAllTestBalls();
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            RunFullDiagnostic();
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            ForceCreateVisibleRing();
        }

        // 实时监控网球状态
        if (enableDetailedDiagnostic)
        {
            MonitorTennisBalls();
        }
    }

    /// <summary>
    /// 检查初始系统状态
    /// </summary>
    void CheckInitialSystemStatus()
    {
        if (impactMarker == null) return;

        Debug.Log("=== 初始系统状态检查 ===");
        Debug.Log($"✅ 系统启用: {impactMarker.enableImpactMarkers}");
        Debug.Log($"✅ 基础圆环大小: {impactMarker.baseRingSize}");
        Debug.Log($"✅ 圆环厚度: {impactMarker.ringThickness}");
        Debug.Log($"✅ 标记生命周期: {impactMarker.markerLifetime}秒");
        Debug.Log($"✅ 发光效果: {impactMarker.enableGlow}");
        Debug.Log($"✅ 当前活动标记数: {impactMarker.GetActiveMarkerCount()}");

        if (!impactMarker.enableImpactMarkers)
        {
            Debug.LogWarning("⚠️ 冲击标记系统已禁用！按F3启用");
        }
    }

    /// <summary>
    /// 创建测试网球
    /// </summary>
    void CreateTestTennisBall()
    {
        Debug.Log("=== 创建测试网球 ===");

        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "TennisBall_SimpleTest";
        testBall.transform.position = new Vector3(0, dropHeight, 0);
        testBall.transform.localScale = Vector3.one * 0.067f; // 网球大小

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f; // 网球质量
        rb.drag = 0.02f;
        rb.angularDrag = 0.02f;

        // 设置物理材质
        Collider collider = testBall.GetComponent<Collider>();
        PhysicMaterial ballMat = new PhysicMaterial("TestBallMaterial");
        ballMat.bounciness = 0.85f;
        ballMat.dynamicFriction = 0.6f;
        ballMat.staticFriction = 0.6f;
        ballMat.frictionCombine = PhysicMaterialCombine.Average;
        ballMat.bounceCombine = PhysicMaterialCombine.Maximum;
        collider.material = ballMat;

        // 设置明显的颜色便于识别
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material ballMaterial = new Material(Shader.Find("Standard"));
        ballMaterial.color = Color.cyan; // 青色便于识别
        ballMaterial.EnableKeyword("_EMISSION");
        ballMaterial.SetColor("_EmissionColor", Color.cyan * 0.5f);
        renderer.material = ballMaterial;

        // 给球一个小的随机初始速度
        Vector3 randomVelocity = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        );
        rb.velocity = randomVelocity;

        Debug.Log($"✅ 测试网球已创建:");
        Debug.Log($"   位置: {testBall.transform.position}");
        Debug.Log($"   初始速度: {randomVelocity}");
        Debug.Log($"   质量: {rb.mass}kg");
        Debug.Log($"   反弹系数: {ballMat.bounciness}");

        // 10秒后自动销毁
        Destroy(testBall, 10f);

        // 开始监控这个球
        StartCoroutine(MonitorSpecificBall(testBall));
    }

    /// <summary>
    /// 监控特定网球的状态
    /// </summary>
    System.Collections.IEnumerator MonitorSpecificBall(GameObject ball)
    {
        Debug.Log($"🔍 开始监控网球: {ball.name}");

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        bool hasLanded = false;
        float lastHeight = ball.transform.position.y;

        while (ball != null && !hasLanded)
        {
            Vector3 currentPos = ball.transform.position;
            Vector3 currentVel = rb.velocity;

            // 检查是否接近地面
            if (currentPos.y <= 0.5f && currentVel.y < 0)
            {
                Debug.Log($"⚠️ 网球接近地面:");
                Debug.Log($"   位置: {currentPos}");
                Debug.Log($"   速度: {currentVel} (大小: {currentVel.magnitude:F2}m/s)");
                Debug.Log($"   垂直速度: {currentVel.y:F2}m/s");
            }

            // 检查是否已经落地
            if (currentPos.y <= 0.3f && lastHeight > 0.3f)
            {
                hasLanded = true;
                Debug.Log($"🎯 网球落地检测:");
                Debug.Log($"   落地位置: {currentPos}");
                Debug.Log($"   落地速度: {currentVel.magnitude:F2}m/s");
                Debug.Log($"   垂直速度: {currentVel.y:F2}m/s");

                // 等待一秒后检查是否创建了圆环标记
                yield return new WaitForSeconds(1f);
                CheckForNewRingMarkers(currentPos);
            }

            lastHeight = currentPos.y;
            yield return new WaitForSeconds(0.1f); // 每0.1秒检查一次
        }

        if (ball == null)
        {
            Debug.Log("🔍 网球监控结束（对象已销毁）");
        }
    }

    /// <summary>
    /// 检查是否创建了新的圆环标记
    /// </summary>
    void CheckForNewRingMarkers(Vector3 landingPosition)
    {
        Debug.Log("=== 检查圆环标记创建结果 ===");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundRingNearLanding = false;
        int totalRings = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring") || obj.name.Contains("Impact"))
            {
                totalRings++;
                Vector3 ringPos = obj.transform.position;
                float distance = Vector3.Distance(ringPos, landingPosition);

                Debug.Log($"   找到圆环: {obj.name}");
                Debug.Log($"     位置: {ringPos}");
                Debug.Log($"     距离落地点: {distance:F2}m");

                if (distance < 2f) // 2米范围内认为是相关的
                {
                    foundRingNearLanding = true;
                    Debug.Log($"     ✅ 这个圆环可能是刚创建的！");

                    // 检查圆环的详细信息
                    CheckRingDetails(obj);
                }
            }
        }

        Debug.Log($"📊 检查结果:");
        Debug.Log($"   总圆环数: {totalRings}");
        Debug.Log($"   落地点附近的圆环: {(foundRingNearLanding ? "有" : "无")}");

        if (!foundRingNearLanding && totalRings == 0)
        {
            Debug.LogWarning("❌ 未发现任何圆环标记！");
            Debug.LogWarning("   可能的问题:");
            Debug.LogWarning("   1. BounceImpactMarker系统未启用");
            Debug.LogWarning("   2. 网球速度不满足触发条件");
            Debug.LogWarning("   3. 圆环创建失败");
            Debug.LogWarning("   4. 圆环位置不正确或不可见");
        }
    }

    /// <summary>
    /// 检查圆环详细信息
    /// </summary>
    void CheckRingDetails(GameObject ring)
    {
        Debug.Log($"🔍 检查圆环详细信息: {ring.name}");

        // 检查Transform
        Transform t = ring.transform;
        Debug.Log($"   位置: {t.position}");
        Debug.Log($"   旋转: {t.rotation.eulerAngles}");
        Debug.Log($"   缩放: {t.localScale}");
        Debug.Log($"   激活状态: {ring.activeInHierarchy}");

        // 检查Renderer
        Renderer renderer = ring.GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log($"   渲染器启用: {renderer.enabled}");
            Debug.Log($"   可见性: {renderer.isVisible}");
            Debug.Log($"   边界: {renderer.bounds}");

            if (renderer.material != null)
            {
                Material mat = renderer.material;
                Debug.Log($"   材质: {mat.name}");
                Debug.Log($"   着色器: {mat.shader.name}");
                Debug.Log($"   颜色: {mat.color}");
                Debug.Log($"   渲染队列: {mat.renderQueue}");
            }
        }
        else
        {
            Debug.LogWarning($"   ⚠️ 缺少Renderer组件！");
        }

        // 检查MeshFilter
        MeshFilter meshFilter = ring.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            Mesh mesh = meshFilter.mesh;
            Debug.Log($"   网格: {mesh.name}");
            Debug.Log($"   顶点数: {mesh.vertexCount}");
            Debug.Log($"   三角形数: {mesh.triangles.Length / 3}");
            Debug.Log($"   边界: {mesh.bounds}");
        }
        else
        {
            Debug.LogWarning($"   ⚠️ 缺少有效的Mesh！");
        }
    }

    /// <summary>
    /// 检查系统状态和标记
    /// </summary>
    void CheckSystemStatusAndMarkers()
    {
        Debug.Log("=== 系统状态和标记检查 ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker系统未找到");
            return;
        }

        Debug.Log($"系统状态: {impactMarker.GetSystemStatus()}");

        // 统计场景中的对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ballCount = 0;
        int ringCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                ballCount++;
                Debug.Log($"   网球: {obj.name} 位置: {obj.transform.position}");
            }
            else if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring"))
            {
                ringCount++;
                Debug.Log($"   圆环: {obj.name} 位置: {obj.transform.position}");
            }
        }

        Debug.Log($"📊 场景统计: {ballCount}个网球, {ringCount}个圆环标记");
    }

    /// <summary>
    /// 清除所有测试网球
    /// </summary>
    void ClearAllTestBalls()
    {
        Debug.Log("=== 清除所有测试网球 ===");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int clearedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall_SimpleTest") || obj.name.Contains("TennisBall_Test"))
            {
                Debug.Log($"清除测试网球: {obj.name}");
                Destroy(obj);
                clearedCount++;
            }
        }

        Debug.Log($"✅ 已清除 {clearedCount} 个测试网球");
    }

    /// <summary>
    /// 运行完整诊断
    /// </summary>
    void RunFullDiagnostic()
    {
        Debug.Log("=== 运行完整诊断 ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker系统未找到");
            return;
        }

        // 1. 系统设置检查
        Debug.Log("1. 系统设置:");
        Debug.Log($"   启用状态: {impactMarker.enableImpactMarkers}");
        Debug.Log($"   基础大小: {impactMarker.baseRingSize}");
        Debug.Log($"   最小大小: {impactMarker.minRingSize}");
        Debug.Log($"   最大大小: {impactMarker.maxRingSize}");
        Debug.Log($"   圆环厚度: {impactMarker.ringThickness}");
        Debug.Log($"   标记生命周期: {impactMarker.markerLifetime}秒");

        // 2. 速度阈值检查
        Debug.Log("2. 速度阈值:");
        Debug.Log($"   低速阈值: {impactMarker.lowSpeedThreshold}m/s");
        Debug.Log($"   中速阈值: {impactMarker.mediumSpeedThreshold}m/s");
        Debug.Log($"   高速阈值: {impactMarker.highSpeedThreshold}m/s");

        // 3. 场景对象统计
        Debug.Log("3. 场景对象统计:");
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ballCount = 0, ringCount = 0, floorCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall")) ballCount++;
            else if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring")) ringCount++;
            else if (obj.name.Contains("Floor") || obj.name.Contains("Ground")) floorCount++;
        }

        Debug.Log($"   网球数量: {ballCount}");
        Debug.Log($"   圆环标记数量: {ringCount}");
        Debug.Log($"   地面对象数量: {floorCount}");

        // 4. 建议
        Debug.Log("4. 诊断建议:");
        if (!impactMarker.enableImpactMarkers)
        {
            Debug.LogWarning("   ⚠️ 系统未启用，按F3启用");
        }
        if (ringCount == 0)
        {
            Debug.LogWarning("   ⚠️ 无圆环标记，尝试发射网球或按F10创建测试圆环");
        }
        if (ballCount == 0)
        {
            Debug.LogWarning("   ⚠️ 无网球，按空格键创建测试网球");
        }
    }

    /// <summary>
    /// 强制创建可见圆环
    /// </summary>
    void ForceCreateVisibleRing()
    {
        Debug.Log("=== 强制创建可见圆环 ===");

        // 创建一个明显的圆柱体作为测试圆环
        GameObject testRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        testRing.name = "ImpactMarker_Ring_ForceTest";

        // 设置位置（摄像机前方）
        Vector3 ringPos = new Vector3(0, 0.05f, 2);
        testRing.transform.position = ringPos;

        // 设置为扁平的圆环形状
        testRing.transform.localScale = new Vector3(1.5f, 0.05f, 1.5f);

        // 创建明亮的材质
        Renderer renderer = testRing.GetComponent<Renderer>();
        Material ringMat = new Material(Shader.Find("Standard"));

        // 设置明亮的红色发光
        ringMat.color = Color.red;
        ringMat.EnableKeyword("_EMISSION");
        ringMat.SetColor("_EmissionColor", Color.red * 3f);
        ringMat.SetFloat("_Metallic", 0f);
        ringMat.SetFloat("_Smoothness", 0.8f);

        renderer.material = ringMat;

        Debug.Log($"✅ 强制测试圆环已创建:");
        Debug.Log($"   名称: {testRing.name}");
        Debug.Log($"   位置: {ringPos}");
        Debug.Log($"   大小: 1.5m直径");
        Debug.Log($"   颜色: 明亮红色发光");
        Debug.Log($"   应该在场景中清晰可见！");

        // 15秒后销毁
        Destroy(testRing, 15f);
    }

    /// <summary>
    /// 实时监控网球状态
    /// </summary>
    void MonitorTennisBalls()
    {
        // 每秒检查一次即可，避免过多日志
        if (Time.frameCount % 60 != 0) return;

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall") && obj.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                Vector3 pos = obj.transform.position;
                Vector3 vel = rb.velocity;

                // 检查异常位置 - 网球掉落到地面以下太深
                if (pos.y < -10f)
                {
                    Debug.LogWarning($"⚠️ 异常网球位置检测: {obj.name} 高度{pos.y:F2}m - 自动清理");
                    Destroy(obj);
                    continue;
                }

                // 检查异常速度 - 速度过快可能是物理系统错误
                if (vel.magnitude > 100f)
                {
                    Debug.LogWarning($"⚠️ 异常网球速度检测: {obj.name} 速度{vel.magnitude:F2}m/s - 自动清理");
                    Destroy(obj);
                    continue;
                }

                // 只在合理范围内且接近地面时输出详细信息
                if (pos.y <= 0.5f && pos.y >= -1f && vel.y < -1f)
                {
                    Debug.Log($"🔍 监控网球 {obj.name}: 高度{pos.y:F2}m, 速度{vel.magnitude:F2}m/s");
                }
            }
        }
    }

    void OnDestroy()
    {
        Debug.Log("SimpleImpactTest 诊断工具已关闭");
    }
}