using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 快速发球机修复工具 - 一键解决鼠标左键和圆环标识问题
/// </summary>
public class QuickLauncherFix : MonoBehaviour
{
    [Header("修复设置")]
    public bool autoFixOnStart = true;
    public bool enableDebugMode = true;

    private BallLauncher ballLauncher;
    private BounceImpactMarker impactMarker;

    void Start()
    {
        Debug.Log("=== 快速发球机修复工具启动 ===");

        // 查找组件
        ballLauncher = FindObjectOfType<BallLauncher>();
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (autoFixOnStart)
        {
            PerformQuickFix();
        }

        Debug.Log("快捷键:");
        Debug.Log("  F1: 一键修复所有问题");
        Debug.Log("  F2: 测试鼠标左键发球");
        Debug.Log("  F3: 强制启用圆环标识");
        Debug.Log("  F4: 创建测试球验证圆环");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PerformQuickFix();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            TestMouseLaunch();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            ForceEnableRingSystem();
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            CreateTestBallForRing();
        }

        // 实时监控鼠标输入
        if (enableDebugMode && Input.GetMouseButtonDown(0))
        {
            CheckMouseInput();
        }
    }

    /// <summary>
    /// 执行一键修复
    /// </summary>
    void PerformQuickFix()
    {
        Debug.Log("=== 执行一键修复 ===");

        // 1. 修复UI阻挡问题
        FixUIBlocking();

        // 2. 修复球体命名问题
        FixBallNaming();

        // 3. 启用圆环标识系统
        EnableRingSystem();

        // 4. 验证发球机设置
        VerifyLauncherSetup();

        Debug.Log("✅ 一键修复完成！");
        Debug.Log("现在可以尝试:");
        Debug.Log("  - 鼠标左键发球");
        Debug.Log("  - 空格键发球");
        Debug.Log("  - 观察圆环标识");
    }

    /// <summary>
    /// 修复UI阻挡问题
    /// </summary>
    void FixUIBlocking()
    {
        Debug.Log("--- 修复UI阻挡问题 ---");

        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
                    // 检查Canvas是否可能阻挡鼠标输入
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // 暂时禁用Canvas的射线阻挡，但保持UI可见
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
            {
                raycaster.enabled = false;
                Debug.Log($"🔧 暂时禁用Canvas '{canvas.name}' 的射线阻挡");
            }
        }
        }

        // 1秒后重新启用UI射线检测
        Invoke("RestoreUIRaycasting", 1f);
    }

    /// <summary>
    /// 恢复UI射线检测
    /// </summary>
    void RestoreUIRaycasting()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null && !raycaster.enabled)
            {
                raycaster.enabled = true;
                Debug.Log($"🔧 恢复Canvas '{canvas.name}' 的射线阻挡");
            }
        }
    }

    /// <summary>
    /// 修复球体命名问题
    /// </summary>
    void FixBallNaming()
    {
        Debug.Log("--- 修复球体命名问题 ---");

        if (ballLauncher != null && ballLauncher.ballPrefab != null)
        {
            // 确保预制体名称正确
            if (!ballLauncher.ballPrefab.name.Contains("TennisBall"))
            {
                ballLauncher.ballPrefab.name = "TennisBall";
                Debug.Log("🔧 修复球体预制体名称为 'TennisBall'");
            }
            else
            {
                Debug.Log("✅ 球体预制体名称正确");
            }
        }

        // 修复场景中现有球体的名称
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int fixedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Ball") && obj.GetComponent<Rigidbody>() != null)
            {
                if (!obj.name.Contains("TennisBall"))
                {
                    obj.name = "TennisBall_" + obj.GetInstanceID();
                    fixedCount++;
                }
            }
        }

        if (fixedCount > 0)
        {
            Debug.Log($"🔧 修复了 {fixedCount} 个球体的名称");
        }
    }

    /// <summary>
    /// 启用圆环标识系统
    /// </summary>
    void EnableRingSystem()
    {
        Debug.Log("--- 启用圆环标识系统 ---");

        if (impactMarker == null)
        {
            Debug.LogError("❌ 未找到BounceImpactMarker组件");
            return;
        }

        // 强制启用系统
        impactMarker.enableImpactMarkers = true;
        Debug.Log("✅ 圆环标识系统已启用");

        // 清除所有已标记的球体，重新开始检测
        var markedBallsField = impactMarker.GetType().GetField("markedBalls",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (markedBallsField != null)
        {
            var markedBalls = markedBallsField.GetValue(impactMarker);
            if (markedBalls != null)
            {
                var clearMethod = markedBalls.GetType().GetMethod("Clear");
                clearMethod?.Invoke(markedBalls, null);
                Debug.Log("🔧 清除所有球体标记状态，重新开始检测");
            }
        }
    }

    /// <summary>
    /// 验证发球机设置
    /// </summary>
    void VerifyLauncherSetup()
    {
        Debug.Log("--- 验证发球机设置 ---");

        if (ballLauncher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件");
            return;
        }

        bool allGood = true;

        if (ballLauncher.ballPrefab == null)
        {
            Debug.LogError("❌ ballPrefab未设置");
            allGood = false;
        }

        if (ballLauncher.launchPoint == null)
        {
            Debug.LogError("❌ launchPoint未设置");
            allGood = false;
        }

        if (allGood)
        {
            Debug.Log("✅ 发球机设置正确");
        }
    }

    /// <summary>
    /// 测试鼠标左键发球
    /// </summary>
    void TestMouseLaunch()
    {
        Debug.Log("=== 测试鼠标左键发球 ===");

        if (ballLauncher == null)
        {
            Debug.LogError("❌ BallLauncher组件未找到");
            return;
        }

        Debug.Log("🚀 强制触发发球...");
        ballLauncher.LaunchBall(Vector3.zero);

        // 检查是否成功创建球体
        StartCoroutine(CheckBallCreation());
    }

    /// <summary>
    /// 检查球体创建
    /// </summary>
    System.Collections.IEnumerator CheckBallCreation()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundNewBall = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall") && obj.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb.velocity.magnitude > 1f) // 正在运动的球
                {
                    Debug.Log($"✅ 发球成功！球体: {obj.name}");
                    Debug.Log($"   位置: {obj.transform.position}");
                    Debug.Log($"   速度: {rb.velocity.magnitude:F2}m/s");
                    foundNewBall = true;
                    break;
                }
            }
        }

        if (!foundNewBall)
        {
            Debug.LogWarning("⚠️ 未检测到新的运动球体，发球可能失败");
        }
    }

    /// <summary>
    /// 强制启用圆环标识
    /// </summary>
    void ForceEnableRingSystem()
    {
        Debug.Log("=== 强制启用圆环标识 ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker组件未找到");
            return;
        }

        impactMarker.enableImpactMarkers = true;
        Debug.Log("✅ 圆环标识系统已强制启用");
        Debug.Log($"   当前活动标记数: {impactMarker.GetActiveMarkerCount()}");
    }

    /// <summary>
    /// 创建测试球验证圆环
    /// </summary>
    void CreateTestBallForRing()
    {
        Debug.Log("=== 创建测试球验证圆环 ===");

        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "TennisBall_RingTest";
        testBall.transform.position = new Vector3(0, 2f, 0);
        testBall.transform.localScale = Vector3.one * 0.067f;

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;
        rb.drag = 0.02f;
        rb.useGravity = true;

        // 设置明显的颜色
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.red;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.red * 0.5f);
        renderer.material = mat;

        // 给球一个向下的速度
        rb.velocity = new Vector3(0, -5f, 0);

        Debug.Log("🎾 红色测试球已创建，应该在落地时产生圆环标识");

        // 5秒后销毁
        Destroy(testBall, 5f);
    }

    /// <summary>
    /// 检查鼠标输入
    /// </summary>
    void CheckMouseInput()
    {
        Debug.Log("🖱️ 检测到鼠标左键点击");

        // 检查是否点击在UI上
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.LogWarning("⚠️ 鼠标点击在UI元素上，可能阻挡了发球");
        }
        else
        {
            Debug.Log("✅ 鼠标点击在游戏区域，应该能触发发球");
        }
    }

    void OnDestroy()
    {
        Debug.Log("快速发球机修复工具已关闭");
    }
}