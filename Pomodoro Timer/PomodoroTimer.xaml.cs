using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pomodoro_Timer
{
    public partial class PomodoroTimer : ContentPage
    {
        private readonly ProgressArc _progressArc;
        private DateTime _startTime;
        private readonly int _duration = 1 * 10; // 5 minutes in seconds
        private double _progress;
        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _isRunning = false; // New flag to track if the timer is running
        private TimeSpan _elapsedBeforePause = TimeSpan.Zero; // New field to store the elapsed time before pause

        public PomodoroTimer()
        {
            InitializeComponent();
            _progressArc = new ProgressArc();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ProgressButton.Text = "\u25B6";
            ProgressView.Drawable = _progressArc;
        }

        // Handle button click events
        private void StartButton_OnClicked(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                _elapsedBeforePause = DateTime.Now - _startTime; // Store the elapsed time before pause
                _cancellationTokenSource.Cancel();
                _isRunning = false;
            }
            else
            {
                _startTime = DateTime.Now - _elapsedBeforePause; // Add the elapsed time before pause to the start time
                _cancellationTokenSource = new CancellationTokenSource();
                UpdateArc();
                _isRunning = true;
            }
        }
        
        private void ResetButton_OnClicked(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _elapsedBeforePause = TimeSpan.Zero; // Reset the elapsed time before pause
            _startTime = DateTime.Now; // Reset the start time
            _isRunning = false; // Set the running flag to false
            ResetView();
        }
        private async void UpdateArc()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                TimeSpan elapsedTime = DateTime.Now - _startTime;
                TimeSpan timeRemaining = TimeSpan.FromSeconds(_duration - elapsedTime.TotalSeconds);

                string timeRemainingFormatted = $"{timeRemaining:mm\\:ss}"; // Format as mm:ss

                ProgressButton.Text = timeRemainingFormatted;

                _progress = Math.Ceiling(elapsedTime.TotalSeconds);
                _progress %= _duration;
                _progressArc.Progress = _progress / _duration;
                ProgressView.Invalidate();

                if (timeRemaining.TotalSeconds <= 0)
                {
                    _cancellationTokenSource.Cancel();
                    _isRunning = false;
                    _elapsedBeforePause = TimeSpan.Zero; // Reset the elapsed time before pause
                    return;
                }

                await Task.Delay(500);
            }

            ResetView();
        }

        private void ResetView()
        {
            _progress = 0;
            _progressArc.Progress = 100;
            ProgressView.Invalidate();
            ProgressButton.Text = "\u25B6";
        }
    }

    public class ProgressArc : IDrawable
    {
        public double Progress { get; set; } = 100;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Angle of the arc in degrees
            var endAngle = 90 - (int)Math.Round(Progress * 360, MidpointRounding.AwayFromZero);
            // Drawing code goes here
            // canvas.StrokeColor = Color.FromRgba("6599ff");
            canvas.StrokeColor = Color.FromRgba("6599ff");
            canvas.StrokeSize = 80;

            canvas.DrawArc(5, 5, (dirtyRect.Width - 10), (dirtyRect.Height - 10), 90, endAngle, false, false);
        }
    }
}