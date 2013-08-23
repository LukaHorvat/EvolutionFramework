using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class Genome
	{
		public List<Gene> Code;

		public Genome()
		{
			Code = new List<Gene>();
		}

		public static Genome ParseString(string code)
		{
			var genome = new Genome();
			genome.Code = code
				.Split(' ')
				.Select(str => Gene.FromString(str))
				.ToList();
			return genome;
		}

		public static Genome FromGenes(List<Gene> code)
		{
			return new Genome
			{
				Code = new List<Gene>(code)
			};
		}

		public Genome Clone()
		{
			return Genome.FromGenes(Code);
		}
	}
}
