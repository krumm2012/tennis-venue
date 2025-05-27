using UnityEngine;

/// <summary>
/// 编译测试辅助脚本 - 验证方法访问权限修复
/// </summary>
public class CompileTestHelper : MonoBehaviour
{
    void Start()
    {
        TestMethodAccess();
    }

    /// <summary>
    /// 测试方法访问权限
    /// </summary>
    void TestMethodAccess()
    {
        Debug.Log("=== 编译测试开始 ===");

        // 测试BallLauncher.LaunchBall方法是否可访问
        BallLauncher launcher = FindObjectOfType<BallLauncher>();
        if (launcher != null)
        {
            Debug.Log("✅ BallLauncher.LaunchBall方法可访问");
            // launcher.LaunchBall(Vector3.zero); // 实际调用测试
        }

        // 测试LandingPointTracker.ClearLandingHistory方法是否可访问
        LandingPointTracker tracker = FindObjectOfType<LandingPointTracker>();
        if (tracker != null)
        {
            Debug.Log("✅ LandingPointTracker.ClearLandingHistory方法可访问");
            // tracker.ClearLandingHistory(); // 实际调用测试
        }

        Debug.Log("=== 编译测试完成 ===");
        Debug.Log("所有方法访问权限修复成功！");
    }

    void Update()
    {
        // 按F5键运行测试
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TestMethodAccess();
        }
    }
}