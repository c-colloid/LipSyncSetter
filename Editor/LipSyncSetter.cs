using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using VRC.SDK3.Avatars.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using LipSyncSetter.Gatosyocora.VRCAvatars3Tools.Utilitys;

/*
MIT License
Copyright (c) 2022 ｺﾛｲﾄﾞ
*/

namespace LipSyncSetter
{
	
public class LipSyncSetter : EditorWindow
{
	[SerializeField]
	VisualTreeAsset UXML;
	Editor.LSSConfig Root;
	bool isInit = false;

	LSSAvatarData _lssAvatarData = new LSSAvatarData();
	List<AnimationClip> _clip;
	[SerializeField]
	AnimatorController _animator;
	string _savefolder;
	bool _newFXLayer;
	[SerializeField]
	Object folder;
	static string folderpath;
		
	
    [MenuItem("Tools/LipSyncSetter")]
	public static void ShowWindow()
	{
		LipSyncSetter wnd = GetWindow<LipSyncSetter>();
        wnd.titleContent = new GUIContent("LipSyncSetter");
        if (AssetDatabase.GetSubFolders(folderpath).ToList<string>().IndexOf(folderpath + "/save") < 0)
	    	AssetDatabase.CreateFolder(folderpath,"save");
        
		//wnd.minSize = new Vector2(600,400);
		//wnd.position = new Rect(0,0,0,0);
	}
    
	private void Init()
	{
		rootVisualElement.Query<ObjectField>().ForEach(q => q.value = null);
		isInit = true;
		EditorApplication.delayCall += () => isInit = false;
	}

    public void CreateGUI()
	{
		folderpath = folder ? AssetDatabase.GetAssetPath(folder) : AssetDatabase.GUIDToAssetPath("379304e029d8cade50802dd4f246456a");
    	_lssAvatarData.LipSyncBlendShape = new List<string>(){"-none-"};
    	_newFXLayer = false;
    	
        // Each editor window contains a root VisualElement object
		var root = new ScrollView(); //UIのRootにScrollViewを付けないとHighlighterが使えない
		rootVisualElement.Add(root);

        // Import UXML
		var visualTree = UXML ?? AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("f2f22abe2a1b07740ae52427ca8deeb1"));
        VisualElement labelFromUXML = visualTree.CloneTree();
		labelFromUXML.style.flexGrow = 1;
        root.Add(labelFromUXML);
        
		root.Q<ObjectField>("NewFXLayer").style.display = DisplayStyle.None;
        
		LSSBlendShapePanel blendshapepanel = new LSSBlendShapePanel(_lssAvatarData);
		blendshapepanel.SetLSSBlendShapePanel(root);
		
        //Avatarのオブジェクトフィールドが変更された時
		root.Q<ObjectField>("AvatarField").RegisterValueChangedCallback(evt =>
		{
			_lssAvatarData.AvatarDescriptor = evt.newValue as VRCAvatarDescriptor;
			if (!evt.newValue) 
			{
				if (!isInit)
					Init();
				return;
			}
			
			if (_lssAvatarData.AvatarDescriptor.lipSync != VRCAvatarDescriptor.LipSyncStyle.VisemeBlendShape)
			{
				root.Q<ObjectField>("FaceMesh").value = null;
				root.Q<ObjectField>("FXLayer").value = null;
				
				var popupfields = root.Query<PopupField<string>>().ToList();
				foreach (PopupField<string> pop in popupfields)
				{
					pop.index = 0;
				}
				return;
			}
			
			root.Q<ObjectField>("FaceMesh").value = _lssAvatarData.AvatarDescriptor.VisemeSkinnedMesh;
			
			var FXAnimatorController = _lssAvatarData.AvatarDescriptor.baseAnimationLayers.SingleOrDefault(l => l.type == VRCAvatarDescriptor.AnimLayerType.FX);
			if (!FXAnimatorController.animatorController) return;
			root.Q<ObjectField>("FXLayer").value = FXAnimatorController.animatorController;
		});
		
		//FXレイヤーのオブジェクトフィールドが変更された時
		root.Q<ObjectField>("FXLayer").RegisterValueChangedCallback(evt => {
			Highlighter.Stop();
		});
		
		//FXレイヤー設定ボタンを押した時
		root.Q<Button>("FXLayerSetting").clicked += () => {
			Highlighter.Stop();
	        if (rootVisualElement.Q<ObjectField>("FaceMesh").value == null) return;
	        
        	//フォルダー作成
	    	string savefolder;
	    	if (AssetDatabase.GetSubFolders(folderpath).ToList<string>().IndexOf(folderpath + "/save") < 0)
        		AssetDatabase.CreateFolder(folderpath,"save");
	        _savefolder = folderpath + "/save";
			
	    	if (AssetDatabase.GetSubFolders(_savefolder).ToList().IndexOf(_savefolder+"/"+_lssAvatarData.AvatarDescriptor.name) < 0)
	    	{
		    	AssetDatabase.CreateFolder(_savefolder,_lssAvatarData.AvatarDescriptor.name);
	    	}
	    	
	    	_savefolder = _savefolder+"/"+_lssAvatarData.AvatarDescriptor.name;
	    	savefolder = EditorUtility.OpenFolderPanel("FXレイヤーの保存場所",_savefolder,"");
	        savefolder = FileUtil.GetProjectRelativePath(savefolder);
	    	
	    	if (_savefolder != savefolder && System.IO.Directory.GetFiles(_savefolder).Length == 0)
	    	{
	    		System.IO.Directory.Delete(_savefolder);
	    	}
	    	
	    	AssetDatabase.SaveAssets();
    		AssetDatabase.Refresh();
	    	if (string.IsNullOrEmpty(savefolder)) return;
	    	_savefolder = savefolder;
	    	
	    	//新アニメーター作成
			_animator = _animator ?? AssetDatabase.LoadAssetAtPath<AnimatorController>(folderpath + "/Resource/SmartLipSync.controller");
	    	AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(_animator),_savefolder + "/LipSyncFXLayer.controller");
	    	
	    	if (!_newFXLayer)
	    	{
	    		ObjectField newfxlayer = root.Q<ObjectField>("NewFXLayer");
	    		newfxlayer.style.display = DisplayStyle.Flex;
	    		newfxlayer.value = AssetDatabase.LoadAssetAtPath<AnimatorController>(savefolder+"/LipSyncFXLayer.controller");
	    	}
	    	else
	    	{
	    		root.Q<ObjectField>(null,null,"NewFXLayer").value = AssetDatabase.LoadAssetAtPath<AnimatorController>(savefolder+"/LipSyncFXLayer.controller");
	    	}
	    	_newFXLayer = true;
        };
        
        //リロードボタンを押した時
        root.Q<Button>("Reload").clicked += () => {
	        if(!isInit)
	        	Init();
	        root.Q<ObjectField>("AvatarField").value = _lssAvatarData?.AvatarDescriptor;
	        root.Q<ObjectField>("NewFXLayer").style.display = DisplayStyle.None;
	        _newFXLayer = false;
        };
        
        //Createボタンを押した時
        root.Q<Button>("Create").clicked += () => {
	        if (root.Q<ObjectField>("FaceMesh").value == null){
	        	root.Q<ObjectField>("FaceMesh").Focus();
		        Highlighter.Highlight(title,"m_facemesh",HighlightSearchMode.Identifier);
		        
	        	Editor.Utilities.LSSEditorUtility.DisplayDialog("FaceMesh is null",
@"FaceMeshが設定されていません"
,"OK");
		        	
	        	return;
	        }
	        
	        if (!root.Q<ObjectField>("FXLayer").value && !_newFXLayer){
		        root.Q<Button>("FXLayerSetting").Focus();
		        Highlighter.Highlight(title,"m_fxlayersetting",HighlightSearchMode.Identifier);
		        
	        	Editor.Utilities.LSSEditorUtility.DisplayDialog("FXLayer is null",
@"FXLayerにアニメーターコントローラーが割り当てられていません
新規アニメーターの作成を行ってください"
		        	,"OK");
				        
		        return;
	        }
	        
	        //フォルダー作成
	        string savefolder;
	        if (!_newFXLayer)
	        {
		        _savefolder = folderpath + "/save";
		        if (AssetDatabase.GetSubFolders(_savefolder).ToList().IndexOf(_savefolder+"/"+_lssAvatarData.AvatarDescriptor.name) < 0)
		    	{
			    	AssetDatabase.CreateFolder(_savefolder,_lssAvatarData.AvatarDescriptor.name);
		    	}
		    	_savefolder = _savefolder+"/"+_lssAvatarData.AvatarDescriptor.name;
	        }
	        
	    	savefolder = EditorUtility.OpenFolderPanel("アニメーションの保存場所",_savefolder,"");
	    	savefolder = FileUtil.GetProjectRelativePath(savefolder);
			if (_savefolder != savefolder && System.IO.Directory.GetFiles(_savefolder).Length == 0)
	    	{
	    		System.IO.Directory.Delete(_savefolder);
	    	}
	    	
	    	AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
	    	if (string.IsNullOrEmpty(savefolder)) return;
    		_savefolder = savefolder;
	    	
	    	//アニメーション・アニメーター作成
	        CreateAnime();
	        var animator = Editor.Utilities.LSSUtility.CreateAnimator(rootVisualElement,_lssAvatarData,_newFXLayer);
	        Editor.Utilities.SetPlayableLayers.SetPlayableToCustom(_lssAvatarData.AvatarDescriptor);
	        Editor.Utilities.SetPlayableLayers.SetFXToCustom(_lssAvatarData.AvatarDescriptor, animator);
	        EditorUtility.DisplayDialog("LipSyncSetter","リップシンクの作成が終了しました!","よっしゃー！");
        };
        
    }
    
    public void CreateAnime()
    {
	    Editor.Utilities.LSSUtility.CreateAnime(rootVisualElement,_lssAvatarData).Select((clip,index) => (clip,index))
		    .ToList().ForEach(c => {
			    EditorUtility.DisplayCancelableProgressBar("Create AnimetionClips",$"Create {c.clip.name}.anim",c.index/15);
			    AssetDatabase.CreateAsset(c.clip,_savefolder + "/v." + c.clip.name + ".anim");
		    });
	    EditorUtility.ClearProgressBar();
    }
}
}
