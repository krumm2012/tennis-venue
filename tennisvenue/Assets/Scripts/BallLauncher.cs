using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallLauncher : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform launchPoint;
    public Camera mainCamera;
    public LayerMask targetLayerMask;

    public Slider angleSlider;
    public Slider speedSlider;
    public Slider directionSlider;
    public TextMeshProUGUI angleText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI directionText;
    public LineRenderer trajectoryLine;
    public int trajectoryPoints = 30;
    public float timeStep = 0.05f;

    [Header("轨迹线设置")]
    public LayerMask obstacleLayerMask = -1; // 障碍物图层（Floor和Wall）
    public float dashLength = 0.3f; // 虚线段长度
    public float gapLength = 0.15f; // 虚线间隙长度

    [Header("增强功能集成")]
    public FlightTimeTracker flightTimeTracker;
    public AirResistanceSystem airResistanceSystem;

    [Header("轨迹线拖动集成")]
    public TrajectoryDragController trajectoryDragController;
    public bool enableTrajectoryDrag = true;

    private float angle = 45f;
    private float speed = 20f;
    private float direction = 0f; // 0度为正前方

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (launchPoint == null)
            launchPoint = this.transform;

        if (angleSlider != null)
        {
            angleSlider.minValue = 15f;
            angleSlider.maxValue = 75f;
            angleSlider.value = angle;
            angleSlider.onValueChanged.AddListener(OnAngleChanged);
        }

        // 初始化发球机的俯仰角
        UpdateLauncherRotation();
        if (speedSlider != null)
        {
            speedSlider.minValue = 10f;
            speedSlider.maxValue = 30f;
            speedSlider.value = speed;
            speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        }

        // 自动查找DirectionSlider（支持从其他脚本设置）
        if (directionSlider == null)
        {
            directionSlider = GameObject.Find("DirectionSlider")?.GetComponent<Slider>();
        }

        if (directionText == null)
        {
            directionText = GameObject.Find("DirectionText")?.GetComponent<TextMeshProUGUI>();
        }

        // 初始化方向滑块
        if (directionSlider != null)
        {
            Debug.Log("DirectionSlider found and initialized");
            directionSlider.minValue = -45f; // 左转45度
            directionSlider.maxValue = 45f;  // 右转45度
            directionSlider.value = direction;
            directionSlider.wholeNumbers = false;
            directionSlider.interactable = true;
            directionSlider.onValueChanged.RemoveAllListeners(); // 清除旧的监听器
            directionSlider.onValueChanged.AddListener(OnDirectionChanged);

            // 修复DirectionSlider的颜色
            FixDirectionSliderColors();
        }
        else
        {
            Debug.LogWarning("DirectionSlider not found, will try to find it later");
        }

        // 修复DirectionText
        if (directionText != null)
        {
            directionText.text = "Direction: 0.0°";
            directionText.color = Color.white;
            directionText.fontSize = 14;
        }
        UpdateUI();

        // 查找空气阻力系统
        if (airResistanceSystem == null)
        {
            airResistanceSystem = FindObjectOfType<AirResistanceSystem>();
        }

        // 查找飞行时间追踪器
        if (flightTimeTracker == null)
        {
            flightTimeTracker = FindObjectOfType<FlightTimeTracker>();
        }
    }

    void Update()
    {
        // 轨迹线实时显示 - 显示从发球机当前角度发射的轨迹
        if (trajectoryLine != null)
        {
            Vector3 launchDirection = transform.forward;
            Vector3 launchVelocity = launchDirection * speed;
            DrawTrajectory(launchPoint.position, launchVelocity);
        }

        // 检查是否有轨迹线拖动控制器，并且是否正在拖动
        bool isDragging = false;
        if (trajectoryDragController == null && enableTrajectoryDrag)
        {
            trajectoryDragController = FindObjectOfType<TrajectoryDragController>();
        }

        if (trajectoryDragController != null && enableTrajectoryDrag)
        {
            isDragging = trajectoryDragController.IsDragging;
        }

        // 按空格键发射网球，或者鼠标左键（但不在拖动轨迹线时）
        if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0) && !isDragging))
        {
            LaunchBall(Vector3.zero); // 不需要目标点，直接发射
        }

        // 测试方向角控制
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetDirection(direction - 10f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetDirection(direction + 10f);
        }
    }

    /// <summary>
    /// 发射网球 - 公共方法，可被外部脚本调用
    /// </summary>
    public void LaunchBall(Vector3 targetPos)
    {
        if (ballPrefab == null || launchPoint == null) return;

        GameObject ball = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb == null) return;

        // 应用优化的空气阻力参数
        if (airResistanceSystem != null)
        {
            airResistanceSystem.ApplyOptimizedResistance(ball);
        }
        else
        {
            // 使用默认物理参数
            SetupDefaultBallPhysics(ball, rb);
        }

        Vector3 launchDirection = transform.forward;
        Vector3 launchVelocity = launchDirection * speed;
        rb.velocity = launchVelocity;

        // 通知飞行时间追踪器开始追踪
        if (flightTimeTracker != null)
        {
            flightTimeTracker.StartFlightTimeTracking();
        }

        // 记录发射信息和阻力影响
        LogLaunchInfo();
    }

    /// <summary>
    /// 设置默认网球物理参数（当空气阻力系统不可用时）
    /// </summary>
    void SetupDefaultBallPhysics(GameObject ball, Rigidbody rb)
    {
        rb.mass = 0.057f; // 57克
        rb.drag = 0.1f;   // 默认空气阻力
        rb.angularDrag = 0.05f; // 默认角阻力

        Collider ballCollider = ball.GetComponent<Collider>();
        if (ballCollider != null)
        {
            PhysicMaterial ballMaterial = new PhysicMaterial("TennisBall");
            ballMaterial.dynamicFriction = 0.6f;
            ballMaterial.staticFriction = 0.6f;
            ballMaterial.bounciness = 0.8f;
            ballMaterial.frictionCombine = PhysicMaterialCombine.Average;
            ballMaterial.bounceCombine = PhysicMaterialCombine.Multiply;

            ballCollider.material = ballMaterial;
        }
    }

    /// <summary>
    /// 记录发射信息和空气阻力影响
    /// </summary>
    void LogLaunchInfo()
    {
        string resistanceInfo = "";
        if (airResistanceSystem != null)
        {
            Vector2 rangeImpact = airResistanceSystem.AnalyzeLandingPointImpact(speed, angle);
            float reduction = (rangeImpact.x - rangeImpact.y) / rangeImpact.x * 100f;
            resistanceInfo = $"，空气阻力影响: 理论{rangeImpact.x:F1}m → 实际{rangeImpact.y:F1}m (减少{reduction:F1}%)";
        }

        Debug.Log($"发射网球：俯仰角 {angle:F1}°，速度 {speed:F1}m/s，方向 {direction:F1}°{resistanceInfo}");
    }

    public Vector3 CalculateTrajectoryVelocity(Vector3 origin, Vector3 target, float angleInDegrees, float speed)
    {
        Vector3 targetDirection = target - origin;
        float yOffset = targetDirection.y;
        targetDirection.y = 0;
        float distance = targetDirection.magnitude;
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        float v0 = speed;
        float vxz = v0 * Mathf.Cos(angleInRadians);
        float vy = v0 * Mathf.Sin(angleInRadians);

        Vector3 velocity = targetDirection.normalized * vxz;
        velocity.y = vy;
        return velocity;
    }

    void DrawTrajectory(Vector3 start, Vector3 velocity)
    {
        var trajectoryPoints = CalculateTrajectoryWithCollision(start, velocity);
        var dashedPoints = CreateDashedLine(trajectoryPoints);

        trajectoryLine.positionCount = dashedPoints.Count;
        for (int i = 0; i < dashedPoints.Count; i++)
        {
            trajectoryLine.SetPosition(i, dashedPoints[i]);
        }
    }

    /// <summary>
    /// 计算带碰撞检测的轨迹点
    /// </summary>
    System.Collections.Generic.List<Vector3> CalculateTrajectoryWithCollision(Vector3 start, Vector3 velocity)
    {
        var points = new System.Collections.Generic.List<Vector3>();
        Vector3 pos = start;
        Vector3 v = velocity;
        Vector3 lastPos = start;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            // 检查从上一个点到当前点是否有碰撞
            if (i > 0)
            {
                RaycastHit hit;
                Vector3 rayDirection = pos - lastPos;
                float distance = rayDirection.magnitude;

                if (Physics.Raycast(lastPos, rayDirection.normalized, out hit, distance, obstacleLayerMask))
                {
                    // 如果碰撞到障碍物，添加碰撞点并停止
                    points.Add(hit.point);
                    break;
                }
            }

            points.Add(pos);
            lastPos = pos;

            // 计算下一个点
            v += Physics.gravity * timeStep;
            pos += v * timeStep;

            // 如果球已经落得太低，停止计算
            if (pos.y < -2f)
                break;
        }

        return points;
    }

    /// <summary>
    /// 将连续的轨迹点转换为虚线效果
    /// </summary>
    System.Collections.Generic.List<Vector3> CreateDashedLine(System.Collections.Generic.List<Vector3> originalPoints)
    {
        var dashedPoints = new System.Collections.Generic.List<Vector3>();

        if (originalPoints.Count < 2)
            return originalPoints;

        var distances = new System.Collections.Generic.List<float>();

        // 计算每段的距离
        for (int i = 1; i < originalPoints.Count; i++)
        {
            float segmentDistance = Vector3.Distance(originalPoints[i-1], originalPoints[i]);
            distances.Add(segmentDistance);
        }

        // 生成虚线
        bool inDash = true; // 开始时绘制实线
        float dashProgress = 0f;

        dashedPoints.Add(originalPoints[0]); // 总是添加起始点

        for (int i = 1; i < originalPoints.Count; i++)
        {
            float segmentDistance = distances[i-1];
            Vector3 startPoint = originalPoints[i-1];
            Vector3 endPoint = originalPoints[i];
            Vector3 segmentDirection = (endPoint - startPoint).normalized;

            float segmentProgress = 0f;

            while (segmentProgress < segmentDistance)
            {
                float remainingInCurrentState = inDash ? (dashLength - dashProgress) : (gapLength - dashProgress);
                float remainingInSegment = segmentDistance - segmentProgress;
                float stepDistance = Mathf.Min(remainingInCurrentState, remainingInSegment);

                if (inDash)
                {
                    // 在实线段中，添加点
                    Vector3 point = startPoint + segmentDirection * (segmentProgress + stepDistance);
                    dashedPoints.Add(point);
                }

                segmentProgress += stepDistance;
                dashProgress += stepDistance;

                // 检查是否需要切换状态
                if (inDash && dashProgress >= dashLength)
                {
                    inDash = false;
                    dashProgress = 0f;
                }
                else if (!inDash && dashProgress >= gapLength)
                {
                    inDash = true;
                    dashProgress = 0f;
                    // 开始新的实线段时，添加起始点
                    if (segmentProgress < segmentDistance)
                    {
                        Vector3 point = startPoint + segmentDirection * segmentProgress;
                        dashedPoints.Add(point);
                    }
                }
            }
        }

        return dashedPoints;
    }

    void OnAngleChanged(float value)
    {
        angle = value;
        // 更新发球机的俯仰角 - 让发球机本身转动
        UpdateLauncherRotation();
        UpdateUI();
    }

    void OnSpeedChanged(float value)
    {
        speed = value;
        UpdateUI();
    }

    void OnDirectionChanged(float value)
    {
        Debug.Log($"Direction changed to: {value}");
        SetDirection(value);
    }

    /// <summary>
    /// 设置方向角度（由外部脚本调用）
    /// </summary>
    public void SetDirection(float newDirection)
    {
        direction = Mathf.Clamp(newDirection, -45f, 45f);
        // 更新发球机的方向角
        UpdateLauncherRotation();
        UpdateUI();

        // 同步Slider值
        if (directionSlider != null && Mathf.Abs(directionSlider.value - direction) > 0.1f)
        {
            directionSlider.value = direction;
        }

        Debug.Log($"Direction set to: {direction:F1}°");
    }

    /// <summary>
    /// 由DirectionSliderFix脚本调用，设置directionSlider引用
    /// </summary>
    public void SetDirectionSlider(Slider slider)
    {
        directionSlider = slider;
        if (directionSlider != null)
        {
            directionSlider.onValueChanged.RemoveAllListeners();
            directionSlider.onValueChanged.AddListener(OnDirectionChanged);
            Debug.Log("DirectionSlider reference set via SetDirectionSlider()");
        }
    }

    /// <summary>
    /// 更新发球机的俯仰角和方向角旋转
    /// </summary>
    void UpdateLauncherRotation()
    {
        // 同时应用俯仰角（X轴）和方向角（Y轴）
        // 负号是因为Unity的旋转方向和我们期望的相反
        transform.rotation = Quaternion.Euler(-angle, direction, 0f);
    }

    void UpdateUI()
    {
        if (angleText != null) angleText.text = $"Angle: {angle:F1}°";
        if (speedText != null) speedText.text = $"Speed: {speed:F1}";
        if (directionText != null)
        {
            string directionDesc = "";
            if (direction < -5f)
                directionDesc = " (Left)";
            else if (direction > 5f)
                directionDesc = " (Right)";
            else
                directionDesc = " (Center)";

            directionText.text = $"Direction: {direction:F1}°{directionDesc}";
        }
    }

    void FixDirectionSliderColors()
    {
        if (directionSlider == null) return;

        // 获取所有Image组件
        Image[] images = directionSlider.GetComponentsInChildren<Image>();

        foreach (Image img in images)
        {
            if (img.name.Contains("Background"))
            {
                img.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                Debug.Log("设置DirectionSlider Background颜色");
            }
            else if (img.name.Contains("Fill"))
            {
                img.color = new Color(0.2f, 0.8f, 0.2f, 0.8f); // 绿色，区别于其他Slider
                Debug.Log("设置DirectionSlider Fill颜色为绿色");
            }
            else if (img.name.Contains("Handle"))
            {
                img.color = new Color(0.8f, 0.8f, 0.8f, 0.9f);
                Debug.Log("设置DirectionSlider Handle颜色");
            }
        }
    }
}
