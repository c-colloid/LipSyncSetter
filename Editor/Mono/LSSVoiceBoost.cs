using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace LipSyncSetter
{
	public class LSSVoiceBoost : MonoBehaviour, VRC.SDKBase.IEditorOnly
	{
		[SerializeField]
		VRCExpressionsMenu _installTargetMenu;

		public VRCExpressionsMenu InstallTargetMenu => _installTargetMenu;

		[MenuItem("GameObject/Add LSSVoiceBoost", false, 27)]
		public static void AddLSSVoiceBoost()
		{
			new GameObject("LSSVoiceBoost", typeof(LSSVoiceBoost)).transform.parent =
			Selection.activeTransform;
		}
	}
}
