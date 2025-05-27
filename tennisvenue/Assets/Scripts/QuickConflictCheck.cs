using UnityEngine;

/// <summary>
/// 快速冲突检查 - 检查圆环标记与TargetPlane的显示冲突
/// </summary>
public class QuickConflictCheck : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Quick Conflict Check Started ===");
        Debug.Log("Press Ctrl+F10 to run conflict analysis");

        // 自动运行检查
        RunConflictCheck();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F10))
        {
            RunConflictCheck();
        }
    }

    void RunConflictCheck()
    {
        Debug.Log("=== Ring-Target Conflict Analysis ===");

        // 查找TargetPlane
        GameObject targetPlane = GameObject.Find("TargetPlane");
        if (targetPlane == null)
        {
            Debug.LogWarning("⚠️ TargetPlane not found!");
            return;
        }

        // 分析TargetPlane
        Vector3 targetPos = targetPlane.transform.position;
        Vector3 targetScale = targetPlane.transform.localScale;
        int targetLayer = targetPlane.layer;

        Debug.Log($"🎯 TargetPlane Analysis:");
        Debug.Log($"   Position: {targetPos}");
        Debug.Log($"   Scale: {targetScale}");
        Debug.Log($"   Layer: {targetLayer} ({LayerMask.LayerToName(targetLayer)})");

        // 检查渲染器
        Renderer targetRenderer = targetPlane.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            Material mat = targetRenderer.material;
            Debug.Log($"   Material: {mat.shader.name}");
            Debug.Log($"   Color: {mat.color}");
            Debug.Log($"   Render Queue: {mat.renderQueue}");
        }

        // 分析圆环标记设置
        BounceImpactMarker impactMarker = FindObjectOfType<BounceImpactMarker>();
        if (impactMarker != null)
        {
            Debug.Log($"⭕ Ring Marker Analysis:");
            Debug.Log($"   Ring height offset: 0.01m (from ground)");
            Debug.Log($"   Ring render queue: 3000 (transparent)");
            Debug.Log($"   Ring layer: 0 (default)");
        }

        // 计算冲突风险
        float heightDiff = 0.01f - targetPos.y; // 圆环高度 - TargetPlane高度
        Debug.Log($"📏 Height Analysis:");
        Debug.Log($"   Height difference: {heightDiff:F3}m");

        if (heightDiff >= 0.02f)
        {
            Debug.Log($"✅ No Z-fighting risk - sufficient height separation");
        }
        else if (heightDiff > 0)
        {
            Debug.LogWarning($"⚠️ Minimal height separation - monitor for Z-fighting");
        }
        else
        {
            Debug.LogError($"❌ Negative height difference - rings below TargetPlane!");
        }

        // 检查现有圆环标记
        CheckExistingRings();

        Debug.Log("=== Conflict Analysis Complete ===");
    }

    void CheckExistingRings()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int ringCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("ImpactMarker") || obj.name.Contains("ImpactRing") || obj.name.Contains("TestRing"))
            {
                ringCount++;
                Debug.Log($"🔍 Found ring: {obj.name} at {obj.transform.position}");

                // 检查与TargetPlane的距离
                GameObject targetPlane = GameObject.Find("TargetPlane");
                if (targetPlane != null)
                {
                    float distance = Vector3.Distance(obj.transform.position, targetPlane.transform.position);
                    if (distance < 1.0f)
                    {
                        Debug.LogWarning($"⚠️ Ring {obj.name} is close to TargetPlane (distance: {distance:F2}m)");
                    }
                }
            }
        }

        Debug.Log($"📊 Total rings found: {ringCount}");
    }
}