using System.Collections.Generic;

namespace LipSyncSetter.Editor
{
	public class VisemeNameList
	{
		static readonly List<string> _visemes = new List<string>
			{
			"sil",
			"PP",
			"FF",
			"TH",
			"DD",
			"kk",
			"CH",
			"SS",
			"nn",
			"RR",
			"aa",
			"E",
			"ih",
			"oh",
			"ou",
			"Disable"
			};

		public static List<string> Visemes => _visemes;
	}
}
