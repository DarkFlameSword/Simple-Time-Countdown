# Simple Time Countdown

<div align="center">

[**English**](README.md) | [**简体中文**](README.zh-CN.md)

</div>

一款面向 Windows 11 的轻量级桌面倒计时应用，基于 WPF + .NET 8 构建。

![img](example.png)

## 项目特点

- 轻量级：原生 WPF，无浏览器运行时和 Electron 级内存负担
- 低占用：本地 JSON 存储，无后台同步服务，适合常驻
- 界面简洁：浮层卡片式 UI，信息聚焦，操作直接

## 仓库结构

- `.github/workflows/`：Windows 构建 CI
- `docs/`：开发流程与维护文档
- `packaging/msix/`：MSIX 清单与打包资源
- `scripts/`：资源生成、发布和安装脚本
- `src/SimpleTimeCountdown.App/`：WPF 主程序
- `src/SimpleTimeCountdown.Setup/`：品牌化安装器
- `artifacts/`：构建输出目录（不应提交）

## 功能

- 类似 Sticky Notes 的桌面悬浮倒计时面板
- 多卡片管理（置顶、状态标签、提醒通知、归档）
- 中英文界面切换
- 搜索与快速定位
- 可选桌面层显示模式（实验性）
- 托盘菜单、开机自启、设置面板
- 本地持久化数据（`%AppData%/TimeCountdown/state.json`）

## 构建

1. 安装 .NET 8 SDK（含 Windows Desktop 支持）
2. 打开 `SimpleTimeCountdown.sln`
3. 执行：

```powershell
dotnet build E:\Work\Github Repository\Time Countdown\SimpleTimeCountdown.sln
```

## 发布

便携版：

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Publish-Portable.ps1
```

MSIX：

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Publish-MSIX.ps1
```

本地安装 MSIX：

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Install-MSIX.ps1
```

经典安装包：

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Build-SetupExe.ps1
```

输出文件：

- 便携包：`SimpleTimeCountdown-Release-win-x64-portable.zip`
- 安装包：`SimpleTimeCountdown-Setup-win-x64.exe`
- MSIX：`SimpleTimeCountdown_1.3.0.0_win-x64.msix`
- 开发证书：`TimeCountdownDev.cer`

## 下载

请优先前往本仓库 GitHub Releases 下载最新版本：

- 最新版本：[Releases / Latest](../../releases/latest)
- 历史版本：[Releases](../../releases)

各安装包区别：

- `SimpleTimeCountdown-Setup-win-x64.exe`（推荐多数用户）
  - 标准安装向导，一键安装
  - 自动创建开始菜单/桌面入口和卸载项
  - 适合日常长期使用
- `SimpleTimeCountdown-Release-win-x64-portable.zip`
  - 免安装，解压即用
  - 不写入系统级安装/卸载记录
  - 适合临时使用、U 盘携带或受限环境
- `SimpleTimeCountdown_*.msix`
  - 基于 MSIX 包模型，安装/卸载更规整
  - 更符合 Windows 包管理体系
  - 适合偏好 MSIX 部署流程的用户
