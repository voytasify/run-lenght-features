using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RunLengthFeatures.Enums;

namespace RunLengthFeatures
{
	public partial class MainWindow
	{
		private Direction _direction;

		public MainWindow()
		{
			InitializeComponent();

			var timer = new Timer(1);
			timer.Elapsed += TimerOnElapsed;
			timer.Start();
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			KeyDown += OnKeyDown;
			KeyUp += OnKeyUp;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Up:
					_direction = Direction.Up;
					break;
				case Key.Down:
					_direction = Direction.Down;
					break;
				case Key.Left:
					_direction = Direction.Left;
					break;
				case Key.Right:
					_direction = Direction.Right;
					break;
			}
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
				_direction = Direction.None;
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				var step = 4;

				switch (_direction)
				{
					case Direction.Up:
						Canvas.SetTop(HoverRectangle, Canvas.GetTop(HoverRectangle) - step);
						break;
					case Direction.Down:
						Canvas.SetTop(HoverRectangle, Canvas.GetTop(HoverRectangle) + step);
						break;
					case Direction.Left:
						Canvas.SetLeft(HoverRectangle, Canvas.GetLeft(HoverRectangle) - step);
						break;
					case Direction.Right:
						Canvas.SetLeft(HoverRectangle, Canvas.GetLeft(HoverRectangle) + step);
						break;
				}
			});
		}
	}
}
