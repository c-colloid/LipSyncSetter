using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace LipSyncSetter.Editor
{
	using Editor = UnityEditor.Editor;
	[CustomEditor(typeof(LSSConfig))]
	public class LSSConfigEditor : Editor
	{
		[SerializeField]
		VisualTreeAsset UXML;

		public override VisualElement CreateInspectorGUI() {
			var root = UXML.CloneTree();

			return base.CreateInspectorGUI();
		}
	}
}
