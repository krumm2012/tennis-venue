using UnityEngine;

/// <summary>
/// 幕布快速设置器 - 一键应用网球场地图案到Curtain
/// 用户需求：厚度0.005，网球场地纹理
/// </summary>
public class CurtainQuickSetup : MonoBehaviour
{
    [Header("快速设置")]
    [Tooltip("按此键快速设置幕布材质")]
    public KeyCode quickSetupKey = KeyCode.F5;

    [Header("诊断功能")]
    [Tooltip("按此键检查幕布状态")]
    public KeyCode diagnosticKey = KeyCode.F6;

    [Header("重置功能")]
    [Tooltip("按此键重置幕布")]
    public KeyCode resetKey = KeyCode.F7;

    private CurtainMaterialUpdater materialUpdater;
    private bool isSetupComplete = false;

    void Start()
    {
        Debug.Log("=== 幕布快速设置器已加载 ===");
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
        if (Input.GetKeyDown(quickSetupKey))
        {
            QuickSetupCurtain();
        }

        if (Input.GetKeyDown(diagnosticKey))
        {
            DiagnoseCurtainStatus();
        }

        if (Input.GetKeyDown(resetKey))
        {
            ResetCurtainToOriginal();
        }
    }

    /// <summary>
    /// 快速设置幕布
    /// </summary>
    [ContextMenu("快速设置幕布")]
    public void QuickSetupCurtain()
    {
        Debug.Log("🚀 开始快速设置幕布...");

        // 查找或创建CurtainMaterialUpdater组件
        if (materialUpdater == null)
        {
            // 查找现有的CurtainMaterialUpdater
            materialUpdater = FindObjectOfType<CurtainMaterialUpdater>();

            if (materialUpdater == null)
            {
                // 创建新的CurtainMaterialUpdater
                GameObject setupObj = new GameObject("CurtainMaterialUpdater");
                materialUpdater = setupObj.AddComponent<CurtainMaterialUpdater>();
                Debug.Log("✅ 已创建CurtainMaterialUpdater组件");
            }
        }

        // 等待一帧确保组件初始化完成
        StartCoroutine(DelayedSetup());
    }

    /// <summary>
    /// 延迟设置确保组件初始化完成
    /// </summary>
    System.Collections.IEnumerator DelayedSetup()
    {
        yield return new WaitForEndOfFrame();

        if (materialUpdater != null)
        {
            // 确保材质更新器参数正确
            materialUpdater.curtainThickness = 0.005f; // 用户要求的厚度
            materialUpdater.transparency = 0.8f; // 轻微透明

            // 应用设置
            materialUpdater.UpdateCurtainMaterial();

            isSetupComplete = true;

            Debug.Log("✅ 幕布快速设置完成！");
            Debug.Log("   - 厚度已设置为 0.005");
            Debug.Log("   - 已应用网球场地纹理");
            Debug.Log("   - 支持网球反弹功能");

            // 显示设置结果
            ShowSetupResult();
        }
        else
        {
            Debug.LogError("❌ CurtainMaterialUpdater组件创建失败");
        }
    }

    /// <summary>
    /// 诊断幕布状态
    /// </summary>
    [ContextMenu("诊断幕布状态")]
    public void DiagnoseCurtainStatus()
    {
        Debug.Log("=== 幕布状态诊断 ===");

        // 查找Curtain对象
        GameObject curtain = GameObject.Find("Curtain");
        if (curtain == null)
        {
            Debug.LogError("❌ 未找到Curtain对象！");
            Debug.LogWarning("💡 请确保场景中存在名为'Curtain'的GameObject");
            return;
        }

        // 检查基本组件
        Renderer renderer = curtain.GetComponent<Renderer>();
        Collider collider = curtain.GetComponent<Collider>();
        CurtainBehavior behavior = curtain.GetComponent<CurtainBehavior>();

        Debug.Log($"📍 Curtain位置: {curtain.transform.position}");
        Debug.Log($"📏 Curtain缩放: {curtain.transform.localScale}");
        Debug.Log($"🎨 有Renderer: {(renderer != null ? "✅" : "❌")}");
        Debug.Log($"🏀 有Collider: {(collider != null ? "✅" : "❌")}");
        Debug.Log($"⚡ 有CurtainBehavior: {(behavior != null ? "✅" : "❌")}");

        if (renderer != null && renderer.material != null)
        {
            Debug.Log($"🎨 当前材质: {renderer.material.name}");
            Debug.Log($"🖼️ 有纹理: {(renderer.material.mainTexture != null ? "✅" : "❌")}");
        }

        // 检查CurtainMaterialUpdater
        if (materialUpdater != null)
        {
            Debug.Log("🔧 CurtainMaterialUpdater状态:");
            Debug.Log(materialUpdater.GetCurtainInfo());
        }
        else
        {
            Debug.Log("🔧 CurtainMaterialUpdater: 未创建");
        }
    }

    /// <summary>
    /// 重置幕布到原始状态
    /// </summary>
    [ContextMenu("重置幕布")]
    public void ResetCurtainToOriginal()
    {
        Debug.Log("🔄 正在重置幕布...");

        if (materialUpdater != null)
        {
            materialUpdater.ResetCurtain();
            isSetupComplete = false;
            Debug.Log("✅ 幕布已重置为原始状态");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到CurtainMaterialUpdater，尝试手动重置");
            ManualResetCurtain();
        }
    }

    /// <summary>
    /// 手动重置幕布
    /// </summary>
    void ManualResetCurtain()
    {
        GameObject curtain = GameObject.Find("Curtain");
        if (curtain != null)
        {
            // 重置缩放
            curtain.transform.localScale = new Vector3(3.5f, 3f, 0.1f); // 假设的原始缩放

            // 重置为默认材质
            Renderer renderer = curtain.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = Color.gray;
            }

            Debug.Log("✅ 手动重置完成");
        }
    }

    /// <summary>
    /// 显示设置结果
    /// </summary>
    void ShowSetupResult()
    {
        GameObject curtain = GameObject.Find("Curtain");
        if (curtain != null)
        {
            Debug.Log("=== 幕布设置结果 ===");
            Debug.Log($"✅ 位置: {curtain.transform.position}");
            Debug.Log($"✅ 缩放: {curtain.transform.localScale}");
            Debug.Log($"✅ 厚度(Z轴): {curtain.transform.localScale.z}");

            Renderer renderer = curtain.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                Debug.Log($"✅ 材质名称: {renderer.material.name}");
                Debug.Log($"✅ 纹理应用: {(renderer.material.mainTexture != null ? "是" : "否")}");
            }

            CurtainBehavior behavior = curtain.GetComponent<CurtainBehavior>();
            if (behavior != null)
            {
                Debug.Log($"✅ 反弹功能: 已启用 (回弹系数: {behavior.bounceCoefficient})");
            }
        }
    }

    /// <summary>
    /// 显示快速使用说明
    /// </summary>
    void ShowQuickInstructions()
    {
        Debug.Log("=== 幕布快速设置使用说明 ===");
        Debug.Log($"🚀 {quickSetupKey}键 - 一键设置网球场地幕布（厚度0.005）");
        Debug.Log($"🔍 {diagnosticKey}键 - 检查幕布当前状态");
        Debug.Log($"🔄 {resetKey}键 - 重置幕布为原始状态");
        Debug.Log("💡 也可在Inspector中使用右键菜单功能");
        Debug.Log("📋 设置完成后将应用:");
        Debug.Log("   - 厚度: 0.005");
        Debug.Log("   - 网球场地纹理（蓝色底，白线，HeHaa文字，分数圆圈）");
        Debug.Log("   - 网球反弹物理");
    }

    /// <summary>
    /// 创建测试球验证反弹
    /// </summary>
    [ContextMenu("创建测试球验证反弹")]
    public void CreateTestBallForCurtain()
    {
        if (!isSetupComplete)
        {
            Debug.LogWarning("⚠️ 请先完成幕布设置！按F5键进行快速设置");
            return;
        }

        Debug.Log("🎾 正在创建测试球验证幕布反弹...");

        // 创建测试球
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "CurtainTestBall";
        testBall.transform.localScale = Vector3.one * 0.067f; // 网球大小

        // 设置位置（在幕布前方）
        GameObject curtain = GameObject.Find("Curtain");
        if (curtain != null)
        {
            Vector3 curtainPos = curtain.transform.position;
            testBall.transform.position = curtainPos + Vector3.back * 2f + Vector3.up * 1f;
        }
        else
        {
            testBall.transform.position = new Vector3(0, 1.5f, -3.5f);
        }

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.1f;

        // 给球一个向幕布的初始速度
        rb.velocity = new Vector3(0, 0, 10f);

        // 设置橙色材质便于识别
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material testMat = new Material(Shader.Find("Standard"));
        testMat.color = Color.red;
        renderer.material = testMat;

        Debug.Log("✅ 红色测试球已创建，正在向幕布飞行");
        Debug.Log("预期: 球将撞击幕布并反弹");

        // 5秒后清理测试球
        Destroy(testBall, 5f);
    }

    /// <summary>
    /// 获取设置状态
    /// </summary>
    public bool IsSetupComplete => isSetupComplete;
}