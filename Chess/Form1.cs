using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        int MaxTreeNum = 1; //Максимальный номер слоя деревьев

        int TreeNum = 0; //Текущий номер слоя деревьев

        
        
        Dictionary<int, string> FiguresNotation = new Dictionary<int, string> //Словарь записи фигур (в цифрах и буквах)
        {
            //Запись фигуры зависит от единицы, в числе на клетке  
            {1, "Кр." },
            {2, "Ф." },
            {3, "С." },
            {4, "К." },
            {5, "Л." },
            {6, "п." },
        };

        Dictionary<int, string> LineNotation = new Dictionary<int, string> //Словарь записи фигур (в цифрах и буквах)
        {
            //Буква зависит от единицы, в числе на клетке  
            {1, "a" },
            {2, "b" },
            {3, "c" },
            {4, "d" },
            {5, "e" },
            {6, "f" },
            {7, "g" },
            {8, "h" },
        };

        Random rnd = new Random(); //Генератор случайных чисел

        bool IsComputerPlaying = false; //Играет ли компьютер

        int ComputerPlayer; //Номер игрока, за которого играет компьютер (1 - белый, 2 - черный)

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
            label2.Text = "0"; //Показать игроку, что сейчас нулевой ход
            label3.Text = "Ход белых"; //Показать игроку, что сейчас ход белых

            //Кнопки "ход назад" и "ход вперед" не видны
            button3.Visible = false;
            button4.Visible = false;
            
            label5.Text = "-"; //Предыдущих ходов нет
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

            if(!IsComputerPlaying || currPlayer != ComputerPlayer)//Если компьютер не играет, или во время игры не ход компьютера 
            {
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



                        if (map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] % 10 == 1 && Math.Abs(pressedButton.Location.X / 50 - prevButton.Location.X / 50) == 2) //Если король сделал рокировку. Условия: на нажатой клетке король, король сдвинулся на две клетки.
                        {
                            int dir = (pressedButton.Location.X / 50 - prevButton.Location.X / 50) / 2; //Направление короля: вправо = 1, влево = -1.
                            if (currPlayer == 1)//Рокируються белые
                            {
                                if (dir == 1)//Направление короля вправо
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

                        if (map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] % 10 == 6 && (pressedButton.Location.Y / 50 == 7 || pressedButton.Location.Y / 50 == 0)) //Если пешка на краю поля
                        {
                            if(!IsComputerPlaying)//Если компьютер не играет
                            {
                                ShowChooseButtonsForPawns();//Показать кнопки превращений пешки

                                DeactivateAllButtons(); //Деактивировать все кнопки (кроме кнопок превращений пешки)
                            }
                            else //Если компьютер играет
                            {
                                map[pressedButton.Location.Y / 50, pressedButton.Location.X / 50] -= 4;//Превратить пешку в королеву
                            }
                            

                        }




                        // Ход входит в историю
                        gamehistory.Add(new int[8, 8]);
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
                        if (PositionNum >= gamehistory.Count - 1) //Если номер последнего хода меньше или равно номеру этого хода
                        {
                            button4.Visible = false;//Кнопка "ход вперед" НЕ видна
                        }
                        else //Если номер последнего хода больше номера этого хода
                        {
                            for (int i = gamehistory.Count - 1; i > PositionNum; i--) //От последней позиции в истории до этой позиции
                            {
                                gamehistory.RemoveAt(i); //Удалить из истории позицию номер i
                            }
                            button4.Visible = false;//Кнопка "ход вперед" НЕ видна
                        }

                        ReDrawMap(); //Перерисовать

                        if (IsThrereCheck(map))//Если есть шах
                        {
                            MakeOurKingRed();//Сделать клетку короля красной
                        }

                        label5.Text = WriteTurn(gamehistory[PositionNum - 1], gamehistory[PositionNum]); //Высветить ход, сделанный игроком

                        if(IsComputerPlaying)//Если компьютер играет
                        {
                            MakePathAI();//Сделать ход искуственного интеллекта
                        }

                    }
                }

                prevButton = pressedButton;
            }
        }

        //Смена игрока
        public void SwitchPlayer() 
        {
            if(currPlayer == 1) //Если игрок - белый
            {
                currPlayer = 2; //Игрок становиться черным
                label3.Text = "Ход черных"; //Показать пользователю, что сейчас ход черных
            }
            else
            {
                currPlayer = 1; //Игрок становиться белым
                label3.Text = "Ход белых"; //Показать пользователю, что сейчас ход белых
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


            IsComputerPlaying = false; //Компьютер не играет в шахматы
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
                            if (!CanPathGiveUsCheck(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure))//Если после хода наш король не под ударом
                            {
                                butts[IcurrFigure + 1 * dir, JcurrFigure].BackColor = Color.Yellow; //Окрасить клетку
                                butts[IcurrFigure + 1 * dir, JcurrFigure].Enabled = true; //Сделать клетку доступной к нажатию
                            }

                            if (InsideBorder(IcurrFigure + 2 * dir, JcurrFigure)) //Находиться ли прямой двойной ход пешкой в пределах доски
                            {
                                if ((IcurrFigure == 1 && currPlayer == 1 || IcurrFigure == 6 && currPlayer == 2) & map[IcurrFigure + 2 * dir, JcurrFigure] == 0) // Если есть возможность сделать два хода вперед
                                {
                                    if (!CanPathGiveUsCheck(IcurrFigure, JcurrFigure, IcurrFigure + 2 * dir, JcurrFigure))//Если после хода наш король не под ударом
                                    {
                                        butts[IcurrFigure + 2 * dir, JcurrFigure].BackColor = Color.Yellow; //Окрасить клетку
                                        butts[IcurrFigure + 2 * dir, JcurrFigure].Enabled = true; //Сделать клетку доступной к нажатию
                                    }
                                }
                            }   
                        }
                    }

                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure + 1)) //Находиться ли правый атакующий (косой) ход пешкой в пределах доски
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                        {
                            if (!CanPathGiveUsCheck(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure + 1))//Если после хода наш король не под ударом
                            {
                                butts[IcurrFigure + 1 * dir, JcurrFigure+1].BackColor = Color.Yellow; //Окрасить клетку
                                butts[IcurrFigure + 1 * dir, JcurrFigure+1].Enabled = true; //Сделать клетку доступной к нажатию
                            }
                        }
                    }
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1)) ////Находиться ли левый атакующий (косой) ход пешкой в пределах доски
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && map[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                        {
                            if (!CanPathGiveUsCheck(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure - 1))//Если после хода наш король не под ударом
                            {
                                butts[IcurrFigure + 1 * dir, JcurrFigure - 1].BackColor = Color.Yellow; //Окрасить клетку
                                butts[IcurrFigure + 1 * dir, JcurrFigure - 1].Enabled = true; //Сделать клетку доступной к нажатию
                            }
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
                    
                    if(!IsThrereCheck(map)) //Если нет шаха
                    {
                        ShowShortCastlingForKing(IcurrFigure, JcurrFigure); //Показать короткую рокировку
                        ShowLongCastlingForKing(IcurrFigure, JcurrFigure); //Показать длинную рокировку
                    }
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
                DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure + 1); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure - 1); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure + 1); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure - 1); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure + 2); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure + 2); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure - 2); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure - 2); //Есть ли на клетке фигура + метод выделения доступной клетки
            }
        }

        

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(IcurrFigure, JcurrFigure, i, j))
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
                    if (!DeterminePath(IcurrFigure, JcurrFigure, i, j))
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
                    if (!DeterminePath(IcurrFigure, JcurrFigure, i, j))
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
                    if (!DeterminePath(IcurrFigure, JcurrFigure, i, j))
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
                    if (!DeterminePath(IcurrFigure, JcurrFigure, i, JcurrFigure)) //Есть ли на клетке фигура + метод выделения доступной клетки
                        break;
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int i = IcurrFigure - 1; i >= 0; i--) //Движение вниз
            {
                if (InsideBorder(i, JcurrFigure)) //Клетка в пределах доски
                {
                    if (!DeterminePath(IcurrFigure, JcurrFigure, i, JcurrFigure)) //Есть ли на клетке фигура + метод выделения доступной клетки
                        break;
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int j = JcurrFigure + 1; j < 8; j++) //Движение вправо
            {
                if (InsideBorder(IcurrFigure, j)) //Клетка в пределах доски
                {
                    if (!DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure, j)) //Есть ли на клетке фигура + метод выделения доступной клетки
                        break;
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
            for (int j = JcurrFigure - 1; j >= 0; j--) //Движение влево
            {
                if (InsideBorder(IcurrFigure, j)) //Клетка в пределах доски
                {
                    if (!DeterminePath(IcurrFigure, JcurrFigure, IcurrFigure, j)) //Есть ли на клетке фигура + метод выделения доступной клетки
                        break;
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }
        }


        public void ShowShortCastlingForKing(int IcurrFigure, int JcurrFigure)
        {
            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForShortCastling(map)) //Если король и ладья на правильном месте для короткой рокировки
            {
                if(IsTrerePlaceForShortCastling(map)) //Если место между королем и правой ладьей свободно
                {
                    if(!CanPathGiveUsCheck(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 1) && !CanPathGiveUsCheck(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 2)) //Если ход не принесет шаха и король не идет через битое поле.
                    {
                        butts[IcurrFigure, JcurrFigure + 2].Enabled = true; //Сделать клетку на два хода вправо от короля доступной к нажатию
                        butts[IcurrFigure, JcurrFigure + 2].BackColor = Color.Orange; //Сделать клетку на два хода вправо от короля красной
                    }
                }
            }
        }

        public void ShowLongCastlingForKing(int IcurrFigure, int JcurrFigure)
        {
            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForLongCastling(map)) //Если король и ладья на правильном месте для длинной рокировки
            {
                if (IsTrerePlaceForLongCastling(map)) //Если место между королем и левой ладьей свободно
                {
                    if (!CanPathGiveUsCheck(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 1) && !CanPathGiveUsCheck(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 2)) //Если ход не принесет шаха и король не идет через битое поле.
                    {
                        butts[IcurrFigure, JcurrFigure - 2].Enabled = true; //Сделать клетку на два хода вправо от короля доступной к нажатию
                        butts[IcurrFigure, JcurrFigure - 2].BackColor = Color.Orange; //Сделать клетку на два хода вправо от короля красной
                    }
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

        public bool IsCastleReadyForShortCastling(int[,] desk)
        {
            if ((desk[0,7] == 15 && currPlayer == 1) || (desk[7, 7] == 25 && currPlayer == 2))//Если ладья на правильном месте для короткой рокировки
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
            if ((currPlayer == 1 && desk[0,5]==0 && desk[0, 6] == 0) || (currPlayer == 2 && desk[7, 5] == 0 && desk[7, 6] == 0))//Есть ли место для короткой рокировки у игрока, который ходит
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





        public bool DeterminePath(int I1, int J1, int I2, int J2) //Обозначить доступную клетку, и узнать есть ли на ней фигура
        {
            if (map[I2, J2] == 0) //Если клетка пуста
            {
                if (!CanPathGiveUsCheck(I1, J1, I2, J2)) //Если при ходе наш король не под шахом
                {
                    butts[I2, J2].BackColor = Color.Yellow;
                    butts[I2, J2].Enabled = true;
                }
            }
            else
            {
                if (map[I2, J2] / 10 != currPlayer) //Если на клетке фигура врага
                {
                    if (!CanPathGiveUsCheck(I1, J1, I2, J2)) //Если при ходе наш король не под шахом
                    {
                        butts[I2, J2].BackColor = Color.Yellow;
                        butts[I2, J2].Enabled = true;
                    }
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
                label5.Text = "-"; //Предыдущих ходов нет
            }
            else //Иначе
            {
                label5.Text = WriteTurn(gamehistory[PositionNum - 1], gamehistory[PositionNum]); //Высветить ход, сделанный игроком
            }
            
            button4.Visible = true;//Кнопка "ход вперед" видна

            
            if (IsThrereCheck(map))//Если есть шах
            {
                MakeOurKingRed();//Сделать клетку короля красной
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
                                                         
            //Регуляция показа кнопки "ход вперед"
            if (PositionNum >= gamehistory.Count - 1) //Если номер последнего хода меньше или равно номеру этого хода
            {
                button4.Visible = false;//Кнопка "ход вперед" НЕ видна
            }

            button3.Visible = true; //Кнопка "ход назад" видна


            if (IsThrereCheck(map))//Если есть шах
            {
                MakeOurKingRed();//Сделать клетку короля красной
            }

            label5.Text = WriteTurn(gamehistory[PositionNum - 1], gamehistory[PositionNum]); //Высветить ход, сделанный игроком
        }

        public void ShowChooseButtonsForPawns() //Показать кнопки превращений пешки
        {
            this.Size = new Size(541, 500);//Расширение поля вниз
            
            
            
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


        private void CastleClick(object sender, EventArgs e)//Превращение пешки в ладью
        {
            map[PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 5; //Запись пешки как ладьи
            ActivateAllButtons(); //Активировать все кнопки
            ReDrawMap(); //Перерисовка карты 
            this.Size = new Size(541, 440);

            if (PositionNum >= gamehistory.Count - 1) //Если номер последнего хода меньше или равно номеру этого хода
            {
                gamehistory[PositionNum][PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 5; //запись превращения в историю
            }
            //Очистка Controls
            Controls.Remove(Castle);
            Controls.Remove(Hourse);
            Controls.Remove(Officer);
            Controls.Remove(Queen);
        }

        private void HourseClick(object sender, EventArgs e)//Превращение пешки в коня
        {
            map[PawnI, PawnJ] = (1 + currPlayer%2) * 10 + 4; //Запись пешки как коня
            ActivateAllButtons(); //Активировать все кнопки
            ReDrawMap(); //Перерисовка карты 
            this.Size = new Size(541, 440);

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

        private void OfficerClick(object sender, EventArgs e)//Превращение пешки в офицера
        {
            map[PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 3; //Запись пешки как офицера
            ActivateAllButtons(); //Активировать все кнопки
            ReDrawMap(); //Перерисовка карты 
            this.Size = new Size(541, 440);

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

        private void QueenClick(object sender, EventArgs e)//Превращение пешки в ферзя
        {
            map[PawnI, PawnJ] = (1 + currPlayer % 2) * 10 + 2; //Запись пешки как королевы
            ActivateAllButtons(); //Активировать все кнопки
            ReDrawMap(); //Перерисовка карты 
            this.Size = new Size(541, 440);

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



        public bool IsThrereCheck(int[,] desk)//Проверка на наличие шаха. Метод принимает доску (двумерный массив)
        {
            List<int> Figures = ReturnAllAttackedFigures(desk); //Записать все фигуры которые под ударом в массив
            foreach(int f in Figures)//Перебор фигур под ударом
            {
                if(f == currPlayer*10 + 1)//Если фигура под ударом - наш король
                {
                    return true; //Вернуть true
                }
            }
            return false;//Если король не обнаружен - вернуть false.
        }

        public List<int> ReturnAllAttackedFigures(int[,] desk)//Обнаружение отакованых фигур. Метод принимает доску (двумерный массив)
        {
            List<int> AllAtackedF = new List<int>(); //Все атакованные фигуры
            //Перебор клеток на доске
            for (int i = 0; i<8; i++) 
            {
                for (int j = 0; j < 8; j++)
                {
                    if(desk[i,j]/10 == 1+(currPlayer%2)) //Если на клетке вражеская фигура
                    {
                        switch(desk[i, j]%10)
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
                    if(desk[i,JcurrFigure]!=0) //Если клетка не пуста
                    {
                        if(desk[i,JcurrFigure]/10 == currPlayer) //если отакована наша фигура 
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

        public List<int> ReturnFiguresAttackedDiagonal(int IcurrFigure, int JcurrFigure, int [,] desk, bool isOneStep = false) ////Обнаружение фигур отакованых по косой линии
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
                if (desk[IcurrFigure-2, JcurrFigure + 1] / 10 == currPlayer) //если атакована наша фигура 
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

        public void MakeOurKingRed()//Сделать нашего короля красным
        {
            //Перебор клеток на доске
            for(int i = 0; i<8; i++)
            {
                for (int j=0; j < 8; j++)
                {
                    if(map[i,j]==currPlayer*10+1)//Если на клетке наш король
                    {
                        butts[i, j].BackColor = Color.Red; //Покрасить кнопку в красный
                    }
                }
            }
        }

        public int[,] MakePheudoPath(Path path, int[,] desk1) //Сделать псевдоход при помощи объекта "ход"
        {
            return MakePheudoPath(path.P1.I, path.P1.J, path.P2.I, path.P2.J, desk1);
        }


        public int[,] MakePheudoPath(int I1, int J1, int I2, int J2 ,int[,] desk1) //Сделать псевдоход
        {
            int[,] desk = new int[8,8];

            //Копирование массива desk1 в desk
            for(int i = 0; i<8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    desk[i, j] = desk1[i, j];
                }
            }

            desk[I2, J2] = desk[I1, J1];//Записать в вторую клетку значение первой
            desk[I1, J1] = 0;//Приравнять значение первой кнопки к нулю

            if(desk1[I1, J1]%10 == 1) //Если ходит король
            {
                if(Math.Abs(J1-J2) == 2) //Если король пошел на две клетки по горизнотали (рокировка)
                {
                    if(J1 - J2 == 2) //Если король пошел вправо
                    {
                        //Переместить ладью на два хода влево
                        desk[I1, 5] = desk[I1, 7]; 
                        desk[I1, 7] = 0;
                    }
                    else //Если король пошел влево
                    {
                        //Переместить ладью на три хода вправо
                        desk[I1, 3] = desk[I1, 0];
                        desk[I1, 0] = 0;
                    }
                }
            }

            if(desk1[I1,J1]%10 == 6) //Еcли ходит пешка
            {
                if (I2 == 0 && I2 == 7) //Если после хода пешка на краю доски (по вертикали)
                {
                    desk[I2, J2] -= 4; //Отниять от значения клетки 4, чтобы сделать пешку королевой
                }
            }

            return desk; //Вернуть массив desk

        }

        public bool CanPathGiveUsCheck(int I1, int J1, int I2, int J2)
        {
            return IsThrereCheck(MakePheudoPath(I1, J1, I2, J2, map)); //Есть ли шах на доске, созданной псевдоходом
        }

        public bool CanGiveUsCheckOnDesk(int I1, int J1, int I2, int J2, int[,] desk)
        {
            return IsThrereCheck(MakePheudoPath(I1, J1, I2, J2, desk)); //Есть ли шах на доске, созданной псевдоходом
        }



        public string WriteTurn(int[,] prevdesk, int[,] desk) //Метод записи одного хода в шахматной нотации
        {
            if(WasThrereShortCastling(prevdesk, desk)) //Если была короткая рокировка
            {
                return "O-O";
            }

            if (WasThrereLongCastling(prevdesk, desk)) //Если была длинная рокировка
            {
                return "O-O-O";
            }

            string Figure = null; //Обозначение фигуры
            string FirstPosition = null; //Первая позиция
            string SecondPosition = null; //Вторая позиция
            string Separator = "-"; //Разделитель позиций
            //Перебор предыдущей и этой доски
            for(int i = 0; i<8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(desk[i,j] != prevdesk[i,j]) //Если на клетках разные записи
                    {
                        if(prevdesk[i,j]!=0 && desk[i,j] == 0) //Если на клетке предыдущей доски есть фигура, а на этой клетке нет
                        {
                            Figure = FiguresNotation[prevdesk[i, j] % 10]; //Запись фигуры исходя из последней цифры по словарю
                            FirstPosition = $"{LineNotation[j+1]}{i+1}"; //Запись первой позиции. В начале столбец (буква из словаря), затем строка.
                        }
                        else  //Если на клетке предыдущей доски нет фигуры, или эта фигура - вражеская
                        {
                            if (prevdesk[i, j] != 0)//Если на клетке предыдущей доски вражеская фигурап
                            {
                                Separator = ":";//Изменить разделитель позиций
                            }
                            SecondPosition = $"{LineNotation[j+1]}{i+1}"; //Запись второй позиции. В начале столбец (буква из словаря), затем строка.
                        }
                    }
                }
            }
            
            string Path = $"{Figure}{FirstPosition}{Separator}{SecondPosition}"; //Записать ход в последовательности: фигура, первая позиция, тире или двоеточие, вторая позиция.

            if(IsThrereCheck(desk)) //Если на этой доске шах
            {
                
                if(CountYourPathes(desk)==0) //Если ходы невозможны
                {
                    Path += "x"; //Добавить к записи хода значок "x"
                }
                else //Если ходы возможны 
                { 
                    Path += "+"; //Добавить к записи хода значок "+"
                }
                
            }
            
            return Path;//Вернуть ход
        }

        public bool WasThrereShortCastling(int[,] prevdesk, int[,] desk) //Была ли короткая рокировка
        {
            if ((prevdesk[0,4]%10==1 && desk[0, 6] % 10 == 1) || (prevdesk[7, 4] % 10 == 1 && desk[7, 6] % 10 == 1)) //Если король переместился с позиции e1 на g1, или с e8 на g8
            {
                return true; //вернуть true
            }
            return false; //вернуть false
        }

        public bool WasThrereLongCastling(int[,] prevdesk, int[,] desk) //Была ли длинная рокировка
        {
            if ((prevdesk[0, 4] % 10 == 1 && desk[0, 2] % 10 == 1) || (prevdesk[7, 4] % 10 == 1 && desk[7, 2] % 10 == 1)) //Если король переместился с позиции e1 на c1, или с e8 на c8
            {
                return true; //вернуть true
            }
            return false; //вернуть false
        }

        public List<string> ConvertHistoryToStringList (List<int[,]> history)
        {
            List<string> Pathes = new List<string>(); //Список ходов
            currPlayer = 2; //Играют черные
            //Перебор списка досок
            for(int i = 1; i<history.Count; i++)
            {
                Pathes.Add(WriteTurn(history[i - 1], history[i]));
                SwitchPlayer();//Сменить игрока
            }

            return Pathes; //Вернуть список ходов
        }

        public void WriteHistorytoFile(List<string> history, string filename)
        {
            StreamWriter sw = new StreamWriter($"{filename}.txt"); //Создать файл с названием filename
            //Перебор истории
            for(int i = 0; i < history.Count; i++)
            {
                sw.WriteLine($"{i + 1}.{history[i]}");
            }
            sw.Close();
        }

        public int CountYourPathes(int[,] desk)//Метод счета ваших ходов
        {
            int AllPathesCount = 0; //Количество ходов
            
            //перебор клеток доски
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(desk[i,j]/10 == currPlayer) //Если нам клетке наша фигура
                    {
                        switch(desk[i,j]%10)
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
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure, desk)))//Если после хода наш король не под ударом
                    {
                        PathesCount++; //Увеличить количество ходов на один
                    }

                    if (InsideBorder(IcurrFigure + 2 * dir, JcurrFigure)) //Находиться ли прямой двойной ход пешкой в пределах доски
                    {
                        if ((IcurrFigure == 1 && currPlayer == 1 || IcurrFigure == 6 && currPlayer == 2) & desk[IcurrFigure + 2 * dir, JcurrFigure] == 0) // Если есть возможность сделать два хода вперед
                        {
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2 * dir, JcurrFigure, desk)))//Если после хода наш король не под ударом
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
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure + 1, desk)))//Если после хода наш король не под ударом
                    {
                        PathesCount++; //Увеличить количество ходов на один
                    }
                }
            }
            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1)) ////Находиться ли левый атакующий (косой) ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && desk[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                {
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure - 1, desk)))//Если после хода наш король не под ударом
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
                    if(desk[i, JcurrFigure] != 0)//Если на клетке есть фигура
                    {
                        if(desk[i, JcurrFigure] /10 != currPlayer)//Если фигура не наша
                        {
                            if(!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, JcurrFigure, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, JcurrFigure, desk))) //Если после хода нет шаха
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
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, JcurrFigure, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, JcurrFigure, desk))) //Если после хода нет шаха
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
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThrereCheck(MakePheudoPath(IcurrFigure, IcurrFigure, IcurrFigure, j, desk))) //Если после хода нет шаха
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
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThrereCheck(MakePheudoPath(IcurrFigure, IcurrFigure, IcurrFigure, j, desk))) //Если после хода нет шаха
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
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
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
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
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
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
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
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
                            {
                                PathesCount++; //Увеличить количество ходов на один
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, i, j, desk))) //Если после хода нет шаха
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
                if (desk[IcurrFigure - 2, JcurrFigure + 1] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure + 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 2, JcurrFigure - 1] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure - 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 2, JcurrFigure + 1] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure + 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 2, JcurrFigure - 1] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure - 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 1, JcurrFigure + 2] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure + 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 1, JcurrFigure + 2] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure + 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 1, JcurrFigure - 2] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure - 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    PathesCount++; //Увеличить количество ходов на один
                }
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 1, JcurrFigure - 2] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure - 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
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
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 1, desk)) && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 2, desk))) //Если ход не принесет шаха и король не идет через битое поле.
                    {
                        PathesCount++; //Добавить ход
                    }
                }
            }

            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForLongCastling(desk)) //Если король и ладья на правильном месте для длинной рокировки
            {
                if (IsTrerePlaceForLongCastling(desk)) //Если место между королем и левой ладьей свободно
                {
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 1, desk)) && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 2, desk))) //Если ход не принесет шаха и король не идет через битое поле.
                    {
                        PathesCount++; //Добавить ход
                    }
                }
            }

            return PathesCount; //Вернуть количество ходов
        }

        //Дать список ходов из любой доски
        public List<Path> GivePathes(int[,] desk)
        {
            List<Path> AllPathes = new List<Path>(); //Список всех ходов

            //Перебор массива 
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(desk[i,j]/10 == currPlayer)//Если на клетке наша фигура
                    {
                        switch(desk[i, j] % 10)
                        {
                            case 6://Для пешек

                                //Перебрать ходы фигуры (cписок ходов пешки)
                                foreach (Path p in GivePawnPathes(desk, i, j))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }

                                break;

                            case 5://Для ладей
                                
                                //Перебрать ходы фигуры (список ходов ладьи)
                                foreach (Path p in GiveHorizontalVericalPathes(desk, i, j))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }

                                break;

                            case 4://Для коней



                                //Перебрать ходы фигуры (список ходов коня)
                                foreach (Path p in GiveHorsePathes(desk, i, j))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }

                                break;

                            case 3://Для офицеров

                                //Перебрать ходы фигуры (список ходов офицера)
                                foreach (Path p in GiveDioganalPathes(desk, i, j))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }

                                break;

                            case 2://Для ферзей


                                //Перебрать ходы фигуры (список ходов офицера)
                                foreach (Path p in GiveDioganalPathes(desk, i, j))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }

                                //Перебрать ходы фигуры (список ходов ладьи)
                                foreach (Path p in GiveHorizontalVericalPathes(desk, i, j))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }

                                break;

                            case 1://Для короля

                                //Перебрать ходы фигуры (список диагональных ходов короля)
                                foreach (Path p in GiveDioganalPathes(desk, i, j, true))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }

                                //Перебрать ходы фигуры (список вертикально-горизонтальных ходов короля)
                                foreach (Path p in GiveHorizontalVericalPathes(desk, i, j, true))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }


                                //Перебрать ходы фигуры (cписок рокировочных ходов короля)
                                foreach (Path p in GiveCastlingPathes(desk, i, j))
                                {
                                    AllPathes.Add(p); //Добавить ход фигуры в список всех ходов
                                }

                                break;
                        }
                    }
                }
            }

            return AllPathes; //Возвращаем все ходы
        }

        //Дать все ходы пешки
        public List<Path> GivePawnPathes(int[,] desk, int IcurrFigure, int JcurrFigure)
        {
            List<Path> Pathes = new List<Path>(); //Список ходов

            int dir = currPlayer == 1 ? 1 : -1; //(если играют белые  dir = 1, иначе dir = -1) dir переменная по направлению пешки.


            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure)) //Находиться ли прямой ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure] == 0) //Если на клетке нет фигур
                {
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure, desk)))//Если после хода наш король не под ударом
                    {
                        Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure)); //Добавить этот ход
                    }

                    if (InsideBorder(IcurrFigure + 2 * dir, JcurrFigure)) //Находиться ли прямой двойной ход пешкой в пределах доски
                    {
                        if ((IcurrFigure == 1 && currPlayer == 1 || IcurrFigure == 6 && currPlayer == 2) & desk[IcurrFigure + 2 * dir, JcurrFigure] == 0) // Если есть возможность сделать два хода вперед
                        {
                            if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2 * dir, JcurrFigure, desk)))//Если после хода наш король не под ударом
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure + 2 * dir, JcurrFigure)); //Добавить этот ход
                            }
                        }
                    }
                }
            }

            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure + 1)) //Находиться ли правый атакующий (косой) ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 && desk[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                {
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure + 1, desk)))//Если после хода наш король не под ударом
                    {
                        Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure + 1)); //Добавить этот ход
                    }
                }
            }
            if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1)) ////Находиться ли левый атакующий (косой) ход пешкой в пределах доски
            {
                if (desk[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 && desk[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != currPlayer) //Два условия: клетка не пуста, фигура на ней - вражеская
                {
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure - 1, desk)))//Если после хода наш король не под ударом
                    {
                        Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure + 1 * dir, JcurrFigure - 1)); //Добавить этот ход
                    }
                }
            }

            List<Path> Pathes2 = new List<Path>(); //Список конечных ходов

            //Перебор начального списка ходов
            foreach (Path p in Pathes)
            {
                if (!CanGiveUsCheckOnDesk(p.P1.I, p.P1.J, p.P2.I, p.P2.J, desk)) //Если при ходе нет мата 
                {
                    Pathes2.Add(p);//Добавить ход в конечный список ходов
                }
            }


            return Pathes2; //Вернуть конечный список ходов
        }

        //Дать все вертикально-горизонтальные ходы
        public List<Path> GiveHorizontalVericalPathes(int[,] desk, int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            List<Path> Pathes = new List<Path>(); //Список ходов

            for (int i = IcurrFigure + 1; i < 8; i++) //Движение вверх
            {
                if (InsideBorder(i, JcurrFigure)) //Клетка в пределах доски
                {
                    if (desk[i, JcurrFigure] != 0)//Если на клетке есть фигура
                    {
                        if (desk[i, JcurrFigure] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, JcurrFigure, desk)) //Если после хода нет шаха
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, JcurrFigure)); //Добавить этот ход
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, JcurrFigure, desk)) //Если после хода нет шаха
                        {
                            Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, JcurrFigure)); //Добавить этот ход
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
                            if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, JcurrFigure, desk)) //Если после хода нет шаха
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, JcurrFigure)); //Добавить этот ход
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, JcurrFigure, desk)) //Если после хода нет шаха
                        {
                            Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, JcurrFigure)); //Добавить этот ход
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
                            if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, IcurrFigure, j, desk)) //Если после хода нет шаха
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure, j)); //Добавить этот ход
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!CanGiveUsCheckOnDesk(IcurrFigure, IcurrFigure, IcurrFigure, j, desk)) //Если после хода нет шаха
                        {
                            Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure, j)); //Добавить этот ход
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
                            if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, IcurrFigure, j, desk)) //Если после хода нет шаха
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure, j)); //Добавить этот ход
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!CanGiveUsCheckOnDesk(IcurrFigure, IcurrFigure, IcurrFigure, j, desk)) //Если после хода нет шаха
                        {
                            Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure, j)); //Добавить этот ход
                        }
                    }
                }
                if (isOneStep) //Если у фигуры только один ход
                    break;
            }

            List<Path> Pathes2 = new List<Path>(); //Список конечных ходов

            //Перебор начального списка ходов
            foreach (Path p in Pathes)
            {
                if (!CanGiveUsCheckOnDesk(p.P1.I, p.P1.J, p.P2.I, p.P2.J, desk)) //Если при ходе нет мата 
                {
                   Pathes2.Add(p);//Добавить ход в конечный список ходов
                }
            }
            
            
            return Pathes2; //Вернуть конечный список ходов
        }

        //Дать все диагональные ходы
        public List<Path> GiveDioganalPathes(int[,] desk, int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            List<Path> Pathes = new List<Path>(); //Список ходов

            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (desk[i, j] != 0)//Если на клетке есть фигура
                    {
                        if (desk[i, j] / 10 != currPlayer)//Если фигура не наша
                        {
                            if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, j, desk)) //Если после хода нет шаха
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, j)); //Добавить этот ход
                            }
                        }
                        break;
                        
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, j, desk)) //Если после хода нет шаха
                        {
                            Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, j)); //Добавить этот ход
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
                            if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, j, desk)) //Если после хода нет шаха
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, j)); //Добавить этот ход
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, j, desk)) //Если после хода нет шаха
                        {
                            Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, j)); //Добавить этот ход
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
                            if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, j, desk)) //Если после хода нет шаха
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, j)); //Добавить этот ход
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, j, desk)) //Если после хода нет шаха
                        {
                            Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, j)); //Добавить этот ход
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
                            if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, j, desk)) //Если после хода нет шаха
                            {
                                Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, j)); //Добавить этот ход
                            }
                        }
                        break; //Закончить цикл
                    }
                    else //Если на клетке нет фигуры
                    {
                        if (!CanGiveUsCheckOnDesk(IcurrFigure, JcurrFigure, i, j, desk)) //Если после хода нет шаха
                        {
                            Pathes.Add(new Path(IcurrFigure, JcurrFigure, i, j)); //Добавить этот ход
                        }
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            List<Path> Pathes2 = new List<Path>(); //Список конечных ходов

            //Перебор начального списка ходов
            foreach (Path p in Pathes)
            {
                if (!CanGiveUsCheckOnDesk(p.P1.I, p.P1.J, p.P2.I, p.P2.J, desk)) //Если при ходе нет мата 
                {
                    Pathes2.Add(p);//Добавить ход в конечный список ходов
                }
            }


            return Pathes2; //Вернуть конечный список ходов
        }

        //Дать все ходы конем
        public List<Path> GiveHorsePathes(int[,] desk, int IcurrFigure, int JcurrFigure)
        {
            List<Path> Pathes = new List<Path>(); //Список ходов

            if (InsideBorder(IcurrFigure - 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 2, JcurrFigure + 1] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure + 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure + 1)); //Добавить этот ход
                }
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 2, JcurrFigure - 1] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure - 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure - 2, JcurrFigure - 1)); //Добавить этот ход
                }
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 2, JcurrFigure + 1] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure + 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure + 1)); //Добавить этот ход
                }
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 2, JcurrFigure - 1] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure - 1, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure + 2, JcurrFigure - 1)); //Добавить этот ход
                }
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 1, JcurrFigure + 2] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure + 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах
                {
                    Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure + 2)); //Добавить этот ход
                }
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 1, JcurrFigure + 2] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure + 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure + 2)); //Добавить этот ход
                }
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure - 1, JcurrFigure - 2] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure - 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure - 1, JcurrFigure - 2)); //Добавить этот ход
                }
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2)) //Клетка в пределах доски
            {
                if (desk[IcurrFigure + 1, JcurrFigure - 2] / 10 != currPlayer && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure - 2, desk))) //если на клетке нет нашей фигуры, и ход не дает нам шах 
                {
                    Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure + 1, JcurrFigure - 2)); //Добавить этот ход
                }
            }

            List<Path> Pathes2 = new List<Path>(); //Список конечных ходов

            //Перебор начального списка ходов
            foreach (Path p in Pathes)
            {
                if (!CanGiveUsCheckOnDesk(p.P1.I, p.P1.J, p.P2.I, p.P2.J, desk)) //Если при ходе нет мата 
                {
                    Pathes2.Add(p);//Добавить ход в конечный список ходов
                }
            }


            return Pathes2; //Вернуть конечный список ходов
        }


        //Дать все рокировочные ходы
        public List<Path> GiveCastlingPathes(int[,] desk, int IcurrFigure, int JcurrFigure)
        {
            List<Path> Pathes = new List<Path>(); //Список ходов

            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForShortCastling(desk)) //Если король и ладья на правильном месте для короткой рокировки
            {
                if (IsTrerePlaceForShortCastling(desk)) //Если место между королем и правой ладьей свободно
                {
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 1, desk)) && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 2, desk))) //Если ход не принесет шаха и король не идет через битое поле.
                    {
                        Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure + 2)); //Добавить этот ход
                    }
                }
            }

            if (IsKingReadyForCastling(IcurrFigure, JcurrFigure) && IsCastleReadyForLongCastling(desk)) //Если король и ладья на правильном месте для длинной рокировки
            {
                if (IsTrerePlaceForLongCastling(desk)) //Если место между королем и левой ладьей свободно
                {
                    if (!IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 1, desk)) && !IsThrereCheck(MakePheudoPath(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 2, desk))) //Если ход не принесет шаха и король не идет через битое поле.
                    {
                        Pathes.Add(new Path(IcurrFigure, JcurrFigure, IcurrFigure, JcurrFigure - 2)); //Добавить этот ход
                    }
                }
            }

            List<Path> Pathes2 = new List<Path>(); //Список конечных ходов


            //Перебор начального списка ходов
            foreach (Path p in Pathes)
            {
                if (!CanGiveUsCheckOnDesk(p.P1.I, p.P1.J, p.P2.I, p.P2.J, desk)) //Если при ходе нет мата 
                {
                    Pathes2.Add(p);//Добавить ход в конечный список ходов
                }
            }


            return Pathes2; //Вернуть конечный список ходов
        }

        //Конвертация списка ходов в позиции
        public List<GamePosition> ConvertPathesToGamePositions (List<Path> Pathes, int[,] desk)
        {
            List<GamePosition> Positions = new List<GamePosition>(); //Создать список позиций

            //Перебор массива ходов
            foreach(Path p in Pathes)
            {
                Positions.Add(new GamePosition(p, MakePheudoPath(p, desk), currPlayer)); //Добавить позицию
            }

            return Positions;
        }

        //Сделать случайный ход
        public void MakeRandomPath()
        {

            DeactivateAllButtons();//Деактивировать все кнопки
            List<Path> Pathes = GivePathes(map);//Массив ходов
            if(Pathes.Count == 0)//Если ходов нет
            {
                ActivateAllButtons(); //Активировать все кнопки
            }
            else
            {
                Path RandomPath = Pathes[rnd.Next(0, Pathes.Count - 1)]; //Выбор случайного хода
                int[,] newDesk = MakePheudoPath(RandomPath, map);//Создание псевдохода

                //Перебор доски
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        map[i, j] = newDesk[i, j]; //Занесение позиции новой доски на действительную
                    }
                }

                SwitchPlayer(); // Сменить игрока

                PositionNum++; //Увеличить номер позиции

                // Ход входит в историю
                gamehistory.Add(new int[8, 8]);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {

                        gamehistory[PositionNum][i, j] = map[i, j];
                    }
                }

                label5.Text = WriteTurn(gamehistory[PositionNum - 1], gamehistory[PositionNum]); //Высветить ход, сделанный игроком

                ReDrawMap();//Перерисовать карту

                ActivateAllButtons();//Активировать все кнопки
            }
        }

        //Конвертациия ходов в деревья ходов
        public List<PathesTree> ConvertPathestoTrees(List<Path> Pathes, int[,] desk)
        {
            List<GamePosition> Positions = ConvertPathesToGamePositions(Pathes, desk); //Получение списка позиций

            List<PathesTree> Trees = new List<PathesTree>(); //Список деревьев

            //Перечисление элементов списка позиций
            for(int i = 0; i < Positions.Count; i++)
            {
                Trees.Add(new PathesTree(Positions[i])); //Добавление дерева ходов
                Trees[i].GamePosition.currPlayer = ComputerPlayer; //В дереве у позиции ход компьютера (необходимо для оценки позиции в пользу компьютера)
            }

            return Trees; //Вернуть список деревьев
        }

        //Изменить текущую доску
        public void ChangeMap(int[,] desk)
        {
            //Перебор клеток доски
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    map[i, j] = desk[i, j]; //Клетка старой доски приравниваеться новой
                }
            }
        }
        
        public void MakePathAI()
        {
            List<Path> FirstPathes = GivePathes(map); //Список возможных первых ходов

            List<PathesTree> Trees = ConvertPathestoTrees(FirstPathes, map); //Список деревьев ходов
            
            if(Trees.Count() != 0) //Если количество ходов больше нуля
            {
                //TreeNum = 0; //Слой дерева нулевой

                ////Просчет ветвей для каждого дерева ходов
                //foreach (PathesTree t in Trees)
                //{
                //    //Перебор списка деревьев в каждом дереве из первого списка
                //    foreach (PathesTree t2 in GiveTurnsToTree(t))
                //    {
                //        t.PathesTrees.Add(t2); //Добавить дерево
                //    }
                //}



                double MaxMark = -1; //Максимальная оценка хода
                int NumOfMaxTree = 0; //Номер дерева с максимальной оценкой

                //Переребрать все деревья
                for (int i = 0; i < Trees.Count; i++)
                {
                    if (Trees[i].GetMark() > MaxMark) //Если оценка больше максимальной
                    {
                        MaxMark = Trees[i].GetMark(); //Маскимальная оценка - оценка этого дерева
                        NumOfMaxTree = i; //Номер максимального дерева - этот
                    }
                }

                ChangeMap(Trees[NumOfMaxTree].GamePosition.Desk); //Изменить карту, на доску дерева с максимальной оценкой
                
                SwitchPlayer(); //Сменить игрока

                PositionNum++; //Увеличить номер позиции

                // Ход входит в историю
                gamehistory.Add(new int[8, 8]);
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {

                        gamehistory[PositionNum][i, j] = map[i, j];
                    }
                }

                label5.Text = WriteTurn(gamehistory[PositionNum - 1], gamehistory[PositionNum]); //Высветить ход, сделанный игроком

                ReDrawMap();//Перерисовать карту

                ActivateAllButtons();//Активировать все кнопки
            }



        }

        ////Просчитать ветви дерева
        //public List<PathesTree> GiveTurnsToTree(PathesTree tree)
        //{
        //    TreeNum++; //Повысить слой дерева

        //    List<Path> Pathes = GivePathes(tree.GamePosition.Desk); //Список возможных ходов
        //    List<PathesTree> Trees = ConvertPathestoTrees(Pathes, tree.GamePosition.Desk); //Список деревьев ходов

        //    //Перебор списка деревьев
        //    for (int i = 0; i < Trees.Count; i++)
        //    {
        //        if (TreeNum < MaxTreeNum) //Если текущий слой дерева меньше максимального
        //        {
        //            //Перебор списка деревьев в каждом дереве из первого списка
        //            foreach (PathesTree t in GiveTurnsToTree(Trees[i]))
        //            {
        //                Trees[i].PathesTrees.Add(t); //Добавить дерево
        //            }
        //        }
        //    }

        //    return Trees;
        //}


        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e) //Если на кнопку сохранить нажали
        {
            if(textBox1.Text != null) //Если текстбокс не пустой
            {
                WriteHistorytoFile(ConvertHistoryToStringList(gamehistory), textBox1.Text); //Записать историю игры в файл
            }
        }

        //Кнопка "играть с компьютером за белых"
        private void button6_Click(object sender, EventArgs e)
        {
            IsComputerPlaying = true; //Компьютер играет в шахматы
            ComputerPlayer = 2; //Компьбтер играет за черных
        }

        //Кнопка "играть с компьютером за черных"
        private void button7_Click(object sender, EventArgs e)
        {
            IsComputerPlaying = true; //Компьютер играет в шахматы
            ComputerPlayer = 1; //Компьбтер играет за белых
            MakePathAI();//Сделать ход искуственного интеллекта
        }
    }

}
