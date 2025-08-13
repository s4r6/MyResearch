# MyResearch

[![Unity](https://img.shields.io/badge/Unity-6000.1.0f1-black?logo=unity)](https://unity.com/)
[![UniRx](https://img.shields.io/badge/Library-UniRx-blue)](https://github.com/neuecc/UniRx)
[![UniTask](https://img.shields.io/badge/Library-UniTask-blue)](https://github.com/Cysharp/UniTask)
[![DoTween](https://img.shields.io/badge/Library-DoTween-blue)](http://dotween.demigiant.com/)

## プロジェクト概要
本プロジェクトは、**3D空間上でリスクアセスメントを体験・学習できるゲーム**です。  
大学院の研究の一環として開発しており、WebGL環境で動作します。  
複数のプレイヤーが同一の仮想空間で学習を行えるよう、**WebSocketを利用したマルチプレイ**機能を実装しています。  

ゲームサーバーは別リポジトリで管理しています。  
サーバー側リポジトリ: [https://github.com/s4r6/MyResearch](https://github.com/s4r6/MyResearch)

---

## 使用している主な技術
- **ゲームエンジン**: Unity  
- **通信**: WebSocketによるリアルタイム通信（マルチプレイ対応）  
- **ビルドターゲット**: WebGL  
- **アーキテクチャ**: Clean Architectureを参考  
  - 主に以下の4層構造  
    - View & Infrastracture
    - Interface Adapter  
    - UseCase  
    - Domain  
  - 開発速度との兼ね合いで一部上記の4層構造を守っていない部分があります

---

## ディレクトリ構成
```
MyResearch/
└── Assets/
  ├── Scripts/
  │ ├──EntryPoint/ # システムのエントリーポイント(DI)
  │ ├── Domain/ # ゲームのドメインロジック
  │ ├── UseCase/ # アプリケーションのユースケース
  │ ├── Presenter/ # Presenterなど
  │ ├── View/ # Frameworkやライブラリ依存部分のうち画面に関係するもの（UI, GameObject等）
  | └── Infrastracture/ # Frameworkやライブラリ依存部分のうち画面とは関係ないもの (WebSocket, Serializer等)
  ├── Scenes/ # Unityシーンデータ
  └── Resources/ # 各種リソース（JSON, テクスチャなど）
    └── Prefabs/ # プレハブデータ
```
