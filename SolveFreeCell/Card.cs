using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.IO;

namespace SolveFreeCell
{
    class Card
    {
        private Byte m_card;
        private static char[] ranks =  { 'A' ,'2','3','4','5','6','7','8','9','T','J','Q','K','?','?','.',};
        private static List<BitMapHash> m_Ranks = null;
        private static List<BitMapHash> m_Suits = null;

        public Card()
        {
            m_card = 0xFF;
        }
        public Card(int suit, int rank)
        {
            m_card = (Byte)(suit * 16 + rank);
        }
        public Card(Bitmap bmRank, Bitmap bmSuit)
        {
            Populate();
            BitMapHash targetSuit = new BitMapHash(-1, bmSuit);
            int suit = -1;
            int rank = -1;
            int best = int.MaxValue;
            foreach(BitMapHash test in m_Suits)
            {
                int nCheck = test.Compare(targetSuit);
                if(nCheck < best)
                {
                    suit = test.Value;
                    best = nCheck;
                }
            }
            BitMapHash targetRank = new BitMapHash(-1, bmRank);
            best = int.MaxValue;
            foreach (BitMapHash test in m_Ranks)
            {
                int nCheck = test.Compare(targetRank);
                if (nCheck < best)
                {
                    rank = test.Value;
                    best = nCheck;
                }
            }
            m_card = (Byte)(suit * 16 + rank);
        }

        private void Populate()
        {
            if(m_Ranks == null)
            {
                m_Ranks = new List<BitMapHash>(26);
                for(int c = 0; c < 13; ++c)
                {
                    string sRes = string.Format("SolveFreeCell.Images.B{0}.bmp", ranks[c]);
                    m_Ranks.Add(new BitMapHash(c, new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream(sRes))));
                    sRes = string.Format("SolveFreeCell.Images.R{0}.bmp", ranks[c]);
                    m_Ranks.Add(new BitMapHash(c, new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream(sRes))));
                }

                m_Suits = new List<BitMapHash>(4);
                m_Suits.Add(new BitMapHash(0, new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("SolveFreeCell.Images.Clubs.bmp"))));
                m_Suits.Add(new BitMapHash(1, new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("SolveFreeCell.Images.Diamonds.bmp"))));
                m_Suits.Add(new BitMapHash(2, new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("SolveFreeCell.Images.Hearts.bmp"))));
                m_Suits.Add(new BitMapHash(3, new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("SolveFreeCell.Images.Spades.bmp"))));
            }
        }

        public override string ToString()
        {
            string sResult = ranks[m_card & 0x0F].ToString();
            switch(m_card & 0xF0)
            {
                case 0x00:  sResult += 'C'; break;
                case 0x10:  sResult += 'D'; break;
                case 0x20:  sResult += 'H'; break;
                case 0x30:  sResult += 'S'; break;
                default:    sResult += '.'; break;
            }
            return sResult;
        }
    }
}
