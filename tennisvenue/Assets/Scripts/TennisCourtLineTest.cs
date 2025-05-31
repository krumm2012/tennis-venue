using UnityEngine;

/// <summary>
/// 网球场线条渲染器测试脚本
/// 用于快速测试和验证线条渲染功能
/// </summary>
public class TennisCourtLineTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool autoTestOnStart = true;
    [SerializeField] private KeyCode testKey = KeyCode.L;
    [SerializeField] private KeyCode clearKey = KeyCode.K;

    private TennisCourtLineRenderer lineRenderer;

    void Start()
    {
        // 查找或创建线条渲染器
        lineRenderer = FindObjectOfType<TennisCourtLineRenderer>();

        if (lineRenderer == null)
        {
            // 创建新的线条渲染器
            GameObject lineRendererObj = new GameObject("TennisCourtLineRenderer");
            lineRenderer = lineRendererObj.AddComponent<TennisCourtLineRenderer>();
            Debug.Log("TennisCourtLineTest: 创建了新的TennisCourtLineRenderer");
        }

        if (autoTestOnStart)
        {
            // 延迟0.5秒执行，确保所有对象都已初始化
            Invoke(nameof(TestCreateLines), 0.5f);
        }
    }

    void Update()
    {
        // 快捷键控制
        if (Input.GetKeyDown(KeyCode.L))
        {
            CreateLines();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ClearLines();
        }

        // T键手动触发线条创建
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestLineCreation();
        }
    }

    /// <summary>
    /// 创建线条
    /// </summary>
    void CreateLines()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("❌ TennisCourtLineRenderer未找到！");
            return;
        }

        Debug.Log("🎨 开始创建网球场线条...");
        lineRenderer.CreateTennisCourtLines();
        Debug.Log("✅ 线条创建命令已发送");
    }

    /// <summary>
    /// 清除线条
    /// </summary>
    void ClearLines()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("❌ TennisCourtLineRenderer未找到！");
            return;
        }

        Debug.Log("🧹 清除网球场线条...");
        lineRenderer.ClearExistingLines();
        Debug.Log("✅ 线条清除完成");
    }

    /// <summary>
    /// 测试线条创建功能
    /// </summary>
    void TestLineCreation()
    {
        Debug.Log("🧪 手动测试线条创建...");

        // 查找TennisCourtLineRenderer
        TennisCourtLineRenderer testRenderer = FindObjectOfType<TennisCourtLineRenderer>();
        if (testRenderer == null)
        {
            Debug.LogError("❌ 未找到TennisCourtLineRenderer组件！");
            return;
        }

        Debug.Log("✅ 找到TennisCourtLineRenderer，开始创建线条...");
        testRenderer.CreateTennisCourtLines();
        Debug.Log("🎾 线条创建命令已发送");
    }

    /// <summary>
    /// 测试创建线条
    /// </summary>
    [ContextMenu("测试创建线条")]
    public void TestCreateLines()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("TennisCourtLineTest: 未找到TennisCourtLineRenderer组件！");
            return;
        }

        Debug.Log("TennisCourtLineTest: 开始创建网球场标识线...");
        lineRenderer.CreateTennisCourtLines();

        int lineCount = lineRenderer.GetLineCount();
        Debug.Log($"TennisCourtLineTest: 成功创建 {lineCount} 条线条");

        // 输出控制说明
        LogControlInstructions();
    }

    /// <summary>
    /// 测试清除线条
    /// </summary>
    [ContextMenu("测试清除线条")]
    public void TestClearLines()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("TennisCourtLineTest: 未找到TennisCourtLineRenderer组件！");
            return;
        }

        Debug.Log("TennisCourtLineTest: 清除所有线条...");
        lineRenderer.ClearExistingLines();
        Debug.Log("TennisCourtLineTest: 线条清除完成");
    }

    /// <summary>
    /// 输出控制说明
    /// </summary>
    private void LogControlInstructions()
    {
        Debug.Log("=== 网球场线条控制说明 ===");
        Debug.Log($"按 {testKey} 键: 创建/重新创建网球场线条");
        Debug.Log($"按 {clearKey} 键: 清除所有线条");
        Debug.Log("按 T 键: 手动触发线条创建");
        Debug.Log("右键点击TennisCourtLineRenderer组件可使用上下文菜单");
        Debug.Log("在Inspector中可以调整线条参数");
    }

    void OnGUI()
    {
        // 在屏幕上显示控制提示
        if (lineRenderer != null)
        {
            GUILayout.BeginArea(new Rect(10, Screen.height - 120, 300, 100));
            GUILayout.Label("网球场线条控制:");
            GUILayout.Label($"按 {testKey} 键: 创建线条");
            GUILayout.Label($"按 {clearKey} 键: 清除线条");
            GUILayout.Label("按 T 键: 手动触发线条创建");
            GUILayout.EndArea();
        }
    }
}