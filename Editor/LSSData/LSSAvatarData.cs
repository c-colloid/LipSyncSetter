//using System;
//using System.Collections;
using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEditor;
using VRC.SDK3.Avatars.Components;

namespace LipSyncSetter
{
	public class LSSAvatarData
	{
		public VRCAvatarDescriptor AvatarDescriptor{get;set;}
		public List<string> LipSyncBlendShape{get;set;} = new List<string>(){"-none-"};
	}
}