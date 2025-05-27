using UnityEngine;

/// <summary>
/// 落点检测快速测试脚本
/// </summary>
public class LandingPointQuickTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 落点检测快速测试启动 ===");

        // 延迟2秒后开始测试，确保所有系统初始化完成
        Invoke("RunQuickTest", 2f);
    }

    void RunQuickTest()
    {
        Debug.Log("--- 开始快速测试 ---");

        LandingPointTracker tracker = FindObjectOfType<LandingPointTracker>();
        if (tracker == null)
        {
            Debug.LogError("❌ 未找到LandingPointTracker组件");
            return;
        }

        Debug.Log("✅ 找到LandingPointTracker组件");
        Debug.Log($"标记创建功能: {(tracker.createLandingMarkers ? "启用" : "禁用")}");

        // 测试1: 手动创建测试标记
        Debug.Log("--- 测试1: 手动创建标记 ---");
        Vector3 testPos1 = new Vector3(2, 0.05f, 3);
        tracker.ManualRecordLandingPoint(testPos1, null);

        // 测试2: 在摄像机前方创建标记
        Debug.Log("--- 测试2: 摄像机前方标记 ---");
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 testPos2 = cam.transform.position + cam.transform.forward * 5f;
            testPos2.y = 0.05f;
            tracker.ManualRecordLandingPoint(testPos2, null);
        }

        // 测试3: 如果有网球，强制检测
        Debug.Log("--- 测试3: 检测现有网球 ---");
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        bool foundBall = false;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("TennisBall"))
            {
                foundBall = true;
                Debug.Log($"找到网球: {obj.name}，强制检测落地状态");
                tracker.ForceCheckBallLanding(obj);
                break;
            }
        }

        if (!foundBall)
        {
            Debug.Log("场景中暂无网球对象");
        }

        Debug.Log("=== 快速测试完成 ===");
        Debug.Log("请观察场景中是否出现红色落点标记");
        Debug.Log("如果看到标记，说明修复成功！");
    }

    void Update()
    {
        // 按F10键重新运行测试
        if (Input.GetKeyDown(KeyCode.F10))
        {
            RunQuickTest();
        }
    }
}