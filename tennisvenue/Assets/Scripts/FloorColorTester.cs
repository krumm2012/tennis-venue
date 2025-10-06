using UnityEngine;

/// <summary>
/// 地面颜色测试器 - 验证地面颜色修复效果
/// </summary>
public class FloorColorTester : MonoBehaviour
{
    [Header("测试设置")]
    [Tooltip("按此键测试地面颜色")]
    public KeyCode testFloorKey = KeyCode.F12;

    void Start()
    {
        Debug.Log("=== 地面颜色测试器已加载 ===");
        ShowTestInstructions();
    }

    void Update()
    {
        if (Input.GetKeyDown(testFloorKey))
        {
            TestFloorColor();
        }
    }

    /// <summary>
    /// 测试地面颜色
    /// </summary>
    [ContextMenu("测试地面颜色")]
    public void TestFloorColor()
    {
        Debug.Log("🧪 开始测试地面颜色...");

        GameObject floor = GameObject.Find("Floor");
        if (floor == null)
        {
            Debug.LogError("❌ 未找到地面对象");
            return;
        }

        Renderer floorRenderer = floor.GetComponent<Renderer>();
        if (floorRenderer == null)
        {
            Debug.LogError("❌ 地面缺少Renderer组件");
            return;
        }

        Material material = floorRenderer.material;
        Color currentColor = material.color;

        Debug.Log("=== 地面颜色测试结果 ===");
        Debug.Log($"🎨 当前颜色: R={currentColor.r:F2}, G={currentColor.g:F2}, B={currentColor.b:F2}");
        Debug.Log($"🎨 颜色十六进制: #{ColorUtility.ToHtmlStringRGB(currentColor)}");

        // 判断颜色是否正确
        bool isCorrectBlue = IsCorrectFloorColor(currentColor);
        Debug.Log($"✅ 颜色正确: {(isCorrectBlue ? "✅ 是" : "❌ 否")}");

        if (isCorrectBlue)
        {
            Debug.Log("🎾 地面颜色为浅蓝色，符合要求！");
        }
        else
        {
            Debug.Log("⚠️ 地面颜色需要修复，建议按F11键修复");
        }

        // 显示材质信息
        Debug.Log($"📋 材质名称: {material.name}");
        Debug.Log($"📋 着色器: {material.shader.name}");
        Debug.Log($"📋 金属度: {material.GetFloat("_Metallic"):F2}");
        Debug.Log($"📋 光滑度: {material.GetFloat("_Glossiness"):F2}");
    }

    /// <summary>
    /// 判断是否为正确的地面颜色（浅蓝色）
    /// </summary>
    bool IsCorrectFloorColor(Color color)
    {
        // 地面应该是浅蓝色
        // 蓝色分量应该很高，绿色分量较高，红色分量中等
        return color.b > 0.9f && color.g > 0.7f && color.r > 0.5f;
    }

    /// <summary>
    /// 显示测试说明
    /// </summary>
    void ShowTestInstructions()
    {
        Debug.Log("=== 地面颜色测试器使用说明 ===");
        Debug.Log($"🧪 {testFloorKey}键 - 测试地面颜色");
        Debug.Log("📋 测试内容:");
        Debug.Log("   ✅ 检查地面颜色是否为浅蓝色");
        Debug.Log("   ✅ 验证材质属性设置");
        Debug.Log("   ✅ 判断颜色是否符合要求");
        Debug.Log("💡 也可在Inspector中使用右键菜单");
    }

    /// <summary>
    /// 创建测试球验证地面反弹
    /// </summary>
    [ContextMenu("创建测试球验证地面反弹")]
    public void CreateTestBallForFloorBounce()
    {
        Debug.Log("🎾 创建测试球验证地面反弹...");

        // 创建测试球
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "FloorBounceTestBall";
        testBall.transform.localScale = Vector3.one * 0.067f; // 网球大小
        testBall.transform.position = new Vector3(0, 2f, 0); // 在空中

        // 设置材质
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material ballMat = new Material(Shader.Find("Standard"));
        ballMat.color = Color.yellow; // 黄色便于识别
        renderer.material = ballMat;

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f; // 网球质量

        Debug.Log("✅ 黄色测试球已创建，正在下落");
        Debug.Log("预期: 球将撞击浅蓝色地面并反弹");
        Debug.Log("观察: 地面应该显示为浅蓝色，球应该有合适的反弹");

        // 10秒后清理测试球
        Destroy(testBall, 10f);
    }
}
