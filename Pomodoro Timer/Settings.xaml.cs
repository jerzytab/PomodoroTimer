using Microsoft.Maui.Controls;

using System;

namespace Pomodoro_Timer
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
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
    }
}