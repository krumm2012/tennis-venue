using UnityEngine;

/// <summary>
/// 网球场标识线快速设置脚本
/// 提供快捷键操作来快速设置和管理网球场标识线
/// H键：创建网球场标识线，J键：清除标识线，K键：切换发球机区域显示
/// </summary>
public class HalfCourtLineQuickSetup : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private TennisCourtLineRenderer lineRenderer;

    [Header("快捷键设置")]
    [SerializeField] private KeyCode createLinesKey = KeyCode.H; // H键创建半场标识线
    [SerializeField] private KeyCode clearLinesKey = KeyCode.J;  // J键清除标识线
    [SerializeField] private KeyCode toggleLauncherZoneKey = KeyCode.K; // K键切换发球机区域
    [SerializeField] private KeyCode diagnosticKey = KeyCode.L; // L键诊断功能

    [Header("状态信息")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool autoFindRenderer = true;

    void Start()
    {
        // 自动查找TennisCourtLineRenderer组件
        if (autoFindRenderer && lineRenderer == null)
        {
            FindLineRenderer();
        }

        if (showDebugInfo)
        {
            Debug.Log("HalfCourtLineQuickSetup: 网球场标识线快速设置已激活");
            Debug.Log($"H键: 创建网球场标识线");
            Debug.Log($"J键: 清除所有标识线");
            Debug.Log($"K键: 切换发球机区域显示");
            Debug.Log($"L键: 诊断标识线状态");
        }
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    /// <summary>
    /// 处理键盘快捷键输入
    /// </summary>
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(createLinesKey))
        {
            CreateHalfCourtLines();
        }
        else if (Input.GetKeyDown(clearLinesKey))
        {
            ClearAllLines();
        }
        else if (Input.GetKeyDown(toggleLauncherZoneKey))
        {
            ToggleLauncherZone();
        }
        else if (Input.GetKeyDown(diagnosticKey))
        {
            RunDiagnostic();
        }
    }

    /// <summary>
    /// 创建半场标识线
    /// </summary>
    [ContextMenu("创建半场标识线")]
    public void CreateHalfCourtLines()
    {
        if (lineRenderer == null)
        {
            FindLineRenderer();
        }

        if (lineRenderer == null)
        {
            Debug.LogError("HalfCourtLineQuickSetup: 未找到TennisCourtLineRenderer组件！");
            return;
        }

        lineRenderer.CreateHalfCourtLines();

        if (showDebugInfo)
        {
            Debug.Log($"HalfCourtLineQuickSetup: 半场标识线创建完成！共 {lineRenderer.GetLineCount()} 条线");
        }
    }

    /// <summary>
    /// 清除所有标识线
    /// </summary>
    [ContextMenu("清除所有标识线")]
    public void ClearAllLines()
    {
        if (lineRenderer == null)
        {
            FindLineRenderer();
        }

        if (lineRenderer == null)
        {
            Debug.LogError("HalfCourtLineQuickSetup: 未找到TennisCourtLineRenderer组件！");
            return;
        }

        lineRenderer.ClearExistingLines();

        if (showDebugInfo)
        {
            Debug.Log("HalfCourtLineQuickSetup: 所有标识线已清除");
        }
    }

    /// <summary>
    /// 切换发球机区域显示
    /// </summary>
    [ContextMenu("切换发球机区域显示")]
    public void ToggleLauncherZone()
    {
        if (lineRenderer == null)
        {
            FindLineRenderer();
        }

        if (lineRenderer == null)
        {
            Debug.LogError("HalfCourtLineQuickSetup: 未找到TennisCourtLineRenderer组件！");
            return;
        }

        // 获取当前状态并切换
        bool currentState = GetLauncherZoneState();
        bool newState = !currentState;

        lineRenderer.ToggleLauncherZone(newState);

        if (showDebugInfo)
        {
            Debug.Log($"HalfCourtLineQuickSetup: 发球机区域显示已{(newState ? "开启" : "关闭")}");
        }
    }

    /// <summary>
    /// 运行诊断功能
    /// </summary>
    [ContextMenu("运行诊断")]
    public void RunDiagnostic()
    {
        Debug.Log("=== 网球场标识线系统诊断 ===");

        // 检查LineRenderer组件
        if (lineRenderer == null)
        {
            FindLineRenderer();
        }

        if (lineRenderer == null)
        {
            Debug.LogError("❌ 未找到TennisCourtLineRenderer组件");
            return;
        }
        Debug.Log("✅ TennisCourtLineRenderer组件已找到");

        // 检查Floor对象
        GameObject floor = GameObject.Find("Floor");
        if (floor == null)
        {
            Debug.LogError("❌ 未找到Floor对象");
        }
        else
        {
            Debug.Log($"✅ Floor对象已找到 - 位置: {floor.transform.position}, 尺寸: {floor.transform.localScale}");
        }

        // 检查线条数量
        int lineCount = lineRenderer.GetLineCount();
        Debug.Log($"📊 当前线条数量: {lineCount}");

        if (lineCount == 0)
        {
            Debug.LogWarning("⚠️ 当前没有标识线，建议按H键创建");
        }
        else
        {
            Debug.Log("✅ 标识线系统正常运行");
        }

        // 检查发球机区域状态
        bool launcherZoneState = GetLauncherZoneState();
        Debug.Log($"🎯 发球机区域显示: {(launcherZoneState ? "开启" : "关闭")}");

        // 显示快捷键信息
        Debug.Log("🎮 快捷键操作:");
        Debug.Log($"  H键: 创建网球场标识线");
        Debug.Log($"  J键: 清除所有标识线");
        Debug.Log($"  K键: 切换发球机区域显示");
        Debug.Log($"  L键: 运行诊断");

        Debug.Log("=== 诊断完成 ===");
    }

    /// <summary>
    /// 自动查找TennisCourtLineRenderer组件
    /// </summary>
    private void FindLineRenderer()
    {
        // 首先在同一GameObject上查找
        lineRenderer = GetComponent<TennisCourtLineRenderer>();

        if (lineRenderer == null)
        {
            // 在场景中查找
            lineRenderer = FindObjectOfType<TennisCourtLineRenderer>();
        }

        if (lineRenderer == null)
        {
            // 创建新的TennisCourtLineRenderer组件
            CreateLineRendererComponent();
        }

        if (showDebugInfo && lineRenderer != null)
        {
            Debug.Log($"HalfCourtLineQuickSetup: 找到TennisCourtLineRenderer组件在 {lineRenderer.name}");
        }
    }

    /// <summary>
    /// 创建新的TennisCourtLineRenderer组件
    /// </summary>
    private void CreateLineRendererComponent()
    {
        // 查找Floor对象作为父对象
        GameObject floor = GameObject.Find("Floor");
        GameObject targetObject = floor != null ? floor : this.gameObject;

        lineRenderer = targetObject.GetComponent<TennisCourtLineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = targetObject.AddComponent<TennisCourtLineRenderer>();

            if (showDebugInfo)
            {
                Debug.Log($"HalfCourtLineQuickSetup: 在 {targetObject.name} 上创建了新的TennisCourtLineRenderer组件");
            }
        }
    }

    /// <summary>
    /// 获取发球机区域显示状态
    /// </summary>
    private bool GetLauncherZoneState()
    {
        if (lineRenderer == null) return false;

        // 通过检查是否存在发球机区域相关的线条来判断状态
        Transform[] children = lineRenderer.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Contains("发球机区域"))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 设置调试信息显示
    /// </summary>
    public void SetDebugInfo(bool enabled)
    {
        showDebugInfo = enabled;
    }

    /// <summary>
    /// 设置线条颜色
    /// </summary>
    public void SetLineColor(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetLineColor(color);
        }
    }

    /// <summary>
    /// 设置线条宽度
    /// </summary>
    public void SetLineWidth(float width)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetLineWidth(width);
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        // 在屏幕上显示快捷键提示
        GUI.color = Color.white;
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;

        string helpText = "网球场标识线快捷键:\nH-创建线条  J-清除线条  K-切换发球机区域  L-诊断";
        GUI.Label(new Rect(10, Screen.height - 80, 300, 70), helpText, style);
    }
}