using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class GenePool
	{
		public List<Score> Survivors;
		public int Capacity;

		public int SavingInterval = 0;
		private int saveCounter = 0;

		public static GenePool ParseString(string code)
		{
			var genomes = code.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
			var pool = new GenePool(genomes.Length);
			foreach (var genome in genomes) pool.IntroduceSample(new Cell(Genome.ParseString(genome)));

			return pool;
		}

		public GenePool(int capacity)
		{
			Capacity = capacity;
			Survivors = new List<Score>();
			for (int i = 0; i < capacity; ++i)
			{
				Survivors.Add(new Score(new Cell(new Genome()), new Report(0)));
			}
		}

		public void IntroduceSample(Cell cell)
		{
			Survivors.Insert(0, new Score(cell, new Report(0)));
		}

		public void Generation(Environment env, Random rand)
		{
			saveCounter++;

			var cells = Survivors.Select(score => score.Competitor).ToList();
			Survivors.Clear();
			var candidates =
				cells
				.Concat(cells.Select(cell => cell.Mutate(rand.NextBiased(100), rand)))
				.Concat(cells.AllPairs((first, second) => first.Sex(second, rand).Mutate(rand.NextBiased(100), rand)));

			Survivors.AddRange(candidates.Select(cell =>
			{
				var rep = env.Test(cell);
				if (rep == null)
				{
					//Cell is dead, skip the review
					return new Score(cell, new Report(long.MinValue));
				}
				return new Score(cell, rep);
			}));

			Survivors.Sort((first, second) =>
			{
				if (first.Value.Score == long.MinValue) return 1;
				if (second.Value.Score == long.MinValue) return -1;
				var diff = second.Value.Score - first.Value.Score;
				return diff == 0 ? 0 : (diff > 0 ? 1 : -1);
			});
			Survivors = Survivors.Take(Capacity).ToList();

			if (SavingInterval != 0 && saveCounter % SavingInterval == 0)
			{
				Save("currentBest.gene");
			}
		}

		public void Save(string path)
		{
			var builder = new StringBuilder();
			foreach (var survivor in Survivors)
			{
				builder.Append("{");
				foreach (var gene in survivor.Competitor.Genome.Code)
				{
					builder.Append(gene + " ");
				}
				builder.Append("}");
			}
			File.WriteAllText(path, builder.ToString());
		}
	}
}
