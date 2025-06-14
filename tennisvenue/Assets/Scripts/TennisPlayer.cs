using UnityEngine;

/// <summary>
/// Tennis Player Controller - 3D网球人物控制器
///
/// 基于pose.md的技术实现，与现有Tennis Venue系统完全集成
///
/// 主要功能：
/// 1. 基础人物控制和动画管理
/// 2. 球检测和击球逻辑
/// 3. 与发球机系统协同
/// 4. 与幕布系统交互
/// 5. 集成到现有UI系统
/// </summary>
public class TennisPlayer : MonoBehaviour
{
    [Header("=== 角色设置 ===")]
    public Transform racketAttachPoint; // 网球拍附加点（右手）
    public GameObject racketPrefab; // 网球拍预制体
    public LayerMask ballLayerMask = -1; // 球检测图层
    public float ballDetectionRange = 3f; // 球检测范围

    [Header("=== 动画控制 ===")]
    public Animator playerAnimator; // 角色动画控制器
    public string idleStateName = "Idle"; // 待机动画状态名
    public string forehandStateName = "ForehandSwing"; // 正手击球动画
    public string backhandStateName = "BackhandSwing"; // 反手击球动画
    public string serveStateName = "ServeMotion"; // 发球动画

    [Header("=== 击球设置 ===")]
    public float hitForce = 20f; // 击球力度
    public float reactionTime = 0.3f; // 反应时间
    public bool enableAutoHit = true; // 自动击球
    public bool enableManualControl = true; // 手动控制
    public Transform hitPoint; // 击球点

    [Header("=== 人物移动 ===")]
    public float moveSpeed = 5f; // 移动速度
    public float rotationSpeed = 180f; // 旋转速度
    public Vector3 homePosition; // 默认位置
    public bool enableMovement = true; // 启用移动

    [Header("=== 系统集成 ===")]
    public BallLauncher ballLauncher; // 发球机引用
    public BounceImpactMarker impactMarker; // 冲击标记系统
    public TennisVenueUIManager uiManager; // UI管理器

    [Header("=== 调试设置 ===")]
    public bool showDebugInfo = true;
    public bool showGizmos = true;
    public Color detectionRangeColor = Color.yellow;

    // 私有变量
    private GameObject currentRacket; // 当前网球拍实例
    private GameObject targetBall; // 目标球对象
    private bool isSwinging = false; // 是否正在挥拍
    private Vector3 ballPredictedPosition; // 球预测位置
    private float lastHitTime = 0f; // 上次击球时间
    private PlayerState currentState = PlayerState.Idle; // 当前状态

    // 状态枚举
    public enum PlayerState
    {
        Idle,       // 待机
        Tracking,   // 跟踪球
        Preparing,  // 准备击球
        Swinging,   // 挥拍中
        Recovering  // 恢复中
    }

    void Start()
    {
        Debug.Log("=== Tennis Player Initialized ===");

        InitializePlayer();
        FindSystemComponents();
        SetupRacket();
        SetupAnimations();

        // 设置默认位置
        if (homePosition == Vector3.zero)
        {
            homePosition = transform.position;
        }

        Debug.Log("Tennis Player Ready!");
        Debug.Log("控制说明：");
        Debug.Log("  T键: 手动触发正手击球");
        Debug.Log("  Y键: 手动触发反手击球");
        Debug.Log("  U键: 手动触发发球动作");
        Debug.Log("  R键: 返回默认位置");
        Debug.Log("  G键: 切换自动击球模式");
    }

    void Update()
    {
        // 手动控制输入
        HandleManualInput();

        // 自动击球逻辑
        if (enableAutoHit)
        {
            HandleAutoHit();
        }

        // 人物移动控制
        if (enableMovement)
        {
            HandleMovement();
        }

        // 更新状态
        UpdatePlayerState();

        // 调试信息
        if (showDebugInfo)
        {
            DisplayDebugInfo();
        }
    }

    /// <summary>
    /// 初始化人物组件
    /// </summary>
    void InitializePlayer()
    {
        // 获取Animator组件
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogError("❌ 未找到Animator组件！请确保角色有Animator Controller");
            }
        }

        // 设置击球点
        if (hitPoint == null)
        {
            // 在角色前方1米处创建击球点
            GameObject hitPointObj = new GameObject("HitPoint");
            hitPointObj.transform.SetParent(transform);
            hitPointObj.transform.localPosition = Vector3.forward * 1f + Vector3.up * 1f;
            hitPoint = hitPointObj.transform;
        }

        // 查找网球拍附加点
        if (racketAttachPoint == null)
        {
            // 尝试查找右手骨骼
            Transform rightHand = FindDeepChild(transform, "RightHand");
            if (rightHand == null)
            {
                rightHand = FindDeepChild(transform, "Hand_R");
            }
            if (rightHand == null)
            {
                rightHand = FindDeepChild(transform, "mixamorig:RightHand");
            }

            if (rightHand != null)
            {
                racketAttachPoint = rightHand;
                Debug.Log($"✅ 找到右手骨骼: {rightHand.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到右手骨骼，将使用角色根节点");
                racketAttachPoint = transform;
            }
        }
    }

    /// <summary>
    /// 查找系统组件
    /// </summary>
    void FindSystemComponents()
    {
        // 查找发球机
        if (ballLauncher == null)
        {
            ballLauncher = FindObjectOfType<BallLauncher>();
            if (ballLauncher != null)
            {
                Debug.Log("✅ 已连接发球机系统");
            }
        }

        // 查找冲击标记系统
        if (impactMarker == null)
        {
            impactMarker = FindObjectOfType<BounceImpactMarker>();
            if (impactMarker != null)
            {
                Debug.Log("✅ 已连接冲击标记系统");
            }
        }

        // 查找UI管理器
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<TennisVenueUIManager>();
            if (uiManager != null)
            {
                Debug.Log("✅ 已连接UI管理系统");
            }
        }
    }

    /// <summary>
    /// 设置网球拍
    /// </summary>
    void SetupRacket()
    {
        if (racketPrefab != null && racketAttachPoint != null)
        {
            // 创建网球拍实例
            currentRacket = Instantiate(racketPrefab, racketAttachPoint);
            currentRacket.transform.localPosition = Vector3.zero;
            currentRacket.transform.localRotation = Quaternion.identity;

            // 添加碰撞检测
            if (currentRacket.GetComponent<Collider>() == null)
            {
                BoxCollider racketCollider = currentRacket.AddComponent<BoxCollider>();
                racketCollider.isTrigger = true;
                racketCollider.size = new Vector3(0.3f, 0.05f, 0.4f); // 网球拍大小
            }

            // 添加网球拍物理脚本
            if (currentRacket.GetComponent<RacketPhysics>() == null)
            {
                RacketPhysics racketPhysics = currentRacket.AddComponent<RacketPhysics>();
                racketPhysics.player = this;
                racketPhysics.hitForce = hitForce;
            }

            Debug.Log("✅ 网球拍设置完成");
        }
        else
        {
            Debug.LogWarning("⚠️ 网球拍预制体或附加点未设置");
        }
    }

    /// <summary>
    /// 设置动画系统
    /// </summary>
    void SetupAnimations()
    {
        if (playerAnimator != null)
        {
            // 检查动画状态是否存在
            bool hasIdleState = HasAnimationState(idleStateName);
            bool hasForehandState = HasAnimationState(forehandStateName);
            bool hasBackhandState = HasAnimationState(backhandStateName);

            Debug.Log($"动画状态检查: Idle={hasIdleState}, Forehand={hasForehandState}, Backhand={hasBackhandState}");

            if (!hasIdleState)
            {
                Debug.LogWarning("⚠️ 缺少Idle动画状态");
            }

            // 设置默认状态为待机
            SetAnimationState(idleStateName);
        }
    }

    /// <summary>
    /// 处理手动输入
    /// </summary>
    void HandleManualInput()
    {
        if (!enableManualControl) return;

        // T键 - 正手击球
        if (Input.GetKeyDown(KeyCode.T))
        {
            TriggerSwing(SwingType.Forehand);
        }

        // Y键 - 反手击球
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TriggerSwing(SwingType.Backhand);
        }

        // U键 - 发球动作
        if (Input.GetKeyDown(KeyCode.U))
        {
            TriggerSwing(SwingType.Serve);
        }

        // R键 - 返回默认位置
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReturnToHomePosition();
        }

        // G键 - 切换自动击球模式
        if (Input.GetKeyDown(KeyCode.G))
        {
            enableAutoHit = !enableAutoHit;
            Debug.Log($"自动击球模式: {(enableAutoHit ? "开启" : "关闭")}");
        }
    }

    /// <summary>
    /// 处理自动击球逻辑
    /// </summary>
    void HandleAutoHit()
    {
        // 检测附近的球
        DetectNearbyBalls();

        // 如果找到目标球且满足击球条件
        if (targetBall != null && CanHitBall())
        {
            // 预测球的位置
            PredictBallPosition();

            // 触发击球
            SwingType swingType = DetermineSwingType();
            TriggerSwing(swingType);
        }
    }

    /// <summary>
    /// 处理人物移动
    /// </summary>
    void HandleMovement()
    {
        // 如果有目标球，朝球的方向移动
        if (targetBall != null && currentState == PlayerState.Tracking)
        {
            Vector3 targetPosition = GetOptimalPosition();
            MoveToPosition(targetPosition);
        }

        // 键盘控制移动（调试用）
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 检测附近的球
    /// </summary>
    void DetectNearbyBalls()
    {
        // 查找所有球对象
        Collider[] ballColliders = Physics.OverlapSphere(transform.position, ballDetectionRange, ballLayerMask);

        GameObject closestBall = null;
        float closestDistance = float.MaxValue;

        foreach (Collider collider in ballColliders)
        {
            // 检查是否是网球
            if (IsTennisBall(collider.gameObject))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBall = collider.gameObject;
                }
            }
        }

        // 更新目标球
        if (closestBall != targetBall)
        {
            targetBall = closestBall;
            if (targetBall != null)
            {
                Debug.Log($"🎯 发现目标球: {targetBall.name} (距离: {closestDistance:F2}m)");
            }
        }
    }

    /// <summary>
    /// 检查是否是网球
    /// </summary>
    bool IsTennisBall(GameObject obj)
    {
        // 检查名称
        if (obj.name.Contains("TennisBall") || obj.name.Contains("Ball"))
        {
            // 检查是否有Rigidbody组件
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 检查是否可以击球
    /// </summary>
    bool CanHitBall()
    {
        if (targetBall == null) return false;
        if (isSwinging) return false;
        if (Time.time - lastHitTime < reactionTime) return false;

        // 检查球的距离
        float distance = Vector3.Distance(transform.position, targetBall.transform.position);
        if (distance > ballDetectionRange) return false;

        // 检查球是否在移动
        Rigidbody ballRb = targetBall.GetComponent<Rigidbody>();
        if (ballRb != null && ballRb.velocity.magnitude < 1f) return false;

        return true;
    }

    /// <summary>
    /// 预测球的位置
    /// </summary>
    void PredictBallPosition()
    {
        if (targetBall == null) return;

        Rigidbody ballRb = targetBall.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            // 简单的线性预测
            Vector3 ballVelocity = ballRb.velocity;
            float timeToHit = reactionTime;
            ballPredictedPosition = targetBall.transform.position + ballVelocity * timeToHit;
        }
        else
        {
            ballPredictedPosition = targetBall.transform.position;
        }
    }

    /// <summary>
    /// 确定挥拍类型
    /// </summary>
    SwingType DetermineSwingType()
    {
        if (targetBall == null) return SwingType.Forehand;

        // 根据球的位置相对于人物的位置来决定
        Vector3 ballDirection = (targetBall.transform.position - transform.position).normalized;
        Vector3 playerRight = transform.right;

        // 如果球在右侧，使用正手；如果在左侧，使用反手
        float dotProduct = Vector3.Dot(ballDirection, playerRight);

        return dotProduct > 0 ? SwingType.Forehand : SwingType.Backhand;
    }

    /// <summary>
    /// 触发挥拍动作
    /// </summary>
    public void TriggerSwing(SwingType swingType)
    {
        if (isSwinging) return;

        isSwinging = true;
        lastHitTime = Time.time;
        currentState = PlayerState.Swinging;

        // 播放对应的动画
        string animationName = "";
        switch (swingType)
        {
            case SwingType.Forehand:
                animationName = forehandStateName;
                Debug.Log("🎾 触发正手击球");
                break;
            case SwingType.Backhand:
                animationName = backhandStateName;
                Debug.Log("🎾 触发反手击球");
                break;
            case SwingType.Serve:
                animationName = serveStateName;
                Debug.Log("🎾 触发发球动作");
                break;
        }

        // 播放动画
        if (!string.IsNullOrEmpty(animationName))
        {
            SetAnimationState(animationName);
        }

        // 转向球的方向
        if (targetBall != null)
        {
            Vector3 lookDirection = (targetBall.transform.position - transform.position).normalized;
            lookDirection.y = 0; // 保持水平
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        // 延迟一段时间后恢复
        StartCoroutine(RecoverFromSwing());
    }

    /// <summary>
    /// 从挥拍动作中恢复
    /// </summary>
    System.Collections.IEnumerator RecoverFromSwing()
    {
        yield return new WaitForSeconds(1f); // 等待动画完成

        isSwinging = false;
        currentState = PlayerState.Idle;

        // 返回待机动画
        SetAnimationState(idleStateName);

        Debug.Log("🎾 挥拍动作完成");
    }

    /// <summary>
    /// 返回默认位置
    /// </summary>
    public void ReturnToHomePosition()
    {
        Debug.Log("🏠 返回默认位置");
        StartCoroutine(MoveToHomePosition());
    }

    /// <summary>
    /// 移动到默认位置
    /// </summary>
    System.Collections.IEnumerator MoveToHomePosition()
    {
        Vector3 startPos = transform.position;
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, homePosition, t);
            yield return null;
        }

        transform.position = homePosition;
        Debug.Log("✅ 已到达默认位置");
    }

    /// <summary>
    /// 获取最佳位置
    /// </summary>
    Vector3 GetOptimalPosition()
    {
        if (targetBall == null) return homePosition;

        // 计算最佳击球位置
        Vector3 ballPos = ballPredictedPosition;
        Vector3 optimalPos = ballPos + Vector3.back * 1.5f; // 在球后方1.5米

        return optimalPos;
    }

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    void MoveToPosition(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 转向移动方向
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 更新人物状态
    /// </summary>
    void UpdatePlayerState()
    {
        if (isSwinging)
        {
            currentState = PlayerState.Swinging;
        }
        else if (targetBall != null)
        {
            float distance = Vector3.Distance(transform.position, targetBall.transform.position);
            if (distance <= ballDetectionRange)
            {
                currentState = PlayerState.Tracking;
            }
            else
            {
                currentState = PlayerState.Idle;
            }
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    /// <summary>
    /// 设置动画状态
    /// </summary>
    void SetAnimationState(string stateName)
    {
        if (playerAnimator != null && HasAnimationState(stateName))
        {
            playerAnimator.Play(stateName);
        }
    }

    /// <summary>
    /// 检查动画状态是否存在
    /// </summary>
    bool HasAnimationState(string stateName)
    {
        if (playerAnimator == null) return false;

        // 简单检查，实际项目中可能需要更复杂的验证
        return !string.IsNullOrEmpty(stateName);
    }

    /// <summary>
    /// 深度查找子对象
    /// </summary>
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name))
            {
                return child;
            }
            Transform result = FindDeepChild(child, name);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// 显示调试信息
    /// </summary>
    void DisplayDebugInfo()
    {
        if (showDebugInfo && targetBall != null)
        {
            Debug.Log($"Player State: {currentState}, Target Ball: {targetBall.name}");
        }
    }

    /// <summary>
    /// 绘制调试信息
    /// </summary>
    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // 绘制检测范围
        Gizmos.color = detectionRangeColor;
        Gizmos.DrawWireSphere(transform.position, ballDetectionRange);

        // 绘制击球点
        if (hitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPoint.position, 0.2f);
        }

        // 绘制目标球连线
        if (targetBall != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetBall.transform.position);
        }

        // 绘制预测位置
        if (ballPredictedPosition != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(ballPredictedPosition, 0.3f);
        }
    }

    // 公共方法，供其他脚本调用
    public void OnBallHit(GameObject ball, Vector3 hitPoint, Vector3 hitDirection)
    {
        Debug.Log($"🎯 击中球体: {ball.name} at {hitPoint}");

        // 通知冲击标记系统
        if (impactMarker != null)
        {
            // 可以在这里添加特殊的击球标记
        }
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    public bool IsSwinging()
    {
        return isSwinging;
    }

    public GameObject GetTargetBall()
    {
        return targetBall;
    }
}

/// <summary>
/// 挥拍类型枚举
/// </summary>
public enum SwingType
{
    Forehand,  // 正手
    Backhand,  // 反手
    Serve      // 发球
}

/// <summary>
/// 网球拍物理控制器
/// </summary>
public class RacketPhysics : MonoBehaviour
{
    [HideInInspector]
    public TennisPlayer player;
    public float hitForce = 20f;
    public Vector3 hitDirection = Vector3.forward;

    void OnTriggerEnter(Collider other)
    {
        // 只在挥拍时才能击球
        if (player != null && player.IsSwinging())
        {
            // 检查是否是网球
            if (other.name.Contains("TennisBall") || other.name.Contains("Ball"))
            {
                Rigidbody ballRb = other.GetComponent<Rigidbody>();
                if (ballRb != null)
                {
                    // 计算击球方向
                    Vector3 forceDirection = player.transform.forward + Vector3.up * 0.3f;
                    forceDirection.Normalize();

                    // 施加力
                    ballRb.velocity = Vector3.zero; // 清除原有速度
                    ballRb.AddForce(forceDirection * hitForce, ForceMode.VelocityChange);

                    // 通知人物
                    player.OnBallHit(other.gameObject, transform.position, forceDirection);

                    Debug.Log($"🎾 网球拍击中球体: {other.name}，力度: {hitForce}");
                }
            }
        }
    }
}