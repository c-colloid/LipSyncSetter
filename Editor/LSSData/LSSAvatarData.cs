using System.Collections.Generic;
using VRC.SDK3.Avatars.Components;

namespace LipSyncSetter
{
	public class LSSAvatarData
	{
		public VRCAvatarDescriptor AvatarDescriptor{get;set;}
		public List<string> LipSyncBlendShape{get;set;} = new List<string>(){"-none-"};
	}
}
