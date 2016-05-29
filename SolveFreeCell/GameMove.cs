using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveFreeCell
{
    enum Position
    {
        col1=0, col2, col3, col4, col5, col6, col7, col8,
        ace1, ace2, ace3, ace4, ace, free1, free2, free3, free4, free,       
    }
    class GameMove
    {
        private Card m_Card;
        private Position m_From;
        public Position From
        {
            get
            {
                return m_From;
            }
        }
        public Position To
        {
            get
            {
                return m_To;
            }
        }
        private Position m_To;
        public GameMove(Card card, Position from, Position to)
        {
            m_Card = card;
            m_From = from;
            m_To = to;
        }
        public override string ToString()
        {
            return m_Card.ToString() + " From " + m_From.ToString() + " to " + m_To;
        }
        public static Position[] StartFrom = {
            Position.free1, Position.free2, Position.free3, Position.free4,
            Position.col1, Position.col2, Position.col3, Position.col4, Position.col5, Position.col6, Position.col7, Position.col8, };
        public static Position[] FinishAt = {
            Position.ace, Position.free, 
            Position.col1, Position.col2, Position.col3, Position.col4, Position.col5, Position.col6, Position.col7, Position.col8, };
    }
}
