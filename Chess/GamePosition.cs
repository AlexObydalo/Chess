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

        public int currPlayer { get; set; } //Игрок который ходит
        public Path Path { get; set; } //Запись хода

        public int OurPathesNum { get; set; } //Количество наших ходов

        public int EnemyPathesNum { get; set; } //Количество ходов противника

        public int GetMark()
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
                    if(IsThereCheck())//Если у нас шах 
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
                    if (IsThereCheck())//Если у врага шах
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

        private bool IsThereCheck()//Проверка на наличие шаха
        {
            List<int> Figures = ReturnAllAttackedFigures(Desk); //Записать все фигуры которые под ударом в массив
            foreach (int f in Figures)//Перебор фигур под ударом
            {
                if (f == currPlayer * 10 + 1)//Если фигура под ударом - наш король
                {
                    return true; //Вернуть true
                }
            }
            return false;//Если король не обнаружен - вернуть false.
        }

        //Создание позиции по ходу, доске, и текущему номеру игрока
        public GamePosition(Path path, int[,] desk, int CurrPlayer)
        {
            Path = path;
            Desk = desk;
            currPlayer = CurrPlayer;
        }

        //Проверка на шах
        

        public List<int> ReturnAllAttackedFigures(int[,] desk)//Обнаружение отакованых фигур. Метод принимает доску (двумерный массив)
        {
            List<int> AllAtackedF = new List<int>(); //Все атакованные фигуры
            //Перебор клеток на доске
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (desk[i, j] / 10 == 1 + (currPlayer % 2)) //Если на клетке вражеская фигура
                    {
                        switch (desk[i, j] % 10)
                        {
                            case 6://Для пешек

                                foreach (int f in ReturnFiguresAttackedByPawn(i, j, desk))//Перебор фигур под ударом пешки
                                {
                                    AllAtackedF.Add(f); //Добавить фигуру в атакованные
                                }

                                break;
                            case 5://Для ладей
                                foreach (int f in ReturnFiguresAttackedVerticalHorizontal(i, j, desk))//Перебор фигур под ударом ладьи
                                {
                                    AllAtackedF.Add(f); //Добавить фигуру в атакованные
                                }

                                break;
                            case 4://Для коней
                                foreach (int f in ReturnFiguresAttackedHorseSteps(i, j, desk))//Перебор фигур под ударом коня
                                {
                                    AllAtackedF.Add(f); //Добавить фигуру в атакованные
                                }

                                break;
                            case 3://Для офицеров
                                foreach (int f in ReturnFiguresAttackedDiagonal(i, j, desk))//Перебор фигур под ударом офицера
                                {
                                    AllAtackedF.Add(f); //Добавить фигуру в атакованные
                                }
                                break;
                            case 2://Для ферзей
                                foreach (int f in ReturnFiguresAttackedVerticalHorizontal(i, j, desk))//Перебор фигур под вертикальным или горизонтальным ударом
                                {
                                    AllAtackedF.Add(f); //Добавить фигуру в атакованные
                                }
                                foreach (int f in ReturnFiguresAttackedDiagonal(i, j, desk))//Перебор фигур под косым ударом
                                {
                                    AllAtackedF.Add(f); //Добавить фигуру в атакованные
                                }
                                break;
                            case 1://Для королей
                                foreach (int f in ReturnFiguresAttackedVerticalHorizontal(i, j, desk, true))//Перебор фигур под вертикальным или горизонтальным ударом на один ход
                                {
                                    AllAtackedF.Add(f); //Добавить фигуру в атакованные
                                }
                                foreach (int f in ReturnFiguresAttackedDiagonal(i, j, desk, true))//Перебор фигур под косым ударом на один ход
                                {
                                    AllAtackedF.Add(f); //Добавить фигуру в атакованные
                                }
                                break;
                        }
                    }
                }
            }

            return AllAtackedF; //Вернуть все атакованые фигуры
        }

        public List<int> ReturnFiguresAttackedByPawn(int IcurrFigure, int JcurrFigure, int[,] desk)//Обнаружение фигур отакованых пешкой. 
        {
            List<int> AttackedF = new List<int>(); //Список атакованных фигур

            int dir = currPlayer == 1 ? -1 : 1; //(если играют белые  dir = -1, иначе dir = 1) dir переменная по направлению пешки. 

            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure + 1)) //Находиться ли правый атакующий (косой) ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 && desk[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 == currPlayer) //Два условия: клетка не пуста, фигура на ней - наша
                {
                    AttackedF.Add(desk[IcurrFigure + 1 * dir, JcurrFigure + 1]); //Записать фигуру в список
                }
            }
            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1)) ////Находиться ли левый атакующий (косой) ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && desk[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 == currPlayer) //Два условия: клетка не пуста, фигура на ней - наша
                {
                    AttackedF.Add(desk[IcurrFigure + 1 * dir, JcurrFigure - 1]); //Записать фигуру в список
                }
            }
            return AttackedF; //Вернуть список атакованных фигур
        }

        public List<int> ReturnFiguresAttackedVerticalHorizontal(int IcurrFigure, int JcurrFigure, int[,] desk, bool isOneStep = false)//Обнаружение фигур отакованых по вертикальной прямой
        {
            List<int> AttackedF = new List<int>(); //Список атакованных фигур

            for (int i = IcurrFigure + 1; i < 8; i++) //Движение вверх
            {
                if (InsideBorder(i, JcurrFigure)) //Клетка в пределах доски
                {
                    if (desk[i, JcurrFigure] != 0) //Если клетка не пуста
                    {
                        if (desk[i, JcurrFigure] / 10 == currPlayer) //если отакована наша фигура 
                        {
                            AttackedF.Add(desk[i, JcurrFigure]); //Записать отакованную фигуру
                        }
                        break;
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }

            for (int i = IcurrFigure - 1; i >= 0; i--) //Движение вниз
            {
                if (InsideBorder(i, JcurrFigure)) //Клетка в пределах доски
                {
                    if (desk[i, JcurrFigure] != 0) //Если клетка не пуста
                    {
                        if (desk[i, JcurrFigure] / 10 == currPlayer) //если отакована наша фигура 
                        {
                            AttackedF.Add(desk[i, JcurrFigure]); //Записать отакованную фигуру
                        }
                        break;
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }

            for (int j = JcurrFigure + 1; j < 8; j++) //Движение вправо
            {
                if (InsideBorder(IcurrFigure, j)) //Клетка в пределах доски
                {
                    if (desk[IcurrFigure, j] != 0) //Если клетка не пуста
                    {
                        if (desk[IcurrFigure, j] / 10 == currPlayer) //если отакована наша фигура 
                        {
                            AttackedF.Add(desk[IcurrFigure, j]); //Записать отакованную фигуру
                        }
                        break;
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }

            for (int j = JcurrFigure - 1; j >= 0; j--) //Движение влево
            {
                if (InsideBorder(IcurrFigure, j)) //Клетка в пределах доски
                {
                    if (desk[IcurrFigure, j] != 0) //Если клетка не пуста
                    {
                        if (desk[IcurrFigure, j] / 10 == currPlayer) //если отакована наша фигура 
                        {
                            AttackedF.Add(desk[IcurrFigure, j]); //Записать отакованную фигуру
                        }
                        break;
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;


            }

            return AttackedF; //Отправить список атакованных фигур
        }

        public List<int> ReturnFiguresAttackedDiagonal(int IcurrFigure, int JcurrFigure, int[,] desk, bool isOneStep = false) ////Обнаружение фигур отакованых по косой линии
        {
            List<int> AttackedF = new List<int>(); //Список атакованных фигур

            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (desk[i, j] != 0) //Если клетка не пуста
                    {
                        if (desk[i, j] / 10 == currPlayer) //если отакована наша фигура 
                        {
                            AttackedF.Add(desk[i, j]); //Записать отакованную фигуру
                        }
                        break;
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (desk[i, j] != 0) //Если клетка не пуста
                    {
                        if (desk[i, j] / 10 == currPlayer) //если отакована наша фигура 
                        {
                            AttackedF.Add(desk[i, j]); //Записать отакованную фигуру
                        }
                        break;
                    }
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, j))
                {
                    if (desk[i, j] != 0) //Если клетка не пуста
                    {
                        if (desk[i, j] / 10 == currPlayer) //если отакована наша фигура 
                        {
                            AttackedF.Add(desk[i, j]); //Записать отакованную фигуру
                        }
                        break;
                    }
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, j))
                {
                    if (desk[i, j] != 0) //Если клетка не пуста
                    {
                        if (desk[i, j] / 10 == currPlayer) //если отакована наша фигура 
                        {
                            AttackedF.Add(desk[i, j]); //Записать отакованную фигуру
                        }
                        break;
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            return AttackedF; //Вернуть отакованные фигуры


        }

        public List<int> ReturnFiguresAttackedHorseSteps(int IcurrFigure, int JcurrFigure, int[,] desk) //Ходы коня
        {
            List<int> AttackedF = new List<int>();

            if (InsideBorder(IcurrFigure - 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 2, JcurrFigure + 1] / 10 == currPlayer) //если атакована наша фигура 
                {
                    AttackedF.Add(desk[IcurrFigure - 2, JcurrFigure + 1]); //Записать атакованную фигуру
                }
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 2, JcurrFigure - 1] / 10 == currPlayer) //если атакована наша фигура 
                {
                    AttackedF.Add(desk[IcurrFigure - 2, JcurrFigure - 1]); //Записать атакованную фигуру
                }
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 2, JcurrFigure + 1] / 10 == currPlayer) //если отакована наша фигура 
                {
                    AttackedF.Add(desk[IcurrFigure + 2, JcurrFigure + 1]); //Записать атакованную фигуру
                }
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 2, JcurrFigure - 1] / 10 == currPlayer) //если атакована наша фигура 
                {
                    AttackedF.Add(desk[IcurrFigure + 2, JcurrFigure - 1]); //Записать атакованную фигуру
                }
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 1, JcurrFigure + 2] / 10 == currPlayer) //если атакована наша фигура 
                {
                    AttackedF.Add(desk[IcurrFigure - 1, JcurrFigure + 2]); //Записать атакованную фигуру
                }
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 1, JcurrFigure + 2] / 10 == currPlayer) //если атакована наша фигура 
                {
                    AttackedF.Add(desk[IcurrFigure + 1, JcurrFigure + 2]); //Записать атакованную фигуру
                }
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 1, JcurrFigure - 2] / 10 == currPlayer) //если атакована наша фигура 
                {
                    AttackedF.Add(desk[IcurrFigure - 1, JcurrFigure - 2]); //Записать атакованную фигуру
                }
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 1, JcurrFigure - 2] / 10 == currPlayer) //если атакована наша фигура 
                {
                    AttackedF.Add(desk[IcurrFigure + 1, JcurrFigure - 2]); //Записать атакованную фигуру
                }
            }

            return AttackedF; //Вернуть атакованные фигуры
        }

        public bool InsideBorder(int ti, int tj) //Проверка находяться ли координаты в пределах доски 
        {
            if (ti >= 8 || tj >= 8 || ti < 0 || tj < 0)
            {
                return false;
            }
            return true;
        }
    } 
}
