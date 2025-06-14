using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 幕布材质更新器 - 将Curtain替换为网球场地图案
/// 根据用户需求设置厚度为0.005，应用网球场地纹理
/// </summary>
public class CurtainMaterialUpdater : MonoBehaviour
{
    [Header("幕布对象设置")]
    public GameObject curtainObject;
    public Material curtainMaterial;
    public Texture2D tennisCourtTexture;

    [Header("幕布参数")]
    [Range(0.001f, 0.1f)]
    public float curtainThickness = 0.005f; // 用户要求的厚度

    [Header("材质设置")]
    public Color curtainColor = Color.white;
    [Range(0f, 1f)]
    public float metallic = 0.1f;
    [Range(0f, 1f)]
    public float smoothness = 0.3f;
    [Range(0f, 1f)]
    public float transparency = 0.8f; // 轻微透明度

    [Header("快捷键控制")]
    public KeyCode updateMaterialKey = KeyCode.N;
    public KeyCode applyTextureKey = KeyCode.M;
    public KeyCode resetKey = KeyCode.Comma;

    [Header("调试信息")]
    public bool showDebugInfo = true;

    private Renderer curtainRenderer;
    private Material originalMaterial;
    private Vector3 originalScale;

    void Start()
    {
        InitializeCurtainMaterialUpdater();
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    /// <summary>
    /// 初始化幕布材质更新器
    /// </summary>
    void InitializeCurtainMaterialUpdater()
    {
        Debug.Log("=== 初始化幕布材质更新器 ===");

        // 查找Curtain对象
        if (curtainObject == null)
        {
            curtainObject = GameObject.Find("Curtain");
            if (curtainObject == null)
            {
                Debug.LogError("❌ 未找到Curtain对象！请确保场景中存在名为'Curtain'的GameObject");
                return;
            }
        }

        // 获取渲染器组件
        curtainRenderer = curtainObject.GetComponent<Renderer>();
        if (curtainRenderer == null)
        {
            Debug.LogError("❌ Curtain对象缺少Renderer组件！");
            return;
        }

        // 保存原始材质和缩放
        originalMaterial = curtainRenderer.material;
        originalScale = curtainObject.transform.localScale;

        // 创建网球场地纹理
        CreateTennisCourtTexture();

        // 创建新材质
        CreateCurtainMaterial();

        // 应用初始设置
        ApplyCurtainSettings();

        ShowInstructions();

        Debug.Log("✅ 幕布材质更新器初始化完成");
    }

    /// <summary>
    /// 创建网球场地纹理
    /// </summary>
    void CreateTennisCourtTexture()
    {
        if (tennisCourtTexture != null) return;

        Debug.Log("🎾 正在创建网球场地纹理...");

        // 创建512x512的纹理
        int width = 512;
        int height = 512;
        tennisCourtTexture = new Texture2D(width, height);

        // 基础场地颜色 - 蓝色
        Color courtBlue = new Color(0.2f, 0.4f, 0.8f, 1f);
        Color lineWhite = Color.white;

        // 填充基础颜色
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tennisCourtTexture.SetPixel(x, y, courtBlue);
            }
        }

        // 绘制网球场线条
        DrawTennisCourtLines(width, height, lineWhite);

        // 添加"HeHaa"文字
        DrawHeHaaText(width, height, lineWhite);

        // 添加分数圆圈
        DrawScoreCircles(width, height, lineWhite);

        // 应用纹理
        tennisCourtTexture.Apply();

        Debug.Log("✅ 网球场地纹理创建完成");
    }

    /// <summary>
    /// 绘制网球场线条
    /// </summary>
    void DrawTennisCourtLines(int width, int height, Color lineColor)
    {
        int lineWidth = 3;

        // 中线（水平）
        int centerY = height / 2;
        for (int x = 0; x < width; x++)
        {
            for (int y = centerY - lineWidth; y < centerY + lineWidth; y++)
            {
                if (y >= 0 && y < height)
                    tennisCourtTexture.SetPixel(x, y, lineColor);
            }
        }

        // 中线（垂直）
        int centerX = width / 2;
        for (int y = 0; y < height; y++)
        {
            for (int x = centerX - lineWidth; x < centerX + lineWidth; x++)
            {
                if (x >= 0 && x < width)
                    tennisCourtTexture.SetPixel(x, y, lineColor);
            }
        }

        // 边界线
        // 顶部边界
        for (int x = 0; x < width; x++)
        {
            for (int y = height - lineWidth; y < height; y++)
            {
                tennisCourtTexture.SetPixel(x, y, lineColor);
            }
        }

        // 底部边界
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < lineWidth; y++)
            {
                tennisCourtTexture.SetPixel(x, y, lineColor);
            }
        }

        // 左边界
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < lineWidth; x++)
            {
                tennisCourtTexture.SetPixel(x, y, lineColor);
            }
        }

        // 右边界
        for (int y = 0; y < height; y++)
        {
            for (int x = width - lineWidth; x < width; x++)
            {
                tennisCourtTexture.SetPixel(x, y, lineColor);
            }
        }
    }

    /// <summary>
    /// 绘制HeHaa文字
    /// </summary>
    void DrawHeHaaText(int width, int height, Color textColor)
    {
        // 在上半部分绘制HeHaa文字（简单像素绘制）
        int textY = height * 3 / 4;
        int textCenterX = width / 2;

        // 简单的像素字体绘制 "HeHaa"
        // 这里使用简化的方法，实际可以使用更复杂的字体渲染
        for (int x = textCenterX - 50; x < textCenterX + 50; x++)
        {
            for (int y = textY - 10; y < textY + 10; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    // 简单的文字模式
                    if ((x - textCenterX) % 10 < 5 && (y - textY) % 4 < 2)
                        tennisCourtTexture.SetPixel(x, y, textColor);
                }
            }
        }
    }

    /// <summary>
    /// 绘制分数圆圈
    /// </summary>
    void DrawScoreCircles(int width, int height, Color circleColor)
    {
        // 绘制分数圆圈：20, 20, 50, 30, 50
        int[] scores = { 20, 20, 50, 30, 50 };
        Vector2[] positions = {
            new Vector2(0.2f, 0.75f),  // 左上 20
            new Vector2(0.8f, 0.75f),  // 右上 20
            new Vector2(0.2f, 0.25f),  // 左下 50
            new Vector2(0.5f, 0.25f),  // 中下 30
            new Vector2(0.8f, 0.25f)   // 右下 50
        };

        for (int i = 0; i < scores.Length; i++)
        {
            int centerX = (int)(positions[i].x * width);
            int centerY = (int)(positions[i].y * height);
            int radius = 25;

            // 绘制圆圈
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                        if (distance <= radius && distance >= radius - 3)
                        {
                            tennisCourtTexture.SetPixel(x, y, circleColor);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 创建幕布材质
    /// </summary>
    void CreateCurtainMaterial()
    {
        curtainMaterial = new Material(Shader.Find("Standard"));
        curtainMaterial.name = "TennisCourtCurtainMaterial";

        // 设置基础属性
        curtainMaterial.color = curtainColor;
        curtainMaterial.SetFloat("_Metallic", metallic);
        curtainMaterial.SetFloat("_Glossiness", smoothness);

        // 应用纹理
        if (tennisCourtTexture != null)
        {
            curtainMaterial.mainTexture = tennisCourtTexture;
        }

        // 设置透明度
        if (transparency < 1f)
        {
            curtainMaterial.SetFloat("_Mode", 3); // Transparent
            curtainMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            curtainMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            curtainMaterial.SetInt("_ZWrite", 0);
            curtainMaterial.DisableKeyword("_ALPHATEST_ON");
            curtainMaterial.EnableKeyword("_ALPHABLEND_ON");
            curtainMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            curtainMaterial.renderQueue = 3000;

            Color color = curtainMaterial.color;
            color.a = transparency;
            curtainMaterial.color = color;
        }

        Debug.Log("✅ 网球场地幕布材质创建完成");
    }

    /// <summary>
    /// 应用幕布设置
    /// </summary>
    void ApplyCurtainSettings()
    {
        if (curtainObject == null || curtainRenderer == null) return;

        // 应用材质
        curtainRenderer.material = curtainMaterial;

        // 调整厚度 - 修改Z轴缩放
        Vector3 newScale = originalScale;
        newScale.z = curtainThickness;
        curtainObject.transform.localScale = newScale;

        // 确保有碰撞器用于网球反弹
        EnsureCurtainCollider();

        // 添加或更新幕布反弹脚本
        AddCurtainBehavior();

        Debug.Log($"✅ 幕布设置已应用 - 厚度: {curtainThickness}");
        if (showDebugInfo)
        {
            Debug.Log($"   位置: {curtainObject.transform.position}");
            Debug.Log($"   缩放: {curtainObject.transform.localScale}");
            Debug.Log($"   材质: {curtainMaterial.name}");
        }
    }

    /// <summary>
    /// 确保幕布有碰撞器
    /// </summary>
    void EnsureCurtainCollider()
    {
        Collider curtainCollider = curtainObject.GetComponent<Collider>();
        if (curtainCollider == null)
        {
            curtainCollider = curtainObject.AddComponent<BoxCollider>();
            Debug.Log("✅ 已添加BoxCollider到幕布");
        }

        // 如果是BoxCollider，确保大小匹配
        if (curtainCollider is BoxCollider boxCollider)
        {
            boxCollider.size = Vector3.one; // 标准大小，通过transform.scale控制
        }
    }

    /// <summary>
    /// 添加幕布反弹行为
    /// </summary>
    void AddCurtainBehavior()
    {
        CurtainBehavior curtainBehavior = curtainObject.GetComponent<CurtainBehavior>();
        if (curtainBehavior == null)
        {
            curtainBehavior = curtainObject.AddComponent<CurtainBehavior>();
            Debug.Log("✅ 已添加CurtainBehavior脚本");
        }
    }

    /// <summary>
    /// 处理键盘输入
    /// </summary>
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(updateMaterialKey))
        {
            UpdateCurtainMaterial();
        }

        if (Input.GetKeyDown(applyTextureKey))
        {
            ApplyCurtainSettings();
        }

        if (Input.GetKeyDown(resetKey))
        {
            ResetCurtain();
        }
    }

    /// <summary>
    /// 更新幕布材质
    /// </summary>
    public void UpdateCurtainMaterial()
    {
        CreateCurtainMaterial();
        ApplyCurtainSettings();
        Debug.Log("🎾 幕布材质已更新");
    }

    /// <summary>
    /// 重置幕布
    /// </summary>
    public void ResetCurtain()
    {
        if (curtainObject != null && curtainRenderer != null && originalMaterial != null)
        {
            curtainRenderer.material = originalMaterial;
            curtainObject.transform.localScale = originalScale;
            Debug.Log("🔄 幕布已重置为原始状态");
        }
    }

    /// <summary>
    /// 保存材质到文件
    /// </summary>
    public void SaveMaterialToAssets()
    {
        if (curtainMaterial == null || tennisCourtTexture == null) return;

        string materialsPath = "Assets/Materials";
        if (!Directory.Exists(materialsPath))
        {
            Directory.CreateDirectory(materialsPath);
        }

        // 保存纹理
        byte[] textureBytes = tennisCourtTexture.EncodeToPNG();
        string texturePath = Path.Combine(materialsPath, "TennisCourtCurtainTexture.png");
        File.WriteAllBytes(texturePath, textureBytes);

        Debug.Log($"✅ 材质和纹理已保存到 {materialsPath}");
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void ShowInstructions()
    {
        Debug.Log("=== 幕布材质更新器使用说明 ===");
        Debug.Log($"📄 {updateMaterialKey}键 - 更新幕布材质");
        Debug.Log($"🎾 {applyTextureKey}键 - 应用网球场地纹理");
        Debug.Log($"🔄 {resetKey}键 - 重置幕布");
        Debug.Log("💡 可在Inspector中调整厚度和材质参数");
    }

    /// <summary>
    /// 获取幕布信息
    /// </summary>
    public string GetCurtainInfo()
    {
        if (curtainObject == null) return "幕布对象未找到";

        string info = $"幕布信息:\n";
        info += $"- 位置: {curtainObject.transform.position}\n";
        info += $"- 缩放: {curtainObject.transform.localScale}\n";
        info += $"- 厚度: {curtainThickness}\n";
        info += $"- 材质: {(curtainMaterial != null ? curtainMaterial.name : "无")}\n";
        info += $"- 纹理: {(tennisCourtTexture != null ? "已应用" : "未应用")}";

        return info;
    }
}

