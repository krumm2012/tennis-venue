using UnityEngine;

/// <summary>
/// 简化版发球机修复工具 - 解决鼠标左键无法发球和圆环标识不显示的问题
/// </summary>
public class SimpleLauncherFix : MonoBehaviour
{
    [Header("修复设置")]
    public bool autoFixOnStart = true;

    private BallLauncher ballLauncher;
    private BounceImpactMarker impactMarker;

    void Start()
    {
        Debug.Log("=== 简化版发球机修复工具启动 ===");

        // 查找组件
        ballLauncher = FindObjectOfType<BallLauncher>();
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (autoFixOnStart)
        {
            PerformQuickFix();
        }

        Debug.Log("快捷键:");
        Debug.Log("  F1: 一键修复所有问题");
        Debug.Log("  F2: 测试发球机发射");
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
            TestLauncherFiring();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            ForceEnableRingSystem();
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            CreateTestBallForRing();
        }
    }

    /// <summary>
    /// 执行一键修复
    /// </summary>
    void PerformQuickFix()
    {
        Debug.Log("=== 执行一键修复 ===");

        // 1. 修复球体命名问题
        FixBallNaming();

        // 2. 启用圆环标识系统
        EnableRingSystem();

        // 3. 验证发球机设置
        VerifyLauncherSetup();

        Debug.Log("✅ 一键修复完成！");
        Debug.Log("现在可以尝试:");
        Debug.Log("  - 鼠标左键发球");
        Debug.Log("  - 空格键发球");
        Debug.Log("  - 观察圆环标识");
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
        try
        {
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
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ 清除标记状态时出错: {e.Message}");
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
    /// 测试发球机发射
    /// </summary>
    void TestLauncherFiring()
    {
        Debug.Log("=== 测试发球机发射 ===");

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

    void OnDestroy()
    {
        Debug.Log("简化版发球机修复工具已关闭");
    }
}