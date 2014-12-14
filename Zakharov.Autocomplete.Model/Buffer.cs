using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zakharov.Autocomplete.Model {
    /// <summary>
    /// Отобранные из словаря слова, начинающиеся с заданного фрагмента
    /// </summary>
    internal class Buffer {
        /// <summary>Фрагмент, с которого начинаются отобранные из словаря слова</summary>
        public string Prefix { get; set; }
        /// <summary>Список всех слов из словаря, начинающиееся с заданного фрагмента</summary>
        public List<Word> Data { get; set; }
        /// <summary>
        /// Создание объекта
        /// </summary>
        /// <param name="prefix">фрагмент, с которого начинаются отобранные из словаря слова</param>
        /// <param name="data">список всех слов из словаря, начинающиееся с заданного фрагмента</param>
        public Buffer(string prefix, List<Word> data) {
            Prefix = prefix;
            Data = data;
        }
    }
}
