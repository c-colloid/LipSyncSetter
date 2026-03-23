using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using LipSyncSetter.Gatosyocora.VRCAvatars3Tools.Utilitys;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace LipSyncSetter.Editor.Utilities
{
	using VRC.SDK3.Avatars.Components;
	public class LSSAnimationBuilder
	{
		private readonly LSSConfig _config;
		public const string SampleAnimatorGUID = "d957552f3f56f1a41b975c602ffc6e6d";

		public LSSAnimationBuilder(LSSConfig config)
		{
			_config = config;
		}

		public static LSSConfig BuildConfig(VisualElement root)
		{
			var config = new LSSConfig();
			root.Q<ListView>("LipSyncList").Query<PopupField<string>>().ToList().Select((p,i) => (p,i)).ToList().ForEach(o =>{
				config.LipSyncs[o.i].label = o.p.label;
				config.LipSyncs[o.i].value = o.p.value;
			});
			config.FaceMesh = root.Q<ObjectField>("FaceMesh").value as SkinnedMeshRenderer;
			config.DefaultAnimator = root.Q<ObjectField>("FXLayer").value as AnimatorController;
			config.NewAnimator = root.Q<ObjectField>("NewFXLayer").value as AnimatorController;
			return config;
		}

		public static LSSConfig BuildConfig(LipSyncSetterMonoBehavior mono)
		{
			var config = new LSSConfig();
			config.LipSyncs = mono.LipSyncs;
			config.FaceMesh = mono.FaceMesh;
			config.DefaultAnimator = mono?.Root?.DefaultAnimator;
			config.NewAnimator = mono?.Root?.NewAnimator;
			return config;
		}

		public List<AnimationClip> CreateAnime(LSSAvatarData lssAvatarData, AnimationCurve curve = null)
		{
			curve ??= AnimationCurve.Linear(0, 0, 1 / 60f, 100);
			var popups = _config.LipSyncs;

			var clips = popups.Select(p => (new AnimationClip(){name = p.label}
				,p.value
				))
				.Select(c =>
				{
					popups
					.Where(p => p.value != "disable" && p.value != "-none-")
					.ToList().ForEach(p =>
					{
						c.Item1.SetCurve(
						AnimationUtility.CalculateTransformPath(
							(_config.FaceMesh as SkinnedMeshRenderer).transform,
							lssAvatarData.AvatarDescriptor.transform),
						typeof(SkinnedMeshRenderer),
						"blendShape."+ p.value,
							p.value == c.value ? curve : AnimationCurve.Constant(0,1/60f,0)
						);
					}
					);
					return c.Item1;
				}
				)
				.ToList();

			return clips;
		}

		public List<AnimationClip> CreateConstantAnime(LSSAvatarData lssAvatarData)
		{
			return CreateAnime(lssAvatarData, AnimationCurve.Constant(0, 1 / 60f, 100));
		}

		public AnimatorController CreateAnimator(LSSAvatarData lssAvatarData, List<AnimationClip> clips, List<AnimationClip> constantClips = null, bool isNewFXLayer = false)
		{
			if (clips.Count == 0) return null;
			var sampleanimator = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(SampleAnimatorGUID));
			var states = sampleanimator.layers[0].stateMachine.states.ToList();

			AssignClipsToStates(states, clips);
			if (constantClips != null) AssignClipsToBlendTree(states, clips, constantClips);

			var animator = _config.DefaultAnimator;
			var temp_animator = _config.NewAnimator;
			AnimatorController result_animator;

			if (!isNewFXLayer)
			{
				sampleanimator.parameters.Where(p => !animator.parameters.Contains(p))
					.ToList().ForEach(p => animator.AddParameter(p));
				sampleanimator.layers.Select(l => l.name).Where(n => animator.layers.Select(l => l.name).Contains(n)).ToList().ForEach(n => animator.RemoveLayer(Array.IndexOf(animator.layers.Select(l => l.name).ToArray(),n)));
				sampleanimator.layers.ToList().ForEach(l => AnimatorControllerUtility.AddLayer(animator,l,true));
				result_animator = animator;
			}
			else
			{
				states = temp_animator.layers[0].stateMachine.states.ToList();

				AssignClipsToStates(states, clips);
				if (constantClips != null) AssignClipsToBlendTree(states, clips, constantClips);
				if (!animator) return temp_animator;

				animator.parameters.Where(p => !temp_animator.parameters.Contains(p))
					.ToList().ForEach(p => temp_animator.AddParameter(p));
				animator.layers.Select((l,index) => (l,index))
					.ToList().ForEach(i => AnimatorControllerUtility.AddLayer(temp_animator,i.l,i.index == 0));

				var baseLayer = lssAvatarData.AvatarDescriptor.baseAnimationLayers;
				var index = Array.IndexOf(baseLayer, baseLayer.Single(l => l.type == VRCAvatarDescriptor.AnimLayerType.FX));
				baseLayer[index].isDefault = false;
				baseLayer[index].animatorController = temp_animator;
				result_animator = temp_animator;
			}

			return result_animator;
		}

		private void AssignClipsToStates(List<ChildAnimatorState> states, List<AnimationClip> clips)
		{
			var labels = _config.LipSyncs.Select(p => p.label).ToList();
			states.Select(state => (state, labels.IndexOf(state.state.name)))
				.Where(i => i.Item2 >= 0 && !(i.state.state.motion is BlendTree))
				.ToList().ForEach(i => i.state.state.motion = clips[i.Item2]);
		}

		private void AssignClipsToBlendTree(List<ChildAnimatorState> states, List<AnimationClip> clips, List<AnimationClip> constantClips)
		{
			var labels = _config.LipSyncs.Select(p => p.label).ToList();
			states.Select(state => (state, labels.IndexOf(state.state.name)))
				.Where(i => i.Item2 >= 0 && i.state.state.motion is BlendTree)
				.ToList().ForEach(i =>
				{
					var blendTree = (BlendTree)i.state.state.motion;
					if (blendTree.children.Length < 2) return;
					var children = blendTree.children;
					children[0].motion = clips[i.Item2];
					children[1].motion = constantClips[i.Item2];
					blendTree.children = children;
				});
		}

		public static void AddVoiceBoostToMenu(VRCExpressionsMenu targetMenu, VRCExpressionParameters parameters)
		{
			if (targetMenu == null || parameters == null) return;

			// パラメータ追加 (重複チェック + null安全)
			var paramArray = parameters.parameters ?? System.Array.Empty<VRCExpressionParameters.Parameter>();
			if (!paramArray.Any(p => p.name == "VoiceBoost"))
			{
				var paramList = paramArray.ToList();
				paramList.Add(new VRCExpressionParameters.Parameter
				{
					name = "VoiceBoost",
					valueType = VRCExpressionParameters.ValueType.Float,
					defaultValue = 0f,
					saved = true,
					networkSynced = true
				});
				parameters.parameters = paramList.ToArray();
				EditorUtility.SetDirty(parameters);
			}

			// メニュー追加 (重複チェック + 上限チェック)
			var controls = targetMenu.controls ?? new List<VRCExpressionsMenu.Control>();
			targetMenu.controls = controls;
			if (!controls.Any(c => c.name == "VoiceBoost"))
			{
				if (controls.Count >= 8)
				{
					Debug.LogWarning("[LipSyncSetter] VoiceBoost: Target menu already has 8 controls (VRC limit). Cannot add VoiceBoost control.");
					return;
				}
				targetMenu.controls.Add(new VRCExpressionsMenu.Control
				{
					name = "VoiceBoost",
					type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
					subParameters = new[]
					{
						new VRCExpressionsMenu.Control.Parameter { name = "VoiceBoost" }
					}
				});
				EditorUtility.SetDirty(targetMenu);
			}
		}
	}
}
