using UnityEngine;

/// <summary>
/// 轨迹线拖动功能测试脚本
/// 用于验证和演示轨迹线拖动控制功能
/// </summary>
public class TrajectoryDragTest : MonoBehaviour
{
    [Header("测试配置")]
    public bool enableTestMode = true;
    public bool showInstructions = true;

    private TrajectoryDragController dragController;
    private BallLauncher ballLauncher;
    private bool testModeActive = false;

    void Start()
    {
        if (!enableTestMode) return;

        Debug.Log("=== 轨迹线拖动功能测试启动 ===");

        // 查找组件
        dragController = FindObjectOfType<TrajectoryDragController>();
        ballLauncher = FindObjectOfType<BallLauncher>();

        if (dragController == null)
        {
            Debug.LogWarning("⚠️ 未找到TrajectoryDragController，将自动创建");
            CreateDragController();
        }
        else
        {
            Debug.Log("✅ 找到TrajectoryDragController组件");
        }

        if (ballLauncher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件");
            return;
        }

        // 显示使用说明
        ShowUsageInstructions();

        Debug.Log("📋 测试快捷键:");
        Debug.Log("  F1: 切换拖动功能开关");
        Debug.Log("  F2: 切换调试信息显示");
        Debug.Log("  F3: 运行自动测试");
        Debug.Log("  F4: 重置发球机参数");
        Debug.Log("  F5: 显示使用说明");
    }

    void Update()
    {
        if (!enableTestMode) return;

        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleDragFunction();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleDebugInfo();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            RunAutoTest();
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            ResetLauncherParameters();
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            ShowUsageInstructions();
        }
    }

    /// <summary>
    /// 自动创建拖动控制器
    /// </summary>
    void CreateDragController()
    {
        GameObject dragControllerObj = new GameObject("TrajectoryDragController");
        dragController = dragControllerObj.AddComponent<TrajectoryDragController>();

        // 自动配置
        dragController.ballLauncher = ballLauncher;
        dragController.mainCamera = Camera.main;

        if (ballLauncher != null)
        {
            dragController.trajectoryLine = ballLauncher.trajectoryLine;
        }

        Debug.Log("✅ 已自动创建TrajectoryDragController");
    }

    /// <summary>
    /// 切换拖动功能
    /// </summary>
    void ToggleDragFunction()
    {
        if (dragController != null)
        {
            dragController.ToggleDragControl();
        }
        else
        {
            Debug.LogWarning("⚠️ TrajectoryDragController未找到");
        }
    }

    /// <summary>
    /// 切换调试信息
    /// </summary>
    void ToggleDebugInfo()
    {
        if (dragController != null)
        {
            dragController.showDebugInfo = !dragController.showDebugInfo;
            Debug.Log($"🔍 调试信息显示: {(dragController.showDebugInfo ? "开启" : "关闭")}");
        }
    }

    /// <summary>
    /// 运行自动测试
    /// </summary>
    void RunAutoTest()
    {
        if (dragController == null || ballLauncher == null)
        {
            Debug.LogError("❌ 缺少必要组件，无法运行测试");
            return;
        }

        Debug.Log("🔄 开始轨迹线拖动自动测试...");
        StartCoroutine(AutoTestSequence());
    }

    /// <summary>
    /// 自动测试序列
    /// </summary>
    System.Collections.IEnumerator AutoTestSequence()
    {
        testModeActive = true;

        // 测试1: 检查组件状态
        Debug.Log("📋 测试1: 检查组件状态");
        yield return new WaitForSeconds(1f);

        bool componentsOK = true;
        if (dragController.ballLauncher == null)
        {
            Debug.LogError("❌ BallLauncher引用缺失");
            componentsOK = false;
        }

        if (dragController.trajectoryLine == null)
        {
            Debug.LogError("❌ TrajectoryLine引用缺失");
            componentsOK = false;
        }

        if (dragController.mainCamera == null)
        {
            Debug.LogError("❌ MainCamera引用缺失");
            componentsOK = false;
        }

        if (componentsOK)
        {
            Debug.Log("✅ 所有组件引用正常");
        }

        yield return new WaitForSeconds(2f);

        // 测试2: 检查轨迹线状态
        Debug.Log("📋 测试2: 检查轨迹线状态");

        if (dragController.trajectoryLine.positionCount > 0)
        {
            Debug.Log($"✅ 轨迹线包含 {dragController.trajectoryLine.positionCount} 个点");

            // 显示轨迹线的起点和终点
            Vector3 startPoint = dragController.trajectoryLine.GetPosition(0);
            Vector3 endPoint = dragController.trajectoryLine.GetPosition(dragController.trajectoryLine.positionCount - 1);
            Debug.Log($"📍 轨迹线起点: {startPoint}");
            Debug.Log($"📍 轨迹线终点: {endPoint}");
        }
        else
        {
            Debug.LogWarning("⚠️ 轨迹线没有点数据");
        }

        yield return new WaitForSeconds(2f);

        // 测试3: 模拟参数变化
        Debug.Log("📋 测试3: 模拟参数变化");

        float originalAngle = ballLauncher.angleSlider?.value ?? 45f;
        float originalSpeed = ballLauncher.speedSlider?.value ?? 20f;
        float originalDirection = ballLauncher.directionSlider?.value ?? 0f;

        Debug.Log($"📊 原始参数 - 角度: {originalAngle:F1}°, 速度: {originalSpeed:F1}, 方向: {originalDirection:F1}°");

        // 改变参数并观察轨迹线变化
        if (ballLauncher.angleSlider != null)
        {
            ballLauncher.angleSlider.value = 30f;
            yield return new WaitForSeconds(1f);
            Debug.Log("🔄 角度调整为30°");
        }

        if (ballLauncher.speedSlider != null)
        {
            ballLauncher.speedSlider.value = 25f;
            yield return new WaitForSeconds(1f);
            Debug.Log("🔄 速度调整为25");
        }

        if (ballLauncher.directionSlider != null)
        {
            ballLauncher.directionSlider.value = 15f;
            yield return new WaitForSeconds(1f);
            Debug.Log("🔄 方向调整为15°");
        }

        yield return new WaitForSeconds(2f);

        // 恢复原始参数
        Debug.Log("🔄 恢复原始参数");
        if (ballLauncher.angleSlider != null) ballLauncher.angleSlider.value = originalAngle;
        if (ballLauncher.speedSlider != null) ballLauncher.speedSlider.value = originalSpeed;
        if (ballLauncher.directionSlider != null) ballLauncher.directionSlider.value = originalDirection;

        yield return new WaitForSeconds(1f);

        Debug.Log("✅ 自动测试完成");
        testModeActive = false;
    }

    /// <summary>
    /// 重置发球机参数
    /// </summary>
    void ResetLauncherParameters()
    {
        if (ballLauncher == null) return;

        Debug.Log("🔄 重置发球机参数到默认值");

        if (ballLauncher.angleSlider != null)
        {
            ballLauncher.angleSlider.value = 45f;
        }

        if (ballLauncher.speedSlider != null)
        {
            ballLauncher.speedSlider.value = 20f;
        }

        if (ballLauncher.directionSlider != null)
        {
            ballLauncher.directionSlider.value = 0f;
        }

        Debug.Log("✅ 参数重置完成");
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void ShowUsageInstructions()
    {
        Debug.Log("📖 === 轨迹线拖动功能使用说明 ===");
        Debug.Log("🎯 基本操作:");
        Debug.Log("  1. 将鼠标悬停在轨迹线上（虚线）");
        Debug.Log("  2. 按住鼠标左键开始拖动");
        Debug.Log("  3. 拖动鼠标到期望的落点位置");
        Debug.Log("  4. 释放鼠标左键完成调整");
        Debug.Log("  5. 按ESC键可以取消拖动并恢复原始参数");
        Debug.Log("");
        Debug.Log("🔧 功能特性:");
        Debug.Log("  • 实时计算最佳发球角度、速度和方向");
        Debug.Log("  • 黄色高亮显示拖动状态");
        Debug.Log("  • 黄色球体指示器显示目标落点");
        Debug.Log("  • 自动更新所有UI滑块值");
        Debug.Log("  • 支持参数范围限制和约束");
        Debug.Log("");
        Debug.Log("⚠️ 注意事项:");
        Debug.Log("  • 确保地面对象在正确的图层（groundLayerMask）");
        Debug.Log("  • 拖动检测半径默认为0.5米");
        Debug.Log("  • 目标位置受发球机物理限制约束");
        Debug.Log("  • 计算基于简化的抛物线模型");
    }

    void OnGUI()
    {
        if (!enableTestMode || !showInstructions) return;

        // 显示使用说明面板
        GUILayout.BeginArea(new Rect(Screen.width - 350, 10, 340, 400));
        GUILayout.Box("轨迹线拖动功能指南");

        GUILayout.Label("🎯 如何使用:");
        GUILayout.Label("1. 鼠标悬停在轨迹线上");
        GUILayout.Label("2. 按住左键拖动到目标位置");
        GUILayout.Label("3. 释放鼠标完成调整");
        GUILayout.Label("4. ESC键取消拖动");

        GUILayout.Space(10);

        GUILayout.Label("🔧 测试功能:");
        if (GUILayout.Button("切换拖动功能 (F1)"))
        {
            ToggleDragFunction();
        }

        if (GUILayout.Button("切换调试信息 (F2)"))
        {
            ToggleDebugInfo();
        }

        if (GUILayout.Button("运行自动测试 (F3)"))
        {
            RunAutoTest();
        }

        if (GUILayout.Button("重置参数 (F4)"))
        {
            ResetLauncherParameters();
        }

        GUILayout.Space(10);

        // 显示状态信息
        if (dragController != null)
        {
            GUILayout.Label($"拖动功能: {(dragController.enableDragControl ? "启用" : "禁用")}");
            GUILayout.Label($"调试信息: {(dragController.showDebugInfo ? "显示" : "隐藏")}");
        }

        if (testModeActive)
        {
            GUILayout.Label("🔄 自动测试运行中...");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("隐藏指南"))
        {
            showInstructions = false;
        }

        GUILayout.EndArea();
    }
}