using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace ASSbot
{
    public class ConnectFour
    {
        IUser[] player = new IUser[2];
        int turn;
        int turnCount = 0;

        Random rdm = new Random();

        bool ongoing = false;

        int[,] board = new int[7, 6];

        public ConnectFour(IUser p1, IUser p2)
        {
            player[0] = p1;
            player[1] = p2;
            turn = rdm.Next(2);
            for (int x = 0; x < 7; x++) for (int y = 0; y < 6; y++) board[x, y] = 0;
        
        }

        public bool IsOngoing() { return ongoing; }
        public IUser GetPlayer(int id) { return player[id]; }
        public int Turn() { return turn; }

        public void Start()
        {
            ongoing = true;
        }

        public void Play(int choice)
        {
            turnCount++;
            choice -= 1;
            for (int y = 0; y < 6; y++)
            {
                if (y == 5 || board[choice, y + 1] != 0) { board[choice, y] = turn; break; }
            }

            if (CheckForWinner()) ongoing = false; 
            else
            {
                if (turn == 1) turn = 2;
                else turn = 1;
            }
        }

        public int TurnCount() { return turnCount; }

        public string GenerateBoard()
        {
            string msg = "";


            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    msg += board[x, y];
                }

                msg += "\n";
            }

            msg = msg.Replace("0", ":black_circle:");
            msg = msg.Replace("1", ":large_blue_circle:");
            msg = msg.Replace("2", ":red_circle:");
            msg = msg.Replace("3", ":star:");

            msg += ":one::two::three::four::five::six::seven:";

            return msg;
        }
        
        int getHeight(){ return 6; }
        int getWidth() { return 7; }

        bool CheckForWinner()
        {
            // horizontalCheck 
            for (int j = 0; j < getHeight() - 3; j++)
            {
                for (int i = 0; i < getWidth(); i++)
                {
                    if (board[i,j] == turn && board[i,j + 1] == turn && board[i,j + 2] == turn && board[i,j + 3] == turn)
                    {
                        board[i, j] = 3;
                        board[i, j+1] = 3;
                        board[i, j+2] = 3;
                        board[i, j+3] = 3;
                        return true;
                    }
                }
            }
            // verticalCheck
            for (int i = 0; i < getWidth() - 3; i++)
            {
                for (int j = 0; j < getHeight(); j++)
                {
                    if (board[i,j] == turn && board[i + 1,j] == turn && board[i + 2,j] == turn && board[i + 3,j] == turn)
                    {
                        board[i, j] = 3;
                        board[i+1, j] = 3;
                        board[i+2, j] = 3;
                        board[i+3, j] = 3;
                        return true;
                    }
                }
            }
            // ascendingDiagonalCheck 
            for (int i = 3; i < getWidth(); i++)
            {
                for (int j = 0; j < getHeight() - 3; j++)
                {
                    if (board[i, j] == turn && board[i - 1, j + 1] == turn && board[i - 2, j + 2] == turn && board[i - 3, j + 3] == turn)
                    {
                        board[i, j] = 3;
                        board[i-1, j+1] = 3;
                        board[i-2, j+2] = 3;
                        board[i-3, j+3] = 3;
                        return true;
                    }
                }
            }
            // descendingDiagonalCheck
            for (int i = 3; i < getWidth(); i++)
            {
                for (int j = 3; j < getHeight(); j++)
                {
                    if (board[i, j] == turn && board[i - 1, j - 1] == turn && board[i - 2, j - 2] == turn && board[i - 3, j - 3] == turn)
                    {
                        board[i-1, j-1] = 3;
                        board[i-2, j-2] = 3;
                        board[i-3, j-3] = 3;
                        board[i-4, j-4] = 3;
                        return true;
                    }
                }
            }
            return false;
        }
        /*
         * fix four in a row
         * fix full columns
         * 
         */
    }
}
