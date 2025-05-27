using UnityEngine;

/// <summary>
/// 反弹分析器 - 诊断反弹高度低的问题
/// </summary>
public class BounceAnalyzer : MonoBehaviour
{
    [Header("分析设置")]
    public bool enableDetailedAnalysis = true;
    public GameObject testBall;

    void Start()
    {
        Debug.Log("=== 反弹分析器已启动 ===");
        Debug.Log("按Ctrl+F11分析当前物理设置");
    }

    void Update()
    {
        // Ctrl + F11 进行详细分析
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F11))
        {
            AnalyzeBounceSettings();
        }

        // Ctrl + F12 创建理想反弹测试
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F12))
        {
            CreateIdealBounceTest();
        }
    }

    /// <summary>
    /// 分析当前反弹设置
    /// </summary>
    void AnalyzeBounceSettings()
    {
        Debug.Log("=== 反弹设置分析 ===");

        // 分析重力设置
        Vector3 gravity = Physics.gravity;
        Debug.Log($"当前重力: {gravity} (标准: 0, -9.81, 0)");
        if (gravity.y < -9.81f)
        {
            Debug.LogWarning("⚠️ 重力过强，可能影响反弹高度");
        }

        // 分析地面材质
        AnalyzeFloorMaterial();

        // 分析网球材质
        AnalyzeBallMaterial();

        // 分析空气阻力系统
        AnalyzeAirResistance();

        // 计算理论反弹效率
        CalculateTheoreticalBounce();
    }

    /// <summary>
    /// 分析地面材质
    /// </summary>
    void AnalyzeFloorMaterial()
    {
        Debug.Log("--- 地面材质分析 ---");

        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Collider floorCollider = floor.GetComponent<Collider>();
            if (floorCollider != null && floorCollider.material != null)
            {
                PhysicMaterial floorMat = floorCollider.material;
                Debug.Log($"地面材质: {floorMat.name}");
                Debug.Log($"  反弹系数: {floorMat.bounciness}");
                Debug.Log($"  反弹组合: {floorMat.bounceCombine}");
                Debug.Log($"  动摩擦: {floorMat.dynamicFriction}");
                Debug.Log($"  静摩擦: {floorMat.staticFriction}");

                if (floorMat.bounceCombine == PhysicMaterialCombine.Multiply)
                {
                    Debug.LogWarning("⚠️ 地面使用Multiply组合，可能大幅降低反弹");
                }
            }
            else
            {
                Debug.LogWarning("❌ 地面缺少物理材质");
            }
        }
        else
        {
            Debug.LogError("❌ 未找到地面对象");
        }
    }

    /// <summary>
    /// 分析网球材质
    /// </summary>
    void AnalyzeBallMaterial()
    {
        Debug.Log("--- 网球材质分析 ---");

        // 查找TennisBall预制体
        GameObject ball = GameObject.Find("TennisBall");
        if (ball == null)
        {
            // 查找场景中的网球实例
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("TennisBall"))
                {
                    ball = obj;
                    break;
                }
            }
        }

        if (ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            Collider ballCollider = ball.GetComponent<Collider>();

            if (rb != null)
            {
                Debug.Log($"网球Rigidbody设置:");
                Debug.Log($"  质量: {rb.mass}kg (标准: 0.057kg)");
                Debug.Log($"  线性阻力: {rb.drag} (建议: 0.02-0.05)");
                Debug.Log($"  角阻力: {rb.angularDrag}");

                if (rb.drag > 0.05f)
                {
                    Debug.LogWarning("⚠️ 线性阻力过高，严重影响反弹高度");
                }
            }

            if (ballCollider != null && ballCollider.material != null)
            {
                PhysicMaterial ballMat = ballCollider.material;
                Debug.Log($"网球材质: {ballMat.name}");
                Debug.Log($"  反弹系数: {ballMat.bounciness}");
                Debug.Log($"  反弹组合: {ballMat.bounceCombine}");

                if (ballMat.bounceCombine == PhysicMaterialCombine.Multiply)
                {
                    Debug.LogWarning("⚠️ 网球使用Multiply组合，可能大幅降低反弹");
                }
            }
        }
        else
        {
            Debug.LogWarning("❌ 未找到网球对象");
        }
    }

    /// <summary>
    /// 分析空气阻力系统
    /// </summary>
    void AnalyzeAirResistance()
    {
        Debug.Log("--- 空气阻力系统分析 ---");

        AirResistanceSystem airSystem = FindObjectOfType<AirResistanceSystem>();
        if (airSystem != null)
        {
            Debug.Log("✅ 发现空气阻力系统");
            Debug.LogWarning("⚠️ 空气阻力系统可能在反弹过程中持续影响球的速度");
        }
        else
        {
            Debug.Log("❌ 未发现空气阻力系统");
        }
    }

    /// <summary>
    /// 计算理论反弹效率
    /// </summary>
    void CalculateTheoreticalBounce()
    {
        Debug.Log("--- 理论反弹计算 ---");

        // 假设参数
        float floorBounce = 0.75f;
        float ballBounce = 0.85f;

        // 不同组合方式的效果
        float averageResult = (floorBounce + ballBounce) / 2f;  // Average组合
        float multiplyResult = floorBounce * ballBounce;        // Multiply组合
        float maximumResult = Mathf.Max(floorBounce, ballBounce); // Maximum组合
        float minimumResult = Mathf.Min(floorBounce, ballBounce); // Minimum组合

        Debug.Log($"不同组合方式的理论反弹系数:");
        Debug.Log($"  Average: {averageResult:F3} ({averageResult*100:F1}%反弹)");
        Debug.Log($"  Multiply: {multiplyResult:F3} ({multiplyResult*100:F1}%反弹) ⚠️ 可能过低");
        Debug.Log($"  Maximum: {maximumResult:F3} ({maximumResult*100:F1}%反弹) ✅ 推荐");
        Debug.Log($"  Minimum: {minimumResult:F3} ({minimumResult*100:F1}%反弹)");

        Debug.Log($"\n从2m高度理论反弹高度:");
        Debug.Log($"  Average组合: {2f * averageResult:F2}m");
        Debug.Log($"  Multiply组合: {2f * multiplyResult:F2}m");
        Debug.Log($"  Maximum组合: {2f * maximumResult:F2}m");
    }

    /// <summary>
    /// 创建理想反弹测试
    /// </summary>
    void CreateIdealBounceTest()
    {
        Debug.Log("=== 创建理想反弹测试 ===");

        // 创建测试球
        GameObject idealBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        idealBall.name = "IdealBounceBall";
        idealBall.transform.position = new Vector3(2, 2, 3);
        idealBall.transform.localScale = Vector3.one * 0.067f;

        // 设置理想的Rigidbody参数
        Rigidbody rb = idealBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.01f;        // 极低阻力
        rb.angularDrag = 0.01f;

        // 设置理想的物理材质
        Collider collider = idealBall.GetComponent<Collider>();
        PhysicMaterial idealMaterial = new PhysicMaterial("IdealBounce");
        idealMaterial.bounciness = 0.9f;  // 高反弹
        idealMaterial.dynamicFriction = 0.1f;  // 低摩擦
        idealMaterial.staticFriction = 0.1f;
        idealMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        idealMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
        collider.material = idealMaterial;

        // 设置红色材质便于识别
        Renderer renderer = idealBall.GetComponent<Renderer>();
        Material redMat = new Material(Shader.Find("Standard"));
        redMat.color = Color.red;
        renderer.material = redMat;

        Debug.Log("✅ 理想反弹测试球已创建（红色球）");
        Debug.Log("对比观察红色球与普通网球的反弹差异");

        // 5秒后销毁
        Destroy(idealBall, 8f);
    }
}