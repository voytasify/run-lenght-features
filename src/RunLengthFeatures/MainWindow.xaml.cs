using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using AwokeKnowing.GnuplotCSharp;
using Microsoft.Win32;
using RunLengthFeatures.Enums;
using RunLengthFeatures.Extensions;
using RunLengthFeatures.Models;
using RunLengthFeatures.Services;
using Rect = OpenCvSharp.Rect;

namespace RunLengthFeatures
{
	public partial class MainWindow
	{
		private readonly Timer _timer;
		private Direction _direction;

		private double CanvasWidth => OverlayCanvas.ActualWidth;
		private double CanvasHeight => OverlayCanvas.ActualHeight;

		private double RectangleWidth => HoverRectangle.ActualWidth;
		private double RectangleHeight => HoverRectangle.ActualHeight;

		public static readonly int ShadesOfGray = 8;
		private readonly RunLengthProvider _runLengthProvider;

		public MainWindow()
		{
			InitializeComponent();

			_timer = new Timer(1);
			_timer.Elapsed += TimerOnElapsed;
			_timer.Start();

			_runLengthProvider = new RunLengthProvider();
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
			if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
				_direction = Direction.None;
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				var step = 5;

				switch (_direction)
				{
					case Direction.Up:
						var top = Math.Max(Canvas.GetTop(HoverRectangle) - step, 0);
						Canvas.SetTop(HoverRectangle, top);
						break;
					case Direction.Down:
						var down = Math.Min(Canvas.GetTop(HoverRectangle) + step, CanvasHeight - RectangleHeight);
						Canvas.SetTop(HoverRectangle, down);
						break;
					case Direction.Left:
						var left = Math.Max(Canvas.GetLeft(HoverRectangle) - step, 0);
						Canvas.SetLeft(HoverRectangle, left);
						break;
					case Direction.Right:
						var right = Math.Min(Canvas.GetLeft(HoverRectangle) + step, CanvasWidth - RectangleWidth);
						Canvas.SetLeft(HoverRectangle, right);
						break;
				}
			});
		}

		private void MainWindow_OnClosed(object sender, EventArgs e)
		{
			_timer.Stop();
		}

		private void MenuItem_LoadImage(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = ".jpeg",
				Filter = "obrazy (*.jpeg, *.png, *.jpg)|*.jpeg;*.png;*.jpg"
			};

			if (dialog.ShowDialog() != true)
				return;

			try
			{
				var image = new BitmapImage(new Uri(dialog.FileName));
				var grayscaleImage = image.ToGrayscale();
				ImageNameTextBlock.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
				ProcessedImage.Source = grayscaleImage;
			}
			catch (Exception)
			{
				MessageBox.Show("Nie udało się wczytać obrazu :(", "Ups..", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private Rect GetHoverRectangleRect()
		{
			return new Rect((int)Canvas.GetLeft(HoverRectangle), (int)Canvas.GetTop(HoverRectangle),
				(int)RectangleWidth, (int)RectangleHeight);
		}

		private BitmapImage GetCurrentImage()
		{
			var img = ProcessedImage.Source as BitmapImage;
			if (img == null)
			{
				MessageBox.Show("Najpierw wybierz obraz", "Ups..", MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}
			return img;
		}

		private void Stat1Clicked(object sender, MouseButtonEventArgs e)
		{
			var image = GetCurrentImage();
			if (image == null)
				return;

			var runLengths = _runLengthProvider.ComputeRunLengths(image, GetHoverRectangleRect());
		}

		private void OnChartClicked(object sender, MouseButtonEventArgs e)
		{
			var image = GetCurrentImage();
			if (image == null)
				return;

			var runs = _runLengthProvider.ComputeRunLengths(image, GetHoverRectangleRect());

			var shades = runs.Select(b => b.Shade).Distinct().ToList();

			var dict = new Dictionary<string, Rlf>();
			foreach (var run in runs)
			{
				var key = run.ToString();
				if (dict.ContainsKey(key))
				{
					dict[key].HowMuch++;
				}
				else
				{
					dict.Add(key, new Rlf
					{
						Shade = shades.IndexOf(run.Shade),
						Length = run.Length
					});
				}
			}


			var runLengths = new double[dict.Count];
			var grayLevels = new double[dict.Count];
			var numberOfRuns = new double[dict.Count];

			var i = 0;
			foreach (var kvp in dict)
			{
				runLengths[i] = kvp.Value.Length;
				numberOfRuns[i] = kvp.Value.HowMuch;
				grayLevels[i] = kvp.Value.Shade;

				++i;
			}

			GnuPlot.Set("xlabel \"Run length\"");
			GnuPlot.Set("ylabel \"Gray level\"");
			GnuPlot.Set("zlabel \"Number of runs\"");
			GnuPlot.Set("title \"Run Length Features Chart\"");

			GnuPlot.Set("hidden3d");
			GnuPlot.Set("dgrid3d 50,50 qnorm 2");
			GnuPlot.SPlot(runLengths, grayLevels, numberOfRuns, "notitle with lines");
		}
	}
}
