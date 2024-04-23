//using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pomodoro_Timer.Models;
using SQLite;

namespace Pomodoro_Timer.Data
{
    public class NoteItemDatabase
    {
        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<NoteItemDatabase> Instance =
            new AsyncLazy<NoteItemDatabase>(async () =>
            {
                var instance = new NoteItemDatabase();
                try
                {
                    CreateTableResult result = await Database.CreateTableAsync<NoteItem>();
                }
                catch (Exception ex)
                {

                    throw;
                }
                return instance;
            });


        public NoteItemDatabase()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public Task<List<NoteItem>> GetItemsAsync()
        {
            return Database.Table<NoteItem>().ToListAsync();
        }

        public Task<List<NoteItem>> GetItemsNotDoneAsync()
        {
            return Database.QueryAsync<NoteItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public Task<NoteItem> GetItemAsync(int id)
        {
            return Database.Table<NoteItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(NoteItem item)
        {
            if (item.ID != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(NoteItem item)
        {
            return Database.DeleteAsync(item);
        }
    }
}