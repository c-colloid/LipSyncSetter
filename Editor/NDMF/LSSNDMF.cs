//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using nadena.dev.ndmf;
using LipSyncSetter.Editor.Utilities;

[assembly: ExportsPlugin(typeof(LipSyncSetter.NDMF.LSSNDMF))]
namespace LipSyncSetter.NDMF
{
	public class LSSNDMF : Plugin<LSSNDMF>
	{
		protected override void Configure()
		{
			InPhase(BuildPhase.Generating).Run("Generate LipSync",ctx =>{
				var target = ctx?.AvatarRootObject?.GetComponentInChildren<LipSyncSetterMonoBehavior>();
				if (target == null) return;
				target.LSSAvatarData.AvatarDescriptor = ctx.AvatarDescriptor;
				LSSUtility.CreateAnime(target,target.LSSAvatarData);
				LSSUtility.CreateAnimator(target,target.LSSAvatarData);
				SetPlayableLayers.SetPlayableToCustom(ctx.AvatarDescriptor);
				SetPlayableLayers.SetFXToCustom(ctx.AvatarDescriptor);
			});
			
			InPhase(BuildPhase.Generating).Run("Remove Component", ctx => {
				Object.DestroyImmediate(ctx.AvatarRootTransform.GetComponentInChildren<LipSyncSetterMonoBehavior>()?.gameObject);
			});
		}
	}
}