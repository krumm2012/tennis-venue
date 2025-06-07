using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 简化版网球场UI管理器
/// 提供基本的UI按钮控制，避免复杂的反射调用
/// </summary>
public class SimpleTennisUI : MonoBehaviour
{
    [Header("UI配置")]
    public bool autoCreateUI = true;
    public bool showDebugInfo = true;

    [Header("系统引用")]
    public BallLauncher ballLauncher;
    public CameraController cameraController;

    private Canvas uiCanvas;
    private bool uiCreated = false;

    void Start()
    {
        Debug.Log("🎮 Simple Tennis UI Started");

        if (autoCreateUI)
        {
            CreateSimpleUI();
        }
    }

    void Update()
    {
        // 快捷键控制
        if (Input.GetKeyDown(KeyCode.F10))
        {
            CreateSimpleUI();
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            TestUIFunctions();
        }
    }

    /// <summary>
    /// 创建简单UI界面
    /// </summary>
    void CreateSimpleUI()
    {
        if (uiCreated)
        {
            Debug.Log("⚠️ UI already created");
            return;
        }

        Debug.Log("🔧 Creating Simple Tennis UI...");

        // 创建Canvas
        CreateCanvas();

        // 创建控制面板
        CreateControlPanel();

        // 查找系统组件
        FindSystemComponents();

        uiCreated = true;
        Debug.Log("✅ Simple Tennis UI created successfully");
    }

    /// <summary>
    /// 创建Canvas
    /// </summary>
    void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Simple Tennis UI Canvas");
        uiCanvas = canvasObj.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.sortingOrder = 100;

        // 添加CanvasScaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 添加GraphicRaycaster
        canvasObj.AddComponent<GraphicRaycaster>();

        Debug.Log("📱 UI Canvas created");
    }

    /// <summary>
    /// 创建控制面板
    /// </summary>
    void CreateControlPanel()
    {
        // 创建主面板
        GameObject panel = CreatePanel("Control Panel", new Vector2(10, -10), new Vector2(300, 400));

        // 添加标题
        CreateLabel(panel, "Tennis Venue Controls", new Vector2(0, -30), 16);

        // 基本控制按钮
        CreateButton(panel, "Launch Ball", new Vector2(0, -80), LaunchBall);
        CreateButton(panel, "Reset Game", new Vector2(0, -120), ResetGame);
        CreateButton(panel, "Clear Balls", new Vector2(0, -160), ClearBalls);

        // 视角控制按钮
        CreateLabel(panel, "Camera Views", new Vector2(0, -210), 14);
        CreateButton(panel, "Default View (R)", new Vector2(0, -250), () => SwitchCamera(0));
        CreateButton(panel, "Top View (T)", new Vector2(0, -290), () => SwitchCamera(1));
        CreateButton(panel, "Side View (F)", new Vector2(0, -330), () => SwitchCamera(2));
        CreateButton(panel, "Close View (C)", new Vector2(0, -370), () => SwitchCamera(3));

        Debug.Log("🎛️ Control panel created");
    }

    /// <summary>
    /// 创建面板
    /// </summary>
    GameObject CreatePanel(string name, Vector2 position, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(uiCanvas.transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);

        return panel;
    }

    /// <summary>
    /// 创建按钮
    /// </summary>
    Button CreateButton(GameObject parent, string text, Vector2 position, System.Action onClick)
    {
        GameObject buttonObj = new GameObject($"Button_{text}");
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(250, 30);

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.6f, 1f, 0.8f);

        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick?.Invoke());

        // 添加文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 12;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;

        return button;
    }

    /// <summary>
    /// 创建标签
    /// </summary>
    void CreateLabel(GameObject parent, string text, Vector2 position, int fontSize)
    {
        GameObject labelObj = new GameObject($"Label_{text}");
        labelObj.transform.SetParent(parent.transform, false);

        RectTransform rect = labelObj.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(250, 25);

        Text textComponent = labelObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.color = Color.yellow;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.fontStyle = FontStyle.Bold;
    }

    /// <summary>
    /// 查找系统组件
    /// </summary>
    void FindSystemComponents()
    {
        if (ballLauncher == null)
            ballLauncher = FindObjectOfType<BallLauncher>();

        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>();

        Debug.Log($"🔍 Components found - BallLauncher: {(ballLauncher != null ? "✅" : "❌")}, CameraController: {(cameraController != null ? "✅" : "❌")}");
    }

    /// <summary>
    /// 发射球
    /// </summary>
    void LaunchBall()
    {
        if (ballLauncher != null)
        {
            // 直接调用LaunchBall方法，而不是模拟Update
            ballLauncher.LaunchBall(Vector3.zero);
            Debug.Log("🚀 Ball launched via SimpleTennisUI button");
        }
        else
        {
            Debug.LogWarning("❌ BallLauncher not found");
        }
    }

    /// <summary>
    /// 重置游戏
    /// </summary>
    void ResetGame()
    {
        ClearBalls();

        if (cameraController != null)
        {
            cameraController.SetCameraPreset(0); // 默认视角
        }

        Debug.Log("🔄 Game reset via UI button");
    }

    /// <summary>
    /// 清除所有球 - 修复版，避免误删重要组件
    /// </summary>
    void ClearBalls()
    {
        List<GameObject> ballsToDestroy = new List<GameObject>();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // 使用安全的网球识别方法
            if (IsSafeTennisBall(obj))
            {
                ballsToDestroy.Add(obj);
            }
        }

        foreach (GameObject ball in ballsToDestroy)
        {
            Debug.Log($"🧹 安全清除网球: {ball.name}");
            Destroy(ball);
        }

        Debug.Log($"🧹 Safely cleared {ballsToDestroy.Count} tennis balls via UI button");
    }

    /// <summary>
    /// 判断对象是否是安全的网球(可以被清除)
    /// </summary>
    bool IsSafeTennisBall(GameObject obj)
    {
        if (obj == null) return false;

        string name = obj.name;

        // 只匹配明确的网球命名模式
        bool isNameMatch = name.StartsWith("TennisBall") ||
                          name.StartsWith("Tennis Ball") ||
                          name.Contains("TennisBall_") ||
                          name.Contains("QuickTest") ||
                          name.Contains("SimpleTest");

        if (!isNameMatch) return false;

        // 确保有物理组件
        bool hasPhysics = obj.GetComponent<Rigidbody>() != null && obj.GetComponent<Collider>() != null;
        if (!hasPhysics) return false;

        // 排除包含重要组件的对象
        bool hasCriticalComponents = obj.GetComponent<BallLauncher>() != null ||
                                   obj.GetComponent<Camera>() != null ||
                                   obj.GetComponent<Canvas>() != null ||
                                   obj.GetComponent<CameraController>() != null;

        return !hasCriticalComponents;
    }

    /// <summary>
    /// 切换摄像机视角
    /// </summary>
    void SwitchCamera(int presetIndex)
    {
        if (cameraController != null)
        {
            cameraController.SetCameraPreset(presetIndex);
            Debug.Log($"📷 Switched to camera preset {presetIndex} via UI button");
        }
        else
        {
            Debug.LogWarning("❌ CameraController not found");
        }
    }

    /// <summary>
    /// 测试UI功能
    /// </summary>
    void TestUIFunctions()
    {
        Debug.Log("🧪 Testing Simple Tennis UI Functions...");

        FindSystemComponents();

        Debug.Log($"   UI Created: {uiCreated}");
        Debug.Log($"   Canvas: {(uiCanvas != null ? "✅" : "❌")}");
        Debug.Log($"   Ball Launcher: {(ballLauncher != null ? "✅" : "❌")}");
        Debug.Log($"   Camera Controller: {(cameraController != null ? "✅" : "❌")}");

        Debug.Log("✅ Simple Tennis UI test completed");
    }

    /// <summary>
    /// GUI显示调试信息
    /// </summary>
    void OnGUI()
    {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(Screen.width - 250, Screen.height - 150, 240, 140));

        GUILayout.Label("=== Simple Tennis UI ===", new GUIStyle() { fontSize = 12, normal = { textColor = Color.cyan } });

        GUILayout.Label($"UI Created: {(uiCreated ? "✅" : "❌")}",
            new GUIStyle() { normal = { textColor = uiCreated ? Color.green : Color.red } });

        GUILayout.Label($"Ball Launcher: {(ballLauncher != null ? "✅" : "❌")}",
            new GUIStyle() { normal = { textColor = ballLauncher != null ? Color.green : Color.red } });

        GUILayout.Label($"Camera Controller: {(cameraController != null ? "✅" : "❌")}",
            new GUIStyle() { normal = { textColor = cameraController != null ? Color.green : Color.red } });

        GUILayout.Space(5);
        GUILayout.Label("F10: Create UI", new GUIStyle() { fontSize = 10, normal = { textColor = Color.white } });
        GUILayout.Label("F11: Test Functions", new GUIStyle() { fontSize = 10, normal = { textColor = Color.white } });

        GUILayout.EndArea();
    }
}