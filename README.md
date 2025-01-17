# LipSyncSetter
[![GitHub release][shields-latest-release]][github-latest]

[shields-latest-release]: https://img.shields.io/github/v/release/c-colloid/LipSyncSetter?display_name=tag&sort=semver
[github-latest]: https://github.com/c-colloid/LipSyncSetter/releases/latest
Version 1.4.1

# 概要
リップシンクをアニメーターに登録して表情との干渉対策が出来ます。

# 使用用途
* 表情アニメーションにリップシンク用のシェイプキーを用意したいとき。
* StateBehaviorでリップシンクを止めた際に変な表情になるのが気になるときに。
* 表情に合わせてリップシンクを変える等したいときに。

# 使い方（従来版）
1. unitypackageをインポート
2. ツールバーのTools>LipSyncSetterを選択
3. 開いたウィンドウにアバターをドラッグ＆ドロップ
4. Cleatボタンを押す

# 使い方（非破壊版）
1. Hierarchy上で設定したいアバターを右クリック
2. 「Add LipSyncSetter」を選択
3. 追加されたオブジェクトのInspectorに顔のメッシュを設定
4. LipSyncの各項目が目的のBlendShapeになっている事を確認、違っていたら目的のBlendShapeをドロップダウンから選択
5. そのままアップロードできます

# その他仕様
* デフォルトではアバターを割り当てると、アバターディスクリプターからリップシンクの情報とFXレイヤーを自動で割り当てます。
* FaceMeshに他のメッシュを割り当てるとそのメッシュのシェイプキーでアニメーションを作成できます。
* LipSyncのドロップダウンメニューにはアバターディスクリプターのシェイプキーがデフォルトで割り当てられますが、自分で変更する事も出来ます。
* デフォルトではアバターに割り当てられているFXレイヤーに上書きしますが、オプションで新しいFXレイヤーに統合する機能があります。

# 連絡先
Twitter @C_Colloid
