using UnityEngine;
using TMPro;

/// <summary>
/// 简化版带数字标识的圆圈标记系统
/// 在落球区域创建10个圆圈，分两行排列，每个圆圈内有数字1-10
/// </summary>
public class SimpleTargetMarkers : MonoBehaviour
{
    [Header("标记配置")]
    public float circleRadius = 0.3f;           // 圆圈半径
    public float markerHeight = 0.01f;          // 标记高度（略高于地面）
    public int markersPerRow = 5;               // 每行标记数量
    public float rowSpacing = 1.2f;             // 行间距
    public float columnSpacing = 0.8f;          // 列间距

    [Header("位置配置")]
    public Vector3 centerPosition = new Vector3(0, 0.01f, 2.5f);  // 标记区域中心位置

    [Header("外观配置")]
    public Color circleColor = new Color(0.2f, 0.6f, 1.0f, 0.8f);  // 圆圈颜色（蓝色半透明）
    public Color numberColor = Color.white;      // 数字颜色
    public float numberSize = 0.5f;             // 数字大小

    private GameObject[] markers = new GameObject[10];  // 存储所有标记
    private bool markersCreated = false;

    void Start()
    {
        Debug.Log("=== 简化版带数字标识的圆圈标记系统启动 ===");
        CreateSimpleMarkers();

        Debug.Log("快捷键:");
        Debug.Log("  F6键: 重新创建标记");
        Debug.Log("  F7键: 切换标记显示/隐藏");
        Debug.Log("  F8键: 调整标记位置");
        Debug.Log("  F9键: 测试标记可见性");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            RecreateMarkers();
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            ToggleMarkersVisibility();
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            AdjustMarkersPosition();
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            TestMarkersVisibility();
        }
    }

    /// <summary>
    /// 创建简化版带数字的圆圈标记
    /// </summary>
    void CreateSimpleMarkers()
    {
        if (markersCreated)
        {
            Debug.LogWarning("标记已存在，请先删除现有标记");
            return;
        }

        Debug.Log("🎯 开始创建简化版带数字的圆圈标记...");

        // 创建父对象来组织所有标记
        GameObject markersParent = new GameObject("SimpleTargetMarkers");
        markersParent.transform.position = Vector3.zero;

        // 计算起始位置（左上角）
        float startX = centerPosition.x - (markersPerRow - 1) * columnSpacing * 0.5f;
        float startZ = centerPosition.z + rowSpacing * 0.5f;

        for (int i = 0; i < 10; i++)
        {
            int row = i / markersPerRow;        // 行索引 (0或1)
            int col = i % markersPerRow;        // 列索引 (0-4)

            // 计算位置
            Vector3 position = new Vector3(
                startX + col * columnSpacing,
                centerPosition.y + markerHeight,
                startZ - row * rowSpacing
            );

            // 创建单个标记
            GameObject marker = CreateSimpleMarker(i + 1, position);
            marker.transform.SetParent(markersParent.transform);
            markers[i] = marker;

            Debug.Log($"✅ 创建标记 {i + 1}: 位置 {position}, 行{row + 1} 列{col + 1}");
        }

        markersCreated = true;
        Debug.Log($"🎯 成功创建 {markers.Length} 个简化版带数字的圆圈标记");
        Debug.Log($"📍 标记区域中心: {centerPosition}");
        Debug.Log($"📏 行间距: {rowSpacing}m, 列间距: {columnSpacing}m");
    }

    /// <summary>
    /// 创建单个简化圆圈标记（使用Unity基本几何体）
    /// </summary>
    GameObject CreateSimpleMarker(int number, Vector3 position)
    {
        // 创建主标记对象
        GameObject marker = new GameObject($"TargetMarker_{number}");
        marker.transform.position = position;

        // 创建圆圈（使用扁平的圆柱体）
        GameObject circle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        circle.name = "Circle";
        circle.transform.SetParent(marker.transform);
        circle.transform.localPosition = Vector3.zero;
        circle.transform.localScale = new Vector3(circleRadius * 2, 0.01f, circleRadius * 2); // 扁平圆柱体

        // 设置圆圈材质
        Renderer circleRenderer = circle.GetComponent<Renderer>();
        Material circleMaterial = new Material(Shader.Find("Standard"));
        circleMaterial.color = circleColor;
        circleMaterial.SetFloat("_Mode", 3); // Transparent mode
        circleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        circleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        circleMaterial.SetInt("_ZWrite", 0);
        circleMaterial.DisableKeyword("_ALPHATEST_ON");
        circleMaterial.EnableKeyword("_ALPHABLEND_ON");
        circleMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        circleMaterial.renderQueue = 3000;
        circleRenderer.material = circleMaterial;

        // 移除碰撞器（不需要物理交互）
        Collider circleCollider = circle.GetComponent<Collider>();
        if (circleCollider != null)
        {
            DestroyImmediate(circleCollider);
        }

        // 创建数字文本
        GameObject numberText = CreateNumberText(number, marker.transform);

        return marker;
    }

    /// <summary>
    /// 创建数字文本
    /// </summary>
    GameObject CreateNumberText(int number, Transform parent)
    {
        GameObject textObj = new GameObject($"Number_{number}");
        textObj.transform.SetParent(parent);
        textObj.transform.localPosition = new Vector3(0, 0.02f, 0); // 略高于圆圈
        textObj.transform.localRotation = Quaternion.Euler(90, 0, 0); // 面向上方

        // 添加TextMeshPro组件
        TextMeshPro textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.text = number.ToString();
        textMesh.fontSize = numberSize;
        textMesh.color = numberColor;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontStyle = FontStyles.Bold;

        // 设置文本大小和位置
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(circleRadius * 2, circleRadius * 2);
        }

        return textObj;
    }

    /// <summary>
    /// 重新创建标记
    /// </summary>
    void RecreateMarkers()
    {
        Debug.Log("🔄 重新创建标记...");

        // 删除现有标记
        DestroyExistingMarkers();

        // 重新创建
        CreateSimpleMarkers();
    }

    /// <summary>
    /// 删除现有标记
    /// </summary>
    void DestroyExistingMarkers()
    {
        // 查找并删除现有的标记父对象
        GameObject existingMarkers = GameObject.Find("SimpleTargetMarkers");
        if (existingMarkers != null)
        {
            DestroyImmediate(existingMarkers);
            Debug.Log("🗑️ 已删除现有标记");
        }

        // 清空数组
        for (int i = 0; i < markers.Length; i++)
        {
            markers[i] = null;
        }

        markersCreated = false;
    }

    /// <summary>
    /// 切换标记显示/隐藏
    /// </summary>
    void ToggleMarkersVisibility()
    {
        GameObject markersParent = GameObject.Find("SimpleTargetMarkers");
        if (markersParent != null)
        {
            bool isActive = markersParent.activeSelf;
            markersParent.SetActive(!isActive);
            Debug.Log($"👁️ 标记显示状态: {(!isActive ? "显示" : "隐藏")}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到标记对象");
        }
    }

    /// <summary>
    /// 调整标记位置
    /// </summary>
    void AdjustMarkersPosition()
    {
        Debug.Log("🔧 调整标记位置...");

        // 可以根据需要调整centerPosition
        centerPosition.z += 0.5f; // 向前移动0.5米

        RecreateMarkers();
        Debug.Log($"📍 新的中心位置: {centerPosition}");
    }

    /// <summary>
    /// 测试标记可见性
    /// </summary>
    void TestMarkersVisibility()
    {
        Debug.Log("🔍 测试标记可见性...");

        GameObject markersParent = GameObject.Find("SimpleTargetMarkers");
        if (markersParent == null)
        {
            Debug.LogError("❌ 未找到标记父对象");
            return;
        }

        int visibleCount = 0;
        int totalCount = 0;

        for (int i = 0; i < markersParent.transform.childCount; i++)
        {
            Transform child = markersParent.transform.GetChild(i);
            totalCount++;

            if (child.gameObject.activeInHierarchy)
            {
                visibleCount++;
                Debug.Log($"✅ 标记 {child.name} 可见 - 位置: {child.position}");
            }
            else
            {
                Debug.Log($"❌ 标记 {child.name} 不可见");
            }
        }

        Debug.Log($"📊 可见性测试结果: {visibleCount}/{totalCount} 个标记可见");

        // 检查摄像机视野
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log($"📷 主摄像机位置: {mainCamera.transform.position}");
            Debug.Log($"📷 主摄像机朝向: {mainCamera.transform.forward}");
            Debug.Log("💡 建议使用T键切换到俯视角度查看标记");
        }
    }

    /// <summary>
    /// 获取指定编号的标记位置
    /// </summary>
    public Vector3 GetMarkerPosition(int markerNumber)
    {
        if (markerNumber < 1 || markerNumber > 10)
        {
            Debug.LogWarning($"⚠️ 标记编号 {markerNumber} 超出范围 (1-10)");
            return Vector3.zero;
        }

        GameObject markersParent = GameObject.Find("SimpleTargetMarkers");
        if (markersParent != null)
        {
            Transform marker = markersParent.transform.Find($"TargetMarker_{markerNumber}");
            if (marker != null)
            {
                return marker.position;
            }
        }

        Debug.LogWarning($"⚠️ 未找到标记 {markerNumber}");
        return Vector3.zero;
    }

    /// <summary>
    /// 高亮指定标记
    /// </summary>
    public void HighlightMarker(int markerNumber, Color highlightColor, float duration = 2.0f)
    {
        GameObject markersParent = GameObject.Find("SimpleTargetMarkers");
        if (markersParent != null)
        {
            Transform marker = markersParent.transform.Find($"TargetMarker_{markerNumber}");
            if (marker != null)
            {
                StartCoroutine(HighlightMarkerCoroutine(marker, highlightColor, duration));
            }
        }
    }

    /// <summary>
    /// 高亮标记协程
    /// </summary>
    System.Collections.IEnumerator HighlightMarkerCoroutine(Transform marker, Color highlightColor, float duration)
    {
        Renderer renderer = marker.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = highlightColor;

            yield return new WaitForSeconds(duration);

            renderer.material.color = originalColor;
        }
    }
}