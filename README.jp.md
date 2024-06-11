<div align="center">

# Decimater For Unity

<em><h5 align="center">(using Unity 2022.3.22f1)</h5></em>

[![GitHub release](https://img.shields.io/github/release/refiaa/MeshDecimater_Unity.svg?color=Green)](https://github.com/refiaa/MeshDecimater_Unity/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/refiaa/MeshDecimater_Unity/total?color=6451f1)](https://github.com/refiaa/MeshDecimater_Unity/releases/latest)
[![MeshDecimater_Unity issues](https://img.shields.io/github/issues/refiaa/MeshDecimater_Unity?color=yellow)](https://github.com/refiaa/MeshDecimater_Unity/issues)
[![MeshDecimater_Unity License](https://img.shields.io/github/license/refiaa/MeshDecimater_Unity?color=orange)](#)

| [English](./README.md) | **日本語** |

Blenderのdecimateみたいな機能をUnity上で使えるようにするPluginです。

**UnityMeshSimplifier**[[1]][UnityMeshSimplifier_github]を利用して作成されました。

<div align="left">

### 導入方法
---

1.  [ここ][download_link]のファイルをダウンロードし、Assetsに展開してください。
  
2.  次に、[ここ][download_link2]から最新のリリースファイル(Unitypackage)をダウンロードしインポートしたら終わりです。


### 使い方
---

Meshが入ってるオブジェクトをHierarchyから選択するか、ObjectFieldから選んで使用してください。

![image](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/e6651c4f-357f-457a-a3a3-e87d0a38b75e)

Blenderと同様、`Decimate Level`を調整し、`Apply Decimation`を押すことでdecimateが実行されます。

`Revert`したらオリジナルのファイルに戻ります。

ただ、`Revert`は、別のオブジェクトをクリックすると使えなくなりますのでご注意ください。（オリジナルのMeshは残っているので置き換えれば戻せます）

<!-- links -->
  [UnityMeshSimplifier_github]: https://github.com/Whinarn/UnityMeshSimplifier
  [download_link]: https://github.com/Whinarn/UnityMeshSimplifier/releases/tag/v3.1.0
  [download_link2]: https://github.com/refiaa/MeshDecimater_Unity/releases/latest
