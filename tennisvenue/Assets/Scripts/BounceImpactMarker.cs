using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 反弹冲击标记系统 - 在网球第一次落地时创建圆环标记，大小反映落地速度
/// </summary>
public class BounceImpactMarker : MonoBehaviour
{
    [Header("圆环标记设置")]
    [Tooltip("是否启用反弹冲击标记")]
    public bool enableImpactMarkers = true;

    [Tooltip("圆环标记保持时间（秒）")]
    public float markerLifetime = 15f;

    [Tooltip("基础圆环大小")]
    public float baseRingSize = 0.5f;

    [Tooltip("速度影响系数")]
    public float velocityScale = 0.1f;

    [Tooltip("最小圆环大小")]
    public float minRingSize = 0.3f;

    [Tooltip("最大圆环大小")]
    public float maxRingSize = 2.0f;

    [Tooltip("圆环厚度")]
    public float ringThickness = 0.05f;

    [Header("视觉效果")]
    [Tooltip("圆环颜色")]
    public Color ringColor = Color.red;

    [Tooltip("是否启用发光效果")]
    public bool enableGlow = true;

    [Tooltip("发光强度")]
    public float glowIntensity = 0.8f;

    [Tooltip("是否启用渐变消失")]
    public bool enableFadeOut = true;

    [Header("速度分级显示")]
    [Tooltip("低速阈值 (m/s)")]
    public float lowSpeedThreshold = 5f;

    [Tooltip("中速阈值 (m/s)")]
    public float mediumSpeedThreshold = 10f;

    [Tooltip("高速阈值 (m/s)")]
    public float highSpeedThreshold = 15f;

    // 追踪已标记的球体，避免重复标记
    private Dictionary<GameObject, bool> markedBalls = new Dictionary<GameObject, bool>();

    // 存储所有创建的标记，便于管理
    private List<GameObject> activeMarkers = new List<GameObject>();

    void Start()
    {
        Debug.Log("=== Bounce Impact Marker System Started ===");
        Debug.Log("Press F3 to toggle impact markers");
        Debug.Log("Press F4 to clear all impact markers");
        Debug.Log("Press F5 to create test impact marker");
    }

    void Update()
    {
        // 监控所有网球的第一次落地
        if (enableImpactMarkers)
        {
            MonitorTennisBallImpacts();
        }

        // 快捷键控制
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ToggleImpactMarkers();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            ClearAllImpactMarkers();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            CreateTestImpactMarker();
        }

        // 清理已销毁的球体记录
        CleanupDestroyedBalls();
    }

    /// <summary>
    /// 监控网球的冲击落地
    /// </summary>
    void MonitorTennisBallImpacts()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // 如果这个球还没有被标记过
                    if (!markedBalls.ContainsKey(obj))
                    {
                        markedBalls[obj] = false; // false表示尚未落地标记
                    }

                    // 检查是否应该创建冲击标记
                    if (!markedBalls[obj])
                    {
                        CheckForImpact(obj, rb);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 检查网球是否发生冲击落地
    /// </summary>
    void CheckForImpact(GameObject ball, Rigidbody rb)
    {
        Vector3 position = ball.transform.position;
        Vector3 velocity = rb.velocity;
        float speed = velocity.magnitude;

        // 检查异常位置 - 网球掉落到地面以下太深
        if (position.y < -5f)
        {
            Debug.LogWarning($"⚠️ 异常网球位置检测: {ball.name} 高度{position.y:F2}m - 移除追踪");
            markedBalls.Remove(ball);
            return;
        }

        // 检查异常速度 - 速度过快可能是物理系统错误
        if (speed > 50f)
        {
            Debug.LogWarning($"⚠️ 异常网球速度检测: {ball.name} 速度{speed:F2}m/s - 移除追踪");
            markedBalls.Remove(ball);
            return;
        }

        // 检查网球是否在合理的场地范围内
        if (Mathf.Abs(position.x) > 10f || Mathf.Abs(position.z) > 10f)
        {
            Debug.LogWarning($"⚠️ 网球超出场地范围: {ball.name} 位置{position} - 移除追踪");
            markedBalls.Remove(ball);
            return;
        }

        // 添加详细调试信息（降低频率避免日志过多）
        if (Time.frameCount % 30 == 0) // 每半秒输出一次状态
        {
            Debug.Log($"🔍 Checking ball {ball.name}: Height={position.y:F3}m, Speed={speed:F2}m/s, VelY={velocity.y:F2}");
        }

        // 检测冲击条件（降低阈值使其更容易触发）：
        // 1. 球接近地面（高度低于0.5m，原来是0.3m）
        // 2. 有向下的速度分量（降低到-0.5，原来是-1f）
        // 3. 速度足够大（降低到1.5，原来是2f）
        // 4. 球在合理高度范围内（不能太低）
        bool heightCondition = position.y <= 0.5f && position.y >= -1f;
        bool velocityCondition = velocity.y < -0.5f;
        bool speedCondition = speed > 1.5f;

        bool isImpacting = heightCondition && velocityCondition && speedCondition;

        // 输出条件检查结果（只在接近触发时输出）
        if (heightCondition || velocityCondition || speedCondition)
        {
            Debug.Log($"🎾 Ball {ball.name} conditions: Height({heightCondition}) Velocity({velocityCondition}) Speed({speedCondition}) = Impact({isImpacting})");
        }

        if (isImpacting)
        {
            Debug.Log($"⚡ Impact conditions met for {ball.name}! Performing raycast...");

            // 使用射线检测确认地面接触
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, 1.0f)) // 增加射线距离
            {
                Debug.Log($"🎯 Raycast hit: {hit.collider.name} at {hit.point}");

                if (hit.collider.name.Contains("Floor") || hit.collider.name.Contains("Ground"))
                {
                    // 创建冲击标记
                    CreateImpactMarker(hit.point, speed, velocity);
                    markedBalls[ball] = true; // 标记为已处理

                    Debug.Log($"🎯 Impact detected - Speed: {speed:F2}m/s at {hit.point}");
                }
                else
                {
                    Debug.Log($"⚠️ Raycast hit non-floor object: {hit.collider.name}");
                }
            }
            else
            {
                Debug.Log($"❌ Raycast missed - no ground detected below {ball.name}");

                // 如果射线检测失败，但条件满足，直接在球的位置创建标记
                Vector3 groundPoint = new Vector3(position.x, 0.01f, position.z);
                CreateImpactMarker(groundPoint, speed, velocity);
                markedBalls[ball] = true;

                Debug.Log($"🎯 Fallback impact marker created at {groundPoint}");
            }
        }
    }

    /// <summary>
    /// 创建冲击标记圆环
    /// </summary>
    void CreateImpactMarker(Vector3 impactPoint, float impactSpeed, Vector3 impactVelocity)
    {
        Debug.Log($"=== Creating Impact Marker ===");
        Debug.Log($"Impact point: {impactPoint}");
        Debug.Log($"Impact speed: {impactSpeed:F2}m/s");

        // 计算圆环大小（基于速度）
        float ringSize = CalculateRingSize(impactSpeed);

        // 创建圆环对象
        GameObject ringMarker = CreateRingGeometry(ringSize);
        ringMarker.name = "ImpactMarker_Ring";

        // 设置位置（稍微抬高避免Z-fighting）
        ringMarker.transform.position = impactPoint + Vector3.up * 0.01f;

        // 设置材质和颜色
        SetupRingMaterial(ringMarker, impactSpeed);

        // 添加到活动标记列表
        activeMarkers.Add(ringMarker);

        // 设置自动销毁
        if (enableFadeOut)
        {
            StartCoroutine(FadeOutMarker(ringMarker));
        }
        else
        {
            Destroy(ringMarker, markerLifetime);
        }

        // 输出详细信息
        LogImpactDetails(impactPoint, impactSpeed, ringSize);
    }

    /// <summary>
    /// 根据冲击速度计算圆环大小
    /// </summary>
    float CalculateRingSize(float speed)
    {
        // 基础大小 + 速度影响
        float size = baseRingSize + (speed * velocityScale);

        // 限制在最小和最大值之间
        size = Mathf.Clamp(size, minRingSize, maxRingSize);

        return size;
    }

    /// <summary>
    /// 创建圆环几何体
    /// </summary>
    GameObject CreateRingGeometry(float outerRadius)
    {
        GameObject ring = new GameObject("ImpactRing");

        // 创建圆环网格
        MeshFilter meshFilter = ring.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = ring.AddComponent<MeshRenderer>();

        // 生成圆环网格
        Mesh ringMesh = GenerateRingMesh(outerRadius, outerRadius - ringThickness, 32);
        meshFilter.mesh = ringMesh;

        return ring;
    }

    /// <summary>
    /// 生成圆环网格
    /// </summary>
    Mesh GenerateRingMesh(float outerRadius, float innerRadius, int segments)
    {
        Mesh mesh = new Mesh();

        // 顶点数组
        Vector3[] vertices = new Vector3[segments * 2];
        Vector2[] uvs = new Vector2[segments * 2];
        int[] triangles = new int[segments * 6];

        float angleStep = 2f * Mathf.PI / segments;

        // 生成顶点
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            // 外圆顶点
            vertices[i * 2] = new Vector3(cos * outerRadius, 0, sin * outerRadius);
            uvs[i * 2] = new Vector2(0, (float)i / segments);

            // 内圆顶点
            vertices[i * 2 + 1] = new Vector3(cos * innerRadius, 0, sin * innerRadius);
            uvs[i * 2 + 1] = new Vector2(1, (float)i / segments);
        }

        // 生成三角形
        for (int i = 0; i < segments; i++)
        {
            int current = i * 2;
            int next = ((i + 1) % segments) * 2;

            // 第一个三角形
            triangles[i * 6] = current;
            triangles[i * 6 + 1] = next;
            triangles[i * 6 + 2] = current + 1;

            // 第二个三角形
            triangles[i * 6 + 3] = current + 1;
            triangles[i * 6 + 4] = next;
            triangles[i * 6 + 5] = next + 1;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// 设置圆环材质
    /// </summary>
    void SetupRingMaterial(GameObject ring, float impactSpeed)
    {
        MeshRenderer renderer = ring.GetComponent<MeshRenderer>();

        // 创建材质
        Material ringMaterial = new Material(Shader.Find("Standard"));

        // 根据速度设置颜色
        Color speedColor = GetSpeedColor(impactSpeed);
        ringMaterial.color = speedColor;

        // 设置材质属性
        ringMaterial.SetFloat("_Metallic", 0.0f);
        ringMaterial.SetFloat("_Smoothness", 0.8f);

        // 添加发光效果
        if (enableGlow)
        {
            ringMaterial.EnableKeyword("_EMISSION");
            ringMaterial.SetColor("_EmissionColor", speedColor * glowIntensity);
        }

        // 设置透明度
        ringMaterial.SetFloat("_Mode", 3); // Transparent mode
        ringMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        ringMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ringMaterial.SetInt("_ZWrite", 0);
        ringMaterial.DisableKeyword("_ALPHATEST_ON");
        ringMaterial.EnableKeyword("_ALPHABLEND_ON");
        ringMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        ringMaterial.renderQueue = 3000;

        renderer.material = ringMaterial;
    }

    /// <summary>
    /// 根据速度获取颜色
    /// </summary>
    Color GetSpeedColor(float speed)
    {
        if (speed < lowSpeedThreshold)
        {
            return Color.green; // 低速 - 绿色
        }
        else if (speed < mediumSpeedThreshold)
        {
            return Color.yellow; // 中速 - 黄色
        }
        else if (speed < highSpeedThreshold)
        {
            return Color.red; // 高速 - 红色
        }
        else
        {
            return Color.magenta; // 极高速 - 紫色
        }
    }

    /// <summary>
    /// 渐变消失效果
    /// </summary>
    System.Collections.IEnumerator FadeOutMarker(GameObject marker)
    {
        MeshRenderer renderer = marker.GetComponent<MeshRenderer>();
        Material material = renderer.material;
        Color originalColor = material.color;

        float fadeTime = markerLifetime * 0.3f; // 最后30%时间用于渐变
        float waitTime = markerLifetime - fadeTime;

        // 等待大部分时间
        yield return new WaitForSeconds(waitTime);

        // 开始渐变
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            Color newColor = originalColor;
            newColor.a = alpha;
            material.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 销毁对象
        activeMarkers.Remove(marker);
        Destroy(marker);
    }

    /// <summary>
    /// 输出冲击详细信息
    /// </summary>
    void LogImpactDetails(Vector3 point, float speed, float ringSize)
    {
        string speedCategory = GetSpeedCategory(speed);

        Debug.Log($"🎯 Impact Marker Created:");
        Debug.Log($"  📍 Position: ({point.x:F2}, {point.z:F2})");
        Debug.Log($"  ⚡ Speed: {speed:F2}m/s ({speedCategory})");
        Debug.Log($"  ⭕ Ring Size: {ringSize:F2}m");
        Debug.Log($"  🎨 Color: {GetSpeedColor(speed)}");
        Debug.Log($"  ⏱️ Lifetime: {markerLifetime}s");
    }

    /// <summary>
    /// 获取速度分类
    /// </summary>
    string GetSpeedCategory(float speed)
    {
        if (speed < lowSpeedThreshold) return "Low";
        else if (speed < mediumSpeedThreshold) return "Medium";
        else if (speed < highSpeedThreshold) return "High";
        else return "Extreme";
    }

    /// <summary>
    /// 切换冲击标记功能
    /// </summary>
    public void ToggleImpactMarkers()
    {
        enableImpactMarkers = !enableImpactMarkers;
        Debug.Log($"Impact markers: {(enableImpactMarkers ? "Enabled" : "Disabled")}");
    }

    /// <summary>
    /// 清除所有冲击标记
    /// </summary>
    public void ClearAllImpactMarkers()
    {
        foreach (GameObject marker in activeMarkers)
        {
            if (marker != null)
            {
                Destroy(marker);
            }
        }
        activeMarkers.Clear();
        markedBalls.Clear();

        Debug.Log("All impact markers cleared");
    }

    /// <summary>
    /// 创建测试冲击标记
    /// </summary>
    void CreateTestImpactMarker()
    {
        Vector3 testPosition = new Vector3(0, 0.05f, 2); // 提高位置
        float testSpeed = Random.Range(8f, 15f); // 增加测试速度

        Debug.Log($"=== Creating Test Impact Marker ===");
        Debug.Log($"Position: {testPosition}");
        Debug.Log($"Speed: {testSpeed:F2}m/s");

        // 创建一个更大更明显的测试标记
        CreateLargeTestMarker(testPosition, testSpeed);

        Debug.Log($"Test impact marker created with speed: {testSpeed:F2}m/s");
    }

    /// <summary>
    /// 创建大的测试标记（更容易看到）
    /// </summary>
    void CreateLargeTestMarker(Vector3 position, float speed)
    {
        // 创建一个简单的圆柱体作为测试圆环
        GameObject testRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        testRing.name = "TestImpactMarker_Large";

        // 设置位置
        testRing.transform.position = position;

        // 设置大小 - 扁平的大圆环
        float ringSize = Mathf.Clamp(baseRingSize + (speed * velocityScale), 1.0f, 3.0f); // 更大的尺寸
        testRing.transform.localScale = new Vector3(ringSize, 0.1f, ringSize);

        // 设置明亮的材质
        Renderer renderer = testRing.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));

        // 根据速度设置颜色
        Color speedColor = GetSpeedColor(speed);
        mat.color = speedColor;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", speedColor * 3f); // 更强的发光

        renderer.material = mat;

        // 添加到活动标记列表
        activeMarkers.Add(testRing);

        // 15秒后销毁
        Destroy(testRing, markerLifetime);

        Debug.Log($"✅ Large test marker created:");
        Debug.Log($"  Position: {position}");
        Debug.Log($"  Size: {ringSize:F2}m");
        Debug.Log($"  Color: {speedColor}");
        Debug.Log($"  Should be visible as a bright {speedColor} cylinder!");
    }

    /// <summary>
    /// 清理已销毁的球体记录
    /// </summary>
    void CleanupDestroyedBalls()
    {
        List<GameObject> ballsToRemove = new List<GameObject>();

        foreach (var ball in markedBalls.Keys)
        {
            if (ball == null)
            {
                ballsToRemove.Add(ball);
            }
        }

        foreach (var ball in ballsToRemove)
        {
            markedBalls.Remove(ball);
        }
    }

    /// <summary>
    /// 获取当前活动标记数量
    /// </summary>
    public int GetActiveMarkerCount()
    {
        return activeMarkers.Count;
    }

    /// <summary>
    /// 获取系统状态信息
    /// </summary>
    public string GetSystemStatus()
    {
        return $"Impact Markers: {(enableImpactMarkers ? "ON" : "OFF")}, " +
               $"Active: {GetActiveMarkerCount()}, " +
               $"Tracked Balls: {markedBalls.Count}";
    }
}