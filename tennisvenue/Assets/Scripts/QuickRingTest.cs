using UnityEngine;

/// <summary>
/// 快速圆环标记测试工具
/// 简单直接的测试第一落地点圆环标识功能
/// </summary>
public class QuickRingTest : MonoBehaviour
{
    [Header("快速测试设置")]
    [Tooltip("测试网球掉落高度")]
    public float dropHeight = 2f;

    [Tooltip("测试网球初始速度")]
    public float initialSpeed = 5f;

    private BounceImpactMarker impactMarker;

    void Start()
    {
        Debug.Log("=== 快速圆环标记测试工具 ===");
        Debug.Log("按键说明:");
        Debug.Log("  1键: 创建单个测试网球（场地中央）");
        Debug.Log("  2键: 创建4个测试网球（四个角落）");
        Debug.Log("  3键: 直接创建测试圆环（无需网球）");
        Debug.Log("  4键: 检查系统状态");
        Debug.Log("  5键: 清除所有测试对象");

        // 查找冲击标记系统
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (impactMarker == null)
        {
            Debug.LogError("❌ 未找到BounceImpactMarker系统！");
        }
        else
        {
            Debug.Log("✅ BounceImpactMarker系统已找到并准备就绪");
        }
    }

    void Update()
    {
        // 数字键快速测试
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreateSingleTestBall();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreateFourCornerBalls();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CreateDirectTestRings();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CheckSystemStatus();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ClearAllTestObjects();
        }
    }

    /// <summary>
    /// 创建单个测试网球
    /// </summary>
    void CreateSingleTestBall()
    {
        Debug.Log("🎾 创建单个测试网球（场地中央）");

        Vector3 position = new Vector3(0, dropHeight, 0);
        CreateTestBall(position, "Center", Color.yellow);

        Debug.Log($"✅ 测试网球已创建在 {position}，等待落地产生圆环标记");
    }

    /// <summary>
    /// 创建四个角落的测试网球
    /// </summary>
    void CreateFourCornerBalls()
    {
        Debug.Log("🎾 创建四个角落测试网球");

        Vector3[] positions = {
            new Vector3(-1.5f, dropHeight, 1.5f),   // 左前
            new Vector3(1.5f, dropHeight, 1.5f),    // 右前
            new Vector3(-1.5f, dropHeight, -1.5f),  // 左后
            new Vector3(1.5f, dropHeight, -1.5f)    // 右后
        };

        string[] names = { "LeftFront", "RightFront", "LeftBack", "RightBack" };
        Color[] colors = { Color.red, Color.green, Color.blue, Color.magenta };

        for (int i = 0; i < positions.Length; i++)
        {
            CreateTestBall(positions[i], names[i], colors[i]);
            Debug.Log($"✅ 创建{names[i]}测试网球在 {positions[i]}");
        }

        Debug.Log("🎯 四个测试网球已创建，观察落地圆环标记");
    }

    /// <summary>
    /// 创建测试网球
    /// </summary>
    void CreateTestBall(Vector3 position, string name, Color color)
    {
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = $"TennisBall_QuickTest_{name}";
        testBall.transform.position = position;
        testBall.transform.localScale = Vector3.one * 0.065f; // 标准网球大小

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f; // 标准网球质量
        rb.drag = 0.02f;
        rb.useGravity = true;

        // 设置初始速度（稍微向下和随机水平方向）
        Vector3 velocity = new Vector3(
            Random.Range(-1f, 1f),
            -initialSpeed,
            Random.Range(-1f, 1f)
        );
        rb.velocity = velocity;

        // 设置材质颜色
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        renderer.material = mat;

        // 8秒后自动销毁
        Destroy(testBall, 8f);

        Debug.Log($"🎾 创建测试网球: {name}");
        Debug.Log($"   位置: {position}");
        Debug.Log($"   速度: {velocity}");
    }

    /// <summary>
    /// 直接创建测试圆环（绕过网球检测）
    /// </summary>
    void CreateDirectTestRings()
    {
        Debug.Log("⭕ 直接创建测试圆环标记");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker系统未找到");
            return;
        }

        // 测试位置和速度
        Vector3[] positions = {
            new Vector3(0, 0.01f, 1),      // 前方
            new Vector3(-1, 0.01f, 0),     // 左侧
            new Vector3(1, 0.01f, 0),      // 右侧
            new Vector3(0, 0.01f, -1)      // 后方
        };

        float[] speeds = { 4f, 8f, 12f, 16f }; // 不同速度测试

        for (int i = 0; i < positions.Length; i++)
        {
            // 使用反射调用CreateImpactMarker方法
            var method = impactMarker.GetType().GetMethod("CreateImpactMarker",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                Vector3 velocity = Vector3.down * speeds[i];
                method.Invoke(impactMarker, new object[] { positions[i], speeds[i], velocity });

                Debug.Log($"✅ 创建直接测试圆环 {i + 1}:");
                Debug.Log($"   位置: {positions[i]}");
                Debug.Log($"   速度: {speeds[i]}m/s");
            }
            else
            {
                Debug.LogError("❌ 无法找到CreateImpactMarker方法");
                break;
            }
        }

        Debug.Log("⭕ 直接测试圆环已创建，请检查场景中的圆环标记");
    }

    /// <summary>
    /// 检查系统状态
    /// </summary>
    void CheckSystemStatus()
    {
        Debug.Log("=== 系统状态检查 ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker系统未找到");
            return;
        }

        Debug.Log("✅ BounceImpactMarker系统状态:");
        Debug.Log($"   启用状态: {impactMarker.enableImpactMarkers}");
        Debug.Log($"   基础圆环大小: {impactMarker.baseRingSize}");
        Debug.Log($"   标记生命周期: {impactMarker.markerLifetime}秒");
        Debug.Log($"   当前活动标记数: {impactMarker.GetActiveMarkerCount()}");

        // 检查场景中的标记对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int markerCount = 0;
        int ballCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring"))
            {
                markerCount++;
                Debug.Log($"   找到圆环标记: {obj.name} 位置: {obj.transform.position}");
            }
            else if (obj.name.Contains("TennisBall_QuickTest"))
            {
                ballCount++;
                Vector3 pos = obj.transform.position;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                Vector3 vel = rb != null ? rb.velocity : Vector3.zero;
                Debug.Log($"   找到测试网球: {obj.name} 位置: {pos} 速度: {vel.magnitude:F2}m/s");
            }
        }

        Debug.Log($"📊 场景统计: {markerCount}个圆环标记, {ballCount}个测试网球");
    }

    /// <summary>
    /// 清除所有测试对象
    /// </summary>
    void ClearAllTestObjects()
    {
        Debug.Log("🧹 清除所有测试对象");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int removedBalls = 0;
        int removedMarkers = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall_QuickTest"))
            {
                Destroy(obj);
                removedBalls++;
            }
            else if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring"))
            {
                Destroy(obj);
                removedMarkers++;
            }
        }

        Debug.Log($"🧹 已清除: {removedBalls}个测试网球, {removedMarkers}个圆环标记");

        // 清除BounceImpactMarker系统中的标记记录
        if (impactMarker != null)
        {
            var method = impactMarker.GetType().GetMethod("ClearAllMarkers",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(impactMarker, null);
                Debug.Log("🧹 已清除系统内部标记记录");
            }
        }

        Debug.Log("✅ 清除完成");
    }
}