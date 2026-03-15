using System.Linq;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;

namespace LipSyncSetter.Editor.Utilities
{
	public static class SetPlayableLayers
	{
		public static void SetPlayableToCustom(VRCAvatarDescriptor descriptor)
		{
			descriptor.customizeAnimationLayers = true;
		}

		public static void SetFXToCustom(VRCAvatarDescriptor descriptor, AnimatorController animator = null)
		{
			var fxIndex = descriptor.baseAnimationLayers.ToList().FindIndex(l => l.type == VRCAvatarDescriptor.AnimLayerType.FX);
			descriptor.baseAnimationLayers[fxIndex].isDefault = false;
			if (animator != null)
				descriptor.baseAnimationLayers[fxIndex].animatorController = animator;
		}
	}
}
