using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SolveFreeCell
{
    class GameState
    {
        T[] InitializeArray<T>(int length) where T : new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }
        private Stack<Card>[] m_Columns;
        private Card[] m_Aces = new Card[4];
        private Card[] m_Free = new Card[4];
        private int m_TotalMoves = 0;
        public GameState()
        {
            m_Columns = InitializeArray<Stack<Card>>(8);
        }
        private void AddCard(int col, Card card)
        {
            m_Columns[col].Push(card);
        }
        public GameState(Image rawImg)
        {
            m_Columns = InitializeArray<Stack<Card>>(8);
            for(int i = 0; i< 4; ++i)
            {
                m_Aces[i] = new Card();
                m_Free[i] = new Card();
            }

            Bitmap bmRank = new Bitmap(40, 36);
            bmRank.SetResolution(rawImg.HorizontalResolution, rawImg.VerticalResolution);
            Graphics gRank = Graphics.FromImage(bmRank);
            Bitmap bmSuit = new Bitmap(36, 36);
            bmRank.SetResolution(rawImg.HorizontalResolution, rawImg.VerticalResolution);
            Graphics gSuit = Graphics.FromImage(bmSuit);
            int nCard = 0;
            for (int row = 0; row < 7; ++row)
            {
                for (int col = 0; col < 8; ++col)
                {
                    if (++nCard > 52) break;
                    Rectangle rRank = new Rectangle(7 + col * 90, 211 + row * 56, 40, 36);
                    gRank.Clear(Color.Black);
                    gRank.DrawImage(rawImg, 0, 0, rRank, GraphicsUnit.Pixel);
                    //string sRank = Path.Combine(Path.GetTempPath(), string.Format("rank{0}-{1}.bmp", row, col));
                    //bmRank.Save(sRank);
                    Rectangle rSuit = new Rectangle(48 + col * 90, 211 + row * 56, 36, 36);
                    gSuit.Clear(Color.Black);
                    gSuit.DrawImage(rawImg, 0, 0, rSuit, GraphicsUnit.Pixel);
                    //string sSuit = Path.Combine(Path.GetTempPath(), string.Format("suit{0}-{1}.bmp", row, col));
                    //bmSuit.Save(sSuit);
                    AddCard(col, new Card(bmRank, bmSuit));
                }
            }

        }
        public void Dump()
        {
            Console.WriteLine("{0} {1} {2} {3}|{4} {5} {6} {7}",
                m_Aces[0], m_Aces[1], m_Aces[2], m_Aces[3],
                m_Free[0], m_Free[1], m_Free[2], m_Free[3]);
            Console.WriteLine("__ __ __ __|__ __ __ __");
            int nMaxRows = 0;
            for(int col = 0; col < 8; ++col)
            {
                nMaxRows = Math.Max(nMaxRows, m_Columns[col].Count);
            }
            for(int row = 0; row < nMaxRows; ++row)
            {
                for (int col = 0; col < 8; ++col)
                {
                    if(row < m_Columns[col].Count)
                    {
                        int index = m_Columns[col].Count - 1 - row;
                        Console.Write("{0} ", m_Columns[col].ElementAt(index).ToString());
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                }
                Console.WriteLine();
            }
        }
        public static GameState operator +(GameState oldState,Move move)
        {
            oldState.m_TotalMoves++;
            Card temp = null;
            switch (move.From)
            {
                case Position.col1: case Position.col2: case Position.col3: case Position.col4:
                case Position.col5: case Position.col6: case Position.col7: case Position.col8:
                    temp = oldState.m_Columns[(int)move.From].Pop();
                    break;
                case Position.ace1: case Position.ace2: case Position.ace3: case Position.ace4:
                    {
                        int index = (int)move.From - (int)Position.ace1;
                        temp = oldState.m_Aces[index];
                        oldState.m_Aces[index] = new Card();
                    }
                    break;
                case Position.free1: case Position.free2: case Position.free3: case Position.free4:
                    {
                        int index = (int)move.From - (int)Position.free1;
                        temp = oldState.m_Free[index];
                        oldState.m_Free[index] = new Card();
                    }
                    break;
            }
            switch (move.To)
            {
                case Position.col1: case Position.col2: case Position.col3: case Position.col4:
                case Position.col5: case Position.col6: case Position.col7: case Position.col8:
                    oldState.m_Columns[(int)move.To].Push(temp);
                    break;
                case Position.ace1: case Position.ace2: case Position.ace3: case Position.ace4:
                    {
                        int index = (int)move.To - (int)Position.ace1;
                        oldState.m_Aces[index] = temp;
                    }
                    break;
                case Position.free1: case Position.free2: case Position.free3: case Position.free4:
                    {
                        int index = (int)move.To - (int)Position.free1;
                        oldState.m_Free[index] = temp;
                    }
                    break;
            }
            return oldState;
        }
    }
}
