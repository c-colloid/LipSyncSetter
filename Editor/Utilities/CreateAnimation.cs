using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using LipSyncSetter.Gatosyocora.VRCAvatars3Tools.Utilitys;

namespace LipSyncSetter.Editor.Utilities
{
	using VRC.SDK3.Avatars.Components;
	public static class LSSUtility
	{
		static List<AnimationClip> cliplist = new List<AnimationClip>();
		static LSSConfig lssroot;
		const string SampleAnimatorGUID = "c349e20dc611ea543b1c93cc8db32def";
		
		public static List<AnimationClip> CreateAnime(LSSConfig root , LSSAvatarData LSSAvatarData)
		{
			var popups = root.LipSyncs;
				//root.Q<ListView>("LipSyncList").Query<PopupField<string>>().ToList();
			
			//for (int i = 0; i < popups.Count(); i++) {
			//	var shapekey = popups[i];
			//	var clip = new AnimationClip();
			//	var curve = new AnimationCurve();
			//	//var keyframe = new Keyframe(0.02f,0);
			//	clip.name = shapekey.label;
			//	var path = AnimationUtility.CalculateTransformPath((root.Q<ObjectField>("FaceMesh").value as SkinnedMeshRenderer).transform,LSSAvatarData.AvatarDescriptor.transform);
			//	foreach (var item in popups)
			//	{
			//		if (item.text == "disable"||item.text == "-none-") continue;
			//		curve.keys = null;
			//		curve.AddKey(0,0);
			//		if (i < popups.Count()-1)
			//		{
			//			bool shapekeybool = false;
			//			if (item.text == shapekey.text)
			//				shapekeybool = true;
			//			//keyframe.value = System.Convert.ToInt32(shapekeybool) * 100;
			//			//curve.AddKey(keyframe);
			//			curve = AnimationCurve.Linear(0,0,1/60.0f,System.Convert.ToInt32(shapekeybool) * 100);
			//		}
			//		clip.SetCurve(path,typeof(SkinnedMeshRenderer),"blendShape."+ item.text,curve);
			//	}
			//	//AssetDatabase.CreateAsset(clip,savefolder + "/v." + clip.name + ".anim");
			//	cliplist.Add(clip);
			//}
			
			
			cliplist = popups.Select(p => (new AnimationClip(){name = p.label}
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
							(root.FaceMesh as SkinnedMeshRenderer).transform,
							LSSAvatarData.AvatarDescriptor.transform),
						typeof(SkinnedMeshRenderer),
						"blendShape."+ p.value,
						AnimationCurve.Linear(0,0,1/60.0f,System.Convert.ToInt32(p.value == c.value) * 100)
						);
					}
					);
					return c.Item1;
				}
				)
				.ToList();
			
			return cliplist;
		}
		
		public static List<AnimationClip> CreateAnime(VisualElement root , LSSAvatarData LSSAvatarData)
		{
			if (lssroot == null) SetLSSRoot(root);
			
			return CreateAnime(lssroot,LSSAvatarData);
		}
		
		public static List<AnimationClip> CreateAnime(LipSyncSetterMonoBehavior root , LSSAvatarData LSSAvatarData)
		{
			if (lssroot == null) SetLSSRoot(root);
			
			return CreateAnime(lssroot,LSSAvatarData);
		}
		
		public static AnimatorController CreateAnimator(LSSConfig root,LSSAvatarData LSSAvatarData, bool isNewFXLayer = false)
		{
			if (cliplist.Count() == 0) return null;
			var sampleanimator = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(SampleAnimatorGUID));
			var states = sampleanimator.layers[0].stateMachine.states.ToList();
			var popups = root.LipSyncs;
			List<string> texts = popups.Select(p => p.label).ToList();
		
			//foreach (var popup in popups)
			//{
			//	texts.Add(popup.label);
			//}
			//foreach (var item in clip)
			//{
			//	var num = texts.IndexOf(item.state.name);
			//	if (num < 0)
			//		continue;
			//	item.state.motion = cliplist[num];
			//}
			states.Select(state =>(state,texts.IndexOf(state.state.name))).Where(i=> i.Item2 >= 0)
				.ToList().ForEach(i => i.state.state.motion = cliplist[i.Item2]);
			
			var animator = root.DefaultAnimator;
			var temp_animator = root.NewAnimator;
			AnimatorController result_animator;
			ScriptableObject.DestroyImmediate(lssroot);
		
			//元のFXレイヤーにリップシンクを追加する場合
			if (!isNewFXLayer)
			{
				//AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(_animator),_savefolder + "/LipSyncFXLayer.controller");
				//サンプルからパラメーターをコピー
				//foreach (var parameter in sampleanimator.parameters)
				//{
				//	//パラメーターが既にある場合スキップ
				//	var newparameters = animator.parameters.ToList();
				//	if (newparameters.IndexOf(parameter) < 0)
				//	{
				//		animator.AddParameter(parameter);
				//	}
				//}
				
				//サンプルからレイヤーをコピー
				//foreach (var layer in sampleanimator.layers)
				//{
				//	//LipSyncLayerが既に存在していたらレイヤーを削除
				//	List<string> newlayers = new List<string>();
				//	foreach (var newlayer in animator.layers)
				//	{
				//		newlayers.Add(newlayer.name);
				//	}
				//	if (newlayers.IndexOf(layer.name) >= 0)
				//	{
				//		animator.RemoveLayer(newlayers.IndexOf(layer.name));
				//	}
				
				//	AnimatorControllerUtility.AddLayer(animator,layer,true);
				//}
				
				sampleanimator.parameters.Where(p => !animator.parameters.Contains(p))
					.ToList().ForEach(p => animator.AddParameter(p));
				sampleanimator.layers.Select(l => l.name).Where(n => animator.layers.Select(l => l.name).Contains(n)).ToList().ForEach(n => animator.RemoveLayer(Array.IndexOf(animator.layers.Select(l => l.name).ToArray(),n)));
				sampleanimator.layers.ToList().ForEach(l => AnimatorControllerUtility.AddLayer(animator,l,true));
				result_animator = animator;
			}
		
			//新FXレイヤーに統合する場合
			else
			{
				states = temp_animator.layers[0].stateMachine.states.ToList();
				//foreach (var item in clip)
				//{
				//	var num = texts.IndexOf(item.state.name);
				//	if (num < 0)
				//		continue;
				//	item.state.motion = cliplist[num];
				//}
				states.Select(state =>(state,texts.IndexOf(state.state.name))).Where(i=> i.Item2 >= 0)
					.ToList().ForEach(i => i.state.state.motion = cliplist[i.Item2]);
				if (!animator) return temp_animator;
				
				////アバターにあるアニメーターからパラメーターをコピー
				//foreach (var A_Parameter in animator.parameters)
				//{
				//	temp_animator.AddParameter(A_Parameter);
				//}
				////アバターにあるアニメーターからレイヤーをコピー
				//int i = 0;
				//foreach (var A_layer in animator.layers)
				//{
				//	if (i == 0)
				//	{
				//		AnimatorControllerUtility.AddLayer(temp_animator,A_layer,true);
				//	}
				//	else
				//	{
				//		AnimatorControllerUtility.AddLayer(temp_animator,A_layer);
				//	}
				//	i++;
				//}
				animator.parameters.Where(p => !temp_animator.parameters.Contains(p))
					.ToList().ForEach(p => temp_animator.AddParameter(p));
				animator.layers.Select((l,index) => (l,index))
					.ToList().ForEach(i => AnimatorControllerUtility.AddLayer(temp_animator,i.l,i.index == 0 ? true : false));
				
				var baseLayer = LSSAvatarData.AvatarDescriptor.baseAnimationLayers;
				var index = Array.IndexOf(baseLayer, baseLayer.Single(l => l.type == VRCAvatarDescriptor.AnimLayerType.FX));
				baseLayer[index].isDefault = false;
				baseLayer[index].animatorController = temp_animator;
				result_animator = temp_animator;
			}
		
			//アバターにあるアニメーターにレイヤーをコピー(レイヤー順調整用)
			//foreach (var temp_layer in temp_animator.layers)
			//{
			//	AnimatorControllerUtility.AddLayer(animator,temp_layer,true);
			//}
			
			return result_animator;
		}
		
		public static AnimatorController CreateAnimator(VisualElement root,LSSAvatarData LSSAvatarData, bool isNewFXLayer = false)
		{
			if (lssroot == null) 
				SetLSSRoot(root);
			
			return CreateAnimator(lssroot,LSSAvatarData,isNewFXLayer);
		}
		
		public static void SetLSSRoot(VisualElement root)
		{
			lssroot = new LSSConfig();
			root.Q<ListView>("LipSyncList").Query<PopupField<string>>().ToList().Select((p,i) => (p,i)).ToList().ForEach(o =>{
				lssroot.LipSyncs[o.i].label = o.p.label;
				lssroot.LipSyncs[o.i].value = o.p.value;
			});
			lssroot.FaceMesh = root.Q<ObjectField>("FaceMesh").value as SkinnedMeshRenderer;
			lssroot.DefaultAnimator = root.Q<ObjectField>("FXLayer").value as AnimatorController;
			lssroot.NewAnimator = root.Q<ObjectField>("NewFXLayer").value as AnimatorController;
		}
		
		public static void SetLSSRoot(LipSyncSetterMonoBehavior root)
		{
			lssroot = new LSSConfig();
			lssroot.LipSyncs = root.LipSyncs;
			lssroot.FaceMesh = root.FaceMesh;
			lssroot.DefaultAnimator = root?.Root?.DefaultAnimator;
			lssroot.NewAnimator = root?.Root?.NewAnimator;
		}
		
		public static AnimatorController CreateAnimator(LipSyncSetterMonoBehavior root,LSSAvatarData LSSAvatarData)
		{
			if (cliplist.Count() == 0) return null;
			var sampleanimator = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(SampleAnimatorGUID));
			var clip = sampleanimator.layers[0].stateMachine.states.ToList();
			var popups = root.LipSyncs;
			List<string> texts = popups.Select(p => p.label).ToList();
			
			clip.Select(c =>(c,texts.IndexOf(c.state.name))).Where(i=> i.Item2 >= 0)
				.ToList().ForEach(i => i.c.state.motion = cliplist[i.Item2]);
			
			var baseLayer = LSSAvatarData.AvatarDescriptor.baseAnimationLayers;
			ScriptableObject.DestroyImmediate(lssroot);
			
			var index = Array.IndexOf(baseLayer, baseLayer.Single(l => l.type == VRCAvatarDescriptor.AnimLayerType.FX));
			var animator = baseLayer[index].animatorController as AnimatorController;
		
			var temp_animator = new AnimatorController(){name = "FX_generate_by_LSS"};
			temp_animator.parameters = sampleanimator.parameters.Select(p => p).ToArray();
			temp_animator.layers = sampleanimator.layers.Select(l => l).ToArray();
			
			if (animator) 
			{
				animator.parameters.Where(p => !temp_animator.parameters.Contains(p))
					.ToList().ForEach(p => temp_animator.AddParameter(p));
				animator.layers.Select((l,index) => (l,index))
					.ToList().ForEach(i => temp_animator.AddLayer(i.l));
			}

			baseLayer[index].isDefault = false;
			baseLayer[index].animatorController = temp_animator;
			
			return temp_animator;
		}
	}
}