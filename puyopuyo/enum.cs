namespace Puyopuyo
{

	/// <summary>
	/// ぷよのタイプ
	/// </summary>
	public enum Puyo
	{
		None = 0,	//なにもない
		Wall = -1,	//壁

		Puyo1 = 1,	//ぷよ1
		Puyo2 = 2,  //ぷよ2
		Puyo3 = 3,  //ぷよ3
		Puyo4 = 4,  //ぷよ4
	}

	/// <summary>
	/// ぷよの回転
	/// </summary>
	public enum Angle
	{
		Angle0 = 0,
		Angle90 = 1,
		Angle180 = 2,
		Angle270 = 3,
	}
}
