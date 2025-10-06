using UnityEngine;

/// <summary>
/// UI布局测试器 - 测试和验证面板布局效果
/// </summary>
public class UILayoutTester : MonoBehaviour
{
    [Header("测试设置")]
    [Tooltip("按此键测试布局")]
    public KeyCode testLayoutKey = KeyCode.F14;

    [Header("测试选项")]
    public bool testLeftPanel = true;
    public bool testRightPanel = true;
    public bool testMiddleArea = true;

    void Start()
    {
        Debug.Log("=== UI布局测试器已加载 ===");
        ShowTestInstructions();
    }

    void Update()
    {
        if (Input.GetKeyDown(testLayoutKey))
        {
            TestUILayout();
        }
    }

    /// <summary>
    /// 测试UI布局
    /// </summary>
    [ContextMenu("测试UI布局")]
    public void TestUILayout()
    {
        Debug.Log("🧪 开始测试UI布局...");

        // 测试左侧面板
        if (testLeftPanel)
        {
            TestLeftPanel();
        }

        // 测试右侧面板
        if (testRightPanel)
        {
            TestRightPanel();
        }

        // 测试中间区域
        if (testMiddleArea)
        {
            TestMiddleArea();
        }

        Debug.Log("✅ UI布局测试完成");
    }

    /// <summary>
    /// 测试左侧面板
    /// </summary>
    void TestLeftPanel()
    {
        Debug.Log("📋 测试左侧面板...");

        GameObject leftPanel = GameObject.Find("Left Control Panel");
        if (leftPanel != null)
        {
            RectTransform rect = leftPanel.GetComponent<RectTransform>();
            Debug.Log($"✅ 左侧面板位置: {rect.anchoredPosition}");
            Debug.Log($"✅ 左侧面板大小: {rect.sizeDelta}");

            // 检查按钮数量
            Button[] buttons = leftPanel.GetComponentsInChildren<Button>();
            Debug.Log($"✅ 左侧面板按钮数量: {buttons.Length}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到左侧控制面板");
        }
    }

    /// <summary>
    /// 测试右侧面板
    /// </summary>
    void TestRightPanel()
    {
        Debug.Log("📋 测试右侧面板...");

        GameObject rightPanel = GameObject.Find("Right Control Panel");
        if (rightPanel != null)
        {
            RectTransform rect = rightPanel.GetComponent<RectTransform>();
            Debug.Log($"✅ 右侧面板位置: {rect.anchoredPosition}");
            Debug.Log($"✅ 右侧面板大小: {rect.sizeDelta}");

            // 检查按钮数量
            Button[] buttons = rightPanel.GetComponentsInChildren<Button>();
            Debug.Log($"✅ 右侧面板按钮数量: {buttons.Length}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到右侧控制面板");
        }
    }

    /// <summary>
    /// 测试中间区域
    /// </summary>
    void TestMiddleArea()
    {
        Debug.Log("📋 测试中间区域...");

        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas != null)
        {
            // 检查中间区域是否有UI元素
            Transform[] allChildren = mainCanvas.GetComponentsInChildren<Transform>();
            int middleElements = 0;

            foreach (Transform child in allChildren)
            {
                if (child.GetComponent<Image>() != null && child.name.Contains("Panel"))
                {
                    RectTransform rect = child.GetComponent<RectTransform>();
                    float xPos = rect.anchoredPosition.x;

                    // 检查是否在中间区域（-100 到 100 之间）
                    if (xPos > -100 && xPos < 100)
                    {
                        middleElements++;
                        Debug.LogWarning($"⚠️ 中间区域发现面板: {child.name} 位置: {xPos}");
                    }
                }
            }

            if (middleElements == 0)
            {
                Debug.Log("✅ 中间区域已清空，无UI面板");
            }
            else
            {
                Debug.Log($"⚠️ 中间区域还有 {middleElements} 个面板需要清理");
            }
        }
    }

    /// <summary>
    /// 创建测试标记
    /// </summary>
    [ContextMenu("创建测试标记")]
    public void CreateTestMarkers()
    {
        Debug.Log("🎯 创建UI布局测试标记...");

        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
        {
            Debug.LogError("❌ 未找到主Canvas");
            return;
        }

        // 创建左侧标记
        CreateTestMarker("Left Marker", new Vector2(-300, 0), Color.green, "LEFT");

        // 创建右侧标记
        CreateTestMarker("Right Marker", new Vector2(300, 0), Color.red, "RIGHT");

        // 创建中间标记
        CreateTestMarker("Middle Marker", new Vector2(0, 0), Color.yellow, "MIDDLE");

        Debug.Log("✅ 测试标记已创建");
        Debug.Log("🎯 绿色标记 = 左侧区域");
        Debug.Log("🎯 红色标记 = 右侧区域");
        Debug.Log("🎯 黄色标记 = 中间区域（应该清空）");
    }

    /// <summary>
    /// 创建测试标记
    /// </summary>
    void CreateTestMarker(string name, Vector2 position, Color color, string text)
    {
        Canvas mainCanvas = FindObjectOfType<Canvas>();

        GameObject marker = new GameObject(name);
        marker.transform.SetParent(mainCanvas.transform, false);

        RectTransform rect = marker.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(100, 30);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        Image image = marker.AddComponent<Image>();
        image.color = new Color(color.r, color.g, color.b, 0.5f);

        // 添加文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(marker.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(100, 30);

        TMPro.TextMeshProUGUI textComponent = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 12;
        textComponent.color = Color.white;
        textComponent.alignment = TMPro.TextAlignmentOptions.Center;
        textComponent.fontStyle = TMPro.FontStyles.Bold;

        // 5秒后自动销毁
        Destroy(marker, 5f);
    }

    /// <summary>
    /// 显示测试说明
    /// </summary>
    void ShowTestInstructions()
    {
        Debug.Log("=== UI布局测试器使用说明 ===");
        Debug.Log($"🧪 {testLayoutKey}键 - 测试UI布局");
        Debug.Log("📋 测试内容:");
        Debug.Log("   ✅ 检查左侧面板位置和功能");
        Debug.Log("   ✅ 检查右侧面板位置和功能");
        Debug.Log("   ✅ 验证中间区域是否清空");
        Debug.Log("   ✅ 创建可视化测试标记");
        Debug.Log("💡 也可在Inspector中使用右键菜单");
    }

    /// <summary>
    /// 清理测试标记
    /// </summary>
    [ContextMenu("清理测试标记")]
    public void ClearTestMarkers()
    {
        Debug.Log("🧹 清理测试标记...");

        GameObject[] markers = GameObject.FindGameObjectsWithTag("Untagged");
        int clearedCount = 0;

        foreach (GameObject obj in markers)
        {
            if (obj.name.Contains("Marker") && obj.GetComponent<Image>() != null)
            {
                DestroyImmediate(obj);
                clearedCount++;
            }
        }

        Debug.Log($"✅ 已清理 {clearedCount} 个测试标记");
    }

    /// <summary>
    /// 生成布局报告
    /// </summary>
    [ContextMenu("生成布局报告")]
    public void GenerateLayoutReport()
    {
        Debug.Log("📊 生成UI布局报告...");

        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas == null)
        {
            Debug.LogError("❌ 未找到主Canvas");
            return;
        }

        Transform[] allChildren = mainCanvas.GetComponentsInChildren<Transform>();
        int leftPanels = 0;
        int rightPanels = 0;
        int middlePanels = 0;
        int totalButtons = 0;

        foreach (Transform child in allChildren)
        {
            if (child.GetComponent<Image>() != null && child.name.Contains("Panel"))
            {
                RectTransform rect = child.GetComponent<RectTransform>();
                float xPos = rect.anchoredPosition.x;

                if (xPos < -50) leftPanels++;
                else if (xPos > 50) rightPanels++;
                else middlePanels++;

                Button[] buttons = child.GetComponentsInChildren<Button>();
                totalButtons += buttons.Length;
            }
        }

        Debug.Log("=== UI布局报告 ===");
        Debug.Log($"📊 左侧面板数量: {leftPanels}");
        Debug.Log($"📊 右侧面板数量: {rightPanels}");
        Debug.Log($"📊 中间面板数量: {middlePanels}");
        Debug.Log($"📊 总按钮数量: {totalButtons}");
        Debug.Log($"📊 屏幕尺寸: {Screen.width} x {Screen.height}");

        if (middlePanels == 0)
        {
            Debug.Log("✅ 布局状态: 优秀 - 中间区域已完全清空");
        }
        else
        {
            Debug.Log($"⚠️ 布局状态: 需要优化 - 中间区域还有 {middlePanels} 个面板");
        }
    }
}


