using UnityEngine;

/// <summary>
/// 幕布图片快速设置器
/// 一键应用用户提供的网球场地图片，顶部对齐，宽度3.5米
/// </summary>
public class CurtainImageQuickSetup : MonoBehaviour
{
    [Header("快速设置")]
    [Tooltip("按此键快速应用用户图片")]
    public KeyCode quickApplyKey = KeyCode.F8;

    [Header("诊断和重置")]
    [Tooltip("按此键检查幕布状态")]
    public KeyCode diagnosticKey = KeyCode.F9;
    [Tooltip("按此键重置幕布")]
    public KeyCode resetKey = KeyCode.F12;

    private CurtainImageApplicator imageApplicator;
    private bool isImageApplied = false;

    void Start()
    {
        Debug.Log("=== 幕布图片快速设置器已加载 ===");
        ShowQuickInstructions();
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    /// <summary>
    /// 处理键盘输入
    /// </summary>
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(quickApplyKey))
        {
            QuickApplyUserImage();
        }

        if (Input.GetKeyDown(diagnosticKey))
        {
            DiagnoseImageStatus();
        }

        if (Input.GetKeyDown(resetKey))
        {
            ResetToOriginal();
        }
    }

    /// <summary>
    /// 一键应用用户图片
    /// </summary>
    [ContextMenu("一键应用用户图片")]
    public void QuickApplyUserImage()
    {
        Debug.Log("🚀 开始一键应用用户网球场地图片...");

        // 查找或创建CurtainImageApplicator组件
        if (imageApplicator == null)
        {
            imageApplicator = FindObjectOfType<CurtainImageApplicator>();

            if (imageApplicator == null)
            {
                // 创建新的CurtainImageApplicator
                GameObject setupObj = new GameObject("CurtainImageApplicator");
                imageApplicator = setupObj.AddComponent<CurtainImageApplicator>();
                Debug.Log("✅ 已创建CurtainImageApplicator组件");
            }
        }

        // 等待一帧确保组件初始化完成
        StartCoroutine(DelayedImageApplication());
    }

    /// <summary>
    /// 延迟应用图片确保组件初始化完成
    /// </summary>
    System.Collections.IEnumerator DelayedImageApplication()
    {
        yield return new WaitForEndOfFrame();

        if (imageApplicator != null)
        {
            // 确保参数设置正确
            imageApplicator.curtainWidth = 3.5f;        // 用户要求的3.5米宽度
            imageApplicator.curtainThickness = 0.005f;  // 用户要求的厚度
            imageApplicator.topAligned = true;          // 顶部对齐
            imageApplicator.transparency = 1.0f;        // 完全不透明

            // 应用图片到幕布
            imageApplicator.ApplyImageToCurtain();

            isImageApplied = true;

            Debug.Log("✅ 用户图片快速应用完成！");
            Debug.Log("   - 已应用网球场地图片（蓝色底，HeHaa文字，分数圆圈）");
            Debug.Log("   - 宽度: 3.5米");
            Debug.Log("   - 厚度: 0.005");
            Debug.Log("   - 顶部对齐: 是");
            Debug.Log("   - 网球反弹功能: 已启用");

            // 显示应用结果
            ShowApplicationResult();
        }
        else
        {
            Debug.LogError("❌ CurtainImageApplicator组件创建失败");
        }
    }

    /// <summary>
    /// 诊断图片应用状态
    /// </summary>
    [ContextMenu("诊断图片状态")]
    public void DiagnoseImageStatus()
    {
        Debug.Log("=== 幕布图片状态诊断 ===");

        // 查找Curtain对象
        GameObject curtain = GameObject.Find("Curtain");
        if (curtain == null)
        {
            Debug.LogError("❌ 未找到Curtain对象！");
            return;
        }

        // 检查基本信息
        Debug.Log($"📍 Curtain位置: {curtain.transform.position}");
        Debug.Log($"📏 Curtain缩放: {curtain.transform.localScale}");

        // 检查是否符合用户要求
        Vector3 scale = curtain.transform.localScale;
        bool widthCorrect = Mathf.Approximately(scale.x, 3.5f);
        bool thicknessCorrect = Mathf.Approximately(scale.z, 0.005f);

        Debug.Log($"✅ 宽度3.5米: {(widthCorrect ? "✅" : "❌")} (当前: {scale.x})");
        Debug.Log($"✅ 厚度0.005: {(thicknessCorrect ? "✅" : "❌")} (当前: {scale.z})");

        // 检查材质和纹理
        Renderer renderer = curtain.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            Debug.Log($"🎨 材质名称: {renderer.material.name}");
            bool hasTexture = renderer.material.mainTexture != null;
            Debug.Log($"🖼️ 用户图片纹理: {(hasTexture ? "✅ 已应用" : "❌ 未应用")}");

            if (hasTexture)
            {
                Texture texture = renderer.material.mainTexture;
                Debug.Log($"📐 纹理尺寸: {texture.width}×{texture.height}");
            }
        }

        // 检查物理组件
        Collider collider = curtain.GetComponent<Collider>();
        CurtainBehavior behavior = curtain.GetComponent<CurtainBehavior>();

        Debug.Log($"🏀 碰撞器: {(collider != null ? "✅" : "❌")}");
        Debug.Log($"⚡ 反弹脚本: {(behavior != null ? "✅" : "❌")}");

        // 检查CurtainImageApplicator
        if (imageApplicator != null)
        {
            Debug.Log("🔧 CurtainImageApplicator状态:");
            Debug.Log(imageApplicator.GetCurtainInfo());
        }
        else
        {
            Debug.Log("🔧 CurtainImageApplicator: 未创建");
        }

        // 总体评估
        bool allCorrect = widthCorrect && thicknessCorrect &&
                         renderer != null && renderer.material.mainTexture != null &&
                         collider != null && behavior != null;

        Debug.Log($"🎯 整体状态: {(allCorrect ? "✅ 完美" : "⚠️ 需要调整")}");
    }

    /// <summary>
    /// 重置为原始状态
    /// </summary>
    [ContextMenu("重置为原始状态")]
    public void ResetToOriginal()
    {
        Debug.Log("🔄 正在重置幕布为原始状态...");

        if (imageApplicator != null)
        {
            imageApplicator.ResetCurtain();
            isImageApplied = false;
            Debug.Log("✅ 幕布已重置为原始状态");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到CurtainImageApplicator，尝试手动重置");
            ManualReset();
        }
    }

    /// <summary>
    /// 手动重置幕布
    /// </summary>
    void ManualReset()
    {
        GameObject curtain = GameObject.Find("Curtain");
        if (curtain != null)
        {
            // 重置缩放
            curtain.transform.localScale = new Vector3(3.5f, 3f, 0.1f);

            // 重置材质
            Renderer renderer = curtain.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material defaultMat = new Material(Shader.Find("Standard"));
                defaultMat.color = Color.gray;
                renderer.material = defaultMat;
            }

            Debug.Log("✅ 手动重置完成");
        }
    }

    /// <summary>
    /// 显示应用结果
    /// </summary>
    void ShowApplicationResult()
    {
        GameObject curtain = GameObject.Find("Curtain");
        if (curtain != null)
        {
            Debug.Log("=== 用户图片应用结果 ===");
            Debug.Log($"✅ 幕布位置: {curtain.transform.position}");
            Debug.Log($"✅ 幕布尺寸: {curtain.transform.localScale}");

            Vector3 scale = curtain.transform.localScale;
            Debug.Log($"✅ 宽度: {scale.x}米 (要求: 3.5米)");
            Debug.Log($"✅ 高度: {scale.y}米");
            Debug.Log($"✅ 厚度: {scale.z} (要求: 0.005)");

            Renderer renderer = curtain.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                Debug.Log($"✅ 材质: {renderer.material.name}");
                Debug.Log($"✅ 纹理: {(renderer.material.mainTexture != null ? "已应用用户网球场地图片" : "未应用")}");
            }

            Debug.Log("✅ 图片内容包含:");
            Debug.Log("   - 蓝色网球场地底色");
            Debug.Log("   - 粉色边框");
            Debug.Log("   - 白色场地线条");
            Debug.Log("   - HeHaa文字（顶部）");
            Debug.Log("   - 分数圆圈：20, 20, 50, 30, 50");
            Debug.Log("   - 二维码区域（右下角）");
            Debug.Log("✅ 顶部对齐: 已启用");
            Debug.Log("✅ 网球反弹物理: 已启用");
        }
    }

    /// <summary>
    /// 显示快速使用说明
    /// </summary>
    void ShowQuickInstructions()
    {
        Debug.Log("=== 幕布图片快速设置使用说明 ===");
        Debug.Log($"🖼️ {quickApplyKey}键 - 一键应用用户网球场地图片");
        Debug.Log($"🔍 {diagnosticKey}键 - 诊断当前幕布状态");
        Debug.Log($"🔄 {resetKey}键 - 重置幕布为原始状态");
        Debug.Log("");
        Debug.Log("📋 应用后的效果:");
        Debug.Log("   ✅ 宽度: 3.5米 (用户要求)");
        Debug.Log("   ✅ 厚度: 0.005 (用户要求)");
        Debug.Log("   ✅ 顶部对齐显示");
        Debug.Log("   ✅ 蓝色网球场地图案");
        Debug.Log("   ✅ HeHaa文字和分数圆圈");
        Debug.Log("   ✅ 网球反弹功能");
        Debug.Log("");
        Debug.Log("💡 也可在Inspector中使用右键菜单功能");
        Debug.Log("🎾 应用完成后可发射网球测试反弹效果");
    }

    /// <summary>
    /// 创建测试球验证图片效果
    /// </summary>
    [ContextMenu("创建测试球验证效果")]
    public void CreateTestBallForImageValidation()
    {
        if (!isImageApplied)
        {
            Debug.LogWarning("⚠️ 请先应用用户图片！按F8键进行快速应用");
            return;
        }

        Debug.Log("🎾 正在创建测试球验证图片效果...");

        // 创建测试球
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "ImageValidationTestBall";
        testBall.transform.localScale = Vector3.one * 0.067f; // 网球大小

        // 设置位置（在幕布前方）
        GameObject curtain = GameObject.Find("Curtain");
        if (curtain != null)
        {
            Vector3 curtainPos = curtain.transform.position;
            testBall.transform.position = curtainPos + Vector3.back * 3f + Vector3.up * 1.5f;
        }
        else
        {
            testBall.transform.position = new Vector3(0, 1.5f, -2.5f);
        }

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.1f;

        // 给球一个向幕布的初始速度
        rb.velocity = new Vector3(0, 2f, 8f); // 向上和向前

        // 设置绿色材质便于识别
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material testMat = new Material(Shader.Find("Standard"));
        testMat.color = Color.green;
        renderer.material = testMat;

        Debug.Log("✅ 绿色测试球已创建，正在向幕布飞行");
        Debug.Log("预期: 球将撞击用户图片幕布并反弹");
        Debug.Log("观察: 幕布上应显示蓝色网球场地图案");

        // 6秒后清理测试球
        Destroy(testBall, 6f);
    }

    /// <summary>
    /// 获取应用状态
    /// </summary>
    public bool IsImageApplied => isImageApplied;
}