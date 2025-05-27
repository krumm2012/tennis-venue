using UnityEngine;

/// <summary>
/// 可见冲击测试 - 创建大的明显的测试圆环
/// </summary>
public class VisibleImpactTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Visible Impact Test Started ===");
        Debug.Log("Press F9 to create large visible test ring");

        // 自动创建一个测试圆环
        CreateLargeTestRing();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            CreateLargeTestRing();
        }
    }

    void CreateLargeTestRing()
    {
        Debug.Log("Creating large visible test ring...");

        // 创建一个大的圆环对象
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "VisibleTestRing";

        // 设置位置在地面上方
        ring.transform.position = new Vector3(0, 0.05f, 2);

        // 设置大小 - 做成扁平的圆环
        ring.transform.localScale = new Vector3(2f, 0.05f, 2f);

        // 设置明亮的颜色
        Renderer renderer = ring.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = Color.cyan;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.cyan * 2f);
        renderer.material = mat;

        // 10秒后销毁
        Destroy(ring, 10f);

        Debug.Log($"✅ Large test ring created at {ring.transform.position}");
        Debug.Log($"Ring scale: {ring.transform.localScale}");
        Debug.Log("Ring should be visible as a bright cyan cylinder");
    }
}