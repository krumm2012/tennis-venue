using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 网球场标识线渲染器
/// 在地面上绘制标准网球场的白色标识线，线宽10cm
/// </summary>
public class TennisCourtLineRenderer : MonoBehaviour
{
    [Header("线条设置")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.1f; // 10cm线宽
    [SerializeField] private float lineHeight = 0.05f; // 增加到5cm高度，确保可见性
    [SerializeField] private Color lineColor = Color.white;

    [Header("网球场尺寸 (基于Floor实际尺寸)")]
    [SerializeField] private float courtLength = 7.8f; // 基于Floor的Z轴尺寸
    [SerializeField] private float courtWidth = 2.6f; // 基于Floor的X轴尺寸
    [SerializeField] private float serviceLineDistance = 2f; // 发球线距离网前的距离
    [SerializeField] private float centerServiceLineLength = 2f; // 中央发球线长度

    [Header("调试设置")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool autoCreateOnStart = true;

    private List<GameObject> lineObjects = new List<GameObject>();
    private Transform floorTransform;

    void Start()
    {
        if (autoCreateOnStart)
        {
            CreateTennisCourtLines();
        }
    }

    /// <summary>
    /// 创建网球场标识线
    /// </summary>
    [ContextMenu("创建网球场标识线")]
    public void CreateTennisCourtLines()
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

        // 创建各种线条
        CreateBaseLines(); // 底线
        CreateSideLines(); // 边线
        CreateServiceLines(); // 发球线
        CreateCenterLines(); // 中线
        CreateNetLine(); // 网线位置标记

        if (showDebugInfo)
        {
            Debug.Log($"TennisCourtLineRenderer: 成功创建 {lineObjects.Count} 条网球场标识线");
            LogCourtDimensions();
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

            // 根据Floor的实际尺寸调整场地尺寸
            courtLength = floorScale.z * 0.7f; // 使用Floor长度的70%作为场地长度
            courtWidth = floorScale.x * 0.7f;  // 使用Floor宽度的70%作为场地宽度

            // 调整发球线距离
            serviceLineDistance = courtLength * 0.25f; // 发球线距离为场地长度的25%
            centerServiceLineLength = courtWidth * 0.5f; // 中央发球线长度为场地宽度的50%

            if (showDebugInfo)
            {
                Debug.Log($"TennisCourtLineRenderer: 找到Floor对象");
                Debug.Log($"Floor位置: {floorTransform.position}");
                Debug.Log($"Floor尺寸: {floorScale}");
                Debug.Log($"调整后场地尺寸: 长度={courtLength:F2}m, 宽度={courtWidth:F2}m");
                Debug.Log($"发球线距离: {serviceLineDistance:F2}m");
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
            lineMaterial.name = "TennisCourtLineMaterial";

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
    /// 创建底线（场地两端的线）
    /// </summary>
    private void CreateBaseLines()
    {
        float floorZ = floorTransform.position.z;
        float floorY = floorTransform.position.y;

        // 后底线 (发球机一侧)
        Vector3 backBaseLinePos = new Vector3(0, floorY + lineHeight, floorZ - courtLength / 2);
        CreateLine("后底线", backBaseLinePos, courtWidth, lineWidth, 0f);

        // 前底线 (接球一侧)
        Vector3 frontBaseLinePos = new Vector3(0, floorY + lineHeight, floorZ + courtLength / 2);
        CreateLine("前底线", frontBaseLinePos, courtWidth, lineWidth, 0f);
    }

    /// <summary>
    /// 创建边线（场地两侧的线）
    /// </summary>
    private void CreateSideLines()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 左边线
        Vector3 leftSideLinePos = new Vector3(-courtWidth / 2, floorY + lineHeight, floorZ);
        CreateLine("左边线", leftSideLinePos, lineWidth, courtLength, 90f);

        // 右边线
        Vector3 rightSideLinePos = new Vector3(courtWidth / 2, floorY + lineHeight, floorZ);
        CreateLine("右边线", rightSideLinePos, lineWidth, courtLength, 90f);
    }

    /// <summary>
    /// 创建发球线
    /// </summary>
    private void CreateServiceLines()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 后发球线 (发球机一侧)
        Vector3 backServiceLinePos = new Vector3(0, floorY + lineHeight, floorZ - serviceLineDistance);
        CreateLine("后发球线", backServiceLinePos, courtWidth, lineWidth, 0f);

        // 前发球线 (接球一侧)
        Vector3 frontServiceLinePos = new Vector3(0, floorY + lineHeight, floorZ + serviceLineDistance);
        CreateLine("前发球线", frontServiceLinePos, courtWidth, lineWidth, 0f);
    }

    /// <summary>
    /// 创建中线
    /// </summary>
    private void CreateCenterLines()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 中央发球线 (从网线到发球线)
        Vector3 centerServiceLinePos = new Vector3(0, floorY + lineHeight, floorZ);
        CreateLine("中央发球线", centerServiceLinePos, lineWidth, centerServiceLineLength * 2, 90f);

        // 中线标记 (在底线内侧的短线，避免与底线重叠)
        float centerMarkOffset = 0.3f; // 距离底线30cm
        Vector3 backCenterMarkPos = new Vector3(0, floorY + lineHeight, floorZ - courtLength / 2 + centerMarkOffset);
        CreateLine("后中线标记", backCenterMarkPos, lineWidth, 0.2f, 90f);

        Vector3 frontCenterMarkPos = new Vector3(0, floorY + lineHeight, floorZ + courtLength / 2 - centerMarkOffset);
        CreateLine("前中线标记", frontCenterMarkPos, lineWidth, 0.2f, 90f);
    }

    /// <summary>
    /// 创建网线位置标记
    /// </summary>
    private void CreateNetLine()
    {
        float floorY = floorTransform.position.y;
        float floorZ = floorTransform.position.z;

        // 网线位置 (场地中央)
        Vector3 netLinePos = new Vector3(0, floorY + lineHeight, floorZ);
        CreateLine("网线标记", netLinePos, courtWidth + 0.4f, lineWidth, 0f); // 稍微超出边线
    }

    /// <summary>
    /// 创建单条线
    /// </summary>
    private void CreateLine(string lineName, Vector3 position, float width, float length, float rotationY)
    {
        GameObject lineObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lineObj.name = $"TennisCourtLine_{lineName}";

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
            Debug.Log($"TennisCourtLineRenderer: 创建线条 '{lineName}' - 最终世界位置: ({finalWorldPos.x:F2}, {finalWorldPos.y:F2}, {finalWorldPos.z:F2}), 本地位置: ({localPosition.x:F2}, {localPosition.y:F2}, {localPosition.z:F2}), 本地缩放: ({localScale.x:F2}, {localScale.y:F2}, {localScale.z:F2})");
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
            if (child != this.transform && child.name.StartsWith("TennisCourtLine_"))
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
    private void LogCourtDimensions()
    {
        Debug.Log($"=== 网球场尺寸信息 ===");
        Debug.Log($"场地长度: {courtLength}m");
        Debug.Log($"场地宽度: {courtWidth}m");
        Debug.Log($"发球线距离: {serviceLineDistance}m");
        Debug.Log($"线条宽度: {lineWidth}m (10cm)");
        Debug.Log($"线条高度: {lineHeight}m");
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
            lineMaterial.color = lineColor;
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
            CreateTennisCourtLines();
        }
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
    }

    void OnDrawGizmosSelected()
    {
        if (floorTransform == null) return;

        // 绘制场地边界
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(0, floorTransform.position.y + 0.1f, floorTransform.position.z);
        Gizmos.DrawWireCube(center, new Vector3(courtWidth, 0.01f, courtLength));

        // 绘制网线位置
        Gizmos.color = Color.red;
        Vector3 netPos = new Vector3(0, floorTransform.position.y + 0.15f, floorTransform.position.z);
        Gizmos.DrawLine(new Vector3(-courtWidth/2 - 0.2f, netPos.y, netPos.z),
                       new Vector3(courtWidth/2 + 0.2f, netPos.y, netPos.z));
    }
}