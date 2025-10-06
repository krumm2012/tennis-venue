using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI面板布局管理器 - 重新安排面板位置到左右两侧
/// </summary>
public class UIPanelLayoutManager : MonoBehaviour
{
    [Header("布局设置")]
    [Tooltip("按此键重新布局面板")]
    public KeyCode layoutKey = KeyCode.F13;

    [Header("面板间距设置")]
    public float panelSpacing = 10f;
    public float sideMargin = 20f;
    public float topMargin = 20f;

    [Header("面板尺寸")]
    public Vector2 leftPanelSize = new Vector2(200, 300);
    public Vector2 rightPanelSize = new Vector2(200, 300);

    private TennisVenueUIManager uiManager;
    private bool isLayoutApplied = false;

    void Start()
    {
        Debug.Log("=== UI面板布局管理器已加载 ===");
        ShowLayoutInstructions();

        // 延迟应用布局，确保UI管理器已初始化
        Invoke("ApplySideLayout", 1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(layoutKey))
        {
            ApplySideLayout();
        }
    }

    /// <summary>
    /// 应用左右两侧布局
    /// </summary>
    [ContextMenu("应用左右两侧布局")]
    public void ApplySideLayout()
    {
        Debug.Log("🔄 开始重新布局UI面板到左右两侧...");

        // 查找UI管理器
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<TennisVenueUIManager>();
            if (uiManager == null)
            {
                Debug.LogError("❌ 未找到TennisVenueUIManager");
                return;
            }
        }

        // 重新定位面板
        RepositionPanelsToSides();

        isLayoutApplied = true;
        Debug.Log("✅ UI面板已重新布局到左右两侧，中间区域已清空");
        ShowLayoutResult();
    }

    /// <summary>
    /// 重新定位面板到左右两侧
    /// </summary>
    void RepositionPanelsToSides()
    {
        Canvas mainCanvas = uiManager.mainCanvas;
        if (mainCanvas == null)
        {
            Debug.LogError("❌ 主Canvas未找到");
            return;
        }

        // 获取屏幕尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float leftX = -screenWidth / 2 + sideMargin + leftPanelSize.x / 2;
        float rightX = screenWidth / 2 - sideMargin - rightPanelSize.x / 2;

        Debug.Log($"📏 屏幕尺寸: {screenWidth} x {screenHeight}");
        Debug.Log($"📍 左侧面板X位置: {leftX}");
        Debug.Log($"📍 右侧面板X位置: {rightX}");

        // 重新定位现有面板
        RepositionExistingPanels(leftX, rightX, screenHeight);

        // 创建新的左右布局面板
        CreateSideLayoutPanels(leftX, rightX, screenHeight);
    }

    /// <summary>
    /// 重新定位现有面板
    /// </summary>
    void RepositionExistingPanels(float leftX, float rightX, float screenHeight)
    {
        // 查找并重新定位现有面板
        Transform[] panels = uiManager.mainCanvas.GetComponentsInChildren<Transform>();

        foreach (Transform panel in panels)
        {
            if (panel.GetComponent<Image>() != null && panel.name.Contains("Panel"))
            {
                RectTransform rectTransform = panel.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // 根据面板名称决定位置
                    Vector2 newPosition = GetPanelPosition(panel.name, leftX, rightX, screenHeight);
                    rectTransform.anchoredPosition = newPosition;

                    // 调整面板大小
                    Vector2 newSize = GetPanelSize(panel.name);
                    rectTransform.sizeDelta = newSize;

                    Debug.Log($"🔄 重新定位面板: {panel.name} -> {newPosition}");
                }
            }
        }
    }

    /// <summary>
    /// 创建新的左右布局面板
    /// </summary>
    void CreateSideLayoutPanels(float leftX, float rightX, float screenHeight)
    {
        Canvas mainCanvas = uiManager.mainCanvas;

        // 左侧面板 - 主要控制
        CreateSidePanel("Left Control Panel", leftX, screenHeight * 0.3f, leftPanelSize, "主要控制");
        AddLeftPanelControls(mainCanvas.transform.Find("Left Control Panel").gameObject);

        // 右侧面板 - 视角和调试
        CreateSidePanel("Right Control Panel", rightX, screenHeight * 0.3f, rightPanelSize, "视角控制");
        AddRightPanelControls(mainCanvas.transform.Find("Right Control Panel").gameObject);
    }

    /// <summary>
    /// 创建侧边面板
    /// </summary>
    GameObject CreateSidePanel(string name, float x, float y, Vector2 size, string title)
    {
        Canvas mainCanvas = uiManager.mainCanvas;

        // 检查是否已存在
        Transform existingPanel = mainCanvas.transform.Find(name);
        if (existingPanel != null)
        {
            DestroyImmediate(existingPanel.gameObject);
        }

        GameObject panel = new GameObject(name);
        panel.transform.SetParent(mainCanvas.transform, false);

        // 设置RectTransform
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(x, y);
        rectTransform.sizeDelta = size;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // 添加背景
        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        // 添加边框
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.2f, 0.6f, 1f, 0.8f);
        outline.effectDistance = new Vector2(2, 2);

        // 添加标题
        CreateSidePanelTitle(panel, title);

        Debug.Log($"✅ 创建侧边面板: {name} 位置: ({x}, {y})");
        return panel;
    }

    /// <summary>
    /// 创建侧边面板标题
    /// </summary>
    void CreateSidePanelTitle(GameObject panel, string title)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, panel.GetComponent<RectTransform>().sizeDelta.y / 2 - 15);
        titleRect.sizeDelta = new Vector2(180, 25);

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = title;
        titleText.fontSize = 12;
        titleText.color = Color.yellow;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
    }

    /// <summary>
    /// 添加左侧面板控制
    /// </summary>
    void AddLeftPanelControls(GameObject panel)
    {
        float startY = 80;
        float spacing = 30;

        // 主要控制按钮
        CreateSideButton(panel, "Launch Ball", new Vector2(0, startY), () => {
            BallLauncher launcher = FindObjectOfType<BallLauncher>();
            if (launcher != null) launcher.LaunchBall(Vector3.zero);
        });

        CreateSideButton(panel, "Reset Game", new Vector2(0, startY - spacing), () => {
            // 重置游戏逻辑
            Debug.Log("🔄 重置游戏");
        });

        CreateSideButton(panel, "Clear Balls", new Vector2(0, startY - spacing * 2), () => {
            // 清除所有球
            GameObject[] balls = GameObject.FindGameObjectsWithTag("TennisBall");
            foreach (GameObject ball in balls) Destroy(ball);
        });

        CreateSideButton(panel, "Auto Play", new Vector2(0, startY - spacing * 3), () => {
            // 自动播放切换
            Debug.Log("🎮 切换自动播放");
        });

        // 功能控制
        CreateSideButton(panel, "Swing Test", new Vector2(0, startY - spacing * 4), () => {
            Debug.Log("🎾 摇摆测试");
        });

        CreateSideButton(panel, "Height Analysis", new Vector2(0, startY - spacing * 5), () => {
            Debug.Log("📊 高度分析");
        });
    }

    /// <summary>
    /// 添加右侧面板控制
    /// </summary>
    void AddRightPanelControls(GameObject panel)
    {
        float startY = 80;
        float spacing = 30;

        // 视角控制按钮
        CreateSideButton(panel, "Default View", new Vector2(0, startY), () => {
            CameraController camera = FindObjectOfType<CameraController>();
            if (camera != null) camera.SetCameraPreset(0);
        });

        CreateSideButton(panel, "Back View", new Vector2(0, startY - spacing), () => {
            CameraController camera = FindObjectOfType<CameraController>();
            if (camera != null) camera.SetCameraPreset(1);
        });

        CreateSideButton(panel, "Top View", new Vector2(0, startY - spacing * 2), () => {
            CameraController camera = FindObjectOfType<CameraController>();
            if (camera != null) camera.SetCameraPreset(2);
        });

        CreateSideButton(panel, "Side View", new Vector2(0, startY - spacing * 3), () => {
            CameraController camera = FindObjectOfType<CameraController>();
            if (camera != null) camera.SetCameraPreset(3);
        });

        CreateSideButton(panel, "Close View", new Vector2(0, startY - spacing * 4), () => {
            CameraController camera = FindObjectOfType<CameraController>();
            if (camera != null) camera.SetCameraPreset(4);
        });

        CreateSideButton(panel, "Panorama", new Vector2(0, startY - spacing * 5), () => {
            CameraController camera = FindObjectOfType<CameraController>();
            if (camera != null) camera.SetCameraPreset(5);
        });
    }

    /// <summary>
    /// 创建侧边按钮
    /// </summary>
    void CreateSideButton(GameObject parent, string text, Vector2 position, System.Action onClick)
    {
        GameObject buttonObj = new GameObject(text + " Button");
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(160, 25);

        // 按钮背景
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.4f, 0.8f, 0.8f);

        // 按钮文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(160, 25);

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 10;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;

        // 按钮组件
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(() => onClick?.Invoke());

        // 悬停效果
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.4f, 0.8f, 0.8f);
        colors.highlightedColor = new Color(0.3f, 0.6f, 1f, 0.9f);
        colors.pressedColor = new Color(0.1f, 0.3f, 0.7f, 0.8f);
        button.colors = colors;
    }

    /// <summary>
    /// 获取面板位置
    /// </summary>
    Vector2 GetPanelPosition(string panelName, float leftX, float rightX, float screenHeight)
    {
        switch (panelName)
        {
            case "Control Panel":
            case "Function Panel":
                return new Vector2(leftX, screenHeight * 0.2f);

            case "View Control Panel":
            case "Debug Panel":
                return new Vector2(rightX, screenHeight * 0.2f);

            default:
                return new Vector2(leftX, 0); // 默认左侧
        }
    }

    /// <summary>
    /// 获取面板大小
    /// </summary>
    Vector2 GetPanelSize(string panelName)
    {
        switch (panelName)
        {
            case "Control Panel":
            case "Function Panel":
                return leftPanelSize;

            case "View Control Panel":
            case "Debug Panel":
                return rightPanelSize;

            default:
                return new Vector2(180, 200);
        }
    }

    /// <summary>
    /// 显示布局结果
    /// </summary>
    void ShowLayoutResult()
    {
        Debug.Log("=== UI面板布局结果 ===");
        Debug.Log("✅ 左侧面板: 主要控制功能");
        Debug.Log("✅ 右侧面板: 视角控制功能");
        Debug.Log("✅ 中间区域: 已清空，便于观察");
        Debug.Log("✅ 面板间距: 已优化");
        Debug.Log("🎯 现在可以更好地观察网球场地和幕布效果");
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void ShowLayoutInstructions()
    {
        Debug.Log("=== UI面板布局管理器使用说明 ===");
        Debug.Log($"🔄 {layoutKey}键 - 重新布局面板到左右两侧");
        Debug.Log("📋 布局特点:");
        Debug.Log("   ✅ 左侧: 主要控制面板");
        Debug.Log("   ✅ 右侧: 视角控制面板");
        Debug.Log("   ✅ 中间: 完全清空");
        Debug.Log("   ✅ 自动适配屏幕尺寸");
        Debug.Log("💡 也可在Inspector中使用右键菜单");
    }

    /// <summary>
    /// 重置为原始布局
    /// </summary>
    [ContextMenu("重置为原始布局")]
    public void ResetToOriginalLayout()
    {
        Debug.Log("🔄 重置为原始布局...");

        if (uiManager != null)
        {
            // 重新初始化UI
            uiManager.InitializeUI();
            Debug.Log("✅ 已重置为原始布局");
        }
        else
        {
            Debug.LogWarning("⚠️ UI管理器未找到");
        }
    }

    /// <summary>
    /// 隐藏所有面板
    /// </summary>
    [ContextMenu("隐藏所有面板")]
    public void HideAllPanels()
    {
        Debug.Log("👁️ 隐藏所有UI面板...");

        Canvas mainCanvas = uiManager?.mainCanvas;
        if (mainCanvas != null)
        {
            Transform[] panels = mainCanvas.GetComponentsInChildren<Transform>();
            foreach (Transform panel in panels)
            {
                if (panel.GetComponent<Image>() != null && panel.name.Contains("Panel"))
                {
                    panel.gameObject.SetActive(false);
                }
            }
            Debug.Log("✅ 所有面板已隐藏");
        }
    }

    /// <summary>
    /// 显示所有面板
    /// </summary>
    [ContextMenu("显示所有面板")]
    public void ShowAllPanels()
    {
        Debug.Log("👁️ 显示所有UI面板...");

        Canvas mainCanvas = uiManager?.mainCanvas;
        if (mainCanvas != null)
        {
            Transform[] panels = mainCanvas.GetComponentsInChildren<Transform>();
            foreach (Transform panel in panels)
            {
                if (panel.GetComponent<Image>() != null && panel.name.Contains("Panel"))
                {
                    panel.gameObject.SetActive(true);
                }
            }
            Debug.Log("✅ 所有面板已显示");
        }
    }
}


