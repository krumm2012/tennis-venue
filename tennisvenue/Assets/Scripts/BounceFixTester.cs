using UnityEngine;

/// <summary>
/// 简化的反弹修复测试器 - 用于快速验证修复效果
/// </summary>
public class BounceFixTester : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Bounce Fix Tester Started ===");
        Debug.Log("Auto-executing bounce fix...");

        // 立即执行修复
        FixBounceIssues();

        Debug.Log("Press F10 to create test ball for verification");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            CreateAndTestBounceBall();
        }
    }

    /// <summary>
    /// 修复反弹问题
    /// </summary>
    void FixBounceIssues()
    {
        Debug.Log("--- Starting bounce issue fix ---");

        // 1. 修复地面材质
        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Collider floorCollider = floor.GetComponent<Collider>();
            if (floorCollider != null)
            {
                PhysicMaterial floorMat = new PhysicMaterial("FixedFloor");
                floorMat.bounciness = 0.75f;
                floorMat.dynamicFriction = 0.6f;
                floorMat.staticFriction = 0.6f;
                floorMat.frictionCombine = PhysicMaterialCombine.Average;
                floorMat.bounceCombine = PhysicMaterialCombine.Maximum;  // 关键修复
                floorCollider.material = floorMat;
                Debug.Log("✅ Floor material fixed - bounce combine: Maximum");
            }
        }

        // 2. 修复网球材质
        GameObject ball = GameObject.Find("TennisBall");
        if (ball != null)
        {
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            Collider ballCollider = ball.GetComponent<Collider>();

            if (ballRb != null)
            {
                ballRb.drag = 0.02f;  // 降低空气阻力
                ballRb.angularDrag = 0.02f;
                Debug.Log("✅ Tennis ball Rigidbody optimized");
            }

            if (ballCollider != null)
            {
                PhysicMaterial ballMat = new PhysicMaterial("FixedTennisBall");
                ballMat.bounciness = 0.85f;
                ballMat.dynamicFriction = 0.6f;
                ballMat.staticFriction = 0.6f;
                ballMat.frictionCombine = PhysicMaterialCombine.Average;
                ballMat.bounceCombine = PhysicMaterialCombine.Maximum;  // 关键修复
                ballCollider.material = ballMat;
                Debug.Log("✅ Tennis ball material fixed - bounce combine: Maximum");
            }
        }

        Debug.Log("Bounce fix completed!");
    }

    /// <summary>
    /// 创建并测试反弹球
    /// </summary>
    void CreateAndTestBounceBall()
    {
        Debug.Log("=== 创建测试球验证反弹效果 ===");

        // 创建测试球
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "BounceTestBall";
        testBall.transform.position = new Vector3(0, 2.5f, 1);  // 2.5m高度
        testBall.transform.localScale = Vector3.one * 0.067f;

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.015f;  // 极低阻力
        rb.angularDrag = 0.015f;

        // 设置理想材质
        Collider collider = testBall.GetComponent<Collider>();
        PhysicMaterial testMat = new PhysicMaterial("TestBounce");
        testMat.bounciness = 0.85f;
        testMat.dynamicFriction = 0.3f;
        testMat.staticFriction = 0.3f;
        testMat.frictionCombine = PhysicMaterialCombine.Average;
        testMat.bounceCombine = PhysicMaterialCombine.Maximum;
        collider.material = testMat;

        // 设置蓝色材质便于识别
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material blueMat = new Material(Shader.Find("Standard"));
        blueMat.color = Color.blue;
        renderer.material = blueMat;

        Debug.Log("✅ 蓝色测试球已创建");
        Debug.Log("预期: 从2.5m高度反弹到约2.1m（85%效率）");

        // 开始监控
        StartCoroutine(MonitorBounce(testBall, 2.5f));

        // 8秒后清理
        Destroy(testBall, 8f);
    }

    /// <summary>
    /// 监控反弹效果
    /// </summary>
    System.Collections.IEnumerator MonitorBounce(GameObject ball, float startHeight)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        float maxBounceHeight = 0f;
        bool hasLanded = false;
        bool hasStartedRising = false;
        bool testComplete = false;

        Debug.Log($"开始监控反弹 - 初始高度: {startHeight:F2}m");

        float startTime = Time.time;
        while (ball != null && !testComplete && (Time.time - startTime) < 6f)
        {
            float currentHeight = ball.transform.position.y;
            float velocityY = rb.velocity.y;

            // 检测落地
            if (!hasLanded && currentHeight <= 0.2f && velocityY <= 0)
            {
                hasLanded = true;
                Debug.Log($"⚡ 球已落地 - 高度: {currentHeight:F3}m");
            }

            // 检测开始上升（反弹）
            if (hasLanded && !hasStartedRising && velocityY > 0.1f)
            {
                hasStartedRising = true;
                Debug.Log($"🚀 开始反弹 - 初始上升速度: {velocityY:F2}m/s");
            }

            // 记录最大高度
            if (hasStartedRising && currentHeight > maxBounceHeight)
            {
                maxBounceHeight = currentHeight;
            }

            // 检测反弹完成
            if (hasStartedRising && velocityY <= 0 && maxBounceHeight > 0.3f)
            {
                testComplete = true;

                float bounceEfficiency = (maxBounceHeight / startHeight) * 100f;
                float expectedEfficiency = 85f;  // Maximum组合下的期望值
                float error = Mathf.Abs(bounceEfficiency - expectedEfficiency);

                Debug.Log($"🏀 反弹测试结果:");
                Debug.Log($"  📏 初始高度: {startHeight:F2}m");
                Debug.Log($"  📏 反弹高度: {maxBounceHeight:F2}m");
                Debug.Log($"  📊 反弹效率: {bounceEfficiency:F1}%");
                Debug.Log($"  🎯 预期效率: {expectedEfficiency:F1}%");
                Debug.Log($"  ❌ 误差: {error:F1}%");

                if (error < 10f)
                {
                    Debug.Log("✅ 反弹修复成功！误差在可接受范围内");
                }
                else if (error < 20f)
                {
                    Debug.LogWarning("⚠️ 反弹基本正常，但仍有改进空间");
                }
                else
                {
                    Debug.LogError("❌ 反弹效果仍不理想，需要进一步调试");
                }
            }

            yield return new WaitForFixedUpdate();
        }

        if (!testComplete)
        {
            Debug.LogWarning("⚠️ 反弹测试超时或未完成");
        }
    }

    /// <summary>
    /// 显示当前物理设置状态
    /// </summary>
    [ContextMenu("显示物理设置")]
    public void ShowPhysicsSettings()
    {
        Debug.Log("=== 当前物理设置状态 ===");

        GameObject floor = GameObject.Find("Floor");
        if (floor?.GetComponent<Collider>()?.material != null)
        {
            PhysicMaterial mat = floor.GetComponent<Collider>().material;
            Debug.Log($"地面: 反弹={mat.bounciness}, 组合={mat.bounceCombine}");
        }

        GameObject ball = GameObject.Find("TennisBall");
        if (ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log($"网球Rigidbody: drag={rb.drag}, mass={rb.mass}");
            }

            Collider col = ball.GetComponent<Collider>();
            if (col?.material != null)
            {
                PhysicMaterial mat = col.material;
                Debug.Log($"网球材质: 反弹={mat.bounciness}, 组合={mat.bounceCombine}");
            }
        }
    }
}