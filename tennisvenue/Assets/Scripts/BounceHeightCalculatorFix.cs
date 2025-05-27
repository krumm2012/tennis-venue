using UnityEngine;
using System.Collections;

/// <summary>
/// 反弹高度计算修复器 - 解决反弹高度计算错误的问题
/// </summary>
public class BounceHeightCalculatorFix : MonoBehaviour
{
    [Header("反弹计算设置")]
    public bool enableRealTimeMonitoring = true;
    public float accurateHeightThreshold = 0.05f;  // 更精确的高度检测阈值
    public float velocityChangeThreshold = 0.2f;   // 速度变化检测阈值

    private bool isMonitoring = false;

    void Start()
    {
        Debug.Log("=== 反弹高度计算修复器已启动 ===");
        Debug.Log("按F10键运行修复版反弹测试");
        Debug.Log("按F11键运行标准反弹测试对比");
    }

    void Update()
    {
        // F10键 - 修复版反弹测试
        if (Input.GetKeyDown(KeyCode.F10))
        {
            RunFixedBounceTest();
        }

        // 实时监控所有网球的反弹
        if (enableRealTimeMonitoring && !isMonitoring)
        {
            StartCoroutine(MonitorAllTennisBalls());
        }
    }

    /// <summary>
    /// 运行修复版反弹测试
    /// </summary>
    void RunFixedBounceTest()
    {
        Debug.Log("=== 开始修复版反弹高度计算测试 ===");

        // 清理之前的测试球
        GameObject[] oldTestBalls = GameObject.FindGameObjectsWithTag("TestBall");
        foreach (GameObject ball in oldTestBalls)
        {
            Destroy(ball);
        }

        // 创建精确配置的测试球
        GameObject testBall = CreatePreciseTestBall();

        // 设置初始位置
        Vector3 startPos = new Vector3(1, 2.5f, 2);  // 2.5m高度，偏离中心避免干扰
        testBall.transform.position = startPos;

        // 开始精确监控
        StartCoroutine(AccurateBounceMonitoring(testBall, startPos.y));

        // 10秒后清理
        Destroy(testBall, 10f);
    }

    /// <summary>
    /// 创建精确配置的测试球
    /// </summary>
    GameObject CreatePreciseTestBall()
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "FixedBounceBall";
        ball.tag = "TestBall";
        ball.transform.localScale = Vector3.one * 0.067f;

        // 添加精确的Rigidbody设置
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 0.057f;           // 标准网球质量
        rb.drag = 0.015f;           // 极低空气阻力
        rb.angularDrag = 0.015f;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // 设置理想的物理材质
        Collider collider = ball.GetComponent<Collider>();
        PhysicMaterial ballMat = new PhysicMaterial("FixedBounceBall");
        ballMat.bounciness = 0.85f;
        ballMat.dynamicFriction = 0.3f;
        ballMat.staticFriction = 0.3f;
        ballMat.frictionCombine = PhysicMaterialCombine.Average;
        ballMat.bounceCombine = PhysicMaterialCombine.Maximum;  // 关键修复
        collider.material = ballMat;

        // 设置绿色材质便于识别
        Renderer renderer = ball.GetComponent<Renderer>();
        Material greenMat = new Material(Shader.Find("Standard"));
        greenMat.color = new Color(0.2f, 0.8f, 0.2f, 1f);  // 绿色
        greenMat.SetFloat("_Metallic", 0.3f);
        greenMat.SetFloat("_Smoothness", 0.8f);
        renderer.material = greenMat;

        Debug.Log("✅ 修复版测试球已创建 (绿色球)");
        return ball;
    }

    /// <summary>
    /// 精确的反弹监控
    /// </summary>
    System.Collections.IEnumerator AccurateBounceMonitoring(GameObject ball, float initialHeight)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        // 状态变量
        float maxBounceHeight = 0f;
        float previousHeight = initialHeight;
        float previousVelocityY = 0f;
        bool hasLanded = false;
        bool isRisingAfterBounce = false;
        bool bounceDetected = false;

        float startTime = Time.time;
        float landingTime = 0f;
        Vector3 landingPosition = Vector3.zero;

        Debug.Log($"🔍 开始精确监控 - 初始高度: {initialHeight:F3}m");

        while (ball != null && (Time.time - startTime) < 8f)
        {
            float currentHeight = ball.transform.position.y;
            float currentVelocityY = rb.velocity.y;
            float velocityChange = currentVelocityY - previousVelocityY;

            // 精确检测落地时刻
            if (!hasLanded && currentHeight <= (accurateHeightThreshold + 0.067f) && currentVelocityY <= 0)
            {
                hasLanded = true;
                landingTime = Time.time;
                landingPosition = ball.transform.position;
                Debug.Log($"⚡ 精确落地检测 - 高度: {currentHeight:F3}m, 速度: {currentVelocityY:F2}m/s");
                Debug.Log($"📍 落地位置: {landingPosition}");
            }

            // 精确检测反弹开始（速度从负转正）
            if (hasLanded && !isRisingAfterBounce && currentVelocityY > 0.05f && previousVelocityY <= 0)
            {
                isRisingAfterBounce = true;
                bounceDetected = true;
                float bounceVelocity = currentVelocityY;
                Debug.Log($"🚀 反弹开始 - 反弹速度: {bounceVelocity:F2}m/s");

                // 基于能量守恒预测理论最大高度
                float theoreticalMaxHeight = (bounceVelocity * bounceVelocity) / (2f * Mathf.Abs(Physics.gravity.y));
                Debug.Log($"📈 理论最大反弹高度: {theoreticalMaxHeight:F3}m");
            }

            // 记录反弹过程中的最大高度
            if (isRisingAfterBounce)
            {
                if (currentHeight > maxBounceHeight)
                {
                    maxBounceHeight = currentHeight;
                }
            }

            // 检测反弹达到最高点
            if (bounceDetected && isRisingAfterBounce && currentVelocityY <= 0 && maxBounceHeight > 0.1f)
            {
                // 计算修复后的准确结果
                float correctedBounceHeight = maxBounceHeight - accurateHeightThreshold; // 减去地面高度偏移
                float accurateBounceEfficiency = (correctedBounceHeight / initialHeight) * 100f;
                float actualBounceCoeff = correctedBounceHeight / initialHeight;

                Debug.Log($"🏀 修复版反弹计算结果:");
                Debug.Log($"  📏 初始高度: {initialHeight:F3}m");
                Debug.Log($"  📏 测量最大高度: {maxBounceHeight:F3}m");
                Debug.Log($"  📏 修正反弹高度: {correctedBounceHeight:F3}m");
                Debug.Log($"  📊 准确反弹效率: {accurateBounceEfficiency:F1}%");
                Debug.Log($"  📊 实际反弹系数: {actualBounceCoeff:F3}");
                Debug.Log($"  ⏱️ 反弹用时: {(Time.time - landingTime):F2}s");

                // 与理论值对比
                float expectedEfficiency = 0.75f * 0.85f; // 地面0.75 × 球0.85 (Maximum组合取较大值0.85)
                float theoreticalHeight = initialHeight * expectedEfficiency;
                float calculationError = Mathf.Abs(correctedBounceHeight - theoreticalHeight);

                Debug.Log($"  🎯 理论预期效率: {(expectedEfficiency*100):F1}%");
                Debug.Log($"  🎯 理论预期高度: {theoreticalHeight:F3}m");
                Debug.Log($"  ❌ 计算误差: {calculationError:F3}m ({(calculationError/initialHeight*100):F1}%)");

                // 误差分析
                if (calculationError < 0.1f)
                {
                    Debug.Log("✅ 反弹高度计算准确！误差在可接受范围内");
                }
                else if (calculationError < 0.3f)
                {
                    Debug.LogWarning("⚠️ 反弹高度计算存在一定误差，可能受空气阻力影响");
                }
                else
                {
                    Debug.LogError("❌ 反弹高度计算误差过大！建议检查物理设置");
                }

                break;
            }

            // 更新前一帧数据
            previousHeight = currentHeight;
            previousVelocityY = currentVelocityY;

            yield return new WaitForFixedUpdate();
        }

        // 监控超时处理
        if (!bounceDetected)
        {
            Debug.LogError("❌ 监控超时，未检测到有效反弹");
            Debug.LogError("可能原因: 球体穿过地面 或 反弹被完全抑制");
        }
    }

    /// <summary>
    /// 监控所有网球的反弹情况（实时）
    /// </summary>
    System.Collections.IEnumerator MonitorAllTennisBalls()
    {
        isMonitoring = true;

        while (enableRealTimeMonitoring)
        {
            // 查找所有包含TennisBall的对象
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("TennisBall") && obj.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = obj.GetComponent<Rigidbody>();
                    float height = obj.transform.position.y;
                    float velocityY = rb.velocity.y;

                    // 检测低速反弹（可能计算错误的情况）
                    if (height > 0.1f && height < 0.8f && velocityY > 0 && velocityY < 3f)
                    {
                        Debug.LogWarning($"⚠️ 检测到低反弹球: {obj.name}");
                        Debug.LogWarning($"   高度: {height:F2}m, 上升速度: {velocityY:F2}m/s");
                        Debug.LogWarning($"   建议检查反弹计算设置");
                    }
                }
            }

            yield return new WaitForSeconds(0.2f); // 每0.2秒检查一次
        }

        isMonitoring = false;
    }

    /// <summary>
    /// 检查和修复地面物理材质
    /// </summary>
    [ContextMenu("修复地面物理材质")]
    public void FixFloorPhysicsMaterial()
    {
        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Collider floorCollider = floor.GetComponent<Collider>();
            if (floorCollider != null)
            {
                PhysicMaterial floorMat = floorCollider.material;
                if (floorMat != null)
                {
                    if (floorMat.bounceCombine == PhysicMaterialCombine.Multiply)
                    {
                        floorMat.bounceCombine = PhysicMaterialCombine.Maximum;
                        Debug.Log("✅ 地面反弹组合已修复: Multiply → Maximum");
                    }
                }
            }
        }
    }
}