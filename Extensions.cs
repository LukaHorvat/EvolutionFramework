using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public static class Extensions
	{
		public static IEnumerable<T> AllPairs<T, T1>(this IEnumerable<T1> list, Func<T1, T1, T> func)
		{
			var newList = new List<T>();
			foreach (var first in list)
			{
				foreach (var second in list)
				{
					newList.Add(func(first, second));
				}
			}
			return newList;
		}

		public static IEnumerable<Tuple<T, T>> AllPairs<T>(this IEnumerable<T> list)
		{
			var newList = new List<Tuple<T, T>>();
			foreach (var first in list)
			{
				foreach (var second in list)
				{
					newList.Add(Tuple.Create(first, second));
				}
			}
			return newList;
		}

		public static int NextBiased(this Random rand, int maxInt)
		{
			var dbl = rand.NextDouble();
			dbl *= dbl;
			if (dbl == 1) dbl = 0.99999999;
			return (int)(dbl * maxInt);
		}
	}
}
