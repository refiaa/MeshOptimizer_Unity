<div align="center">

# Decimater For Unity

![ezgif-4-3e959bfecd](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/bdb7fedf-1df1-4be4-a9e2-54346e11d564)

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
![image](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/e6651c4f-357f-457a-a3a3-e87d0a38b75e)

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