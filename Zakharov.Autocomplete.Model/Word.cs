using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zakharov.Autocomplete.Model {
    /// <summary>
    /// Слово из частотного словаря
    /// </summary>
    internal class Word {
        /// <summary>Само слово из текста</summary>
        public string Text { get; set; }
        /// <summary>Частота, с которой слово встречается в тексте</summary>
        public int Frequency { get; set; }
        /// <summary>
        /// Создание объекта
        /// </summary>
        /// <param name="text">слово из текста</param>
        /// <param name="frequency">частота, с которой слово встречается в тексте</param>
        public Word(string text, int frequency) {
            Text = text;
            Frequency = frequency;
        }
    }
}
