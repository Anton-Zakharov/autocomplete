using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zakharov.Autocomplete.Model;

namespace Zakharov.Autocomplete.Console {
    class Program {
        static void Main(string[] args) {           
            Dictionary dictionary = new Dictionary(10);
            // Получаем количество слов в частотном словаре
            int dictionaryCount = Int32.Parse(System.Console.ReadLine());
            #region Заполнение частотного словаря
            for (int i = 0; i < dictionaryCount; i++) {
                string[] word = System.Console.ReadLine().Split(' ');
                if (word.Length != 2)
                    throw new Exception("Входная строка частотного словаря должна иметь вид: слово частота");
                if (word[0].Length > 15)
                    throw new Exception("Слово должно содержать не более 15 символов");
                int frequency = 0;
                if (!Int32.TryParse(word[1], out frequency) || frequency <= 0)
                    throw new Exception("Частота должна быть целым положительным числом");
                dictionary.AddWord(word[0], frequency);
            }
            #endregion
            // Получаем количество префиксов
            int prefixCount = Int32.Parse(System.Console.ReadLine());
            List<string> prefixes = new List<string>();
            #region Заполнение списка префиксов
            for (int i = 0; i < prefixCount; i++) {
                string prefix = System.Console.ReadLine();
                if (!prefixes.Contains(prefix))
                    // Добавляем префиксы без повторов
                    prefixes.Add(prefix);
            }
            #endregion
            // Сортируем список префиксов по алфавиту для ускорения поиска автодополнений
            prefixes.Sort();
            foreach (string prefix in prefixes) {
                // Выводим пустую строку для визуального разделения результатов автодополнения
                System.Console.WriteLine();
                foreach (string autocomplete in dictionary.GetAutocomplete(prefix)) {
                    System.Console.WriteLine(autocomplete);
                }
            }            
        }
    }
}
