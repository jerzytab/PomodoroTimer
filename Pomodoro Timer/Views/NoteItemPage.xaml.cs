using Pomodoro_Timer.Data;
using Pomodoro_Timer.Models;

namespace Pomodoro_Timer.Views
{
    public partial class NoteItemPage : ContentPage
    {
        public NoteItemPage()
        {
            InitializeComponent();
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            var noteItem = (NoteItem)BindingContext;
            NoteItemDatabase database = await NoteItemDatabase.Instance;
            await database.SaveItemAsync(noteItem);
            await Navigation.PopAsync();
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var noteItem = (NoteItem)BindingContext;
            NoteItemDatabase database = await NoteItemDatabase.Instance;
            await database.DeleteItemAsync(noteItem);
            await Navigation.PopAsync();
        }

        async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}