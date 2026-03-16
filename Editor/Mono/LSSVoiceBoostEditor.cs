using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace LipSyncSetter.Editor
{
	using Editor = UnityEditor.Editor;
	[CustomEditor(typeof(LSSVoiceBoost))]
	public class LSSVoiceBoostEditor : Editor
	{
		[SerializeField]
		VisualTreeAsset UXML;

		public override VisualElement CreateInspectorGUI()
		{
			var root = UXML.CloneTree();
			root.Bind(serializedObject);
			return root;
		}
	}
}
