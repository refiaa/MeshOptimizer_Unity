<div align="center">

# Decimater For Unity

<em><h5 align="center">(using Unity 2022.3.22f1)</h5></em>

![img](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/e4747f15-c537-4d83-a6d1-5e69100c244c)

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

```shell
MeshDecimater_Unity
│  .gitignore
│  LICENSE
│  README.jp.md
│  README.md
│  Shader.meta
│  Src.meta
│
├─.github
│  └─ISSUE_TEMPLATE
│          bugreport.yml
│          config.yml
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
![src](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/1830fee5-2ae0-49d0-bac4-929a3e42ab4a)

Meshが入ってるオブジェクトをHierarchyから選択するか、ObjectFieldから選んで使用してください。

Blenderと同様、`Decimate Level`を調整し、`Apply Decimation`を押すことでdecimateが実行されます。

`Revert`したらオリジナルのファイルに戻ります。

ただ、`Revert`は、別のオブジェクトをクリックすると使えなくなりますのでご注意ください。（オリジナルのMeshは残っているので置き換えれば戻せます）

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
