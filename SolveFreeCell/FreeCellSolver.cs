using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace SolveFreeCell
{
    public partial class FreeCellSolver : Form
    {
        private List<string> m_ImagePaths = new List<string>();
        private GameState m_Game = null;
        public FreeCellSolver()
        {
            InitializeComponent();
        }

        private void FreeCellSolver_Load(object sender, EventArgs e)
        {
            Outlook.Application oApp = new Outlook.Application();
            Outlook.MAPIFolder inbox = oApp.ActiveExplorer().Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            Outlook.Items items = inbox.Items;
            Outlook.MailItem mailItem = null;
            object folderItem;
            string subjectName = string.Empty;
            string filter = "[Subject] = 'Large free cell image'";
            folderItem = items.Find(filter);
            while (folderItem != null)
            {
                mailItem = folderItem as Outlook.MailItem;
                if (mailItem != null)
                {
                    foreach(Outlook.Attachment att in mailItem.Attachments)
                    {
                        switch (Path.GetExtension(att.FileName).ToLower())
                        {
                            case ".png":
                                string imgPath = Path.Combine(Path.GetTempPath(), att.FileName);
                                m_ImagePaths.Add(imgPath);
                                toolStripComboBox1.Items.Add(Path.GetFileName(imgPath));
                                att.SaveAsFile(imgPath);
                                break;
                        }
                    }
                }
                folderItem = items.FindNext();
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Image img = Image.FromFile(Path.Combine(Path.GetTempPath(), toolStripComboBox1.Items[toolStripComboBox1.SelectedIndex].ToString()));
            Console.WriteLine("image({0},{1})", img.Width, img.Height);
            pictureBox1.Image = img;
            m_Game = new GameState(img);
            m_Game.Dump();
            Move m = new Move(new Card(1, 1), Position.col1, Position.col2);
            Console.WriteLine("Move {0}", m);
            GameState g = m_Game + m;
            g.Dump();
            m = new Move(new Card(1, 1), Position.col1, Position.free1);
            Console.WriteLine("Move {0}", m);
            g = g + m;
            g.Dump();
        }

    }
}
