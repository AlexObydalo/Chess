using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class PathesTree // Древо ходов
    {
        
        public int Count()
        {
            return PathesTrees.Count;
        }
        
        public bool IsThisTurnLast()
        {
            return Count() == 0;
        }
        public GamePosition GamePosition { get; set; } // Корневая позиция

        public int TurnNum { get; set; } //Номер хода
        
        //Запись дерева по игровой позиции и номеру хода
        public PathesTree(GamePosition gamePosition, int turnNum)
        {
            GamePosition = gamePosition;
            TurnNum = turnNum;
        }

        public List<PathesTree> PathesTrees { get; set; } //Массив ветвей

        //Добавление дерева в массив ветвей
        public void Add(PathesTree pathesTree)
        {
            PathesTrees.Add(pathesTree);
        }

        //Добавление позиции в массив ветвей
        public void Add(GamePosition gamePosition)
        {
            PathesTrees.Add(new PathesTree(gamePosition, TurnNum+1));
        }

        //Возвращение дерева из массива ветвей
        public PathesTree GiveTree(int num)
        {
            return PathesTrees[num];
        }

        //Возвращение позиции из массива ветвей
        public GamePosition GivePosition(int num)
        {
            return PathesTrees[num].GamePosition;
        }

        //Удаление дерева из массива ветвей
        public void DelTree(int num)
        {
            PathesTrees.RemoveAt(num);
        }

        //Получить оценку позиции
        public double GetPositionMark()
        {
            return GamePosition.GetMark();
        }

        //Удалить все ветви кроме ветви с лучшей оценкой
        public void CutLowTrees()
        {
            double maxMark = -2000; //Максимальная оценка (так как минимальная оценка -1000, она в любом случае измениться)
            int MaxTreeNum = 0; //Номер ветви с максимальной оценкой
            
            //Перебор массива ветвей
            for(int i = 0; i < Count(); i++)
            {
                if(PathesTrees[i].GetPositionMark() > maxMark) //Если оценка позиции больше максимальной
                {
                    maxMark = PathesTrees[i].GetPositionMark(); //Записать оценку позиции как максимальную
                    MaxTreeNum = i; //Записать номер дерева как максимальный
                }
            }

            PathesTree MaxTree = PathesTrees[MaxTreeNum]; //Записать дерево с максимальной оценкой

            PathesTrees.Clear(); //Очистка массива ветвей

            PathesTrees.Add(MaxTree); //Добавление в массив ветвей, дерева с максимальной оценкой
        }

        //Получить оценку дерева
        public double GetMark()
        {
            if(!IsThisTurnLast()) //Если это не последний ход
            {
                double minMark = 2000; //Минимальная оценка (так как максимальнаяя оценка 1000, она в любом случае измениться)

                //Перебор массива ветвей
                for (int i = 0; i < Count(); i++)
                {
                    if (PathesTrees[i].GetMark() < minMark) //Если оценка позиции меньше минимальной
                    {
                        minMark = PathesTrees[i].GetPositionMark(); //Записать оценку позиции как минимальную
                    }
                }

                return minMark;
            }
            else
            {
                return GetPositionMark();
            }
        }
    }
}
