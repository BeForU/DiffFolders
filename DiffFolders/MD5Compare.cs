using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace DiffFolders
{
    class MD5Compare : IEqualityComparer<FileInfo>
    {
        private MD5 _md5_1st;
        private MD5 _md5_2nd;
        private string _hash1;
        private string _hash2;
        FileStream _stream1;
        FileStream _stream2;

        public MD5Compare()
        {
            _md5_1st = MD5.Create();
            _md5_2nd = MD5.Create();
        }

        public bool Equals(FileInfo file1, FileInfo file2)
        {
            if (file1 == file2)
            {
                return true;
            }

            _stream1 = File.OpenRead(file1.FullName);
            _stream2 = File.OpenRead(file2.FullName);

            //파일 크기 체크
            if (_stream1.Length != _stream2.Length)
            {
                _stream1.Close();
                _stream2.Close();

                return false;
            }

            //MD5 체크
            _hash1 = _md5_1st.ComputeHash(_stream1).ToString();
            _hash2 = _md5_2nd.ComputeHash(_stream2).ToString();

            _stream1.Close();
            _stream2.Close();

            return _hash1 == _hash2;
        }

        public int GetHashCode(System.IO.FileInfo fi)
        {
            return fi.Name.GetHashCode();
        }
    }
}
