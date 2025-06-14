using UnityEngine;

/// <summary>
/// 幕布反弹行为脚本
/// 实现网球撞击幕布时的反弹物理效果
/// 基于reference.md的Vector3.Reflect公式
/// </summary>
public class CurtainBehavior : MonoBehaviour
{
    [Header("反弹参数")]
    [Range(0.1f, 1.0f)]
    [Tooltip("反弹系数，控制反弹力度")]
    public float bounceCoefficient = 0.6f;

    [Range(0f, 2f)]
    [Tooltip("向上推力，模拟幕布的柔软性")]
    public float upwardBoost = 0.5f;

    [Header("调试设置")]
    public bool showDebugInfo = true;
    public bool showImpactEffect = true;

    [Header("音效设置")]
    public AudioClip bounceSound;
    public float soundVolume = 0.3f;

    private AudioSource audioSource;
    private ParticleSystem impactParticles;

    void Start()
    {
        InitializeCurtainBehavior();
    }

    /// <summary>
    /// 初始化幕布行为组件
    /// </summary>
    void InitializeCurtainBehavior()
    {
        // 确保有碰撞器
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
            Debug.Log("✅ CurtainBehavior: 已添加BoxCollider");
        }

        // 设置为触发器以便捕获碰撞事件
        if (!collider.isTrigger)
        {
            collider.isTrigger = false; // 保持物理碰撞
        }

        // 添加音效组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = soundVolume;
        }

        // 创建撞击粒子效果
        CreateImpactParticleSystem();

        if (showDebugInfo)
        {
            Debug.Log($"✅ CurtainBehavior初始化完成 - 反弹系数: {bounceCoefficient}");
        }
    }

    /// <summary>
    /// 创建撞击粒子效果系统
    /// </summary>
    void CreateImpactParticleSystem()
    {
        if (!showImpactEffect) return;

        GameObject particleObj = new GameObject("CurtainImpactParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;

        impactParticles = particleObj.AddComponent<ParticleSystem>();

        var main = impactParticles.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 2f;
        main.startSize = 0.1f;
        main.startColor = Color.white;
        main.maxParticles = 20;

        var emission = impactParticles.emission;
        emission.enabled = false; // 手动触发

        var shape = impactParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;

        Debug.Log("✅ 撞击粒子效果系统已创建");
    }

    /// <summary>
    /// 处理网球碰撞
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        // 检查是否是网球
        if (IsBasketball(collision.gameObject))
        {
            HandleBallBounce(collision);
        }
    }

    /// <summary>
    /// 判断是否是网球对象
    /// </summary>
    bool IsBasketball(GameObject obj)
    {
        // 检查多种可能的网球标识
        return obj.name.ToLower().Contains("ball") ||
               obj.name.ToLower().Contains("tennis") ||
               obj.name.ToLower().Contains("basketball") ||
               obj.tag == "Ball" ||
               obj.GetComponent<Rigidbody>() != null; // 有刚体的球类对象
    }

    /// <summary>
    /// 处理球的反弹
    /// </summary>
    void HandleBallBounce(Collision collision)
    {
        GameObject ball = collision.gameObject;
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();

        if (ballRb == null) return;

        // 获取碰撞信息
        Vector3 contactPoint = collision.contacts[0].point;
        Vector3 contactNormal = collision.contacts[0].normal;
        Vector3 incomingVelocity = ballRb.velocity;

        if (showDebugInfo)
        {
            Debug.Log($"🎾 网球撞击幕布!");
            Debug.Log($"   撞击点: {contactPoint}");
            Debug.Log($"   入射速度: {incomingVelocity}");
            Debug.Log($"   法向量: {contactNormal}");
        }

        // 使用Vector3.Reflect计算反弹速度（基于reference.md规格）
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, contactNormal);

        // 应用反弹系数
        reflectedVelocity *= bounceCoefficient;

        // 添加向上推力模拟幕布的柔软性
        if (upwardBoost > 0)
        {
            reflectedVelocity.y += upwardBoost;
        }

        // 应用新的速度
        ballRb.velocity = reflectedVelocity;

        if (showDebugInfo)
        {
            Debug.Log($"   反弹速度: {reflectedVelocity}");
            Debug.Log($"   反弹系数: {bounceCoefficient}");
        }

        // 播放音效和粒子效果
        PlayBounceEffects(contactPoint);
    }

    /// <summary>
    /// 播放反弹效果
    /// </summary>
    void PlayBounceEffects(Vector3 position)
    {
        // 播放音效
        if (audioSource != null && bounceSound != null)
        {
            audioSource.PlayOneShot(bounceSound);
        }

        // 播放粒子效果
        if (impactParticles != null && showImpactEffect)
        {
            impactParticles.transform.position = position;
            impactParticles.Emit(10);
        }

        // 简单的视觉反馈 - 短暂改变颜色
        StartCoroutine(FlashImpactFeedback());
    }

    /// <summary>
    /// 撞击视觉反馈
    /// </summary>
    System.Collections.IEnumerator FlashImpactFeedback()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            Color originalColor = renderer.material.color;

            // 短暂变亮
            Color flashColor = originalColor + Color.white * 0.2f;
            flashColor.a = originalColor.a;
            renderer.material.color = flashColor;

            yield return new WaitForSeconds(0.1f);

            // 恢复原色
            renderer.material.color = originalColor;
        }
    }

    /// <summary>
    /// 调整反弹参数
    /// </summary>
    [ContextMenu("测试反弹参数")]
    public void TestBounceParameters()
    {
        Debug.Log("=== 幕布反弹参数测试 ===");
        Debug.Log($"反弹系数: {bounceCoefficient}");
        Debug.Log($"向上推力: {upwardBoost}");
        Debug.Log($"调试信息: {showDebugInfo}");
        Debug.Log($"撞击效果: {showImpactEffect}");

        // 创建一个测试球验证反弹
        CreateTestBallForBounce();
    }

    /// <summary>
    /// 创建测试球验证反弹效果
    /// </summary>
    void CreateTestBallForBounce()
    {
        GameObject testBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        testBall.name = "CurtainBounceTestBall";
        testBall.transform.localScale = Vector3.one * 0.067f;

        // 在幕布前方创建球
        Vector3 testPosition = transform.position + Vector3.back * 2f + Vector3.up * 1f;
        testBall.transform.position = testPosition;

        // 添加物理组件
        Rigidbody rb = testBall.AddComponent<Rigidbody>();
        rb.mass = 0.057f;

        // 给球一个向幕布的速度
        Vector3 towardsCurtain = (transform.position - testPosition).normalized;
        rb.velocity = towardsCurtain * 8f + Vector3.up * 2f;

        // 设置橙色材质
        Renderer renderer = testBall.GetComponent<Renderer>();
        Material testMat = new Material(Shader.Find("Standard"));
        testMat.color = Color.cyan;
        renderer.material = testMat;

        Debug.Log("🎾 青色测试球已创建，正在向幕布飞行测试反弹");

        // 5秒后清理
        Destroy(testBall, 5f);
    }

    /// <summary>
    /// 获取反弹信息
    /// </summary>
    public string GetBounceInfo()
    {
        string info = "幕布反弹信息:\n";
        info += $"- 反弹系数: {bounceCoefficient}\n";
        info += $"- 向上推力: {upwardBoost}\n";
        info += $"- 音效: {(bounceSound != null ? "已设置" : "未设置")}\n";
        info += $"- 粒子效果: {(impactParticles != null ? "已启用" : "未启用")}\n";
        info += $"- 调试信息: {showDebugInfo}";

        return info;
    }

    /// <summary>
    /// 设置反弹参数
    /// </summary>
    public void SetBounceParameters(float coefficient, float boost)
    {
        bounceCoefficient = Mathf.Clamp(coefficient, 0.1f, 1.0f);
        upwardBoost = Mathf.Clamp(boost, 0f, 2f);

        if (showDebugInfo)
        {
            Debug.Log($"🔧 反弹参数已更新: 系数={bounceCoefficient}, 推力={upwardBoost}");
        }
    }

    /// <summary>
    /// 启用/禁用调试模式
    /// </summary>
    public void SetDebugMode(bool enabled)
    {
        showDebugInfo = enabled;
        showImpactEffect = enabled;

        Debug.Log($"🔍 幕布调试模式: {(enabled ? "启用" : "禁用")}");
    }
}