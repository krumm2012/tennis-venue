using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 脚本编译诊断和修复工具
/// 解决"无法找到脚本类"的问题
/// </summary>
public class ScriptCompileFixer : MonoBehaviour
{
    [Header("诊断信息")]
    public bool showDebugInfo = true;

    void Start()
    {
        DiagnoseScriptIssues();
    }

    /// <summary>
    /// 诊断脚本问题
    /// </summary>
    [ContextMenu("Diagnose Script Issues")]
    public void DiagnoseScriptIssues()
    {
        Debug.Log("🔍 ===== Script Compilation Diagnosis =====");

        // 检查TennisVenueUIManager文件
        string scriptPath = "Assets/Scripts/TennisVenueUIManager.cs";
        if (File.Exists(scriptPath))
        {
            Debug.Log("✅ TennisVenueUIManager.cs file exists");

            // 读取文件内容检查
            string content = File.ReadAllText(scriptPath);
            if (content.Contains("public class TennisVenueUIManager"))
            {
                Debug.Log("✅ Class declaration found");
            }
            else
            {
                Debug.LogError("❌ Class declaration not found");
            }

            if (content.Contains("MonoBehaviour"))
            {
                Debug.Log("✅ MonoBehaviour inheritance found");
            }
            else
            {
                Debug.LogError("❌ MonoBehaviour inheritance missing");
            }
        }
        else
        {
            Debug.LogError("❌ TennisVenueUIManager.cs file not found");
        }

        // 检查编译状态
        CheckCompilationStatus();

        Debug.Log("==========================================");
    }

    /// <summary>
    /// 检查编译状态
    /// </summary>
    void CheckCompilationStatus()
    {
        Debug.Log("📊 Compilation Status Check:");

        // 尝试查找TennisVenueUIManager类型
        var type = System.Type.GetType("TennisVenueUIManager");
        if (type != null)
        {
            Debug.Log("✅ TennisVenueUIManager type found in current assembly");
        }
        else
        {
            Debug.LogWarning("⚠️ TennisVenueUIManager type not found - compilation issue");

            // 检查所有程序集
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var foundType = assembly.GetType("TennisVenueUIManager");
                if (foundType != null)
                {
                    Debug.Log($"✅ Found TennisVenueUIManager in assembly: {assembly.FullName}");
                    return;
                }
            }

            Debug.LogError("❌ TennisVenueUIManager not found in any assembly");
        }
    }

    /// <summary>
    /// 强制重新编译脚本
    /// </summary>
    [ContextMenu("Force Recompile Scripts")]
    public void ForceRecompileScripts()
    {
        Debug.Log("🔄 Forcing script recompilation...");

#if UNITY_EDITOR
        // 强制重新编译
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        Debug.Log("✅ Script recompilation requested");
#else
        Debug.LogWarning("Script recompilation only available in Editor");
#endif
    }

    /// <summary>
    /// 修复脚本问题
    /// </summary>
    [ContextMenu("Fix Script Issues")]
    public void FixScriptIssues()
    {
        Debug.Log("🔧 Attempting to fix script issues...");

        // 1. 强制重新编译
        ForceRecompileScripts();

        // 2. 清理并重新导入
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.ImportAsset("Assets/Scripts/TennisVenueUIManager.cs",
            UnityEditor.ImportAssetOptions.ForceUpdate);
        Debug.Log("✅ TennisVenueUIManager.cs reimported");
#endif

        // 3. 等待编译完成后再次检查
        StartCoroutine(DelayedCheck());
    }

    System.Collections.IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(2f);
        DiagnoseScriptIssues();
    }

    /// <summary>
    /// 创建测试GameObject并添加组件
    /// </summary>
    [ContextMenu("Test Add Component")]
    public void TestAddComponent()
    {
        Debug.Log("🧪 Testing component addition...");

        GameObject testObj = new GameObject("Test UI Manager");

        try
        {
            // 尝试通过类型添加组件
            var component = testObj.AddComponent<TennisVenueUIManager>();
            if (component != null)
            {
                Debug.Log("✅ Successfully added TennisVenueUIManager component by type");
                DestroyImmediate(testObj);
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to add component by type: {e.Message}");
        }

        try
        {
            // 尝试通过字符串添加组件
            var component = testObj.AddComponent<TennisVenueUIManager>();
            if (component != null)
            {
                Debug.Log("✅ Successfully added TennisVenueUIManager component by string");
                DestroyImmediate(testObj);
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to add component by string: {e.Message}");
        }

        DestroyImmediate(testObj);
        Debug.LogError("❌ Both methods failed - script compilation issue confirmed");
    }

    void Update()
    {
        // F9键：诊断脚本问题
        if (Input.GetKeyDown(KeyCode.F9))
        {
            DiagnoseScriptIssues();
        }

        // Shift+F9：修复脚本问题
        if (Input.GetKeyDown(KeyCode.F9) && Input.GetKey(KeyCode.LeftShift))
        {
            FixScriptIssues();
        }

        // Ctrl+F9：测试添加组件
        if (Input.GetKeyDown(KeyCode.F9) && Input.GetKey(KeyCode.LeftControl))
        {
            TestAddComponent();
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        GUI.color = new Color(1, 1, 1, 0.8f);
        GUI.Label(new Rect(10, Screen.height - 80, 400, 70),
                  "Script Compile Fixer:\nF9: Diagnose Issues\nShift+F9: Fix Issues\nCtrl+F9: Test Add Component");
    }
}