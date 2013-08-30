using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	class Program
	{
		static EatingChallenge env;

		static void Main(string[] args)
		{
			env = new EatingChallenge();

			var random = new Random();
			var pool = new GenePool(10)
			{
				SavingInterval = 500
			};

			int generation = 0;
			while (true)
			{
				pool.Generation(env, random);
				generation++;
				if (generation % 100 == 0) Console.WriteLine(generation);
			}
		}
	}
}
