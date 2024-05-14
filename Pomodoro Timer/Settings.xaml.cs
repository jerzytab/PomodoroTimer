using Microsoft.Maui.Controls;
using System;
using System.IO;

namespace Pomodoro_Timer
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();

            // Get the work and break durations from Preferences
            var workDuration = Preferences.Get("workDuration", 25); // Default to 25 minutes if not set
            var breakDuration = Preferences.Get("breakDuration", 5); // Default to 5 minutes if not set

            // Set the sliders' values
            WorkDurationSlider.Value = workDuration;
            BreakDurationSlider.Value = breakDuration;
        }

        private void OnWorkDurationSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newValue = Math.Round(e.NewValue);
            WorkDurationLabel.Text = $"Długość pracy: {newValue} minut";
            Preferences.Set("workDuration", (int)newValue);
        }

        private void OnBreakDurationSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newValue = Math.Round(e.NewValue);
            BreakDurationLabel.Text = $"Długość przerwy: {newValue} minut";
            Preferences.Set("breakDuration", (int)newValue);
        }

        private async void OnChangeBackgroundButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Please pick a photo"
                });

                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    var filePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                    using (var fileStream = File.OpenWrite(filePath))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    Console.WriteLine($"Saved image to: {filePath}"); // Log the file path

                    Preferences.Set("backgroundImage", filePath);
                    MessagingCenter.Send(this, "BackgroundImageChanged", filePath);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or display an error to the user
                Console.WriteLine(ex.Message);
            }
        }
    }
}