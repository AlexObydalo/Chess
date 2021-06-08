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

        public double GetMark()
        {
            double OurPathesNum = CountYourPathes(Desk); //Подсчет наших ходов
            double EnemyPathesNum = CountEnemyPathes(Desk); //Подсчет ходов врага

            //Если у каждого есть ходы
            if (OurPathesNum!=0 && EnemyPathesNum != 0)
            {
                return OurPathesNum / EnemyPathesNum;
            }
            else
            {
                if(OurPathesNum == 0)//Если у нас нет ходов
                {
                    if(IsThereCheck(Desk))//Если у нас шах 
                    {
                        return -1; //Нашим мат
                    }
                    else
                    {
                        return 0; //Пат
                    }
                }
                else //Если у врага нет ходов
                {
                    if (IsThereCheck(Desk))//Если у врага шах
                    {
                        return 1000; //Врагам мат
                    }
                    else
                    {
                        return 0; //Пат
                    }
                }
            }
        }

        private bool IsThereCheck(int[,] desk)//Проверка на наличие шаха
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









        //Смена игрока
        public void SwitchPlayer()
        {
            if (currPlayer == 1) //Если игрок - белый
            {
                currPlayer = 2; //Игрок становиться черным
            }
            else
            {
                currPlayer = 1; //Игрок становиться белым
            }
        }

        //Просчитать количество вражеских ходов
        public int CountEnemyPathes(int[,] desk)
        {
            int AllEnemyPathes;//Переменная количества ходов
            
            SwitchPlayer(); //Смена игрока (на врага)
            
            AllEnemyPathes = CountYourPathes(desk); //Подсчет ходов текущего игрока
            
            SwitchPlayer(); //Смена игрока (обратно на нас)

            return AllEnemyPathes; //Вернуть количество ходов врага
        }


        //Просчитать количество наших ходов

        public int CountYourPathes(int[,] desk)//Метод счета ваших ходов
        {
            int AllPathesCount = 0; //Количество ходов

            //перебор клеток доски
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (desk[i, j] / 10 == currPlayer) //Если нам клетке наша фигура
                    {
                        switch (desk[i, j] % 10)
                        {
                            case 6://Для пешек

                                AllPathesCount += CountPawnPathes(desk, i, j); //Добавить к ходам ходы пешек

                                break;

                            case 5://Для ладей

                                AllPathesCount += CountHorisotalVerticalPathes(desk, i, j); //Добавить к ходам ходы ладей

                                break;

                            case 4://Для коней

                                AllPathesCount += CountHorsePathes(desk, i, j); //Добавить к ходам ходы лошадей

                                break;

                            case 3://Для офицеров

                                AllPathesCount += CountDioganalPathes(desk, i, j); //Добавить к ходам ходы офицеров

                                break;

                            case 2://Для ферзей

                                AllPathesCount += CountHorisotalVerticalPathes(desk, i, j); //Добавить к ходам ходы ладей
                                AllPathesCount += CountDioganalPathes(desk, i, j); //Добавить к ходам ходы офицеров

                                break;

                            case 1://Для короля

                                AllPathesCount += CountHorisotalVerticalPathes(desk, i, j, true); //Добавить к ходам ходы ладей с ограничением в один шаг
                                AllPathesCount += CountDioganalPathes(desk, i, j, true); //Добавить к ходам ходы офицеров ограничением в один шаг
                                AllPathesCount += CountCastlingPathes(desk, i, j); //Добавить ходы рокировки


                                break;
                        }
                    }
                }
            }
            return AllPathesCount; //Вернуть количество 
        }

        public int CountPawnPathes(int[,] desk, int IcurrFigure, int JcurrFigure)
        {
            int PathesCount = 0; //Количество ходов пешки

            int dir = currPlayer == 1 ? 1 : -1; //(если играют белые  dir = 1, иначе dir = -1) dir переменная по направлению пешки.


            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure)) //Находиться ли прямой ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure] == 0) //Если на клетке нет фигур
                {
                    if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure, desk)))//Если после хода наш король не под ударом
                    {
                        PathesCount++; //Увеличить количество ходов на один
                    }

                    if (InsideBorder(IcurrFigure + 2 * dir, JcurrFigure)) //Находиться ли прямой двойной ход пешкой в пределах доски
                    {
                        if ((IcurrFigure == 1 && currPlayer == 1 || IcurrFigure == 6 && currPlayer == 2) & desk[IcurrFigure + 2 * dir, JcurrFigure] == 0) // Если есть возможность сделать два хода вперед
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2 * dir, JcurrFigure, desk)))//Если после хода наш король не под ударом
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                    }
                }
            }

            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure + 1)) //Находиться ли правый атакующий (косой) ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 && desk[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                {
                    if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure + 1, desk)))//Если после хода наш король не под ударом
                    {
                        PathesCount++; //Увеличить количество ходов на один
                    }
                }
            }
            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1)) ////Находиться ли левый атакующий (косой) ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && desk[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                {
                    if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure - 1, desk)))//Если после хода наш король не под ударом
                    {
                        PathesCount++; //Увеличить количество ходов на один
                    }
                }
            }

            return PathesCount; //Вернуть количество ходов
        }

        public int CountHorisotalVerticalPathes(int[,] desk, int IcurrFigure, int JcurrFigure, bool isOneStep = false) // Подсчет вертикально-горизонтальных ходов
        {
            int PathesCount = 0; //Количество ходов пешки

            for (int i = IcurrFigure + 1; i < 8; i++) //Движение вверх
            {
                if (InsideBorder(i, JcurrFigure)) //Клетка в пределах доски
                {
                    if (desk[i, JcurrFigure] != 0)//Если на клетке есть фигура
                    {
                        if (desk[i, JcurrFigure] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, JcurrFigure, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, JcurrFigure, desk))) //Если после хода нет шаха
                        {
                            PathesCount++; //Увеличить количество ходов на один
                        }
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int i = IcurrFigure - 1; i >= 0; i--) //Движение вниз
            {
                if (InsideBorder(i, JcurrFigure)) //Клетка в пределах доски
                {
                    if (desk[i, JcurrFigure] != 0)//Если на клетке есть фигура
                    {
                        if (desk[i, JcurrFigure] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, JcurrFigure, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, JcurrFigure, desk))) //Если после хода нет шаха
                        {
                            PathesCount++; //Увеличить количество ходов на один
                        }
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int j = JcurrFigure + 1; j < 8; j++) //Движение вправо
            {
                if (InsideBorder(IcurrFigure, j)) //Клетка в пределах доски
                {
                    if (desk[IcurrFigure, j] != 0)//Если на клетке есть фигура
                    {
                        if (desk[IcurrFigure, j] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThereCheck(MakePheudoPath(IcurrFigure, IcurrFigure, IcurrFigure, j, desk))) //Если после хода нет шаха
                        {
                            PathesCount++; //Увеличить количество ходов на один
                        }
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int j = JcurrFigure - 1; j >= 0; j--) //Движение влево
            {
                if (InsideBorder(IcurrFigure, j)) //Клетка в пределах доски
                {
                    if (desk[IcurrFigure, j] != 0)//Если на клетке есть фигура
                    {
                        if (desk[IcurrFigure, j] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThereCheck(MakePheudoPath(IcurrFigure, IcurrFigure, IcurrFigure, j, desk))) //Если после хода нет шаха
                        {
                            PathesCount++; //Увеличить количество ходов на один
                        }
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }

            return PathesCount; //Вернуть количество ходов
        }

        public int CountDioganalPathes(int[,] desk, int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            int PathesCount = 0; //Количество ходов пешки

            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (desk[i, j] != 0)//Если на клетке есть фигура
                    {
                        if (desk[i, j] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                        {
                            PathesCount++; //Увеличить количество ходов на один
                        }
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
                    if (desk[i, j] != 0)//Если на клетке есть фигура
                    {
                        if (desk[i, j] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                        {
                            PathesCount++; //Увеличить количество ходов на один
                        }
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
                    if (desk[i, j] != 0)//Если на клетке есть фигура
                    {
                        if (desk[i, j] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                        {
                            PathesCount++; //Увеличить количество ходов на один
                        }
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
                    if (desk[i, j] != 0)//Если на клетке есть фигура
                    {
                        if (desk[i, j] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                        {
                            PathesCount++; //Увеличить количество ходов на один
                        }
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
            return PathesCount; // Вернуть количество ходов
        }

        public int CountHorsePathes(int[,] desk, int IcurrFigure, int JcurrFigure) //Посчитать ходы коня
        {
            int PathesCount = 0; //Количество ходов пешки

            if (InsideBorder(IcurrFigure - 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 2, JcurrFigure + 1] / 10 != currPlayer && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure + 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 2, JcurrFigure - 1] / 10 != currPlayer && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure - 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 2, JcurrFigure + 1] / 10 != currPlayer && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure + 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 2, JcurrFigure - 1] / 10 != currPlayer && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure - 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 1, JcurrFigure + 2] / 10 != currPlayer && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure + 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 1, JcurrFigure + 2] / 10 != currPlayer && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure + 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 1, JcurrFigure - 2] / 10 != currPlayer && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure - 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 1, JcurrFigure - 2] / 10 != currPlayer && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure - 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }

            return PathesCount; // Вернуть количество ходов
        }

        public int CountCastlingPathes(int[,] desk, int IcurrFigure, int JcurrFigure)//Посчитать рокировочные ходы
        {
            int PathesCount = 0; //Количество ходов

            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForShortCastling(desk)) //Если король и ладья на правильном месте для короткой рокировки
            {
                if (IsTrerePlaceForShortCastling(desk)) //Если место между королем и правой ладьей свободно
                {
                    if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 1, desk)) && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 2, desk))) //Если ход не принесет шаха и король не идет через битое поле.
                    {
                        PathesCount++; //Добавить ход
                    }
                }
            }

            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForLongCastling(desk)) //Если король и ладья на правильном месте для длинной рокировки
            {
                if (IsTrerePlaceForLongCastling(desk)) //Если место между королем и левой ладьей свободно
                {
                    if (!IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 1, desk)) && !IsThereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 2, desk))) //Если ход не принесет шаха и король не идет через битое поле.
                    {
                        PathesCount++; //Добавить ход
                    }
                }
            }

            return PathesCount; //Вернуть количество ходов
        }

        public int[,] MakePheudoPath(int I1, int J1, int I2, int J2, int[,] desk1) //Сделать псевдоход
        {
            int[,] desk = new int[8, 8];

            //Копирование массива desk1 в desk
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    desk[i, j] = desk1[i, j];
                }
            }

            desk[I2, J2] = desk[I1, J1];//Записать в вторую клетку значение первой
            desk[I1, J1] = 0;//Приравнять значение первой кнопки к нулю

            return desk; //Вернуть массив desk

        }


        //Проверка на рокировку
        public bool IsKingReadyForCastling(int IcurrFigure, int JcurrFigure)
        {
            if (JcurrFigure == 4 && (IcurrFigure == 0 && currPlayer == 1 || IcurrFigure == 7 && currPlayer == 2)) //Если король на правильном месте для рокировки
            {
                return true;
            }
            return false;
        }

        public bool IsCastleReadyForShortCastling(int[,] desk)
        {
            if ((desk[0, 7] == 15 && currPlayer == 1) || (desk[7, 7] == 25 && currPlayer == 2))//Если ладья на правильном месте для короткой рокировки
            {
                return true;
            }
            return false;
        }

        public bool IsCastleReadyForLongCastling(int[,] desk)
        {
            if ((desk[0, 0] == 15 && currPlayer == 1) || (desk[7, 0] == 25 && currPlayer == 2)) //Если ладья на правильном месте для длинной рокировки
            {
                return true;
            }
            return false;
        }

        public bool IsTrerePlaceForShortCastling(int[,] desk)
        {
            if ((currPlayer == 1 && desk[0, 5] == 0 && desk[0, 6] == 0) || (currPlayer == 2 && desk[7, 5] == 0 && desk[7, 6] == 0))//Есть ли место для короткой рокировки у игрока, который ходит
            {
                return true;
            }
            return false;
        }

        public bool IsTrerePlaceForLongCastling(int[,] desk)
        {
            if ((currPlayer == 1 && desk[0, 3] == 0 && desk[0, 2] == 0 && desk[0, 1] == 0) || (currPlayer == 2 && desk[7, 3] == 0 && desk[7, 2] == 0 && desk[7, 1] == 0))//Есть ли место для длинной рокировки у игрока, который ходит
            {
                return true;
            }
            return false;
        }
    } 
}
