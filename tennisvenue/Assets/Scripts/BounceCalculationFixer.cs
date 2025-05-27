using UnityEngine;

/// <summary>
/// 反弹计算修复器 - 专门解决反弹高度计算错误的问题
/// </summary>
public class BounceCalculationFixer : MonoBehaviour
{
    [Header("问题诊断")]
    public bool autoFixOnStart = true;
    public bool enableDebugLogging = true;

    void Start()
    {
        if (autoFixOnStart)
        {
            FixBounceCalculationIssues();
        }

        Debug.Log("=== 反弹计算修复器已启动 ===");
        Debug.Log("发现的主要问题:");
        Debug.Log("1. BounceTestDemo中使用了Multiply组合模式");
        Debug.Log("2. 空气阻力系统可能过度影响反弹");
        Debug.Log("3. 时间条件逻辑错误");
        Debug.Log("按F10键运行完整修复测试");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            RunComprehensiveFixTest();
        }
    }

    /// <summary>
    /// 修复反弹计算问题
    /// </summary>
    public void FixBounceCalculationIssues()
    {
        Debug.Log("=== 开始修复反弹计算问题 ===");

        // 1. 修复地面物理材质
        FixFloorMaterial();

        // 2. 修复网球物理材质
        FixTennisBallMaterial();

        // 3. 优化空气阻力设置
        OptimizeAirResistanceForBounce();

        // 4. 检查重力设置
        CheckGravitySettings();

        Debug.Log("✅ 反弹计算问题修复完成");
    }

    /// <summary>
    /// 修复地面物理材质
    /// </summary>
    void FixFloorMaterial()
    {
        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Collider floorCollider = floor.GetComponent<Collider>();
            if (floorCollider != null)
            {
                if (floorCollider.material == null)
                {
                    // 创建新的地面材质
                    PhysicMaterial floorMat = new PhysicMaterial("FixedFloor");
                    floorMat.bounciness = 0.75f;
                    floorMat.dynamicFriction = 0.75f;
                    floorMat.staticFriction = 0.8f;
                    floorMat.frictionCombine = PhysicMaterialCombine.Average;
                    floorMat.bounceCombine = PhysicMaterialCombine.Maximum;  // 关键修复
                    floorCollider.material = floorMat;
                    Debug.Log("✅ 地面材质已修复 - 反弹组合: Maximum");
                }
                else
                {
                    PhysicMaterial existingMat = floorCollider.material;
                    if (existingMat.bounceCombine != PhysicMaterialCombine.Maximum)
                    {
                        existingMat.bounceCombine = PhysicMaterialCombine.Maximum;
                        Debug.Log("✅ 地面反弹组合已修复: → Maximum");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到地面对象");
        }
    }

    /// <summary>
    /// 修复网球物理材质
    /// </summary>
    void FixTennisBallMaterial()
    {
        // 查找网球预制体
        GameObject tennisBall = GameObject.Find("TennisBall");
        if (tennisBall != null)
        {
            Collider ballCollider = tennisBall.GetComponent<Collider>();
            Rigidbody ballRb = tennisBall.GetComponent<Rigidbody>();

            if (ballCollider != null)
            {
                PhysicMaterial ballMat = new PhysicMaterial("FixedTennisBall");
                ballMat.bounciness = 0.85f;
                ballMat.dynamicFriction = 0.6f;
                ballMat.staticFriction = 0.6f;
                ballMat.frictionCombine = PhysicMaterialCombine.Average;
                ballMat.bounceCombine = PhysicMaterialCombine.Maximum;  // 关键修复
                ballCollider.material = ballMat;
                Debug.Log("✅ 网球材质已修复 - 反弹组合: Maximum");
            }

            if (ballRb != null)
            {
                // 优化网球物理参数
                ballRb.mass = 0.057f;
                ballRb.drag = 0.02f;  // 减少空气阻力对反弹的影响
                ballRb.angularDrag = 0.02f;
                Debug.Log("✅ 网球Rigidbody参数已优化");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到网球预制体");
        }
    }

    /// <summary>
    /// 优化空气阻力设置以减少对反弹的影响
    /// </summary>
    void OptimizeAirResistanceForBounce()
    {
        AirResistanceSystem airSystem = FindObjectOfType<AirResistanceSystem>();
        if (airSystem != null)
        {
            // 降低线性阻力以减少对反弹的影响
            airSystem.optimizedLinearDrag = 0.02f;  // 从0.12f降至0.02f
            airSystem.optimizedAngularDrag = 0.02f;
            Debug.Log("✅ 空气阻力系统已优化 - 减少对反弹的影响");
        }
    }

    /// <summary>
    /// 检查重力设置
    /// </summary>
    void CheckGravitySettings()
    {
        Vector3 gravity = Physics.gravity;
        if (gravity.y < -9.81f)
        {
            Debug.LogWarning($"⚠️ 重力设置过强: {gravity.y:F2}m/s² (标准: -9.81m/s²)");
            Debug.LogWarning("建议在Project Settings > Physics中调整重力设置");
        }
        else
        {
            Debug.Log($"✅ 重力设置正常: {gravity.y:F2}m/s²");
        }
    }

    /// <summary>
    /// 运行综合修复测试
    /// </summary>
    void RunComprehensiveFixTest()
    {
        Debug.Log("=== 开始综合修复测试 ===");

        // 强制应用所有修复
        FixBounceCalculationIssues();

        // 创建理想测试球
        GameObject testBall = CreateIdealTestBall();

        // 开始测试
        StartCoroutine(RunPreciseBounceTest(testBall));
    }

    /// <summary>
    /// 创建理想测试球
    /// </summary>
    GameObject CreateIdealTestBall()
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "IdealTestBall";
        ball.transform.position = new Vector3(0, 2.5f, 1);  // 2.5m高度
        ball.transform.localScale = Vector3.one * 0.067f;

        // 理想的物理设置
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.01f;  // 极低阻力
        rb.angularDrag = 0.01f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // 理想的物理材质
        Collider collider = ball.GetComponent<Collider>();
        PhysicMaterial idealMat = new PhysicMaterial("IdealBounce");
        idealMat.bounciness = 0.85f;
        idealMat.dynamicFriction = 0.3f;
        idealMat.staticFriction = 0.3f;
        idealMat.frictionCombine = PhysicMaterialCombine.Average;
        idealMat.bounceCombine = PhysicMaterialCombine.Maximum;
        collider.material = idealMat;

        // 蓝色材质便于识别
        Renderer renderer = ball.GetComponent<Renderer>();
        Material blueMat = new Material(Shader.Find("Standard"));
        blueMat.color = new Color(0.3f, 0.3f, 1f, 1f);
        renderer.material = blueMat;

        Debug.Log("✅ 理想测试球已创建 (蓝色球)");
        return ball;
    }

    /// <summary>
    /// 运行精确反弹测试
    /// </summary>
    System.Collections.IEnumerator RunPreciseBounceTest(GameObject ball)
    {
        float startHeight = 2.5f;
        float maxBounceHeight = 0f;
        bool bounceDetected = false;
        bool isMonitoring = false;

        Rigidbody rb = ball.GetComponent<Rigidbody>();

        Debug.Log($"🔍 开始精确反弹测试 - 预期从{startHeight}m反弹到~{startHeight * 0.85f:F2}m");

        while (ball != null && !bounceDetected)
        {
            float currentHeight = ball.transform.position.y;
            float velocityY = rb.velocity.y;

            // 检测落地
            if (!isMonitoring && currentHeight <= 0.15f && velocityY <= 0)
            {
                isMonitoring = true;
                Debug.Log("⚡ 球已落地，开始监控反弹");
            }

            // 检测反弹上升
            if (isMonitoring && velocityY > 0.1f)
            {
                maxBounceHeight = Mathf.Max(maxBounceHeight, currentHeight);
            }

            // 检测反弹完成
            if (isMonitoring && velocityY <= 0 && maxBounceHeight > 0.2f)
            {
                bounceDetected = true;

                float bounceEfficiency = (maxBounceHeight / startHeight) * 100f;
                float theoreticalEfficiency = 85f;  // 理论效率（Maximum组合下的最佳值）
                float error = Mathf.Abs(bounceEfficiency - theoreticalEfficiency);

                Debug.Log($"🏀 修复后反弹测试结果:");
                Debug.Log($"  📏 初始高度: {startHeight:F2}m");
                Debug.Log($"  📏 反弹高度: {maxBounceHeight:F2}m");
                Debug.Log($"  📊 实际效率: {bounceEfficiency:F1}%");
                Debug.Log($"  📊 理论效率: {theoreticalEfficiency:F1}%");
                Debug.Log($"  ❌ 计算误差: {error:F1}%");

                if (error < 5f)
                {
                    Debug.Log("✅ 反弹计算准确！修复成功");
                }
                else if (error < 15f)
                {
                    Debug.LogWarning("⚠️ 反弹计算基本准确，但仍有优化空间");
                }
                else
                {
                    Debug.LogError("❌ 反弹计算仍有较大误差，需要进一步调试");
                }
            }

            yield return new WaitForFixedUpdate();
        }

        // 5秒后清理测试球
        if (ball != null)
        {
            Destroy(ball, 5f);
        }
    }

    /// <summary>
    /// 获取当前反弹效率问题诊断
    /// </summary>
    [ContextMenu("诊断反弹效率问题")]
    public void DiagnoseBounceEfficiencyIssues()
    {
        Debug.Log("=== 反弹效率问题诊断 ===");

        // 检查地面设置
        GameObject floor = GameObject.Find("Floor");
        if (floor?.GetComponent<Collider>()?.material != null)
        {
            PhysicMaterial floorMat = floor.GetComponent<Collider>().material;
            Debug.Log($"地面反弹系数: {floorMat.bounciness}");
            Debug.Log($"地面反弹组合: {floorMat.bounceCombine}");

            if (floorMat.bounceCombine == PhysicMaterialCombine.Multiply)
            {
                Debug.LogError("❌ 发现问题: 地面使用Multiply组合，会严重降低反弹");
            }
        }

        // 检查网球设置
        GameObject ball = GameObject.Find("TennisBall");
        if (ball?.GetComponent<Collider>()?.material != null)
        {
            PhysicMaterial ballMat = ball.GetComponent<Collider>().material;
            Debug.Log($"网球反弹系数: {ballMat.bounciness}");
            Debug.Log($"网球反弹组合: {ballMat.bounceCombine}");

            if (ballMat.bounceCombine == PhysicMaterialCombine.Multiply)
            {
                Debug.LogError("❌ 发现问题: 网球使用Multiply组合，会严重降低反弹");
            }
        }

        // 检查空气阻力影响
        AirResistanceSystem airSystem = FindObjectOfType<AirResistanceSystem>();
        if (airSystem != null)
        {
            if (airSystem.optimizedLinearDrag > 0.05f)
            {
                Debug.LogWarning($"⚠️ 空气阻力过高: {airSystem.optimizedLinearDrag}，可能影响反弹");
            }
        }

        Debug.Log("诊断完成。请按F10运行修复测试");
    }
}