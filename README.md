# GalgameSearchFor

![License](https://img.shields.io/badge/License-MIT-blue.svg)

一个用于搜索 Galgame 资源的 C# 项目，支持从多个网站获取游戏信息。

## 项目简介
GalgameSearchFor 是一个 C# 开发的 Galgame 资源搜索工具，支持从多个网站获取以下信息：
- 游戏名称
- 下载链接
- 文件大小
- 支持语言
- 运行平台

已支持网站：TouchGalgame、真红小站、梓澪の妙妙屋等

## 功能特性
✨ **多网站支持**
- DaoHe
- LiangZiACG 
- MiaoYuan
- TouchGal
- ZhenHong
- ZiLing

🚀 **异步搜索**
- 基于异步编程实现高效搜索

📊 **可视化结果**
- 控制台展示结构化搜索结果
- 实时显示文件大小、语言、平台等信息

⏳ **交互体验**
- 搜索中显示加载动画
- 支持连续搜索流程

## 代码结构
```text
GalgameSearchFor/
├── ConsoleStyle/           # 控制台交互
│   ├── ANSI/               # ANSI转义序列
│   └── Loading/            # 加载动画
│
├── GalGames/               # 核心逻辑
│   ├── Sites/              # 网站解析实现
│   ├── Results/            # 数据结构
│   ├── RequestTable/       # 请求参数
│   ├── ISearcherFormResult.cs
│   ├── IWrieConsole.cs
│   └── SearcherFormResult.cs
│
└── Program.cs              # 主入口
```
## 核心组件
# 类
 - SearcherFormResult<TResult> - 搜索基类（HTTP请求方法）
 - HtmlAnalysisSite<TGalgameInfo> - HTML解析基类
 - 网站解析实现类：DaoHe, LiangZiACG 等

