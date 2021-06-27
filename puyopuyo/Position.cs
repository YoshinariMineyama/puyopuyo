using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puyopuyo
{
	public class Position
	{
		public Position(int colum, int row)
		{
			Colum = colum;
			Row = row;
		}

		public int Colum
		{
			get;
			private set;
		}

		public int Row
		{
			get;
			private set;
		}
	}
}
