# Simple Time Countdown

<div align="center">

[**English**](README.md) | [简体中文](README.zh-CN.md)

</div>

一个面向 Windows 11 的轻量级桌面倒计时应用，基于 WPF 和 .NET 8 构建。

![img](example.png)

## 项目特点

Simple Time Countdown 主要围绕三个目标设计：

- 轻量级：采用桌面原生 WPF 技术栈，不依赖浏览器运行时，没有 Electron 一类的额外内存负担
- 低资源占用：使用本地 JSON 存储，无后台同步服务，适合作为常驻桌面小工具长期运行
- 界面简洁：以悬浮卡片和桌面浮层为核心，信息清晰，交互直接，不做复杂冗余的界面元素

## 仓库结构

- `.github/workflows/`：Windows 构建 CI
- `docs/`：项目和维护文档
- `packaging/msix/`：MSIX 清单与打包资源
- `scripts/`：图标生成、发布、安装脚本
- `src/SimpleTimeCountdown.App/`：WPF 主程序
- `src/SimpleTimeCountdown.Setup/`：安装器程序
- `artifacts/`：构建输出目录，不应提交到版本库

## 功能概览

- 类似 Sticky Notes 的桌面悬浮倒计时面板
- 轻量原生架构，适合低开销常驻运行
- 简洁卡片式 UI，适合快速查看和管理截止日期
- 多倒计时卡片，支持置顶、紧急和过期状态
- 中英文界面切换
- 搜索和筛选
- 实验性的桌面层显示模式
- 独立设置窗口
- 可从非交互区域拖动主面板
- 本地 JSON 持久化
- 托盘菜单、开机自启、提醒通知
- 支持 `Setup.exe` 和 `MSIX` 两种发布方式

## 构建

1. 安装带 Windows Desktop 支持的 .NET 8 SDK。
2. 使用 Visual Studio 2022 打开 [SimpleTimeCountdown.sln](/E:/Work/Github%20Repository/Time%20Countdown/SimpleTimeCountdown.sln)，或在 PowerShell 中执行：

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

首次安装自签名开发版 MSIX 时，Windows 可能要求你以管理员 PowerShell 运行安装脚本，以便在系统级信任证书。

经典安装包：

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Build-SetupExe.ps1
```

输出文件：

- 便携版 ZIP：[SimpleTimeCountdown-Release-win-x64-portable.zip](/E:/Work/Github%20Repository/Time%20Countdown/artifacts/packages/SimpleTimeCountdown-Release-win-x64-portable.zip)
- 安装包 EXE：[SimpleTimeCountdown-Setup-win-x64.exe](/E:/Work/Github%20Repository/Time%20Countdown/artifacts/packages/SimpleTimeCountdown-Setup-win-x64.exe)
- MSIX：[SimpleTimeCountdown_1.2.0.0_win-x64.msix](/E:/Work/Github%20Repository/Time%20Countdown/artifacts/packages/SimpleTimeCountdown_1.2.0.0_win-x64.msix)
- 开发证书：[TimeCountdownDev.cer](/E:/Work/Github%20Repository/Time%20Countdown/artifacts/certificates/TimeCountdownDev.cer)
