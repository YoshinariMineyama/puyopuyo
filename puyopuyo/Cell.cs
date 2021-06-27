using System.Drawing;

namespace Puyopuyo
{

	/// <summary>
	/// ぷよの位置設定
	/// </summary>
	public class Cell
	{
		public Cell(int x, int y)
		{
			LeftTop = new Point(x, y);
		}

		public Point LeftTop
		{
			get;
			private set;
		} = new Point(0, 0);

		static public Size Size
		{
			get;
		} = new Size(30, 30);

		public void SetColumRow(int colum, int row)
		{
			Colum = colum;
			Row = row;
		}

		public int Colum
		{
			get;
			private set;
		} = 0;

		public int Row
		{
			get;
			private set;
		} = 0;


		/// <summary>
		/// ぷよ同士の当たり判定用
		/// </summary>
		public bool IsFixed
		{
			get;
			set;
		} = false;

		public Puyo Puyo
		{
			get;
			set;
		} = Puyo.None;

		public bool isChecked
		{
			get;
			set;
		}
		= false;
	}
}

