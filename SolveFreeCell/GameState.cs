using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
        private Stack<GameMove> m_History = new Stack<GameMove>();
        public GameState()
        {
            m_Columns = InitializeArray<Stack<Card>>(8);
        }
        private void AddCard(int col, Card card)
        {
            m_Columns[col].Push(card);
        }
        public Card GetCard(Position at)
        {
            switch (at)
            {
                case Position.col1: return m_Columns[0].ElementAtOrDefault(0);
                case Position.col2: return m_Columns[1].ElementAtOrDefault(0);
                case Position.col3: return m_Columns[2].ElementAtOrDefault(0);
                case Position.col4: return m_Columns[3].ElementAtOrDefault(0);
                case Position.col5: return m_Columns[4].ElementAtOrDefault(0);
                case Position.col6: return m_Columns[5].ElementAtOrDefault(0);
                case Position.col7: return m_Columns[6].ElementAtOrDefault(0);
                case Position.col8: return m_Columns[7].ElementAtOrDefault(0);
                case Position.ace1: return m_Aces[0];
                case Position.ace2: return m_Aces[1];
                case Position.ace3: return m_Aces[2];
                case Position.ace4: return m_Aces[3];
                case Position.free1: return m_Free[0];
                case Position.free2: return m_Free[1];
                case Position.free3: return m_Free[2];
                case Position.free4: return m_Free[3];
                case Position.free:
                    // return empty if any 1 of the 4 free cells is empty
                    foreach(Card f in m_Free)
                    {
                        if (f.IsEmpty()) return f;
                    }
                    // they're all occupied, just return the 1st
                    return m_Free[0];
            }
            return null;
        }
        public bool IsLegal(Position from, Position to)
        {
            if (from == to) return false;
            Card first = GetCard(from);
            if (first.IsEmpty()) return false;
            if(first.Rank == 0)
            {
                if (to != Position.ace) return false;
                return true;
            }
            Card second = GetCard(to);
            switch (to)
            {
                case Position.free:
                    return second.IsEmpty();
                case Position.ace1:
                case Position.ace2:
                case Position.ace3:
                case Position.ace4:
                    if (second.IsEmpty())
                    {
                        return first.Rank == 0;
                    }
                    if(first.Suit != second.Suit)
                    {
                        return false;
                    }
                    if(first.Rank == (second.Rank + 1)){
                        return true;
                    }
                    return false;
                case Position.col1:
                case Position.col2:
                case Position.col3:
                case Position.col4:
                case Position.col5:
                case Position.col6:
                case Position.col7:
                case Position.col8:
                    if (second.IsEmpty()) return true;
                    if(first.IsBlack() != second.IsBlack())
                    {
                        return ((first.Rank+1) == second.Rank);
                    }
                    break;
                case Position.free1:
                case Position.free2:
                case Position.free3:
                case Position.free4:
                    Console.WriteLine("oops");
                    break;
            }
            return false;
        }
        public GameState(GameState orig)
        {
            this.m_Columns = orig.m_Columns;
            this.m_Aces = orig.m_Aces;
            this.m_Free = orig.m_Free;
            this.m_History = orig.m_History;
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
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0} {1} {2} {3}|{4} {5} {6} {7}\n",
                m_Aces[0], m_Aces[1], m_Aces[2], m_Aces[3],
                m_Free[0], m_Free[1], m_Free[2], m_Free[3]);
            sb.AppendLine("__ __ __ __|__ __ __ __");
            int nMaxRows = 0;
            for (int col = 0; col < 8; ++col)
            {
                nMaxRows = Math.Max(nMaxRows, m_Columns[col].Count);
            }
            for (int row = 0; row < nMaxRows; ++row)
            {
                for (int col = 0; col < 8; ++col)
                {
                    if (row < m_Columns[col].Count)
                    {
                        int index = m_Columns[col].Count - 1 - row;
                        sb.AppendFormat("{0} ", m_Columns[col].ElementAt(index).ToString());
                    }
                    else
                    {
                        sb.AppendFormat("   ");
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
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
            //for(int row = 1; row <= m_History.Count; ++row)
            //{
            //    Console.WriteLine("History[{1}] {0}", m_History.ElementAt(m_History.Count - row), row);
            //}
        }
        public override bool Equals(object obj)
        {
            GameState other = obj as GameState;
            int freeCount = 0;
            int colCount = 0;
            for(int i = 0; i < 4; ++i)
            {
                for(int j = 0; j < 4; ++j)
                {
                    if(m_Free[i] == other.m_Free[j])
                    {
                        freeCount++;
                        break;
                    }
                }
            }
            if (freeCount != 4) return false;
            for(int i = 0; i < 8; ++i)
            {
                if (colCount != i) return false;
                for(int j = 0; j < 8; ++j)
                {
                    if (m_Columns[i].Equals(other.m_Columns[j]))
                    {
                        colCount++;
                        break;
                    }
                }
            }
            if (colCount != 8) return false;
            return true;
        }
        public static GameState operator +(GameState oldState,GameMove move)
        {
            GameState nextState = new GameState(oldState);
            Card temp = null;
            switch (move.From)
            {
                case Position.col1: case Position.col2: case Position.col3: case Position.col4:
                case Position.col5: case Position.col6: case Position.col7: case Position.col8:
                    temp = nextState.m_Columns[(int)move.From].Pop();
                    break;
                case Position.ace1: case Position.ace2: case Position.ace3: case Position.ace4:
                    {
                        int index = (int)move.From - (int)Position.ace1;
                        temp = nextState.m_Aces[index];
                        nextState.m_Aces[index] = new Card();
                    }
                    break;
                case Position.free1: case Position.free2: case Position.free3: case Position.free4:
                    {
                        int index = (int)move.From - (int)Position.free1;
                        temp = nextState.m_Free[index];
                        nextState.m_Free[index] = new Card();
                    }
                    break;
            }
            switch (move.To)
            {
                case Position.col1: case Position.col2: case Position.col3: case Position.col4:
                case Position.col5: case Position.col6: case Position.col7: case Position.col8:
                    nextState.m_Columns[(int)move.To].Push(temp);
                    break;
                case Position.ace1: case Position.ace2: case Position.ace3: case Position.ace4:
                case Position.ace:
                    {
                        if(temp.Rank == 0)
                        {
                            for(int col = 0; col < 4; ++col)
                            {
                                if (nextState.m_Aces[col].IsEmpty())
                                {
                                    nextState.m_Aces[col] = temp;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int col = 0; col < 4; ++col)
                            {
                                if (nextState.m_Aces[col].Suit == temp.Suit)
                                {
                                    nextState.m_Aces[col] = temp;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case Position.free:
                    {
                        for(int i = 0; i < 4; ++i)
                        {
                            if (nextState.m_Free[i].IsEmpty())
                            {
                                nextState.m_Free[i] = temp;
                                break;
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Illegal move");
                    break;
            }
            nextState.m_History.Push(move);
            return nextState;
        }
        public bool IsWinner()
        {
            for (int col = 0; col < 8; ++col)
            {
                if (m_Columns[col].Count > 1)
                {
                    int prevCard = m_Columns[col].ElementAt(0).Rank;
                    for (int i = 1; i < m_Columns[col].Count; ++i)
                    {
                        if((prevCard + 1) != m_Columns[col].ElementAt(i).Rank)
                        {
                            return false;
                        }
                        prevCard++;
                    }
                }
            }
            return true;
        }
    }
}
