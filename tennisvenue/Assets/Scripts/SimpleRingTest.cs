using UnityEngine;

/// <summary>
/// 简单圆环测试 - 立即创建可见的圆环
/// </summary>
public class SimpleRingTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== Simple Ring Test Started ===");

        // 立即创建一个大的可见圆环
        CreateVisibleRing();

        Debug.Log("Press F9 to create another ring");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            CreateVisibleRing();
        }
    }

    void CreateVisibleRing()
    {
        Debug.Log("Creating visible ring...");

        // 创建圆柱体
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "VisibleRing_" + Time.time;

        // 设置位置在地面上方
        ring.transform.position = new Vector3(Random.Range(-1f, 1f), 0.1f, Random.Range(0f, 3f));

        // 设置大小 - 扁平的圆环
        ring.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);

        // 设置明亮的材质
        Renderer renderer = ring.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));

        // 随机颜色
        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };
        Color ringColor = colors[Random.Range(0, colors.Length)];

        mat.color = ringColor;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", ringColor * 2f);

        renderer.material = mat;

        // 10秒后销毁
        Destroy(ring, 10f);

        Debug.Log($"✅ Visible ring created at {ring.transform.position}");
        Debug.Log($"Color: {ringColor}, Scale: {ring.transform.localScale}");
    }
}