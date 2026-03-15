using UnityEngine;
using UnityEngine.UIElements;

namespace LipSyncSetter.Editor.Manipulator
{
	public class ClickFieldManipulator : Manipulator
	{
		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerDownEvent>(OnPointerUp);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerDownEvent>(OnPointerUp);
		}

		private void OnPointerUp(PointerDownEvent evt)
		{
			Debug.Log("click");
			evt.StopPropagation();
		}
	}
}
