using UnityEngine;

/// <summary>
/// 手动反弹测试 - 立即创建测试球验证修复效果
/// </summary>
public class ManualBounceTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 手动反弹测试开始 ===");

        // 延迟1秒后创建测试球，确保修复已完成
        Invoke("CreateTestBall", 1f);
    }

    void CreateTestBall()
    {
        Debug.Log("=== 创建测试球验证反弹效果 ===");

        // 创建测试球
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "BounceTestBall";
        testBall.transform.position = new Vector3(0, 3.0f, 1);  // 3m高度
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

        Debug.Log("✅ 蓝色测试球已创建 - 高度: 3.0m");
        Debug.Log("预期: 从3.0m高度反弹到约2.55m（85%效率）");

        // 开始监控
        StartCoroutine(MonitorBounce(testBall, 3.0f));

        // 10秒后清理
        Destroy(testBall, 10f);
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
        while (ball != null && !testComplete && (Time.time - startTime) < 8f)
        {
            float currentHeight = ball.transform.position.y;
            float velocityY = rb.velocity.y;

            // 检测落地
            if (!hasLanded && currentHeight <= 0.2f && velocityY <= 0)
            {
                hasLanded = true;
                Debug.Log($"⚡ 球已落地 - 高度: {currentHeight:F3}m, 速度: {velocityY:F2}m/s");
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
            if (maxBounceHeight > 0)
            {
                float bounceEfficiency = (maxBounceHeight / startHeight) * 100f;
                Debug.Log($"⚠️ 部分结果 - 最大反弹高度: {maxBounceHeight:F2}m, 效率: {bounceEfficiency:F1}%");
            }
        }
    }
}