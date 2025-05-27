using UnityEngine;

/// <summary>
/// 反弹冲击标记诊断工具 - 帮助调试为什么圆环标记没有显示
/// </summary>
public class ImpactMarkerDiagnostic : MonoBehaviour
{
    private BounceImpactMarker impactMarker;

    void Start()
    {
        impactMarker = FindObjectOfType<BounceImpactMarker>();

        Debug.Log("=== Impact Marker Diagnostic Started ===");
        Debug.Log("Press F6 to run diagnostic");
        Debug.Log("Press F7 to force create test marker");
        Debug.Log("Press F8 to check system status");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker system not found!");
        }
        else
        {
            Debug.Log("✅ BounceImpactMarker system found");
            Debug.Log($"System enabled: {impactMarker.enableImpactMarkers}");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            RunDiagnostic();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            ForceCreateTestMarker();
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            CheckSystemStatus();
        }

        // 实时监控网球状态
        MonitorTennisBalls();
    }

    void RunDiagnostic()
    {
        Debug.Log("=== Running Impact Marker Diagnostic ===");

        // 检查系统状态
        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker system not found!");
            return;
        }

        Debug.Log($"✅ System enabled: {impactMarker.enableImpactMarkers}");
        Debug.Log($"✅ Active markers: {impactMarker.GetActiveMarkerCount()}");
        Debug.Log($"✅ Base ring size: {impactMarker.baseRingSize}");
        Debug.Log($"✅ Marker lifetime: {impactMarker.markerLifetime}s");

        // 检查场景中的网球
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int tennisBallCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                tennisBallCount++;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log($"🎾 Found tennis ball: {obj.name}");
                    Debug.Log($"   Position: {obj.transform.position}");
                    Debug.Log($"   Velocity: {rb.velocity} (magnitude: {rb.velocity.magnitude:F2})");
                    Debug.Log($"   Height: {obj.transform.position.y:F3}m");

                    // 检查是否满足冲击条件
                    bool heightOK = obj.transform.position.y <= 0.3f;
                    bool velocityOK = rb.velocity.y < -1f;
                    bool speedOK = rb.velocity.magnitude > 2f;

                    Debug.Log($"   Impact conditions: Height({heightOK}) Velocity({velocityOK}) Speed({speedOK})");
                }
            }
        }

        Debug.Log($"Total tennis balls found: {tennisBallCount}");

        // 检查地面
        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            Debug.Log($"✅ Floor found: {floor.name}");
            Collider floorCollider = floor.GetComponent<Collider>();
            Debug.Log($"   Floor has collider: {floorCollider != null}");
        }
        else
        {
            Debug.LogWarning("⚠️ Floor not found!");
        }
    }

    void ForceCreateTestMarker()
    {
        Debug.Log("=== Force Creating Test Marker ===");

        if (impactMarker == null)
        {
            Debug.LogError("❌ BounceImpactMarker system not found!");
            return;
        }

        // 直接调用私有方法创建标记
        Vector3 testPosition = new Vector3(0, 0.01f, 2);
        float testSpeed = 10f;

        var method = impactMarker.GetType().GetMethod("CreateImpactMarker",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(impactMarker, new object[] { testPosition, testSpeed, Vector3.down * testSpeed });
            Debug.Log($"✅ Test marker created at {testPosition} with speed {testSpeed}m/s");
        }
        else
        {
            Debug.LogError("❌ Could not find CreateImpactMarker method");
        }
    }

    void CheckSystemStatus()
    {
        Debug.Log("=== System Status Check ===");

        if (impactMarker != null)
        {
            Debug.Log(impactMarker.GetSystemStatus());
        }

        // 检查场景中是否有圆环标记
        GameObject[] rings = GameObject.FindObjectsOfType<GameObject>();
        int ringCount = 0;

        foreach (GameObject obj in rings)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("ImpactRing"))
            {
                ringCount++;
                Debug.Log($"🔍 Found ring marker: {obj.name} at {obj.transform.position}");
            }
        }

        Debug.Log($"Total ring markers in scene: {ringCount}");
    }

    void MonitorTennisBalls()
    {
        // 每秒检查一次
        if (Time.frameCount % 60 == 0)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("TennisBall"))
                {
                    Rigidbody rb = obj.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 pos = obj.transform.position;
                        Vector3 vel = rb.velocity;

                        // 检查是否接近地面
                        if (pos.y <= 0.5f && vel.magnitude > 1f)
                        {
                            Debug.Log($"🎾 Ball {obj.name} approaching ground:");
                            Debug.Log($"   Height: {pos.y:F3}m, Speed: {vel.magnitude:F2}m/s");
                            Debug.Log($"   Velocity: {vel}");
                        }
                    }
                }
            }
        }
    }
}