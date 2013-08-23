using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class Report
	{
		public int Score;
		public string Info;

		public Report(int score, string info = "")
		{
			Score = score;
			Info = info;
		}
	}
}
