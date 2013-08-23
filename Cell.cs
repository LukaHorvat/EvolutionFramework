using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class Cell
	{
		public Genome Genome;
		public int Ticks;

		public Cell(Genome genome)
		{
			Genome = genome;
		}

		public void Cycle(Enviroment env)
		{
			Ticks = 0;
			int index = 0;
			var stack = new Stack<int>();
			while (index < Genome.Code.Count && Ticks < env.Lifetime)
			{
				if (index < 0) index = 0;
				if (Genome.Code[index] is Value)
				{
					stack.Push(((Value)Genome.Code[index]).Number);
				}
				else
				{
					var command = (Command)Genome.Code[index];
					int first, second;
					if (stack.Count >= Command.ArgumentNumber[command.CommandType])
					{
						switch (command.CommandType)
						{
							case CommandType.Pop:
								stack.Pop();
								break;
							case CommandType.Add:
								stack.Push(stack.Pop() + stack.Pop());
								break;
							case CommandType.Sub:
								second = stack.Pop();
								stack.Push(stack.Pop() - second);
								break;
							case CommandType.Mult:
								stack.Push(stack.Pop() * stack.Pop());
								break;
							case CommandType.Div:
								second = stack.Pop();
								if (second == 0)
								{
									stack.Push(0);
									break;
								}
								stack.Push(stack.Pop() / second);
								break;
							case CommandType.Jmp:
								index = stack.Pop() - 1;
								break;
							case CommandType.Cjmp:
								second = stack.Pop();
								first = stack.Pop();
								if (first > 0) index = second - 1;
								break;
							case CommandType.Set:
								second = stack.Pop();
								env[stack.Pop()] = second;
								break;
							case CommandType.Get:
								stack.Push(env[stack.Pop()]);
								break;
							case CommandType.Gt:
								second = stack.Pop();
								if (stack.Pop() > second) stack.Push(1);
								else stack.Push(0);
								break;
							case CommandType.Lt:
								second = stack.Pop();
								if (stack.Pop() < second) stack.Push(1);
								else stack.Push(0);
								break;
							case CommandType.Eq:
								second = stack.Pop();
								if (stack.Pop() == second) stack.Push(1);
								else stack.Push(0);
								break;
							case CommandType.And:
								second = stack.Pop();
								if (stack.Pop() * second != 0) stack.Push(1);
								else stack.Push(0);
								break;
							case CommandType.Or:
								second = stack.Pop();
								if (stack.Pop() != 0 || second != 0) stack.Push(1);
								else stack.Push(0);
								break;
							case CommandType.Xor:
								second = stack.Pop();
								first = stack.Pop();
								if ((first != 0 && second == 0) || (first == 0 && second != 0)) stack.Push(1);
								else stack.Push(0);
								break;
							case CommandType.Swp:
								second = stack.Pop();
								first = stack.Pop();
								stack.Push(second);
								stack.Push(first);
								break;
							case CommandType.Dup:
								stack.Push(stack.Peek());
								break;
						}
					}
				}
				index++;
				Ticks++;
			}
		}

		public Cell Mutate(int severity, Random rand)
		{
			var genome = Genome.Clone();
			for (int i = 0; i < severity; ++i)
			{
				switch (rand.Next(4))
				{
					case 0:
						//This mutation doesn't do anything
						break;
					case 1:
						//This mutation removes a random gene
						if (genome.Code.Count == 0) break;
						genome.Code.RemoveAt(rand.Next(genome.Code.Count));
						break;
					case 2:
						//This mutation adds a new random gene at a random location in the genome
						if (rand.Next(2) == 0)
						{
							//Add a value gene
							genome.Code.Insert(rand.Next(genome.Code.Count), new Value(0));
						}
						else
						{
							//Add a command gene
							genome.Code.Insert(rand.Next(genome.Code.Count), new Command((CommandType)rand.Next(Enum.GetNames(typeof(CommandType)).Length)));
						}
						break;
					case 3:
						//This mutation increments or decrements a value, or switches a command for some other command
						if (genome.Code.Count == 0) break;
						int index = rand.Next(genome.Code.Count);
						if (genome.Code[index] is Value)
						{
							if (rand.Next(2) == 0) ((Value)genome.Code[index]).Number++;
							else ((Value)genome.Code[index]).Number--;
						}
						else
						{
							genome.Code[index] = new Command((CommandType)rand.Next(Enum.GetNames(typeof(CommandType)).Length));
						}
						break;
				}
			}
			return new Cell(genome);
		}

		public Cell Sex(Cell mate, Random rand)
		{
			var genome = Genome.FromGenes(
				Genome.Code.Take(rand.Next(Genome.Code.Count))
				.Concat(
				mate.Genome.Code.Skip(rand.Next(mate.Genome.Code.Count)))
				.ToList());
			return new Cell(genome);

		}
	}
}
