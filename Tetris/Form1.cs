using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form1 : Form
    {
        public const int width = 15, height = 25, k = 15;// Размеры поля и размер клетки в пикселях
        public int[,] shape = new int[2, 4];          // Массив для хранения падающей фигурки (для каждого блока 2 координаты [0, i] и [1, i]
        public int[,] field = new int[width, height];      // Массив для хранения поля
        public Bitmap bitfield = new Bitmap(k * (width + 1) + 1, k * (height + 3) + 1);//Создаем изображение с размерами (k*(widht)+1) и k*(height + 3) +1
        public Graphics gr; // Для рисования поля на PictureBox
        public Form1()
        {
            InitializeComponent();//Инициализируем форму
            SetShape();//Вызываем процедуру SetShape
        }
        public void FillField() //Рисуем "стакан" в котором будут распологаться объекты
        {
            var gr = Graphics.FromImage(bitfield);
            gr.Clear(Color.Black); //Заливаем черным цветом
            gr.DrawRectangle(Pens.Red, k, k, (width - 1) * k, (height - 1) * k);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    gr.FillRectangle(Brushes.Green, i * k, j * k, (k - 1) * field[i, j], (k - 1) * field[i, j]);
            for (int i = 0; i < 4; i++)
                gr.FillRectangle(Brushes.Red, shape[1, i] * k, shape[0, i] * k, k - 1, k - 1);
            FieldPictureBox.Image = bitfield;
        }
        private void TickTimer_Tick(object sender, EventArgs e)
        {
            if (field[8, 4] == 1)
                Environment.Exit(0);
            foreach (int i in (from i in Enumerable.Range(0, field.GetLength(1)) where (Enumerable.Range(0, field.GetLength(0)).Select(j => field[j, i]).Sum() >= width - 1) select i).ToArray().Take(1))
                for (int k = i; k > 1; k--)
                    for (int l = 1; l < width; l++)
                        field[l, k] = field[l, k - 1];
            Move(0, 1);
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A: Move(-1, 0); break;
                case Keys.D: Move(1, 0); break;
                case Keys.W:
                    var shapeT = new int[2, 4];
                    Array.Copy(shape, shapeT, shape.Length);
                    int maxx = Enumerable.Range(0, 4).Select(j => shape[1, j]).ToArray().Max();
                    int maxy = Enumerable.Range(0, 4).Select(j => shape[0, j]).ToArray().Max();
                    for (int i = 0; i < 4; i++)
                    {
                        int temp = shape[0, i];
                        shape[0, i] = maxy - (maxx - shape[1, i]) - 1;
                        shape[1, i] = maxx - (3 - (maxy - temp)) + 1;
                    }
                    if (FindMistake())
                        Array.Copy(shapeT, shape, shape.Length);
                    break;
                case Keys.S: TickTimer.Interval = 50; break;
            }
        }
        public void SetShape() //Процедура вызова фигур
        {
            Random x = new Random(DateTime.Now.Millisecond); //Задаем x, по которой выбирается одна из 6 фигур
            switch (x.Next(7))
            {
                case 0: shape = new int[,] { { 2, 3, 4, 5 }, { 8, 8, 8, 8 } }; break;
                case 1: shape = new int[,] { { 2, 3, 2, 3 }, { 8, 8, 9, 9 } }; break;
                case 2: shape = new int[,] { { 2, 3, 4, 4 }, { 8, 8, 8, 9 } }; break;
                case 3: shape = new int[,] { { 2, 3, 4, 4 }, { 8, 8, 8, 7 } }; break;
                case 4: shape = new int[,] { { 3, 3, 4, 4 }, { 7, 8, 8, 9 } }; break;
                case 5: shape = new int[,] { { 3, 3, 4, 4 }, { 9, 8, 8, 7 } }; break;
                case 6: shape = new int[,] { { 3, 4, 4, 4 }, { 8, 7, 8, 9 } }; break;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e) => TickTimer.Interval = 250;
        public new void Move(int x, int y)
        {
            for (int i = 0; i < 4; i++)
            {
                shape[1, i] += x; //Смещаем координату фигуры
                shape[0, i] += y; //Смещаем координату фигуры
            }
            if (FindMistake())
            {
                Move(-x, -y);
                if (y != 0)
                {
                    for (int i = 0; i < 4; i++)
                        field[shape[1, i], shape[0, i]]++;
                    SetShape();
                }
            }
            FillField();
        }
        public bool FindMistake()
        {
            for (int i = 0; i < 4; i++)
                if (shape[1, i] >= width || shape[0, i] >= height || shape[1, i] <= 0 || shape[0, i] <= 0 || field[shape[1, i], shape[0, i]] == 1)
                    return true;
            return false;
        }
    }
}

