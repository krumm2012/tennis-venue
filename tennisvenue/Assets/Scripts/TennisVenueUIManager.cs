using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Tennis Venue UI管理器 - 增强版
/// 统一管理所有UI界面，提供完整的用户交互体验
/// </summary>
public class TennisVenueUIManager : MonoBehaviour
{
    [Header("UI面板引用")]
    public Canvas mainCanvas;
    public GameObject controlPanelPrefab;

    [Header("UI组件")]
    public Button launchButton;
    public Button resetButton;
    public Button clearBallsButton;

    [Header("视角控制组")]
    public Button defaultViewButton;
    public Button backViewButton;
    public Button topViewButton;
    public Button sideViewButton;
    public Button closeViewButton;
    public Button panoramaViewButton;

    [Header("功能控制组")]
    public Button swingTestButton;
    public Button heightAnalysisButton;
    public Button airResistanceButton;
    public Button landingPointButton;
    public Button impactMarkerButton;
    public Button trajectoryDragButton;

    [Header("调试控制组")]
    public Button systemStatusButton;
    public Button clearHistoryButton;
    public Button toggleMarkersButton;
    public Button createTestBallButton;
    public Button diagnosticsButton;

    [Header("新增功能组")]
    public Button autoPlayButton;
    public Button settingsButton;
    public Button helpButton;
    public Button fullscreenButton;

    [Header("系统引用")]
    public BallLauncher ballLauncher;
    public CameraController cameraController;
    public FlightTimeTracker flightTimeTracker;
    public LandingPointTracker landingPointTracker;
    public BounceImpactMarker bounceImpactMarker;
    public TrajectoryDragController trajectoryDragController;
    public AirResistanceSystem airResistanceSystem;

    // UI面板组织
    private GameObject controlPanel;
    private GameObject viewControlPanel;
    private GameObject functionPanel;
    private GameObject debugPanel;
    private GameObject settingsPanel;
    private GameObject helpPanel;

    // 状态管理
    private bool isUIInitialized = false;
    private Dictionary<string, bool> featureStates = new Dictionary<string, bool>();
    private bool isAutoPlayMode = false;
    private bool isSettingsPanelOpen = false;
    private bool isHelpPanelOpen = false;

    // 自动播放相关
    private Coroutine autoPlayCoroutine;
    private float autoPlayInterval = 3f;

    void Start()
    {
        InitializeUI();
        FindSystemComponents();
        SetupButtonEvents();
        UpdateButtonStates();
        StartCoroutine(DelayedUISetup());
    }

    /// <summary>
    /// 延迟UI设置，确保所有组件都已加载
    /// </summary>
    IEnumerator DelayedUISetup()
    {
        yield return new WaitForSeconds(0.5f);

        // 检查并修复UI组件
        CheckAndFixUIComponents();

        // 显示欢迎信息
        ShowWelcomeMessage();
    }

    /// <summary>
    /// 检查并修复UI组件
    /// </summary>
    void CheckAndFixUIComponents()
    {
        // 检查现有的滑块是否正常工作
        Slider[] sliders = FindObjectsOfType<Slider>();
        Debug.Log($"🔍 Found {sliders.Length} sliders in scene");

        foreach (Slider slider in sliders)
        {
            if (slider.name.Contains("Direction") && !slider.interactable)
            {
                slider.interactable = true;
                Debug.Log($"✅ Fixed DirectionSlider interactability");
            }
        }

        // 检查ViewSwitchButton
        GameObject viewButton = GameObject.Find("ViewSwitchButton");
        if (viewButton != null)
        {
            Button btn = viewButton.GetComponent<Button>();
            if (btn != null && btn.onClick.GetPersistentEventCount() == 0)
            {
                btn.onClick.AddListener(() => SwitchView("toggle"));
                Debug.Log("✅ Fixed ViewSwitchButton event");
            }
        }
    }

    /// <summary>
    /// 显示欢迎信息
    /// </summary>
    void ShowWelcomeMessage()
    {
        Debug.Log("🎾 ===== Tennis Venue UI Manager 已启动 =====");
        Debug.Log("🎮 UI功能:");
        Debug.Log("   📱 分组控制面板 - 四个功能区域");
        Debug.Log("   🎯 智能状态管理 - 实时功能切换");
        Debug.Log("   ⌨️ 快捷键兼容 - 保持所有原有快捷键");
        Debug.Log("   🔧 自动修复 - 智能检测和修复UI问题");
        Debug.Log("   📊 实时监控 - 系统状态实时显示");
        Debug.Log("🔑 新增快捷键:");
        Debug.Log("   F1: 切换自动播放模式");
        Debug.Log("   F2: 打开/关闭设置面板");
        Debug.Log("   F3: 打开/关闭帮助面板");
        Debug.Log("   ESC: 关闭所有弹出面板");
        Debug.Log("==========================================");
    }

    /// <summary>
    /// 初始化UI界面
    /// </summary>
    void InitializeUI()
    {
        // 查找或创建主Canvas
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                CreateMainCanvas();
            }
        }

        // 创建UI面板
        CreateControlPanels();

        Debug.Log("✅ Tennis Venue UI Manager initialized");
        isUIInitialized = true;
    }

    /// <summary>
    /// 创建主Canvas
    /// </summary>
    void CreateMainCanvas()
    {
        GameObject canvasObj = new GameObject("Main Canvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 100;

        // 添加CanvasScaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // 添加GraphicRaycaster
        canvasObj.AddComponent<GraphicRaycaster>();

        Debug.Log("✅ Main Canvas created");
    }

    /// <summary>
    /// 创建控制面板
    /// </summary>
    void CreateControlPanels()
    {
        // 主控制面板 - 左上角
        controlPanel = CreatePanel("Control Panel", new Vector2(-200, 200), new Vector2(180, 320));
        CreateBasicControls(controlPanel);

        // 视角控制面板 - 右上角
        viewControlPanel = CreatePanel("View Control Panel", new Vector2(200, 200), new Vector2(180, 280));
        CreateViewControls(viewControlPanel);

        // 功能面板 - 左下角
        functionPanel = CreatePanel("Function Panel", new Vector2(-200, -150), new Vector2(180, 220));
        CreateFunctionControls(functionPanel);

        // 调试面板 - 右下角
        debugPanel = CreatePanel("Debug Panel", new Vector2(200, -150), new Vector2(180, 220));
        CreateDebugControls(debugPanel);

        // 设置面板 - 中央（初始隐藏）
        settingsPanel = CreatePanel("Settings Panel", new Vector2(0, 0), new Vector2(300, 400));
        CreateSettingsControls(settingsPanel);
        settingsPanel.SetActive(false);

        // 帮助面板 - 中央（初始隐藏）
        helpPanel = CreatePanel("Help Panel", new Vector2(0, 0), new Vector2(400, 500));
        CreateHelpControls(helpPanel);
        helpPanel.SetActive(false);
    }

    /// <summary>
    /// 创建UI面板
    /// </summary>
    GameObject CreatePanel(string name, Vector2 position, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(mainCanvas.transform, false);

        // 添加RectTransform
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // 添加背景
        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        // 添加边框效果
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.3f, 0.6f, 1f, 0.8f);
        outline.effectDistance = new Vector2(2, 2);

        // 添加标题
        CreatePanelTitle(panel, name);

        return panel;
    }

    /// <summary>
    /// 创建面板标题
    /// </summary>
    void CreatePanelTitle(GameObject panel, string title)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, panel.GetComponent<RectTransform>().sizeDelta.y / 2 - 20);
        titleRect.sizeDelta = new Vector2(160, 30);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = title;
        titleText.fontSize = 14;
        titleText.color = Color.yellow;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
    }

    /// <summary>
    /// 创建基本控制按钮
    /// </summary>
    void CreateBasicControls(GameObject parent)
    {
        float startY = 120;
        float spacing = 35;

        launchButton = CreateButton(parent, "🚀 Launch Ball", new Vector2(0, startY), LaunchBall);
        resetButton = CreateButton(parent, "🔄 Reset Game", new Vector2(0, startY - spacing), ResetGame);
        clearBallsButton = CreateButton(parent, "🧹 Clear Balls", new Vector2(0, startY - spacing * 2), ClearAllBalls);
        autoPlayButton = CreateButton(parent, "⏯️ Auto Play", new Vector2(0, startY - spacing * 3), ToggleAutoPlay);

        // 添加快捷键提示
        CreateKeyHint(parent, "Space / Click / F1", new Vector2(0, startY - spacing * 4 - 10));
    }

    /// <summary>
    /// 创建视角控制按钮
    /// </summary>
    void CreateViewControls(GameObject parent)
    {
        float startY = 100;
        float spacing = 30;

        defaultViewButton = CreateButton(parent, "📷 Default (R)", new Vector2(0, startY), () => SwitchView("default"));
        backViewButton = CreateButton(parent, "🔙 Back (B)", new Vector2(0, startY - spacing), () => SwitchView("back"));
        topViewButton = CreateButton(parent, "⬆️ Top (T)", new Vector2(0, startY - spacing * 2), () => SwitchView("top"));
        sideViewButton = CreateButton(parent, "↔️ Side (F)", new Vector2(0, startY - spacing * 3), () => SwitchView("side"));
        closeViewButton = CreateButton(parent, "🔍 Close (C)", new Vector2(0, startY - spacing * 4), () => SwitchView("close"));
        panoramaViewButton = CreateButton(parent, "🌐 Panorama (V)", new Vector2(0, startY - spacing * 5), () => SwitchView("panorama"));

        // 添加快捷键提示
        CreateKeyHint(parent, "R/T/F/C/V/B Keys", new Vector2(0, startY - spacing * 6 - 10));
    }

    /// <summary>
    /// 创建功能控制按钮
    /// </summary>
    void CreateFunctionControls(GameObject parent)
    {
        float startY = 80;
        float spacing = 30;

        swingTestButton = CreateButton(parent, "🎾 Swing Test (P)", new Vector2(0, startY), TriggerSwingTest);
        heightAnalysisButton = CreateButton(parent, "📊 Height Analysis (H)", new Vector2(0, startY - spacing), ShowHeightAnalysis);
        airResistanceButton = CreateButton(parent, "🌪️ Air Resistance (U)", new Vector2(0, startY - spacing * 2), ToggleAirResistance);
        landingPointButton = CreateButton(parent, "🎯 Landing Point (L)", new Vector2(0, startY - spacing * 3), ToggleLandingPoint);
        impactMarkerButton = CreateButton(parent, "💥 Impact Marker (M)", new Vector2(0, startY - spacing * 4), ToggleImpactMarker);

        // 添加快捷键提示
        CreateKeyHint(parent, "P/H/U/L/M Keys", new Vector2(0, startY - spacing * 5 - 10));
    }

    /// <summary>
    /// 创建调试控制按钮
    /// </summary>
    void CreateDebugControls(GameObject parent)
    {
        float startY = 80;
        float spacing = 30;

        systemStatusButton = CreateButton(parent, "📋 System Status (I)", new Vector2(0, startY), ShowSystemStatus);
        clearHistoryButton = CreateButton(parent, "🗑️ Clear History", new Vector2(0, startY - spacing), ClearHistory);
        toggleMarkersButton = CreateButton(parent, "🏷️ Toggle Markers", new Vector2(0, startY - spacing * 2), ToggleMarkers);
        createTestBallButton = CreateButton(parent, "⚽ Test Ball", new Vector2(0, startY - spacing * 3), CreateTestBall);
        diagnosticsButton = CreateButton(parent, "🔧 Diagnostics", new Vector2(0, startY - spacing * 4), RunDiagnostics);

        // 添加快捷键提示
        CreateKeyHint(parent, "I/F3/F4/F5 Keys", new Vector2(0, startY - spacing * 5 - 10));
    }

    /// <summary>
    /// 创建设置控制面板
    /// </summary>
    void CreateSettingsControls(GameObject parent)
    {
        float startY = 150;
        float spacing = 40;

        // 标题
        CreateLabel(parent, "⚙️ 游戏设置", new Vector2(0, startY), 18, Color.cyan);

        // 自动播放间隔设置
        CreateLabel(parent, "自动播放间隔 (秒):", new Vector2(0, startY - spacing), 12, Color.white);
        CreateSlider(parent, "AutoPlayInterval", new Vector2(0, startY - spacing - 25), 1f, 10f, autoPlayInterval, OnAutoPlayIntervalChanged);

        // 功能开关
        CreateToggle(parent, "启用轨迹拖动", new Vector2(0, startY - spacing * 2), true, OnTrajectoryDragToggle);
        CreateToggle(parent, "显示调试信息", new Vector2(0, startY - spacing * 2.5f), true, OnDebugInfoToggle);
        CreateToggle(parent, "自动清理网球", new Vector2(0, startY - spacing * 3), false, OnAutoClearToggle);

        // 关闭按钮
        CreateButton(parent, "❌ 关闭设置", new Vector2(0, startY - spacing * 4), CloseSettingsPanel);
    }

    /// <summary>
    /// 创建帮助控制面板
    /// </summary>
    void CreateHelpControls(GameObject parent)
    {
        float startY = 200;
        float spacing = 25;

        // 标题
        CreateLabel(parent, "❓ 使用帮助", new Vector2(0, startY), 18, Color.cyan);

        // 帮助内容
        string[] helpTexts = {
            "🎮 基本操作:",
            "• 空格键或鼠标左键发射网球",
            "• 拖动滑块调节发球参数",
            "• 拖动轨迹线调节落点",
            "",
            "📷 视角控制:",
            "• R/T/F/C/V/B键切换预设视角",
            "• WASD移动摄像机",
            "• 鼠标右键拖拽旋转视角",
            "",
            "🔧 功能快捷键:",
            "• P键测试挥拍动作",
            "• H键显示高度分析",
            "• U键切换空气阻力",
            "• L键切换落点追踪",
            "• M键切换冲击标记",
            "• I键显示系统状态"
        };

        for (int i = 0; i < helpTexts.Length; i++)
        {
            Color textColor = helpTexts[i].StartsWith("🎮") || helpTexts[i].StartsWith("📷") || helpTexts[i].StartsWith("🔧")
                ? Color.yellow : Color.white;
            CreateLabel(parent, helpTexts[i], new Vector2(0, startY - spacing * (i + 1)), 10, textColor);
        }

        // 关闭按钮
        CreateButton(parent, "❌ 关闭帮助", new Vector2(0, startY - spacing * (helpTexts.Length + 2)), CloseHelpPanel);
    }

    /// <summary>
    /// 创建按钮
    /// </summary>
    Button CreateButton(GameObject parent, string text, Vector2 position, System.Action onClick)
    {
        GameObject buttonObj = new GameObject(text + " Button");
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(160, 28);

        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick?.Invoke());

        // 按钮背景
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.4f, 0.8f, 0.8f);

        // 按钮文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(150, 25);

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 11;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;

        return button;
    }

    /// <summary>
    /// 创建标签
    /// </summary>
    void CreateLabel(GameObject parent, string text, Vector2 position, int fontSize, Color color)
    {
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(parent.transform, false);

        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchoredPosition = position;
        labelRect.sizeDelta = new Vector2(280, 20);

        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = text;
        labelText.fontSize = fontSize;
        labelText.color = color;
        labelText.alignment = TextAlignmentOptions.Center;
    }

    /// <summary>
    /// 创建滑块
    /// </summary>
    void CreateSlider(GameObject parent, string name, Vector2 position, float minValue, float maxValue, float currentValue, System.Action<float> onValueChanged)
    {
        GameObject sliderObj = new GameObject(name + " Slider");
        sliderObj.transform.SetParent(parent.transform, false);

        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchoredPosition = position;
        sliderRect.sizeDelta = new Vector2(200, 20);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = currentValue;
        slider.onValueChanged.AddListener(onValueChanged.Invoke);

        // 添加背景
        Image background = sliderObj.AddComponent<Image>();
        background.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
    }

    /// <summary>
    /// 创建开关
    /// </summary>
    void CreateToggle(GameObject parent, string text, Vector2 position, bool isOn, System.Action<bool> onValueChanged)
    {
        GameObject toggleObj = new GameObject(text + " Toggle");
        toggleObj.transform.SetParent(parent.transform, false);

        RectTransform toggleRect = toggleObj.AddComponent<RectTransform>();
        toggleRect.anchoredPosition = position;
        toggleRect.sizeDelta = new Vector2(200, 25);

        Toggle toggle = toggleObj.AddComponent<Toggle>();
        toggle.isOn = isOn;
        toggle.onValueChanged.AddListener(onValueChanged.Invoke);

        // 添加文本
        CreateLabel(toggleObj, text, new Vector2(20, 0), 10, Color.white);
    }

    /// <summary>
    /// 创建快捷键提示
    /// </summary>
    void CreateKeyHint(GameObject parent, string hint, Vector2 position)
    {
        GameObject hintObj = new GameObject("Key Hint");
        hintObj.transform.SetParent(parent.transform, false);

        RectTransform hintRect = hintObj.AddComponent<RectTransform>();
        hintRect.anchoredPosition = position;
        hintRect.sizeDelta = new Vector2(160, 20);

        TextMeshProUGUI hintText = hintObj.AddComponent<TextMeshProUGUI>();
        hintText.text = hint;
        hintText.fontSize = 9;
        hintText.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.fontStyle = FontStyles.Italic;
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

        if (flightTimeTracker == null)
            flightTimeTracker = FindObjectOfType<FlightTimeTracker>();

        if (landingPointTracker == null)
            landingPointTracker = FindObjectOfType<LandingPointTracker>();

        if (bounceImpactMarker == null)
            bounceImpactMarker = FindObjectOfType<BounceImpactMarker>();

        if (trajectoryDragController == null)
            trajectoryDragController = FindObjectOfType<TrajectoryDragController>();

        if (airResistanceSystem == null)
            airResistanceSystem = FindObjectOfType<AirResistanceSystem>();
    }

    /// <summary>
    /// 设置按钮事件
    /// </summary>
    void SetupButtonEvents()
    {
        // 初始化功能状态
        featureStates["airResistance"] = true;
        featureStates["landingPoint"] = true;
        featureStates["impactMarker"] = true;
        featureStates["trajectoryDrag"] = true;
        featureStates["autoPlay"] = false;
    }

    /// <summary>
    /// 更新按钮状态
    /// </summary>
    void UpdateButtonStates()
    {
        if (!isUIInitialized) return;

        // 更新功能按钮颜色
        UpdateButtonColor(airResistanceButton, featureStates.GetValueOrDefault("airResistance", true));
        UpdateButtonColor(landingPointButton, featureStates.GetValueOrDefault("landingPoint", true));
        UpdateButtonColor(impactMarkerButton, featureStates.GetValueOrDefault("impactMarker", true));
        UpdateButtonColor(autoPlayButton, featureStates.GetValueOrDefault("autoPlay", false));
    }

    /// <summary>
    /// 更新按钮颜色
    /// </summary>
    void UpdateButtonColor(Button button, bool isActive)
    {
        if (button == null) return;

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isActive ?
                new Color(0.2f, 0.8f, 0.2f, 0.8f) : // 绿色 - 激活
                new Color(0.2f, 0.4f, 0.8f, 0.8f);   // 蓝色 - 默认
        }
    }

    // ==================== 按钮事件处理 ====================

    /// <summary>
    /// 发射网球
    /// </summary>
    void LaunchBall()
    {
        if (ballLauncher != null)
        {
            ballLauncher.LaunchBall(Vector3.zero);
            Debug.Log("🚀 Ball launched via UI button");
        }
    }

    /// <summary>
    /// 重置游戏
    /// </summary>
    void ResetGame()
    {
        // 清除所有网球
        ClearAllBalls();

        // 重置摄像机到默认视角
        if (cameraController != null)
        {
            SwitchView("default");
        }

        // 停止自动播放
        if (isAutoPlayMode)
        {
            ToggleAutoPlay();
        }

        Debug.Log("🔄 Game reset via UI button");
    }

    /// <summary>
    /// 清除所有网球
    /// </summary>
    void ClearAllBalls()
    {
        // 使用安全的网球查找方法
        List<GameObject> ballsToDestroy = new List<GameObject>();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall") || obj.name.Contains("Tennis Ball") || obj.name.Contains("Ball"))
            {
                // 确保对象有物理组件，更可能是真实的网球
                if (obj.GetComponent<Rigidbody>() != null || obj.GetComponent<Collider>() != null)
                {
                    ballsToDestroy.Add(obj);
                }
            }
        }

        // 销毁找到的网球
        foreach (GameObject ball in ballsToDestroy)
        {
            if (ball != null)
                DestroyImmediate(ball);
        }

        Debug.Log($"🧹 {ballsToDestroy.Count} tennis balls cleared via UI button");
    }

    /// <summary>
    /// 切换自动播放模式
    /// </summary>
    void ToggleAutoPlay()
    {
        isAutoPlayMode = !isAutoPlayMode;
        featureStates["autoPlay"] = isAutoPlayMode;

        if (isAutoPlayMode)
        {
            autoPlayCoroutine = StartCoroutine(AutoPlayCoroutine());
            Debug.Log($"⏯️ Auto play started (interval: {autoPlayInterval}s)");
        }
        else
        {
            if (autoPlayCoroutine != null)
            {
                StopCoroutine(autoPlayCoroutine);
                autoPlayCoroutine = null;
            }
            Debug.Log("⏹️ Auto play stopped");
        }

        UpdateButtonStates();
    }

    /// <summary>
    /// 自动播放协程
    /// </summary>
    IEnumerator AutoPlayCoroutine()
    {
        while (isAutoPlayMode)
        {
            yield return new WaitForSeconds(autoPlayInterval);

            if (ballLauncher != null)
            {
                LaunchBall();
            }
        }
    }

    /// <summary>
    /// 切换视角
    /// </summary>
    void SwitchView(string viewType)
    {
        if (cameraController == null) return;

        switch (viewType.ToLower())
        {
            case "default":
                cameraController.SetCameraPreset(0); // R键
                break;
            case "top":
                cameraController.SetCameraPreset(1); // T键
                break;
            case "side":
                cameraController.SetCameraPreset(2); // F键
                break;
            case "close":
                cameraController.SetCameraPreset(3); // C键
                break;
            case "panorama":
                cameraController.SetCameraPreset(4); // V键
                break;
            case "back":
                cameraController.SetCameraPreset(5); // B键
                break;
            case "toggle":
                // 在默认视角和后场视角之间切换
                if (cameraController.CurrentPresetIndex == 0)
                    cameraController.SetCameraPreset(5);
                else
                    cameraController.SetCameraPreset(0);
                break;
        }

        Debug.Log($"📷 Switched to {viewType} view via UI button");
    }

    /// <summary>
    /// 触发挥拍测试
    /// </summary>
    void TriggerSwingTest()
    {
        PlayerModel playerModel = FindObjectOfType<PlayerModel>();
        if (playerModel != null)
        {
            // 使用反射调用TriggerSwing方法
            var method = playerModel.GetType().GetMethod("TriggerSwing",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(playerModel, null);
                Debug.Log("🎾 Swing test triggered via UI button");
            }
        }
    }

    /// <summary>
    /// 显示击球高度分析
    /// </summary>
    void ShowHeightAnalysis()
    {
        PlayerModel playerModel = FindObjectOfType<PlayerModel>();
        if (playerModel != null)
        {
            // 使用反射调用ShowHeightAnalysis方法
            var method = playerModel.GetType().GetMethod("ShowHeightAnalysis",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(playerModel, null);
                Debug.Log("📊 Height analysis shown via UI button");
            }
        }
    }

    /// <summary>
    /// 切换空气阻力显示
    /// </summary>
    void ToggleAirResistance()
    {
        featureStates["airResistance"] = !featureStates["airResistance"];

        if (airResistanceSystem != null)
        {
            // 使用反射切换显示状态
            var field = airResistanceSystem.GetType().GetField("showUI",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(airResistanceSystem, featureStates["airResistance"]);
            }
        }

        UpdateButtonStates();
        Debug.Log($"🌪️ Air resistance display: {(featureStates["airResistance"] ? "ON" : "OFF")}");
    }

    /// <summary>
    /// 切换落点追踪
    /// </summary>
    void ToggleLandingPoint()
    {
        featureStates["landingPoint"] = !featureStates["landingPoint"];

        if (landingPointTracker != null)
        {
            landingPointTracker.enabled = featureStates["landingPoint"];
        }

        UpdateButtonStates();
        Debug.Log($"🎯 Landing point tracking: {(featureStates["landingPoint"] ? "ON" : "OFF")}");
    }

    /// <summary>
    /// 切换冲击标记
    /// </summary>
    void ToggleImpactMarker()
    {
        featureStates["impactMarker"] = !featureStates["impactMarker"];

        if (bounceImpactMarker != null)
        {
            bounceImpactMarker.enabled = featureStates["impactMarker"];
        }

        UpdateButtonStates();
        Debug.Log($"💥 Impact marker: {(featureStates["impactMarker"] ? "ON" : "OFF")}");
    }

    /// <summary>
    /// 显示系统状态
    /// </summary>
    void ShowSystemStatus()
    {
        Debug.Log("=== Tennis Venue System Status ===");
        Debug.Log($"Ball Launcher: {(ballLauncher != null ? "✅ Active" : "❌ Missing")}");
        Debug.Log($"Camera Controller: {(cameraController != null ? "✅ Active" : "❌ Missing")}");
        Debug.Log($"Flight Time Tracker: {(flightTimeTracker != null ? "✅ Active" : "❌ Missing")}");
        Debug.Log($"Landing Point Tracker: {(landingPointTracker != null ? "✅ Active" : "❌ Missing")}");
        Debug.Log($"Bounce Impact Marker: {(bounceImpactMarker != null ? "✅ Active" : "❌ Missing")}");
        Debug.Log($"Air Resistance System: {(airResistanceSystem != null ? "✅ Active" : "❌ Missing")}");
        Debug.Log($"Trajectory Drag Controller: {(trajectoryDragController != null ? "✅ Active" : "❌ Missing")}");
        Debug.Log($"Auto Play Mode: {(isAutoPlayMode ? "✅ ON" : "❌ OFF")}");
        Debug.Log("================================");
    }

    /// <summary>
    /// 清除历史记录
    /// </summary>
    void ClearHistory()
    {
        if (landingPointTracker != null)
        {
            // 使用反射调用ClearLandingHistory方法
            var method = landingPointTracker.GetType().GetMethod("ClearLandingHistory",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(landingPointTracker, null);
            }
        }

        Debug.Log("🧹 Landing point history cleared via UI button");
    }

    /// <summary>
    /// 切换标记显示
    /// </summary>
    void ToggleMarkers()
    {
        if (landingPointTracker != null)
        {
            // 使用反射切换标记显示
            var field = landingPointTracker.GetType().GetField("createLandingMarkers",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                bool currentValue = (bool)field.GetValue(landingPointTracker);
                field.SetValue(landingPointTracker, !currentValue);
                Debug.Log($"🎯 Landing markers: {(!currentValue ? "ON" : "OFF")}");
            }
        }
    }

    /// <summary>
    /// 创建测试球
    /// </summary>
    void CreateTestBall()
    {
        if (ballLauncher != null && ballLauncher.ballPrefab != null)
        {
            Vector3 testPosition = new Vector3(0, 3, 0); // 场地中央上方3米
            GameObject testBall = Instantiate(ballLauncher.ballPrefab, testPosition, Quaternion.identity);

            Rigidbody rb = testBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero; // 让球自由下落
            }

            Debug.Log("🎾 Test ball created at (0, 3, 0) via UI button");
        }
    }

    /// <summary>
    /// 运行诊断
    /// </summary>
    void RunDiagnostics()
    {
        Debug.Log("🔍 Running system diagnostics...");

        // 检查场景中的网球数量 - 使用安全方法
        int ballCount = CountTennisBalls();
        Debug.Log($"Tennis balls in scene: {ballCount}");

        // 检查UI组件状态
        Debug.Log($"UI Panels created: {(controlPanel != null && viewControlPanel != null && functionPanel != null && debugPanel != null)}");

        // 检查系统组件
        ShowSystemStatus();

        Debug.Log("✅ Diagnostics completed");
    }

    /// <summary>
    /// 安全地计算网球数量
    /// </summary>
    int CountTennisBalls()
    {
        int count = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall") || obj.name.Contains("Ball"))
            {
                // 确保对象有物理组件，更可能是真实的网球
                if (obj.GetComponent<Rigidbody>() != null || obj.GetComponent<Collider>() != null)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // ==================== 设置面板事件 ====================

    /// <summary>
    /// 自动播放间隔改变
    /// </summary>
    void OnAutoPlayIntervalChanged(float value)
    {
        autoPlayInterval = value;
        Debug.Log($"⏱️ Auto play interval changed to {value:F1} seconds");
    }

    /// <summary>
    /// 轨迹拖动开关
    /// </summary>
    void OnTrajectoryDragToggle(bool isOn)
    {
        if (trajectoryDragController != null)
        {
            trajectoryDragController.enabled = isOn;
            Debug.Log($"🎯 Trajectory drag: {(isOn ? "ON" : "OFF")}");
        }
    }

    /// <summary>
    /// 调试信息开关
    /// </summary>
    void OnDebugInfoToggle(bool isOn)
    {
        // 这里可以控制调试信息的显示
        Debug.Log($"🔧 Debug info: {(isOn ? "ON" : "OFF")}");
    }

    /// <summary>
    /// 自动清理开关
    /// </summary>
    void OnAutoClearToggle(bool isOn)
    {
        // 这里可以实现自动清理功能
        Debug.Log($"🧹 Auto clear: {(isOn ? "ON" : "OFF")}");
    }

    /// <summary>
    /// 打开设置面板
    /// </summary>
    void OpenSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            isSettingsPanelOpen = true;
            Debug.Log("⚙️ Settings panel opened");
        }
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    void CloseSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            isSettingsPanelOpen = false;
            Debug.Log("⚙️ Settings panel closed");
        }
    }

    /// <summary>
    /// 打开帮助面板
    /// </summary>
    void OpenHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
            isHelpPanelOpen = true;
            Debug.Log("❓ Help panel opened");
        }
    }

    /// <summary>
    /// 关闭帮助面板
    /// </summary>
    void CloseHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
            isHelpPanelOpen = false;
            Debug.Log("❓ Help panel closed");
        }
    }

    /// <summary>
    /// 关闭所有弹出面板
    /// </summary>
    void CloseAllPanels()
    {
        CloseSettingsPanel();
        CloseHelpPanel();
    }

    void Update()
    {
        // 保持与键盘快捷键的兼容性
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowSystemStatus();
        }

        // 新增快捷键
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleAutoPlay();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            if (isSettingsPanelOpen)
                CloseSettingsPanel();
            else
                OpenSettingsPanel();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            if (isHelpPanelOpen)
                CloseHelpPanel();
            else
                OpenHelpPanel();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllPanels();
        }

        // 实时更新按钮状态
        if (Time.frameCount % 60 == 0) // 每秒更新一次
        {
            UpdateButtonStates();
        }
    }
}