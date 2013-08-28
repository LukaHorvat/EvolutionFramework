using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class Report
	{
		public long Score;
		public string Info;

		public Report(long score, string info = "")
		{
			Score = score;
			Info = info;
		}

		public override string ToString()
		{
			if (Info != "") return Info + ", TOTAL SCORE: " + Score;
			else return "TOTAL SCORE: " + Score;
		}
	}
}
