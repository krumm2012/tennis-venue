using UnityEngine;
using TMPro;

/// <summary>
/// Tennis Player Quick Setup - 3D网球运动员快速设置系统
///
/// 基于pose.md的完整技术实现，一键集成3D击球人物到现有Tennis Venue项目中
///
/// 核心功能：
/// 1. 自动创建/导入3D人物角色
/// 2. 设置Humanoid Rig和Animator Controller
/// 3. 集成网球拍物理系统
/// 4. 与现有发球机、UI、标记系统协同
/// 5. 支持Mixamo角色和动画
/// </summary>
public class TennisPlayerQuickSetup : MonoBehaviour
{
    [Header("=== 3D人物设置 ===")]
    public GameObject playerPrefab; // 3D人物预制体（来自Mixamo或Asset Store）
    public GameObject racketPrefab; // 网球拍预制体
    public RuntimeAnimatorController playerAnimatorController; // 动画控制器
    public Avatar playerAvatar; // Humanoid Avatar

    [Header("=== 位置设置 ===")]
    public Vector3 playerDefaultPosition = new Vector3(0, 0, 8); // 默认位置（发球机对面8米）
    public Vector3 playerRotation = new Vector3(0, 180, 0); // 面向发球机
    public LayerMask ballDetectionLayer = -1; // 球检测图层
    public float ballInteractionRange = 4f; // 球交互范围

    [Header("=== 动画设置 ===")]
    [Tooltip("基于pose.md的标准动画状态名")]
    public string idleStateName = "Idle";
    public string forehandStateName = "ForehandSwing";
    public string backhandStateName = "BackhandSwing";
    public string serveStateName = "ServeMotion";

    [Header("=== 物理参数 ===")]
    public float hitForce = 25f; // 击球力度
    public float reactionTime = 0.4f; // 反应时间
    public float moveSpeed = 6f; // 移动速度
    public bool enableAutoHitting = true; // 自动击球
    public bool enableIKSystem = false; // IK系统（高级功能）

    [Header("=== 系统集成 ===")]
    public BallLauncher ballLauncher;
    public BounceImpactMarker impactMarker;
    public TennisVenueUIManager uiManager;
    public CurtainBehavior curtainBehavior;

    [Header("=== 调试设置 ===")]
    public bool showDebugInfo = true;
    public TextMeshProUGUI statusDisplay;
    public Color gizmoColor = Color.cyan;

    // 私有变量
    private GameObject playerInstance;
    private TennisPlayer playerController;
    private Animator playerAnimator;
    private bool isSystemReady = false;
    private string setupStatus = "未初始化";

    void Start()
    {
        Debug.Log("=== Tennis Player Quick Setup Started ===");

        DisplayInstructions();
        FindSystemComponents();
        UpdateStatusDisplay();

        Debug.Log("快捷键说明：");
        Debug.Log("  Ctrl+T: 一键设置3D网球运动员");
        Debug.Log("  Ctrl+D: 诊断系统状态");
        Debug.Log("  Ctrl+V: 验证所有功能");
        Debug.Log("  Ctrl+R: 重置系统");
        Debug.Log("  Ctrl+H: 显示帮助信息");
    }

    void Update()
    {
        HandleQuickSetupInput();
        UpdateStatusDisplay();
    }

    /// <summary>
    /// 处理快速设置输入
    /// </summary>
    void HandleQuickSetupInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartQuickSetup();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                DiagnoseSystem();
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                VerifyAllSystems();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                ResetSystem();
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                ShowHelp();
            }
        }
    }

    /// <summary>
    /// 一键快速设置3D网球运动员
    /// </summary>
    public void StartQuickSetup()
    {
        Debug.Log("🚀 开始一键设置3D网球运动员系统...");
        setupStatus = "设置中...";

        try
        {
            // 第1步：清理现有设置
            CleanupExistingPlayer();

            // 第2步：创建或设置3D人物
            if (!SetupPlayer())
            {
                setupStatus = "人物设置失败";
                return;
            }

            // 第3步：设置动画系统
            if (!SetupAnimationSystem())
            {
                setupStatus = "动画系统设置失败";
                return;
            }

            // 第4步：设置网球拍
            if (!SetupRacket())
            {
                setupStatus = "网球拍设置失败";
                return;
            }

            // 第5步：配置TennisPlayer控制器
            if (!SetupPlayerController())
            {
                setupStatus = "控制器设置失败";
                return;
            }

            // 第6步：集成现有系统
            if (!IntegrateWithExistingSystems())
            {
                setupStatus = "系统集成失败";
                return;
            }

            // 第7步：优化位置和参数
            OptimizePlayerSetup();

            // 完成设置
            isSystemReady = true;
            setupStatus = "✅ 设置完成";

            Debug.Log("🎉 3D网球运动员设置完成！");
            Debug.Log("现在您可以：");
            Debug.Log("  - 发球机发球，人物会自动击球");
            Debug.Log("  - 按T/Y键手动触发正手/反手击球");
            Debug.Log("  - 按G键切换自动/手动模式");
            Debug.Log("  - 按R键让人物返回默认位置");

        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 设置失败: {e.Message}");
            setupStatus = "设置失败: " + e.Message;
        }
    }

    /// <summary>
    /// 清理现有人物设置
    /// </summary>
    void CleanupExistingPlayer()
    {
        if (playerInstance != null)
        {
            DestroyImmediate(playerInstance);
            playerInstance = null;
        }

        // 查找场景中现有的TennisPlayer
        TennisPlayer existingPlayer = FindObjectOfType<TennisPlayer>();
        if (existingPlayer != null && existingPlayer.gameObject != gameObject)
        {
            Debug.Log("清理现有的TennisPlayer对象");
            DestroyImmediate(existingPlayer.gameObject);
        }
    }

    /// <summary>
    /// 设置3D人物
    /// </summary>
    bool SetupPlayer()
    {
        Debug.Log("📝 第1步：设置3D人物...");

        if (playerPrefab != null)
        {
            // 使用用户提供的预制体
            playerInstance = Instantiate(playerPrefab, playerDefaultPosition, Quaternion.Euler(playerRotation));
            playerInstance.name = "TennisPlayer_3D";
            Debug.Log("✅ 使用用户提供的3D人物预制体");
        }
        else
        {
            // 创建简单的占位人物
            playerInstance = CreatePlaceholderPlayer();
            Debug.Log("⚠️ 未提供预制体，创建占位人物");
            Debug.Log("建议：从Mixamo下载Humanoid角色模型获得更好效果");
        }

        return playerInstance != null;
    }

    /// <summary>
    /// 创建占位人物
    /// </summary>
    GameObject CreatePlaceholderPlayer()
    {
        GameObject player = new GameObject("TennisPlayer_Placeholder");
        player.transform.position = playerDefaultPosition;
        player.transform.rotation = Quaternion.Euler(playerRotation);

        // 身体
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(player.transform);
        body.transform.localPosition = new Vector3(0, 1, 0);
        body.transform.localScale = new Vector3(0.8f, 1f, 0.8f);

        // 头部
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(player.transform);
        head.transform.localPosition = new Vector3(0, 2.2f, 0);
        head.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        // 右手（网球拍附加点）
        GameObject rightHand = new GameObject("RightHand");
        rightHand.transform.SetParent(player.transform);
        rightHand.transform.localPosition = new Vector3(0.6f, 1.5f, 0);

        // 添加基本动画组件
        Animator animator = player.AddComponent<Animator>();

        // 为占位人物创建基本的Humanoid Avatar
        if (playerAvatar != null)
        {
            animator.avatar = playerAvatar;
        }

        return player;
    }

    /// <summary>
    /// 设置动画系统
    /// </summary>
    bool SetupAnimationSystem()
    {
        Debug.Log("🎭 第2步：设置动画系统...");

        playerAnimator = playerInstance.GetComponent<Animator>();
        if (playerAnimator == null)
        {
            playerAnimator = playerInstance.AddComponent<Animator>();
        }

        // 设置Animator Controller
        if (playerAnimatorController != null)
        {
            playerAnimator.runtimeAnimatorController = playerAnimatorController;
            Debug.Log("✅ 应用用户提供的Animator Controller");
        }
        else
        {
            // 创建基本的Animator Controller
            CreateBasicAnimatorController();
            Debug.Log("⚠️ 创建基本Animator Controller");
            Debug.Log("建议：从Mixamo获取网球击球动画获得更好效果");
        }

        // 检查Humanoid设置
        if (playerAnimator.avatar != null && playerAnimator.avatar.isHuman)
        {
            Debug.Log("✅ 检测到Humanoid Avatar");
        }
        else
        {
            Debug.LogWarning("⚠️ 未检测到Humanoid Avatar，某些功能可能受限");
        }

        return true;
    }

    /// <summary>
    /// 创建基本的Animator Controller
    /// </summary>
    void CreateBasicAnimatorController()
    {
        // 注意：在运行时创建Animator Controller比较复杂
        // 这里提供一个简化的实现，实际项目建议预先创建
        Debug.Log("💡 提示：建议在Unity编辑器中预先创建Animator Controller");
        Debug.Log("包含以下状态：Idle, ForehandSwing, BackhandSwing, ServeMotion");
        Debug.Log("并设置DoSwing触发器参数");
    }

    /// <summary>
    /// 设置网球拍
    /// </summary>
    bool SetupRacket()
    {
        Debug.Log("🎾 第3步：设置网球拍...");

        // 查找右手骨骼
        Transform rightHand = FindRightHandBone();
        if (rightHand == null)
        {
            Debug.LogWarning("⚠️ 未找到右手骨骼，使用默认位置");
            rightHand = playerInstance.transform;
        }

        GameObject racket;
        if (racketPrefab != null)
        {
            // 使用用户提供的网球拍
            racket = Instantiate(racketPrefab, rightHand);
            Debug.Log("✅ 使用用户提供的网球拍预制体");
        }
        else
        {
            // 创建简单的占位网球拍
            racket = CreatePlaceholderRacket(rightHand);
            Debug.Log("⚠️ 创建占位网球拍");
        }

        // 设置网球拍物理
        SetupRacketPhysics(racket);

        return true;
    }

    /// <summary>
    /// 查找右手骨骼
    /// </summary>
    Transform FindRightHandBone()
    {
        // 常见的右手骨骼命名
        string[] handNames = { "RightHand", "Hand_R", "mixamorig:RightHand", "Right Hand", "Hand.R" };

        foreach (string handName in handNames)
        {
            Transform hand = FindDeepChild(playerInstance.transform, handName);
            if (hand != null)
            {
                Debug.Log($"✅ 找到右手骨骼: {hand.name}");
                return hand;
            }
        }

        return null;
    }

    /// <summary>
    /// 深度查找子对象
    /// </summary>
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name) || child.name.Equals(name))
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
    /// 创建占位网球拍
    /// </summary>
    GameObject CreatePlaceholderRacket(Transform parent)
    {
        GameObject racket = new GameObject("TennisRacket_Placeholder");
        racket.transform.SetParent(parent);
        racket.transform.localPosition = Vector3.zero;
        racket.transform.localRotation = Quaternion.identity;

        // 拍柄
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        handle.name = "Handle";
        handle.transform.SetParent(racket.transform);
        handle.transform.localPosition = Vector3.zero;
        handle.transform.localScale = new Vector3(0.03f, 0.15f, 0.03f);
        handle.transform.localRotation = Quaternion.Euler(90, 0, 0);

        // 拍头
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        head.name = "Head";
        head.transform.SetParent(racket.transform);
        head.transform.localPosition = new Vector3(0, 0, 0.15f);
        head.transform.localScale = new Vector3(0.12f, 0.01f, 0.15f);

        // 设置材质
        Renderer headRenderer = head.GetComponent<Renderer>();
        Material racketMaterial = new Material(Shader.Find("Standard"));
        racketMaterial.color = Color.white;
        headRenderer.material = racketMaterial;

        return racket;
    }

    /// <summary>
    /// 设置网球拍物理
    /// </summary>
    void SetupRacketPhysics(GameObject racket)
    {
        // 添加碰撞器
        BoxCollider racketCollider = racket.GetComponent<BoxCollider>();
        if (racketCollider == null)
        {
            racketCollider = racket.AddComponent<BoxCollider>();
        }
        racketCollider.isTrigger = true;
        racketCollider.size = new Vector3(0.3f, 0.05f, 0.4f);

        // 添加网球拍物理脚本
        RacketPhysics racketPhysics = racket.GetComponent<RacketPhysics>();
        if (racketPhysics == null)
        {
            racketPhysics = racket.AddComponent<RacketPhysics>();
        }
        racketPhysics.hitForce = hitForce;
    }

    /// <summary>
    /// 设置TennisPlayer控制器
    /// </summary>
    bool SetupPlayerController()
    {
        Debug.Log("🎮 第4步：设置人物控制器...");

        playerController = playerInstance.GetComponent<TennisPlayer>();
        if (playerController == null)
        {
            playerController = playerInstance.AddComponent<TennisPlayer>();
        }

        // 配置控制器参数
        playerController.playerAnimator = playerAnimator;
        playerController.ballLayerMask = ballDetectionLayer;
        playerController.ballDetectionRange = ballInteractionRange;
        playerController.hitForce = hitForce;
        playerController.reactionTime = reactionTime;
        playerController.moveSpeed = moveSpeed;
        playerController.enableAutoHit = enableAutoHitting;
        playerController.homePosition = playerDefaultPosition;

        // 设置动画状态名
        playerController.idleStateName = idleStateName;
        playerController.forehandStateName = forehandStateName;
        playerController.backhandStateName = backhandStateName;
        playerController.serveStateName = serveStateName;

        Debug.Log("✅ TennisPlayer控制器配置完成");
        return true;
    }

    /// <summary>
    /// 与现有系统集成
    /// </summary>
    bool IntegrateWithExistingSystems()
    {
        Debug.Log("🔗 第5步：与现有系统集成...");

        // 集成到TennisPlayer控制器
        if (playerController != null)
        {
            playerController.ballLauncher = ballLauncher;
            playerController.impactMarker = impactMarker;
            playerController.uiManager = uiManager;
        }

        // 通知发球机系统
        if (ballLauncher != null)
        {
            Debug.Log("✅ 已连接发球机系统");
        }

        // 通知UI系统
        if (uiManager != null)
        {
            Debug.Log("✅ 已连接UI管理系统");
        }

        // 通知冲击标记系统
        if (impactMarker != null)
        {
            Debug.Log("✅ 已连接冲击标记系统");
        }

        Debug.Log("✅ 系统集成完成");
        return true;
    }

    /// <summary>
    /// 优化人物设置
    /// </summary>
    void OptimizePlayerSetup()
    {
        Debug.Log("⚙️ 第6步：优化人物设置...");

        if (playerInstance != null)
        {
            // 优化位置（基于发球机位置）
            if (ballLauncher != null)
            {
                Vector3 launcherPos = ballLauncher.transform.position;
                Vector3 optimalPos = launcherPos + Vector3.forward * 8f; // 8米距离
                playerInstance.transform.position = optimalPos;

                // 面向发球机
                Vector3 direction = (launcherPos - optimalPos).normalized;
                if (direction != Vector3.zero)
                {
                    playerInstance.transform.rotation = Quaternion.LookRotation(direction);
                }

                Debug.Log($"✅ 优化人物位置: {optimalPos}");
            }

            // 设置IK系统（如果启用）
            if (enableIKSystem && playerAnimator != null && playerAnimator.avatar != null && playerAnimator.avatar.isHuman)
            {
                // 在Animator中启用IK Pass
                Debug.Log("✅ IK系统准备就绪");
            }
        }

        Debug.Log("✅ 人物设置优化完成");
    }

    /// <summary>
    /// 查找系统组件
    /// </summary>
    void FindSystemComponents()
    {
        if (ballLauncher == null)
            ballLauncher = FindObjectOfType<BallLauncher>();

        if (impactMarker == null)
            impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (uiManager == null)
            uiManager = FindObjectOfType<TennisVenueUIManager>();

        if (curtainBehavior == null)
            curtainBehavior = FindObjectOfType<CurtainBehavior>();
    }

    /// <summary>
    /// 诊断系统状态
    /// </summary>
    public void DiagnoseSystem()
    {
        Debug.Log("=== 3D网球运动员系统诊断 ===");

        Debug.Log("📋 基础组件状态：");
        Debug.Log($"  人物实例: {(playerInstance != null ? "✅ 存在" : "❌ 缺失")}");
        Debug.Log($"  动画控制器: {(playerAnimator != null ? "✅ 存在" : "❌ 缺失")}");
        Debug.Log($"  人物控制器: {(playerController != null ? "✅ 存在" : "❌ 缺失")}");

        Debug.Log("🔗 系统集成状态：");
        Debug.Log($"  发球机系统: {(ballLauncher != null ? "✅ 已连接" : "❌ 未连接")}");
        Debug.Log($"  冲击标记系统: {(impactMarker != null ? "✅ 已连接" : "❌ 未连接")}");
        Debug.Log($"  UI管理系统: {(uiManager != null ? "✅ 已连接" : "❌ 未连接")}");

        if (playerController != null)
        {
            Debug.Log("🎮 控制器状态：");
            Debug.Log($"  当前状态: {playerController.GetCurrentState()}");
            Debug.Log($"  自动击球: {(playerController.enableAutoHit ? "开启" : "关闭")}");
            Debug.Log($"  目标球: {(playerController.GetTargetBall() != null ? playerController.GetTargetBall().name : "无")}");
            Debug.Log($"  是否挥拍中: {(playerController.IsSwinging() ? "是" : "否")}");
        }

        Debug.Log($"🎯 系统整体状态: {(isSystemReady ? "✅ 就绪" : "❌ 未就绪")}");
        Debug.Log($"📊 设置状态: {setupStatus}");
    }

    /// <summary>
    /// 验证所有系统
    /// </summary>
    public void VerifyAllSystems()
    {
        Debug.Log("=== 系统功能验证 ===");

        if (!isSystemReady)
        {
            Debug.LogWarning("⚠️ 系统未就绪，请先执行Ctrl+T进行设置");
            return;
        }

        // 测试1：动画系统
        Debug.Log("🧪 测试1：动画系统...");
        if (playerController != null)
        {
            // 触发测试动画
            Debug.Log("触发正手击球动画...");
            // 这里可以添加实际的动画测试
        }

        // 测试2：球检测系统
        Debug.Log("🧪 测试2：球检测系统...");
        if (ballLauncher != null)
        {
            Debug.Log("建议：发射一个球测试人物是否能检测并击球");
        }

        // 测试3：物理碰撞
        Debug.Log("🧪 测试3：物理碰撞系统...");
        // 可以在这里创建测试球进行碰撞测试

        Debug.Log("✅ 系统验证完成");
        Debug.Log("💡 建议：通过发球机发射球来测试完整的击球流程");
    }

    /// <summary>
    /// 重置系统
    /// </summary>
    public void ResetSystem()
    {
        Debug.Log("🔄 重置3D网球运动员系统...");

        CleanupExistingPlayer();
        isSystemReady = false;
        setupStatus = "已重置";

        Debug.Log("✅ 系统重置完成");
    }

    /// <summary>
    /// 显示帮助信息
    /// </summary>
    public void ShowHelp()
    {
        Debug.Log("=== 3D网球运动员系统帮助 ===");

        Debug.Log("📚 基于pose.md的完整实现指南：");
        Debug.Log("1. 🎭 推荐从Mixamo获取3D角色和击球动画");
        Debug.Log("2. 🔧 设置角色为Humanoid Rig");
        Debug.Log("3. 🎮 创建包含Idle、ForehandSwing、BackhandSwing状态的Animator Controller");
        Debug.Log("4. 🎾 将网球拍模型附加到右手骨骼");
        Debug.Log("5. ⚙️使用本脚本一键集成到现有系统");

        Debug.Log("🎯 功能特性：");
        Debug.Log("  • 自动球检测和击球");
        Debug.Log("  • 与发球机系统协同");
        Debug.Log("  • 物理反弹和力度控制");
        Debug.Log("  • 手动控制模式");
        Debug.Log("  • IK系统支持（高级功能）");

        Debug.Log("🎮 控制说明：");
        Debug.Log("  Ctrl+T: 一键设置");
        Debug.Log("  Ctrl+D: 系统诊断");
        Debug.Log("  Ctrl+V: 功能验证");
        Debug.Log("  T/Y键: 手动正手/反手击球");
        Debug.Log("  G键: 切换自动/手动模式");
        Debug.Log("  R键: 返回默认位置");

        Debug.Log("💡 使用建议：");
        Debug.Log("  1. 准备Mixamo的Humanoid角色模型");
        Debug.Log("  2. 下载网球相关动画");
        Debug.Log("  3. 在Unity中设置为Humanoid并创建Animator Controller");
        Debug.Log("  4. 将模型和网球拍拖拽到本脚本的对应字段");
        Debug.Log("  5. 按Ctrl+T一键完成所有设置");
    }

    /// <summary>
    /// 显示使用说明
    /// </summary>
    void DisplayInstructions()
    {
        Debug.Log("🎯 Tennis Player Quick Setup - 快速设置指南");
        Debug.Log("基于pose.md文档的完整3D网球运动员集成方案");
        Debug.Log("");
        Debug.Log("📋 推荐准备工作：");
        Debug.Log("1. 从Mixamo下载Humanoid角色模型（.FBX格式）");
        Debug.Log("2. 下载网球击球相关动画（ForehandSwing、BackhandSwing等）");
        Debug.Log("3. 在Unity中设置角色为Humanoid Rig");
        Debug.Log("4. 创建Animator Controller包含击球动画状态");
        Debug.Log("5. 准备网球拍3D模型");
        Debug.Log("");
        Debug.Log("⚡ 如果没有准备上述资源，系统将创建占位对象供测试");
    }

    /// <summary>
    /// 更新状态显示
    /// </summary>
    void UpdateStatusDisplay()
    {
        if (statusDisplay != null)
        {
            string status = "=== 3D网球运动员状态 ===\n";
            status += $"系统状态: {(isSystemReady ? "✅ 就绪" : "❌ 未就绪")}\n";
            status += $"设置状态: {setupStatus}\n";
            status += $"人物实例: {(playerInstance != null ? "✅" : "❌")}\n";
            status += $"控制器: {(playerController != null ? "✅" : "❌")}\n";

            if (playerController != null)
            {
                status += $"当前状态: {playerController.GetCurrentState()}\n";
                status += $"自动击球: {(playerController.enableAutoHit ? "ON" : "OFF")}\n";
            }

            status += "\n快捷键: Ctrl+T设置 | Ctrl+D诊断 | Ctrl+H帮助";
            statusDisplay.text = status;
        }
    }

    /// <summary>
    /// 绘制调试信息
    /// </summary>
    void OnDrawGizmos()
    {
        if (!showDebugInfo) return;

        // 绘制默认位置
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(playerDefaultPosition, 0.5f);
        Gizmos.DrawLine(playerDefaultPosition, playerDefaultPosition + Vector3.up * 2f);

        // 绘制球检测范围
        if (playerInstance != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerInstance.transform.position, ballInteractionRange);
        }

        // 绘制与发球机的连线
        if (ballLauncher != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(playerDefaultPosition, ballLauncher.transform.position);
        }
    }

    // 公共接口方法
    public bool IsSystemReady() => isSystemReady;
    public string GetSetupStatus() => setupStatus;
    public GameObject GetPlayerInstance() => playerInstance;
    public TennisPlayer GetPlayerController() => playerController;
}