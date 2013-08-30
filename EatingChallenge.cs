using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionFramework
{
	class EatingChallenge : Environment
	{
		bool[] preset;
		bool[] field;
		int x;
		int y;
		bool[] path;
		Bitmap bmp;

		public EatingChallenge()
			: base(1000)
		{
			IllegalSyntaxRule = IllegalSyntaxBehavior.Kill;
			OutOfBoundsJumpRule = OutOfBoundsJumpBehavior.Kill;
			preset = new bool[256 * 256];
			field = new bool[256 * 256];
			path = new bool[256 * 256];

			var rand = new Random();
			for (int i = 0; i < 500; ++i)
			{
				preset[rand.Next(256) * 256 + rand.Next(256)] = true;
			}
		}

		public override void OnSet(int index, int value)
		{
			base.OnSet(index, value);
			if (index == 0) x--;
			if (index == 1) y--;
			if (index == 2) x++;
			if (index == 3) y++;
			if (x < 0 || x >= 256 || y < 0 || y >= 256) return;

			path[x * 256 + y] = true;
			if (field[x * 256 + y] == true)
			{
				field[x * 256 + y] = false;
				Score += 10;
				CurrentCell.Ticks -= 10;
			}
		}

		public override void OnClean()
		{
			base.OnClean();
			x = 128;
			y = 128;
			Array.Copy(preset, field, preset.Length);
			Array.Clear(path, 0, path.Length);
		}

		long bestScore = 0;
		public override Report OnReview()
		{
			var score = Score - (CurrentCell.Genome.Code.Count > 10 ? CurrentCell.Genome.Code.Count : 0);
			if (score > bestScore)
			{
				bestScore = score;
				Console.WriteLine("New best: " + bestScore);
				bmp = new Bitmap(256, 256);
				for (int i = 0; i < 256; ++i)
				{
					for (int j = 0; j < 256; ++j)
					{
						if (path[i * 256 + j] == true)
						{
							bmp.SetPixel(i, j, Color.Black);
						}
					}
				}
				bmp.Save("image.png");
			}
			return new Report(score);
		}
	}
}
