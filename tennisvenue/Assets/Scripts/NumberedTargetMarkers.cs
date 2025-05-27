using UnityEngine;
using TMPro;

/// <summary>
/// 带数字标识的圆圈标记系统
/// 在落球区域创建10个圆圈，分两行排列，每个圆圈内有数字1-10
/// </summary>
public class NumberedTargetMarkers : MonoBehaviour
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

    [Header("材质配置")]
    public Material circleMaterial;             // 圆圈材质

    private GameObject[] markers = new GameObject[10];  // 存储所有标记
    private bool markersCreated = false;

    void Start()
    {
        Debug.Log("=== 带数字标识的圆圈标记系统启动 ===");
        CreateNumberedMarkers();

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
    /// 创建带数字的圆圈标记
    /// </summary>
    void CreateNumberedMarkers()
    {
        if (markersCreated)
        {
            Debug.LogWarning("标记已存在，请先删除现有标记");
            return;
        }

        Debug.Log("🎯 开始创建带数字的圆圈标记...");

        // 创建父对象来组织所有标记
        GameObject markersParent = new GameObject("NumberedTargetMarkers");
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
            GameObject marker = CreateSingleMarker(i + 1, position);
            marker.transform.SetParent(markersParent.transform);
            markers[i] = marker;

            Debug.Log($"✅ 创建标记 {i + 1}: 位置 {position}, 行{row + 1} 列{col + 1}");
        }

        markersCreated = true;
        Debug.Log($"🎯 成功创建 {markers.Length} 个带数字的圆圈标记");
        Debug.Log($"📍 标记区域中心: {centerPosition}");
        Debug.Log($"📏 行间距: {rowSpacing}m, 列间距: {columnSpacing}m");
    }

    /// <summary>
    /// 创建单个圆圈标记
    /// </summary>
    GameObject CreateSingleMarker(int number, Vector3 position)
    {
        // 创建主标记对象
        GameObject marker = new GameObject($"TargetMarker_{number}");
        marker.transform.position = position;

        // 创建圆圈几何体
        GameObject circle = CreateCircleGeometry(marker.transform);

        // 创建数字文本
        GameObject numberText = CreateNumberText(number, marker.transform);

        return marker;
    }

    /// <summary>
    /// 创建圆圈几何体
    /// </summary>
    GameObject CreateCircleGeometry(Transform parent)
    {
        GameObject circle = new GameObject("Circle");
        circle.transform.SetParent(parent);
        circle.transform.localPosition = Vector3.zero;
        circle.transform.localRotation = Quaternion.Euler(90, 0, 0); // 平躺在地面

        // 添加网格组件
        MeshFilter meshFilter = circle.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = circle.AddComponent<MeshRenderer>();

        // 创建圆圈网格
        meshFilter.mesh = CreateCircleMesh();

        // 设置材质
        if (circleMaterial == null)
        {
            circleMaterial = CreateCircleMaterial();
        }
        meshRenderer.material = circleMaterial;

        return circle;
    }

    /// <summary>
    /// 创建圆圈网格
    /// </summary>
    Mesh CreateCircleMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "CircleMesh";

        int segments = 32;  // 圆圈分段数
        int vertexCount = segments + 1; // 中心点 + 圆周点

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[segments * 3];

        // 中心点
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        // 圆周点
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2;
            vertices[i + 1] = new Vector3(
                Mathf.Cos(angle) * circleRadius,
                0,
                Mathf.Sin(angle) * circleRadius
            );
            uv[i + 1] = new Vector2(
                Mathf.Cos(angle) * 0.5f + 0.5f,
                Mathf.Sin(angle) * 0.5f + 0.5f
            );
        }

        // 创建三角形
        for (int i = 0; i < segments; i++)
        {
            int triangleIndex = i * 3;
            triangles[triangleIndex] = 0;                           // 中心点
            triangles[triangleIndex + 1] = i + 1;                   // 当前圆周点
            triangles[triangleIndex + 2] = (i + 1) % segments + 1;  // 下一个圆周点
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// 创建圆圈材质
    /// </summary>
    Material CreateCircleMaterial()
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = "CircleMarkerMaterial";

        // 设置颜色和透明度
        material.color = circleColor;
        material.SetFloat("_Mode", 3); // Transparent mode
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        // 添加发光效果
        material.SetFloat("_Metallic", 0.0f);
        material.SetFloat("_Glossiness", 0.8f);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", circleColor * 0.3f);

        return material;
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
        rectTransform.sizeDelta = new Vector2(circleRadius * 2, circleRadius * 2);

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
        CreateNumberedMarkers();
    }

    /// <summary>
    /// 删除现有标记
    /// </summary>
    void DestroyExistingMarkers()
    {
        // 查找并删除现有的标记父对象
        GameObject existingMarkers = GameObject.Find("NumberedTargetMarkers");
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
        GameObject markersParent = GameObject.Find("NumberedTargetMarkers");
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

        GameObject markersParent = GameObject.Find("NumberedTargetMarkers");
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

        GameObject markersParent = GameObject.Find("NumberedTargetMarkers");
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
        GameObject markersParent = GameObject.Find("NumberedTargetMarkers");
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
        MeshRenderer renderer = marker.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = highlightColor;

            yield return new WaitForSeconds(duration);

            renderer.material.color = originalColor;
        }
    }
}