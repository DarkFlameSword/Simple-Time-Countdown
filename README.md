# Simple Time Countdown

<div align="center">

[**English**](README.md) | [简体中文](README.zh-CN.md)

</div>


A lightweight floating countdown app for Windows 11, built with WPF and .NET 8.

## Why this project

Simple Time Countdown is designed around three product goals:

- Lightweight: desktop-native WPF app, no browser runtime, no Electron-style memory overhead
- Low resource usage: local JSON storage, no background sync service, and a small always-available footprint
- Clean UI: floating glass-style panel, focused countdown cards, and a simple desktop-first interaction model

## Repository structure

- `.github/workflows/` contains CI for Windows builds
- `docs/` contains repository and release documentation
- `packaging/msix/` contains the MSIX manifest and packaging assets
- `scripts/` contains asset generation, publish, and install scripts
- `src/SimpleTimeCountdown.App/` contains the WPF desktop app
- `src/SimpleTimeCountdown.Setup/` contains the branded installer
- `artifacts/` is generated output only and should not be committed

## Features

- Floating desktop panel inspired by Sticky Notes
- Lightweight desktop-native architecture focused on low overhead
- Clean, minimal UI that stays readable as a desktop utility
- Multiple countdown cards with pinned, urgent, and overdue states
- English and Simplified Chinese UI language switching
- Search and filter for all, urgent, pinned, and overdue items
- Experimental desktop-layer mode that can attach the panel to the Windows desktop host
- Dedicated settings window for display, startup, defaults, and panel placement
- Drag the floating panel from any non-interactive area, not only the header
- Local JSON persistence under `%AppData%\TimeCountdown\state.json`
- Tray icon with quick show, add, settings, always-on-top toggle, and exit
- Registry-based launch at startup
- Reminder balloons and due notifications
- App icon assets plus portable and MSIX packaging scripts

## Build

1. Install the .NET 8 SDK with WPF desktop support.
2. Open [SimpleTimeCountdown.sln](/E:/Work/Github%20Repository/Time%20Countdown/SimpleTimeCountdown.sln) in Visual Studio 2022 or build from a Developer PowerShell.
3. Run:

```powershell
dotnet build E:\Work\Github Repository\Time Countdown\SimpleTimeCountdown.sln
```

## Publish

Portable zip:

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Publish-Portable.ps1
```

MSIX package:

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Publish-MSIX.ps1
```

Install the generated MSIX locally:

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Install-MSIX.ps1
```

For the first install of the self-signed development package, Windows may require running the install step from an elevated PowerShell so the certificate can be trusted at the machine level.

Classic `Setup.exe` installer:

```powershell
powershell -ExecutionPolicy Bypass -File E:\Work\Github Repository\Time Countdown\scripts\Build-SetupExe.ps1
```

Outputs:

- Portable zip: [SimpleTimeCountdown-Release-win-x64-portable.zip](/E:/Work/Github%20Repository/Time%20Countdown/artifacts/packages/SimpleTimeCountdown-Release-win-x64-portable.zip)
- Setup EXE: [SimpleTimeCountdown-Setup-win-x64.exe](/E:/Work/Github%20Repository/Time%20Countdown/artifacts/packages/SimpleTimeCountdown-Setup-win-x64.exe)
- MSIX: [SimpleTimeCountdown_1.1.0.0_win-x64.msix](/E:/Work/Github%20Repository/Time%20Countdown/artifacts/packages/SimpleTimeCountdown_1.1.0.0_win-x64.msix)
- Dev certificate: [TimeCountdownDev.cer](/E:/Work/Github%20Repository/Time%20Countdown/artifacts/certificates/TimeCountdownDev.cer)
