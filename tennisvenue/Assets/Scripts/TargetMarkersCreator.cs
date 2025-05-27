using UnityEngine;

/// <summary>
/// 目标标记创建器
/// 直接在场景中创建10个带数字的圆圈标记
/// </summary>
public class TargetMarkersCreator : MonoBehaviour
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

    void Start()
    {
        Debug.Log("=== 目标标记创建器启动 ===");
        CreateTargetMarkers();

        Debug.Log("快捷键:");
        Debug.Log("  F6键: 重新创建标记");
        Debug.Log("  F7键: 切换标记显示/隐藏");
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
    }

    /// <summary>
    /// 创建目标标记
    /// </summary>
    void CreateTargetMarkers()
    {
        Debug.Log("🎯 开始创建目标标记...");

        // 删除现有标记
        GameObject existingMarkers = GameObject.Find("TargetMarkers");
        if (existingMarkers != null)
        {
            DestroyImmediate(existingMarkers);
        }

        // 创建父对象来组织所有标记
        GameObject markersParent = new GameObject("TargetMarkers");
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
            CreateSingleMarker(i + 1, position, markersParent.transform);

            Debug.Log($"✅ 创建标记 {i + 1}: 位置 {position}, 行{row + 1} 列{col + 1}");
        }

        Debug.Log($"🎯 成功创建 10 个目标标记");
        Debug.Log($"📍 标记区域中心: {centerPosition}");
        Debug.Log($"📏 行间距: {rowSpacing}m, 列间距: {columnSpacing}m");
    }

    /// <summary>
    /// 创建单个标记
    /// </summary>
    void CreateSingleMarker(int number, Vector3 position, Transform parent)
    {
        // 创建主标记对象
        GameObject marker = new GameObject($"Marker_{number}");
        marker.transform.position = position;
        marker.transform.SetParent(parent);

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

        // 设置透明度
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

        // 创建数字文本（使用3D文本）
        GameObject textObj = new GameObject($"Number_{number}");
        textObj.transform.SetParent(marker.transform);
        textObj.transform.localPosition = new Vector3(0, 0.02f, 0); // 略高于圆圈
        textObj.transform.localRotation = Quaternion.Euler(90, 0, 0); // 面向上方
        // textObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // 可选：根据需要调整文本对象的整体缩放

        // 添加3D文本组件
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = number.ToString();
        textMesh.fontSize = 20; // 可以根据 Font 资源和 Character Size 调整
        textMesh.characterSize = 0.1f; // 关键属性：调整字符的实际大小
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontStyle = FontStyle.Bold;

        // 设置文本渲染器 (暂时注释掉手动材质设置，观察默认效果)
        /*
        MeshRenderer textRenderer = textObj.GetComponent<MeshRenderer>();
        if (textRenderer != null)
        {
            Material textMaterial = new Material(Shader.Find("Standard"));
            textMaterial.color = Color.white;
            textMaterial.SetFloat("_Metallic", 0.0f);
            textMaterial.SetFloat("_Glossiness", 0.0f);
            textRenderer.material = textMaterial;
        }
        */
    }

    /// <summary>
    /// 重新创建标记
    /// </summary>
    void RecreateMarkers()
    {
        Debug.Log("🔄 重新创建标记...");
        CreateTargetMarkers();
    }

    /// <summary>
    /// 切换标记显示/隐藏
    /// </summary>
    void ToggleMarkersVisibility()
    {
        GameObject markersParent = GameObject.Find("TargetMarkers");
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
    /// 获取指定编号的标记位置
    /// </summary>
    public Vector3 GetMarkerPosition(int markerNumber)
    {
        if (markerNumber < 1 || markerNumber > 10)
        {
            Debug.LogWarning($"⚠️ 标记编号 {markerNumber} 超出范围 (1-10)");
            return Vector3.zero;
        }

        GameObject markersParent = GameObject.Find("TargetMarkers");
        if (markersParent != null)
        {
            Transform marker = markersParent.transform.Find($"Marker_{markerNumber}");
            if (marker != null)
            {
                return marker.position;
            }
        }

        Debug.LogWarning($"⚠️ 未找到标记 {markerNumber}");
        return Vector3.zero;
    }
}