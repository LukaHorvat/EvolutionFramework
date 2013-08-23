using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class Score
	{
		public Cell Competitor;
		public Report Value;

		public Score(Cell competior, Report value)
		{
			Competitor = competior;
			Value = value;
		}
	}
}
