using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorySearch
{
    public class DS
    {
        /// <summary>
        /// Выводит список находящихся внутри каталогов
        /// </summary>
        /// <param name="path">Путь до исследуемой папки, к примеру: C:\Windows</param>
        /// <returns></returns>
        public static string[] dirList(string path)
        {
            string[] allFoundFiles = Directory.GetDirectories(path);
            return allFoundFiles;
        }

        /// <summary>
        /// Поиск подкаталогов в каталоге и их удаление
        /// </summary>
        /// <param name="path">Путь до исследуемой папки, к примеру: C:\Windows</param>
        /// <param name="text">Ключевое слово, по которому будет производиться поиск</param>
        /// <returns></returns>
        public static void RemoveSearch(string path, string text)
        {
            
            string[] allFoundFiles = Directory.GetDirectories(path);

            if (!allFoundFiles.Contains(text))
            {
                string[] newPath = parcer(allFoundFiles, text);
                if (newPath != null)
                {
                    foreach (var a in newPath)
                    {
                        if (a.Contains(text))
                        {
                            Directory.Delete(a, true);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Directories not found!");
                }
            }

        }

        private static string[] parcer(string[] args, string text)
        {
            bool checker = false;
            string[] path = null;
            foreach (var a in args)
            {
                if (checker == true) goto end;
                string[] allFoundFiles = Directory.GetDirectories(a);
                if (allFoundFiles.Contains(text))
                {
                    checker = true;
                    path = allFoundFiles;
                }
            end:;
            }
            return path;
        }

        /// <summary>
        /// Путь до папки AppData\LocalLow
        /// </summary>
        /// <returns></returns>
        public static string localLowAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        /// <summary>
        /// Путь до папки AppData\Roaming
        /// </summary>
        /// <returns></returns>
        public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Путь до папки AppData\Local
        /// </summary>
        /// <returns></returns>
        public static string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }
}
