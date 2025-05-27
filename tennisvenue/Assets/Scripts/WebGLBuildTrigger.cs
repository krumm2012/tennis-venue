using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
#endif

public class WebGLBuildTrigger : MonoBehaviour
{
    [Header("WebGL构建控制")]
    [SerializeField] private bool autoStartBuild = false;
    
    void Start()
    {
        if (autoStartBuild)
        {
            Debug.Log("🌐 自动启动WebGL构建...");
            StartWebGLBuild();
        }
    }
    
    void Update()
    {
        // 按Ctrl+B触发WebGL构建
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("🎮 检测到Ctrl+B快捷键，启动WebGL构建...");
            StartWebGLBuild();
        }
    }
    
    public void StartWebGLBuild()
    {
#if UNITY_EDITOR
        Debug.Log("🚀 准备执行WebGL构建...");
        
        // 停止游戏模式（如果正在运行）
        if (EditorApplication.isPlaying)
        {
            Debug.Log("⏹️ 停止游戏模式...");
            EditorApplication.isPlaying = false;
            
            // 延迟执行构建，等待游戏模式完全停止
            EditorApplication.delayCall += () => {
                ExecuteWebGLBuild();
            };
        }
        else
        {
            ExecuteWebGLBuild();
        }
#else
        Debug.LogWarning("⚠️ WebGL构建只能在Unity编辑器中执行");
#endif
    }
    
#if UNITY_EDITOR
    private void ExecuteWebGLBuild()
    {
        try
        {
            Debug.Log("📋 配置构建设置...");
            
            // 1. 设置构建场景
            string[] scenes = { "Assets/Scenes/SampleScene.unity" };
            
            // 2. 配置Player Settings
            PlayerSettings.companyName = "Tennis Venue Studio";
            PlayerSettings.productName = "Tennis Venue Simulator";
            PlayerSettings.bundleVersion = "1.0.0";
            
            // WebGL特定设置
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.memorySize = 512;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.decompressionFallback = true;
            
            // 3. 设置构建选项
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.scenes = scenes;
            buildOptions.locationPathName = "WebGL-Build";
            buildOptions.target = BuildTarget.WebGL;
            buildOptions.options = BuildOptions.None;
            
            Debug.Log("🎯 构建目标: WebGL");
            Debug.Log("📁 输出路径: WebGL-Build");
            Debug.Log("🎮 场景: SampleScene");
            
            // 4. 开始构建
            Debug.Log("⚡ 开始WebGL构建...");
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            
            // 5. 检查结果
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("🎉 WebGL构建成功完成！");
                Debug.Log($"📦 构建大小: {report.summary.totalSize / (1024 * 1024):F2} MB");
                Debug.Log($"⏱️ 构建时间: {report.summary.totalTime}");
                
                ShowPostBuildInstructions();
            }
            else
            {
                Debug.LogError($"❌ WebGL构建失败: {report.summary.result}");
                Debug.LogError($"错误数量: {report.summary.totalErrors}");
                Debug.LogError($"警告数量: {report.summary.totalWarnings}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"💥 构建过程中发生异常: {e.Message}");
            Debug.LogError($"堆栈跟踪: {e.StackTrace}");
        }
    }
    
    private void ShowPostBuildInstructions()
    {
        Debug.Log("📖 WebGL构建完成！后续步骤:");
        Debug.Log("1. 🖥️ 本地测试:");
        Debug.Log("   cd WebGL-Build");
        Debug.Log("   python -m http.server 8000");
        Debug.Log("   浏览器访问: http://localhost:8000");
        Debug.Log("");
        Debug.Log("2. 🌐 部署到网络:");
        Debug.Log("   • GitHub Pages: 上传到GitHub仓库");
        Debug.Log("   • Netlify: 拖拽文件夹到netlify.com");
        Debug.Log("   • 自己的服务器: 上传WebGL-Build文件夹内容");
        Debug.Log("");
        Debug.Log("3. 🎮 游戏控制说明:");
        Debug.Log("   • 空格键/鼠标左键: 发射网球");
        Debug.Log("   • WASD: 摄像机移动");
        Debug.Log("   • R/T/F/C/V: 切换视角");
        Debug.Log("   • 滑块: 调整角度、速度、方向");
    }
#endif
    
    // 在Inspector中显示构建按钮
    [ContextMenu("立即执行WebGL构建")]
    public void BuildWebGLFromContext()
    {
        StartWebGLBuild();
    }
}