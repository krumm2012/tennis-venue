using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 网球场半场标识线渲染器
/// 在地面上绘制半场网球场的白色标识线，发球机位于中线位置
/// 适配场馆尺寸：宽3.5m × 长11m × 高3m
/// </summary>
public class TennisCourtLineRenderer : MonoBehaviour
{
    [Header("线条设置")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.05f; // 10cm线宽
    [SerializeField] private float lineHeight = 0.05f; // 5cm高度，确保可见性
    [SerializeField] private Color lineColor = Color.white;

    [Header("网球场尺寸 (基于reference.md规格)")]
    [SerializeField] private float courtLength = 11f; // 场地长度，从中线到底线
    [SerializeField] private float courtWidth = 3.5f; // 场地宽度，与场馆宽度一致
    [SerializeField] private float serviceLineDistance = 2.0f; // 发球线距离中线的距离
    [SerializeField] private float serviceBoxWidth = 1.75f; // 发球区宽度（场地宽度的一半）

    [Header("发球机设置")]
    [SerializeField] private bool showLauncherZone = true; // 显示发球机区域标识
    [SerializeField] private float launcherZoneSize = 0.5f; // 发球机区域标识大小

    [Header("调试设置")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool autoCreateOnStart = true;

    private List<GameObject> lineObjects = new List<GameObject>();
    private Transform floorTransform;

    void Start()
    {
        if (autoCreateOnStart)
        {
            CreateHalfCourtLines();
        }
    }

    /// <summary>
    /// 创建半场网球场标识线
    /// </summary>
    [ContextMenu("创建半场网球场标识线")]
    public void CreateHalfCourtLines()
    {
        // 清除现有线条
        ClearExistingLines();

        // 查找地面对象并获取其实际尺寸
        FindFloorObject();

        if (floorTransform == null)
        {
            Debug.LogError("TennisCourtLineRenderer: 未找到Floor对象！");
            return;
        }

        // 创建线条材质
        CreateLineMaterial();

        // 创建半场各种线条
        CreateCenterLine(); // 中线（发球机位置）
        CreateBaseLine(); // 底线
        CreateSideLines(); // 边线
        CreateServiceLine(); // 发球线
        CreateServiceBoxLine(); // 发球区中线

        if (showLauncherZone)
        {
            CreateLauncherZone(); // 发球机区域标识
        }

        if (showDebugInfo)
        {
            Debug.Log($"TennisCourtLineRenderer: 成功创建 {lineObjects.Count} 条半场网球场标识线");
            LogHalfCourtDimensions();
        }
    }

    /// <summary>
    /// 查找地面对象并获取其实际尺寸
    /// </summary>
    private void FindFloorObject()
    {
        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            floorTransform = floor.transform;

            // 获取Floor的实际尺寸
            Vector3 floorScale = floorTransform.localScale;

            // 根据reference.md规格设置场地尺寸（宽3.5m × 长11m）
            // 使用完整场地长度
            courtWidth = 3.5f; // 固定宽度3.5米
            courtLength = 11f; // 场地长度11米（从中线到底线）

            // 调整发球线距离和发球区宽度
            serviceLineDistance = 2.0f; // 发球线距离中线2米
            serviceBoxWidth = courtWidth / 2f; // 发球区宽度为场地宽度的一半

            if (showDebugInfo)
            {
                Debug.Log($"TennisCourtLineRenderer: 找到Floor对象");
                Debug.Log($"Floor位置: {floorTransform.position}");
                Debug.Log($"Floor尺寸: {floorScale}");
                Debug.Log($"场地尺寸: 宽度={courtWidth:F1}m, 长度={courtLength:F1}m");
                Debug.Log($"发球线距离: {serviceLineDistance:F1}m");
                Debug.Log($"发球区宽度: {serviceBoxWidth:F1}m");
            }
        }
        else
        {
            Debug.LogError("TennisCourtLineRenderer: 未找到Floor对象！请确保场景中有名为'Floor'的对象。");
        }
    }

    /// <summary>
    /// 创建线条材质
    /// </summary>
    private void CreateLineMaterial()
    {
        if (lineMaterial == null)
        {
            // 使用Unlit材质确保在任何光照条件下都可见
            lineMaterial = new Material(Shader.Find("Unlit/Color"));
            lineMaterial.color = lineColor;
            lineMaterial.name = "HalfCourtLineMaterial";

            // 设置渲染模式为不透明
            lineMaterial.SetFloat("_Mode", 0);
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            lineMaterial.SetInt("_ZWrite", 1);
            lineMaterial.DisableKeyword("_ALPHATEST_ON");
            lineMaterial.DisableKeyword("_ALPHABLEND_ON");
            lineMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            lineMaterial.renderQueue = -1;

            // 增加亮度，确保在暗环境下也可见
            lineMaterial.color = new Color(lineColor.r * 1.5f, lineColor.g * 1.5f, lineColor.b * 1.5f, lineColor.a);

            if (showDebugInfo)
            {
                Debug.Log("TennisCourtLineRenderer: 创建了增强亮度的Unlit材质，确保线条在任何光照下都清晰可见");
            }
        }
    }

    /// <summary>
    /// 创建中线（发球机位置线）
    /// </summary>
    private void CreateCenterLine()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 中线位置（发球机位置）
        Vector3 centerLinePos = new Vector3(0, floorY + lineHeight, floorZ);
        CreateLine("中线_发球机位置", centerLinePos, courtWidth + 0.4f, lineWidth, 0f); // 稍微超出边线以示重要性
    }

    /// <summary>
    /// 创建底线（接球端）
    /// </summary>
    private void CreateBaseLine()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 底线位置（接球端，距离中线courtLength距离）
        Vector3 baseLinePos = new Vector3(0, floorY + lineHeight, floorZ + courtLength);
        CreateLine("底线", baseLinePos, courtWidth, lineWidth, 0f);
    }

    /// <summary>
    /// 创建边线
    /// </summary>
    private void CreateSideLines()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 左边线（从中线到底线）
        Vector3 leftSideLinePos = new Vector3(-courtWidth / 2, floorY + lineHeight, floorZ + courtLength / 2);
        CreateLine("左边线", leftSideLinePos, lineWidth, courtLength, 90f);

        // 右边线（从中线到底线）
        Vector3 rightSideLinePos = new Vector3(courtWidth / 2, floorY + lineHeight, floorZ + courtLength / 2);
        CreateLine("右边线", rightSideLinePos, lineWidth, courtLength, 90f);
    }

    /// <summary>
    /// 创建发球线
    /// </summary>
    private void CreateServiceLine()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 发球线（距离中线serviceLineDistance距离）
        Vector3 serviceLinePos = new Vector3(0, floorY + lineHeight, floorZ + serviceLineDistance);
        CreateLine("发球线", serviceLinePos, courtWidth, lineWidth, 0f);
    }

    /// <summary>
    /// 创建发球区中线
    /// </summary>
    private void CreateServiceBoxLine()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 发球区中线（从中线到发球线，垂直分割发球区）
        Vector3 serviceBoxLinePos = new Vector3(0, floorY + lineHeight, floorZ + serviceLineDistance / 2);
        CreateLine("发球区中线", serviceBoxLinePos, lineWidth, serviceLineDistance, 90f);
    }

    /// <summary>
    /// 创建发球机区域标识
    /// </summary>
    private void CreateLauncherZone()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 发球机区域标识（在中线位置创建一个小方框）
        Vector3 launcherZonePos = new Vector3(0, floorY + lineHeight + 0.01f, floorZ - 0.2f); // 稍微向后偏移

        // 创建发球机区域的四条边
        CreateLine("发球机区域_前", new Vector3(0, launcherZonePos.y, launcherZonePos.z + launcherZoneSize/2),
                   launcherZoneSize, lineWidth, 0f);
        CreateLine("发球机区域_后", new Vector3(0, launcherZonePos.y, launcherZonePos.z - launcherZoneSize/2),
                   launcherZoneSize, lineWidth, 0f);
        CreateLine("发球机区域_左", new Vector3(-launcherZoneSize/2, launcherZonePos.y, launcherZonePos.z),
                   lineWidth, launcherZoneSize, 90f);
        CreateLine("发球机区域_右", new Vector3(launcherZoneSize/2, launcherZonePos.y, launcherZonePos.z),
                   lineWidth, launcherZoneSize, 90f);
    }

    /// <summary>
    /// 创建单条线
    /// </summary>
    private void CreateLine(string lineName, Vector3 position, float width, float length, float rotationY)
    {
        GameObject lineObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lineObj.name = $"HalfCourtLine_{lineName}";

        // 设置父对象为Floor，这样线条会跟随Floor移动
        lineObj.transform.SetParent(floorTransform);

        // 直接使用世界坐标，然后转换为本地坐标
        Vector3 worldPosition = position;
        // 确保线条在Floor表面上方足够的高度
        worldPosition.y = floorTransform.position.y + (floorTransform.localScale.y * 0.5f) + lineHeight;

        // 转换为本地坐标
        Vector3 localPosition = floorTransform.InverseTransformPoint(worldPosition);

        lineObj.transform.localPosition = localPosition;
        lineObj.transform.localRotation = Quaternion.Euler(0, rotationY, 0);

        // 计算相对于Floor缩放的本地缩放
        Vector3 localScale = new Vector3(
            width / floorTransform.localScale.x,
            (lineHeight * 2) / floorTransform.localScale.y,
            length / floorTransform.localScale.z
        );
        lineObj.transform.localScale = localScale;

        // 设置材质
        Renderer renderer = lineObj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = lineMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // 不投射阴影
            renderer.receiveShadows = false; // 不接收阴影
        }

        // 移除碰撞器（线条不需要物理碰撞）
        Collider collider = lineObj.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }

        // 添加到列表
        lineObjects.Add(lineObj);

        if (showDebugInfo)
        {
            Vector3 finalWorldPos = lineObj.transform.position;
            Debug.Log($"TennisCourtLineRenderer: 创建线条 '{lineName}' - 最终世界位置: ({finalWorldPos.x:F2}, {finalWorldPos.y:F2}, {finalWorldPos.z:F2})");
        }
    }

    /// <summary>
    /// 清除现有线条
    /// </summary>
    [ContextMenu("清除所有线条")]
    public void ClearExistingLines()
    {
        foreach (GameObject lineObj in lineObjects)
        {
            if (lineObj != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(lineObj);
                }
                else
                {
                    DestroyImmediate(lineObj);
                }
            }
        }
        lineObjects.Clear();

        // 也清除可能存在的旧线条
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != this.transform && (child.name.StartsWith("TennisCourtLine_") || child.name.StartsWith("HalfCourtLine_")))
            {
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        if (showDebugInfo)
        {
            Debug.Log("TennisCourtLineRenderer: 已清除所有现有线条");
        }
    }

    /// <summary>
    /// 输出场地尺寸信息
    /// </summary>
    private void LogHalfCourtDimensions()
    {
        Debug.Log($"=== 网球场尺寸信息 ===");
        Debug.Log($"场地宽度: {courtWidth}m (与场馆宽度一致)");
        Debug.Log($"场地长度: {courtLength}m (从中线到底线)");
        Debug.Log($"发球线距离: {serviceLineDistance}m (距离中线)");
        Debug.Log($"发球区宽度: {serviceBoxWidth}m");
        Debug.Log($"线条宽度: {lineWidth}m (10cm)");
        Debug.Log($"线条高度: {lineHeight}m");
        Debug.Log($"发球机位置: 场地中线 (Z坐标: {floorTransform.position.z})");
        Debug.Log($"地面位置: {(floorTransform != null ? floorTransform.position.ToString() : "未知")}");
    }

    /// <summary>
    /// 调整线条颜色
    /// </summary>
    public void SetLineColor(Color newColor)
    {
        lineColor = newColor;
        if (lineMaterial != null)
        {
            lineMaterial.color = new Color(newColor.r * 1.5f, newColor.g * 1.5f, newColor.b * 1.5f, newColor.a);
        }
    }

    /// <summary>
    /// 调整线条宽度
    /// </summary>
    public void SetLineWidth(float newWidth)
    {
        lineWidth = newWidth;
        // 重新创建线条以应用新宽度
        if (lineObjects.Count > 0)
        {
            CreateHalfCourtLines();
        }
    }

    /// <summary>
    /// 切换发球机区域显示
    /// </summary>
    public void ToggleLauncherZone(bool show)
    {
        showLauncherZone = show;
        CreateHalfCourtLines(); // 重新创建线条
    }

    /// <summary>
    /// 获取线条数量
    /// </summary>
    public int GetLineCount()
    {
        return lineObjects.Count;
    }

    void OnValidate()
    {
        // 确保线宽不小于1cm
        lineWidth = Mathf.Max(lineWidth, 0.01f);
        // 确保线高度合理
        lineHeight = Mathf.Max(lineHeight, 0.001f);
        // 确保场地尺寸合理
        courtWidth = Mathf.Max(courtWidth, 1f);
        courtLength = Mathf.Max(courtLength, 1f);
        serviceLineDistance = Mathf.Clamp(serviceLineDistance, 1f, courtLength - 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        if (floorTransform == null) return;

        // 绘制场地边界
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(0, floorTransform.position.y + 0.1f, floorTransform.position.z + courtLength / 2);
        Gizmos.DrawWireCube(center, new Vector3(courtWidth, 0.01f, courtLength));

        // 绘制中线位置（发球机位置）
        Gizmos.color = Color.red;
        Vector3 centerLinePos = new Vector3(0, floorTransform.position.y + 0.15f, floorTransform.position.z);
        Gizmos.DrawLine(new Vector3(-courtWidth/2 - 0.2f, centerLinePos.y, centerLinePos.z),
                       new Vector3(courtWidth/2 + 0.2f, centerLinePos.y, centerLinePos.z));

        // 绘制发球机区域
        if (showLauncherZone)
        {
            Gizmos.color = Color.blue;
            Vector3 launcherPos = new Vector3(0, floorTransform.position.y + 0.2f, floorTransform.position.z - 0.2f);
            Gizmos.DrawWireCube(launcherPos, new Vector3(launcherZoneSize, 0.01f, launcherZoneSize));
        }

        // 绘制发球线
        Gizmos.color = Color.green;
        Vector3 serviceLinePos = new Vector3(0, floorTransform.position.y + 0.12f, floorTransform.position.z + serviceLineDistance);
        Gizmos.DrawLine(new Vector3(-courtWidth/2, serviceLinePos.y, serviceLinePos.z),
                       new Vector3(courtWidth/2, serviceLinePos.y, serviceLinePos.z));
    }
}