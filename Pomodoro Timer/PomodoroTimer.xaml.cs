using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pomodoro_Timer
{
    public partial class PomodoroTimer : ContentPage
    {
        private readonly ProgressArc _progressArc;
        private DateTime _startTime;
        private int _duration; // 25 minutes in seconds
        private int _breakDuration; // 5 minutes in seconds 5*60
        private double _progress;
        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _isRunning = false; // New flag to track if the timer is running
        private bool _isBreakTime = false; // New flag to track if it's break time
        private TimeSpan _elapsedBeforePause = TimeSpan.Zero; // New field to store the elapsed time before pause

        public PomodoroTimer()
        {
            InitializeComponent();
            _progressArc = new ProgressArc();

            // Get the work and break durations from Preferences
            _duration = Preferences.Get("workDuration", 25) * 60; // Default to 25 minutes if not set
            _breakDuration = Preferences.Get("breakDuration", 5) * 60; // Default to 5 minutes if not set

            MessagingCenter.Subscribe<Settings, string>(this, "BackgroundImageChanged", (sender, arg) => {
                LoadImage(arg);
            });

            // Set the initial background image
            var initialBackgroundImage = Preferences.Get("backgroundImage", "background.jpg");
            LoadImage(initialBackgroundImage);
        }

        private async void LoadImage(string imagePath)
        {
            Console.WriteLine($"Received image path: {imagePath}"); // Log the received file path
            try
            {
                if (File.Exists(imagePath))
                {
                    var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Please pick a photo"
                    });

                    if (result != null)
                    {
                        ImageSource imageSource = ImageSource.FromStream(() => result.OpenReadAsync().Result);
                        this.BackgroundImageSource = imageSource;
                    }
                }
                else
                {
                    Console.WriteLine($"File does not exist: {imagePath}"); // Log a warning
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load image: {ex.Message}"); // Log any errors
            }
        } 

        async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Settings());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _duration = Preferences.Get("workDuration", 25) * 60; // Update to the latest value
            _breakDuration = Preferences.Get("breakDuration", 5) * 60; // Update to the latest value
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
            _duration = 25 * 60; // Set the duration to 25 minutes
            _isBreakTime = false; // Set the break time flag to false
            ResetView();
        }

        private async void UpdateArc()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                TimeSpan elapsedTime = DateTime.Now - _startTime;
                TimeSpan timeRemaining = TimeSpan.FromSeconds(_duration - elapsedTime.TotalSeconds);

                if (timeRemaining.TotalSeconds <= 0)
                {
                    _cancellationTokenSource.Cancel();
                    _isRunning = false;
                    _elapsedBeforePause = TimeSpan.Zero; // Reset the elapsed time before pause

                    if (_isBreakTime)
                    {
                        ProgressButton.Text = "\u25B6"; // Unicode for play icon
                        _duration = 1 * 10; // Set the duration to 25 minutes
                        _isBreakTime = false;
                    }
                    else
                    {
                        ProgressButton.Text = "\u2615"; // Unicode for coffee icon
                        _duration = _breakDuration; // Set the duration to break time
                        _isBreakTime = true;
                    }

                    return;
                }
                else
                {
                    string timeRemainingFormatted = $"{timeRemaining:mm\\:ss}"; // Format as mm:ss
                    ProgressButton.Text = timeRemainingFormatted;
                }

                _progress = Math.Ceiling(elapsedTime.TotalSeconds);
                _progress %= _duration;
                _progressArc.Progress = _progress / _duration;
                ProgressView.Invalidate();

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

            // Reset the duration to the user's settings
            _duration = Preferences.Get("workDuration", 25) * 60; // Default to 25 minutes if not set
            _breakDuration = Preferences.Get("breakDuration", 5) * 60; // Default to 5 minutes if not set
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<Settings, string>(this, "BackgroundImageChanged");
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