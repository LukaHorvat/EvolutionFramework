using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public class Cell
	{
		public Genome Genome;
		public int Ticks;

		public bool Dead { get; private set; }

		public Cell(Genome genome)
		{
			Genome = genome;
		}

		private void WriteStack(Stack<int> stack, StringBuilder builder)
		{
			builder.Append("[ ");
			foreach (var n in stack.Reverse()) builder.Append(n + " ");
			builder.Append("]");
		}

		public void Cycle(Environment env, bool log = false)
		{
			var builder = new StringBuilder();
			Ticks = 0;
			int index = 0;
			var stack = new Stack<int>();
			while (index < Genome.Code.Count && Ticks < env.MaxLifetime)
			{
				if (Dead) return;

				if (index < 0 || index >= Genome.Code.Count)
				{
					if (env.OutOfBoundsJumpRule == Environment.OutOfBoundsJumpBehavior.SetToZero)
					{
						index = 0;
					}
					else if (env.OutOfBoundsJumpRule == Environment.OutOfBoundsJumpBehavior.Wrap)
					{
						index = index % Genome.Code.Count;
						if (index < 0) index += Genome.Code.Count;
					}
					else if (env.OutOfBoundsJumpRule == Environment.OutOfBoundsJumpBehavior.Kill)
					{
						Kill();
						return;
					}
				}
				if (log)
				{
					builder.Append("<" + index + ">\t");
					if (index < 10) builder.Append('\t');
					WriteStack(stack, builder);
				}
				if (Genome.Code[index] is Value)
				{
					if (log) builder.Append(" PUSH " + ((Value)Genome.Code[index]).Number);
					stack.Push(((Value)Genome.Code[index]).Number);
				}
				else
				{
					var command = (Command)Genome.Code[index];
					int first, second;
					if (stack.Count >= Command.ArgumentNumber[command.CommandType])
					{
						try
						{
							switch (command.CommandType)
							{
								case CommandType.Pop:
									if (log) builder.Append(" POP");
									stack.Pop();
									break;
								case CommandType.Add:
									if (log) builder.Append(" ADD");
									stack.Push(stack.Pop() + stack.Pop());
									break;
								case CommandType.Sub:
									if (log) builder.Append(" SUB");
									second = stack.Pop();
									stack.Push(stack.Pop() - second);
									break;
								case CommandType.Mult:
									if (log) builder.Append(" MULT");
									stack.Push(stack.Pop() * stack.Pop());
									break;
								case CommandType.Div:
									if (log) builder.Append(" DIV");
									second = stack.Pop();
									if (second == 0)
									{
										stack.Push(0);
										break;
									}
									stack.Push(stack.Pop() / second);
									break;
								case CommandType.Jmp:
									if (log) builder.Append(" JMP");
									index = stack.Pop() - 1;
									break;
								case CommandType.Cjmp:
									if (log) builder.Append(" CJMP");
									second = stack.Pop();
									first = stack.Pop();
									if (first > 0) index = second - 1;
									break;
								case CommandType.Set:
									if (log) builder.Append(" SET");
									second = stack.Pop();
									env.OnSet(stack.Pop(), second);
									break;
								case CommandType.Get:
									if (log) builder.Append(" GET");
									stack.Push(env.OnGet(stack.Pop()));
									break;
								case CommandType.Gt:
									if (log) builder.Append(" GT");
									second = stack.Pop();
									if (stack.Pop() > second) stack.Push(1);
									else stack.Push(0);
									break;
								case CommandType.Lt:
									if (log) builder.Append(" LT");
									second = stack.Pop();
									if (stack.Pop() < second) stack.Push(1);
									else stack.Push(0);
									break;
								case CommandType.Eq:
									if (log) builder.Append(" EQ");
									second = stack.Pop();
									if (stack.Pop() == second) stack.Push(1);
									else stack.Push(0);
									break;
								case CommandType.And:
									if (log) builder.Append(" AND");
									second = stack.Pop();
									if (stack.Pop() * second != 0) stack.Push(1);
									else stack.Push(0);
									break;
								case CommandType.Or:
									if (log) builder.Append(" OR");
									second = stack.Pop();
									if (stack.Pop() != 0 || second != 0) stack.Push(1);
									else stack.Push(0);
									break;
								case CommandType.Xor:
									if (log) builder.Append(" XOR");
									second = stack.Pop();
									first = stack.Pop();
									if ((first != 0 && second == 0) || (first == 0 && second != 0)) stack.Push(1);
									else stack.Push(0);
									break;
								case CommandType.Swp:
									if (log) builder.Append(" SWP");
									second = stack.Pop();
									first = stack.Pop();
									stack.Push(second);
									stack.Push(first);
									break;
								case CommandType.Dup:
									if (log) builder.Append(" DUP");
									stack.Push(stack.Peek());
									break;
							}
						}
						catch (OverflowException)
						{
							//Kill the cell
							Kill();
							return;
						}
					}
					else
					{
						if (env.IllegalSyntaxRule == Environment.IllegalSyntaxBehavior.Kill)
						{
							Kill();
							return;
						}
					}
				}
				index++;
				Ticks++;
				if (log) builder.Append('\n');
			}
			if (log) File.WriteAllText("log.txt", builder.ToString());
		}

		public Cell Mutate(int severity, Random rand)
		{
			var genome = Genome.Clone();
			for (int i = 0; i < severity; ++i)
			{
				switch (rand.Next(3))
				{
					case 0:
						//This mutation removes a random gene
						if (genome.Code.Count == 0) break;
						genome.Code.RemoveAt(rand.Next(genome.Code.Count));
						break;
					case 1:
						//This mutation adds a new random gene at a random location in the genome
						if (rand.Next(2) == 0)
						{
							//Add a value gene
							genome.Code.Insert(rand.Next(genome.Code.Count), new Value(0));
						}
						else
						{
							//Add a command gene
							genome.Code.Insert(rand.Next(genome.Code.Count), Command.FromInt[rand.Next(Command.NumCommands)]);
						}
						break;
					case 2:
						//This mutation increments or decrements a value, or switches a command for some other command
						if (genome.Code.Count == 0) break;
						int index = rand.Next(genome.Code.Count);
						if (genome.Code[index] is Value)
						{
							if (rand.Next(2) == 0) genome.Code[index] = new Value(((Value)genome.Code[index]).Number + 1);
							else genome.Code[index] = new Value(((Value)genome.Code[index]).Number - 1);
						}
						else
						{
							genome.Code[index] = Command.FromInt[rand.Next(Command.NumCommands)];
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

		public void Kill()
		{
			Dead = true;
		}
	}
}
