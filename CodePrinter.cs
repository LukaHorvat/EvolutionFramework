using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class CodePrinter
	{
		public static void PrintGenome(Genome genome)
		{
			Console.Write('\t');
			for (int i = 0; i < genome.Code.Count; ++i)
			{
				Console.Write(genome.Code[i] + " ");
				if (genome.Code[i] is Command)
				{
					Console.WriteLine();
					Console.Write('\t');
				}
			}
			Console.WriteLine();
		}
	}
}
