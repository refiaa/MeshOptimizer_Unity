<div align="center">

# Mesh Optimizer For Unity
[![GitHub release](https://img.shields.io/github/release/refiaa/MeshDecimater_Unity.svg?color=Green)](https://github.com/refiaa/MeshDecimater_Unity/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/refiaa/MeshDecimater_Unity/total?color=6451f1)](https://github.com/refiaa/MeshDecimater_Unity/releases/latest)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/68363bc4bcd84df3b43651374cb8caea)](https://app.codacy.com/gh/refiaa/MeshOptimizer_Unity/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![MeshDecimater_Unity issues](https://img.shields.io/github/issues/refiaa/MeshDecimater_Unity?color=yellow)](https://github.com/refiaa/MeshDecimater_Unity/issues)
[![MeshDecimater_Unity License](https://img.shields.io/github/license/refiaa/MeshDecimater_Unity?color=orange)](#)

<em><h5 align="center">(using Unity 2022.3.22f1)</h5></em>

![GIF](./img/showup.gif)

| [English](./README.md) | **日本語** |

Blenderのdecimateみたいな機能をUnity上で使えるようにするPluginです。

**UnityMeshSimplifier**[[1]][UnityMeshSimplifier_github]を利用して作成されました。

<div align="left">

### 導入方法
---

**How to Install**

1.  [ここ][download_link]のファイルをダウンロードし、Assetsに展開してください。
  
2.  次に、[ここ][download_link2]から最新のリリースファイル(Unitypackage)をダウンロードしインポートしたら終わりです。

**File Tree Structure**

導入方法(2)の項目に関しては、treeの構造が以下のようになるようにしてください。

```sql
Assets
├─ *UnityMeshSimplifier* // Should be here !
└─ MeshDecimater_Unity // MeshOptimizer_Unity directory name is `MeshDecimater_Unity`
   ├─  .gitignore
   │  LICENSE
   │  README.jp.md
   │  README.md
   │
   ├─Editor
   │      DecimaterMain.cs
   │      DecimaterMain.cs.meta
   │      MeshInfoDisplay.cs
   │      MeshInfoDisplay.cs.meta
   │      MeshPreviewer.cs
   │      MeshPreviewer.cs.meta
   │      WireframeDrawer.cs
   │      WireframeDrawer.cs.meta
   │
   ├─Runtime
   │      MeshDecimaterUtility.cs
   │      MeshDecimaterUtility.cs.meta
   │      MeshUtils.cs
   │      MeshUtils.cs.meta
   │
   └─Shader
         Wireframe.shader
         Wireframe.shader.meta

```

### 使い方
---
![IMG](./img/main.png)

Meshが入ってるオブジェクトをHierarchyから選択するか、ObjectFieldから選んで使用してください。

Blenderと同様、`Decimate Level`を調整し、`Apply Decimation`を押すことでdecimateが実行されます。

`Revert`したらオリジナルのファイルに戻ります。

ただ、`Revert`は、別のオブジェクトをクリックすると使えなくなりますのでご注意ください。（オリジナルのMeshは残っているので置き換えれば戻せます）

### 更新履歴
---

v0.0.1：
>・リリースしました。

v0.0.2：
>・Apply後にComponentsが消える問題を修正しました。

v0.0.3：
>・BlendsShapeによる問題を解決しました。
>
>・Skinned Mesh RendererがMesh PreviewでApply後に更新されない問題を修正しました。

v0.0.4：
>・Skinned Mesh Rendererのdecimateを最適化しました。

v0.0.5：
>・一部のモデルで発生する`IndexOutOfRangeException`問題を修正
>
>・ tangentsの複製・計算における問題を修正

v0.0.6：
>・preview用のmaterialを外部参照からコード内部生成方式に変えました。

v0.0.6.2：
>・MaterialとWireframeが場合によってロードされない問題を修正
>
>・decimateによるmesh情報を改善
>
>・meshの容量とかの減少量の表記の改善

v0.0.7：
>・`FileNotFoundException Error`を直しました。

v0.0.8：

・submeshにdecimateした時にsubmeshのマテリアル数が１になってしまう問題を修正しました。

・表示方法を変更しました
> wireframeシェーダーを修正しました
>
> submeshにpreviewのマテリアルが適用されない問題を修正しました。

・decimateしたオブジェクトがVRCにアップロードした時に消える問題を修正しました
> decimateを行ったobjectがAssetに保存されるようになりました。

v0.0.9:

> 名前を"Decimate"から"Optimizer"に変更しました
>
> `Revert to Original`を追加しました
> 
> `Optimize(decimate) level`が保存されます

```
work confirmed in

・Unity 2022.3.22f1

・Unity 2019.4.31f1
```

<!-- links -->
  [UnityMeshSimplifier_github]: https://github.com/Whinarn/UnityMeshSimplifier
  [download_link]: https://github.com/Whinarn/UnityMeshSimplifier/releases/tag/v3.1.0
  [download_link2]: https://github.com/refiaa/MeshDecimater_Unity/releases/latest

```
Boothのやつと同じやつです。内容に変わりはありません。(shaderの名前がちょっと違うくらい)
```
