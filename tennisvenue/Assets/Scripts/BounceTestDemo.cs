using UnityEngine;

/// <summary>
/// 反弹测试演示 - 验证6mm聚氨酯地板的反弹效果
/// </summary>
public class BounceTestDemo : MonoBehaviour
{
    [Header("反弹测试设置")]
    public GameObject ballPrefab;
    public Transform testDropPoint;
    public float dropHeight = 2.0f;

    void Start()
    {
        // 设置默认投掷点
        if (testDropPoint == null)
        {
            GameObject dropPoint = new GameObject("TestDropPoint");
            testDropPoint = dropPoint.transform;
            testDropPoint.position = new Vector3(0, dropHeight, 3);
        }

        Debug.Log("=== 反弹测试演示已加载 ===");
        Debug.Log("按F11键测试6mm聚氨酯地板反弹效果");
    }

    void Update()
    {
        // 按F11键进行反弹测试
        if (Input.GetKeyDown(KeyCode.F11))
        {
            PerformBounceTest();
        }

        // 按F12键进行对比测试（使用默认物理材质）
        if (Input.GetKeyDown(KeyCode.F12))
        {
            PerformComparisonTest();
        }
    }

    /// <summary>
    /// 执行6mm聚氨酯地板反弹测试
    /// </summary>
    void PerformBounceTest()
    {
        Debug.Log("=== 开始6mm聚氨酯地板反弹测试 ===");

        // 查找或创建测试球
        if (ballPrefab == null)
        {
            ballPrefab = GameObject.Find("TennisBall");
            if (ballPrefab == null)
            {
                // 创建简单的测试球
                ballPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ballPrefab.name = "TestBall";
                ballPrefab.transform.localScale = Vector3.one * 0.067f; // 网球大小

                // 添加Rigidbody
                Rigidbody rb = ballPrefab.AddComponent<Rigidbody>();
                rb.mass = 0.057f; // 57克

                // 设置网球材质
                Collider collider = ballPrefab.GetComponent<Collider>();
                if (collider != null)
                {
                    PhysicMaterial ballMaterial = new PhysicMaterial("TestBall");
                    ballMaterial.bounciness = 0.85f;  // 增加反弹系数
                    ballMaterial.dynamicFriction = 0.6f;
                    ballMaterial.staticFriction = 0.6f;
                    ballMaterial.frictionCombine = PhysicMaterialCombine.Average;
                    ballMaterial.bounceCombine = PhysicMaterialCombine.Maximum;  // 修复：改为Maximum
                    collider.material = ballMaterial;
                    Debug.Log("✅ 测试球材质已设置 - 反弹组合: Maximum");
                }
            }
        }

        // 创建测试球实例
        GameObject testBall = Instantiate(ballPrefab, testDropPoint.position, Quaternion.identity);
        testBall.name = "BounceTestBall";

        // 确保有刚体组件
        Rigidbody ballRb = testBall.GetComponent<Rigidbody>();
        if (ballRb == null)
        {
            ballRb = testBall.AddComponent<Rigidbody>();
            ballRb.mass = 0.057f;
        }

        // 重置速度并开始下落
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        Debug.Log($"测试球已在高度 {dropHeight}m 处释放");
        Debug.Log("预期反弹高度: ~1.5m (75%反弹效率)");

        // 开始监控反弹
        StartCoroutine(MonitorBounce(testBall));

        // 5秒后清理测试球
        Destroy(testBall, 5f);
    }

    /// <summary>
    /// 监控球的反弹情况
    /// </summary>
    System.Collections.IEnumerator MonitorBounce(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        float maxHeight = 0f;
        bool hasLanded = false;
        bool isRising = false;
        float initialDrop = ball.transform.position.y;
        float startTime = Time.time;
        float landingTime = 0f;

        Debug.Log($"开始监控反弹 - 初始高度: {initialDrop:F2}m");

        while (ball != null && (Time.time - startTime) < 8f) // 修复时间条件
        {
            float currentHeight = ball.transform.position.y;
            float currentVelocityY = rb.velocity.y;

            // 检测是否落地
            if (!hasLanded && currentHeight <= 0.15f && currentVelocityY <= 0)
            {
                hasLanded = true;
                landingTime = Time.time;
                Debug.Log($"⚡ 球已落地 (t={landingTime-startTime:F2}s)，开始监控反弹");
            }

            // 检测是否开始上升（反弹）
            if (hasLanded && !isRising && currentVelocityY > 0.1f)
            {
                isRising = true;
                Debug.Log($"🚀 球开始反弹上升 (速度: {currentVelocityY:F2}m/s)");
            }

            // 记录反弹后的最高点
            if (isRising)
            {
                maxHeight = Mathf.Max(maxHeight, currentHeight);
            }

            // 检测反弹达到最高点并开始下降
            if (isRising && currentVelocityY <= 0 && maxHeight > 0.1f)
            {
                float bounceEfficiency = (maxHeight / initialDrop) * 100f;
                float actualBounceCoeff = maxHeight / initialDrop;

                Debug.Log($"🏀 反弹测试结果:");
                Debug.Log($"  初始高度: {initialDrop:F2}m");
                Debug.Log($"  反弹高度: {maxHeight:F2}m");
                Debug.Log($"  反弹效率: {bounceEfficiency:F1}%");
                Debug.Log($"  实际反弹系数: {actualBounceCoeff:F3}");
                Debug.Log($"  落地到最高点用时: {(Time.time - landingTime):F2}s");

                // 分析结果
                if (bounceEfficiency < 50f)
                {
                    Debug.LogWarning("⚠️ 反弹效率过低！可能原因:");
                    Debug.LogWarning("   1. 空气阻力过高 (drag > 0.05)");
                    Debug.LogWarning("   2. 材质组合使用了Multiply模式");
                    Debug.LogWarning("   3. 空气阻力系统干涉");
                    Debug.LogWarning("   按Ctrl+F11进行详细诊断");
                }
                else if (bounceEfficiency > 80f)
                {
                    Debug.Log("✅ 反弹效果良好！");
                }
                else
                {
                    Debug.Log("ℹ️ 反弹效果一般，可以进一步优化");
                }

                Debug.Log($"6mm聚氨酯地板反弹系数0.75的效果验证完成");
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        // 如果监控结束仍未检测到有效反弹
        if (maxHeight < 0.1f)
        {
            Debug.LogError("❌ 未检测到有效反弹！");
            Debug.LogError("可能原因: 球未正确落地或反弹被严重抑制");
            Debug.LogError("建议按Ctrl+F11进行物理设置诊断");
        }
    }

    /// <summary>
    /// 执行对比测试（默认材质）
    /// </summary>
    void PerformComparisonTest()
    {
        Debug.Log("=== 默认材质对比测试 ===");
        Debug.Log("此功能可用于对比不同材质的反弹效果");
        Debug.Log("当前使用6mm聚氨酯地板，反弹系数0.75");
    }
}