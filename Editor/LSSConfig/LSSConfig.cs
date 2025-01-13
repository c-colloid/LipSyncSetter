using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace LipSyncSetter.Editor
{
	//[CreateAssetMenu(menuName = "LSS/LSSConfig", fileName = "LSSConfig")]
	public class LSSConfig : ScriptableObject
	{
		public SkinnedMeshRenderer FaceMesh;
		public List<LipSync> LipSyncs = new List<LipSync>(VisemeNameList.Visemes.AsEnumerable().Select(v => new LipSync(){label = v}));
		public AnimatorController DefaultAnimator;
		public AnimatorController NewAnimator;
		
		public void OnDestroy()
		{
			FaceMesh = null;
			LipSyncs = null;
			DefaultAnimator = null;
			NewAnimator = null;
		}
	}
}