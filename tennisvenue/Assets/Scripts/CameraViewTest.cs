using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 摄像机视角功能测试脚本
/// 用于验证新增的后场视角和UI按钮切换功能
/// </summary>
public class CameraViewTest : MonoBehaviour
{
    [Header("测试配置")]
    public bool enableTestMode = true;
    public bool showDebugInfo = true;

    private CameraController cameraController;
    private Camera mainCamera;

    void Start()
    {
        if (!enableTestMode) return;

        Debug.Log("=== 摄像机视角功能测试启动 ===");

        // 查找摄像机控制器
        cameraController = FindObjectOfType<CameraController>();
        mainCamera = Camera.main;

        if (cameraController == null)
        {
            Debug.LogError("❌ 未找到CameraController组件");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("❌ 未找到主摄像机");
            return;
        }

        Debug.Log("✅ 摄像机控制器已找到");

        // 测试预设视角
        TestCameraPresets();

        // 测试UI按钮
        TestUIButton();

        Debug.Log("📋 测试快捷键:");
        Debug.Log("  F10: 测试所有视角");
        Debug.Log("  F11: 测试UI按钮功能");
        Debug.Log("  F12: 显示当前摄像机状态");
    }

    void Update()
    {
        if (!enableTestMode) return;

        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.F10))
        {
            TestAllViews();
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            TestUIButtonFunction();
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            ShowCameraStatus();
        }
    }

    /// <summary>
    /// 测试摄像机预设视角
    /// </summary>
    void TestCameraPresets()
    {
        Debug.Log("🔍 测试摄像机预设视角...");

        // 检查是否有6个预设视角（包括新增的后场视角）
        var presetsField = typeof(CameraController).GetField("cameraPresets",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (presetsField != null)
        {
            var presets = presetsField.GetValue(cameraController) as CameraController.CameraPreset[];
            if (presets != null)
            {
                Debug.Log($"✅ 找到 {presets.Length} 个预设视角");

                for (int i = 0; i < presets.Length; i++)
                {
                    Debug.Log($"  视角 {i}: {presets[i].name} - 位置: {presets[i].position}");
                }

                if (presets.Length >= 6)
                {
                    Debug.Log("✅ 后场视角已正确添加");
                }
                else
                {
                    Debug.LogWarning("⚠️ 预设视角数量不足，可能缺少后场视角");
                }
            }
        }
    }

    /// <summary>
    /// 测试UI按钮
    /// </summary>
    void TestUIButton()
    {
        Debug.Log("🔍 测试UI按钮...");

        // 查找视角切换按钮
        Button viewSwitchButton = GameObject.Find("ViewSwitchButton")?.GetComponent<Button>();

        if (viewSwitchButton != null)
        {
            Debug.Log("✅ 找到视角切换按钮");

            // 检查按钮文本
            Text buttonText = viewSwitchButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                Debug.Log($"✅ 按钮文本: {buttonText.text}");
            }
            else
            {
                Debug.LogWarning("⚠️ 按钮文本组件未找到");
            }

            // 检查按钮事件
            if (viewSwitchButton.onClick.GetPersistentEventCount() > 0)
            {
                Debug.Log("✅ 按钮事件已绑定");
            }
            else
            {
                Debug.LogWarning("⚠️ 按钮事件未绑定");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到视角切换按钮，可能需要自动创建");
        }
    }

    /// <summary>
    /// 测试所有视角切换
    /// </summary>
    void TestAllViews()
    {
        if (cameraController == null) return;

        Debug.Log("🔄 开始测试所有视角...");
        StartCoroutine(TestViewsSequence());
    }

    /// <summary>
    /// 视角测试序列
    /// </summary>
    System.Collections.IEnumerator TestViewsSequence()
    {
        string[] viewNames = { "默认视角", "俯视角度", "侧面视角", "近距离观察", "全景视角", "后场视角" };

        for (int i = 0; i < 6; i++)
        {
            Debug.Log($"🎥 切换到视角 {i}: {viewNames[i]}");
            cameraController.SetCameraPreset(i);

            // 显示当前摄像机状态
            ShowCameraStatus();

            yield return new WaitForSeconds(2f); // 等待2秒观察效果
        }

        Debug.Log("✅ 所有视角测试完成");
    }

    /// <summary>
    /// 测试UI按钮功能
    /// </summary>
    void TestUIButtonFunction()
    {
        if (cameraController == null) return;

        Debug.Log("🔄 测试UI按钮切换功能...");

        // 模拟按钮点击
        cameraController.ToggleSimpleView();

        ShowCameraStatus();
    }

    /// <summary>
    /// 显示当前摄像机状态
    /// </summary>
    void ShowCameraStatus()
    {
        if (mainCamera == null) return;

        Vector3 pos = mainCamera.transform.position;
        Vector3 rot = mainCamera.transform.eulerAngles;
        float fov = mainCamera.fieldOfView;

        Debug.Log($"📷 摄像机状态:");
        Debug.Log($"  位置: ({pos.x:F2}, {pos.y:F2}, {pos.z:F2})");
        Debug.Log($"  旋转: ({rot.x:F1}°, {rot.y:F1}°, {rot.z:F1}°)");
        Debug.Log($"  视野: {fov:F1}°");

        // 检查是否为后场视角
        if (Mathf.Approximately(pos.z, 4.5f) && Mathf.Approximately(rot.y, 180f))
        {
            Debug.Log("✅ 当前为后场视角");
        }
        else if (Mathf.Approximately(pos.z, -5f) && Mathf.Approximately(rot.y, 0f))
        {
            Debug.Log("✅ 当前为默认视角");
        }
    }

    void OnGUI()
    {
        if (!enableTestMode || !showDebugInfo) return;

        GUILayout.BeginArea(new Rect(Screen.width - 250, 100, 240, 200));
        GUILayout.Box("摄像机视角测试");

        if (GUILayout.Button("测试所有视角 (F10)"))
        {
            TestAllViews();
        }

        if (GUILayout.Button("测试UI按钮 (F11)"))
        {
            TestUIButtonFunction();
        }

        if (GUILayout.Button("显示状态 (F12)"))
        {
            ShowCameraStatus();
        }

        if (GUILayout.Button("切换到后场视角"))
        {
            cameraController?.SetCameraPreset(5);
        }

        if (GUILayout.Button("切换到默认视角"))
        {
            cameraController?.SetCameraPreset(0);
        }

        GUILayout.EndArea();
    }
}