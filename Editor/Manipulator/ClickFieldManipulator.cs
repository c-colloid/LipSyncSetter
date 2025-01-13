using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace LipSyncSetter.Editor.Manipulator
{
	using UnityEngine.UIElements;
	using UnityEditor.UIElements;
	public class ClickFieldManipulator : Manipulator
	{
		protected override void RegisterCallbacksOnTarget()
		{
			// 各種ポインターイベントを登録
			target.RegisterCallback<PointerDownEvent>(OnPointerUp);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerDownEvent>(OnPointerUp);
		}
		
		private void OnPointerUp(PointerDownEvent evt)
		{
			Debug.Log("clike");
			evt.StopPropagation();
		}
	}
}