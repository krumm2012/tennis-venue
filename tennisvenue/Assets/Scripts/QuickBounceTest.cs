using UnityEngine;

/// <summary>
/// 快速反弹测试 - 验证修复效果
/// </summary>
public class QuickBounceTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Quick Bounce Test Started ===");

        // 立即创建测试球
        CreateTestBall();
    }

    void CreateTestBall()
    {
        // 创建测试球
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "QuickTestBall";
        testBall.transform.position = new Vector3(0.5f, 2.0f, 0);  // 2m height, slightly offset to avoid overlapping with other balls
        testBall.transform.localScale = Vector3.one * 0.067f;

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.02f;  // Using repaired low resistance
        rb.angularDrag = 0.02f;

        // 设置修复后的材质
        Collider collider = testBall.GetComponent<Collider>();
        PhysicMaterial testMat = new PhysicMaterial("QuickTestMat");
        testMat.bounciness = 0.85f;
        testMat.dynamicFriction = 0.6f;
        testMat.staticFriction = 0.6f;
        testMat.frictionCombine = PhysicMaterialCombine.Average;
        testMat.bounceCombine = PhysicMaterialCombine.Maximum;  // Key repair
        collider.material = testMat;

        // 设置红色材质便于识别
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material redMat = new Material(Shader.Find("Standard"));
        redMat.color = Color.red;
        renderer.material = redMat;

        Debug.Log("✅ Red test ball created - height: 2.0m");
        Debug.Log("Expected bounce height: about 1.7m (85% efficiency)");

        // 开始监控
        StartCoroutine(MonitorBounce(testBall));

        // 8秒后清理
        Destroy(testBall, 8f);
    }

    System.Collections.IEnumerator MonitorBounce(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        float startHeight = 2.0f;
        float maxBounceHeight = 0f;
        bool hasLanded = false;
        bool hasStartedRising = false;

        Debug.Log("Starting bounce monitoring...");

        float startTime = Time.time;
        while (ball != null && (Time.time - startTime) < 6f)
        {
            float currentHeight = ball.transform.position.y;
            float velocityY = rb.velocity.y;

            // 检测落地
            if (!hasLanded && currentHeight <= 0.15f && velocityY <= 0)
            {
                hasLanded = true;
                Debug.Log($"⚡ Ball landed - height: {currentHeight:F3}m");
            }

            // 检测开始上升（反弹）
            if (hasLanded && !hasStartedRising && velocityY > 0.05f)
            {
                hasStartedRising = true;
                Debug.Log($"🚀 Starting bounce - rising speed: {velocityY:F2}m/s");
            }

            // 记录最大高度
            if (hasStartedRising && currentHeight > maxBounceHeight)
            {
                maxBounceHeight = currentHeight;
            }

            // 检测反弹完成
            if (hasStartedRising && velocityY <= 0 && maxBounceHeight > 0.2f)
            {
                float bounceEfficiency = (maxBounceHeight / startHeight) * 100f;

                Debug.Log($"🏀 Bounce test completed:");
                Debug.Log($"  📏 Initial height: {startHeight:F2}m");
                Debug.Log($"  📏 Bounce height: {maxBounceHeight:F2}m");
                Debug.Log($"  📊 Bounce efficiency: {bounceEfficiency:F1}%");

                if (bounceEfficiency > 75f)
                {
                    Debug.Log("✅ Bounce repair successful! Good efficiency");
                }
                else if (bounceEfficiency > 50f)
                {
                    Debug.LogWarning("⚠️ Bounce improved, but still needs optimization");
                }
                else
                {
                    Debug.LogError("❌ Bounce effect not ideal");
                }
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}