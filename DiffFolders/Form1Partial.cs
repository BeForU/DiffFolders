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

        private void UpdateIgnoreListBox()
        {
            IgnoreList.Instance.LoadData();

            listBox2.Items.Clear();

            var enumerator = IgnoreList.Instance.ignoreList.GetEnumerator();

            while(enumerator.MoveNext())
            {
                listBox2.Items.Add(enumerator.Current.Key);
            }
        }

        private void SetDirText(int boxNum, DirectoryInfo info)
        {
            string name = info.FullName;
            if (name.EndsWith("\\") == false)
            {
                name += "\\";
            }
            switch (boxNum)
            {
                case 1:
                    textBox1.Text = name;
                    break;
                case 2:
                    textBox2.Text = name;
                    break;
            }
        }

        private bool CheckDirExists(string path1, string path2)
        {
            DirectoryInfo dirInfo;

            dirInfo = new DirectoryInfo(path1);
            if (!dirInfo.Exists) return false;
            _files1 = dirInfo.GetFiles(filterText.Text, SearchOption.AllDirectories);
            SetDirText(1, dirInfo);

            dirInfo = new DirectoryInfo(path2);
            if (!dirInfo.Exists) return false;
            _files2 = dirInfo.GetFiles(filterText.Text, SearchOption.AllDirectories);
            SetDirText(2, dirInfo);

            return true;
        }

        private void CheckFiles()
        {
            //\Assets\Resources\xml\
            //\Assets\MobileF\Resources\Mobile\xml\
            IEnumerator<FileInfo> enumerator;
            //FileCompare fileCompare = new FileCompare();
            NameCompare nameCompare = new NameCompare();

            // 1에만 있는 파일 확인
            IEnumerable<FileInfo> query1 = (from file in _files1
                                            select file).Except(_files2, FileCompare.Instance);

            // 2에만 있는 파일 확인
            IEnumerable<FileInfo> query2 = (from file in _files2
                                            select file).Except(_files1, FileCompare.Instance);

            // 양쪽에 다 존재하지만 내용이 다름. 목록은 1번 기준.
            IEnumerable<FileInfo> querySame = query1.Intersect(query2, nameCompare);

            enumerator = querySame.GetEnumerator();
            string strPath;

            while (enumerator.MoveNext())
            {
                strPath = enumerator.Current.FullName.ToString().Substring(textBox1.Text.Length);
                if(CheckIgnoreList(strPath))
                {
                    continue;
                }
                else
                {
                    listBox1.Items.Add("(내용 다름)\t" + strPath);
                }
            }

            query1 = (from file in query1 select file).Except(querySame, nameCompare);
            enumerator = query1.GetEnumerator();
            while (enumerator.MoveNext())
            {
                strPath = enumerator.Current.FullName.ToString();
                if (CheckIgnoreList(strPath.Substring(textBox1.Text.Length)))
                {
                    continue;
                }
                else
                {
                    listBox1.Items.Add("(경로1에만 있음)\t" + strPath);
                }
            }

            query2 = (from file in query2 select file).Except(querySame, nameCompare);
            enumerator = query2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                strPath = enumerator.Current.FullName.ToString();
                if (CheckIgnoreList(strPath.Substring(textBox2.Text.Length)))
                {
                    continue;
                }
                else
                {
                    listBox1.Items.Add("(경로2에만 있음)\t" + strPath);
                }
            }
        }

        private bool CheckIgnoreList(string path)
        {
            string[] md5s = MakeMD5Array(path);

            return IgnoreList.Instance.IgnoreListExists(path, md5s);
        }

        private void SetIgnoreList(string path)
        {
            if (_selectedPath != null)
            {
                path = _selectedPath;
            }

            string[] md5s = MakeMD5Array(path);
            IgnoreList.Instance.AddIgnoreList(path, md5s);
            UpdateIgnoreListBox();
        }

        private void RemoveIgnoreList(string path)
        {
            string[] md5s = MakeMD5Array(path);
            IgnoreList.Instance.RemoveListExists(path);
            UpdateIgnoreListBox();
        }
        private string[] MakeMD5Array(string path)
        {
            string[] md5s = { "", "" };

            FileInfo fileInfo = new FileInfo(textBox1.Text + path);
            if (fileInfo.Exists)
            {
                md5s[0] = FileCompare.Instance.GetMD5(fileInfo.FullName);
            }

            fileInfo = new FileInfo(textBox2.Text + path);
            if (fileInfo.Exists)
            {
                md5s[1] = FileCompare.Instance.GetMD5(fileInfo.FullName);
            }

            return md5s;
        }

        private bool SetSelectedMenuItem(object menuItem)
        {
            // 다를때
            _selectedPath = null;
            string itemText = menuItem.ToString();
            if (itemText.Contains("(내용 다름)"))
            {
                _selectedMenuItem = menuItem.ToString().Substring(8);
                return true;
            }

            // 한쪽에만 있을 때
            string path = menuItem.ToString().Substring(11);
            string fileName = Path.GetFileName(path);
            int dirLength = path.Length - fileName.Length;

            if (itemText.Contains("(경로1"))
            {
                dirLength -= textBox1.Text.Length;
                _selectedPath = path.Substring(textBox1.Text.Length);
                _selectedMenuItem = path.Substring(textBox1.Text.Length, dirLength);
            }
            else if (itemText.Contains("(경로2"))
            {
                dirLength -= textBox2.Text.Length;
                _selectedPath = path.Substring(textBox2.Text.Length);
                _selectedMenuItem = path.Substring(textBox2.Text.Length, dirLength);
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
