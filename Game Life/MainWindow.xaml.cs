using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;
using ExtensionLibrary;
using System.Threading;

namespace Game_Life
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int timerInterval = 1000; // время обновления таймера в мс
        private int k; // коэффициент, на который умножается время отрисовки
        private int size; // размер клеток
        private bool[,] rectanglesLivesArray; // массив, хранящий состояния всех клеток
        private Rectangle[,] rectanglesArray; // массив, хранящий клетки
        private bool indicMoore; // переключает режим между соседями со всех сторон (соседство Мура) - true, к соседям только по вертикалям и горизонталям (соседство Фон Неймана) - false
        private int startLife = 3; // число, которое задает начальное кол-во соседей, при которых клетка рождается
        private int endLife = 3; // число, которое задает конечное кол-во соседей, при которых клетка рождается
        private bool randInd = false; // индикатор включен ли режим рандомного расрееделения живых клеток
        private int density = 9; // число, определеяющее плотность заселения
        //private int startDie = 4; // число, которое задает начальное кол-во соседей, при которых клетка умирает
        //private int endDie = 9; // число, которое задает конечное кол-во соседей, при которых клетка умирает

        // S_3/D_0123456789   S_5678/D_45678    S_1/D_0123456789 
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            K = 5;
            Size = 17;
            timer.Tick += Timer_Tick;
            DensityObjectsInMenu.IsEnabled = false;
            DensityObjectsInMenu.Opacity = 0.5;
            Print();
        }


        private int Size
        {
            get => size;
            set
            {
                size = value;
                SixeOfSquare.Text = (size - 10).ToString();
            }
        }
        private int K
        {
            get => k;
            set
            {
                k = value;
                timer.Interval = new TimeSpan(0, 0, 0, 0, timerInterval / k);
                TimerInterval.Text = k.ToString();
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (randInd)
            {
                PrintNewGeneration();
            }
            Logic();
        }
        /// <summary>
        /// Управляет логикой игры "Жизнь"
        /// </summary>
        private void Logic()
        {
            int aWidth = Convert.ToInt32(CanvasMap.Width / Size);
            int aHeight = Convert.ToInt32(CanvasMap.Height / Size);
            bool[,] newRectanglesLivesArray = new bool[aHeight, aWidth];
            for (int i = 0; i < rectanglesLivesArray.GetLength(0); i++)
            {
                for (int j = 0; j < rectanglesLivesArray.GetLength(1); j++)
                {
                    int neighbours =  CheckNeighbors(i, j);
                    if (neighbours == endLife && !rectanglesLivesArray[i, j])
                        newRectanglesLivesArray[i, j] = true;
                    else if (neighbours >= startLife && neighbours <= endLife && rectanglesLivesArray[i, j])
                        newRectanglesLivesArray[i, j] = rectanglesLivesArray[i, j];

                    else
                        newRectanglesLivesArray[i, j] = false;
                }
            }
                rectanglesLivesArray = newRectanglesLivesArray;            
            PrintNewGeneration();
        }
        /// <summary>
        /// Отрисовывае новое поколение
        /// </summary>
        private void PrintNewGeneration()
        {
            var rand = new Random();
            for (int i = 0; i < rectanglesLivesArray.GetLength(0); i++)
            {
                for (int j = 0; j < rectanglesLivesArray.GetLength(1); j++)
                {
                    if (randInd)
                    {
                        if (!rectanglesLivesArray[i, j])
                        {                            
                            rectanglesLivesArray[i, j] = rand.Next(0, density) == 2;
                        }
                    }
                    if (rectanglesLivesArray[i, j])
                        rectanglesArray[i, j].Fill = Brushes.LightGreen;
                    else
                        rectanglesArray[i, j].Fill = Brushes.Black;
                }
            }
            randInd = false;
        }
        /// <summary>
        /// Проверяет кол-во соседей у клетки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CheckNeighbors(int x, int y)
        {
            int neighbours = 0;
            if (indicMoore)
            {
                for (int i = -1; i < 2; i++) 
                {
                    for (int j = -1; j < 2; j++)
                    {
                        int row = (x + i + rectanglesLivesArray.GetLength(0)) % rectanglesLivesArray.GetLength(0); // если выхожу за массив, то смотрю элемент снизу карты
                        int col = (y + j + rectanglesLivesArray.GetLength(1)) % rectanglesLivesArray.GetLength(1); 
                        bool isSelfChecking = row == x && col == y;
                        if (rectanglesLivesArray[row, col] && !isSelfChecking)
                            neighbours++;
                    }
                }
            }
            else
            {
                int row = (x + 1 + rectanglesLivesArray.GetLength(0)) % rectanglesLivesArray.GetLength(0); 
                int col = (y + 1 + rectanglesLivesArray.GetLength(1)) % rectanglesLivesArray.GetLength(1);
                int row1 = (x - 1 + rectanglesLivesArray.GetLength(0)) % rectanglesLivesArray.GetLength(0);
                int col1 = (y - 1 + rectanglesLivesArray.GetLength(1)) % rectanglesLivesArray.GetLength(1);
                if (rectanglesLivesArray[row, y])
                    neighbours++;
                if (rectanglesLivesArray[row1, y])
                    neighbours++;
                if (rectanglesLivesArray[x, col]) 
                    neighbours++;
                if (rectanglesLivesArray[x, col1])
                    neighbours++;
            }
            return neighbours;
        }
        /// <summary>
        /// При нажатии мыши делает клетку "живой"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetLife(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                int i = (int)Math.Floor(Mouse.GetPosition(rectanglesArray[0, 0]).Y / Size );
                int j = (int)Math.Floor(Mouse.GetPosition(rectanglesArray[0, 0]).X / Size);
                if (rectanglesLivesArray[i, j] == false)
                {
                    rectanglesLivesArray[i, j] = true;
                    rectanglesArray[i, j].Fill = Brushes.LightGreen;
                }
                else
                {
                    rectanglesLivesArray[i, j] = false;
                    rectanglesArray[i, j].Fill = Brushes.Black;
                }
            }
        }
        private void Print()
        {
            int aWidth = Convert.ToInt32(CanvasMap.Width / Size); // получаю сколько в ширину могу уместить квадратов
            int aHeight = Convert.ToInt32(CanvasMap.Height / Size); // получаю сколько в высоту могу уместить квадратов 
            CanvasMap.Children.Clear();
            rectanglesLivesArray = new bool[aHeight, aWidth];
            rectanglesArray = new Rectangle[aHeight, aWidth];
            for (int i = 0; i < aHeight; i++)
            {
                for (int j = 0; j < aWidth; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Width = Size - 1.0;
                    rectangle.Height = Size - 1.0;
                    rectangle.MouseDown += SetLife;
                    rectangle.Opacity = 0.5;
                    Canvas.SetLeft(rectangle, j * Size);
                    Canvas.SetTop(rectangle, i * Size);
                    rectanglesArray[i, j] = rectangle;
                    rectangle.Fill = Brushes.Black;
                    CanvasMap.Children.Add(rectangle);
                }
            }
        }
        /// <summary>
        /// Уменьшает коэффициент на 1, если он > 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntervalDown_Click(object sender, RoutedEventArgs e)
        {
            if (K - 1 < 1)
                return;
            else
                K--;
        }
        /// <summary>
        /// Увеличивает коэффициент на 1, если он < 25
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntervalUp_Click(object sender, RoutedEventArgs e)
        {
            if (K + 1 > 25)
                return;
            else
                K++;
        }
        /// <summary>
        /// Запускает таймер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        { 
            if (StartSettings())
            {
                Start.IsEnabled = false;
                IntervalDown.IsEnabled = false;
                IntervalUp.IsEnabled = false;
                SizeDown.IsEnabled = false;
                SizeUp.IsEnabled = false;
                MyMenu.IsEnabled = false;
                IndicMoore.IsEnabled = false;
                IndicRand.IsEnabled = false;
                Start.Opacity = 0.5;
                IndicRand.Opacity = 0.5;
                IndicMoore.Opacity = 0.5;
                MyMenu.Opacity = 0.5;
                IntervalDown.Opacity = 0.5;
                IntervalUp.Opacity = 0.5;
                SizeDown.Opacity = 0.5;
                SizeUp.Opacity = 0.5;
                timer.Start();
            }
        }
        /// <summary>
        /// Задает начальные настройки игры
        /// </summary>
        private bool StartSettings()
        {
            indicMoore = !(bool)IndicMoore.IsChecked;
            if (StartLifeSetting(ref startLife, StartLife, ref endLife, EndLife, 0))
                //if (StartLifeSetting(ref startDie, StartDie, ref endDie, EndDie, 1))
                    return true;
            return false;
        }
        /// <summary>
        /// Устанавливает значения кол-ва клеток необходимых для зарождения клетки и вымирания клетки
        /// </summary>
        /// <param name="startValue">   Начальная граница    </param>
        /// <param name="itemStart">    Источник значения    </param>
        /// <param name="endValue">     Конечная граница     </param>
        /// <param name="itemEnd">      Источник значения    </param>
        /// <param name="mode">     0 - для зарождения клетки, 1 - для вымирания клетки     </param>
        private bool StartLifeSetting(ref int startValue, TextBox itemStart, ref int endValue, TextBox itemEnd, int mode)
        {
            int tempStart = startValue;
            int tempEnd = endValue;
            try
            {
                startValue = itemStart.Text.ToInt();
                endValue = itemEnd.Text.ToInt();
                if (startValue == 0 && mode == 0 || startValue >= endValue ||
                    startValue >= 9 || endValue >= 9 || startValue < 0 && mode == 1 || endValue < 0)   // для зарождения жизни не может быть 0 || значения начала не может быть больше значения конца
                    throw new Exception();
                return true;
            }
            catch (Exception)
            {
                startValue = tempStart;
                endValue = tempEnd;
                itemStart.Text = startValue.ToString();
                itemEnd.Text = endValue.ToString();
                if (mode == 0)
                    MessageBox.Show("Значение должно быть в пределах от 1 до 8 включительно.\n" +
                        "Начальное значение должно быть меньше конечного.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                //else
                //    MessageBox.Show("Значение должно быть в пределах от 0 до 8 включительно.\n" +
                //       "Начальное значение должно быть меньше конечного.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        /// <summary>
        /// Останавливает таймер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                Start.IsEnabled = true;
                IntervalDown.IsEnabled = true;
                IntervalUp.IsEnabled = true;
                SizeDown.IsEnabled = true;
                SizeUp.IsEnabled = true;
                MyMenu.IsEnabled = true;
                IndicMoore.IsEnabled = true;
                Start.Opacity = 1;
                IndicMoore.Opacity = 1;
                MyMenu.Opacity = 1;
                IntervalDown.Opacity = 1;
                IntervalUp.Opacity = 1;
                SizeDown.Opacity = 1;
                SizeUp.Opacity = 1;
            }
        }
        /// <summary>
        /// Уменьшает размер на 1, если он > 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SizeDown_Click(object sender, RoutedEventArgs e)
        {
            if (Size - 1 - 10 < 1) 
                return;
            else
            {
                Size--;
                Print();
            }
        }
        /// <summary>
        /// Увеличивает размер клетки на 1, если он < 10
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SizeUp_Click(object sender, RoutedEventArgs e)
        {
            if (size + 1 - 10 > 10) 
                return;
            else
            {
                Size++;
                Print();
            }
        }
        /// <summary>
        /// Обработка закрытия окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (timer.IsEnabled) 
                timer.Stop();
            App.Current.Shutdown();
            BindingOperations.ClearAllBindings(TimerInterval);
        }
        /// <summary>
        /// Обработка кнопки перезапуска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Stop_Click(sender, e);
            IndicRand.IsEnabled = true;
            IndicRand.Opacity = 1;
            randInd = true;
            Print();     
        }
        /// <summary>
        /// увеличивает плотность населения, уменьшая density
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DensityUp_Click(object sender, RoutedEventArgs e)
        {
            if (Density.Text.ToInt() < 5)
            {
                density -= 1;
                Density.Text = (Density.Text.ToInt() + 1).ToString();
            }
        }
        /// <summary>
        /// Уменьшает плотность населения, увеличивая density
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DensityDown_Click(object sender, RoutedEventArgs e)
        {
            if (Density.Text.ToInt() > 0)
            {
                density += 1;
                Density.Text = (Density.Text.ToInt() - 1).ToString();
            }
        }
        /// <summary>
        /// Происходит, когда пользователь выбирает рандомное заселение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IndicRand_Click(object sender, RoutedEventArgs e)
        {
            randInd = (bool)IndicRand.IsChecked;
            if (randInd)
            {
                DensityObjectsInMenu.IsEnabled = true;
                DensityObjectsInMenu.Opacity = 1;
            }
            else
            {
                DensityObjectsInMenu.IsEnabled = false;
                DensityObjectsInMenu.Opacity = 0.5;
            }
        }
    }

}
