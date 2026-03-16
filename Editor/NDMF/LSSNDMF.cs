using System.Collections.Immutable;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using nadena.dev.ndmf;
using nadena.dev.ndmf.animator;
using LipSyncSetter.Editor.Utilities;
using VRC.SDK3.Avatars.Components;

[assembly: ExportsPlugin(typeof(LipSyncSetter.NDMF.LSSNDMF))]
namespace LipSyncSetter.NDMF
{
	public class LSSNDMF : Plugin<LSSNDMF>
	{
		protected override void Configure()
		{
			InPhase(BuildPhase.Generating)
				.WithRequiredExtension(typeof(VirtualControllerContext), seq =>
				{
					seq.Run("Generate LipSync", GenerateLipSync);
					seq.Run("Setup VoiceBoost Menu", SetupVoiceBoostMenu);
					seq.Run("Remove Component", ctx =>
					{
						var lssObj = ctx.AvatarRootTransform.GetComponentInChildren<LipSyncSetterMonoBehavior>()?.gameObject;
						if (lssObj != null) Object.DestroyImmediate(lssObj);

						var voiceBoostObj = ctx.AvatarRootTransform.GetComponentInChildren<LSSVoiceBoost>()?.gameObject;
						if (voiceBoostObj != null) Object.DestroyImmediate(voiceBoostObj);
					});
				});
		}

		private static void GenerateLipSync(BuildContext ctx)
		{
			var target = ctx.AvatarRootObject?.GetComponentInChildren<LipSyncSetterMonoBehavior>();
			if (target == null) return;

			target.LSSAvatarData.AvatarDescriptor = ctx.AvatarDescriptor;
			var config = LSSAnimationBuilder.BuildConfig(target);
			var builder = new LSSAnimationBuilder(config);
			var clips = builder.CreateAnime(target.LSSAvatarData);
			if (clips.Count == 0) return;

			var constantClips = builder.CreateConstantAnime(target.LSSAvatarData);

			var controllerCtx = ctx.Extension<VirtualControllerContext>();
			var fxController = (VirtualAnimatorController)controllerCtx.Controllers[VRCAvatarDescriptor.AnimLayerType.FX];

			// サンプルアニメーターを読み込み、VirtualControllerContext 経由で deep clone
			var sampleAnimator = AssetDatabase.LoadAssetAtPath<AnimatorController>(
				AssetDatabase.GUIDToAssetPath(LSSAnimationBuilder.SampleAnimatorGUID)
			);
			var clonedSample = controllerCtx.Clone(sampleAnimator);

			// クローンされた VirtualState にアニメーションクリップを割り当て
			var labels = config.LipSyncs.Select(p => p.label).ToList();
			foreach (var layer in clonedSample.Layers)
			{
				if (layer.StateMachine == null) continue;
				foreach (var state in layer.StateMachine.AllStates())
				{
					var labelIndex = labels.IndexOf(state.Name);
					if (labelIndex < 0) continue;

					if (state.Motion is VirtualBlendTree blendTree && blendTree.Children.Count >= 2)
					{
						// BlendTree children にカーブアニメと固定100アニメを設定
						blendTree.Children[0].Motion = controllerCtx.Clone(clips[labelIndex]);
						blendTree.Children[1].Motion = controllerCtx.Clone(constantClips[labelIndex]);
					}
					else
					{
						// フォールバック: BlendTreeでない場合は直接割り当て
						state.Motion = controllerCtx.Clone(clips[labelIndex]);
					}
				}
			}

			// クローンされたレイヤーを FX コントローラーに追加
			foreach (var layer in clonedSample.Layers)
			{
				layer.DefaultWeight = 1f;
				fxController.AddLayer(LayerPriority.Default, layer);
			}

			// パラメータをマージ
			var parameters = fxController.Parameters;
			foreach (var p in clonedSample.Parameters)
			{
				if (!parameters.ContainsKey(p.Key))
					parameters = parameters.Add(p.Key, p.Value);
			}
			fxController.Parameters = parameters;
		}

		private static void SetupVoiceBoostMenu(BuildContext ctx)
		{
			var voiceBoost = ctx.AvatarRootObject?.GetComponentInChildren<LSSVoiceBoost>();
			if (voiceBoost == null) return;

			var targetMenu = voiceBoost.InstallTargetMenu;
			var expressionParameters = ctx.AvatarDescriptor.expressionParameters;

			LSSAnimationBuilder.AddVoiceBoostToMenu(targetMenu, expressionParameters);
		}
	}
}
