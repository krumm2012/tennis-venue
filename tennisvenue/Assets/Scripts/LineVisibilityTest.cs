using UnityEngine;

/// <summary>
/// 线条可见性测试脚本
/// 用于验证网球场线条的显示效果和位置
/// </summary>
public class LineVisibilityTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool autoTestOnStart = true;
    [SerializeField] private bool showLineInfo = true;

    void Start()
    {
        if (autoTestOnStart)
        {
            Invoke(nameof(TestLineVisibility), 1f);
        }
    }

    void Update()
    {
        // V键测试线条可见性
        if (Input.GetKeyDown(KeyCode.V))
        {
            TestLineVisibility();
        }

        // R键重新创建线条
        if (Input.GetKeyDown(KeyCode.R))
        {
            RecreateLines();
        }

        // Q键快速检查线条状态
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuickLineCheck();
        }
    }

    /// <summary>
    /// 测试线条可见性
    /// </summary>
    [ContextMenu("测试线条可见性")]
    public void TestLineVisibility()
    {
        Debug.Log("🔍 开始测试线条可见性...");

        // 查找所有网球场线条
        GameObject[] lineObjects = GameObject.FindGameObjectsWithTag("Untagged");
        int lineCount = 0;

        foreach (GameObject obj in lineObjects)
        {
            if (obj.name.StartsWith("TennisCourtLine_") || obj.name.StartsWith("HalfCourtLine_"))
            {
                lineCount++;
                if (showLineInfo)
                {
                    Vector3 pos = obj.transform.position;
                    Vector3 scale = obj.transform.lossyScale;
                    Renderer renderer = obj.GetComponent<Renderer>();
                    bool isVisible = renderer != null && renderer.isVisible;

                    Debug.Log($"线条: {obj.name}");
                    Debug.Log($"  位置: ({pos.x:F2}, {pos.y:F2}, {pos.z:F2})");
                    Debug.Log($"  尺寸: ({scale.x:F2}, {scale.y:F2}, {scale.z:F2})");
                    Debug.Log($"  可见: {isVisible}");
                    Debug.Log($"  材质: {(renderer?.material?.name ?? "无")}");
                }
            }
        }

        Debug.Log($"✅ 找到 {lineCount} 条网球场线条");

        // 检查Floor对象
        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Vector3 floorPos = floor.transform.position;
            Vector3 floorScale = floor.transform.localScale;
            Debug.Log($"Floor位置: ({floorPos.x:F2}, {floorPos.y:F2}, {floorPos.z:F2})");
            Debug.Log($"Floor尺寸: ({floorScale.x:F2}, {floorScale.y:F2}, {floorScale.z:F2})");
        }
    }

    /// <summary>
    /// 重新创建线条
    /// </summary>
    public void RecreateLines()
    {
        Debug.Log("🔄 重新创建网球场线条...");

        TennisCourtLineRenderer lineRenderer = FindObjectOfType<TennisCourtLineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.ClearExistingLines();
            lineRenderer.CreateHalfCourtLines();
            Debug.Log("✅ 半场线条重新创建完成");
        }
        else
        {
            Debug.LogError("❌ 未找到TennisCourtLineRenderer组件");
        }
    }

    /// <summary>
    /// 快速检查线条状态
    /// </summary>
    public void QuickLineCheck()
    {
        Debug.Log("🔍 快速线条状态检查...");

        // 检查TennisCourtLineRenderer组件
        TennisCourtLineRenderer lineRenderer = FindObjectOfType<TennisCourtLineRenderer>();
        if (lineRenderer != null)
        {
            int lineCount = lineRenderer.GetLineCount();
            Debug.Log($"✅ TennisCourtLineRenderer找到，当前线条数量: {lineCount}");

            if (lineCount >= 6 && lineCount <= 10)
            {
                Debug.Log("🎯 半场线条数量正确！建议按T键切换到俯视角度查看半场布局");
            }
            else
            {
                Debug.Log($"⚠️ 线条数量异常，预期6-10条，实际{lineCount}条");
            }
        }
        else
        {
            Debug.LogError("❌ 未找到TennisCourtLineRenderer组件");
        }

        // 检查摄像机位置
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 camPos = mainCamera.transform.position;
            Debug.Log($"📷 当前摄像机位置: ({camPos.x:F1}, {camPos.y:F1}, {camPos.z:F1})");

            if (camPos.y < 2f)
            {
                Debug.Log("💡 建议按T键切换到俯视角度，或按R键回到默认视角");
            }
        }

        // 检查Floor对象
        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Vector3 floorPos = floor.transform.position;
            Debug.Log($"🏟️ Floor位置: ({floorPos.x:F2}, {floorPos.y:F2}, {floorPos.z:F2})");
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 100));
        GUILayout.Label("线条可见性测试:");
        GUILayout.Label("按 V 键: 测试线条可见性");
        GUILayout.Label("按 R 键: 重新创建线条");
        GUILayout.Label("按 Q 键: 快速检查线条状态");
        GUILayout.EndArea();
    }
}