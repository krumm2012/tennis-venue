using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Clear Balls Bug Fixer - 修复清除网球功能影响发球机的问题
/// </summary>
public class ClearBallsBugFixer : MonoBehaviour
{
    [Header("诊断设置")]
    public bool enableDetailedLogs = true;
    public bool enableAutoFix = true;

    [Header("系统引用")]
    public BallLauncher ballLauncher;
    public TennisVenueUIManager uiManager;

    void Start()
    {
        Debug.Log("=== Clear Balls Bug Fixer 已启动 ===");
        Debug.Log("🔧 将监控和修复清除网球功能的问题");
        Debug.Log("⌨️ 快捷键:");
        Debug.Log("   F8: 手动诊断清除网球问题");
        Debug.Log("   Ctrl+F8: 修复发球机状态");
        Debug.Log("   Shift+F8: 安全清除所有网球");

        if (enableAutoFix)
        {
            // 自动查找组件
            FindSystemComponents();

            // 验证发球机状态
            VerifyBallLauncherState();
        }
    }

    void Update()
    {
        // 手动诊断
        if (Input.GetKeyDown(KeyCode.F8))
        {
            DiagnoseClearBallsIssue();
        }

        // 修复发球机状态
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F8))
        {
            FixBallLauncherState();
        }

        // 安全清除网球
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F8))
        {
            SafeClearAllBalls();
        }
    }

    /// <summary>
    /// 查找系统组件
    /// </summary>
    void FindSystemComponents()
    {
        if (ballLauncher == null)
        {
            ballLauncher = FindObjectOfType<BallLauncher>();
            if (ballLauncher != null)
            {
                Debug.Log($"✅ 找到BallLauncher: {ballLauncher.gameObject.name}");
            }
            else
            {
                Debug.LogError("❌ 未找到BallLauncher组件！");
            }
        }

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<TennisVenueUIManager>();
            if (uiManager != null)
            {
                Debug.Log($"✅ 找到TennisVenueUIManager: {uiManager.gameObject.name}");
            }
        }
    }

    /// <summary>
    /// 诊断清除网球问题
    /// </summary>
    void DiagnoseClearBallsIssue()
    {
        Debug.Log("=== 诊断 Clear Balls 功能问题 ===");

        // 检查发球机状态
        VerifyBallLauncherState();

        // 检查所有可能被误删的对象
        CheckPotentiallyAffectedObjects();

        // 检查UI清除方法
        AnalyzeClearBallsMethods();

        Debug.Log("=== 诊断完成 ===");
    }

    /// <summary>
    /// 验证发球机状态
    /// </summary>
    void VerifyBallLauncherState()
    {
        Debug.Log("--- 验证发球机状态 ---");

        if (ballLauncher == null)
        {
            Debug.LogError("❌ BallLauncher组件丢失！");

            // 尝试重新查找
            ballLauncher = FindObjectOfType<BallLauncher>();
            if (ballLauncher != null)
            {
                Debug.Log("✅ 重新找到BallLauncher");
            }
            else
            {
                Debug.LogError("💥 BallLauncher完全丢失，可能被清除功能误删！");
                return;
            }
        }

        // 检查发球机的关键组件
        bool allGood = true;

        if (ballLauncher.ballPrefab == null)
        {
            Debug.LogError("❌ ballPrefab引用丢失");
            allGood = false;
        }

        if (ballLauncher.launchPoint == null)
        {
            Debug.LogError("❌ launchPoint引用丢失");
            allGood = false;
        }

        if (ballLauncher.mainCamera == null)
        {
            Debug.LogWarning("⚠️ mainCamera引用丢失，将尝试自动修复");
            ballLauncher.mainCamera = Camera.main;
            if (ballLauncher.mainCamera != null)
            {
                Debug.Log("✅ mainCamera已自动修复");
            }
        }

        // 检查UI引用
        if (ballLauncher.angleSlider == null)
        {
            Debug.LogWarning("⚠️ angleSlider引用丢失");
            allGood = false;
        }

        if (ballLauncher.speedSlider == null)
        {
            Debug.LogWarning("⚠️ speedSlider引用丢失");
            allGood = false;
        }

        if (allGood)
        {
            Debug.Log("✅ BallLauncher状态正常");
        }
        else
        {
            Debug.LogWarning("⚠️ BallLauncher存在问题，需要修复");
        }
    }

    /// <summary>
    /// 检查可能被影响的对象
    /// </summary>
    void CheckPotentiallyAffectedObjects()
    {
        Debug.Log("--- 检查可能被清除功能影响的对象 ---");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> suspiciousObjects = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            string name = obj.name.ToLower();

            // 检查名称中包含"ball"但不是网球的对象
            if (name.Contains("ball") && !name.Contains("tennisball"))
            {
                // 这些对象可能被误删
                if (obj.GetComponent<BallLauncher>() != null ||
                    obj.GetComponent<Camera>() != null ||
                    obj.GetComponent<Canvas>() != null)
                {
                    suspiciousObjects.Add(obj);
                    Debug.LogWarning($"⚠️ 可疑对象: {obj.name} (包含'ball'但不是网球)");
                }
            }
        }

        if (suspiciousObjects.Count == 0)
        {
            Debug.Log("✅ 未发现可疑对象");
        }
        else
        {
            Debug.LogWarning($"⚠️ 发现 {suspiciousObjects.Count} 个可疑对象");
        }
    }

    /// <summary>
    /// 分析清除网球方法
    /// </summary>
    void AnalyzeClearBallsMethods()
    {
        Debug.Log("--- 分析清除网球方法 ---");

        if (uiManager != null)
        {
            Debug.Log("✅ TennisVenueUIManager存在");
            Debug.Log("   其ClearAllBalls方法使用以下条件:");
            Debug.Log("   - 对象名包含'TennisBall', 'Tennis Ball', 或 'Ball'");
            Debug.Log("   - 对象有Rigidbody或Collider组件");
            Debug.Log("   ⚠️ 问题: 包含'Ball'的条件可能太宽泛");
        }

        // 建议更安全的清除方法
        Debug.Log("💡 建议的安全清除方法:");
        Debug.Log("   1. 只匹配'TennisBall'开头的对象");
        Debug.Log("   2. 排除包含重要组件的对象");
        Debug.Log("   3. 使用白名单而不是黑名单");
    }

    /// <summary>
    /// 修复发球机状态
    /// </summary>
    void FixBallLauncherState()
    {
        Debug.Log("=== 修复发球机状态 ===");

        FindSystemComponents();

        if (ballLauncher == null)
        {
            Debug.LogError("❌ 无法修复：BallLauncher不存在");
            Debug.Log("💡 解决方案：重新启动场景或重新创建BallLauncher");
            return;
        }

        // 修复摄像机引用
        if (ballLauncher.mainCamera == null)
        {
            ballLauncher.mainCamera = Camera.main;
            Debug.Log("✅ 修复了mainCamera引用");
        }

        // 修复发射点引用
        if (ballLauncher.launchPoint == null)
        {
            ballLauncher.launchPoint = ballLauncher.transform;
            Debug.Log("✅ 修复了launchPoint引用");
        }

        // 尝试重新查找UI组件
        if (ballLauncher.angleSlider == null)
        {
            Slider angleSlider = GameObject.Find("AngleSlider")?.GetComponent<Slider>();
            if (angleSlider != null)
            {
                // 使用反射设置私有字段
                var field = typeof(BallLauncher).GetField("angleSlider",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(ballLauncher, angleSlider);
                    Debug.Log("✅ 修复了angleSlider引用");
                }
            }
        }

        Debug.Log("🔧 发球机状态修复完成");
    }

    /// <summary>
    /// 安全清除所有网球
    /// </summary>
    void SafeClearAllBalls()
    {
        Debug.Log("=== 安全清除所有网球 ===");

        List<GameObject> ballsToDestroy = new List<GameObject>();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // 只清除明确的网球对象
            if (IsSafeTennisBall(obj))
            {
                ballsToDestroy.Add(obj);
            }
        }

        // 销毁网球
        foreach (GameObject ball in ballsToDestroy)
        {
            if (ball != null)
            {
                Debug.Log($"🧹 安全清除: {ball.name}");
                Destroy(ball);
            }
        }

        Debug.Log($"✅ 安全清除了 {ballsToDestroy.Count} 个网球");
    }

    /// <summary>
    /// 判断对象是否是安全的网球(可以被清除)
    /// </summary>
    bool IsSafeTennisBall(GameObject obj)
    {
        string name = obj.name;

        // 只匹配明确的网球命名模式
        if (name.StartsWith("TennisBall") ||
            name.StartsWith("Tennis Ball") ||
            name.Contains("TennisBall_") ||
            name.Contains("QuickTest") ||
            name.Contains("SimpleTest"))
        {
            // 确保有物理组件
            if (obj.GetComponent<Rigidbody>() != null && obj.GetComponent<Collider>() != null)
            {
                // 排除重要组件
                if (obj.GetComponent<BallLauncher>() == null &&
                    obj.GetComponent<Camera>() == null &&
                    obj.GetComponent<Canvas>() == null &&
                    obj.GetComponent<TennisVenueUIManager>() == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 替换UI管理器的清除方法
    /// </summary>
    public void ReplaceClearBallsMethod()
    {
        if (uiManager != null)
        {
            Debug.Log("🔧 使用安全的清除方法替代原有方法");
            SafeClearAllBalls();
        }
    }

    void OnGUI()
    {
        if (!enableDetailedLogs) return;

        GUILayout.BeginArea(new Rect(Screen.width - 300, 10, 290, 200));
        GUILayout.BeginVertical("box");

        GUILayout.Label("Clear Balls Bug Fixer", new GUIStyle("label") { fontStyle = FontStyle.Bold });

        // 状态显示
        if (ballLauncher != null)
        {
            GUI.color = Color.green;
            GUILayout.Label("✅ BallLauncher: OK");
        }
        else
        {
            GUI.color = Color.red;
            GUILayout.Label("❌ BallLauncher: Missing");
        }

        GUI.color = Color.white;
        GUILayout.Label($"网球数量: {CountTennisBalls()}");

        // 控制按钮
        if (GUILayout.Button("F8: 诊断问题"))
        {
            DiagnoseClearBallsIssue();
        }

        if (GUILayout.Button("Ctrl+F8: 修复发球机"))
        {
            FixBallLauncherState();
        }

        if (GUILayout.Button("Shift+F8: 安全清除"))
        {
            SafeClearAllBalls();
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /// <summary>
    /// 统计网球数量
    /// </summary>
    int CountTennisBalls()
    {
        int count = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (IsSafeTennisBall(obj))
            {
                count++;
            }
        }

        return count;
    }
}