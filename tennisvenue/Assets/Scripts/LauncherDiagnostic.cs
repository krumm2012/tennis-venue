using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 发球机诊断工具 - 解决鼠标左键无法发球和圆环标识不显示的问题
/// </summary>
public class LauncherDiagnostic : MonoBehaviour
{
    [Header("诊断设置")]
    public bool enableMouseInputDiagnostic = true;
    public bool enableBallNamingFix = true;
    public bool enableDetailedLogging = true;

    private BallLauncher ballLauncher;
    private BounceImpactMarker impactMarker;

    void Start()
    {
        Debug.Log("=== 发球机诊断工具启动 ===");

        // 查找组件
        ballLauncher = FindObjectOfType<BallLauncher>();
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (ballLauncher == null)
        {
            Debug.LogError("❌ 未找到BallLauncher组件！");
            return;
        }

        if (impactMarker == null)
        {
            Debug.LogError("❌ 未找到BounceImpactMarker组件！");
        }

        Debug.Log("✅ 发球机诊断工具初始化完成");
        Debug.Log("快捷键:");
        Debug.Log("  F1: 诊断鼠标输入问题");
        Debug.Log("  F2: 修复球体命名问题");
        Debug.Log("  F3: 测试发球机发射");
        Debug.Log("  F4: 检查圆环标识系统");
        Debug.Log("  F8: 检查当前鼠标状态");
        Debug.Log("💡 注意: 空格键和鼠标左键已保留给BallLauncher正常发射");
    }

    void Update()
    {
        // 实时监控鼠标输入
        if (enableMouseInputDiagnostic)
        {
            MonitorMouseInput();
        }

        // 快捷键控制
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DiagnoseMouseInput();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            FixBallNaming();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            TestLauncherFiring();
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            CheckRingSystem();
        }
    }

    /// <summary>
    /// 手动检查鼠标状态（改为F8键触发，避免与BallLauncher冲突）
    /// </summary>
    void MonitorMouseInput()
    {
        // 移除自动鼠标监听，改为按键触发检查
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Debug.Log("🔍 F8键触发鼠标状态检查");
            CheckCurrentMouseState();
        }
    }
    
    /// <summary>
    /// 检查当前鼠标状态
    /// </summary>
    void CheckCurrentMouseState()
    {
        // 检查是否点击在UI上
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.LogWarning("⚠️ 当前鼠标位置在UI元素上");

            // 获取当前鼠标下的UI元素
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                Debug.Log($"   UI元素: {result.gameObject.name}");
            }
        }
        else
        {
            Debug.Log("✅ 当前鼠标位置在游戏区域");
        }
    }

    /// <summary>
    /// 诊断鼠标输入问题
    /// </summary>
    void DiagnoseMouseInput()
    {
        Debug.Log("=== 鼠标输入诊断 ===");

        // 检查EventSystem
        if (EventSystem.current == null)
        {
            Debug.LogWarning("⚠️ 场景中没有EventSystem，这可能影响UI交互");
        }
        else
        {
            Debug.Log("✅ EventSystem存在");
        }

        // 检查Canvas设置
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"场景中的Canvas数量: {canvases.Length}");

        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"Canvas: {canvas.name}");
            Debug.Log($"  渲染模式: {canvas.renderMode}");
            Debug.Log($"  排序层级: {canvas.sortingOrder}");
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            bool blocksRaycasts = raycaster != null && raycaster.enabled;
            Debug.Log($"  阻挡射线: {blocksRaycasts}");

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay && blocksRaycasts)
            {
                Debug.LogWarning($"⚠️ Canvas '{canvas.name}' 可能阻挡鼠标输入");
            }
        }

        // 检查BallLauncher的Update方法
        if (ballLauncher != null)
        {
            Debug.Log("✅ BallLauncher组件存在");
            Debug.Log("   检查Input.GetMouseButtonDown(0)是否正常工作...");

            // 强制测试鼠标输入
            if (Input.GetMouseButton(0))
            {
                Debug.Log("✅ 当前鼠标左键正在按下");
            }
            else
            {
                Debug.Log("ℹ️ 当前鼠标左键未按下");
            }
        }
    }

    /// <summary>
    /// 修复球体命名问题
    /// </summary>
    void FixBallNaming()
    {
        Debug.Log("=== 修复球体命名问题 ===");

        // 检查ballPrefab的名称
        if (ballLauncher != null && ballLauncher.ballPrefab != null)
        {
            string prefabName = ballLauncher.ballPrefab.name;
            Debug.Log($"当前球体预制体名称: {prefabName}");

            if (!prefabName.Contains("TennisBall"))
            {
                Debug.LogWarning($"⚠️ 球体预制体名称 '{prefabName}' 不包含 'TennisBall'");
                Debug.LogWarning("   这会导致BounceImpactMarker无法检测到球体");

                // 尝试修复预制体名称
                ballLauncher.ballPrefab.name = "TennisBall";
                Debug.Log("✅ 已将球体预制体名称修改为 'TennisBall'");
            }
            else
            {
                Debug.Log("✅ 球体预制体名称正确");
            }
        }

        // 检查场景中现有的球体
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ballCount = 0;
        int fixedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Ball") && obj.GetComponent<Rigidbody>() != null)
            {
                ballCount++;

                if (!obj.name.Contains("TennisBall"))
                {
                    string oldName = obj.name;
                    obj.name = "TennisBall_" + obj.GetInstanceID();
                    Debug.Log($"🔧 修复球体名称: {oldName} → {obj.name}");
                    fixedCount++;
                }
            }
        }

        Debug.Log($"📊 球体检查结果: 总数{ballCount}, 修复{fixedCount}个");
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

        // 检查必要组件
        if (ballLauncher.ballPrefab == null)
        {
            Debug.LogError("❌ ballPrefab未设置");
            return;
        }

        if (ballLauncher.launchPoint == null)
        {
            Debug.LogError("❌ launchPoint未设置");
            return;
        }

        Debug.Log("✅ 发球机组件检查通过");
        Debug.Log($"   球体预制体: {ballLauncher.ballPrefab.name}");
        Debug.Log($"   发射点: {ballLauncher.launchPoint.position}");

        // 强制发射一个球
        Debug.Log("🚀 强制发射测试球...");
        ballLauncher.LaunchBall(Vector3.zero);

        // 等待一秒后检查球是否创建
        StartCoroutine(CheckBallCreation());
    }

    /// <summary>
    /// 检查球是否成功创建
    /// </summary>
    System.Collections.IEnumerator CheckBallCreation()
    {
        yield return new WaitForSeconds(1f);

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundNewBall = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall") && obj.GetComponent<Rigidbody>() != null)
            {
                Vector3 pos = obj.transform.position;
                Vector3 vel = obj.GetComponent<Rigidbody>().velocity;

                Debug.Log($"✅ 找到测试球: {obj.name}");
                Debug.Log($"   位置: {pos}");
                Debug.Log($"   速度: {vel.magnitude:F2}m/s");
                foundNewBall = true;
                break;
            }
        }

        if (!foundNewBall)
        {
            Debug.LogError("❌ 未找到新创建的球体！发射可能失败");
        }
    }

    /// <summary>
    /// 检查圆环标识系统
    /// </summary>
    void CheckRingSystem()
    {
        Debug.Log("=== 检查圆环标识系统 ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker组件未找到");
            return;
        }

        Debug.Log("✅ BounceImpactMarker组件存在");
        Debug.Log($"   系统启用: {impactMarker.enableImpactMarkers}");
        Debug.Log($"   活动标记数: {impactMarker.GetActiveMarkerCount()}");

        if (!impactMarker.enableImpactMarkers)
        {
            Debug.LogWarning("⚠️ 圆环标识系统已禁用！按F3键启用");
        }

        // 检查球体检测逻辑
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int detectedBalls = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                detectedBalls++;
                Vector3 pos = obj.transform.position;
                Rigidbody rb = obj.GetComponent<Rigidbody>();

                Debug.Log($"🎾 检测到球体: {obj.name}");
                Debug.Log($"   位置: {pos}");

                if (rb != null)
                {
                    Debug.Log($"   速度: {rb.velocity.magnitude:F2}m/s");

                    // 检查是否满足圆环创建条件
                    bool heightOK = pos.y <= 0.5f && pos.y >= -1f;
                    bool velocityOK = rb.velocity.y < -0.5f;
                    bool speedOK = rb.velocity.magnitude > 1.5f;

                    Debug.Log($"   圆环条件: 高度({heightOK}) 速度({velocityOK}) 速率({speedOK})");
                }
            }
        }

        Debug.Log($"📊 总检测球体数: {detectedBalls}");

        if (detectedBalls == 0)
        {
            Debug.LogWarning("⚠️ 未检测到任何TennisBall对象");
            Debug.LogWarning("   请先发射球体或按F2修复球体命名");
        }
    }

    void OnDestroy()
    {
        Debug.Log("发球机诊断工具已关闭");
    }
}