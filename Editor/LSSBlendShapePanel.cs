using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using VRC.SDK3.Avatars.Components;

namespace LipSyncSetter
{
	public class LSSBlendShapePanel
	{	
		LSSAvatarData _lssAvatarData;
		
		public VRCAvatarDescriptor AvatarDescriptor{get => _lssAvatarData.AvatarDescriptor;}
		public List<string> LipSyncBlendShape{get => _lssAvatarData.LipSyncBlendShape;set => _lssAvatarData.LipSyncBlendShape = value;}
		
		public LSSBlendShapePanel(LSSAvatarData lssAvatarData)
		{
			_lssAvatarData = lssAvatarData;
		}
		
		public void SetLSSBlendShapePanel(VisualElement root)
		{
			root.Q<ObjectField>("FaceMesh").objectType = typeof(SkinnedMeshRenderer);
        
			//LipSyncのリストを作成
			var visemeList = root.Q<ListView>("LipSyncList");
			visemeList.itemsSource = Editor.VisemeNameList.Visemes;
			visemeList.makeItem = () => {
				var listitem = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("9713a612841646347bdbdcce0be5ac5d")).CloneTree();
				#if UNITY_2022_3_OR_NEWER
				return listitem;
				#else
				return new PopupField<string>("Name");
				#endif
			};
			//Popupフィールドの設定
			visemeList.bindItem = (VisualElement ve,int i) => {
				var targetfield = ve.Q<PopupField<string>>();
				targetfield.Q<Label>().text = visemeList.itemsSource[i] as string;
				targetfield.choices = LipSyncBlendShape;
				var blendshapeindex = 0;
				if (AvatarDescriptor != null && AvatarDescriptor.VisemeBlendShapes.Length == 15)
					blendshapeindex = LipSyncBlendShape.IndexOf(i < 15 ? AvatarDescriptor?.VisemeBlendShapes[i] : "disable");
				targetfield.index = blendshapeindex != -1 ? blendshapeindex : 0;
				var warningcolor = Color.yellow * (EditorGUIUtility.isProSkin ? Color.gray : Color.white);
				targetfield.style.backgroundColor = blendshapeindex > 0 ? default : warningcolor;
				targetfield.RegisterValueChangedCallback(evt => 
					targetfield.style.backgroundColor = targetfield.index > 0 ? default : warningcolor);
				ve.Q<PopupField<string>>().Q<VisualElement>(null,"unity-base-popup-field__input").pickingMode = PickingMode.Position;
				//ve.Q<PopupField<string>>().Q<VisualElement>(null,"unity-base-popup-field__input")
				//	.AddManipulator(new Editor.Manipulator.ClickFieldManipulator());
			};
			
			//FaceMeshオブジェクトフィールドを変更した時
			root.Q<ObjectField>("FaceMesh").RegisterValueChangedCallback(evt =>
			{
				//if (!AvatarDescriptor) return;
				var mesh = evt.newValue as SkinnedMeshRenderer;
				var textlist = root.Query<VisualElement>(null,null,"myfield").ToList();
				LipSyncBlendShape.Clear();
				LipSyncBlendShape.Add("-none-");
				LipSyncBlendShape.Add("disable");
				if (!mesh) {
					root.Q<ListView>("LipSyncList").Rebuild(); 
					return;}
				int i = 0;
			
				var target = mesh.sharedMesh;
				
				for (int j = 0; j < target.blendShapeCount ; j++) {
					LipSyncBlendShape.Add(target.GetBlendShapeName(j));
				}
			
				//Generate ListViewItem
				//foreach (var item in _avatrDescriptor.VisemeBlendShapes)
				//{
				//	//Debug.Log(item);
				//	if (i < textlist.Count)
				//	{
				//		var oldfield = textlist[i];
				//		var blendshapeindex = 0;
				//		if (_avatrDescriptor.VisemeSkinnedMesh == mesh)
				//			blendshapeindex = LipSyncBlendShape.IndexOf(_avatrDescriptor.VisemeBlendShapes[i]);
				//		var newfield = new PopupField<string>(oldfield.Query<Label>().First().text,LipSyncBlendShape,blendshapeindex);
				//		newfield.AddToClassList("myfield");
				//		root.Q<ListView>("LipSyncList").Remove(oldfield);
				//		root.Q<ListView>("LipSyncList").Add(newfield);
				//	}
				//	i++;
				//}
				//root.Q<ListView>("LipSyncList").Remove(textlist.Last());
				//var field = new PopupField<string>("Disable",LipSyncBlendShape,"disable");
				//field.AddToClassList("myfield");
				//root.Q<ListView>("LipSyncList").Add(field);
				root.Q<ListView>("LipSyncList").Rebuild();
			});
		}
	}
}