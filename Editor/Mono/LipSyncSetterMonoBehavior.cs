using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using LipSyncSetter.Editor;

namespace LipSyncSetter
{
	public class LipSyncSetterMonoBehavior : MonoBehaviour,VRC.SDKBase.IEditorOnly
	{
		[SerializeField]
		LSSAvatarData _lssAvatarData = new LSSAvatarData();
		[SerializeField]
		SkinnedMeshRenderer _faceMesh;
		[SerializeField]
		List<LipSync> _lipSyncs = new List<LipSync>(VisemeNameList.Visemes.AsEnumerable().Select(v => new LipSync(){label = v}));
		[SerializeField]
		AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1f/60f, 100);
		[SerializeField]
		Editor.LSSConfig _lssRoot;

		public LSSAvatarData LSSAvatarData => _lssAvatarData;
		public SkinnedMeshRenderer FaceMesh {get => _faceMesh; set => _faceMesh = value;}
		public List<LipSync> LipSyncs {get => _lipSyncs; set => _lipSyncs = value;}
		public AnimationCurve Curve {get => _curve; set => _curve = value;}
		public Editor.LSSConfig Root {get => _lssRoot; set => _lssRoot = value;}

		[MenuItem("GameObject/Add LipSyncSetter",false,26)]
		public static void AddLSS()
		{
			new GameObject("LipSyncSetter",typeof(LipSyncSetterMonoBehavior)).transform.parent =
			Selection.activeTransform;
		}
	}
}
