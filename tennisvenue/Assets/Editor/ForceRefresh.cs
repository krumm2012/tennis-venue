using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ForceRefresh
{
    static ForceRefresh()
    {
        EditorApplication.delayCall += RefreshAssets;
    }

    static void RefreshAssets()
    {
        Debug.Log("🔄 强制刷新资源数据库...");
        AssetDatabase.Refresh();
        Debug.Log("✅ 资源数据库刷新完成");
    }

    [MenuItem("Tools/Force Refresh Assets")]
    public static void ManualRefresh()
    {
        Debug.Log("🔄 手动刷新资源数据库...");
        AssetDatabase.Refresh();
        Debug.Log("✅ 手动刷新完成");
    }
}