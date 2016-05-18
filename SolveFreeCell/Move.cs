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
        ace1, ace2, ace3, ace4, free1, free2, free3, free4,        
    }
    class Move
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
        public Move(Card card, Position from, Position to)
        {
            m_Card = card;
            m_From = from;
            m_To = to;
        }
        public override string ToString()
        {
            return m_Card.ToString() + " From " + m_From.ToString() + " to " + m_To;
        }

    }
}
