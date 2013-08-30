using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	public abstract class Environment
	{
		public enum OutOfBoundsJumpBehavior
		{
			Kill,
			SetToZero,
			Wrap
		}

		public enum IllegalSyntaxBehavior
		{
			Kill,
			Ignore
		}

		public enum OutOfMemoryBehavior
		{
			Kill,
			Ignore
		}

		public int MaxLifetime;
		public long Score;

		//Options handled by the cell
		public OutOfBoundsJumpBehavior OutOfBoundsJumpRule = OutOfBoundsJumpBehavior.SetToZero;
		public IllegalSyntaxBehavior IllegalSyntaxRule = IllegalSyntaxBehavior.Ignore;

		//Options handled by the environment
		public OutOfMemoryBehavior OutOfMemoryRule = OutOfMemoryBehavior.Kill;

		protected Cell CurrentCell;
		protected Dictionary<int, int> State;
		protected int MaxSetFields = 1000;

		/// <summary>
		/// Initializes the environment with set max life duration
		/// </summary>
		/// <param name="maxLifetime">Maximum number of ticks before the cell dies.</param>
		protected Environment(int maxLifetime)
		{
			State = new Dictionary<int, int>();
			MaxLifetime = maxLifetime;
		}

		protected void Clean()
		{
			State.Clear();
			Score = 0;
			OnClean();
		}

		public virtual Report OnReview()
		{
			return new Report(Score);
		}

		public virtual void OnClean()
		{

		}
		public virtual void OnSet(int index, int value)
		{
			State[index] = value;
			if (State.Count > MaxSetFields)
			{
				if (OutOfMemoryRule == OutOfMemoryBehavior.Kill) CurrentCell.Kill();
			}
		}

		public virtual int OnGet(int index)
		{
			int ret;
			if (State.TryGetValue(index, out ret)) return ret;
			return 0;
		}

		/// <summary>
		/// Tests the cell in this environment. Returns a report with its score or null if the cell died.
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		public Score Test(Cell cell)
		{
			Clean();
			CurrentCell = cell;
			cell.Cycle(this);
			if (cell.Dead) return null;
			return new Score(cell, OnReview());
		}
	}
}
