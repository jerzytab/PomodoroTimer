using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pomodoro_Timer.Data;
using Pomodoro_Timer.Models;

namespace Pomodoro_Timer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class NoteListPage : ContentPage
    {
        public NoteListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            NoteItemDatabase database = await NoteItemDatabase.Instance;
            listView.ItemsSource = await database.GetItemsAsync();
        }

        async void OnItemAdded(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NoteItemPage
            {
                BindingContext = new NoteItem()
            });
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NoteItemPage
                {
                    BindingContext = e.SelectedItem as NoteItem
                });
            }
        }
    }

}