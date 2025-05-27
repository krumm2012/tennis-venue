using UnityEngine;

public class LauncherFixTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 发球机修复测试启动 ===");
        Debug.Log("快捷键:");
        Debug.Log("  F1: 测试发球机发射");
        Debug.Log("  F2: 检查圆环标识系统");
        Debug.Log("  F3: 创建测试球");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestLauncher();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            CheckRingSystem();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            CreateTestBall();
        }
    }

    void TestLauncher()
    {
        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        if (launcher != null)
        {
            Debug.Log("🚀 测试发球机发射...");
            launcher.LaunchBall(Vector3.zero);
        }
        else
        {
            Debug.LogError("❌ 未找到BallLauncher组件");
        }
    }

    void CheckRingSystem()
    {
        BounceImpactMarker marker = FindObjectOfType<BounceImpactMarker>();
        if (marker != null)
        {
            Debug.Log("✅ BounceImpactMarker系统存在");
            Debug.Log($"   系统启用: {marker.enableImpactMarkers}");
            Debug.Log($"   活动标记数: {marker.GetActiveMarkerCount()}");
        }
        else
        {
            Debug.LogError("❌ 未找到BounceImpactMarker组件");
        }
    }

    void CreateTestBall()
    {
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "TennisBall_Test";
        testBall.transform.position = new Vector3(0, 2f, 0);
        testBall.transform.localScale = Vector3.one * 0.067f;

        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.02f;
        rb.useGravity = true;
        rb.velocity = new Vector3(0, -5f, 0);

        Renderer renderer = testBall.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.yellow;
        renderer.material = mat;

        Debug.Log("🎾 黄色测试球已创建");
        Destroy(testBall, 5f);
    }
}