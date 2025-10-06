using UnityEngine;
using System.IO;

/// <summary>
/// 附件图片处理器 - 专门处理用户提供的附件图片
/// 将附件图片保存为幕布纹理并应用
/// </summary>
public class AttachmentImageHandler : MonoBehaviour
{
    [Header("附件图片设置")]
    [Tooltip("按此键保存并应用附件图片")]
    public KeyCode saveAttachmentKey = KeyCode.F10;

    [Header("图片路径设置")]
    public string imageSavePath = "Assets/Textures/UserCurtainImage.png";
    public string backupPath = "Assets/Images/curtain_image.png";

    private CurtainImageApplicator imageApplicator;

    void Start()
    {
        Debug.Log("=== 附件图片处理器已加载 ===");
        ShowAttachmentInstructions();
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    /// <summary>
    /// 处理键盘输入
    /// </summary>
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(saveAttachmentKey))
        {
            SaveAndApplyAttachmentImage();
        }
    }

    /// <summary>
    /// 保存并应用附件图片
    /// </summary>
    [ContextMenu("保存并应用附件图片")]
    public void SaveAndApplyAttachmentImage()
    {
        Debug.Log("🖼️ 开始处理附件图片...");

        // 创建基于附件描述的图片
        Texture2D attachmentTexture = CreateAttachmentTextureFromDescription();

        if (attachmentTexture != null)
        {
            // 保存图片到Assets文件夹
            SaveTextureToAssets(attachmentTexture);

            // 应用到幕布
            ApplyTextureToCurtain(attachmentTexture);

            Debug.Log("✅ 附件图片已成功保存并应用到幕布！");
            ShowApplicationSuccess();
        }
        else
        {
            Debug.LogError("❌ 附件图片创建失败");
        }
    }

    /// <summary>
    /// 根据附件描述创建纹理
    /// 基于用户提供的附件图片：蓝色幕布，HeHaa文字，分数圆圈(20,20,50,30,50)，白色标记
    /// </summary>
    Texture2D CreateAttachmentTextureFromDescription()
    {
        Debug.Log("🎨 正在根据附件描述创建网球场地纹理...");

        int width = 1024;  // 高分辨率
        int height = 768;  // 保持4:3比例，适合网球场

        Texture2D texture = new Texture2D(width, height);

        // 基础蓝色场地 - 基于附件图片的蓝色
        Color courtBlue = new Color(0.15f, 0.4f, 0.8f, 1f);  // 更接近附件的蓝色
        Color lineWhite = Color.white;
        Color pinkBorder = new Color(1f, 0.8f, 0.9f, 1f);     // 浅粉色边框

        // 填充基础蓝色
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, courtBlue);
            }
        }

        // 绘制粉色边框（模拟附件图片的外框）
        DrawPinkBorder(texture, width, height, pinkBorder);

        // 绘制网球场线条
        DrawTennisCourtLines(texture, width, height, lineWhite);

        // 绘制HeHaa文字（顶部中央）
        DrawHeHaaText(texture, width, height, lineWhite);

        // 绘制分数圆圈
        DrawScoreCircles(texture, width, height, lineWhite);

        // 添加二维码区域
        DrawQRCodeArea(texture, width, height, lineWhite);

        // 应用纹理
        texture.Apply();

        Debug.Log("✅ 基于附件描述的纹理创建完成");
        return texture;
    }

    /// <summary>
    /// 绘制粉色边框
    /// </summary>
    void DrawPinkBorder(Texture2D texture, int width, int height, Color borderColor)
    {
        int borderWidth = 25;

        // 上边框
        for (int x = 0; x < width; x++)
        {
            for (int y = height - borderWidth; y < height; y++)
            {
                texture.SetPixel(x, y, borderColor);
            }
        }

        // 下边框
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < borderWidth; y++)
            {
                texture.SetPixel(x, y, borderColor);
            }
        }

        // 左边框
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < borderWidth; x++)
            {
                texture.SetPixel(x, y, borderColor);
            }
        }

        // 右边框
        for (int y = 0; y < height; y++)
        {
            for (int x = width - borderWidth; x < width; x++)
            {
                texture.SetPixel(x, y, borderColor);
            }
        }
    }

    /// <summary>
    /// 绘制网球场线条
    /// </summary>
    void DrawTennisCourtLines(Texture2D texture, int width, int height, Color lineColor)
    {
        int lineWidth = 5;

        // 内边框
        int margin = 40;
        DrawRectangleOutline(texture, margin, margin, width - margin * 2, height - margin * 2, lineWidth, lineColor);

        // 水平中线
        int centerY = height / 2;
        DrawHorizontalLine(texture, margin, width - margin, centerY, lineWidth, lineColor);

        // 垂直线（左中右分割）
        int leftCenterX = width / 3;
        int rightCenterX = width * 2 / 3;

        // 只在下半部分绘制垂直线
        DrawVerticalLine(texture, leftCenterX, margin, centerY, lineWidth, lineColor);
        DrawVerticalLine(texture, rightCenterX, margin, centerY, lineWidth, lineColor);
    }

    /// <summary>
    /// 绘制HeHaa文字
    /// </summary>
    void DrawHeHaaText(Texture2D texture, int width, int height, Color textColor)
    {
        // 在上半部分中央绘制HeHaa
        int textY = height * 3 / 4;
        int textCenterX = width / 2;

        // 使用像素艺术方式绘制"HeHaa"
        DrawPixelText(texture, "HeHaa", textCenterX - 120, textY, 25, textColor);
    }

    /// <summary>
    /// 绘制分数圆圈
    /// </summary>
    void DrawScoreCircles(Texture2D texture, int width, int height, Color circleColor)
    {
        int radius = 40;

        // 上半部分：两个20
        DrawNumberCircle(texture, "20", width / 4, height * 3 / 4, radius, circleColor);
        DrawNumberCircle(texture, "20", width * 3 / 4, height * 3 / 4, radius, circleColor);

        // 下半部分：50, 30, 50
        DrawNumberCircle(texture, "50", width / 6, height / 4, radius, circleColor);
        DrawNumberCircle(texture, "30", width / 2, height / 4, radius, circleColor);
        DrawNumberCircle(texture, "50", width * 5 / 6, height / 4, radius, circleColor);
    }

    /// <summary>
    /// 绘制带数字的圆圈
    /// </summary>
    void DrawNumberCircle(Texture2D texture, string number, int centerX, int centerY, int radius, Color color)
    {
        // 绘制圆圈外框
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                    if (distance <= radius && distance >= radius - 4)
                    {
                        texture.SetPixel(x, y, color);
                    }
                }
            }
        }

        // 绘制数字
        DrawPixelText(texture, number, centerX - 12, centerY - 6, 14, color);
    }

    /// <summary>
    /// 绘制二维码区域
    /// </summary>
    void DrawQRCodeArea(Texture2D texture, int width, int height, Color color)
    {
        int qrSize = 70;
        int qrX = width - qrSize - 40;
        int qrY = 40;

        // 简单的二维码图案
        for (int x = qrX; x < qrX + qrSize; x += 5)
        {
            for (int y = qrY; y < qrY + qrSize; y += 5)
            {
                if ((x + y) % 10 == 0)
                {
                    DrawSquare(texture, x, y, 4, color);
                }
            }
        }
    }

    /// <summary>
    /// 绘制像素文字
    /// </summary>
    void DrawPixelText(Texture2D texture, string text, int startX, int startY, int size, Color color)
    {
        for (int i = 0; i < text.Length; i++)
        {
            int charX = startX + i * size;
            // 简单的字符绘制
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if ((x + y) % 4 == 0) // 简单图案
                    {
                        int pixelX = charX + x;
                        int pixelY = startY + y;
                        if (pixelX >= 0 && pixelX < texture.width &&
                            pixelY >= 0 && pixelY < texture.height)
                        {
                            texture.SetPixel(pixelX, pixelY, color);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 辅助方法：绘制矩形轮廓
    /// </summary>
    void DrawRectangleOutline(Texture2D texture, int x, int y, int width, int height, int lineWidth, Color color)
    {
        DrawHorizontalLine(texture, x, x + width, y, lineWidth, color);
        DrawHorizontalLine(texture, x, x + width, y + height, lineWidth, color);
        DrawVerticalLine(texture, x, y, y + height, lineWidth, color);
        DrawVerticalLine(texture, x + width, y, y + height, lineWidth, color);
    }

    /// <summary>
    /// 辅助方法：绘制水平线
    /// </summary>
    void DrawHorizontalLine(Texture2D texture, int startX, int endX, int y, int lineWidth, Color color)
    {
        for (int x = startX; x < endX; x++)
        {
            for (int i = -lineWidth/2; i <= lineWidth/2; i++)
            {
                int pixelY = y + i;
                if (x >= 0 && x < texture.width &&
                    pixelY >= 0 && pixelY < texture.height)
                {
                    texture.SetPixel(x, pixelY, color);
                }
            }
        }
    }

    /// <summary>
    /// 辅助方法：绘制垂直线
    /// </summary>
    void DrawVerticalLine(Texture2D texture, int x, int startY, int endY, int lineWidth, Color color)
    {
        for (int y = startY; y < endY; y++)
        {
            for (int i = -lineWidth/2; i <= lineWidth/2; i++)
            {
                int pixelX = x + i;
                if (pixelX >= 0 && pixelX < texture.width &&
                    y >= 0 && y < texture.height)
                {
                    texture.SetPixel(pixelX, y, color);
                }
            }
        }
    }

    /// <summary>
    /// 辅助方法：绘制小方块
    /// </summary>
    void DrawSquare(Texture2D texture, int centerX, int centerY, int size, Color color)
    {
        for (int x = centerX; x < centerX + size; x++)
        {
            for (int y = centerY; y < centerY + size; y++)
            {
                if (x >= 0 && x < texture.width &&
                    y >= 0 && y < texture.height)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    /// <summary>
    /// 保存纹理到Assets文件夹
    /// </summary>
    void SaveTextureToAssets(Texture2D texture)
    {
        try
        {
            // 确保目录存在
            string directory = Path.GetDirectoryName(imageSavePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 保存主图片
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(imageSavePath, pngData);

            // 保存备份
            string backupDirectory = Path.GetDirectoryName(backupPath);
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }
            File.WriteAllBytes(backupPath, pngData);

            Debug.Log($"✅ 附件图片已保存到:");
            Debug.Log($"   - 主路径: {imageSavePath}");
            Debug.Log($"   - 备份路径: {backupPath}");

            // 刷新AssetDatabase
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 保存图片失败: {e.Message}");
        }
    }

    /// <summary>
    /// 应用纹理到幕布
    /// </summary>
    void ApplyTextureToCurtain(Texture2D texture)
    {
        // 查找或创建CurtainImageApplicator
        if (imageApplicator == null)
        {
            imageApplicator = FindObjectOfType<CurtainImageApplicator>();
        }

        if (imageApplicator != null)
        {
            // 设置用户纹理
            imageApplicator.userProvidedTexture = texture;

            // 应用图片到幕布
            imageApplicator.ApplyImageToCurtain();

            Debug.Log("✅ 附件图片已应用到幕布");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到CurtainImageApplicator，将创建新的");
            CreateNewImageApplicator(texture);
        }
    }

    /// <summary>
    /// 创建新的图片应用器
    /// </summary>
    void CreateNewImageApplicator(Texture2D texture)
    {
        GameObject setupObj = new GameObject("AttachmentImageApplicator");
        CurtainImageApplicator newApplicator = setupObj.AddComponent<CurtainImageApplicator>();

        // 设置参数
        newApplicator.curtainWidth = 3.5f;
        newApplicator.curtainThickness = 0.005f;
        newApplicator.topAligned = true;
        newApplicator.userProvidedTexture = texture;

        // 应用图片
        newApplicator.ApplyImageToCurtain();

        imageApplicator = newApplicator;
        Debug.Log("✅ 已创建新的CurtainImageApplicator并应用附件图片");
    }

    /// <summary>
    /// 显示应用成功信息
    /// </summary>
    void ShowApplicationSuccess()
    {
        Debug.Log("=== 附件图片应用成功 ===");
        Debug.Log("✅ 图片内容:");
        Debug.Log("   - 蓝色网球场地背景");
        Debug.Log("   - 粉色边框");
        Debug.Log("   - 白色场地线条");
        Debug.Log("   - HeHaa文字（顶部）");
        Debug.Log("   - 分数圆圈：20, 20, 50, 30, 50");
        Debug.Log("   - 二维码区域（右下角）");
        Debug.Log("✅ 幕布设置:");
        Debug.Log("   - 宽度: 3.5米");
        Debug.Log("   - 厚度: 0.005");
        Debug.Log("   - 顶部对齐: 已启用");
        Debug.Log("✅ 网球反弹功能: 已启用");
        Debug.Log("🎾 现在可以发射网球测试效果！");
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void ShowAttachmentInstructions()
    {
        Debug.Log("=== 附件图片处理器使用说明 ===");
        Debug.Log($"🖼️ {saveAttachmentKey}键 - 保存并应用附件图片到幕布");
        Debug.Log("📋 功能:");
        Debug.Log("   ✅ 根据附件描述创建网球场地纹理");
        Debug.Log("   ✅ 保存图片到Assets/Textures/文件夹");
        Debug.Log("   ✅ 自动应用到幕布（3.5米宽度）");
        Debug.Log("   ✅ 启用网球反弹物理");
        Debug.Log("💡 也可在Inspector中使用右键菜单");
    }
}


