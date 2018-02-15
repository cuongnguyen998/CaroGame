using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCaro
{
    public class SocketData
    {
        private int command;
        public int Command
        {
            get
            {
                return command;
            }

            set
            {
                command = value;
            }
        }
        private Point? point;
        public Point? Point
        {
            get
            {
                return point;
            }

            set
            {
                point = value;
            }
        }
        private string message;
        private int nOTIFY;

        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
            }
        }

        
        public SocketData(int command, string message = null, Point? point = null)
        {
            this.Command = command;
            this.Point = point;
            this.Message = message;
        }

        public SocketData(int nOTIFY)
        {
            this.nOTIFY = nOTIFY;
        }

        public enum SocketCommand
        {
            SEND_POINT,
            NOTIFY,
            NEW_GAME,
            UNDO,
            QUIT
        }

        
    }
}
