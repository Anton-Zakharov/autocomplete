using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zakharov.Autocomplete.Model {
    /// <summary>
    /// Частотный словарь
    /// </summary>
    public class Dictionary {
        /// <summary>Максимальное количество слов, выбираемых для автодополнения</summary>
        public int TopCount { get; set; }
        /// <summary>Список уникальных слов из текста с указанием частоты, с которой встречается каждое слово</summary>
        private List<Word> Words = null;
        /// <summary>Стек отдельных списков слов из частотного словаря, начинающихся с определенного фрагмента</summary>
        private Stack<Buffer> Buffers = null;        
        /// <summary>
        /// Создание словаря
        /// </summary>
        /// <param name="topCount">максимальное количество слов, выбираемых для автодополнения</param>
        public Dictionary(int topCount) {
            if (topCount <= 0)
                throw new ArgumentException("Значение количества выбираемых слов должно быть положительным числом", "topCount");
            TopCount = topCount;
            Words = new List<Word>();
            Buffers = new Stack<Buffer>();
        }
        /// <summary>
        /// Добавление слова в частотный словарь
        /// </summary>
        /// <param name="text">слово из текста</param>
        /// <param name="frequency">частота, с которой слово встречается в тексте</param>
        public void AddWord(string text, int frequency) {
            Words.Add(new Word(text, frequency));
        }
        /// <summary>
        /// Плолучение списка наиболее часто встречающихся слов, начинающихся с заданного фрагмента
        /// </summary>
        /// <param name="prefix">фрагмент, с которого начинаются отобранные из словаря слова</param>
        /// <returns>список наиболее часто встречающихся слов, начинающихся с заданного фрагмента</returns>
        /// <remarks>для более быстро поиска при повторных вызовах следует передавать префиксы, упорядоченные по алфавиту</remarks>
        public List<string> GetAutocomplete(string prefix) {
            // Получаем подходящий под переданный префикс список слов из словаря
            // Используем полученные до этого списки слов, начинающихся с начальной части переданного префикса
            Buffer buffer = new Buffer(prefix, FindBuffer(prefix).Data.Where(q => q.Text.StartsWith(prefix)).ToList());
            // Добавляем полученный усеченный список слов для последующих вызово
            Buffers.Push(buffer);
            // Из полученного усеченного списка слов выбираем наиболее часто встречающиеся и сортируем полученные слова вначале по частоте, а затем по словам
            return buffer.Data.OrderByDescending(q => q.Frequency).ThenBy(q => q.Text).Select(q => q.Text).Take(TopCount).ToList();
        }
        /// <summary>
        /// Поиск в стеке подходящего под заданный префикс списка слов
        /// </summary>
        /// <param name="prefix">фрагмент, с которого начинаются отобранные из словаря слова</param>
        /// <returns>список слов, начинающихся с заданного префикса</returns>
        private Buffer FindBuffer(string prefix) {
            if (Buffers.Count == 0)
                // Если стек пустой, то возвращаем список, содержащий все слова из частотного словаря
                return new Buffer(prefix, Words);
            bool found = false;
            Buffer current = null;
            #region Пытаемся в стеке найти список слов, подходящий под заданный префикс
            do {
                current = Buffers.Peek();
                if (prefix.StartsWith(current.Prefix))
                    // Нашли список слов, начинающихся с заданного префикса
                    found = true;
                else
                    // Текущий список слов не подходит под переданный префикс и поэтому удаляется из стека
                    current = Buffers.Pop();
            } while (!found && Buffers.Count > 0);
            #endregion
            if (found)
                // Возвращаем найденный в стеке список слов
                return current;
            else
                // Возвращаем список, содержащий все слова из частотного словаря
                return new Buffer(prefix, Words);
        }
    }
}
