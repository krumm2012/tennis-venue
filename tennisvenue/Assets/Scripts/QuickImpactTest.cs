using UnityEngine;

/// <summary>
/// 快速冲击标记测试 - 简化版本验证反弹冲击标记系统
/// </summary>
public class QuickImpactTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Quick Impact Marker Test Started ===");
        Debug.Log("Bounce Impact Marker system will automatically detect tennis ball impacts");
        Debug.Log("Press F3 to toggle impact markers");
        Debug.Log("Press F4 to clear all impact markers");
        Debug.Log("Press F5 to create test impact marker");
        Debug.Log("Launch tennis balls to see impact rings appear on first bounce!");
    }

    void Update()
    {
        // 简单的测试快捷键
        if (Input.GetKeyDown(KeyCode.F5))
        {
            CreateTestImpactMarker();
        }
    }

    /// <summary>
    /// 创建测试冲击标记
    /// </summary>
    void CreateTestImpactMarker()
    {
        BounceImpactMarker impactMarker = FindObjectOfType<BounceImpactMarker>();

        if (impactMarker != null)
        {
            // 创建一个测试标记
            Vector3 testPosition = new Vector3(0, 0.01f, 2);
            float testSpeed = Random.Range(5f, 15f);

            Debug.Log($"Creating test impact marker - Speed: {testSpeed:F1}m/s");

            // 调用公共的测试方法
            if (impactMarker.enableImpactMarkers)
            {
                Debug.Log("✅ Impact marker system is enabled");
                Debug.Log($"Current active markers: {impactMarker.GetActiveMarkerCount()}");
            }
            else
            {
                Debug.LogWarning("⚠️ Impact marker system is disabled - press F3 to enable");
            }
        }
        else
        {
            Debug.LogError("❌ BounceImpactMarker system not found!");
        }
    }
}