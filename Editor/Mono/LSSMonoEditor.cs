using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;

namespace LipSyncSetter.Editor
{
	using Editor = UnityEditor.Editor;
	[CustomEditor(typeof(LipSyncSetterMonoBehavior))]
	public class LSSMonoEditor : Editor
	{
		[SerializeField]
		VisualTreeAsset UXML;
		
		public override VisualElement CreateInspectorGUI() {
			//var baseroot = base.CreateInspectorGUI();
			
			VisualElement root = new VisualElement();
			root.Add(UXML.CloneTree());
			root.Bind(serializedObject);
			
			LipSyncSetterMonoBehavior lss = target as LipSyncSetterMonoBehavior;
			var LipSyncBlendShape = lss.LSSAvatarData.LipSyncBlendShape;
			var AvatarDescriptor = lss.LSSAvatarData.AvatarDescriptor;
			LSSBlendShapePanel lssBlendShapePanel = new LSSBlendShapePanel(lss.LSSAvatarData);
			lssBlendShapePanel.SetLSSBlendShapePanel(root);
			
			lss.LSSAvatarData.AvatarDescriptor = lss.transform.GetComponentInParent<VRCAvatarDescriptor>() != null ? lss.transform.GetComponentInParent<VRCAvatarDescriptor>() : null;
			
			root.Q<ObjectField>("FaceMesh").bindingPath = "_faceMesh";
			
			var visemeList = root.Q<ListView>("LipSyncList");
			root.Q<ObjectField>("FaceMesh").RegisterValueChangedCallback(evt => {
				visemeList.Query<PopupField<string>>().ToList().Select((p,index) => (p,index)).ToList().ForEach(o => {
					var index = LipSyncBlendShape.IndexOf(o.index < 15 ? AvatarDescriptor.VisemeBlendShapes[o.index] : "disable");
					o.p.value = LipSyncBlendShape.ElementAt(index < 0 ? 0 : index);
				});
			});
			
			visemeList.bindItem = (VisualElement ve,int i) => {
				
				ve.Q<PopupField<string>>().choices = LipSyncBlendShape;
				
				ve.Q<PopupField<string>>().Q<Label>().bindingPath = $"_lipSyncs.Array.data[{i}].label";
				ve.Q<PopupField<string>>().bindingPath = $"_lipSyncs.Array.data[{i}].value";
				
				visemeList.Bind(serializedObject);
				
				//var blendshapeindex = 0;
				//if (AvatarDescriptor != null)
				//	blendshapeindex = LipSyncBlendShape.IndexOf(i < 15 ? AvatarDescriptor.VisemeBlendShapes[i] : "disable");
				//ve.Q<PopupField<string>>().index = blendshapeindex != -1 ? blendshapeindex : 0;
			};
				
			//lss.Root.DefaultAnimator = lss.LSSAvatarData.AvatarDescriptor.baseAnimationLayers[Array.IndexOf(lss.LSSAvatarData.AvatarDescriptor.baseAnimationLayers,lss.LSSAvatarData.AvatarDescriptor.baseAnimationLayers.Single(l => l.type == VRCAvatarDescriptor.AnimLayerType.FX))].animatorController as AnimatorController;
			
			
			var fold = new Foldout(){text = "dafalt",value = false ,style = {display = DisplayStyle.None}};
			InspectorElement.FillDefaultInspector(fold,serializedObject,this);
			root.Add(fold);
			
			return root;
		}
	}
}