using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace LipSyncSetter.Editor.Utilities
{
	public partial class LSSEditorUtility : EditorWindow
	{
		[SerializeField]
		VisualTreeAsset UXML;
		static string DialogMessage {get;set;}
		static string ButtonMessage {get;set;}
		static bool ButtonIsValied {get;set;} = false;
		
		public static void DisplayDialog(string title,string msg,string OK)
		{
			ButtonIsValied = true;
			ButtonMessage = OK;
			DisplayDialog(title,msg);
		}
		
		public static void DisplayDialog(string title,string msg)
		{
			var wnd = CreateInstance<LSSEditorUtility>();
			wnd.title = title;
			DialogMessage = msg;
			wnd.ShowAuxWindow();
		}
		
		private void CreateGUI()
		{
			UXML.CloneTree(rootVisualElement);
			rootVisualElement.Q<Label>("Message").text = DialogMessage;
			if (!ButtonIsValied) return;
			rootVisualElement.Q<Button>("OK").text = ButtonMessage;
			rootVisualElement.Q<Button>("OK").clicked += () => Close();
		}
	}
}