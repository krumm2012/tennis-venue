using UnityEngine;

/// <summary>
/// 圆环标记与TargetPlane显示冲突检查器
/// 检查圆环标记是否与TargetPlane有Z-fighting或渲染冲突
/// </summary>
public class RingTargetConflictChecker : MonoBehaviour
{
    [Header("检查设置")]
    [Tooltip("是否自动修复冲突")]
    public bool autoFixConflicts = true;

    [Tooltip("圆环与地面的安全高度差")]
    public float safeHeightOffset = 0.02f;

    [Tooltip("TargetPlane的渲染队列优先级")]
    public int targetPlaneRenderQueue = 2000;

    [Tooltip("圆环标记的渲染队列优先级")]
    public int ringMarkerRenderQueue = 3000;

    private GameObject targetPlane;
    private BounceImpactMarker impactMarker;

    void Start()
    {
        Debug.Log("=== Ring-Target Conflict Checker Started ===");
        Debug.Log("Press F10 to check for display conflicts");
        Debug.Log("Press F11 to fix conflicts automatically");
        Debug.Log("Press F12 to create test ring at TargetPlane position");

        // 查找相关组件
        targetPlane = GameObject.Find("TargetPlane");
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (targetPlane == null)
        {
            Debug.LogWarning("⚠️ TargetPlane not found in scene!");
        }

        if (impactMarker == null)
        {
            Debug.LogWarning("⚠️ BounceImpactMarker system not found!");
        }

        // 自动运行初始检查
        CheckForConflicts();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            CheckForConflicts();
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            FixConflicts();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            CreateTestRingAtTargetPlane();
        }
    }

    /// <summary>
    /// 检查显示冲突
    /// </summary>
    void CheckForConflicts()
    {
        Debug.Log("=== Checking Ring-Target Display Conflicts ===");

        if (targetPlane == null || impactMarker == null)
        {
            Debug.LogError("❌ Missing required components for conflict check");
            return;
        }

        // 1. 检查位置冲突
        CheckPositionConflicts();

        // 2. 检查渲染队列冲突
        CheckRenderQueueConflicts();

        // 3. 检查材质透明度冲突
        CheckMaterialConflicts();

        // 4. 检查层级冲突
        CheckLayerConflicts();

        Debug.Log("=== Conflict Check Complete ===");
    }

    /// <summary>
    /// 检查位置冲突（Z-fighting）
    /// </summary>
    void CheckPositionConflicts()
    {
        Vector3 targetPos = targetPlane.transform.position;
        Debug.Log($"🎯 TargetPlane position: {targetPos}");
        Debug.Log($"   Scale: {targetPlane.transform.localScale}");

        // TargetPlane在y=-0.05，圆环在y=0.01，应该没有Z-fighting
        float heightDifference = 0.01f - targetPos.y; // 圆环高度 - TargetPlane高度

        if (heightDifference < safeHeightOffset)
        {
            Debug.LogWarning($"⚠️ Potential Z-fighting detected!");
            Debug.LogWarning($"   Height difference: {heightDifference:F3}m (recommended: >{safeHeightOffset:F3}m)");
        }
        else
        {
            Debug.Log($"✅ Position conflict check passed - Height difference: {heightDifference:F3}m");
        }
    }

    /// <summary>
    /// 检查渲染队列冲突
    /// </summary>
    void CheckRenderQueueConflicts()
    {
        Renderer targetRenderer = targetPlane.GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogWarning("⚠️ TargetPlane has no Renderer component");
            return;
        }

        Material targetMaterial = targetRenderer.material;
        int targetQueue = targetMaterial.renderQueue;

        Debug.Log($"🎯 TargetPlane render queue: {targetQueue}");
        Debug.Log($"⭕ Ring markers render queue: {ringMarkerRenderQueue} (from BounceImpactMarker)");

        if (targetQueue >= ringMarkerRenderQueue)
        {
            Debug.LogWarning($"⚠️ Render queue conflict detected!");
            Debug.LogWarning($"   TargetPlane ({targetQueue}) should render before rings ({ringMarkerRenderQueue})");
        }
        else
        {
            Debug.Log($"✅ Render queue check passed - TargetPlane renders first");
        }
    }

    /// <summary>
    /// 检查材质冲突
    /// </summary>
    void CheckMaterialConflicts()
    {
        Renderer targetRenderer = targetPlane.GetComponent<Renderer>();
        if (targetRenderer == null) return;

        Material targetMaterial = targetRenderer.material;

        Debug.Log($"🎯 TargetPlane material info:");
        Debug.Log($"   Shader: {targetMaterial.shader.name}");
        Debug.Log($"   Color: {targetMaterial.color}");
        Debug.Log($"   Render queue: {targetMaterial.renderQueue}");

        // 检查是否透明
        bool isTransparent = targetMaterial.renderQueue >= 3000;
        if (isTransparent)
        {
            Debug.LogWarning($"⚠️ TargetPlane uses transparent material - may conflict with ring transparency");
        }
        else
        {
            Debug.Log($"✅ TargetPlane uses opaque material - good for ring visibility");
        }
    }

    /// <summary>
    /// 检查层级冲突
    /// </summary>
    void CheckLayerConflicts()
    {
        int targetLayer = targetPlane.layer;
        Debug.Log($"🎯 TargetPlane layer: {targetLayer} ({LayerMask.LayerToName(targetLayer)})");

        // 圆环标记使用默认层级0
        Debug.Log($"⭕ Ring markers layer: 0 (Default)");

        if (targetLayer == 0)
        {
            Debug.LogWarning($"⚠️ Both TargetPlane and rings use same layer - may cause sorting issues");
        }
        else
        {
            Debug.Log($"✅ Different layers used - good for sorting control");
        }
    }

    /// <summary>
    /// 自动修复冲突
    /// </summary>
    void FixConflicts()
    {
        Debug.Log("=== Auto-fixing Display Conflicts ===");

        if (targetPlane == null)
        {
            Debug.LogError("❌ Cannot fix - TargetPlane not found");
            return;
        }

        // 1. 修复TargetPlane渲染队列
        FixTargetPlaneRenderQueue();

        // 2. 优化TargetPlane材质
        OptimizeTargetPlaneMaterial();

        // 3. 确保圆环标记有正确的高度偏移
        EnsureRingHeightOffset();

        Debug.Log("✅ Conflict fixes applied");
    }

    /// <summary>
    /// 修复TargetPlane渲染队列
    /// </summary>
    void FixTargetPlaneRenderQueue()
    {
        Renderer targetRenderer = targetPlane.GetComponent<Renderer>();
        if (targetRenderer == null) return;

        Material targetMaterial = targetRenderer.material;
        int originalQueue = targetMaterial.renderQueue;

        targetMaterial.renderQueue = targetPlaneRenderQueue;

        Debug.Log($"🔧 TargetPlane render queue: {originalQueue} → {targetPlaneRenderQueue}");
    }

    /// <summary>
    /// 优化TargetPlane材质
    /// </summary>
    void OptimizeTargetPlaneMaterial()
    {
        Renderer targetRenderer = targetPlane.GetComponent<Renderer>();
        if (targetRenderer == null) return;

        Material targetMaterial = targetRenderer.material;

        // 确保使用不透明渲染模式
        targetMaterial.SetFloat("_Mode", 0); // Opaque
        targetMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        targetMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        targetMaterial.SetInt("_ZWrite", 1);
        targetMaterial.DisableKeyword("_ALPHATEST_ON");
        targetMaterial.DisableKeyword("_ALPHABLEND_ON");
        targetMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        Debug.Log($"🔧 TargetPlane material optimized for opaque rendering");
    }

    /// <summary>
    /// 确保圆环标记有正确的高度偏移
    /// </summary>
    void EnsureRingHeightOffset()
    {
        if (impactMarker == null) return;

        // 检查当前场景中的圆环标记
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ringCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("ImpactRing"))
            {
                ringCount++;
                Vector3 currentPos = obj.transform.position;
                Vector3 targetPos = targetPlane.transform.position;

                float heightDiff = currentPos.y - targetPos.y;
                if (heightDiff < safeHeightOffset)
                {
                    Vector3 newPos = new Vector3(currentPos.x, targetPos.y + safeHeightOffset, currentPos.z);
                    obj.transform.position = newPos;
                    Debug.Log($"🔧 Adjusted ring {obj.name} height: {currentPos.y:F3} → {newPos.y:F3}");
                }
            }
        }

        Debug.Log($"🔧 Checked {ringCount} existing ring markers for height conflicts");
    }

    /// <summary>
    /// 在TargetPlane位置创建测试圆环
    /// </summary>
    void CreateTestRingAtTargetPlane()
    {
        if (targetPlane == null)
        {
            Debug.LogError("❌ Cannot create test ring - TargetPlane not found");
            return;
        }

        Vector3 targetPos = targetPlane.transform.position;
        Vector3 testRingPos = new Vector3(targetPos.x, targetPos.y + safeHeightOffset, targetPos.z);

        Debug.Log($"=== Creating Test Ring at TargetPlane Position ===");
        Debug.Log($"TargetPlane position: {targetPos}");
        Debug.Log($"Test ring position: {testRingPos}");

        // 创建测试圆环
        GameObject testRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        testRing.name = "TestRing_TargetPlane_Conflict";
        testRing.transform.position = testRingPos;
        testRing.transform.localScale = new Vector3(1.0f, 0.05f, 1.0f); // 扁平圆环

        // 设置明亮的材质便于观察
        Renderer renderer = testRing.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.cyan;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.cyan * 2f);
        mat.renderQueue = ringMarkerRenderQueue; // 使用圆环的渲染队列

        renderer.material = mat;

        // 10秒后销毁
        Destroy(testRing, 10f);

        Debug.Log($"✅ Test ring created - observe for conflicts with TargetPlane");
        Debug.Log($"Ring should appear as bright cyan cylinder above TargetPlane");
    }

    /// <summary>
    /// 获取冲突检查报告
    /// </summary>
    public string GetConflictReport()
    {
        if (targetPlane == null || impactMarker == null)
        {
            return "❌ Missing components for conflict analysis";
        }

        Vector3 targetPos = targetPlane.transform.position;
        float heightDiff = 0.01f - targetPos.y; // 圆环默认高度 - TargetPlane高度

        Renderer targetRenderer = targetPlane.GetComponent<Renderer>();
        int targetQueue = targetRenderer != null ? targetRenderer.material.renderQueue : -1;

        return $"Conflict Analysis:\n" +
               $"• Height difference: {heightDiff:F3}m (safe: >{safeHeightOffset:F3}m)\n" +
               $"• TargetPlane queue: {targetQueue}\n" +
               $"• Ring queue: {ringMarkerRenderQueue}\n" +
               $"• TargetPlane layer: {targetPlane.layer}\n" +
               $"• Status: {(heightDiff >= safeHeightOffset && targetQueue < ringMarkerRenderQueue ? "✅ No conflicts" : "⚠️ Potential conflicts")}";
    }
}