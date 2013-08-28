using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	class DrawingChallenge : Environment
	{
		bool[] field;
		long bestScore = 0;
		Bitmap bmp;

		public DrawingChallenge()
			: base(10000)
		{
			IllegalSyntaxRule = IllegalSyntaxBehavior.Kill;
			OutOfBoundsJumpRule = OutOfBoundsJumpBehavior.Kill;
			field = new bool[256 * 256];
		}

		public override Report OnReview()
		{
			var score = Score - (CurrentCell.Genome.Code.Count > 20 ? CurrentCell.Genome.Code.Count / 5 : 0);
			if (score > bestScore)
			{
				bestScore = score;
				Console.WriteLine("New best: " + bestScore);
				bmp = new Bitmap(256,256);
				for (int i = 0; i < 256; ++i)
				{
					for (int j = 0; j < 256; ++j)
					{
						if (field[i * 256 + j] == true)
						{
							bmp.SetPixel(i, j, Color.Black);
						}
					}
				}
				bmp.Save("image.png");
			}
			return new Report(score);
		}

		public override void OnSet(int index, int value)
		{
			base.OnSet(index, value);

			if (index == 2)
			{
				if (State[0] < 0 || State[0] >= 256 || State[1] < 0 || State[1] >= 256)
				{
					Score--;
					return;
				}
				int fieldIndex = State[0] * 256 + State[1];
				if (field[fieldIndex] == false)
				{
					field[fieldIndex] = true;
					Score++;
					CurrentCell.Ticks--;
					return;
				}
			}
		}

		public override void OnClean()
		{
			Array.Clear(field, 0, field.Length);
			State[0] = 0;
			State[1] = 0;
		}
	}
}
