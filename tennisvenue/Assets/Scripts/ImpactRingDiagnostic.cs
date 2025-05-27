using UnityEngine;
using System.Reflection;

/// <summary>
/// ImpactMarker_Ring诊断工具 - 专门解决圆环标记不显示的问题
/// </summary>
public class ImpactRingDiagnostic : MonoBehaviour
{
    [Header("诊断设置")]
    [Tooltip("是否启用详细日志")]
    public bool enableDetailedLogging = true;

    [Tooltip("测试圆环大小")]
    public float testRingSize = 1.0f;

    [Tooltip("测试圆环高度")]
    public float testRingHeight = 0.05f;

    private BounceImpactMarker impactMarker;

    void Start()
    {
        Debug.Log("=== ImpactMarker_Ring 诊断工具启动 ===");
        Debug.Log("快捷键说明:");
        Debug.Log("  F9: 运行完整诊断");
        Debug.Log("  F10: 强制创建可见圆环");
        Debug.Log("  Shift+F9: 创建简单测试圆环");
        Debug.Log("  Shift+F10: 检查材质和渲染");

        // 查找BounceImpactMarker系统
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (impactMarker == null)
        {
            Debug.LogError("❌ 未找到BounceImpactMarker系统！");
            Debug.LogError("   请确保场景中存在BounceImpactMarkerSystem对象");
        }
        else
        {
            Debug.Log("✅ BounceImpactMarker系统已找到");
            RunInitialDiagnostic();
        }
    }

    void Update()
    {
        // 快捷键处理
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                CreateSimpleTestRing();
            }
            else
            {
                RunFullDiagnostic();
            }
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                CheckMaterialAndRendering();
            }
            else
            {
                ForceCreateVisibleRing();
            }
        }
    }

    /// <summary>
    /// 运行初始诊断
    /// </summary>
    void RunInitialDiagnostic()
    {
        Debug.Log("=== 初始诊断 ===");

        if (impactMarker == null) return;

        Debug.Log($"✅ 系统启用状态: {impactMarker.enableImpactMarkers}");
        Debug.Log($"✅ 基础圆环大小: {impactMarker.baseRingSize}");
        Debug.Log($"✅ 圆环厚度: {impactMarker.ringThickness}");
        Debug.Log($"✅ 标记生命周期: {impactMarker.markerLifetime}秒");
        Debug.Log($"✅ 发光效果: {impactMarker.enableGlow}");
        Debug.Log($"✅ 渐变消失: {impactMarker.enableFadeOut}");

        // 检查当前活动标记
        int activeMarkers = impactMarker.GetActiveMarkerCount();
        Debug.Log($"✅ 当前活动标记数: {activeMarkers}");

        if (activeMarkers == 0)
        {
            Debug.LogWarning("⚠️ 当前没有活动的圆环标记");
            Debug.LogWarning("   尝试发射网球或按F10创建测试圆环");
        }
    }

    /// <summary>
    /// 运行完整诊断
    /// </summary>
    void RunFullDiagnostic()
    {
        Debug.Log("=== 运行完整诊断 ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker系统未找到");
            return;
        }

        // 1. 检查系统设置
        Debug.Log("1. 检查系统设置:");
        Debug.Log($"   启用状态: {impactMarker.enableImpactMarkers}");
        Debug.Log($"   基础大小: {impactMarker.baseRingSize}");
        Debug.Log($"   最小大小: {impactMarker.minRingSize}");
        Debug.Log($"   最大大小: {impactMarker.maxRingSize}");
        Debug.Log($"   圆环厚度: {impactMarker.ringThickness}");

        // 2. 检查场景中的圆环对象
        Debug.Log("2. 检查场景中的圆环对象:");
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ringCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring") || obj.name.Contains("Impact"))
            {
                ringCount++;
                Debug.Log($"   找到对象: {obj.name}");
                Debug.Log($"     位置: {obj.transform.position}");
                Debug.Log($"     激活状态: {obj.activeInHierarchy}");
                Debug.Log($"     缩放: {obj.transform.localScale}");

                // 检查渲染组件
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Debug.Log($"     渲染器: {renderer.enabled}");
                    Debug.Log($"     材质: {renderer.material.name}");
                    Debug.Log($"     颜色: {renderer.material.color}");
                }
                else
                {
                    Debug.LogWarning($"     ⚠️ 缺少Renderer组件");
                }

                // 检查网格
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.mesh != null)
                {
                    Debug.Log($"     网格顶点数: {meshFilter.mesh.vertexCount}");
                    Debug.Log($"     网格三角形数: {meshFilter.mesh.triangles.Length / 3}");
                }
                else
                {
                    Debug.LogWarning($"     ⚠️ 缺少有效的Mesh");
                }
            }
        }

        Debug.Log($"3. 总共找到 {ringCount} 个相关对象");

        // 3. 检查网球状态
        Debug.Log("4. 检查网球状态:");
        CheckTennisBallStatus();

        // 4. 建议解决方案
        Debug.Log("5. 建议解决方案:");
        if (ringCount == 0)
        {
            Debug.LogWarning("   ⚠️ 未找到任何圆环标记对象");
            Debug.LogWarning("   建议: 按F10强制创建测试圆环");
        }
        else
        {
            Debug.Log("   ✅ 找到圆环对象，检查位置和材质设置");
        }
    }

    /// <summary>
    /// 强制创建可见圆环
    /// </summary>
    void ForceCreateVisibleRing()
    {
        Debug.Log("=== 强制创建可见圆环 ===");

        // 创建一个简单的圆柱体作为圆环
        GameObject visibleRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visibleRing.name = "ImpactMarker_Ring_Visible";

        // 设置位置（在摄像机前方）
        Vector3 ringPosition = new Vector3(0, testRingHeight, 2);
        visibleRing.transform.position = ringPosition;

        // 设置为扁平的圆环形状
        visibleRing.transform.localScale = new Vector3(testRingSize, 0.05f, testRingSize);

        // 创建明亮的材质
        Renderer renderer = visibleRing.GetComponent<Renderer>();
        Material ringMaterial = new Material(Shader.Find("Standard"));

        // 设置明亮的红色
        ringMaterial.color = Color.red;
        ringMaterial.EnableKeyword("_EMISSION");
        ringMaterial.SetColor("_EmissionColor", Color.red * 2f);

        // 设置材质属性
        ringMaterial.SetFloat("_Metallic", 0.0f);
        ringMaterial.SetFloat("_Smoothness", 0.8f);

        renderer.material = ringMaterial;

        Debug.Log($"✅ 可见圆环已创建:");
        Debug.Log($"   名称: {visibleRing.name}");
        Debug.Log($"   位置: {ringPosition}");
        Debug.Log($"   大小: {testRingSize}m");
        Debug.Log($"   颜色: 明亮红色");
        Debug.Log($"   应该在摄像机前方清晰可见！");

        // 10秒后销毁
        Destroy(visibleRing, 10f);
    }

    /// <summary>
    /// 创建简单测试圆环
    /// </summary>
    void CreateSimpleTestRing()
    {
        Debug.Log("=== 创建简单测试圆环 ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker系统未找到");
            return;
        }

        // 使用反射调用CreateImpactMarker方法
        MethodInfo createMethod = impactMarker.GetType().GetMethod("CreateImpactMarker",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (createMethod != null)
        {
            Vector3 testPosition = new Vector3(0, 0.01f, 1);
            float testSpeed = 10f;
            Vector3 testVelocity = Vector3.down * testSpeed;

            Debug.Log($"调用CreateImpactMarker方法:");
            Debug.Log($"   位置: {testPosition}");
            Debug.Log($"   速度: {testSpeed}m/s");

            try
            {
                createMethod.Invoke(impactMarker, new object[] { testPosition, testSpeed, testVelocity });
                Debug.Log("✅ CreateImpactMarker方法调用成功");

                // 等待一帧后检查结果
                StartCoroutine(CheckCreationResult());
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ CreateImpactMarker方法调用失败: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("❌ 无法找到CreateImpactMarker方法");
        }
    }

    /// <summary>
    /// 检查创建结果
    /// </summary>
    System.Collections.IEnumerator CheckCreationResult()
    {
        yield return null; // 等待一帧

        Debug.Log("=== 检查圆环创建结果 ===");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundNewRing = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker_Ring") || obj.name.Contains("ImpactRing"))
            {
                foundNewRing = true;
                Debug.Log($"✅ 找到新创建的圆环: {obj.name}");
                Debug.Log($"   位置: {obj.transform.position}");
                Debug.Log($"   激活状态: {obj.activeInHierarchy}");

                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Debug.Log($"   渲染器启用: {renderer.enabled}");
                    Debug.Log($"   材质: {renderer.material.name}");
                    Debug.Log($"   颜色: {renderer.material.color}");
                }
            }
        }

        if (!foundNewRing)
        {
            Debug.LogWarning("⚠️ 未找到新创建的圆环对象");
            Debug.LogWarning("   可能的原因:");
            Debug.LogWarning("   1. 圆环创建失败");
            Debug.LogWarning("   2. 圆环位置不在视野内");
            Debug.LogWarning("   3. 圆环材质透明或不可见");
        }
    }

    /// <summary>
    /// 检查材质和渲染
    /// </summary>
    void CheckMaterialAndRendering()
    {
        Debug.Log("=== 检查材质和渲染 ===");

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int checkedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("Ring"))
            {
                checkedCount++;
                Debug.Log($"检查对象: {obj.name}");

                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = renderer.material;
                    Debug.Log($"   材质名称: {mat.name}");
                    Debug.Log($"   着色器: {mat.shader.name}");
                    Debug.Log($"   主颜色: {mat.color}");
                    Debug.Log($"   渲染队列: {mat.renderQueue}");

                    // 检查透明度设置
                    if (mat.HasProperty("_Mode"))
                    {
                        Debug.Log($"   渲染模式: {mat.GetFloat("_Mode")}");
                    }

                    // 检查发光
                    if (mat.IsKeywordEnabled("_EMISSION"))
                    {
                        Color emissionColor = mat.GetColor("_EmissionColor");
                        Debug.Log($"   发光颜色: {emissionColor}");
                    }

                    // 检查可见性
                    bool isVisible = renderer.isVisible;
                    Debug.Log($"   当前可见: {isVisible}");
                    Debug.Log($"   边界: {renderer.bounds}");
                }
                else
                {
                    Debug.LogWarning($"   ⚠️ 对象 {obj.name} 缺少Renderer组件");
                }

                // 检查网格
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.mesh != null)
                {
                    Mesh mesh = meshFilter.mesh;
                    Debug.Log($"   网格: {mesh.name}");
                    Debug.Log($"   顶点数: {mesh.vertexCount}");
                    Debug.Log($"   边界: {mesh.bounds}");
                }
                else
                {
                    Debug.LogWarning($"   ⚠️ 对象 {obj.name} 缺少有效的Mesh");
                }
            }
        }

        Debug.Log($"总共检查了 {checkedCount} 个相关对象");
    }

    /// <summary>
    /// 检查网球状态
    /// </summary>
    void CheckTennisBallStatus()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ballCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                ballCount++;
                Debug.Log($"   网球: {obj.name}");
                Debug.Log($"     位置: {obj.transform.position}");

                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log($"     速度: {rb.velocity} (大小: {rb.velocity.magnitude:F2}m/s)");
                    Debug.Log($"     高度: {obj.transform.position.y:F3}m");

                    // 检查冲击条件
                    bool heightOK = obj.transform.position.y <= 0.3f;
                    bool velocityOK = rb.velocity.y < -1f;
                    bool speedOK = rb.velocity.magnitude > 2f;

                    Debug.Log($"     冲击条件: 高度({heightOK}) 垂直速度({velocityOK}) 总速度({speedOK})");
                }
            }
        }

        if (ballCount == 0)
        {
            Debug.LogWarning("   ⚠️ 场景中没有找到网球");
            Debug.LogWarning("   建议: 发射网球来触发圆环标记创建");
        }
        else
        {
            Debug.Log($"   找到 {ballCount} 个网球");
        }
    }

    void OnDestroy()
    {
        Debug.Log("ImpactRingDiagnostic 诊断工具已关闭");
    }
}