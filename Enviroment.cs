using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class Enviroment
	{
		private Dictionary<int, int> dict;
		public event Action<int, int> Access;
		public event Action<int, int> Update;
		public int Lifetime;

		public Func<Cell, Report> ScoringFunction;
		private Action resetFunction;

		public Enviroment(Action resetFunction, Func<Cell, Report> scoringFunction, int lifetime)
		{
			dict = new Dictionary<int, int>();
			this.ScoringFunction = scoringFunction;
			this.resetFunction = resetFunction;
			Lifetime = lifetime;
		}

		public int this[int index]
		{
			get
			{
				if (!dict.ContainsKey(index)) dict[index] = 0;
				if (Access != null) Access(index, dict[index]);
				return dict[index];
			}
			set
			{
				if (Update != null) Update(index, value);
				dict[index] = value;
			}
		}

		public void Clean()
		{
			dict.Clear();
			resetFunction();
		}
	}
}
