using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

//https://msdn.microsoft.com/en-us/library/mt693036.aspx
namespace DiffFolders
{
    public enum RESULT
    {
        Same = 1,
        Different = 0,
        Exist1Only,
        Exist2Only,
    }

    class FileCompare : IEqualityComparer<FileInfo>
	{
        private MD5 _md5_1st;
        private MD5 _md5_2nd;
        private string _hash1;
        private string _hash2;
        FileStream _stream1;
        FileStream _stream2;

        public FileCompare()
        {
            _md5_1st = MD5.Create();
            _md5_2nd = MD5.Create();
        }

        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.Length == f2.Length);
        }

        // Return a hash that reflects the comparison criteria. According to the   
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
        // also be equal. Because equality as defined here is a simple value equality, not  
        // reference identity, it is possible that two or more objects will produce the same  
        // hash code.  

        public int GetHashCode(System.IO.FileInfo fi)
        {
            string s;
            using (FileStream stream = File.OpenRead(fi.FullName))
            {
                s = _md5_1st.ComputeHash(stream).ToString();
            }
            return s.GetHashCode();
        }


        public RESULT MD5Compare(FileInfo file1, FileInfo file2)
        {
            if (file1 == file2)
            {
                return RESULT.Same;
            }

            _stream1 = File.OpenRead(file1.FullName);
            _stream2 = File.OpenRead(file2.FullName);

            //파일 크기 체크
            if (_stream1.Length != _stream2.Length)
            {
                _stream1.Close();
                _stream2.Close();

                return RESULT.Different;
            }

            //MD5 체크
            _hash1 = _md5_1st.ComputeHash(_stream1).ToString();
            _hash2 = _md5_2nd.ComputeHash(_stream2).ToString();

            _stream1.Close();
            _stream2.Close();

            return _hash1 == _hash2 ? RESULT.Same : RESULT.Different;
        }
    }
}