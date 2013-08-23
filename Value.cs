using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class Value : Gene
	{
		public int Number;

		public Value(int number)
		{
			Number = number;
		}

		public override string ToString()
		{
			return Number.ToString();
		}
	}
}
