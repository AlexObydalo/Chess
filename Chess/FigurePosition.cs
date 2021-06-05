using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class FigurePosition //Запись позиции фигуры
    {
        public int I { get; set; } //Позиция  по I
        public int J { get; set; } //Позиция по J

        //Метод создания позиции фигуры
        public FigurePosition(int i, int j)
        {
            I = i;
            J = j;
        }
    }
}
