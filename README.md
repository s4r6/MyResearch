# MyResearch

[![Unity](https://img.shields.io/badge/Unity-202X.x-black?logo=unity)](https://unity.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

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
    - View & Infra  
    - Interface Adapter  
    - UseCase  
    - Domain  
  - 個人開発のため、厳密な分離は行っていない

---

## ディレクトリ構成（例）
