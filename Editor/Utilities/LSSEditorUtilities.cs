using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace LipSyncSetter.Editor.Utilities
{
	public partial class LSSEditorUtility : EditorWindow
	{
		[SerializeField]
		VisualTreeAsset UXML;
		string _dialogMessage;
		string _buttonMessage;
		bool _buttonIsValid = false;

		public static void DisplayDialog(string title,string msg,string ok)
		{
			var wnd = CreateInstance<LSSEditorUtility>();
			wnd.title = title;
			wnd._dialogMessage = msg;
			wnd._buttonMessage = ok;
			wnd._buttonIsValid = true;
			wnd.ShowAuxWindow();
		}

		public static void DisplayDialog(string title,string msg)
		{
			var wnd = CreateInstance<LSSEditorUtility>();
			wnd.title = title;
			wnd._dialogMessage = msg;
			wnd.ShowAuxWindow();
		}

		private void CreateGUI()
		{
			UXML.CloneTree(rootVisualElement);
			rootVisualElement.Q<Label>("Message").text = _dialogMessage;
			if (!_buttonIsValid) return;
			rootVisualElement.Q<Button>("OK").text = _buttonMessage;
			rootVisualElement.Q<Button>("OK").clicked += () => Close();
		}
	}
}
