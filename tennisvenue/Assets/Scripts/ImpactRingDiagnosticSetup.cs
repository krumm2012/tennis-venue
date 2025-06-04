using UnityEngine;

/// <summary>
/// ImpactRingDiagnostic 自动设置脚本
/// 自动创建并启用ImpactRingDiagnostic诊断工具
/// </summary>
public class ImpactRingDiagnosticSetup : MonoBehaviour
{
    [Header("自动设置选项")]
    public bool autoSetupOnStart = true;
    public bool showSetupInstructions = true;

    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupImpactRingDiagnostic();
        }

        if (showSetupInstructions)
        {
            ShowUsageInstructions();
        }
    }

    /// <summary>
    /// 设置ImpactRingDiagnostic
    /// </summary>
    [ContextMenu("Setup Impact Ring Diagnostic")]
    public void SetupImpactRingDiagnostic()
    {
        Debug.Log("🔧 Setting up ImpactRingDiagnostic...");

        // 检查是否已存在
        ImpactRingDiagnostic existingDiagnostic = FindObjectOfType<ImpactRingDiagnostic>();
        if (existingDiagnostic != null)
        {
            Debug.Log("✅ ImpactRingDiagnostic already exists and is active");
            return;
        }

        // 查找或创建诊断对象
        GameObject diagnosticObj = GameObject.Find("ImpactRingDiagnostic");
        if (diagnosticObj == null)
        {
            diagnosticObj = new GameObject("ImpactRingDiagnostic");
            Debug.Log("📦 Created ImpactRingDiagnostic GameObject");
        }

        // 添加ImpactRingDiagnostic组件
        ImpactRingDiagnostic diagnostic = diagnosticObj.GetComponent<ImpactRingDiagnostic>();
        if (diagnostic == null)
        {
            diagnostic = diagnosticObj.AddComponent<ImpactRingDiagnostic>();
            Debug.Log("✅ ImpactRingDiagnostic component added");

            // 配置默认设置
            diagnostic.enableDetailedLogging = true;
            diagnostic.testRingSize = 1.0f;
            diagnostic.testRingHeight = 0.05f;
        }

        // 确保组件启用
        diagnostic.enabled = true;
        diagnosticObj.SetActive(true);

        Debug.Log("🎉 ImpactRingDiagnostic setup completed!");
        Debug.Log("   Use F9/F10/Shift+F9/Shift+F10 keys to run diagnostics");
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void ShowUsageInstructions()
    {
        Debug.Log("=== ImpactRingDiagnostic 使用说明 ===");
        Debug.Log("🔑 快捷键操作:");
        Debug.Log("   F9键: 运行完整圆环诊断");
        Debug.Log("   F10键: 强制创建可见测试圆环");
        Debug.Log("   Shift+F9: 创建简单测试圆环");
        Debug.Log("   Shift+F10: 检查材质和渲染状态");
        Debug.Log("");
        Debug.Log("🎯 诊断功能:");
        Debug.Log("   • 检查BounceImpactMarker系统状态");
        Debug.Log("   • 分析圆环创建失败原因");
        Debug.Log("   • 验证圆环可见性问题");
        Debug.Log("   • 测试强制圆环创建");
        Debug.Log("   • 检查材质和渲染设置");
        Debug.Log("");
        Debug.Log("💡 故障排除:");
        Debug.Log("   1. 按F9运行完整诊断");
        Debug.Log("   2. 如果没有圆环，按F10创建测试圆环");
        Debug.Log("   3. 检查BounceImpactMarker系统是否启用");
        Debug.Log("   4. 确保网球速度满足触发条件");
        Debug.Log("========================================");
    }

    /// <summary>
    /// 手动触发设置（用于测试）
    /// </summary>
    void Update()
    {
        // Ctrl+F9: 手动设置ImpactRingDiagnostic
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F9))
        {
            SetupImpactRingDiagnostic();
        }

        // Ctrl+Shift+F9: 显示使用说明
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F9))
        {
            ShowUsageInstructions();
        }
    }

    /// <summary>
    /// 在Scene视图中显示帮助信息
    /// </summary>
    void OnGUI()
    {
        if (!showSetupInstructions) return;

        GUI.color = new Color(1, 1, 1, 0.8f);
        GUI.Label(new Rect(10, 10, 500, 120),
                  "ImpactRingDiagnostic Setup:\n" +
                  "Ctrl+F9: Setup Diagnostic\n" +
                  "Ctrl+Shift+F9: Show Instructions\n" +
                  "\nAfter setup, use F9/F10 keys for diagnostics");
    }
}