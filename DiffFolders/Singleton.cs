using System;
using System.Reflection;

//https://codereview.stackexchange.com/questions/10554/a-generic-singleton
namespace DiffFolders
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

        public static T Instance { get { return instance.Value; } }
    }
}
