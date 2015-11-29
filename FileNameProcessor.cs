﻿using System.IO;

namespace QuickRename
{
    public class FileNameProcessor
    {
        public string[] UpperCaseExceptions = { "HD", "HQ", "SD" };

        private MainViewModel model;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        public FileNameProcessor(MainViewModel viewModel)
        {
            model = viewModel;
        }

        /// <summary>
        /// Make each word to start with upper case
        /// </summary>
        private string ChangeToUpperCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string[] words = text.Split(' ');

            string result = string.Empty;

            foreach (var word in words)
            {
                if (word.Length > 0)
                {
                    if (UpperCaseExceptions.IsStringInArray(word, true))
                        result += word + ' ';
                    else
                        result += char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant() + ' ';
                }
            }

            return result.Substring(0, result.Length - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string ApplyRules(string file)
        {
            if (model.ShowFullPath && model.ShowExtension)
            {
                return file;
            }
            else if (model.ShowFullPath)
            {
                return Path.GetDirectoryName(file) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file);
            }
            else if (model.ShowExtension)
            {
                return Path.GetFileName(file);
            }
            else
            {
               return Path.GetFileNameWithoutExtension(file);
            }
        }

        /// <summary>
        /// Get new file name
        /// </summary>
        public string QRename(string file)
        {
            string newStr;
            bool isDirectory = Directory.Exists(file);

            FileInfo fi = new FileInfo(file);
            string directory = fi.DirectoryName;

            file = Path.GetFileNameWithoutExtension(file);

            char separator = FindSeparator(file);

            switch (separator)
            {
                case '-':
                    newStr = ProcessHyphen(file);
                    break;

                case '.':
                    int bodkaIndex = file.LastIndexOf('.');
                    newStr = file.Replace('.', ' ');
                    if (!isDirectory && bodkaIndex >= 0)
                    {
                        newStr = newStr.ReplaceChar(bodkaIndex, '.');
                    }
                    break;

                case ' ':
                    newStr = file;
                    break;

                case '%':
                    newStr = file.Replace("%20", " ");
                    break;

                default:
                    newStr = ProcessDefault(file, separator);
                    break;
            }

            if (model.StartWithUpperCase)
            {
                newStr = ChangeToUpperCase(newStr);
            }

            newStr = PostProcess(newStr);

            return new FileInfo(directory + "\\" + newStr + fi.Extension).FullName;
        }

        private string ProcessHyphen(string file)
        {
            string newStr = string.Empty;

            for (int j = 0; j < file.Length; j++)
            {
                if (file[j] == '-')
                {
                    if ((j > 0) && (file[j - 1] != newStr[newStr.Length - 1]))
                        newStr += '-';
                    else
                        newStr += ' ';
                }
                else
                {
                    newStr += file[j];
                }
            }
            return newStr;
        }

        private string ProcessDefault(string file, char separator)
        {
            string newStr = file.Replace(separator, ' ');

            if (newStr.Contains("-"))
            {
                for (int j = 1; j < newStr.Length - 1; j++)
                {
                    if ((newStr[j] == '-') && (newStr[j - 1] != ' ') && (newStr[j + 1] != ' '))
                    {
                        newStr = newStr.Insert(j, " ").Insert(j + 2, " ");
                        j++;
                    }
                }
            }
            return newStr;
        }

        private char FindSeparator(string str)
        {
            int hyphens = 0;
            int dots = 0;
            int perc20s = 0;

            if (str.Contains(" "))
                return ' ';

            for (int i = 0; i < str.Length; i++)
                switch (str[i])
                {
                    case '_':
                        return '_';

                    case '.':
                        dots++;
                        break;

                    case '-':
                        hyphens++;
                        break;

                    case '0': // "%20"
                        if ((i > 1) && (str[i - 1] == '2') && (str[i - 2] == '%'))
                            perc20s++;
                        break;
                }

            int[] array = { hyphens, dots, perc20s };
            char[] separators = { ' ', '-', '.', '%' };

            int index = Utility.Max(array) + 1;

            return separators[index];
        }

        private string PostProcess(string file)
        {
            string newFile = string.Empty;

            bool lastSpace = false;

            foreach (char s in file)
            {
                if (s == ' ')
                {
                    if (lastSpace) // next
                        continue;

                    lastSpace = true; // first
                }
                else
                {
                    lastSpace = false; // none
                }

                newFile += s;
            }

            return newFile;
        }
    }
}