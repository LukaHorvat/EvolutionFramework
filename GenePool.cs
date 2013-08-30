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
				cells //Add current survivors,
				.Concat(cells.Select(cell => cell.Mutate(rand.NextBiased(100) + 1, rand))) //then mutated survivors,
				.Concat(cells.AllPairs((first, second) => first.Sex(second, rand))); //then breed each survivor with each other and add those

			Survivors.AddRange(candidates
				.Select(env.Test)
				.Where(score => score != null));

			Survivors.Sort((first, second) =>
			{
				long x = first.Value.Score;
				long y = second.Value.Score;
				return x == y ? 0 : (y > x ? 1 : -1);
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
