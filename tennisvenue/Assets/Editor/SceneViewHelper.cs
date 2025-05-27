using UnityEngine;
using UnityEditor;

public class SceneViewHelper : MonoBehaviour
{
    [MenuItem("Tools/Scene View/Best Overview Angle")]
    public static void SetBestOverviewAngle()
    {
        // 获取当前的Scene视图
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            // 设置最佳俯视角度 - 能看到整个网球场地和所有元素
            Vector3 courtCenter = new Vector3(0f, 1.5f, 0f);

            sceneView.pivot = courtCenter;
            // 45度角俯视，轻微旋转以获得最佳视角
            sceneView.rotation = Quaternion.Euler(35f, 45f, 0f);
            sceneView.size = 10f; // 适中的缩放距离

            // 刷新Scene视图
            sceneView.Repaint();

            Debug.Log("已设置为最佳俯视视角 - 可以看到整个网球场地");
        }
    }

    [MenuItem("Tools/Scene View/Side View")]
    public static void SetSideView()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            // 侧视图 - 适合观察网球轨迹
            Vector3 courtCenter = new Vector3(0f, 1.5f, 0f);

            sceneView.pivot = courtCenter;
            sceneView.rotation = Quaternion.Euler(0f, 90f, 0f); // 侧面视角
            sceneView.size = 8f;

            sceneView.Repaint();
            Debug.Log("已设置为侧视图 - 适合观察网球轨迹");
        }
    }

    [MenuItem("Tools/Scene View/Front View")]
    public static void SetFrontView()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            // 正面视图 - 从发射器角度观看
            Vector3 courtCenter = new Vector3(0f, 1.5f, 0f);

            sceneView.pivot = courtCenter;
            sceneView.rotation = Quaternion.Euler(15f, 0f, 0f); // 正面视角，稍微向下
            sceneView.size = 12f;

            sceneView.Repaint();
            Debug.Log("已设置为正面视图 - 从发射器角度观看");
        }
    }

    [MenuItem("Tools/Scene View/Focus on Ball Launcher")]
    public static void FocusOnBallLauncher()
    {
        // 查找BallLauncher对象
        GameObject ballLauncher = GameObject.Find("BallLauncher");
        if (ballLauncher != null)
        {
            // 选中对象
            Selection.activeGameObject = ballLauncher;

            // 聚焦到对象
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                sceneView.FrameSelected();
                Debug.Log("已聚焦到网球发射器");
            }
        }
        else
        {
            Debug.LogWarning("未找到BallLauncher对象");
        }
    }

    [MenuItem("Tools/Scene View/Top Down View")]
    public static void SetTopDownView()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            // 正上方俯视图
            Vector3 courtCenter = new Vector3(0f, 1.5f, 0f);

            sceneView.pivot = courtCenter;
            sceneView.rotation = Quaternion.Euler(90f, 0f, 0f); // 直接向下看
            sceneView.size = 8f;

            sceneView.Repaint();
            Debug.Log("已设置为正上方俯视图");
        }
    }

    [MenuItem("Tools/Scene View/Reset to Default")]
    public static void ResetToDefault()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            // 重置到Unity默认视角
            Vector3 defaultPosition = new Vector3(0f, 1f, -10f);

            sceneView.pivot = Vector3.zero;
            sceneView.rotation = Quaternion.identity;
            sceneView.size = 10f;

            sceneView.Repaint();
            Debug.Log("已重置到Unity默认视角");
        }
    }
}