using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 圆环可见性修复器 - 专门解决圆环标记在游戏模式下不可见的问题
/// </summary>
public class RingVisibilityFixer : MonoBehaviour
{
    [Header("可见性修复设置")]
    [Tooltip("是否启用实时监控")]
    public bool enableRealTimeMonitoring = true;

    [Tooltip("监控间隔（秒）")]
    public float monitorInterval = 1f;

    [Tooltip("强制可见的圆环大小")]
    public float forceVisibleSize = 1.5f;

    [Tooltip("强制可见的圆环高度")]
    public float forceVisibleHeight = 0.2f;

    private BounceImpactMarker impactMarker;
    private float lastMonitorTime = 0f;

    void Start()
    {
        Debug.Log("=== 圆环可见性修复器启动 ===");
        Debug.Log("快捷键说明:");
        Debug.Log("  F6: 运行可见性诊断");
        Debug.Log("  F7: 创建强制可见测试圆环");
        Debug.Log("  F8: 修复现有圆环可见性");
        Debug.Log("  F9: 创建超大测试圆环");

        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (impactMarker == null)
        {
            Debug.LogError("❌ 未找到BounceImpactMarker系统！");
        }
        else
        {
            Debug.Log("✅ BounceImpactMarker系统已找到");
            RunInitialDiagnostic();
        }
    }

    void Update()
    {
        // 实时监控
        if (enableRealTimeMonitoring && Time.time - lastMonitorTime > monitorInterval)
        {
            MonitorRingVisibility();
            lastMonitorTime = Time.time;
        }

        // 快捷键控制
        HandleKeyboardInput();
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            RunVisibilityDiagnostic();
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            CreateForceVisibleRing();
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            FixExistingRingsVisibility();
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            CreateSuperLargeTestRing();
        }
    }

    /// <summary>
    /// 运行初始诊断
    /// </summary>
    void RunInitialDiagnostic()
    {
        Debug.Log("🔍 运行初始可见性诊断...");

        // 检查摄像机位置
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log($"📷 主摄像机位置: {mainCamera.transform.position}");
            Debug.Log($"📷 主摄像机旋转: {mainCamera.transform.rotation.eulerAngles}");
            Debug.Log($"📷 主摄像机视野: {mainCamera.fieldOfView}°");
        }

        // 检查地面位置
        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Debug.Log($"🏢 地面位置: {floor.transform.position}");
            Debug.Log($"🏢 地面大小: {floor.transform.localScale}");
        }

        // 检查现有圆环
        CheckExistingRings();
    }

    /// <summary>
    /// 运行可见性诊断
    /// </summary>
    void RunVisibilityDiagnostic()
    {
        Debug.Log("=== 圆环可见性诊断 ===");

        // 检查场景中的所有圆环标记
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> rings = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("ImpactRing") || obj.name.Contains("Ring"))
            {
                rings.Add(obj);
            }
        }

        Debug.Log($"🔍 找到 {rings.Count} 个圆环对象");

        foreach (GameObject ring in rings)
        {
            AnalyzeRingVisibility(ring);
        }

        if (rings.Count == 0)
        {
            Debug.LogWarning("⚠️ 场景中没有找到圆环标记对象");
            Debug.Log("   可能的原因:");
            Debug.Log("   1. 圆环创建失败");
            Debug.Log("   2. 圆环已被销毁");
            Debug.Log("   3. 圆环名称不匹配");
        }
    }

    /// <summary>
    /// 分析单个圆环的可见性
    /// </summary>
    void AnalyzeRingVisibility(GameObject ring)
    {
        Debug.Log($"🔍 分析圆环: {ring.name}");
        Debug.Log($"   位置: {ring.transform.position}");
        Debug.Log($"   大小: {ring.transform.localScale}");
        Debug.Log($"   激活状态: {ring.activeInHierarchy}");

        // 检查渲染器
        Renderer renderer = ring.GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log($"   渲染器启用: {renderer.enabled}");
            Debug.Log($"   材质: {renderer.material.name}");
            Debug.Log($"   颜色: {renderer.material.color}");
            Debug.Log($"   渲染队列: {renderer.material.renderQueue}");

            // 检查是否在摄像机视野内
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Bounds bounds = renderer.bounds;
                bool inView = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(mainCamera), bounds);
                Debug.Log($"   在摄像机视野内: {inView}");

                // 计算距离摄像机的距离
                float distance = Vector3.Distance(mainCamera.transform.position, ring.transform.position);
                Debug.Log($"   距离摄像机: {distance:F2}m");
            }
        }
        else
        {
            Debug.LogError($"   ❌ 圆环 {ring.name} 没有渲染器组件！");
        }

        // 检查网格
        MeshFilter meshFilter = ring.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            Debug.Log($"   网格顶点数: {meshFilter.mesh.vertexCount}");
            Debug.Log($"   网格三角形数: {meshFilter.mesh.triangles.Length / 3}");
        }
        else
        {
            Debug.LogError($"   ❌ 圆环 {ring.name} 没有有效的网格！");
        }
    }

    /// <summary>
    /// 创建强制可见的测试圆环
    /// </summary>
    void CreateForceVisibleRing()
    {
        Debug.Log("🎯 创建强制可见的测试圆环");

        // 创建一个简单的圆柱体作为测试圆环
        GameObject testRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        testRing.name = "ForceVisibleTestRing";

        // 设置位置在摄像机前方
        Camera mainCamera = Camera.main;
        Vector3 cameraPos = mainCamera != null ? mainCamera.transform.position : Vector3.zero;
        Vector3 ringPosition = new Vector3(0, 0.1f, cameraPos.z + 2f);
        testRing.transform.position = ringPosition;

        // 设置大小 - 扁平的大圆环
        testRing.transform.localScale = new Vector3(forceVisibleSize, forceVisibleHeight, forceVisibleSize);

        // 设置超亮的材质
        Renderer renderer = testRing.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.red;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.red * 5f); // 超强发光
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Smoothness", 0.9f);
        renderer.material = mat;

        // 15秒后销毁
        Destroy(testRing, 15f);

        Debug.Log($"✅ 强制可见测试圆环已创建:");
        Debug.Log($"   位置: {ringPosition}");
        Debug.Log($"   大小: {testRing.transform.localScale}");
        Debug.Log($"   应该显示为明亮的红色圆柱体！");

        // 调整摄像机角度以确保能看到
        if (mainCamera != null)
        {
            Vector3 direction = (ringPosition - mainCamera.transform.position).normalized;
            mainCamera.transform.LookAt(ringPosition);
            Debug.Log("📷 摄像机已调整朝向测试圆环");
        }
    }

    /// <summary>
    /// 修复现有圆环的可见性
    /// </summary>
    void FixExistingRingsVisibility()
    {
        Debug.Log("🔧 修复现有圆环的可见性");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int fixedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("ImpactRing"))
            {
                FixSingleRingVisibility(obj);
                fixedCount++;
            }
        }

        Debug.Log($"✅ 已修复 {fixedCount} 个圆环的可见性");
    }

    /// <summary>
    /// 修复单个圆环的可见性
    /// </summary>
    void FixSingleRingVisibility(GameObject ring)
    {
        Debug.Log($"🔧 修复圆环: {ring.name}");

        // 确保对象激活
        ring.SetActive(true);

        // 调整位置（确保在地面上方）
        Vector3 pos = ring.transform.position;
        pos.y = Mathf.Max(pos.y, 0.05f);
        ring.transform.position = pos;

        // 调整大小（确保足够大）
        Vector3 scale = ring.transform.localScale;
        if (scale.x < 0.5f || scale.z < 0.5f)
        {
            scale.x = Mathf.Max(scale.x, 1f);
            scale.z = Mathf.Max(scale.z, 1f);
            scale.y = Mathf.Max(scale.y, 0.1f);
            ring.transform.localScale = scale;
            Debug.Log($"   调整大小为: {scale}");
        }

        // 修复材质
        Renderer renderer = ring.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;

            // 确保颜色不透明
            Color color = mat.color;
            color.a = 1f;
            mat.color = color;

            // 增强发光效果
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 2f);

            // 设置渲染模式为不透明
            mat.SetFloat("_Mode", 0); // Opaque mode
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 2000; // 不透明渲染队列

            renderer.enabled = true;
            Debug.Log($"   材质已修复，颜色: {color}");
        }

        Debug.Log($"✅ 圆环 {ring.name} 可见性已修复");
    }

    /// <summary>
    /// 创建超大测试圆环
    /// </summary>
    void CreateSuperLargeTestRing()
    {
        Debug.Log("🎯 创建超大测试圆环");

        GameObject superRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        superRing.name = "SuperLargeTestRing";

        // 设置在场地中央
        superRing.transform.position = new Vector3(0, 0.3f, 0);
        superRing.transform.localScale = new Vector3(3f, 0.3f, 3f);

        // 设置超亮的黄色材质
        Renderer renderer = superRing.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.yellow;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.yellow * 10f); // 超超强发光
        renderer.material = mat;

        // 20秒后销毁
        Destroy(superRing, 20f);

        Debug.Log("✅ 超大测试圆环已创建 - 应该非常明显！");
        Debug.Log("   如果您还是看不到，可能是摄像机角度问题");
        Debug.Log("   请按T键切换到俯视角度");
    }

    /// <summary>
    /// 监控圆环可见性
    /// </summary>
    void MonitorRingVisibility()
    {
        if (impactMarker == null) return;

        int activeMarkers = impactMarker.GetActiveMarkerCount();
        if (activeMarkers > 0)
        {
            Debug.Log($"📊 当前活动圆环标记数: {activeMarkers}");

            // 检查是否有不可见的圆环
            CheckExistingRings();
        }
    }

    /// <summary>
    /// 检查现有圆环
    /// </summary>
    void CheckExistingRings()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int visibleCount = 0;
        int invisibleCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("ImpactRing"))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null && renderer.enabled && obj.activeInHierarchy)
                {
                    visibleCount++;
                }
                else
                {
                    invisibleCount++;
                    Debug.LogWarning($"⚠️ 发现不可见圆环: {obj.name} 位置: {obj.transform.position}");
                }
            }
        }

        if (invisibleCount > 0)
        {
            Debug.LogWarning($"⚠️ 发现 {invisibleCount} 个不可见圆环，{visibleCount} 个可见圆环");
            Debug.Log("   按F8键修复可见性问题");
        }
    }
}