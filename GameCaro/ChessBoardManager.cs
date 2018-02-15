using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public class ChessBoardManager
    {
     
        #region Properties
        private Panel chessBoard;
        public Panel ChessBoard
        {
            get { return chessBoard; }
            set { chessBoard = value; }
        }
        private List<Player> player;
        public List<Player> Player
        {
            get
            {
                return player;
            }

            set
            {
                player = value;
            }
        }

        public int CurrentPlayer
        {
            get
            {
                return currentPlayer;
            }

            set
            {
                currentPlayer = value;
            }
        }
        private TextBox playerName;
        public TextBox PlayerName
        {
            get
            {
                return playerName;
            }

            set
            {
                playerName = value;
            }
        }
        private PictureBox playerMark;
        public PictureBox PlayerMark
        {
            get
            {
                return playerMark;
            }

            set
            {
                playerMark = value;
            }
        }
        private List<List<Button>> matrix;
        public List<List<Button>> Matrix
        {
            get
            {
                return matrix;
            }

            set
            {
                matrix = value;
            }
        }

        

        private event EventHandler playerMarked;
        public event EventHandler PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }
        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }

        private int currentPlayer;
        private Stack<PlayInfo> playTimeLine;
        public Stack<PlayInfo> PlayTimeLine
        {
            get
            {
                return playTimeLine;
            }

            set
            {
                playTimeLine = value;
            }
        }
        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard, TextBox playerName, PictureBox mark)
        {
            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;
            this.Player = new List<Player>()
            {
                new Player("P1",Image.FromFile (Application.StartupPath + "\\Resources\\p1.png")),
                new Player("P2",Image.FromFile (Application.StartupPath + "\\Resources\\p2.png"))
            };
            
            
        }
        #endregion

        #region Methods
        public void DrawChessBar()
        {
            
            ChessBoard.Enabled = true;
            ChessBoard.Controls.Clear();
            PlayTimeLine = new Stack<PlayInfo>();
            CurrentPlayer = 0;
            ChangePlayer();

            Matrix = new List<List<Button>>();
            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < Cons.BoardChessHeight; i++)
            {
                Matrix.Add(new List<Button>());
                for (int j = 0; j < Cons.BoardChessWidth; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.ChessWidth,
                        Height = Cons.ChessHeight,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag=i.ToString()
                    };
                    btn.Click += Btn_Click;
                    ChessBoard.Controls.Add(btn);
                    Matrix[i].Add(btn);
                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.ChessHeight);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null)
            {
                return;
            }
            Mark(btn);
            PlayTimeLine.Push(new PlayInfo(getChessPoint(btn), CurrentPlayer));
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            ChangePlayer();
            if (playerMarked != null)
            {
                playerMarked(this, new EventArgs());
            }
            if (isEndGame(btn))
            {
                EndGame();
            }
        }
        public void EndGame()
        {
            if (endedGame != null)
            {
                endedGame(this, new EventArgs());
            }
        }
        public bool Undo()
        {
            if (PlayTimeLine.Count <= 0)
            {
                return false;
            }
            PlayInfo oldPoint = PlayTimeLine.Pop();
            Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];
            btn.BackgroundImage = null;
            if (PlayTimeLine.Count <= 0)
            {
                CurrentPlayer = 0;
            }
            else
            {
                oldPoint = PlayTimeLine.Peek();
                CurrentPlayer = oldPoint.CurrentPlayer == 1 ? 0 : 1;
            }
            ChangePlayer();
            return true;
        }
        private bool isEndGame(Button btn)
        {
            return isEndHorizontal(btn) || isEndVertical(btn) || isEndPrimary(btn) || isEndSub(btn);
        }
        private Point getChessPoint(Button btn)
        {
            
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);
            Point point = new Point(horizontal,vertical);
            return point;
        }
        private bool isEndHorizontal(Button btn)
        {
            Point point = getChessPoint(btn);
            int CountLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    CountLeft++;
                }
                else
                {
                    break;
                }
            }
            int CountRight = 0;
            for (int i = point.X + 1; i < Cons.BoardChessWidth; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    CountRight++;
                }
                else
                {
                    break;
                }
            }

            return CountLeft + CountRight == 5;
        }
        private bool isEndVertical(Button btn)
        {
            Point point = getChessPoint(btn);
            int CountTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    CountTop++;
                }
                else
                {
                    break;
                }
            }
            int CountBottom = 0;
            for (int i = point.Y + 1; i < Cons.BoardChessHeight; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    CountBottom++;
                }
                else
                {
                    break;
                }
            }

            return CountTop + CountBottom == 5;
        }
        private bool isEndPrimary(Button btn)
        {
            Point point = getChessPoint(btn);
            int CountTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.Y - i < 0 || point.X - i < 0)
                {
                    break;
                }
                if (Matrix[point.Y - i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    CountTop++;
                }
                else
                {
                    break;
                }
            }
            int CountBottom = 0;
            for (int i = 1; i <= Cons.BoardChessWidth - point.X; i++)
            {
                if (point.Y + i >= Cons.BoardChessHeight || point.X + i >= Cons.BoardChessWidth)
                {
                    break;
                }
                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    CountBottom++;
                }
                else
                {
                    break;
                }
            }

            return CountTop + CountBottom == 5;
        }
        private bool isEndSub(Button btn)
        {
            Point point = getChessPoint(btn);
            int CountTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Cons.BoardChessWidth || point.Y  - i < 0)
                {
                    break;
                }
                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    CountTop++;
                }
                else
                {
                    break;
                }
            }
            int CountBottom = 0;
            for (int i = 1; i <= Cons.BoardChessWidth - point.X; i++)
            {
                if (point.Y + i >= Cons.BoardChessHeight || point.X - i < 0)
                {
                    break;
                }
                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    CountBottom++;
                }
                else
                {
                    break;
                }
            }

            return CountTop + CountBottom == 5;
        }
        private void Mark(Button btn)
        {
            btn.BackgroundImage = Player[CurrentPlayer].Mark;
            
        }
        private void ChangePlayer()
        {
            PlayerName.Text = Player[CurrentPlayer].Name;
            PlayerMark.Image = Player[CurrentPlayer].Mark;
        }
        #endregion

    }
}
