using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class PathesTree // Древо ходов
    {
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
    }
}
