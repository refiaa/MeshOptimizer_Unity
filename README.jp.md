<div align="center">

# Decimater For Unity

<em><h5 align="center">(using Unity 2022.3.22f1)</h5></em>

![img](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/e4747f15-c537-4d83-a6d1-5e69100c244c)

| [English](./README.md) | **日本語** |

Blenderのdecimateみたいな機能をUnity上で使えるようにするPluginです。

**UnityMeshSimplifier**[[1]][UnityMeshSimplifier_github]を利用して作成されました。

<div align="left">

## VRCSDK3との互換性問題

現在、このDecimaterツールとVRCSDK3-AVATARとの間に重大な互換性の問題が発生しています。以下の症状が確認されています：

1. **アップロード時の異常動作**：
   - Decimaterをインポートした状態でアバターをアップロードすると、VRCSDKが予期せぬ動作をします。
   - 具体的には、Hierarchy上のプレハブではなく、AssetsフォルダにBlueprint IDを名前とする新たなPrefabが生成され、それがアップロード対象として参照されてしまいます。

2. **エラーの発生**：
   - 上記の動作により、`FileNotFoundException`が発生し、アップロードが失敗します。

### 問題の詳細

- この問題はDecimaterツールをプロジェクトにインポートしただけで発生します。
- Decimating操作を実行しなくても問題が再現されます。
- 問題の原因はVRCSDKとの相互作用にあると考えられますが、VRCSDKの`VRCAvatarBuilder`クラスと`VRC.SDK3.Builder.VRCAvatarBuilder.ExportCurrentAvatarResource`メソッドの内部実装の把握ができないため、対応が困難な状況です。


### 導入方法
---

**How to Install**

1.  [ここ][download_link]のファイルをダウンロードし、Assetsに展開してください。
  
2.  次に、[ここ][download_link2]から最新のリリースファイル(Unitypackage)をダウンロードしインポートしたら終わりです。

**File Tree Structure**

導入方法(2)の項目に関しては、treeの構造が以下のようになるようにしてください。

```shell
Assets
├─MeshDecimater_Unity
│  ├─Material
│  ├─Shader
│  └─src
└─UnityMeshSimplifier
    ├─.circleci
    │  ├─ProjectSettings
    │  └─scripts
    ├─.github
    │  ├─ISSUE_TEMPLATE
    │  └─workflows
    ├─Editor
    ├─Runtime
    │  ├─Components
    │  ├─Exceptions
    │  ├─Internal
    │  ├─Math
    │  └─Utility
    └─Tests
        └─Editor
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
