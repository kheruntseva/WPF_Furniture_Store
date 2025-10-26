using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace mywpf
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Cursors",
                "Turquoise1.cur");

            Console.WriteLine($"Путь к курсору: {fullPath}");
            Console.WriteLine($"Файл существует: {File.Exists(fullPath)}");

            if (File.Exists(fullPath))
            {
                try
                {
                    Mouse.OverrideCursor = new Cursor(fullPath);
                    Console.WriteLine("Курсор успешно загружен");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки: {ex.Message}");
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
            else
            {
                Console.WriteLine("Файл курсора не найден!");
                Mouse.OverrideCursor = Cursors.Arrow;
            }

            base.OnStartup(e);
        }

    }
}
