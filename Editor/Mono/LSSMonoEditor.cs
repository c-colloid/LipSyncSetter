using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using VRC.SDK3.Avatars.Components;

namespace LipSyncSetter.Editor
{
	using Editor = UnityEditor.Editor;
	[CustomEditor(typeof(LipSyncSetterMonoBehavior))]
	public class LSSMonoEditor : Editor
	{
		[SerializeField]
		VisualTreeAsset UXML;

		public override VisualElement CreateInspectorGUI() {
			//var baseroot = base.CreateInspectorGUI();

			VisualElement root = new VisualElement();
			root.Add(UXML.CloneTree());

			LipSyncSetterMonoBehavior lss = target as LipSyncSetterMonoBehavior;

			// AvatarDescriptor を先に設定してからキャプチャする
			lss.LSSAvatarData.AvatarDescriptor = lss.transform.GetComponentInParent<VRCAvatarDescriptor>();

			var LipSyncBlendShape = lss.LSSAvatarData.LipSyncBlendShape;
			LSSBlendShapePanel lssBlendShapePanel = new LSSBlendShapePanel(lss.LSSAvatarData);
			lssBlendShapePanel.SetLSSBlendShapePanel(root);

			root.Q<ObjectField>("FaceMesh").bindingPath = "_faceMesh";

			var visemeList = root.Q<ListView>("LipSyncList");
			root.Q<ObjectField>("FaceMesh").RegisterValueChangedCallback(evt => {
				// コールバック時点の最新の AvatarDescriptor を参照する
				var avatarDescriptor = lss.LSSAvatarData.AvatarDescriptor;
				if (avatarDescriptor == null) return;

				visemeList.Query<PopupField<string>>().ToList().Select((p,index) => (p,index)).ToList().ForEach(o => {
					var index = LipSyncBlendShape.IndexOf(o.index < 15 ? avatarDescriptor.VisemeBlendShapes[o.index] : "disable");
					o.p.value = LipSyncBlendShape.ElementAt(index < 0 ? 0 : index);
				});
			});

			// Bind は全てのコールバック登録後に呼ぶ
			root.Bind(serializedObject);

			visemeList.bindItem = (VisualElement ve,int i) => {

				ve.Q<PopupField<string>>().choices = LipSyncBlendShape;

				ve.Q<PopupField<string>>().Q<Label>().bindingPath = $"_lipSyncs.Array.data[{i}].label";
				ve.Q<PopupField<string>>().bindingPath = $"_lipSyncs.Array.data[{i}].value";

				visemeList.Bind(serializedObject);
			};

			var fold = new Foldout(){text = "dafalt",value = false ,style = {display = DisplayStyle.None}};
			InspectorElement.FillDefaultInspector(fold,serializedObject,this);
			root.Add(fold);

			return root;
		}
	}
}
