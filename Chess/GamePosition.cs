using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class GamePosition //Запись позиции игры
    {

        public int[,] Desk { get; set; } //Запись доски после хода

        public Path Path { get; set; } //Запись хода

        public int СurrPlayer { get; set; } //Запись номера игрока

        public int OurPathesNum { get; set; } //Количество наших ходов

        public int EnemyPathesNum { get; set; } //Количество ходов противника

        public int GetMark(bool IsThereCheck)
        {
            //Если у каждого есть ходы
            if(OurPathesNum!=0 && EnemyPathesNum != 0)
            {
                return OurPathesNum / EnemyPathesNum;
            }
            else
            {
                if(OurPathesNum == 0)//Если у нас нет ходов
                {
                    if(IsThereCheck)//Если у нас шах 
                    {
                        return -1000; //Нашим мат
                    }
                    else
                    {
                        return -500; //Пат
                    }
                }
                else //Если у врага нет ходов
                {
                    if (IsThereCheck)//Если у врага шах
                    {
                        return 1000; //Врагам мат
                    }
                    else
                    {
                        return -500; //Пат
                    }
                }
            }
        }

        //Создание позиции по ходу и доске
        public GamePosition(Path path, int[,] desk)
        {
            Path = path;
            Desk = desk;
        }
    } 
}
