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
            pictureBox1.Image = img;
            m_Game = new GameState(img);
            m_Game.Dump();
            //foreach(Position from in GameMove.StartFrom)
            //{
            //    foreach(Position to in GameMove.FinishAt)
            //    {
            //        if (m_Game.IsLegal(from, to))
            //        {
            //            Console.WriteLine("{0}", new GameMove(m_Game.GetCard(from), from, to));
            //        }
            //    }
            //}
            solveWorker.RunWorkerAsync(m_Game);
        }

        private void solveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Queue<GameState> gameQueue = new Queue<GameState>(40000);
            GameState g = e.Argument as GameState;
            int qTrigger = 1000;
            gameQueue.Enqueue(g);
            while(gameQueue.Count > 0)
            {
                g = gameQueue.Dequeue();
                foreach (Position from in GameMove.StartFrom)
                {
                    foreach (Position to in GameMove.FinishAt)
                    {
                        if (g.IsLegal(from, to))
                        {
                            GameMove mv = new GameMove(m_Game.GetCard(from), from, to);
                            GameState nextState = g + mv;
                            if (nextState.IsWinner())
                            {
                                nextState.Dump();
                                gameQueue.Clear();
                                return;
                            }
                            bool dup = false;
                            foreach(GameState x in gameQueue)
                            {
                                if (nextState.Equals(x))
                                {
                                    dup = true;
                                    break;
                                }
                            }
                            if (!dup)
                            {
                                if(gameQueue.Count > qTrigger)
                                {
                                    qTrigger += 1000;
                                    Console.WriteLine("queueLength {0}", gameQueue.Count);
                                }
                                gameQueue.Enqueue(nextState);
                            }
                        }
                    }
                }

            }
        }

        private void solveWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void solveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
