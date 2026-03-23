# LipSyncSetter 次期開発計画

## 最近の更新まとめ（v1.4.2-beta〜v1.5.0）

### 主要な変更点
1. **VoiceBoost機能の実装** — BlendTree + ExpressionMenu統合
2. **カスタムAnimationCurveサポート** — EditorWindow/NDMF両対応
3. **GUIDベースのアセットロード** — フォルダ移動耐性の向上
4. **リファクタリング** — static除去、NDMF VirtualControllerContext API移行、namespace競合解決

---

## 次期開発計画

### A. VoiceBoostの安定化（優先度：高）

| # | 重要度 | 内容 | ファイル | 行 |
|---|--------|------|----------|-----|
| 1 | CRITICAL | `ctx.AvatarDescriptor`のnullチェック不足 | LSSNDMF.cs | 103 |
| 2 | CRITICAL | `expressionParameters`がnullの場合のハンドリング不足 | LSSNDMF.cs | 103-105 |
| 3 | CRITICAL | `parameters.parameters`配列のnull検証不足 | LSSAnimationBuilder.cs | 158 |
| 4 | HIGH | VRCパラメータ上限チェックなし（最大約32） | LSSAnimationBuilder.cs | 160-169 |
| 5 | HIGH | メニューコントロール上限チェックなし（最大約8） | LSSAnimationBuilder.cs | 176-184 |
| 6 | HIGH | `InstallTargetMenu`のnullチェック不足 | LSSNDMF.cs | 102 |
| 7 | MEDIUM | clips/constantClipsの配列境界チェック不足 | LSSNDMF.cs | 69-70 |
| 8 | MEDIUM | clips/constantClipsの配列境界チェック不足（レガシー） | LSSAnimationBuilder.cs | 145-148 |
| 9 | MEDIUM-LOW | targetMenuがnullの場合のサイレント失敗 | LSSNDMF.cs | 105 |
| 10 | MEDIUM-LOW | UXMLのnullチェック不足 | LSSVoiceBoostEditor.cs | 17 |
| 11 | LOW | VoiceBoostセットアップ失敗時のログ出力なし | 複数ファイル | - |

### B. UI/UX改善（優先度：中）

#### 高優先度
- **インラインスタイルのUSS化** — LipSyncSetter.uxmlのハードコードされたスタイルをUSSクラスに抽出
- **レイアウトの改善** — ボタン配置にabsolute positioningではなくflexレイアウトを使用（LipSyncSetter.uxml:20-21）
- **警告色のコントラスト改善** — ライトテーマでの黄色警告が見づらい（LSSBlendShapePanel.cs:47）
- **進捗表示の追加** — アセット操作時のプログレスバー追加（LipSyncSetter.cs:147-156）
- **タイポ修正** — "AnimetionClips" → "AnimationClips"（LipSyncSetter.cs:249）

#### 中優先度
- **ワークフロー明確化** — 必須/任意フィールドの視覚的区別
- **エラーメッセージ改善** — 対処方法を含むメッセージに変更
- **ホバーステート追加** — ボタン等のインタラクティブ要素
- **ツールチップ充実** — AnimationCurveフィールド、VoiceBoost設定の説明追加
- **空状態のUI** — アバター未選択時の案内メッセージ

#### 低優先度
- **Undo対応** — アセット変更前にUndo.RecordObject()を呼ぶ
- **ダイアログの改善** — 成功メッセージをより情報量のあるものに
- **Toggle表示名** — "Toggle" → 意味のあるラベルに変更

### C. ドキュメント整備（優先度：中）

#### 高優先度
- **READMEのバージョン更新** — 1.4.1 → 1.5.0に更新
- **VoiceBoost機能のドキュメント追加** — 設定方法、使い方を記載
- **AnimationCurve機能のドキュメント追加** — カーブの役割と使い方
- **NDMF統合の説明拡充** — 従来方式との違い、セットアップ手順

#### 中優先度
- **CHANGELOGの整合性修正** — v1.5.0とv1.4.2-beta間のバージョン整理
- **package.jsonの更新** — バージョン番号の統一、documentationUrl設定
- **インストール手順の拡充** — VPM/unitypackage/NDMF各方式の手順

#### 低優先度
- **トラブルシューティングセクション追加**
- **英語ドキュメントの検討**
- **連絡先情報の更新**（GitHub Issues追加）

---

## 推奨実施順序

1. **VoiceBoost CRITICAL/HIGHバグ修正**（#1〜#6） — ビルド失敗を防ぐ
2. **タイポ修正 & 簡単なUI修正** — すぐ対応可能な改善
3. **READMEバージョン更新 & VoiceBoostドキュメント追加**
4. **UI レイアウト改善 & ツールチップ追加**
5. **CHANGELOG整理 & package.json更新**
6. **残りのMEDIUM/LOW修正**
