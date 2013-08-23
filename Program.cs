using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	class Program
	{
		static int score;
		static void Main(string[] args)
		{
			var env = new Enviroment(() => score = 0, cell => new Report(score * 10 - cell.Genome.Code.Count - cell.Ticks, "Before reduction: " + score), 1000);
			env.Update += (index, value) =>
			{
				if (index == 0) score++;
			};
			var random = new Random();
			var pool = new GenePool(10);
			for (int i = 0; i < 5000; ++i)
			{
				pool.Generation(env, random);
			}
			Console.WriteLine("Score: ");
			foreach (var score in pool.Survivors)
			{
				Console.WriteLine(score.Competitor + ": " + score.Value);
			}
			CodePrinter.PrintGenome(pool.Survivors.First().Competitor.Genome);

			Console.ReadKey();
		}
	}
}
