using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DiffFolders
{
    public partial class Form1 : Form
	{
        private string _selectedMenuItem;
        private string _selectedPath;
        private readonly ContextMenuStrip list1MenuStrip;
        private readonly ContextMenuStrip list2MenuStrip;

        public Form1()
		{
			InitializeComponent();

            UpdateIgnoreListBox();
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            listBox1.MouseDown += listBox1_MouseDown;

            var toolStripMenuItem1 = new ToolStripMenuItem { Text = "WinMerge 실행" };
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            var toolStripMenuItem2 = new ToolStripMenuItem { Text = "무시 목록에 추가" };
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            list1MenuStrip = new ContextMenuStrip();
            list1MenuStrip.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2 });
            
            var toolStripMenuItem3 = new ToolStripMenuItem { Text = "무시 목록에서 제거" };
            toolStripMenuItem3.Click += toolStripMenuItem3_Click;

            listBox2.MouseDown += listBox2_MouseDown;
            list2MenuStrip = new ContextMenuStrip();
            list2MenuStrip.Items.AddRange(new ToolStripItem[] { toolStripMenuItem3 });
        }

        //비교버튼 누름
        private void button1_Click(object sender, System.EventArgs e)
		{
            listBox1.Items.Clear();

            if (CheckDirExists(textBox1.Text, textBox2.Text) == false)
			{
				MessageBox.Show("경로가 올바르지 않습니다.");
                return;
			}

            CheckFiles();
        }

        void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                SetSelectedMenuItem(listBox1.Items[index]);
                OpenWinMerge();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenWinMerge();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetIgnoreList(_selectedMenuItem);
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            RemoveIgnoreList(_selectedMenuItem);
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                listBox1.SetSelected(index, true);
                SetSelectedMenuItem(listBox1.Items[index]);
              
                list1MenuStrip.Show(Cursor.Position);
                list1MenuStrip.Visible = true;
            }
            else
            {
                list1MenuStrip.Visible = false;
            }
        }

        private void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var index = listBox2.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                listBox2.SetSelected(index, true);
                _selectedMenuItem = listBox2.Items[index].ToString();

                list2MenuStrip.Show(Cursor.Position);
                list2MenuStrip.Visible = true;
            }
            else
            {
                list2MenuStrip.Visible = false;
            }
        }
    }
}