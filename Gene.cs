using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public abstract class Gene
	{
		public static Gene FromString(string code)
		{
			int val;
			if (!int.TryParse(code, out val))
			{
				return new Command((CommandType)Enum.Parse(typeof(CommandType), code));
			}
			else
			{
				return new Value(val);
			}
		}
	}
}
