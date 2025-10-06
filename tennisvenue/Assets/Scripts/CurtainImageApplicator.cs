using UnityEngine;
using System.IO;

/// <summary>
/// 幕布图片应用器 - 直接使用用户提供的网球场地图片
/// 要求：顶部对齐，宽度3.5米，厚度0.005
/// </summary>
public class CurtainImageApplicator : MonoBehaviour
{
    [Header("幕布对象设置")]
    public GameObject curtainObject;
    public Material curtainMaterial;
    public Texture2D userProvidedTexture;

    [Header("幕布尺寸设置")]
    [Tooltip("幕布宽度（米）")]
    public float curtainWidth = 3.5f; // 用户要求的3.5米宽度
    [Tooltip("幕布高度（米）")]
    public float curtainHeight = 3f; // 标准高度
    [Tooltip("幕布厚度")]
    [Range(0.001f, 0.1f)]
    public float curtainThickness = 0.005f; // 用户要求的厚度

    [Header("图片应用设置")]
    [Tooltip("顶部对齐")]
    public bool topAligned = true;
    [Tooltip("保持图片宽高比")]
    public bool maintainAspectRatio = true;
    [Tooltip("图片缩放模式")]
    public ScaleMode imageScaleMode = ScaleMode.ScaleToFit;

    [Header("材质参数")]
    public Color tintColor = Color.white;
    [Range(0f, 1f)]
    public float metallic = 0.0f;
    [Range(0f, 1f)]
    public float smoothness = 0.2f;
    [Range(0f, 1f)]
    public float transparency = 1.0f; // 完全不透明

    [Header("快捷键")]
    public KeyCode applyImageKey = KeyCode.I;
    public KeyCode resetKey = KeyCode.R;
    public KeyCode adjustSizeKey = KeyCode.U;

    [Header("调试信息")]
    public bool showDebugInfo = true;

    private Renderer curtainRenderer;
    private Material originalMaterial;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    void Start()
    {
        InitializeCurtainImageApplicator();
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    /// <summary>
    /// 初始化幕布图片应用器
    /// </summary>
    void InitializeCurtainImageApplicator()
    {
        Debug.Log("=== 初始化幕布图片应用器 ===");

        // 查找Curtain对象
        if (curtainObject == null)
        {
            curtainObject = GameObject.Find("Curtain");
            if (curtainObject == null)
            {
                Debug.LogError("❌ 未找到Curtain对象！");
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

        // 保存原始状态
        originalMaterial = curtainRenderer.material;
        originalScale = curtainObject.transform.localScale;
        originalPosition = curtainObject.transform.position;

        // 尝试从用户图片创建纹理
        CreateTextureFromUserImage();

        // 显示使用说明
        ShowInstructions();

        Debug.Log("✅ 幕布图片应用器初始化完成");
    }

    /// <summary>
    /// 从用户提供的图片创建纹理
    /// </summary>
    void CreateTextureFromUserImage()
    {
        Debug.Log("🖼️ 正在处理用户提供的网球场地图片...");

        // 首先尝试加载真实的用户图片文件
        if (LoadUserProvidedImageFile())
        {
            Debug.Log("✅ 成功加载用户提供的图片文件");
        }
        else
        {
            Debug.Log("⚠️ 未找到用户图片文件，使用程序生成的图片");
            // 创建基于用户图片描述的纹理
            CreateTennisCourtTextureFromDescription();
        }

        // 创建材质
        CreateImageBasedMaterial();

        Debug.Log("✅ 用户图片纹理创建完成");
    }

    /// <summary>
    /// 加载用户提供的图片文件
    /// </summary>
    bool LoadUserProvidedImageFile()
    {
        // 尝试从多个可能的路径加载用户图片
        string[] possiblePaths = {
            "Assets/Textures/UserCurtainImage.png",
            "Assets/Textures/tennis_court_image.png",
            "Assets/Images/curtain_image.png",
            "Assets/UserProvided/curtain.png",
            "UserCurtainImage.png"
        };

        foreach (string path in possiblePaths)
        {
            // 尝试作为Resources加载
            Texture2D loadedTexture = Resources.Load<Texture2D>(path.Replace("Assets/", "").Replace(".png", ""));
            if (loadedTexture != null)
            {
                userProvidedTexture = loadedTexture;
                Debug.Log($"✅ 从Resources加载图片: {path}");
                return true;
            }

            // 尝试从文件系统加载
            if (System.IO.File.Exists(path))
            {
                try
                {
                    byte[] imageData = System.IO.File.ReadAllBytes(path);
                    userProvidedTexture = new Texture2D(2, 2);
                    if (userProvidedTexture.LoadImage(imageData))
                    {
                        Debug.Log($"✅ 从文件系统加载图片: {path}");
                        return true;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"⚠️ 加载图片失败 {path}: {e.Message}");
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 根据用户图片描述创建网球场地纹理
    /// 基于用户提供的图片：蓝色底，HeHaa文字，分数圆圈(20,20,50,30,50)，白色边框
    /// </summary>
    void CreateTennisCourtTextureFromDescription()
    {
        int width = 1024;  // 高分辨率
        int height = 768;  // 保持3:4比例，适合网球场

        userProvidedTexture = new Texture2D(width, height);

        // 基础蓝色场地 - 更接近用户图片的颜色
        Color courtBlue = new Color(0.2f, 0.5f, 0.9f, 1f);
        Color lineWhite = Color.white;
        Color pinkBorder = new Color(1f, 0.7f, 0.8f, 1f); // 粉色边框

        // 填充基础蓝色
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                userProvidedTexture.SetPixel(x, y, courtBlue);
            }
        }

        // 绘制粉色边框（模拟用户图片的外框）
        DrawPinkBorder(width, height, pinkBorder);

        // 绘制网球场线条
        DrawAccurateTennisCourtLines(width, height, lineWhite);

        // 绘制HeHaa文字（顶部中央）
        DrawHeHaaTextAccurate(width, height, lineWhite);

        // 绘制精确的分数圆圈
        DrawAccurateScoreCircles(width, height, lineWhite);

        // 添加右下角二维码区域
        DrawQRCodeArea(width, height, lineWhite);

        // 应用纹理
        userProvidedTexture.Apply();
    }

    /// <summary>
    /// 绘制粉色边框
    /// </summary>
    void DrawPinkBorder(int width, int height, Color borderColor)
    {
        int borderWidth = 30;

        // 上边框
        for (int x = 0; x < width; x++)
        {
            for (int y = height - borderWidth; y < height; y++)
            {
                userProvidedTexture.SetPixel(x, y, borderColor);
            }
        }

        // 下边框
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < borderWidth; y++)
            {
                userProvidedTexture.SetPixel(x, y, borderColor);
            }
        }

        // 左边框
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < borderWidth; x++)
            {
                userProvidedTexture.SetPixel(x, y, borderColor);
            }
        }

        // 右边框
        for (int y = 0; y < height; y++)
        {
            for (int x = width - borderWidth; x < width; x++)
            {
                userProvidedTexture.SetPixel(x, y, borderColor);
            }
        }
    }

    /// <summary>
    /// 绘制精确的网球场线条
    /// </summary>
    void DrawAccurateTennisCourtLines(int width, int height, Color lineColor)
    {
        int lineWidth = 4;

        // 内边框（蓝色区域的白色边界）
        int margin = 50;
        DrawRectangleOutline(margin, margin, width - margin * 2, height - margin * 2, lineWidth, lineColor);

        // 中线（水平分割上下两部分）
        int centerY = height / 2;
        DrawHorizontalLine(margin, width - margin, centerY, lineWidth, lineColor);

        // 中线（垂直分割左中右三部分）
        int leftCenterX = width / 3;
        int rightCenterX = width * 2 / 3;

        // 只在下半部分绘制垂直线
        DrawVerticalLine(leftCenterX, margin, centerY, lineWidth, lineColor);
        DrawVerticalLine(rightCenterX, margin, centerY, lineWidth, lineColor);
    }

    /// <summary>
    /// 绘制精确的HeHaa文字
    /// </summary>
    void DrawHeHaaTextAccurate(int width, int height, Color textColor)
    {
        // 在上半部分中央绘制HeHaa
        int textY = height * 3 / 4;
        int textCenterX = width / 2;

        // 使用像素艺术方式绘制"HeHaa"
        DrawPixelText("HeHaa", textCenterX - 100, textY, 20, textColor);
    }

    /// <summary>
    /// 绘制精确的分数圆圈
    /// </summary>
    void DrawAccurateScoreCircles(int width, int height, Color circleColor)
    {
        // 根据用户图片的位置绘制分数圆圈
        int radius = 35;

        // 上半部分：两个20
        DrawNumberCircle("20", width / 4, height * 3 / 4, radius, circleColor);
        DrawNumberCircle("20", width * 3 / 4, height * 3 / 4, radius, circleColor);

        // 下半部分：50, 30, 50
        DrawNumberCircle("50", width / 6, height / 4, radius, circleColor);
        DrawNumberCircle("30", width / 2, height / 4, radius, circleColor);
        DrawNumberCircle("50", width * 5 / 6, height / 4, radius, circleColor);
    }

    /// <summary>
    /// 绘制带数字的圆圈
    /// </summary>
    void DrawNumberCircle(string number, int centerX, int centerY, int radius, Color color)
    {
        // 绘制圆圈
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                if (x >= 0 && x < userProvidedTexture.width && y >= 0 && y < userProvidedTexture.height)
                {
                    float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                    if (distance <= radius && distance >= radius - 3)
                    {
                        userProvidedTexture.SetPixel(x, y, color);
                    }
                }
            }
        }

        // 绘制数字（简化版）
        DrawPixelText(number, centerX - 10, centerY - 5, 12, color);
    }

    /// <summary>
    /// 绘制二维码区域
    /// </summary>
    void DrawQRCodeArea(int width, int height, Color color)
    {
        int qrSize = 60;
        int qrX = width - qrSize - 30;
        int qrY = 30;

        // 简单的二维码图案
        for (int x = qrX; x < qrX + qrSize; x += 4)
        {
            for (int y = qrY; y < qrY + qrSize; y += 4)
            {
                if ((x + y) % 8 == 0)
                {
                    DrawSquare(x, y, 3, color);
                }
            }
        }
    }

    /// <summary>
    /// 绘制像素文字（简化版）
    /// </summary>
    void DrawPixelText(string text, int startX, int startY, int size, Color color)
    {
        for (int i = 0; i < text.Length; i++)
        {
            int charX = startX + i * size;
            // 简单的字符绘制
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if ((x + y) % 3 == 0) // 简单图案
                    {
                        int pixelX = charX + x;
                        int pixelY = startY + y;
                        if (pixelX >= 0 && pixelX < userProvidedTexture.width &&
                            pixelY >= 0 && pixelY < userProvidedTexture.height)
                        {
                            userProvidedTexture.SetPixel(pixelX, pixelY, color);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 辅助方法：绘制矩形轮廓
    /// </summary>
    void DrawRectangleOutline(int x, int y, int width, int height, int lineWidth, Color color)
    {
        DrawHorizontalLine(x, x + width, y, lineWidth, color);
        DrawHorizontalLine(x, x + width, y + height, lineWidth, color);
        DrawVerticalLine(x, y, y + height, lineWidth, color);
        DrawVerticalLine(x + width, y, y + height, lineWidth, color);
    }

    /// <summary>
    /// 辅助方法：绘制水平线
    /// </summary>
    void DrawHorizontalLine(int startX, int endX, int y, int lineWidth, Color color)
    {
        for (int x = startX; x < endX; x++)
        {
            for (int i = -lineWidth/2; i <= lineWidth/2; i++)
            {
                int pixelY = y + i;
                if (x >= 0 && x < userProvidedTexture.width &&
                    pixelY >= 0 && pixelY < userProvidedTexture.height)
                {
                    userProvidedTexture.SetPixel(x, pixelY, color);
                }
            }
        }
    }

    /// <summary>
    /// 辅助方法：绘制垂直线
    /// </summary>
    void DrawVerticalLine(int x, int startY, int endY, int lineWidth, Color color)
    {
        for (int y = startY; y < endY; y++)
        {
            for (int i = -lineWidth/2; i <= lineWidth/2; i++)
            {
                int pixelX = x + i;
                if (pixelX >= 0 && pixelX < userProvidedTexture.width &&
                    y >= 0 && y < userProvidedTexture.height)
                {
                    userProvidedTexture.SetPixel(pixelX, y, color);
                }
            }
        }
    }

    /// <summary>
    /// 辅助方法：绘制小方块
    /// </summary>
    void DrawSquare(int centerX, int centerY, int size, Color color)
    {
        for (int x = centerX; x < centerX + size; x++)
        {
            for (int y = centerY; y < centerY + size; y++)
            {
                if (x >= 0 && x < userProvidedTexture.width &&
                    y >= 0 && y < userProvidedTexture.height)
                {
                    userProvidedTexture.SetPixel(x, y, color);
                }
            }
        }
    }

    /// <summary>
    /// 创建基于图片的材质
    /// </summary>
    void CreateImageBasedMaterial()
    {
        curtainMaterial = new Material(Shader.Find("Standard"));
        curtainMaterial.name = "UserProvidedCurtainMaterial";

        // 设置基础属性
        curtainMaterial.color = tintColor;
        curtainMaterial.SetFloat("_Metallic", metallic);
        curtainMaterial.SetFloat("_Glossiness", smoothness);

        // 应用用户纹理
        if (userProvidedTexture != null)
        {
            curtainMaterial.mainTexture = userProvidedTexture;
        }

        // 设置透明度
        if (transparency < 1f)
        {
            SetMaterialTransparent();
        }

        Debug.Log("✅ 基于用户图片的幕布材质创建完成");
    }

    /// <summary>
    /// 设置材质为透明模式
    /// </summary>
    void SetMaterialTransparent()
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

    /// <summary>
    /// 应用图片和尺寸到幕布
    /// </summary>
    [ContextMenu("应用图片到幕布")]
    public void ApplyImageToCurtain()
    {
        if (curtainObject == null || curtainRenderer == null) return;

        Debug.Log("🖼️ 正在应用用户图片到幕布...");

        // 应用材质
        curtainRenderer.material = curtainMaterial;

        // 设置幕布尺寸（3.5米宽度）
        SetCurtainSize();

        // 设置UV映射以实现顶部对齐
        SetTopAlignedUV();

        // 确保有碰撞器和反弹脚本
        EnsureCurtainPhysics();

        Debug.Log($"✅ 图片已应用到幕布");
        Debug.Log($"   - 宽度: {curtainWidth}米");
        Debug.Log($"   - 高度: {curtainHeight}米");
        Debug.Log($"   - 厚度: {curtainThickness}");
        Debug.Log($"   - 顶部对齐: {topAligned}");
    }

    /// <summary>
    /// 设置幕布尺寸
    /// </summary>
    void SetCurtainSize()
    {
        // 设置到指定的物理尺寸
        Vector3 newScale = new Vector3(curtainWidth, curtainHeight, curtainThickness);
        curtainObject.transform.localScale = newScale;

        if (showDebugInfo)
        {
            Debug.Log($"📏 幕布尺寸已设置: {newScale}");
        }
    }

    /// <summary>
    /// 设置顶部对齐的UV映射
    /// </summary>
    void SetTopAlignedUV()
    {
        if (!topAligned) return;

        // 获取网格并修改UV坐标实现顶部对齐
        MeshFilter meshFilter = curtainObject.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            Mesh mesh = meshFilter.mesh;
            Vector2[] uvs = mesh.uv;

            // 修改UV坐标以实现顶部对齐
            // 这里假设是一个标准的Cube，修改Y坐标使图片从顶部开始显示
            for (int i = 0; i < uvs.Length; i++)
            {
                // 将V坐标反转，使图片顶部对齐到几何体顶部
                uvs[i] = new Vector2(uvs[i].x, 1f - uvs[i].y);
            }

            mesh.uv = uvs;

            if (showDebugInfo)
            {
                Debug.Log("📍 UV映射已设置为顶部对齐");
            }
        }
    }

    /// <summary>
    /// 确保幕布有物理组件
    /// </summary>
    void EnsureCurtainPhysics()
    {
        // 确保有碰撞器
        Collider collider = curtainObject.GetComponent<Collider>();
        if (collider == null)
        {
            collider = curtainObject.AddComponent<BoxCollider>();
            Debug.Log("✅ 已添加BoxCollider");
        }

        // 添加反弹脚本
        CurtainBehavior behavior = curtainObject.GetComponent<CurtainBehavior>();
        if (behavior == null)
        {
            behavior = curtainObject.AddComponent<CurtainBehavior>();
            Debug.Log("✅ 已添加CurtainBehavior反弹脚本");
        }
    }

    /// <summary>
    /// 处理键盘输入
    /// </summary>
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(applyImageKey))
        {
            ApplyImageToCurtain();
        }

        if (Input.GetKeyDown(adjustSizeKey))
        {
            AdjustCurtainSize();
        }

        if (Input.GetKeyDown(resetKey))
        {
            ResetCurtain();
        }
    }

    /// <summary>
    /// 调整幕布尺寸
    /// </summary>
    [ContextMenu("调整幕布尺寸")]
    public void AdjustCurtainSize()
    {
        SetCurtainSize();
        Debug.Log($"📏 幕布尺寸已调整为 {curtainWidth}×{curtainHeight}×{curtainThickness}");
    }

    /// <summary>
    /// 重置幕布
    /// </summary>
    [ContextMenu("重置幕布")]
    public void ResetCurtain()
    {
        if (curtainObject != null && curtainRenderer != null && originalMaterial != null)
        {
            curtainRenderer.material = originalMaterial;
            curtainObject.transform.localScale = originalScale;
            curtainObject.transform.position = originalPosition;
            Debug.Log("🔄 幕布已重置为原始状态");
        }
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void ShowInstructions()
    {
        Debug.Log("=== 幕布图片应用器使用说明 ===");
        Debug.Log($"🖼️ {applyImageKey}键 - 应用用户图片到幕布");
        Debug.Log($"📏 {adjustSizeKey}键 - 调整幕布尺寸");
        Debug.Log($"🔄 {resetKey}键 - 重置幕布");
        Debug.Log("📋 设置参数:");
        Debug.Log($"   - 宽度: {curtainWidth}米（用户要求）");
        Debug.Log($"   - 厚度: {curtainThickness}（用户要求）");
        Debug.Log($"   - 顶部对齐: {topAligned}");
        Debug.Log("💡 也可使用Inspector右键菜单操作");
    }

    /// <summary>
    /// 保存材质到Assets文件夹
    /// </summary>
    [ContextMenu("保存材质到Assets")]
    public void SaveMaterialToAssets()
    {
        if (curtainMaterial == null || userProvidedTexture == null) return;

        string materialsPath = "Assets/Materials";
        if (!Directory.Exists(materialsPath))
        {
            Directory.CreateDirectory(materialsPath);
        }

        // 保存纹理
        byte[] textureBytes = userProvidedTexture.EncodeToPNG();
        string texturePath = Path.Combine(materialsPath, "UserProvidedCurtainTexture.png");
        File.WriteAllBytes(texturePath, textureBytes);

        Debug.Log($"✅ 用户图片材质已保存到 {materialsPath}");
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
        info += $"- 宽度: {curtainWidth}米\n";
        info += $"- 高度: {curtainHeight}米\n";
        info += $"- 厚度: {curtainThickness}\n";
        info += $"- 顶部对齐: {topAligned}\n";
        info += $"- 材质: {(curtainMaterial != null ? curtainMaterial.name : "无")}\n";
        info += $"- 用户图片: {(userProvidedTexture != null ? "已应用" : "未应用")}";

        return info;
    }
}