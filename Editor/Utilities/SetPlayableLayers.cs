using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;

namespace LipSyncSetter.Editor.Utilities
{
	public static class SetPlayableLayers
	{
		static int fxIndex;
		
		public static void SetPlayableToCustom(VRCAvatarDescriptor descriptor)
		{
			descriptor.customizeAnimationLayers = true;
		}
		
		public static void SetFXToCustom(VRCAvatarDescriptor descriptor)
		{
			fxIndex = descriptor.baseAnimationLayers.ToList().FindIndex(l => l.type == VRCAvatarDescriptor.AnimLayerType.FX);
			descriptor.baseAnimationLayers[fxIndex].isDefault = false;
		}
		
		public static void SetFXToCustom(VRCAvatarDescriptor descriptor, AnimatorController animator)
		{
			SetFXToCustom(descriptor);
			descriptor.baseAnimationLayers[fxIndex].animatorController = animator;
		}
	}
}