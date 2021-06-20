using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Model.Interfaces;
using ToDoApp.Model.Models;

namespace ToDoApp.Model.Services
{
    public class SyncStore : ISyncStore
    {
        private readonly ApiConnector _api;
        public ApplicationContext DbContext { get; private set; } = new ApplicationContext();

        public SyncStore(HttpClient httpClient)
        {
            _api = new ApiConnector(httpClient);
        }

        public async Task<bool> Sync(User user)
        {
            try
            {
                _api.SetToken(user.Token.Token);

                var (ok, updatedLists) = await _api.GetUpdatedToDoListsAsync(user.LastSyncTime);
                if (!ok)
                {
                    await FinishFailedSync();
                    return false;
                }

                UpdateLocalStorage(updatedLists, user);

                ok = await UploadLocalUpdates(user);
                if (!ok)
                {
                    return await FinishFailedSync();
                }

                return await FinishSuccessSync();
            }
            catch
            {
                return await FinishFailedSync();
            }

            async Task<bool> FinishSuccessSync()
            {
                user.LastSyncTime = DateTime.UtcNow;
                DbContext.Update(user);
                await DbContext.SaveChangesAsync();
                _api.RemoveCurrentToken();
                return true;
            }

            async Task<bool> FinishFailedSync()
            {
                _api.RemoveCurrentToken();
                await DbContext.DisposeAsync();
                DbContext = new ApplicationContext();
                return false;
            }
        }

        private async void UpdateLocalStorage(IEnumerable<ToDoList> lists, User user)
        {
            foreach (var list in lists)
            {
                var listTasks = list.Tasks;
                list.Tasks = null;

                var currentList = await DbContext.ToDoLists.FindAsync(list.Id);
                if (currentList != null)
                {
                    if (list.IsDeleted)
                    {
                        DbContext.ToDoLists.Remove(currentList);
                    }
                    else
                    {
                        currentList.Name = list.Name;
                        currentList.IsUpdated = false;
                        DbContext.ToDoLists.Update(currentList);
                    }
                }
                else
                {
                    list.User = user;
                    DbContext.ToDoLists.Add(list);
                }

                foreach (var task in listTasks)
                {
                    var currentTask = await DbContext.Tasks.FindAsync(task.Id);
                    if (currentTask != null)
                    {
                        if (task.IsDeleted)
                        {
                            DbContext.Tasks.Remove(currentTask);
                        }
                        else
                        {
                            currentTask.Name = task.Name;
                            currentTask.IsDone = task.IsDone;
                            currentTask.Priority = task.Priority;
                            currentTask.Date = task.Date;
                            currentTask.TimeOfBeginning = task.TimeOfBeginning;
                            currentTask.TimeOfEnd = task.TimeOfEnd;
                            currentTask.RepeatingConditions = task.RepeatingConditions;
                            currentTask.IsUpdated = false;
                            DbContext.Tasks.Update(currentTask);
                        }
                    }
                    else
                    {
                        DbContext.Tasks.Add(task);
                    }
                }
            }
        }

        private async Task<bool> UploadLocalUpdates(User user)
        {
            var uploadingLists = GetUploadingLists(user);

            var deletingLists = TakeDeletingLists(uploadingLists);
            var ok = await _api.DeleteToDoListsAsync(deletingLists.Select(list => list.Id));
            if (!ok)
            {
                return false;
            }

            var deletingTasks = TakeDeletingTasks(uploadingLists);
            ok = await _api.DeleteTasksAsync(deletingTasks.Select(task => task.Id));
            if (!ok)
            {
                return false;
            }

            var addingLists = TakeAddingLists(uploadingLists);
            ok = await _api.AddToDoListsAsync(addingLists);
            if (!ok)
            {
                return false;
            }

            var addingTasks = TakeAddingTasks(uploadingLists);
            ok = await _api.AddTasksAsync(addingTasks);
            if (!ok)
            {
                return false;
            }

            var updatingLists = TakeUpdatingLists(uploadingLists);
            ok = await _api.UpdateToDoListsAsync(updatingLists);
            if (!ok)
            {
                return false;
            }

            var updatingTasks = TakeUpdatingTasks(uploadingLists);
            ok = await _api.UpdateTasksAsync(updatingTasks);
            if (!ok)
            {
                return false;
            }

            return true;
        }

        private List<ToDoList> GetUploadingLists(User user)
        {
            var lists = DbContext.ToDoLists
                .Where(list => list.User == user && (list.IsUpdated || list.IsDeleted || list.IsAdded))
                .Include(list => list.Tasks).ToList();
            lists.ForEach(list => list.Tasks.RemoveAll(task => !(task.IsUpdated || task.IsDeleted || task.IsAdded)));
            return lists;
        }

        private List<ToDoList> TakeDeletingLists(List<ToDoList> lists)
        {
            var deletingLists = lists.Where(list => list.IsDeleted).ToList();
            DbContext.ToDoLists.RemoveRange(deletingLists);
            lists.RemoveAll(list => list.IsDeleted);
            return deletingLists;
        }

        private List<Task> TakeDeletingTasks(List<ToDoList> lists)
        {
            var deletingTasks = lists.SelectMany(list => list.Tasks.Where(task => task.IsDeleted)).ToList();
            DbContext.Tasks.RemoveRange(deletingTasks);
            lists.ForEach(list => list.Tasks.RemoveAll(task => task.IsDeleted));
            return deletingTasks;
        }

        private List<ToDoList> TakeUpdatingLists(List<ToDoList> lists)
        {
            var updatingLists = lists.Where(list => list.IsUpdated).ToList();
            updatingLists.ForEach(list =>
            {
                list.IsUpdated = false;
                DbContext.ToDoLists.Update(list);
            });
            updatingLists.SelectMany(list => list.Tasks).ToList().ForEach(task =>
            {
                task.IsUpdated = false;
                DbContext.Tasks.Update(task);
            });
            lists.RemoveAll(list => list.IsUpdated);
            return updatingLists;
        }

        private List<Task> TakeUpdatingTasks(List<ToDoList> lists)
        {
            var updatingTasks = lists.SelectMany(list => list.Tasks.Where(task => task.IsUpdated)).ToList();
            updatingTasks.ForEach(task =>
            {
                task.IsUpdated = false;
                DbContext.Tasks.Update(task);
            });
            return updatingTasks;
        }

        private List<ToDoList> TakeAddingLists(List<ToDoList> lists)
        {
            var addingLists = lists.Where(list => list.IsAdded || list.IsUpdated).ToList();
            addingLists.ForEach(list =>
            {
                list.IsAdded = false;
                list.IsUpdated = false;
                DbContext.ToDoLists.Update(list);
            });
            addingLists.SelectMany(list => list.Tasks).ToList().ForEach(task =>
            {
                task.IsAdded = false;
                task.IsUpdated = false;
                DbContext.Tasks.Update(task);
            });
            return addingLists;
        }

        private List<Task> TakeAddingTasks(List<ToDoList> lists)
        {
            var addingTasks = lists.SelectMany(list => list.Tasks.Where(task => task.IsAdded)).ToList();
            addingTasks.ForEach(task =>
            {
                task.IsAdded = false;
                task.IsUpdated = false;
                DbContext.Tasks.Update(task);
            });
            return addingTasks;
        }
    }
}