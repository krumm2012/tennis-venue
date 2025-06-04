using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 轨迹线拖动控制器
/// 允许用户通过拖动轨迹线来即时调节发球机参数和落点位置
/// </summary>
public class TrajectoryDragController : MonoBehaviour
{
    [Header("组件引用")]
    public BallLauncher ballLauncher;
    public Camera mainCamera;
    public LineRenderer trajectoryLine;

    [Header("拖动设置")]
    public float detectionRadius = 0.5f; // 轨迹线检测半径
    public LayerMask groundLayerMask = 1; // 地面图层
    public bool enableDragControl = true;
    public bool showDebugInfo = false;

    [Header("参数限制")]
    public float minAngle = 15f;
    public float maxAngle = 75f;
    public float minSpeed = 10f;
    public float maxSpeed = 30f;
    public float minDirection = -45f;
    public float maxDirection = 45f;

    [Header("视觉反馈")]
    public GameObject dragIndicator; // 拖动指示器
    public Color highlightColor = Color.yellow;
    public Color normalColor = Color.white;

    private bool isDragging = false;
    private Vector3 dragStartPoint;
    private Vector3 targetLandingPoint;
    private List<Vector3> currentTrajectoryPoints;
    private int closestPointIndex = -1;

    // 原始参数备份
    private float originalAngle;
    private float originalSpeed;
    private float originalDirection;

    /// <summary>
    /// 公共属性：检查是否正在拖动轨迹线
    /// </summary>
    public bool IsDragging => isDragging;

    void Start()
    {
        // 自动查找组件
        if (ballLauncher == null)
            ballLauncher = FindObjectOfType<BallLauncher>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (trajectoryLine == null && ballLauncher != null)
            trajectoryLine = ballLauncher.trajectoryLine;

        // 创建拖动指示器
        CreateDragIndicator();

        Debug.Log("✅ TrajectoryDragController initialized");
    }

    void Update()
    {
        if (!enableDragControl || ballLauncher == null || trajectoryLine == null)
            return;

        HandleMouseInput();
        UpdateDragIndicator();
    }

    /// <summary>
    /// 处理鼠标输入
    /// </summary>
    void HandleMouseInput()
    {
        // 鼠标按下 - 检测是否点击在轨迹线上
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            if (IsMouseOnTrajectory(mouseWorldPos, out closestPointIndex))
            {
                StartDragging(mouseWorldPos);
            }
        }

        // 鼠标拖动 - 更新目标位置
        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            UpdateDragTarget(mouseWorldPos);
        }

        // 鼠标释放 - 结束拖动
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDragging();
        }

        // ESC键取消拖动
        if (Input.GetKeyDown(KeyCode.Escape) && isDragging)
        {
            CancelDragging();
        }
    }

    /// <summary>
    /// 获取鼠标在世界坐标中的位置（投射到地面）
    /// </summary>
    Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 尝试投射到地面
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            return hit.point;
        }

        // 如果没有击中地面，投射到Y=0平面
        float distance = -ray.origin.y / ray.direction.y;
        if (distance > 0)
        {
            return ray.origin + ray.direction * distance;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// 检测鼠标是否在轨迹线上
    /// </summary>
    bool IsMouseOnTrajectory(Vector3 mouseWorldPos, out int closestIndex)
    {
        closestIndex = -1;

        if (trajectoryLine.positionCount < 2)
            return false;

        float minDistance = float.MaxValue;

        // 获取当前轨迹线的所有点
        currentTrajectoryPoints = new List<Vector3>();
        for (int i = 0; i < trajectoryLine.positionCount; i++)
        {
            currentTrajectoryPoints.Add(trajectoryLine.GetPosition(i));
        }

        // 检查每个线段
        for (int i = 0; i < currentTrajectoryPoints.Count - 1; i++)
        {
            Vector3 lineStart = currentTrajectoryPoints[i];
            Vector3 lineEnd = currentTrajectoryPoints[i + 1];

            // 计算点到线段的最短距离
            float distance = DistancePointToLineSegment(mouseWorldPos, lineStart, lineEnd);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        bool isOnTrajectory = minDistance <= detectionRadius;

        if (showDebugInfo && isOnTrajectory)
        {
            Debug.Log($"🎯 Mouse on trajectory: distance={minDistance:F2}, index={closestIndex}");
        }

        return isOnTrajectory;
    }

    /// <summary>
    /// 计算点到线段的最短距离
    /// </summary>
    float DistancePointToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;

        if (lineLength < 0.001f)
            return Vector3.Distance(point, lineStart);

        lineDirection /= lineLength;

        Vector3 pointToStart = point - lineStart;
        float projection = Vector3.Dot(pointToStart, lineDirection);

        // 限制投影在线段范围内
        projection = Mathf.Clamp(projection, 0f, lineLength);

        Vector3 closestPoint = lineStart + lineDirection * projection;
        return Vector3.Distance(point, closestPoint);
    }

    /// <summary>
    /// 开始拖动
    /// </summary>
    void StartDragging(Vector3 mouseWorldPos)
    {
        isDragging = true;
        dragStartPoint = mouseWorldPos;

        // 备份原始参数
        originalAngle = ballLauncher.angleSlider?.value ?? 45f;
        originalSpeed = ballLauncher.speedSlider?.value ?? 20f;
        originalDirection = ballLauncher.directionSlider?.value ?? 0f;

        // 高亮轨迹线
        if (trajectoryLine != null)
        {
            trajectoryLine.startColor = highlightColor;
            trajectoryLine.endColor = highlightColor;
        }

        Debug.Log("🎯 Started trajectory dragging");
    }

    /// <summary>
    /// 更新拖动目标
    /// </summary>
    void UpdateDragTarget(Vector3 mouseWorldPos)
    {
        targetLandingPoint = mouseWorldPos;

        // 根据目标落点计算发球机参数
        CalculateLaunchParameters(targetLandingPoint);

        if (showDebugInfo)
        {
            Debug.Log($"🎯 Drag target: {targetLandingPoint}");
        }
    }

    /// <summary>
    /// 根据目标落点计算发球机参数
    /// </summary>
    void CalculateLaunchParameters(Vector3 targetPoint)
    {
        if (ballLauncher == null || ballLauncher.launchPoint == null)
            return;

        Vector3 launchPos = ballLauncher.launchPoint.position;
        Vector3 toTarget = targetPoint - launchPos;

        // 计算水平距离和高度差
        float horizontalDistance = new Vector3(toTarget.x, 0, toTarget.z).magnitude;
        float heightDifference = toTarget.y;

        // 计算方向角
        float newDirection = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
        newDirection = Mathf.Clamp(newDirection, minDirection, maxDirection);

        // 使用抛物线方程计算角度和速度
        // 尝试多个角度，找到最合适的组合
        float bestAngle = originalAngle;
        float bestSpeed = originalSpeed;
        float minError = float.MaxValue;

        for (float testAngle = minAngle; testAngle <= maxAngle; testAngle += 1f)
        {
            for (float testSpeed = minSpeed; testSpeed <= maxSpeed; testSpeed += 0.5f)
            {
                Vector3 calculatedLanding = CalculateLandingPoint(launchPos, testAngle, testSpeed, newDirection);
                float error = Vector3.Distance(calculatedLanding, targetPoint);

                if (error < minError)
                {
                    minError = error;
                    bestAngle = testAngle;
                    bestSpeed = testSpeed;
                }
            }
        }

        // 应用计算出的参数
        ApplyLaunchParameters(bestAngle, bestSpeed, newDirection);
    }

    /// <summary>
    /// 计算给定参数下的落点位置
    /// </summary>
    Vector3 CalculateLandingPoint(Vector3 launchPos, float angle, float speed, float direction)
    {
        // 计算发射方向
        float angleRad = angle * Mathf.Deg2Rad;
        float directionRad = direction * Mathf.Deg2Rad;

        Vector3 launchDirection = new Vector3(
            Mathf.Sin(directionRad) * Mathf.Cos(angleRad),
            Mathf.Sin(angleRad),
            Mathf.Cos(directionRad) * Mathf.Cos(angleRad)
        );

        Vector3 velocity = launchDirection * speed;

        // 简化的抛物线计算（忽略空气阻力）
        float timeToGround = (-velocity.y - Mathf.Sqrt(velocity.y * velocity.y - 2 * Physics.gravity.y * launchPos.y)) / Physics.gravity.y;

        if (timeToGround > 0)
        {
            Vector3 landingPoint = launchPos + new Vector3(
                velocity.x * timeToGround,
                velocity.y * timeToGround + 0.5f * Physics.gravity.y * timeToGround * timeToGround,
                velocity.z * timeToGround
            );

            return landingPoint;
        }

        return launchPos;
    }

    /// <summary>
    /// 应用计算出的发射参数
    /// </summary>
    void ApplyLaunchParameters(float angle, float speed, float direction)
    {
        if (ballLauncher == null) return;

        // 更新滑块值并触发事件
        if (ballLauncher.angleSlider != null)
        {
            ballLauncher.angleSlider.value = angle;
            // 手动触发滑块的OnValueChanged事件
            ballLauncher.angleSlider.onValueChanged.Invoke(angle);
        }

        if (ballLauncher.speedSlider != null)
        {
            ballLauncher.speedSlider.value = speed;
            // 手动触发滑块的OnValueChanged事件
            ballLauncher.speedSlider.onValueChanged.Invoke(speed);
        }

        if (ballLauncher.directionSlider != null)
        {
            ballLauncher.directionSlider.value = direction;
            // 手动触发滑块的OnValueChanged事件
            ballLauncher.directionSlider.onValueChanged.Invoke(direction);
        }

        // 直接调用BallLauncher的设置方法以确保参数生效
        ballLauncher.SetDirection(direction);

        // 强制更新轨迹线预测
        ForceUpdateTrajectory();

        if (showDebugInfo)
        {
            Debug.Log($"🎯 Applied parameters - Angle: {angle:F1}°, Speed: {speed:F1}, Direction: {direction:F1}°");
        }
    }

    /// <summary>
    /// 强制更新轨迹线
    /// </summary>
    void ForceUpdateTrajectory()
    {
        if (ballLauncher == null) return;

        // 调用BallLauncher的轨迹更新方法
        // 使用反射调用私有方法（如果需要）
        var updateMethod = ballLauncher.GetType().GetMethod("UpdateTrajectoryLine",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (updateMethod != null)
        {
            updateMethod.Invoke(ballLauncher, null);
        }
        else
        {
            // 尝试调用公共的Update方法作为备选
            var publicUpdateMethod = ballLauncher.GetType().GetMethod("Update",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (publicUpdateMethod != null)
            {
                publicUpdateMethod.Invoke(ballLauncher, null);
            }
        }
    }

    /// <summary>
    /// 结束拖动
    /// </summary>
    void EndDragging()
    {
        isDragging = false;

        // 保存当前拖动后的参数作为新的基准参数
        SaveCurrentParameters();

        // 恢复轨迹线颜色
        if (trajectoryLine != null)
        {
            trajectoryLine.startColor = normalColor;
            trajectoryLine.endColor = normalColor;
        }

        Debug.Log("✅ Trajectory dragging completed - Parameters saved");
    }

    /// <summary>
    /// 保存当前参数
    /// </summary>
    void SaveCurrentParameters()
    {
        if (ballLauncher == null) return;

        // 更新原始参数为当前值，这样下次拖动时使用新的基准
        originalAngle = ballLauncher.angleSlider?.value ?? originalAngle;
        originalSpeed = ballLauncher.speedSlider?.value ?? originalSpeed;
        originalDirection = ballLauncher.directionSlider?.value ?? originalDirection;

        if (showDebugInfo)
        {
            Debug.Log($"💾 Parameters saved - Angle: {originalAngle:F1}°, Speed: {originalSpeed:F1}, Direction: {originalDirection:F1}°");
        }
    }

    /// <summary>
    /// 取消拖动，恢复原始参数
    /// </summary>
    void CancelDragging()
    {
        if (!isDragging) return;

        // 恢复原始参数
        ApplyLaunchParameters(originalAngle, originalSpeed, originalDirection);

        // 直接结束拖动，不调用SaveCurrentParameters
        isDragging = false;

        // 恢复轨迹线颜色
        if (trajectoryLine != null)
        {
            trajectoryLine.startColor = normalColor;
            trajectoryLine.endColor = normalColor;
        }

        Debug.Log("❌ Trajectory dragging cancelled - Parameters restored");
    }

    /// <summary>
    /// 创建拖动指示器
    /// </summary>
    void CreateDragIndicator()
    {
        if (dragIndicator == null)
        {
            dragIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dragIndicator.name = "TrajectoryDragIndicator";
            dragIndicator.transform.localScale = Vector3.one * 0.2f;

            // 设置材质
            Renderer renderer = dragIndicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = highlightColor;
                mat.SetFloat("_Mode", 3); // 透明模式
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                renderer.material = mat;
            }

            // 移除碰撞器
            Collider collider = dragIndicator.GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }

            dragIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// 更新拖动指示器
    /// </summary>
    void UpdateDragIndicator()
    {
        if (dragIndicator == null) return;

        if (isDragging)
        {
            dragIndicator.SetActive(true);
            dragIndicator.transform.position = targetLandingPoint + Vector3.up * 0.1f;
        }
        else
        {
            dragIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// 切换拖动控制功能
    /// </summary>
    public void ToggleDragControl()
    {
        enableDragControl = !enableDragControl;
        Debug.Log($"🎯 Trajectory drag control: {(enableDragControl ? "Enabled" : "Disabled")}");

        if (!enableDragControl && isDragging)
        {
            CancelDragging();
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 300, 300, 150));
        GUILayout.Box("轨迹线拖动控制");

        GUILayout.Label($"拖动状态: {(isDragging ? "拖动中" : "待机")}");
        GUILayout.Label($"功能启用: {enableDragControl}");

        if (isDragging)
        {
            GUILayout.Label($"目标位置: {targetLandingPoint}");
        }

        if (GUILayout.Button("切换拖动功能"))
        {
            ToggleDragControl();
        }

        if (GUILayout.Button("切换调试信息"))
        {
            showDebugInfo = !showDebugInfo;
        }

        GUILayout.EndArea();
    }
}