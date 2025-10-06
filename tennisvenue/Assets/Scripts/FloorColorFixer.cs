using UnityEngine;

/// <summary>
/// 地面颜色修复器 - 确保地面显示正确的深绿色
/// </summary>
public class FloorColorFixer : MonoBehaviour
{
    [Header("地面修复设置")]
    [Tooltip("按此键修复地面颜色")]
    public KeyCode fixFloorKey = KeyCode.F11;

    [Header("地面材质设置")]
    public Color floorColor = new Color(0.6f, 0.8f, 1.0f, 1f); // 浅蓝色
    public float metallic = 0.0f;
    public float smoothness = 0.3f;

    void Start()
    {
        Debug.Log("=== 地面颜色修复器已加载 ===");
        ShowFloorFixInstructions();

        // 自动修复地面颜色
        FixFloorColor();
    }

    void Update()
    {
        if (Input.GetKeyDown(fixFloorKey))
        {
            FixFloorColor();
        }
    }

    /// <summary>
    /// 修复地面颜色
    /// </summary>
    [ContextMenu("修复地面颜色")]
    public void FixFloorColor()
    {
        Debug.Log("🔧 开始修复地面颜色...");

        // 查找地面对象
        GameObject floor = GameObject.Find("Floor");
        if (floor == null)
        {
            Debug.LogError("❌ 未找到地面对象 'Floor'");
            return;
        }

        // 获取渲染器组件
        Renderer floorRenderer = floor.GetComponent<Renderer>();
        if (floorRenderer == null)
        {
            Debug.LogError("❌ 地面对象缺少Renderer组件");
            return;
        }

        // 检查当前材质
        Material currentMaterial = floorRenderer.material;
        Debug.Log($"📋 当前地面材质: {currentMaterial.name}");
        Debug.Log($"🎨 当前颜色: {currentMaterial.color}");

        // 修复材质颜色
        FixMaterialColor(currentMaterial);

        // 确保物理材质也正确
        EnsureFloorPhysics();

        Debug.Log("✅ 地面颜色修复完成！");
        ShowFixResult(floorRenderer.material);
    }

    /// <summary>
    /// 修复材质颜色
    /// </summary>
    void FixMaterialColor(Material material)
    {
        // 设置正确的深绿色
        material.color = floorColor;

        // 设置材质属性
        material.SetFloat("_Metallic", metallic);
        material.SetFloat("_Glossiness", smoothness);

        // 确保使用Standard着色器
        if (material.shader.name != "Standard")
        {
            material.shader = Shader.Find("Standard");
            Debug.Log("🔄 已切换到Standard着色器");
        }

        Debug.Log($"✅ 地面材质颜色已设置为: {floorColor}");
        Debug.Log($"   - 金属度: {metallic}");
        Debug.Log($"   - 光滑度: {smoothness}");
    }

    /// <summary>
    /// 确保地面物理材质正确
    /// </summary>
    void EnsureFloorPhysics()
    {
        GameObject floor = GameObject.Find("Floor");
        if (floor == null) return;

        Collider floorCollider = floor.GetComponent<Collider>();
        if (floorCollider == null)
        {
            Debug.LogWarning("⚠️ 地面缺少Collider组件");
            return;
        }

        // 检查物理材质
        if (floorCollider.material == null)
        {
            // 创建地面物理材质
            PhysicMaterial floorPhysicMat = new PhysicMaterial("FloorPhysics");
            floorPhysicMat.bounciness = 0.75f;
            floorPhysicMat.dynamicFriction = 0.75f;
            floorPhysicMat.staticFriction = 0.8f;
            floorPhysicMat.frictionCombine = PhysicMaterialCombine.Average;
            floorPhysicMat.bounceCombine = PhysicMaterialCombine.Maximum;

            floorCollider.material = floorPhysicMat;
            Debug.Log("✅ 已创建地面物理材质");
        }
        else
        {
            Debug.Log($"✅ 地面已有物理材质: {floorCollider.material.name}");
        }
    }

    /// <summary>
    /// 创建标准网球场地材质
    /// </summary>
    [ContextMenu("创建标准网球场地材质")]
    public void CreateStandardTennisCourtMaterial()
    {
        Debug.Log("🎾 创建标准网球场地材质...");

        GameObject floor = GameObject.Find("Floor");
        if (floor == null) return;

        Renderer floorRenderer = floor.GetComponent<Renderer>();
        if (floorRenderer == null) return;

        // 创建新的网球场地材质
        Material tennisCourtMaterial = new Material(Shader.Find("Standard"));
        tennisCourtMaterial.name = "TennisCourtMaterial";

        // 设置网球场地颜色（浅蓝色）
        tennisCourtMaterial.color = new Color(0.6f, 0.8f, 1.0f, 1f);
        tennisCourtMaterial.SetFloat("_Metallic", 0.0f);
        tennisCourtMaterial.SetFloat("_Glossiness", 0.2f);

        // 应用材质
        floorRenderer.material = tennisCourtMaterial;

        Debug.Log("✅ 标准网球场地材质已创建并应用");
        Debug.Log("   - 颜色: 浅蓝色 (0.6, 0.8, 1.0)");
        Debug.Log("   - 金属度: 0.0");
        Debug.Log("   - 光滑度: 0.2");
    }

    /// <summary>
    /// 显示修复结果
    /// </summary>
    void ShowFixResult(Material material)
    {
        Debug.Log("=== 地面颜色修复结果 ===");
        Debug.Log($"✅ 材质名称: {material.name}");
        Debug.Log($"✅ 材质颜色: {material.color}");
        Debug.Log($"✅ 着色器: {material.shader.name}");
        Debug.Log($"✅ 金属度: {material.GetFloat("_Metallic")}");
        Debug.Log($"✅ 光滑度: {material.GetFloat("_Glossiness")}");
        Debug.Log("");
        Debug.Log("🎾 地面现在应该显示为浅蓝色");
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void ShowFloorFixInstructions()
    {
        Debug.Log("=== 地面颜色修复器使用说明 ===");
        Debug.Log($"🔧 {fixFloorKey}键 - 修复地面颜色");
        Debug.Log("📋 功能:");
        Debug.Log("   ✅ 将地面颜色设置为浅蓝色");
        Debug.Log("   ✅ 设置合适的材质属性");
        Debug.Log("   ✅ 确保物理材质正确");
        Debug.Log("   ✅ 自动在启动时修复");
        Debug.Log("💡 也可在Inspector中使用右键菜单");
    }

    /// <summary>
    /// 诊断地面状态
    /// </summary>
    [ContextMenu("诊断地面状态")]
    public void DiagnoseFloorStatus()
    {
        Debug.Log("=== 地面状态诊断 ===");

        GameObject floor = GameObject.Find("Floor");
        if (floor == null)
        {
            Debug.LogError("❌ 未找到地面对象");
            return;
        }

        Debug.Log($"📍 地面位置: {floor.transform.position}");
        Debug.Log($"📏 地面缩放: {floor.transform.localScale}");

        Renderer floorRenderer = floor.GetComponent<Renderer>();
        if (floorRenderer != null && floorRenderer.material != null)
        {
            Material mat = floorRenderer.material;
            Debug.Log($"🎨 材质名称: {mat.name}");
            Debug.Log($"🎨 材质颜色: {mat.color}");
            Debug.Log($"🎨 着色器: {mat.shader.name}");

            // 检查颜色是否正确（浅蓝色）
            Color currentColor = mat.color;
            bool colorCorrect = currentColor.r > 0.5f && currentColor.g > 0.7f && currentColor.b > 0.9f;
            Debug.Log($"✅ 颜色正确: {(colorCorrect ? "✅" : "❌")}");
        }

        Collider floorCollider = floor.GetComponent<Collider>();
        Debug.Log($"🏀 碰撞器: {(floorCollider != null ? "✅" : "❌")}");
        Debug.Log($"⚡ 物理材质: {(floorCollider != null && floorCollider.material != null ? "✅" : "❌")}");

        Debug.Log("🎯 地面应该显示为浅蓝色，支持网球反弹");
    }
}
