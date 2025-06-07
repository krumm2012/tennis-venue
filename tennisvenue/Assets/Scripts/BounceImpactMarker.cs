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
    public float baseRingSize = 0.335f;

    [Tooltip("速度影响系数")]
    public float velocityScale = 0.02f;

    [Tooltip("最小圆环大小")]
    public float minRingSize = 0.3f;

    [Tooltip("最大圆环大小")]
    public float maxRingSize = 0.5f;

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

    [Header("调试设置")]
    [Tooltip("启用详细调试日志")]
    public bool enableDetailedLogging = false;
    [Tooltip("启用条件检查日志")]
    public bool enableConditionLogging = false;
    [Tooltip("日志输出间隔（帧数）")]
    public int logFrameInterval = 60; // 每60帧（约1秒）输出一次

    [Header("位置修复集成")]
    [Tooltip("启用位置修复功能")]
    public bool enablePositionFix = true;
    public ImpactMarkerPositionFixer positionFixer;

    // 追踪已标记的球体，避免重复标记
    private Dictionary<GameObject, bool> markedBalls = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, int> ballLastLogFrame = new Dictionary<GameObject, int>(); // 记录每个球最后日志输出的帧数

    // 存储所有创建的标记，便于管理
    private List<GameObject> activeMarkers = new List<GameObject>();

    void Start()
    {
        Debug.Log("=== Bounce Impact Marker System Started ===");
        Debug.Log("Press F3 to toggle impact markers");
        Debug.Log("Press F4 to clear all impact markers");
        Debug.Log("Press F5 to create test impact marker");

        // 查找位置修复器
        if (positionFixer == null)
        {
            positionFixer = FindObjectOfType<ImpactMarkerPositionFixer>();
            if (positionFixer != null)
            {
                Debug.Log("✅ 位置修复器已连接");
            }
        }
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
            if (enableDetailedLogging)
            {
                Debug.LogWarning($"⚠️ 异常网球位置检测: {ball.name} 高度{position.y:F2}m - 移除追踪");
            }
            markedBalls.Remove(ball);
            ballLastLogFrame.Remove(ball);
            return;
        }

        // 检查异常速度 - 速度过快可能是物理系统错误
        if (speed > 50f)
        {
            if (enableDetailedLogging)
            {
                Debug.LogWarning($"⚠️ 异常网球速度检测: {ball.name} 速度{speed:F2}m/s - 移除追踪");
            }
            markedBalls.Remove(ball);
            ballLastLogFrame.Remove(ball);
            return;
        }

        // 检查网球是否在合理的场地范围内
        if (Mathf.Abs(position.x) > 10f || Mathf.Abs(position.z) > 10f)
        {
            if (enableDetailedLogging)
            {
                Debug.LogWarning($"⚠️ 网球超出场地范围: {ball.name} 位置{position} - 移除追踪");
            }
            markedBalls.Remove(ball);
            ballLastLogFrame.Remove(ball);
            return;
        }

        // 智能日志控制 - 避免重复输出
        bool shouldLogForThisBall = false;
        int currentFrame = Time.frameCount;

        if (!ballLastLogFrame.ContainsKey(ball))
        {
            ballLastLogFrame[ball] = currentFrame;
            shouldLogForThisBall = true;
        }
        else if (currentFrame - ballLastLogFrame[ball] >= logFrameInterval)
        {
            ballLastLogFrame[ball] = currentFrame;
            shouldLogForThisBall = true;
        }

        // 详细调试信息（降低频率避免日志过多）
        if (enableDetailedLogging && shouldLogForThisBall)
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

        // 只在启用条件日志且需要输出时才显示条件检查结果
        if (enableConditionLogging && shouldLogForThisBall && (heightCondition || velocityCondition || speedCondition))
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
                if (enableDetailedLogging)
                {
                    Debug.Log($"🎯 Raycast hit: {hit.collider.name} at {hit.point}");
                }

                if (hit.collider.name.Contains("Floor") || hit.collider.name.Contains("Ground"))
                {
                    // 创建冲击标记
                    CreateImpactMarker(hit.point, speed, velocity);
                    markedBalls[ball] = true; // 标记为已处理
                    ballLastLogFrame.Remove(ball); // 清理日志记录

                    Debug.Log($"🎯 Impact detected - Speed: {speed:F2}m/s at {hit.point}");
                }
                else if (enableDetailedLogging)
                {
                    Debug.Log($"⚠️ Raycast hit non-floor object: {hit.collider.name}");
                }
            }
            else
            {
                if (enableDetailedLogging)
                {
                    Debug.Log($"❌ Raycast missed - no ground detected below {ball.name}");
                }

                // 如果射线检测失败，但条件满足，直接在球的位置创建标记
                Vector3 groundPoint = new Vector3(position.x, 0.01f, position.z);
                CreateImpactMarker(groundPoint, speed, velocity);
                markedBalls[ball] = true;
                ballLastLogFrame.Remove(ball); // 清理日志记录

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
        Debug.Log($"Original impact point: {impactPoint}");
        Debug.Log($"Impact speed: {impactSpeed:F2}m/s");

        // 使用位置修复器修正圆环位置（如果可用）
        Vector3 correctedPosition = impactPoint;
        if (enablePositionFix && positionFixer != null)
        {
            // 查找触发这个标记的网球
            GameObject triggerBall = FindRecentlyLandedBall(impactPoint);
            if (triggerBall != null)
            {
                correctedPosition = positionFixer.GetCorrectedImpactPosition(impactPoint, triggerBall);
                Debug.Log($"🔧 Position corrected from {impactPoint} to {correctedPosition}");
            }
        }

        // 计算圆环大小（基于速度）
        float ringSize = CalculateRingSize(impactSpeed);

        // 创建圆环对象 - 使用简单圆柱体确保可见性
        GameObject ringMarker = CreateVisibleRingGeometry(ringSize);
        ringMarker.name = "ImpactMarker_Ring";

        // 设置修正后的位置（明显抬高确保可见）
        ringMarker.transform.position = correctedPosition + Vector3.up * 0.1f;

        // 设置材质和颜色 - 使用不透明材质
        SetupEnhancedRingMaterial(ringMarker, impactSpeed);

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
        LogImpactDetails(correctedPosition, impactSpeed, ringSize);
    }

    /// <summary>
    /// 查找最近落地的网球（用于位置修复）
    /// </summary>
    GameObject FindRecentlyLandedBall(Vector3 impactPoint)
    {
        GameObject closestBall = null;
        float closestDistance = float.MaxValue;

        foreach (var ballPair in markedBalls)
        {
            GameObject ball = ballPair.Key;
            if (ball != null)
            {
                float distance = Vector3.Distance(ball.transform.position, impactPoint);
                if (distance < closestDistance && distance < 2f) // 2米范围内
                {
                    closestDistance = distance;
                    closestBall = ball;
                }
            }
        }

        if (closestBall != null)
        {
            Debug.Log($"🎾 找到关联网球: {closestBall.name}, 距离: {closestDistance:F2}m");
        }

        return closestBall;
    }

    /// <summary>
    /// 创建可见圆环几何体 - 使用简单圆柱体
    /// </summary>
    GameObject CreateVisibleRingGeometry(float outerRadius)
    {
        GameObject ring = new GameObject("ImpactRing");

        // 外圆（圆柱体）
        GameObject outerCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        outerCylinder.transform.SetParent(ring.transform);
        outerCylinder.transform.localPosition = Vector3.zero;
        outerCylinder.transform.localScale = new Vector3(outerRadius * 2, 0.02f, outerRadius * 2);

        // 内圆（圆柱体）- 用作减去区域
        GameObject innerCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        innerCylinder.transform.SetParent(ring.transform);
        innerCylinder.transform.localPosition = Vector3.up * 0.001f; // 稍微高一点
        innerCylinder.transform.localScale = new Vector3((outerRadius - ringThickness) * 2, 0.03f, (outerRadius - ringThickness) * 2);

        // 为内圆设置透明材质来"挖洞"
        MeshRenderer innerRenderer = innerCylinder.GetComponent<MeshRenderer>();
        Material holeMaterial = new Material(Shader.Find("Standard"));
        holeMaterial.color = new Color(1, 1, 1, 0); // 完全透明
        holeMaterial.SetFloat("_Mode", 3); // Transparent
        holeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        holeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        holeMaterial.SetInt("_ZWrite", 0);
        holeMaterial.EnableKeyword("_ALPHABLEND_ON");
        holeMaterial.renderQueue = 2999; // 比外圆稍早渲染
        innerRenderer.material = holeMaterial;

        // 移除碰撞器（不需要物理碰撞）
        if (outerCylinder.GetComponent<Collider>())
            DestroyImmediate(outerCylinder.GetComponent<Collider>());
        if (innerCylinder.GetComponent<Collider>())
            DestroyImmediate(innerCylinder.GetComponent<Collider>());

        return ring;
    }

    /// <summary>
    /// 设置增强圆环材质 - 确保可见性
    /// </summary>
    void SetupEnhancedRingMaterial(GameObject ring, float impactSpeed)
    {
        // 获取外圆的渲染器
        Transform outerCylinder = ring.transform.GetChild(0);
        MeshRenderer renderer = outerCylinder.GetComponent<MeshRenderer>();

        // 创建不透明材质
        Material ringMaterial = new Material(Shader.Find("Standard"));

        // 根据速度设置颜色 - 更鲜艳
        Color speedColor = GetEnhancedSpeedColor(impactSpeed);
        ringMaterial.color = speedColor;

        // 设置材质属性 - 不透明且明亮
        ringMaterial.SetFloat("_Metallic", 0.2f);
        ringMaterial.SetFloat("_Smoothness", 0.9f);
        ringMaterial.SetFloat("_Mode", 0); // Opaque mode

        // 添加强发光效果
        if (enableGlow)
        {
            ringMaterial.EnableKeyword("_EMISSION");
            ringMaterial.SetColor("_EmissionColor", speedColor * (glowIntensity * 2)); // 双倍发光强度
        }

        renderer.material = ringMaterial;

        Debug.Log($"圆环材质设置完成 - 颜色: {speedColor}, 速度: {impactSpeed:F2}m/s");
    }

    /// <summary>
    /// 获取增强速度颜色 - 更明显的颜色
    /// </summary>
    Color GetEnhancedSpeedColor(float speed)
    {
        Color baseColor;
        if (speed < lowSpeedThreshold)
        {
            baseColor = new Color(0, 1, 0, 1); // 鲜绿色
        }
        else if (speed < mediumSpeedThreshold)
        {
            baseColor = new Color(1, 1, 0, 1); // 鲜黄色
        }
        else if (speed < highSpeedThreshold)
        {
            baseColor = new Color(1, 0.2f, 0, 1); // 鲜橙红色
        }
        else
        {
            baseColor = new Color(1, 0, 1, 1); // 鲜紫色
        }

        // 增加亮度
        return baseColor * 1.2f;
    }

    /// <summary>
    /// 根据速度获取颜色 - 保持向后兼容
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
    /// 渐变消失效果
    /// </summary>
    System.Collections.IEnumerator FadeOutMarker(GameObject marker)
    {
        // 获取外圆（第一个子对象）的MeshRenderer
        Transform outerCylinder = marker.transform.GetChild(0);
        MeshRenderer renderer = outerCylinder.GetComponent<MeshRenderer>();

        if (renderer == null)
        {
            Debug.LogWarning($"⚠️ No MeshRenderer found on {marker.name} outer cylinder, skipping fade effect");
            yield return new WaitForSeconds(markerLifetime);
            activeMarkers.Remove(marker);
            Destroy(marker);
            yield break;
        }

        Material material = renderer.material;
        Color originalColor = material.color;
        Vector3 originalScale = marker.transform.localScale;

        float fadeTime = markerLifetime * 0.3f; // 最后30%时间用于渐变
        float waitTime = markerLifetime - fadeTime;

        // 等待大部分时间
        yield return new WaitForSeconds(waitTime);

        // 开始渐变 - 使用缩放+颜色变暗的组合效果
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            float progress = elapsedTime / fadeTime;

            // 缩放效果：从100%缩放到0%
            float scaleMultiplier = Mathf.Lerp(1f, 0f, progress);
            marker.transform.localScale = originalScale * scaleMultiplier;

            // 颜色变暗效果：保持颜色但降低亮度
            float brightnessMultiplier = Mathf.Lerp(1f, 0.1f, progress);
            Color newColor = originalColor * brightnessMultiplier;
            newColor.a = 1f; // 保持完全不透明
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
        Debug.Log($"  🎨 Color: {GetEnhancedSpeedColor(speed)}");
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
        ballLastLogFrame.Clear(); // 清理日志记录

        Debug.Log("All impact markers and logs cleared");
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
        Color speedColor = GetEnhancedSpeedColor(speed);
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
            ballLastLogFrame.Remove(ball); // 同时清理日志记录
        }

        // 额外清理ballLastLogFrame中可能的孤立记录
        List<GameObject> logBallsToRemove = new List<GameObject>();
        foreach (var ball in ballLastLogFrame.Keys)
        {
            if (ball == null)
            {
                logBallsToRemove.Add(ball);
            }
        }

        foreach (var ball in logBallsToRemove)
        {
            ballLastLogFrame.Remove(ball);
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
               $"Tracked Balls: {markedBalls.Count}, " +
               $"DetailLog: {(enableDetailedLogging ? "ON" : "OFF")}, " +
               $"ConditionLog: {(enableConditionLogging ? "ON" : "OFF")}";
    }
}