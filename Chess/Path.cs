using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Path //Запись хода
    {
        public FigurePosition P1 { get; set; } //Первая позиция
        public FigurePosition P2 { get; set; } //Вторая позиция

        //Создание записи хода из двух позиций
        public Path(FigurePosition p1, FigurePosition p2)
        {
            P1 = p1;
            P2 = p2;
        }

        //Создание записи хода из координат двух позиций
        public Path(int I, int J, int I2, int J2)
        {
            P1 = new FigurePosition(I, J);
            P2 = new FigurePosition(I2, J2);
        }
    }
}
