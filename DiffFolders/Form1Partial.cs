using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DiffFolders
{
    partial class Form1
    {
        private IEnumerable<FileInfo> _files1;
        private IEnumerable<FileInfo> _files2;

        private bool CheckDirExists(string path1, string path2)
        {
            DirectoryInfo dirInfo;

            dirInfo = new DirectoryInfo(path1);
            if (!dirInfo.Exists) return false;
            _files1 = dirInfo.GetFiles(filterText.Text, SearchOption.AllDirectories);

            dirInfo = new DirectoryInfo(path2);
            if (!dirInfo.Exists) return false;
            _files2 = dirInfo.GetFiles(filterText.Text, SearchOption.AllDirectories);

            return true;
        }

        private void CheckFiles()
        {
            //\Assets\Resources\xml\
            //\Assets\MobileF\Resources\xml\
            IEnumerator<FileInfo> enumerator;
            FileCompare fileCompare = new FileCompare();
            MD5Compare md5Compare = new MD5Compare();
            NameCompare nameCompare = new NameCompare();

            // 1에만 있는 파일 확인
            IEnumerable<FileInfo> query1 = (from file in _files1
                                            select file).Except(_files2, fileCompare);

            // 2에만 있는 파일 확인
            IEnumerable<FileInfo> query2 = (from file in _files2
                                            select file).Except(_files1, fileCompare);

            // 양쪽에 다 존재하지만 내용이 다름. 목록은 1번 기준.
            IEnumerable<FileInfo> querySame = query1.Intersect(query2, nameCompare);

            enumerator = querySame.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string pathString = enumerator.Current.FullName.ToString().Substring(textBox1.Text.Length + 1);
                CheckIgnoreList(pathString);
                listBox1.Items.Add("(내용 다름)\t" + pathString);
            }

            query1 = (from file in query1 select file).Except(querySame, nameCompare);
            enumerator = query1.GetEnumerator();
            while (enumerator.MoveNext())
            {
                listBox1.Items.Add("(경로1에만 있음)\t" + enumerator.Current.FullName.ToString());
            }

            query2 = (from file in query2 select file).Except(querySame, nameCompare);
            enumerator = query2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                listBox1.Items.Add("(경로2에만 있음)\t" + enumerator.Current.FullName.ToString());
            }
        }

        private void CheckIgnoreList(string path)
        {
        }

        private void SetIgnoreList(string pathString)
        {
            MessageBox.Show(pathString);
        }

        private bool SetSelectedMenuItem(object menuItem)
        {
            string itemText = menuItem.ToString();
            if (itemText.Contains("(내용 다름)"))
            {
                _selectedMenuItem = menuItem.ToString().Substring(8);
                return true;
            }

            string path = menuItem.ToString().Substring(11);
            string fileName = Path.GetFileName(path);
            int dirLength = path.Length - fileName.Length;

            if (itemText.Contains("(경로1"))
            {
                dirLength -= this.textBox1.Text.Length;
                _selectedMenuItem = Path.GetFileName(path.Substring(this.textBox1.Text.Length , dirLength));
            }
            else if (itemText.Contains("(경로2"))
            {
                dirLength -= this.textBox2.Text.Length;
                _selectedMenuItem = Path.GetFileName(path.Substring(this.textBox2.Text.Length, dirLength));
            }

            Console.WriteLine(_selectedMenuItem);
            return false;
        }

        private void OpenWinMerge()
        {
            string parmaters = textBox1.Text + "/" + _selectedMenuItem + " " + textBox2.Text + "/" + _selectedMenuItem;
            System.Diagnostics.Process.Start(winMergePath.Text + "/WinMergeU.exe", parmaters);
        }
    }
}
