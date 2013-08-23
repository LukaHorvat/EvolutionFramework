using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class GenePool
	{
		public List<Score> Survivors;
		public int Capacity;

		public GenePool(int capacity)
		{
			Capacity = capacity;
			Survivors = new List<Score>();
			for (int i = 0; i < capacity; ++i)
			{
				Survivors.Add(new Score(new Cell(new Genome()), new Report(0)));
			}
		}

		public void Generation(Enviroment env, Random rand)
		{
			var cells = Survivors.Select(score => score.Competitor).ToList();
			for (int i = 0; i < cells.Count; ++i)
			{
				for (int j = 0; j < cells.Count; ++j)
				{
					var contender = cells[i].Sex(cells[j], rand).Mutate(10, rand);
					env.Clean();
					contender.Cycle(env);
					Survivors.Add(new Score(contender, env.ScoringFunction(contender)));
				}
			}
			Survivors.Sort((first, second) => second.Value.Score - first.Value.Score);
			Survivors = Survivors.Take(Capacity).ToList();
		}
	}
}
