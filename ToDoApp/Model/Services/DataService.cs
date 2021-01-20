using ToDoApp.Model.Interfaces;

namespace ToDoApp.Model
{
    public class DataService : IDataService
    {
        public ApplicationContext DbContext { get; } = new ApplicationContext();
        public bool Sync(string token)
        {
            throw new System.NotImplementedException();
        }
    }
}