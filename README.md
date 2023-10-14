# MaterialPropertyBaker

マテリアルに変更を加えることなく、タイムラインで動的にマテリアルの値を調整することができるシステムです。

## Install

### gitから直接導入

UnityのPackage Managerの`Add package from git URL...` に以下URLを入れることで導入可能

```
https://github.com/sui4/MaterialPropertyBaker.git
```

### ダウンロード後、ローカルで導入

1. 任意のフォルダに git clone や ZIPとしてMaterialPropertyBakerをダウンロード

2. UnityのPackage Managerの`Add package from disk...` からダウンロードしたフォルダ直下にある`package.json`を選択することで導入可能

Git Submoduleを使うことでバージョン管理が用意になります。

## 注意事項

デモプロジェクトにはキャラクタへの使用例としてぞん子3Dモデルを用いています。

※ぞん子3Dモデル（「ZONKO 3D MODEL type-N」）の利用規約を必ずご確認の上、使用してください。
 利用規約：https://zone-energy.jp/3dmodel/terms.pdf

## セットアップ手順

1. マテリアルをタイムラインで制御したい対象に`MPB TargetGroup` コンポーネントをアタッチ
2. `Create MPB Profile` ボタンを押してprofileを生成
3. profileからベースのマテリアルから変更したい値を設定
4. Timelineに`MaterialPropertyBaker/TargetGroup Track` を追加
5. Clipを生成、紐づけたいProfileをClipにアタッチ

6. タイムラインを再生すればprofileの値が適用されます！

## 補助ツール

