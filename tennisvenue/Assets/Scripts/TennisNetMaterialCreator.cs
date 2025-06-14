using UnityEngine;
using UnityEditor;

/// <summary>
/// 网球网材质创建器
/// 自动生成白色细格网纹理和材质，用于左右墙面
/// </summary>
public class TennisNetMaterialCreator : MonoBehaviour
{
    [Header("网格设置")]
    [SerializeField] private int textureSize = 512;
    [SerializeField] private int gridSize = 32; // 网格密度，数值越小网格越密
    [SerializeField] private int lineWidth = 2; // 网格线宽度（像素）
    [SerializeField] private Color netColor = Color.white;
    [SerializeField] private Color backgroundColor = new Color(1f, 1f, 1f, 0f); // 透明背景

    [Header("材质设置")]
    [SerializeField] private bool makeTransparent = true;
    [SerializeField] private float transparency = 0.8f;

    [Header("自动应用")]
    [SerializeField] private bool autoApplyToWalls = true;
    [SerializeField] private bool createOnStart = false;

    void Start()
    {
        if (createOnStart)
        {
            CreateTennisNetMaterial();
        }

        Debug.Log("🎾 TennisNetMaterialCreator Ready!");
        Debug.Log("   Press N: Create Tennis Net Material");
        Debug.Log("   Press M: Apply to Walls");
    }

    void Update()
    {
        // N键：创建网球网材质
        if (Input.GetKeyDown(KeyCode.N))
        {
            CreateTennisNetMaterial();
        }

        // M键：应用到墙面
        if (Input.GetKeyDown(KeyCode.M))
        {
            ApplyToWalls();
        }
    }

    /// <summary>
    /// 创建网球网材质
    /// </summary>
    [ContextMenu("Create Tennis Net Material")]
    public void CreateTennisNetMaterial()
    {
        Debug.Log("🎾 Creating Tennis Net Material...");

        // 生成纹理
        Texture2D netTexture = GenerateNetTexture();

        // 保存纹理
        string texturePath = "Assets/Materials/TennisNetTexture.png";
        SaveTexture(netTexture, texturePath);

        // 创建材质
        Material netMaterial = CreateNetMaterial(netTexture);

        // 保存材质
        string materialPath = "Assets/Materials/TennisNetMaterial.mat";
        SaveMaterial(netMaterial, materialPath);

        if (autoApplyToWalls)
        {
            ApplyToWalls();
        }

        Debug.Log("✅ Tennis Net Material Created Successfully!");
    }

    /// <summary>
    /// 生成白色细格网纹理
    /// </summary>
    Texture2D GenerateNetTexture()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[textureSize * textureSize];

        // 填充背景色
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }

        // 绘制垂直线
        for (int x = 0; x < textureSize; x += gridSize)
        {
            for (int y = 0; y < textureSize; y++)
            {
                for (int w = 0; w < lineWidth && x + w < textureSize; w++)
                {
                    int index = (y * textureSize) + (x + w);
                    if (index >= 0 && index < pixels.Length)
                    {
                        pixels[index] = netColor;
                    }
                }
            }
        }

        // 绘制水平线
        for (int y = 0; y < textureSize; y += gridSize)
        {
            for (int x = 0; x < textureSize; x++)
            {
                for (int w = 0; w < lineWidth && y + w < textureSize; w++)
                {
                    int index = ((y + w) * textureSize) + x;
                    if (index >= 0 && index < pixels.Length)
                    {
                        pixels[index] = netColor;
                    }
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Point; // 保持清晰的像素边缘

        return texture;
    }

    /// <summary>
    /// 创建网球网材质
    /// </summary>
    Material CreateNetMaterial(Texture2D texture)
    {
        Material material;

        if (makeTransparent)
        {
            // 创建透明材质
            material = new Material(Shader.Find("Standard"));
            material.SetFloat("_Mode", 3); // Transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.EnableKeyword("_ALPHABLEND_ON");
            material.renderQueue = 3000;
            material.color = new Color(1f, 1f, 1f, transparency);
        }
        else
        {
            // 创建不透明材质
            material = new Material(Shader.Find("Standard"));
            material.color = Color.white;
        }

        material.mainTexture = texture;
        material.name = "TennisNetMaterial";

        // 设置纹理平铺
        material.mainTextureScale = new Vector2(2f, 4f); // 调整网格的重复频率

        return material;
    }

    /// <summary>
    /// 保存纹理到文件
    /// </summary>
    void SaveTexture(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
        Debug.Log($"✅ Texture saved to: {path}");
#endif
    }

    /// <summary>
    /// 保存材质到文件
    /// </summary>
    void SaveMaterial(Material material, string path)
    {
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(material, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Material saved to: {path}");
#endif
    }

    /// <summary>
    /// 应用材质到左右墙面
    /// </summary>
    [ContextMenu("Apply to Walls")]
    public void ApplyToWalls()
    {
        Debug.Log("🎾 Applying Tennis Net Material to Walls...");

        // 查找保存的材质
        Material netMaterial = Resources.Load<Material>("Materials/TennisNetMaterial");
        if (netMaterial == null)
        {
            // 尝试直接加载
            netMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/TennisNetMaterial.mat");
        }

        if (netMaterial == null)
        {
            Debug.LogError("❌ Tennis Net Material not found! Please create it first with N key.");
            return;
        }

        // 应用到LeftWall
        GameObject leftWall = GameObject.Find("LeftWall");
        if (leftWall != null)
        {
            ApplyMaterialToWall(leftWall, netMaterial, "LeftWall");
        }
        else
        {
            Debug.LogWarning("⚠️ LeftWall not found in scene");
        }

        // 应用到RightWall
        GameObject rightWall = GameObject.Find("RightWall");
        if (rightWall != null)
        {
            ApplyMaterialToWall(rightWall, netMaterial, "RightWall");
        }
        else
        {
            Debug.LogWarning("⚠️ RightWall not found in scene");
        }

        Debug.Log("✅ Tennis Net Material Applied to Walls!");
    }

    /// <summary>
    /// 应用材质到特定墙面
    /// </summary>
    void ApplyMaterialToWall(GameObject wall, Material material, string wallName)
    {
        MeshRenderer renderer = wall.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = material;
            Debug.Log($"✅ Applied Tennis Net Material to {wallName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ {wallName} has no MeshRenderer component");
        }
    }

    /// <summary>
    /// 快速预览效果
    /// </summary>
    [ContextMenu("Quick Preview")]
    public void QuickPreview()
    {
        Debug.Log("🎾 Creating Quick Preview...");

        // 创建预览对象
        GameObject preview = GameObject.CreatePrimitive(PrimitiveType.Quad);
        preview.name = "TennisNetPreview";
        preview.transform.position = new Vector3(0, 2, -2);
        preview.transform.localScale = new Vector3(2, 2, 1);

        // 生成材质并应用
        Texture2D previewTexture = GenerateNetTexture();
        Material previewMaterial = CreateNetMaterial(previewTexture);

        MeshRenderer renderer = preview.GetComponent<MeshRenderer>();
        renderer.material = previewMaterial;

        Debug.Log("✅ Preview created! Check the TennisNetPreview object in scene.");
        Debug.Log("   Delete it when satisfied with the result.");
    }
}