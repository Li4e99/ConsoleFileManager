using ConsoleShortPathName;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFileManager
{
    class Program
    {
        const int WINDOW_HEGHT = 50;
        const int WINDOW_WIDTH = 120;
        private static string currentDir = Properties.Settings.Default.LastPath;

        static void Main()
        {
            
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "ConsoleFileManager";
            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEGHT);
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEGHT);
            DrawWindow(0, 0, WINDOW_WIDTH, 38);
            DrawWindow(0, 38, WINDOW_WIDTH, 8);
            bool b = true;
            try
            {
                while (b)
                {
                    Console.SetCursorPosition(2, 39);
                    Console.WriteLine("Для работы с консольным менеджером нажмите 1.");
                    Console.SetCursorPosition(2, 40);
                    Console.WriteLine("Для выхода из консольного менеджера в меню введите exit.");
                    Console.SetCursorPosition(2, 41);
                    Console.Write("Чтобы завершить работу программу введите 0: ");
                    int TaskNumber = Convert.ToInt32(Console.ReadLine());
                    DrawWindow(0, 38, WINDOW_WIDTH, 8);
                    switch (TaskNumber)
                    {
                        case 1:
                            UpdateConsole();
                            break;
                        case 0:
                            b = false;
                            Console.SetCursorPosition(2, 39);
                            Console.WriteLine("Завершение работы приложения...");
                            Console.ReadKey(true);
                            Process.GetCurrentProcess().Kill();
                            break;
                        default:
                            DrawWindow(0, 38, WINDOW_WIDTH, 8);
                            Console.SetCursorPosition(2, 39);
                            Console.WriteLine("Повторите ввод.");
                            Console.ReadKey(true);
                            break;
                    }
                }
            }
            catch (Exception errors)
            {
                DateTime date = DateTime.Now;
                DrawWindow(0, 38, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 39);
                Console.WriteLine("Обнаружена ошибка: " + errors.Message);
                Console.ReadKey(true);
                if (!File.Exists($@"{Environment.CurrentDirectory}\random-name-exception.txt"))
                {
                    File.Create($@"{Environment.CurrentDirectory}\random-name-exception.txt");
                }
                File.AppendAllText($@"{Environment.CurrentDirectory}\random-name-exception.txt", ($"{date} {errors.Message} \n"));
            }
            finally
            {
                UpdateConsole();
                DrawWindow(0, 38, WINDOW_WIDTH, 8);
                Main();
            }
        }
        /// <summary>
        /// Метод для отрисовки окна
        /// </summary>
        /// <param name="x"> точка по оси Х</param>
        /// <param name="y">точка по оси Y</param>
        /// <param name="width">ширина окна</param>
        /// <param name="height"> высота окна</param>
        static void DrawWindow(int x, int y, int width, int height)
        {
            //шапка
            Console.SetCursorPosition(x, y);
            Console.Write("┌");
            for (int i = 0; i < width - 2; i++)
                Console.Write("─");
            Console.Write("┐");
            //окно
            Console.SetCursorPosition(x, y + 1);
            for (int i = 0; i < height - 2; i++)
            {
                Console.Write("│");
                for (int j = x + 1; j < x + width - 1; j++)
                    Console.Write(" ");
                Console.Write("│");
            }
            //подвал
            Console.Write("└");
            for (int i = 0; i < width - 2; i++)
                Console.Write("─");
            Console.Write("┘");
            Console.SetCursorPosition(x, y);
        }
        /// <summary>
        /// Метод для отрисовки консоли
        /// </summary>
        /// <param name="dir">текущая директория</param>
        /// <param name="x"> точка по оси Х</param>
        /// <param name="y">точка по оси Y</param>
        /// <param name="width">ширина окна</param>
        /// <param name="height"> высота окна</param>
        static void DrawConsole(string dir, int x, int y, int width, int height)
        {
            DrawWindow(x, y, width, height);
            Console.SetCursorPosition(x + 1, y + height / 2);
            Console.Write($"{dir}>");
        }
        /// <summary>
        /// Метод для обновления ввода консоли
        /// </summary>
        static void UpdateConsole()
        {
            DrawConsole(GetShortPathName(currentDir), 0, 46, WINDOW_WIDTH, 3);
            ProcessEnterCommand(WINDOW_WIDTH);
        }
        /// <summary>
        /// Вспомогательный метод для получения позиции курсора
        /// </summary>
        /// <returns></returns>
        static (int left, int top) GetCursorPosition()
        {
            return (Console.CursorLeft, Console.CursorTop);

        }
        /// <summary>
        /// Метод для обработки процесса ввода консоли
        /// <param name="width">длина строки ввода</param>
        /// </summary>
        static void ProcessEnterCommand(int width)
        {
            (int left, int top) = GetCursorPosition();
            StringBuilder command = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            char key;
            do
            {
                keyInfo = Console.ReadKey();
                key = keyInfo.KeyChar;
                if (keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Backspace &&
                    keyInfo.Key != ConsoleKey.UpArrow)
                    command.Append(key);
                (int currentLeft, int currentTop) = GetCursorPosition();

                if (currentLeft == width - 2)
                {
                    Console.SetCursorPosition(currentLeft - 1, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, top);
                }

                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (command.Length > 0)
                        command.Remove(command.Length - 1, 1);
                    if (currentLeft >= left)
                    {
                        Console.SetCursorPosition(currentLeft, top);
                        Console.Write(" ");
                        Console.SetCursorPosition(currentLeft, top);
                    }
                    else
                    {
                        command.Clear();
                        Console.SetCursorPosition(left, top);
                    }
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);
            ParseCommandString(command.ToString());


        }
        /// <summary>
        /// Метод для обработки команд
        /// </summary>
        /// <param name="command">Вводимая команда</param>
        private static void ParseCommandString(string command)
        {
            string[] commandParams = command.ToLower().Split(' ');
            
            if (commandParams.Length > 0)
            {
                bool b = true;
                while (b)
                {
                    switch (commandParams[0])
                    {
                        case "cd":
                            if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            {
                                currentDir = commandParams[1];
                            }
                            else if (commandParams.Length > 1 && !Directory.Exists(commandParams[1]))
                            {
                                commandParams = command.ToLower().Split('\"');
                                currentDir = commandParams[1].Trim();
                            }
                            UpdateConsole();
                            break;
                        case "ls":
                            if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            {
                                if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), n);
                                  
                                }
                                else
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), 1);
                                   
                                }
                            }
                            else if (commandParams.Length > 1 && !Directory.Exists(commandParams[1]))
                            {
                                commandParams = command.ToLower().Split('\"');
                                if (commandParams.Length > 3 && commandParams[2].Trim() == "-p" && int.TryParse(commandParams[3].Trim(), out int n))
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), n);
                                   
                                }
                                else
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), 1);
                                   
                                }
                            }
                            UpdateConsole();
                            break;

                        case "cp":
                            if (commandParams.Length > 2)
                            {
                                if (File.Exists(commandParams[1]))
                                {
                                    if (Directory.Exists(commandParams[2]))
                                    {
                                        File.Create(commandParams[2]);
                                        File.Copy(commandParams[1], commandParams[2], true);
                                    }
                                    else if (!Directory.Exists(commandParams[2]))
                                    {
                                        Directory.CreateDirectory(commandParams[2]);
                                        File.Create(commandParams[2]);
                                        File.Copy(commandParams[1], commandParams[2], true);
                                    }
                                }
                                else if (Directory.Exists(commandParams[1]))
                                {
                                    if (!Directory.Exists(commandParams[2]))
                                    {
                                        Directory.CreateDirectory(commandParams[2]);
                                    }
                                    else
                                    {
                                        CopyDir(commandParams[1], commandParams[2], true);
                                    }
                                }
                            }
                            else if (commandParams.Length > 2)
                            {
                                commandParams = command.ToLower().Split('\"');
                                if (File.Exists(commandParams[1].Trim()))
                                {

                                    if (Directory.Exists(commandParams[1].Trim()))
                                    {
                                        File.Create(commandParams[3].Trim());
                                        File.Copy(commandParams[1].Trim(), commandParams[3].Trim(), true);
                                    }

                                    else if (!Directory.Exists(commandParams[3].Trim()))
                                    {
                                        Directory.CreateDirectory(commandParams[3].Trim());
                                        File.Create(commandParams[3].Trim());
                                        File.Copy(commandParams[1].Trim(), commandParams[3].Trim(), true);
                                    }
                                }
                                else if (Directory.Exists(commandParams[1].Trim()))
                                {
                                    if (!Directory.Exists(commandParams[3].Trim()))
                                    {
                                        Directory.CreateDirectory(commandParams[3].Trim());
                                        CopyDir(commandParams[1].Trim(), commandParams[3].Trim(), true);
                                    }
                                    else
                                    {
                                        CopyDir(commandParams[1].Trim(), commandParams[3].Trim(), true);
                                    }
                                }
                            }
                            UpdateConsole();
                            break;

                        case "file":
                            if (commandParams.Length > 1)
                            {
                                if (File.Exists(commandParams[1]))
                                {
                                    DrawWindow(0, 38, WINDOW_WIDTH, 8);
                                    Info(new FileInfo(commandParams[1]));
                                }
                                else if (Directory.Exists(commandParams[1]))
                                {
                                    DrawWindow(0, 38, WINDOW_WIDTH, 8);
                                    DIRInfo(new DirectoryInfo(commandParams[1]));
                                }
                            }
                            else
                            {
                                commandParams = command.ToLower().Split('\"');
                                if (File.Exists(commandParams[1].Trim()))
                                {
                                    DrawWindow(0, 38, WINDOW_WIDTH, 8);
                                    Info(new FileInfo(commandParams[1].Trim()));
                                }
                                else if (Directory.Exists(commandParams[1].Trim()))
                                {
                                    DrawWindow(0, 38, WINDOW_WIDTH, 8);
                                    DIRInfo(new DirectoryInfo(commandParams[1].Trim()));
                                }
                            }
                            UpdateConsole();
                            break;
                        case "rm":
                            if (commandParams.Length > 1)
                            {
                                if (File.Exists(commandParams[1]))
                                {
                                    File.Delete(commandParams[1]);
                                }
                                else if (Directory.Exists(commandParams[1]))
                                {
                                    Directory.Delete(commandParams[1], true);
                                }
                            }
                            UpdateConsole();
                            break;
                        case "exit":
                            b = false;
                            GC.Collect();
                            DrawWindow(0, 46, WINDOW_WIDTH, 3);
                            Main();
                            UpdateConsole();
                            break;
                        default:
                            GC.Collect();
                            UpdateConsole();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Метод для копирования директории
        /// </summary>
        /// <param name="fromDir">Начальная директория</param>
        /// <param name="toDir">Конечная директория</param>
        /// <param name="recursive"></param>
        static void CopyDir (string fromDir, string toDir, bool recursive)
        {
            var dir = new DirectoryInfo(fromDir);
                if (!dir.Exists)
                {
                    return;
                }
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(toDir);
                foreach (FileInfo file in dir.GetFiles())
                {
                    string targetFilePath = Path.Combine(toDir, file.Name);
                    file.CopyTo(targetFilePath);
                }

                if (recursive)
                {
                    foreach (DirectoryInfo subDir in dirs)
                    {
                        string newDestinationDir = Path.Combine(toDir, subDir.Name);
                        CopyDir(subDir.FullName, newDestinationDir, true);
                    }
                }
        }
            
        /// <summary>
        /// Метод для укорачивания пути директории
        /// </summary>
        /// <param name="path"> текущая директория</param>
        /// <returns></returns>
        static string GetShortPathName(string path)
        {
            StringBuilder shortPathName = new StringBuilder((int)API.MAX_PATH);
            API.GetShortPathName(path, shortPathName, API.MAX_PATH);
            return shortPathName.ToString();
        }

        /// <summary>
        /// Метод для получения дерева файлов
        /// </summary>
        /// <param name="tree">запись в динамическую строку</param>
        /// <param name="dir">текущая директория</param>
        /// <param name="indent">пробел</param>
        /// <param name="lastDirectory">проверка на крайность директории</param>
        static void GetTree(StringBuilder tree, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            
            tree.Append(indent);
            if (lastDirectory)
            {
                tree.Append("└─");
                indent += " ";
            }
            else
            {
                tree.Append("├─");
            }
            tree.Append($"{dir.Name}\n");

            FileInfo[] subFiles = dir.GetFiles();

            for (int i = 0; i < subFiles.Length; i++)
            {
                if (i == subFiles.Length)
                {
                    tree.Append($"{indent}└─{subFiles[i].Name}\n ");
                }
                else
                {
                    tree.Append($"{indent}├─{subFiles[i].Name}\n");
                }
                DirectoryInfo[] subDirects = dir.GetDirectories();
                for (int j = 0; j < subDirects.Length; j++)
                    GetTree(tree, subDirects[j], indent, j == subDirects.Length - 1);
            }
        }
        /// <summary>
        /// Метод для отрисовки дерева
        /// </summary>
        /// <param name="dir"> текущая директория</param>
        /// <param name="page">количсество отрображаемых строк на странице</param>
       static void DrawTree(DirectoryInfo dir, int page)
       {
            StringBuilder tree = new StringBuilder();
            GetTree(tree, dir, "", true);
            DrawWindow(0, 0, WINDOW_WIDTH, 38);
            (int currentLeft, int currentTop) = GetCursorPosition();
            int pageLines = 36;
            string[] lines = tree.ToString().Split('\n');
            int pageTotal = (lines.Length + pageLines - 1) / pageLines;
            if (page > pageTotal)
                page = pageTotal; 
            for (int i = (page-1)*pageLines, counter = 0; i<page*pageLines; i++, counter++)
            {
                if (lines.Length-1>i)
                {
                    Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                    Console.WriteLine(lines[i]);
                }
            }
            //Отрисовка количества страниц
            string footer = $"┤ {page} of {pageTotal} ├";
            Console.SetCursorPosition(WINDOW_WIDTH / 2 - footer.Length / 2, 37);
            Console.WriteLine(footer);
       }
        ///Метод для вывода информации  файле
        static void Info(FileInfo file)
        {
            Console.SetCursorPosition(2, 39);
            Console.Write("Аттрибуты файла\t");
            Console.WriteLine(file.Attributes);
            Console.SetCursorPosition(2, 40);
            Console.Write("Время создания файла\t");
            Console.WriteLine(file.CreationTime);
            Console.SetCursorPosition(2, 41);
            Console.Write("Время последнего доступа к файлу\t");
            Console.WriteLine(file.LastAccessTime);
            Console.SetCursorPosition(2, 43);
            Console.Write("Размер файла в байтах\t");
            Console.WriteLine(file.Length);

        }
        // Метод для вывода информации о папке
        static void DIRInfo(DirectoryInfo dir)
        {
            Console.SetCursorPosition(2, 39);
            Console.Write("Аттрибуты директории\t");
            Console.WriteLine(dir.Attributes);
            Console.SetCursorPosition(2, 40);
            Console.Write("Время создания директории\t");
            Console.WriteLine(dir.CreationTime);
            Console.SetCursorPosition(2, 41);
            Console.Write("Время последнего доступа к каталогу\t");
            Console.WriteLine(dir.LastAccessTime);
            Console.SetCursorPosition(2, 43);
            Console.Write("Путь к директории\t");
            Console.WriteLine(dir.FullName);
        }
    } 
}
