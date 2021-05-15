using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        public Image chessSprites;

        public int PositionNum = 0;
        
        public int[,] map = new int[8, 8] //Изначальная расстановка фигур
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

        public Button[,] butts = new Button[8, 8]; // Массив кнопок - клеток поля
        
        public List<int[,]> gamehistory = new List<int[,]> (); // Список, в котором записана история ходов

        public int currPlayer; //Номер игрока, который ходит (1 - белый, 2 - черный)

        public Button prevButton; // Кнопка которая была нажата предпоследней

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
            map = new int[8, 8] // Расстановка фигур на карте
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
            map = new int[8, 8] // Расстановка фигур на карте
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
                    butt.Click += new EventHandler(OnFigurePress); // Добавление кноки к общей функции обработки кликов

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
                    
                    isMoving = false; //Завершить ход

                    
                    
                    CloseSteps(); //Закрыть ходы
                    ActivateAllButtons(); //Активировать все кнопки
                    SwitchPlayer(); // Сменить игрока

                    PositionNum++; //Увеличить номер позиции

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

        public void DeactivateAllButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = false;
                }
            }
        }

        public void ActivateAllButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = true;
                }
            }
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
                        }

                        if ((IcurrFigure == 1 && currPlayer == 1 || IcurrFigure == 6 && currPlayer == 2) & map[IcurrFigure + 1 * dir, JcurrFigure] == 0) // Если есть возможность сделать два хода вперед
                        {
                            butts[IcurrFigure + 2 * dir, JcurrFigure].BackColor = Color.Yellow; //Окрасить клетку
                            butts[IcurrFigure + 2 * dir, JcurrFigure].Enabled = true; //Сделать клетку доступной к нажатию
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

                case 5:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    break;
                case 3:
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 2:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;
                case 1:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure, true);
                    ShowDiagonal(IcurrFigure, JcurrFigure, true);
                    break;
                case 4:
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

        private void button3_Click(object sender, EventArgs e) //Осторожно! Есть неведомые ошибки
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
