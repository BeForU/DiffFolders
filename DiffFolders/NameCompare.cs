using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffFolders
{
    class NameCompare : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo file1, FileInfo file2)
        {
            return file1.Name.Equals(file2.Name);
        }
        public int GetHashCode(System.IO.FileInfo fi)
        {
            return fi.Name.GetHashCode();
        }
    }
}
