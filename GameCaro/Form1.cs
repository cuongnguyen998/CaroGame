using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GameCaro.SocketData;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        SocketManager socket;
        public Form1()
        {
            InitializeComponent();
            ChessBoard = new ChessBoardManager(pnlChessBoard, txbPlayerName, pctbMark);
            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;
            prcbCoolDown.Step = Cons.CoolDownStep;
            prcbCoolDown.Maximum = Cons.CoolDownTime;
            prcbCoolDown.Value = 0;
            tmCoolDown.Interval = Cons.CoolDownInterval;
            tmCoolDown.Start();
            socket = new SocketManager();
            NewGame();
        }
        void EndGame()
        {
            tmCoolDown.Stop();
            pnlChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            MessageBox.Show("End Game");
        }
        void NewGame()
        {
            prcbCoolDown.Value = 0;
            tmCoolDown.Stop();
            undoToolStripMenuItem.Enabled = true;
            ChessBoard.DrawChessBar();
        }
        void Undo()
        {
            ChessBoard.Undo();
        }
        void Quit()
        {
            Application.Exit();
        }
        private void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmCoolDown.Start();
            prcbCoolDown.Value = 0;
            socket.Send(new SocketData((int)SocketCommand.SEND_POINT));
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            prcbCoolDown.PerformStep();
            if (prcbCoolDown.Value >= prcbCoolDown.Maximum)
            {
                EndGame();
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Quit Game?", "Notification", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                e.Cancel = true;
            }

        }

        private void btnLAN_Click(object sender, EventArgs e)
        {
            socket.IP = txbIP.Text;
            if (!socket.ConnectServer())
            {
                socket.CreateServer();
            }
            else
            {
                Listen();
                //socket.Send(new SocketData((int)SocketCommand.NOTIFY, "Client đã kết nối"));
                
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            if (string.IsNullOrEmpty(txbIP.Text))
            {
                txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }

        void Listen()
        {            
                Thread listenThread = new Thread(() =>
                {
                    try
                    {
                        SocketData data = (SocketData)socket.Receive();
                        ProcessData(data);
                    }
                    catch
                    {
                                                
                    }                    
                });
                listenThread.IsBackground = true;
                listenThread.Start();
            }

        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.NOTIFY:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketCommand.NEW_GAME:
                    break;
                case (int)SocketCommand.SEND_POINT:
                    break;
                case (int)SocketCommand.QUIT:
                    break;
                case (int)SocketCommand.UNDO:
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}

