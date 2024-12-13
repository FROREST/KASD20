using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
public enum VariableType
{
    Int,
    Float,
    Double
}
class Program
{
    static void Main(string[] args)
    {

        string inputFilePath = "/Users/liga/Desktop/Учебники/Задачи KASD/KASD20/KASD20/input.txt";
        string outputFilePath = "/Users/liga/Desktop/Учебники/Задачи KASD/KASD20/KASD20/output.txt";

        // Регулярное выражение для поиска определения переменной
        string pattern = @"(int|float|double)\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(\d+);";
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Хеш-таблица для хранения переменных
        MyHashMap<string, (VariableType, string)> variables = new MyHashMap<string, (VariableType, string)>();

        // Список для некорректных определений
        List<string> invalidDefinitions = new List<string>();

        // Проверка наличия файла
        if (File.Exists(inputFilePath))
        {
            // Чтение строк из файла
            string[] lines = File.ReadAllLines(inputFilePath);
            string line = string.Join(" ", lines); // Объединяем строки в одну

            // Поиск всех определений в строке
            MatchCollection matches = regex.Matches(line);

            foreach (Match match in matches)
            {
                string type = match.Groups[1].Value.ToLower();
                string variableName = match.Groups[2].Value;
                string value = match.Groups[3].Value;

                // Проверка на корректность типа
                VariableType varType = VariableType.Int;
                if (type == "float") varType = VariableType.Float;
                else if (type == "double") varType = VariableType.Double;
               

                // Проверка на переопределение переменной
                else if (variables.ContainsKey(variableName))
                {
                    // Если переменная уже существует, выводим сообщение о переопределении
                    Console.WriteLine($"Переменная {variableName} уже определена. Оставлена первая версия.");
                    continue;
                }
                else Console.WriteLine($"ОШИБКА! Есть неопределённые переменные"); 

                // Добавление переменной в хеш-таблицу
                variables.Put(variableName, (varType, value));
            }

            // Сохранение результатов в файл
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (var entry in variables.EntrySet())
                {
                    string typeString = entry.Value.Item1.ToString().ToLower();
                    string output = $"{typeString} => {entry.Key}({entry.Value.Item2})";
                    writer.WriteLine(output);
                }
            }

            Console.WriteLine("Результат записан в файл output.txt.");
        }
        else
        {
            Console.WriteLine($"Файл {inputFilePath} не найден.");
        }
    }
}