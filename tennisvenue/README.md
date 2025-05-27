# 网球场地Unity项目

## 项目概述
这是一个Unity网球模拟项目，包含网球发射器、轨迹预测和物理模拟功能。

## 项目结构
```
tennisvenue/
├── Assets/
│   ├── Materials/          # 材质文件
│   ├── Prefabs/           # 预制体
│   ├── Scenes/            # 场景文件
│   ├── Scripts/           # C#脚本
│   └── UI/               # UI资源
├── ProjectSettings/       # 项目设置
└── Packages/             # 包管理器配置
```

## 功能特点
1. **网球发射器** - BallLauncher组件控制网球发射
2. **物理模拟** - 真实的网球物理碰撞和运动
3. **轨迹预测** - TrajectoryLine显示网球飞行路径
4. **UI控制** - 角度和速度滑块控制发射参数
5. **动态摄像机** - 支持5种预设视角和实时控制

## 最近修复的问题

### Unity MCP Bridge连接错误
**问题**: Unity启动时出现 "Failed to ensure server installation: Error: ConnectFailure" 错误

**原因**: Unity MCP Bridge包尝试连接外部服务器失败，可能是由于：
- 网络连接问题
- 防火墙阻止
- 服务器地址无效

**解决方案**: 已暂时移除 `com.justinpbarnett.unity-mcp` 包以避免连接错误

如需重新启用MCP Bridge功能，请在Packages/manifest.json中添加：
```json
"com.justinpbarnett.unity-mcp": "https://github.com/justinpbarnett/unity-mcp.git?path=/UnityMcpBridge"
```

## 场景视角恢复
如果Unity场景视图角度不合适，可以使用以下方法恢复：

### 🚀 一键视角恢复（推荐）
使用Unity菜单栏中的 `Tools > Scene View` 选项：

1. **最佳俯视角度** (`Tools > Scene View > Best Overview Angle`)
   - 35度俯视角，45度旋转
   - 完美查看整个网球场地和所有元素
   - **这是推荐的最佳视角！**

2. **侧视图** (`Tools > Scene View > Side View`)
   - 90度侧面视角
   - 适合观察网球轨迹和发射路径

3. **正面视图** (`Tools > Scene View > Front View`)
   - 从发射器角度观看
   - 适合调试发射器设置

4. **正上方俯视图** (`Tools > Scene View > Top Down View`)
   - 直接向下俯视
   - 适合查看场地布局

5. **聚焦发射器** (`Tools > Scene View > Focus on Ball Launcher`)
   - 自动聚焦到网球发射器
   - 快速定位关键对象

### 📋 传统方法
1. **快捷键方法**：
   - 选择任意对象（如Main Camera或BallLauncher）
   - 按 `F` 键聚焦到选中对象
   - 使用鼠标滚轮调整缩放

2. **手动调整**：
   - 鼠标中键拖拽：平移视图
   - Alt + 鼠标左键：旋转视图
   - Alt + 鼠标右键：缩放视图

## 使用说明
1. 打开Unity编辑器
2. 加载 `Assets/Scenes/SampleScene.unity` 场景
3. 点击播放按钮开始模拟
4. 使用UI滑块调整发射角度和速度
5. 观察网球轨迹和物理效果

### 🎥 摄像机视角控制
游戏运行时，你可以通过以下方式调整视角：

#### 🔤 预设视角（字母键 - 不使用小键盘）
- **R键**：默认视角 - 重置到标准观察角度
- **T键**：俯视角度 - 从上方观察场地 (Top)
- **F键**：侧面视角 - 从侧面观察轨迹 (Front)
- **C键**：近距离观察 - 靠近发射器 (Close)
- **V键**：全景视角 - 观看整个场地 (View)

#### 🎮 实时控制
- **WASD键**：移动摄像机位置
- **Q/E键**：上下移动摄像机
- **鼠标滚轮**：前后缩放
- **右键拖拽**：旋转视角

#### 📱 UI控制
- 使用UI按钮快速切换预设视角
- 调整视野角度滑块改变FOV
- 实时显示摄像机位置信息

## 技术栈
- Unity 2022.3.57f1c2
- C# 脚本
- Unity Physics
- Unity UI系统

## 维护记录
- 2024-12-19: 修复Unity MCP Bridge连接错误 ✅
- 2024-12-19: 添加场景视角恢复脚本 ✅
- 2024-12-19: 完善项目文档 ✅
- 2024-12-19: 成功测试Unity MCP连接，所有功能正常 ✅

## 故障排除
如遇到其他问题，请检查：
1. Unity版本兼容性
2. 网络连接状态
3. 防火墙设置
4. 项目完整性