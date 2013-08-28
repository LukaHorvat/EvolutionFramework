using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public enum CommandType : int
	{
		Pop = 0,
		Add, Sub, Mult, Div,
		Jmp, Cjmp,
		Set, Get,
		Gt, Lt, Eq,
		And, Or, Xor,
		Swp, Dup
	}

	public class Command : Gene
	{
		public static readonly int NumCommands;

		static Command()
		{
			NumCommands = Enum.GetNames(typeof(CommandType)).Length;
			FromInt = new Command[NumCommands];
			for (int i = 0; i < NumCommands; ++i)
			{
				FromInt[i] = new Command((CommandType)i);
			}
		}

		public CommandType CommandType;
		public static Dictionary<CommandType, int> ArgumentNumber = new Dictionary<CommandType, int>
		{
			{CommandType.Pop, 1},
			{CommandType.Add, 2},
			{CommandType.Sub, 2},
			{CommandType.Mult, 2},
			{CommandType.Div, 2},
			{CommandType.Jmp, 1},
			{CommandType.Cjmp, 2},
			{CommandType.Set, 2},
			{CommandType.Get, 1},
			{CommandType.Gt, 2},
			{CommandType.Lt, 2},
			{CommandType.Eq, 2},
			{CommandType.And, 2},
			{CommandType.Or, 2},
			{CommandType.Xor, 2},
			{CommandType.Swp, 2},
			{CommandType.Dup, 1}
		};
		public static Command[] FromInt;

		public Command(CommandType type)
		{
			CommandType = type;
		}

		public override string ToString()
		{
			return Enum.GetName(typeof(CommandType), CommandType);
		}
	}
}
