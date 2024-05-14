using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Pomodoro_Timer;

public partial class Account : ContentPage, INotifyPropertyChanged
{
    private int _pomodoroSessionsCount;
    private int _notesCount;
    
    public Account()
    {
        InitializeComponent();
        BindingContext = this;
    }
    public int PomodoroSessionsCount
    {
        get { return _pomodoroSessionsCount; }
        set
        {
            if (_pomodoroSessionsCount != value)
            {
                _pomodoroSessionsCount = value;
                OnPropertyChanged(nameof(PomodoroSessionsCount));
            }
        }
    }

    public int NotesCount
    {
        get { return _notesCount; }
        set
        {
            if (_notesCount != value)
            {
                _notesCount = value;
                OnPropertyChanged(nameof(NotesCount));
            }
        }
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // TODO: Replace these with actual data
        string username = "jerzytab";


        UsernameLabel.Text = username;
        PomodoroSessionsLabel.Text = $"Liczba sesji Pomodoro: {PomodoroSessionsCount}";
        NotesCountLabel.Text = $"Liczba notatek: {NotesCount}";
    }
}