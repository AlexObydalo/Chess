using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        public Image chessSprites;

        public int TurningCode = 0; //Код превращений для пешки на краю доски
        
        public int PositionNum = 0;
        
        public int[,] StartMap = new int[8, 8] //Изначальная расстановка фигур
        {
            {15, 14, 13, 12, 11, 13, 14, 15}, //Десятки овечают за цвет (1 - белый, 2 - черный)
            {16, 16, 16, 16, 16, 16, 16, 16}, //Еденицы отвечают за тип фигуры 
            {0, 0, 0, 0, 0, 0, 0, 0}, // 1 - король, 2 - ферзь, 3 - офицер, 4 - конь, 5 - ладья
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {26, 26, 26, 26, 26, 26, 26, 26},
            {25, 24, 23, 22, 21, 23, 24, 25},
        };

        public int[,] map;

        public Button[,] butts = new Button[8, 8]; // Массив кнопок - клеток поля
        
        public List<int[,]> gamehistory = new List<int[,]> (); // Список, в котором записана история ходов

        public int currPlayer; //Номер игрока, который ходит (1 - белый, 2 - черный)

        public Button prevButton; // Кнопка которая была нажата предпоследней

        public Point prevCastlePlaceForCastling = new Point(); //Предыдущее место ладьи для рокировки
        public Point CastlePlaceForCastling = new Point(); //Место ладьи для рокировки

        //Кнопки превращений пешек
        Button Queen = new Button();//Создание кнопки "Превратить пешку в королеву"
        Button Officer = new Button();//Создание кнопки "Превратить пешку в офицера"
        Button Hourse = new Button();//Создание кнопки "Превратить пешку в коня"
        Button Castle = new Button();//Создание кнопки "Превратить пешку в ладью"

        //Запись координат пешки на краю доски
        int PawnI; //I пешки
        int PawnJ; //J пешки

        public bool isMoving = false; // Буллеровкая переменная - происходит ли ход?
        public Form1()
        {
            InitializeComponent();

            chessSprites = new Bitmap ("chess.png"); // загрузка картинки со всеми фигурами
            //Image part = new Bitmap(50, 50);
            //Graphics g = Graphics.FromImage(part);
            //g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0, 0, 150, 150, GraphicsUnit.Pixel);
            //button1.BackgroundImage = part;

            Init();
        }

        //Потготовка новой игры
        public void Init()
        {
            currPlayer = 1;
            
            // Расстановка фигур на карте
            map = new int[8, 8] 
        {
            {15, 14, 13, 12, 11, 13, 14, 15}, 
            {16, 16, 16, 16, 16, 16, 16, 16}, 
            {0, 0, 0, 0, 0, 0, 0, 0}, 
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {26, 26, 26, 26, 26, 26, 26, 26},
            {25, 24, 23, 22, 21, 23, 24, 25},
        };




            CreateMap(); // Метод прорисовки игового поля

            // Ход входит в историю
            gamehistory.Add(new int[8, 8]);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    gamehistory[PositionNum][i, j] = map[i, j];
                }
            }
        }

        public void ReInit() //Перезапуск игры
        {
            currPlayer = 1;
            PositionNum = 0;

            // Расстановка фигур на карте
            map = new int[8, 8]
            {
               {15, 14, 13, 12, 11, 13, 14, 15},
               {16, 16, 16, 16, 16, 16, 16, 16},
               {0, 0, 0, 0, 0, 0, 0, 0},
               {0, 0, 0, 0, 0, 0, 0, 0},
               {0, 0, 0, 0, 0, 0, 0, 0},
               {0, 0, 0, 0, 0, 0, 0, 0},
               {26, 26, 26, 26, 26, 26, 26, 26},
               {25, 24, 23, 22, 21, 23, 24, 25}, 
             };

            ReDrawMap(); // Метод прорисовки игового поля
            gamehistory.Clear(); //Удаление истории игры

            // Ход входит в историю
            gamehistory.Add(new int[8, 8]);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    gamehistory[PositionNum][i, j] = map[i, j];
                }
            }
        }

        // Прорисовка игрового поля
        public void CreateMap()
        {
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j] = new Button(); // инициализация члена массива butts

                    //Создание новой кнопки (клетки на шахматном поле)
                    Button butt = new Button();
                    butt.Size = new Size(50, 50);
                    butt.Location = new Point (j * 50, i * 50); // Определение мета кнопки по кооординатам j и i

                    switch (map[i, j]/10)
                    {
                        //Если фигуры - белые
                        case 1:
                            Image part = new Bitmap(50, 50); // Инициализация картинки для фигуры
                            Graphics g = Graphics.FromImage(part); //создание Graphics для картинки
                            g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0+150*(map[i,j] % 10 - 1), 0, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
                            butt.BackgroundImage = part; //Прилепливаем картинку на кнопку
                            break;
                        //Если фигуры - черные
                        case 2:
                            Image part2 = new Bitmap(50, 50);
                            Graphics g2 = Graphics.FromImage(part2);
                            g2.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * (map[i, j] % 10 - 1), 150, 150, 150, GraphicsUnit.Pixel); // Разница лишь в том, что здесь мы взяли кусок картинки на 150 пикселей ниже.
                            butt.BackgroundImage = part2;
                            break;
                    }
                    butt.Click += new EventHandler(OnFigurePress); // Добавление кнопки к общей функции обработки кликов

                    //butt.BackColor = Color.White;

                    if ((i + j) % 2 == 0) //Разукраска клеток шахмат в серо-белый узор 
                    {
                        butt.BackColor = Color.DarkGray;
                    }
                    else
                    {
                        butt.BackColor = Color.White;
                    }


                    this.Controls.Add(butt); // Добавление кнопки в Controls
                    butts[i, j] = butt; // Добавление кнопки в массив кнопок
                }
            }
        }

        public void ReDrawMap()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    butts[i, j].BackgroundImage = null; //Удаление старого рисунка
                    switch (map[i, j] / 10)
                    {
                        //Если фигуры - белые
                        case 1:
                            Image part = new Bitmap(50, 50); // Инициализация картинки для фигуры
                            Graphics g = Graphics.FromImage(part); //создание Graphics для картинки
                            g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * (map[i, j] % 10 - 1), 0, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
                            butts[i,j].BackgroundImage = part; //Прилепливаем картинку на кнопку
                            break;
                        //Если фигуры - черные
                        case 2:
                            Image part2 = new Bitmap(50, 50);
                            Graphics g2 = Graphics.FromImage(part2);
                            g2.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * (map[i, j] % 10 - 1), 150, 150, 150, GraphicsUnit.Pixel); // Разница лишь в том, что здесь мы взяли кусок картинки на 150 пикселей ниже.
                            butts[i,j].BackgroundImage = part2;
                            break;
                    }
                    

                    //butt.BackColor = Color.White;

                    if ((i + j) % 2 == 0) //Разукраска клеток шахмат в серо-белый узор 
                    {
                        butts[i,j].BackColor = Color.DarkGray;
                    }
                    else
                    {
                        butts[i,j].BackColor = Color.White;
                    }


                    
                }
            }
        }

        public void OnFigurePress(object sender, EventArgs e) //Общая фунция кликов по кнопкам
        {
            //?? если это не рокировочная кнопка, то сбросить рокировоччную
            //if (prevButton != null)
            //    prevButton.BackColor = Color.White;

            Button pressedButton = sender as Button;

            //pressedButton.Enabled = false;

            if (map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] != 0 && map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] / 10 == currPlayer) //Два условия: клетка не пуста, на клетке наша фигура
            {
                CloseSteps(); //Закрыть ходы
                //pressedButton.BackColor = Color.Red;
                DeactivateAllButtons(); //Деактивировать кнопки
                pressedButton.Enabled = true; //Активировать нажатую кнопку
                ShowSteps(pressedButton.Location.Y / 50, pressedButton.Location.X / 50, map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50]); //Показать ходы

                if (isMoving) //Если идет ход
                {
                    CloseSteps(); //Закрыть ходы
                    //pressedButton.BackColor = Color.White;
                    ActivateAllButtons(); //Активировать все кнопки
                    isMoving = false; //Ход не идет
                    

                }
                else
                    isMoving = true; //Начать ход
            }
            else
            {
                if (isMoving) //Если идет ход
                {
                    
                    //Поменять значения клеток местами
                    int temp = map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50];
                    map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] = map[prevButton.Location.Y / 50, prevButton.Location.X / 50];
                    map[prevButton.Location.Y / 50, prevButton.Location.X / 50] = 0;
                    
                    //Поменять  изображения кнопок местами
                    pressedButton.BackgroundImage = prevButton.BackgroundImage;
                    prevButton.BackgroundImage = null;
                    
                    

                    //если кнопка рокировочная, 
                    
                    
                    
                    if(map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] % 10 == 1 && Math.Abs(pressedButton.Location.X / 50 - prevButton.Location.X / 50)==2) //Если король сделал рокировку. Условия: на нажатой клетке король, король сдвинулся на две клетки.
                    {
                        int dir = (pressedButton.Location.X / 50 - prevButton.Location.X / 50) / 2; //Направление короля: вправо = 1, влево = -1.
                        if(currPlayer==1)//Рокируються белые
                        {
                            if(dir==1)//Направление короля вправо
                            {
                                //перемещение ладьи в короткой рокировке
                                map[0, 5] = map[0, 7]; //Клетка 0.5 приравниваеться клетке 0.7 
                                map[0, 7] = 0; //Клетка 0.7 обнуляеться
                            }
                            else //Направление короля влево
                            {
                                //перемещение ладьи в длинной рокировке
                                map[0, 3] = map[0, 0]; //Клетка 0.3 приравниваеться клетке 0.0
                                map[0, 0] = 0; //Клетка 0.0 обнуляеться
                            }
                        }
                        else//Рокируються черные
                        {
                            if (dir == 1)//Направление короля вправо
                            {
                                //перемещение ладьи в короткой рокировке
                                map[7, 5] = map[7, 7]; //Клетка 7.5 приравниваеться клетке 7.7 
                                map[7, 7] = 0; //Клетка 7.7 обнуляеться
                            }
                            else //Направление короля влево
                            {
                                //перемещение ладьи в длинной рокировке
                                map[7, 3] = map[7, 0]; //Клетка 7.3 приравниваеться клетке 7.0
                                map[7, 0] = 0; //Клетка 7.0 обнуляеться
                            }
                        }
                        ReDrawMap(); //Перерисовать
                    }
                    
                    
                    
                    isMoving = false; //Завершить ход

                    
                    
                    CloseSteps(); //Закрыть ходы
                    ActivateAllButtons(); //Активировать все кнопки
                    SwitchPlayer(); // Сменить игрока

                    PositionNum++; //Увеличить номер позиции

                    if(map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50]%10 == 6 && (pressedButton.Location.Y / 50 == 7 || pressedButton.Location.Y / 50 == 0)) //Если пешка на краю поля
                    {
                        ShowChooseButtonsForPawns();//Показать кнопки превращений пешки

                        DeactivateAllButtons(); //Деактивировать все кнопки (кроме кнопок превращений пешки)

                        PawnI = pressedButton.Location.Y / 50; //записать I пешки
                        PawnJ = pressedButton.Location.X / 50; //записать J пешки
                    }
                    

                    
                    
                    // Ход входит в историю
                    gamehistory.Add(new int[8,8]);
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {

                            gamehistory[PositionNum][i, j] = map[i, j];
                        }
                    }

                    label2.Text = Convert.ToString(PositionNum); //Отображение номера хода пользователю
                    button3.Visible = true; //Кнопка "ход назад" видна

                    //Регуляция показа кнопки "ход вперед"
                    if(PositionNum >= gamehistory.Count-1) //Если номер последнего хода меньше или равно номеру этого хода
                    {
                        button4.Visible = false;//Кнопка "ход вперед" НЕ видна
                    }
                    else //Если номер последнего хода больше номера этого хода
                    {
                        for(int i = gamehistory.Count-1; i > PositionNum ; i--) //От последней позиции в истории до этой позиции
                        {
                            gamehistory.RemoveAt(i); //Удалить из истории позицию номер i
                        }
                        button4.Visible = false;//Кнопка "ход вперед" НЕ видна
                    }

                    
                    
                    
                }
            }

            prevButton = pressedButton;
        }

        //Смена игрока
        public void SwitchPlayer() 
        {
            if(currPlayer == 1) //Если игрок - белый
            {
                currPlayer = 2; //Игрок становиться черным
            }
            else
            {
                currPlayer = 1; //Игрок становиться белым
            }
        }

        private void button1_Click(object sender, EventArgs e) // Не обращайте на эту функию внимание, она не играет роль в коде.
        {

        }
       
        //Кнопка запуска игры заново (к сожалению работает только один раз).
        private void button2_Click(object sender, EventArgs e) 
        {
            //this.Controls.Clear(); //Удаление всех кнопок из Controls
            //Init(); //Запуск новой игры

            ReInit();//Перезапуск игры
        }

        public void DeactivateAllButtons() //Деактивировать все кнопки
        {
            //Перебор матрицы кнопок
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = false;
                }
            }
            

            button2.Enabled = false; //кнопка Reset
            button3.Enabled = false; //кнопка ход назад
            button4.Enabled = false; //кнопка ход вперед

        }

        public void ActivateAllButtons() //Активировать все кнопки
        {
            //Перебор матрицы кнопок
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = true;
                }
            }

            button2.Enabled = true; //кнопка Reset
            button3.Enabled = true; //кнопка ход назад
            button4.Enabled = true; //кнопка ход вперед
        }

        public void CloseSteps() //Закраска шахмат после хода
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 0) //Разукраска клеток шахмат в серо-белый узор 
                    {
                        butts[i,j].BackColor = Color.DarkGray;
                    }
                    else
                    {
                        butts[i, j].BackColor = Color.White;
                    }
                }
            }
        }

        public bool InsideBorder(int ti, int tj) //Проверка находяться ли координаты в пределах доски 
        {
            if(ti >= 8 || tj >= 8 || ti < 0 || tj < 0)
            {
                return false;
            }
            return true;
        }

        public void ShowSteps(int IcurrFigure, int JcurrFigure, int currFigure)// Показать ходы
        {
            int dir = currPlayer == 1 ? 1 : -1; //(если играют белые  dir = 1, иначе dir = -1) dir переменная по направлению пешки.

            switch(currFigure%10) // Определение типа фигуры
            {
                case 6: // Для пешек
                    
                    

                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure)) //Находиться ли прямой ход пешкой в пределах доски
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure] == 0) //Если на клетке нет фигур
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure].BackColor = Color.Yellow; //Окрасить клетку
                            butts[IcurrFigure + 1 * dir, JcurrFigure].Enabled = true; //Сделать клетку доступной к нажатию

                            if ((IcurrFigure == 1 && currPlayer == 1 || IcurrFigure == 6 && currPlayer == 2) & map[IcurrFigure + 2 * dir, JcurrFigure] == 0) // Если есть возможность сделать два хода вперед
                            {
                                butts[IcurrFigure + 2 * dir, JcurrFigure].BackColor = Color.Yellow; //Окрасить клетку
                                butts[IcurrFigure + 2 * dir, JcurrFigure].Enabled = true; //Сделать клетку доступной к нажатию
                            }
                        }
                    }

                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure + 1)) //Находиться ли правый атакующий (косой) ход пешкой в пределах доски
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].BackColor = Color.Yellow; //Окрасить клетку
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].Enabled = true; //Сделать клетку доступной к нажатию
                        }
                    }
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1)) ////Находиться ли левый атакующий (косой) ход пешкой в пределах доски
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].BackColor = Color.Yellow; //Окрасить клетку
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].Enabled = true; //Сделать клетку доступной к нажатию
                        }
                    }
                    break;

                case 5: //Для Ладьи
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    break;
                case 3: //Для офицера
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 2: //Для ферзя 
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 1: //Для короля
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure, true);
                    ShowDiagonal(IcurrFigure, JcurrFigure, true);
                    ShowShortCastlingForKing(IcurrFigure, JcurrFigure); //Показать короткую рокировку
                    ShowLongCastlingForKing(IcurrFigure, JcurrFigure); //Показать длинную рокировку
                    break;
                case 4: //Для коня
                    ShowHorseSteps(IcurrFigure, JcurrFigure);
                    break;
            }
        }

        public void ShowHorseSteps(int IcurrFigure, int JcurrFigure) //Ходы коня
        {
            if (InsideBorder(IcurrFigure - 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure + 1); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure - 1); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure + 1); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure - 1); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure + 2); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure + 1, JcurrFigure + 2); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure - 2); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure + 1, JcurrFigure - 2); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
        }

        

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
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
                    if (!DeterminePath(i, j))
                        break;
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
                    if (!DeterminePath(i, j))
                        break;
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
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }


        }
        public void ShowVerticalHorizontal(int IcurrFigure, int JcurrFigure, bool isOneStep = false) //Перпедикулярное движение на несколько ходов 
        {
            for (int i = IcurrFigure + 1; i < 8; i++) //Движение вверх
            {
                if (InsideBorder(i, JcurrFigure)) //Клетка в пределах доски
                {
                    if (!DeterminePath(i, JcurrFigure)) //Есть ли на клетке фигура + метод выделения доступной клетки
                        break;
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int i = IcurrFigure - 1; i >= 0; i--) //Движение вниз
            {
                if (InsideBorder(i, JcurrFigure)) //Клетка в пределах доски
                {
                    if (!DeterminePath(i, JcurrFigure)) //Есть ли на клетке фигура + метод выделения доступной клетки
                        break;
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int j = JcurrFigure + 1; j < 8; j++) //Движение вправо
            {
                if (InsideBorder(IcurrFigure, j)) //Клетка в пределах доски
                {
                    if (!DeterminePath(IcurrFigure, j)) //Есть ли на клетке фигура + метод выделения доступной клетки
                        break;
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int j = JcurrFigure - 1; j >= 0; j--) //Движение влево
            {
                if (InsideBorder(IcurrFigure, j)) //Клетка в пределах доски
                {
                    if (!DeterminePath(IcurrFigure, j)) //Есть ли на клетке фигура + метод выделения доступной клетки
                        break;
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
        }


        public void ShowShortCastlingForKing(int IcurrFigure, int JcurrFigure)
        {
            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForShortCastling(IcurrFigure, JcurrFigure)) //Если король и ладья на правильном месте для короткой рокировки
            {
                if(IsTrerePlaceForShortCastling()) //Если место между королем и правой ладьей свободно
                {
                    butts[IcurrFigure, JcurrFigure + 2].Enabled = true; //Сделать клетку на два хода вправо от короля доступной к нажатию
                    butts[IcurrFigure, JcurrFigure + 2].BackColor = Color.Red; //Сделать клетку на два хода вправо от короля красной
                    //запомнить эту клетку и клетку, куда ставить ладью
                    prevCastlePlaceForCastling = new Point(IcurrFigure, JcurrFigure + 3);
                    prevCastlePlaceForCastling = new Point(IcurrFigure, JcurrFigure + 1);
                }
            }
        }

        public void ShowLongCastlingForKing(int IcurrFigure, int JcurrFigure)
        {
            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForLongCastling(IcurrFigure, JcurrFigure)) //Если король и ладья на правильном месте для длинной рокировки
            {
                if (IsTrerePlaceForLongCastling()) //Если место между королем и левой ладьей свободно
                {
                    butts[IcurrFigure, JcurrFigure - 2].Enabled = true; //Сделать клетку на два хода влево от короля доступной к нажатию
                    butts[IcurrFigure, JcurrFigure - 2].BackColor = Color.Red; //Сделать клетку на два хода влево от короля красной
                    
                    //запомнить эту клетку и клетку, куда ставить ладью
                    prevCastlePlaceForCastling = new Point(IcurrFigure, JcurrFigure - 4);
                    prevCastlePlaceForCastling = new Point(IcurrFigure, JcurrFigure - 1);
                }
            }
        }

        public bool IsKingReadyForCastling(int IcurrFigure, int JcurrFigure)
        {
            if (JcurrFigure == 4 && (IcurrFigure == 0 && currPlayer == 1 || IcurrFigure == 7 && currPlayer == 2)) //Если король на правильном месте для рокировки
            {
                return true;
            }
            return false;
        }

        public bool IsCastleReadyForShortCastling(int IcurrFigure, int JcurrFigure)
        {
            if ((map[0,7] == 15 && currPlayer == 1) || (map[7, 7] == 25 && currPlayer == 2))//Если ладья на правильном месте для короткой рокировки
            {
                return true;
            }
            return false;
        }

        public bool IsCastleReadyForLongCastling(int IcurrFigure, int JcurrFigure)
        {
            if ((map[0, 0] == 15 && currPlayer == 1) || (map[7, 0] == 25 && currPlayer == 2)) //Если ладья на правильном месте для длинной рокировки
            {
                return true;
            }
            return false;
        }

        public bool IsTrerePlaceForShortCastling() 
        {
            if ((currPlayer == 1 && map[0,5]==0 && map[0, 6] == 0) || (currPlayer == 2 && map[7, 5] == 0 && map[7, 6] == 0))//Есть ли место для короткой рокировки у игрока, который ходит
            {
                return true;
            }
            return false;
        }

        public bool IsTrerePlaceForLongCastling()
        {
            if ((currPlayer == 1 && map[0, 3] == 0 && map[0, 2] == 0 && map[0, 1] == 0) || (currPlayer == 2 && map[7, 3] == 0 && map[7, 2] == 0 && map[7, 1] == 0))//Есть ли место для длинной рокировки у игрока, который ходит
            {
                return true;
            }
            return false;
        }





        public bool DeterminePath(int IcurrFigure, int j) //Обозначить доступную клетку, и узнать есть ли на ней фигура
        {
            if (map[IcurrFigure, j] == 0) //Если клетка пуста
            {
                butts[IcurrFigure, j].BackColor = Color.Yellow;
                butts[IcurrFigure, j].Enabled = true;
            }
            else
            {
                if (map[IcurrFigure, j] / 10 != currPlayer) //Если на клетке фигура врага
                {
                    butts[IcurrFigure, j].BackColor = Color.Yellow;
                    butts[IcurrFigure, j].Enabled = true;
                }
                return false;
            }
            return true;
        }

        private void button3_Click(object sender, EventArgs e) //Кнопка по возрату предыдущего хода
        {
            PositionNum--; //Уменьшение номера позиции

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    map[i, j] = gamehistory[PositionNum][i, j];
                }
            }

            

            
            
           

            ReDrawMap(); //Перерисовка доски
            SwitchPlayer();//Смена игрока
            
            label2.Text = Convert.ToString(PositionNum); //Отображение номера хода пользователю
            if(PositionNum==0) //Если вернулись к нулевой позиции
            {
                button3.Visible = false; //Кнопка "ход назад" не видна
            }
            
            button4.Visible = true;//Кнопка "ход вперед" видна


        }

        private void button4_Click(object sender, EventArgs e)
        {
            PositionNum++; //Увеличение номера позиции

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    map[i, j] = gamehistory[PositionNum][i, j];
                }
            }







            ReDrawMap(); //Перерисовка доски
            SwitchPlayer();//Смена игрока

            label2.Text = Convert.ToString(PositionNum); //Отображение номера хода пользователю
                                                         
            //Регуляция показа кнопки "ход вперед"
            if (PositionNum >= gamehistory.Count - 1) //Если номер последнего хода меньше или равно номеру этого хода
            {
                button4.Visible = false;//Кнопка "ход вперед" НЕ видна
            }

            button3.Visible = true; //Кнопка "ход назад" видна

        }

        public void ShowChooseButtonsForPawns() //Показать кнопки превращений пешки
        {
            this.Size = new Size(490, 500);//Расширение поля вниз
            
            
            
            //Создание кнопки "Превратить пешку в королеву"
            
            Queen.Size = new Size(50, 50); //Размер кнопки 50 на 50
            Queen.Location = new Point(0, 410); //Задаем кнопки местоположение под доской
            Image part = new Bitmap(50, 50); // Инициализация картинки для фигуры
            Graphics g = Graphics.FromImage(part); //создание Graphics для картинки
            if(currPlayer == 1) //Если играют белые
            {
                g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * 1, 150, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
            }
            else //Если играют черные
            {
                g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * 1, 0, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
            }
            
            Queen.BackgroundImage = part; //Прилепливаем картинку на кнопку
            Queen.BackColor = Color.LightGreen; //Окрашиваем кноку в зеленый цвет
            Controls.Add(Queen); //Добавление кнопки в Controls

            Queen.Click += new EventHandler(QueenClick); // Добавление кнопки к общей функции обработки клика по этой кнопке





            //Создание кнопки "Превратить пешку в офицера"

            Officer.Size = new Size(50, 50); //Размер кнопки 50 на 50
            Officer.Location = new Point(50, 410); //Задаем кнопки местоположение под доской, правее ферзя
            Image part2 = new Bitmap(50, 50); // Инициализация картинки для фигуры
            Graphics g2 = Graphics.FromImage(part2); //создание Graphics для картинки
            if (currPlayer == 1) //Если играют белые
            {
                g2.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * 2, 150, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
            }
            else //Если играют черные
            {
                g2.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * 2, 0, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
            }

            Officer.BackgroundImage = part2; //Прилепливаем картинку на кнопку
            Officer.BackColor = Color.LightGreen; //Окрашиваем кноку в зеленый цвет
            Controls.Add(Officer); //Добавление кнопки в Controls

            Officer.Click += new EventHandler(OfficerClick); // Добавление кнопки к общей функции обработки клика по этой кнопке






            //Создание кнопки "Превратить пешку в коня"

            Hourse.Size = new Size(50, 50); //Размер кнопки 50 на 50
            Hourse.Location = new Point(100, 410); //Задаем кнопки местоположение под доской, правее ферзя
            Image part3 = new Bitmap(50, 50); // Инициализация картинки для фигуры
            Graphics g3 = Graphics.FromImage(part3); //создание Graphics для картинки
            if (currPlayer == 1) //Если играют белые
            {
                g3.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * 3, 150, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
            }
            else //Если играют черные
            {
                g3.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * 3, 0, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
            }

            Hourse.BackgroundImage = part3; //Прилепливаем картинку на кнопку
            Hourse.BackColor = Color.LightGreen; //Окрашиваем кноку в зеленый цвет
            Controls.Add(Hourse); //Добавление кнопки в Controls

            Hourse.Click += new EventHandler(HourseClick); // Добавление кнопки к общей функции обработки клика по этой кнопке




            //Создание кнопки "Превратить пешку в ладью"

            Castle.Size = new Size(50, 50); //Размер кнопки 50 на 50
            Castle.Location = new Point(150, 410); //Задаем кнопки местоположение под доской, правее ферзя
            Image part4 = new Bitmap(50, 50); // Инициализация картинки для фигуры
            Graphics g4 = Graphics.FromImage(part4); //создание Graphics для картинки
            if (currPlayer == 1) //Если играют белые
            {
                g4.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * 4, 150, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
            }
            else //Если играют черные
            {
                g4.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * 4, 0, 150, 150, GraphicsUnit.Pixel); //Взятие кусочка картинки с фигурами, для одной фигуры
            }

            Castle.BackgroundImage = part4; //Прилепливаем картинку на кнопку
            Castle.BackColor = Color.LightGreen; //Окрашиваем кноку в зеленый цвет
            Controls.Add(Castle); //Добавление кнопки в Controls

            Castle.Click += new EventHandler(CastleClick); // Добавление кнопки к общей функции обработки клика по этой кнопке


            

        }


        private void CastleClick(object sender, EventArgs e)
        {
            map[PawnI, PawnJ] -= 1; //Запись пешки как ладьи
            ActivateAllButtons(); //Активировать все кнопки
            ReDrawMap(); //Перерисовка карты 
            this.Size = new Size(490, 440);

            //if (PositionNum >= gamehistory.Count - 1) //Если номер последнего хода меньше или равно номеру этого хода
            //{
            //    gamehistory[PositionNum][PawnI, PawnJ] = currPlayer*10 + 5; //запись превращения в историю
            //}
            //Очистка Controls
            Controls.Remove(Castle);
            Controls.Remove(Hourse);
            Controls.Remove(Officer);
            Controls.Remove(Queen);
        }

        private void HourseClick(object sender, EventArgs e)
        {
            map[PawnI, PawnJ] = (1 + currPlayer%2) * 10 + 4; //Запись пешки как коня
            ActivateAllButtons(); //Активировать все кнопки
            ReDrawMap(); //Перерисовка карты 
            this.Size = new Size(490, 440);

            if (PositionNum == gamehistory.Count - 1) //Если номер последнего хода меньше или равно номеру этого хода
            {
                gamehistory[PositionNum][PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 4; //запись превращения в историю
            }
            //Очистка Controls
            Controls.Remove(Castle);
            Controls.Remove(Hourse);
            Controls.Remove(Officer);
            Controls.Remove(Queen);
        }

        private void OfficerClick(object sender, EventArgs e)
        {
            map[PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 3; //Запись пешки как офицера
            ActivateAllButtons(); //Активировать все кнопки
            ReDrawMap(); //Перерисовка карты 
            this.Size = new Size(490, 440);

            if (PositionNum == gamehistory.Count - 1) //Если номер последнего хода меньше или равно номеру этого хода
            {
                gamehistory[PositionNum][PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 3; //запись превращения в историю
            }
            //Очистка Controls
            Controls.Remove(Castle);
            Controls.Remove(Hourse);
            Controls.Remove(Officer);
            Controls.Remove(Queen);
        }

        private void QueenClick(object sender, EventArgs e)
        {
            map[PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 2; //Запись пешки как королевы
            ActivateAllButtons(); //Активировать все кнопки
            ReDrawMap(); //Перерисовка карты 
            this.Size = new Size(490, 440);

            if (PositionNum == gamehistory.Count - 1) //Если номер последнего хода меньше или равно номеру этого хода
            {
                gamehistory[PositionNum][PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 2; //запись превращения в историю
            }
            //Очистка Controls
            Controls.Remove(Castle);
            Controls.Remove(Hourse);
            Controls.Remove(Officer);
            Controls.Remove(Queen);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
