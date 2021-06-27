/*---------------------------------------------*/
/* 作成日　2021/6/10(木)～6/25(金)
-----------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puyopuyo
{
    /// <summary>
    /// 初期設定
    /// </summary>
    public partial class Form1 : Form
    {
        //セルの配列
        Cell[,] Cells = null;

        //フィールドの幅と高さ
        int FIELD_WIDTH = 6 + 2;
        int FIELD_HEIGHT = 12 + 1 + 1;

        // ぷよの回転軸になる側の位置
        int puyoPositionX = puyoStartPositionX;
        int puyoPositionY = puyoStartPositionY;

        // ぷよが最初にあらわれる部分の位置
        const int puyoStartPositionX = 3;
        const int puyoStartPositionY = 1;

        //ぷよはどれだけ回転したか？
        Angle PuyoAngle = Angle.Angle0;

        Puyo DropingPuyo = Puyo.None;
        Puyo SubDropingPuyo = Puyo.None;

        Image redPuyoImage = null;
        Image bluePuyoImage = null;
        Image greenPuyoImage = null;
        Image yellowPuyoImage = null;
        Image wallImage = null;

        List<Image> redImages = new List<Image>();
        List<Image> greenImages = new List<Image>();
        List<Image> blueImages = new List<Image>();
        List<Image> yellowImages = new List<Image>();

        Random Random = new Random();
        bool isIgnoreKeyDown = false;


        /// <summary>
        /// Formの設定
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Field.Paint += Field_Paint;
            Field.KeyDown += Field_KeyDown;

            this.BackColor = Color.Black;
            Field.BorderStyle = BorderStyle.None;

            label1.ForeColor = Color.White;
            label1.Font = new Font("ＭＳ ゴシック", 12, FontStyle.Bold);

            Score = 0;

            GetPuyoImages();
            InitCells();
        }

        /// <summary>
        /// セルの作成
        /// </summary>
        void InitCells()
        {
            Cells = new Cell[FIELD_HEIGHT, FIELD_WIDTH];

            int selWidth = Cell.Size.Width;
            int selHeight = Cell.Size.Height;

            for (int i = 0; i < FIELD_HEIGHT; i++)
            {
                for (int j = 0; j < FIELD_WIDTH; j++)
                {
                    // (i - 1)とすることで一番上の行はコントロールの外側になり表示されない
                    Cells[i, j] = new Cell(j * selWidth, (i - 1) * selHeight);
                    Cells[i, j].SetColumRow(j, i);
                }
            }

            // 外枠を表示させる
            for (int i = 0; i < FIELD_HEIGHT; i++)
                SetPuyo(0, i, Puyo.Wall);
            for (int i = 0; i < FIELD_HEIGHT; i++)
                SetPuyo(FIELD_WIDTH - 1, i, Puyo.Wall);
            for (int i = 0; i < FIELD_WIDTH; i++)
                SetPuyo(i, FIELD_HEIGHT - 1, Puyo.Wall);
        }

        /// <summary>
        /// 移動キー設定
        /// </summary>
        private void Field_KeyDown(object sender, KeyEventArgs e)
        {
            if (isIgnoreKeyDown)
                return;

            if (e.KeyCode == Keys.Left)
                PuyoMoveLeftCheck();
            if (e.KeyCode == Keys.Right)
                PuyoMoveRightCheck();
            if (e.KeyCode == Keys.Down)
                PuyoMoveDownCheck();
            if (e.KeyCode == Keys.Space)
                PuyoRotationCheck();
        }

        /// <summary>
        ///  ぷよが回転可能か判別
        /// </summary>
        void PuyoRotationCheck()
        {
            Position oldSubPos = GetSubPuyoPosition(puyoPositionX, puyoPositionY, PuyoAngle);

            Angle angle = Angle.Angle0;
            if (PuyoAngle == Angle.Angle0)
                angle = Angle.Angle90;
            else if (PuyoAngle == Angle.Angle90)
                angle = Angle.Angle180;
            else if (PuyoAngle == Angle.Angle180)
                angle = Angle.Angle270;
            else if (PuyoAngle == Angle.Angle270)
                angle = Angle.Angle0;

            Position newSubPos = GetSubPuyoPosition(puyoPositionX, puyoPositionY, angle);
            if (CanMove(newSubPos))
            {
                // 回転角度をフィールド変数にセット
                PuyoAngle = angle;

                // もとの位置のセルは「ぷよなし」の状態にして、新しい位置にぷよをセットする
                SetPuyo(oldSubPos.Colum, oldSubPos.Row, Puyo.None);
                SetPuyo(newSubPos.Colum, newSubPos.Row, SubDropingPuyo);
            }
        }

        private void Field_Paint(object sender, PaintEventArgs e)
        {
            DrawPuyos(e.Graphics);
        }

        /// <summary>
        ///  回転軸ではないぷよの位置を取得する
        /// </summary>
        /// <param name="mainColun">列</param>
        /// <param name="mainRow">行</param>
        /// <param name="angle">回転</param>
        /// <returns></returns>
        Position GetSubPuyoPosition(int mainColun, int mainRow, Angle angle)
        {
            if (angle == Angle.Angle0)
                return new Position(mainColun, mainRow - 1);
            if (angle == Angle.Angle90)
                return new Position(mainColun + 1, mainRow);
            if (angle == Angle.Angle180)
                return new Position(mainColun, mainRow + 1);
            if (angle == Angle.Angle270)
                return new Position(mainColun - 1, mainRow);

            return null;
        }

        /// <summary>
        /// ぷよの描画
        /// </summary>
        /// <param name="g">ぷよの画像</param>
        void DrawPuyos(Graphics g)
        {
            Field.BackColor = Color.Black;
            for (int i = 0; i < FIELD_HEIGHT; i++)
            {
                for (int j = 0; j < FIELD_WIDTH; j++)
                {
                    Cell cell = Cells[i, j];
                    Rectangle rect = new Rectangle(cell.LeftTop, Cell.Size);

                    Image image = GetImageFromPuyoType(cell.Puyo);
                    if (image != null)
                        g.DrawImage(image, rect);
                }
            }
        }


        /// <summary>
        /// ぷよの連鎖回数
        /// </summary>
        void GetPuyoImages()
        {
            redPuyoImage = Properties.Resources.Red;
            bluePuyoImage = Properties.Resources.Blue;
            greenPuyoImage = Properties.Resources.Green;
            yellowPuyoImage = Properties.Resources.Yellow;
            wallImage = Properties.Resources.Wall;

            redImages.Add(Properties.Resources.Red01);
            redImages.Add(Properties.Resources.Red02);
            redImages.Add(Properties.Resources.Red03);
            redImages.Add(Properties.Resources.Red04);
            redImages.Add(Properties.Resources.Red05);
            redImages.Add(Properties.Resources.Red06);
            redImages.Add(Properties.Resources.Red07);
            redImages.Add(Properties.Resources.Red08);
            redImages.Add(Properties.Resources.Red09);
            redImages.Add(Properties.Resources.Red10);
           
            greenImages.Add(Properties.Resources.Green01);
            greenImages.Add(Properties.Resources.Green02);
            greenImages.Add(Properties.Resources.Green03);
            greenImages.Add(Properties.Resources.Green04);
            greenImages.Add(Properties.Resources.Green05);
            greenImages.Add(Properties.Resources.Green06);
            greenImages.Add(Properties.Resources.Green07);
            greenImages.Add(Properties.Resources.Green08);
            greenImages.Add(Properties.Resources.Green09);
            greenImages.Add(Properties.Resources.Green10);

            blueImages.Add(Properties.Resources.Blue01);
            blueImages.Add(Properties.Resources.Blue02);
            blueImages.Add(Properties.Resources.Blue03);
            blueImages.Add(Properties.Resources.Blue04);
            blueImages.Add(Properties.Resources.Blue05);
            blueImages.Add(Properties.Resources.Blue06);
            blueImages.Add(Properties.Resources.Blue07);
            blueImages.Add(Properties.Resources.Blue08);
            blueImages.Add(Properties.Resources.Blue09);
            blueImages.Add(Properties.Resources.Blue10);

            yellowImages.Add(Properties.Resources.Yellow01);
            yellowImages.Add(Properties.Resources.Yellow02);
            yellowImages.Add(Properties.Resources.Yellow03);
            yellowImages.Add(Properties.Resources.Yellow04);
            yellowImages.Add(Properties.Resources.Yellow05);
            yellowImages.Add(Properties.Resources.Yellow06);
            yellowImages.Add(Properties.Resources.Yellow07);
            yellowImages.Add(Properties.Resources.Yellow08);
            yellowImages.Add(Properties.Resources.Yellow09);
            yellowImages.Add(Properties.Resources.Yellow10);
        }

        /// <summary>
        /// 色ごとのぷよの連鎖設定
        /// </summary>
        Image GetImageFromPuyoTypeRensa(Puyo puyo, int rensa)
        {
            if (puyo == Puyo.Puyo1)
            {
                if (rensa < redImages.Count)
                    return redImages[rensa - 1];
                else
                    return redImages[redImages.Count - 1];
            }
            if (puyo == Puyo.Puyo2)
            {
                if (rensa < blueImages.Count)
                    return blueImages[rensa - 1];
                else
                    return blueImages[blueImages.Count - 1];
            }
            if (puyo == Puyo.Puyo3)
            {
                if (rensa < greenImages.Count)
                    return greenImages[rensa - 1];
                else
                    return greenImages[greenImages.Count - 1];
            }
            if (puyo == Puyo.Puyo4)
            {
                if (rensa < yellowImages.Count)
                    return yellowImages[rensa - 1];
                else
                    return yellowImages[yellowImages.Count - 1];
            }
            return null;
        }

        /// <summary>
        /// ぷよのタイプを指定
        /// </summary>
        /// <param name="puyo">ぷよ</param>
        /// <returns>ぷよのタイプ</returns>
        Image GetImageFromPuyoType(Puyo puyo)
        {
            if (puyo == Puyo.Puyo1)
                return redPuyoImage;
            else if (puyo == Puyo.Puyo2)
                return bluePuyoImage;
            else if (puyo == Puyo.Puyo3)
                return greenPuyoImage;
            else if (puyo == Puyo.Puyo4)
                return yellowPuyoImage;
            else if (puyo == Puyo.Wall)
                return wallImage;

            return null;
        }

        /// <summary>
        /// ぷよを表示するセルにデータをセット
        /// </summary>
        /// <param name="colum">列</param>
        /// <param name="row">行</param>
        /// <param name="puyo">ぷよ</param>
        void SetPuyo(int colum, int row, Puyo puyo)
        {
            Cell cell = Cells[row, colum];
            cell.Puyo = puyo;
            Field.Invalidate();
        }

        void ClearCheck()
        {
            for (int i = 0; i < FIELD_HEIGHT; i++)
            {
                for (int j = 0; j < FIELD_WIDTH; j++)
                {
                    Cell cell = Cells[i, j];
                    cell.isChecked = false;
                }
            }
        }

        /// <summary>
        /// 4つ以上つながっているぷよを探す
        /// </summary>
        /// <param name="cell">セル</param>
        /// <returns>4つ以上つながっているぷよ</returns>
        List<Cell> GetConectedCell(Cell cell)
        {
            int colum = cell.Colum;
            int row = cell.Row;

            List<Cell> cells = new List<Cell>();

            if (0 < colum && colum < FIELD_WIDTH - 1 && 0 < row && row < FIELD_HEIGHT - 1)
            {
                Puyo puyo = Cells[row, colum].Puyo;
                if (puyo == Puyo.None || puyo == Puyo.Wall)
                    return cells;

                if (!Cells[row + 1, colum].isChecked)
                {
                    Cells[row + 1, colum].isChecked = true;
                    if (Cells[row + 1, colum].Puyo == puyo)
                    {
                        cells.Add(Cells[row + 1, colum]);
                        cells.AddRange(GetConectedCell(Cells[row + 1, colum]));
                    }
                }

                if (!Cells[row - 1, colum].isChecked)
                {
                    Cells[row - 1, colum].isChecked = true;
                    if (Cells[row - 1, colum].Puyo == puyo)
                    {
                        cells.Add(Cells[row - 1, colum]);
                        cells.AddRange(GetConectedCell(Cells[row - 1, colum]));
                    }
                }

                if (!Cells[row, colum + 1].isChecked)
                {
                    Cells[row, colum + 1].isChecked = true;
                    if (Cells[row, colum + 1].Puyo == puyo)
                    {
                        cells.Add(Cells[row, colum + 1]);
                        cells.AddRange(GetConectedCell(Cells[row, colum + 1]));
                    }
                }

                if (!Cells[row, colum - 1].isChecked)
                {
                    Cells[row, colum - 1].isChecked = true;
                    if (Cells[row, colum - 1].Puyo == puyo)
                    {
                        cells.Add(Cells[row, colum - 1]);
                        cells.AddRange(GetConectedCell(Cells[row, colum - 1]));
                    }
                }
            }
            return cells;
        }

        /// <summary>
        ///  空洞をうめるための処理
        /// </summary>
        async Task<bool> DownPuyoIfSpaces()
        {
            bool ret = false;
            while (DownPuyoIfSpace())
            {
                ret = true;
                await Task.Delay(100);
            }
            return ret;
        }

        /// <summary>
        /// ぷよの直下空洞チェック　
        /// </summary>
        bool DownPuyoIfSpace()
        {
            bool ret = false;
            for (int y = FIELD_HEIGHT - 3; y >= 0; y--)
            {
                for (int x = 0; x < FIELD_WIDTH; x++)
                {
                    if (Cells[y + 1, x].Puyo == Puyo.None && Cells[y, x].Puyo != Puyo.None)
                    {
                        // 下に空間があるぷよは下に落とす
                        SetPuyo(x, y + 1, Cells[y, x].Puyo);
                        Cells[y + 1, x].IsFixed = Cells[y, x].IsFixed;

                        SetPuyo(x, y, Puyo.None);
                        Cells[y, x].IsFixed = false;

                        ret = true;
                    }
                }
            }
            if (ret)
                Field.Invalidate();

            return ret;
        }

        /// <summary>t
        /// 上から落ちてきたぷよとの当たり判定
        /// </summary>
        /// <param name="colum">列</param>
        /// <param name="row">行</param>
        void FixPuyo(int colum, int row)
        {
            Cells[row, colum].IsFixed = true;
        }

        /// <summary>
        ///  左に移動できるか判別
        /// </summary>
        void PuyoMoveLeftCheck()
        {
            int oldX = puyoPositionX;
            int oldY = puyoPositionY;

            Position subPosition = GetSubPuyoPosition(puyoPositionX, puyoPositionY, PuyoAngle);
            int oldSubX = subPosition.Colum;
            int oldSubY = subPosition.Row;

            // 移動できるのか？
            int newX = puyoPositionX - 1;
            int newY = puyoPositionY;
            int newSubX = oldSubX - 1;
            int newSubY = subPosition.Row;

            bool ret1 = CanMove(new Position(newX, newY));
            bool ret2 = CanMove(new Position(newSubX, newSubY));

            if (ret1 && ret2)
            {
                SetPuyo(oldX, oldY, Puyo.None);
                SetPuyo(oldSubX, oldSubY, Puyo.None);

                puyoPositionX = newX;
                puyoPositionY = newY;
                SetPuyo(puyoPositionX, puyoPositionY, DropingPuyo);
                SetPuyo(newSubX, newSubY, SubDropingPuyo);
            }
        }

        /// <summary>
        /// ぷよが移動可能か判別
        /// </summary>
        bool CanMove(Position newPos)
        {
            if (
                newPos.Colum < 0 || newPos.Colum > FIELD_WIDTH - 1 ||
                newPos.Row < 0 || newPos.Row > FIELD_HEIGHT - 1 ||
                Cells[newPos.Row, newPos.Colum].IsFixed ||
                Cells[newPos.Row, newPos.Colum].Puyo == Puyo.Wall
            )
                return false;
            else
                return true;
        }

        bool CanMoveDown(Position newPos)
        {
            if (
                newPos.Row < 0 || newPos.Row > FIELD_HEIGHT - 2 ||
                Cells[newPos.Row, newPos.Colum].IsFixed
            )
                return false;
            else
                return true;
        }


        /// <summary>
        ///  右に移動できるか判別
        /// </summary>
        void PuyoMoveRightCheck()
        {
            int oldX = puyoPositionX;
            int oldY = puyoPositionY;

            Position subPosition = GetSubPuyoPosition(puyoPositionX, puyoPositionY, PuyoAngle);
            int oldSubX = subPosition.Colum;
            int oldSubY = subPosition.Row;

            // 移動できるのか？
            int newX = puyoPositionX + 1;
            int newY = puyoPositionY;
            int newSubX = oldSubX + 1;
            int newSubY = subPosition.Row;

            bool ret1 = CanMove(new Position(newX, newY));
            bool ret2 = CanMove(new Position(newSubX, newSubY));

            if (ret1 && ret2)
            {
                SetPuyo(oldX, oldY, Puyo.None);
                if (oldSubY > -1)
                    SetPuyo(oldSubX, oldSubY, Puyo.None);

                puyoPositionX = newX;
                puyoPositionY = newY;
                SetPuyo(puyoPositionX, puyoPositionY, DropingPuyo);
                SetPuyo(newSubX, newSubY, SubDropingPuyo);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            PuyoMoveDownCheck();
        }


        /// <summary>
        /// 新しいぷよ作成
        /// </summary>
        void CreateNewPuyo()
        {
            PuyoAngle = Angle.Angle0;

            int r = Random.Next(0, 4);
            if (r == 0)
                DropingPuyo = Puyo.Puyo1;
            if (r == 1)
                DropingPuyo = Puyo.Puyo2;
            if (r == 2)
                DropingPuyo = Puyo.Puyo3;
            if (r == 3)
                DropingPuyo = Puyo.Puyo4;

            r = Random.Next(0, 4);
            if (r == 0)
                SubDropingPuyo = Puyo.Puyo1;
            if (r == 1)
                SubDropingPuyo = Puyo.Puyo2;
            if (r == 2)
                SubDropingPuyo = Puyo.Puyo3;
            if (r == 3)
                SubDropingPuyo = Puyo.Puyo4;

            puyoPositionX = puyoStartPositionX;
            puyoPositionY = puyoStartPositionY;
            SetPuyo(puyoPositionX, puyoPositionY, DropingPuyo);
            SetPuyo(puyoPositionX, puyoPositionY - 1, SubDropingPuyo);
        }


        /// <summary>
        ///  下に移動できるか判別
        /// </summary>
        void PuyoMoveDownCheck()
        {
            int oldX = puyoPositionX;
            int oldY = puyoPositionY;

            Position subPosition = GetSubPuyoPosition(puyoPositionX, puyoPositionY, PuyoAngle);
            int oldSubX = subPosition.Colum;
            int oldSubY = subPosition.Row;

            // 移動できるのか？
            int newX = puyoPositionX;
            int newY = puyoPositionY + 1;
            int newSubX = oldSubX;
            int newSubY = subPosition.Row + 1;

            // 下げられないなら固定する
            if (!CanMoveDown(new Position(newX, newY)) || !CanMoveDown(new Position(newSubX, newSubY)))
            {
                FixPuyo(oldX, oldY);
                FixPuyo(oldSubX, oldSubY);
                OnFixed();
                return;
            }

            SetPuyo(oldX, oldY, Puyo.None);
            SetPuyo(oldSubX, oldSubY, Puyo.None);

            puyoPositionY = newY;
            SetPuyo(puyoPositionX, puyoPositionY, DropingPuyo);
            SetPuyo(newSubX, newSubY, SubDropingPuyo);
        }

        /// <summary>
        /// ぷよ削除用
        /// </summary>
        async Task<int> DeletePuyo(int rensa)
        {
            // 設置されたプヨの下に空洞があれば上のプヨを落として空洞をうめる
            Task<bool> task = DownPuyoIfSpaces();
            await task;

            List<Cell> deleteCells = new List<Cell>();

            // 連結ボーナス計算用
            List<int> vs = new List<int>();

            // 4つつながっているぷよがあるか探す
            for (int i = 0; i < FIELD_HEIGHT; i++)
            {
                for (int j = 0; j < FIELD_WIDTH; j++)
                {
                    Cell cell = Cells[i, j];
                    if (deleteCells.Any(x => x == cell))
                        continue;

                    // 連鎖と同時消しは区別するため別の変数に格納する
                    List<Cell> cells = GetConectedCell(cell);
                    ClearCheck();

                    if (cells.Count >= 4)
                    {
                        deleteCells.AddRange(cells);
                        vs.Add(cells.Count);

                        // 消えるぷよに連鎖回数がわかるように番号をつける
                        Puyo puyo = cells[0].Puyo;
                        Image image = GetImageFromPuyoTypeRensa(puyo, rensa);

                        Graphics g = Graphics.FromHwnd(Field.Handle);
                        foreach (Cell cell1 in cells)
                        {
                            Rectangle rect = new Rectangle(cell1.LeftTop, Cell.Size);
                            g.DrawImage(image, rect);
                        }
                        g.Dispose();
                    }
                }
            }

            // ぷよの消えた数 ×（連鎖ボーナス＋連結ボーナス＋色数ボーナス）× 10

            // ぷよの消えた数 deleteCells
            int puyoCount = deleteCells.Count;

            // 連鎖ボーナス rensa
            int rensaBonus = GetRensaBonus(rensa);

            // 連結ボーナス cells.Count
            int renketsuBonus = vs.Sum(x => GetRenketsuBonus(x));

            // 色数ボーナス
            var colorsKind = deleteCells.Distinct(new PuyoEqualityComparer()).Count();
            int colorsBonus = GetColorCountBonus(colorsKind);

            int score = 0;
            if (rensaBonus + renketsuBonus + colorsBonus != 0)
                score = puyoCount * (rensaBonus + renketsuBonus + colorsBonus) * 10;
            else
                score = puyoCount * 10;

            // ぷよを実際に消す
            foreach (Cell cell0 in deleteCells)
            {
                cell0.Puyo = Puyo.None;
            }
            if (deleteCells.Count != 0)
            {
                await Task.Delay(1000);

                Field.Invalidate();
            }

            return score;
        }

        // 連鎖ボーナスの計算
        int GetRenketsuBonus(int i)
        {
            if (i == 4)
                return 0;
            if (i >= 5 || i <= 10)
                return i - 3;
            if (i < 11)
                return 10;
            return 0;
        }

        // 連鎖ボーナスの計算
        int GetRensaBonus(int i)
        {
            if (i == 1)
                return 0;
            if (i == 2)
                return 8;
            if (i == 3)
                return 16;
            if (i >= 4 || i <= 19)
                return (i - 3) * 32;
            if (i > 19)
                return 512;
            return 0;
        }

        // 連鎖ボーナスの計算
        int GetColorCountBonus(int i)
        {
            if (i == 1)
                return 0;
            if (i == 2)
                return 3;
            if (i == 3)
                return 6;
            if (i == 4)
                return 12;
            if (i == 5)
                return 24;
            return 0;
        }

        int score = 0;


        /// <summary>
        /// 　ぷよが消えたときに空洞をうめる処理
        /// </summary>
        async void OnFixed()
        {
            isIgnoreKeyDown = true;
            timer1.Stop();

            int rensa = 1;
            int tempScore = 0;

            while (true)
            {
                Task<int> task = DeletePuyo(rensa);
                int ret = await task;
                if (ret == 0)
                    break;

                tempScore += ret;
                rensa++;
            }

            Score += tempScore;

            isIgnoreKeyDown = false;
            timer1.Start();

            CreateNewPuyo();
        }


        /// <summary>
        /// スコア表示
        /// </summary>
        int Score
        {
            get { return score; }
            set
            {
                score = value;
                label1.Text = "Score " + score.ToString();
            }
        }

        private void GameStartMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewPuyo();
            timer1.Start();
        }
    }

    class PuyoEqualityComparer : IEqualityComparer<Cell>
    {
        public bool Equals(Cell cell1, Cell cell2)
        {
            if (cell2 == null && cell1 == null)
                return true;
            else if (cell1 == null || cell2 == null)
                return false;
            else if (cell1.Puyo == cell2.Puyo)
                return true;
            else
                return false;
        }

        public int GetHashCode(Cell cell)
        {
            int hCode = cell.Puyo.GetHashCode();
            return hCode.GetHashCode();
        }
    }
}
