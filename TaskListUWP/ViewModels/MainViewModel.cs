using Persistance.DTOs;
using Persistance.Converters;
using Persistance.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace TaskList.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public static string SaveFolder = ApplicationData.Current.LocalFolder.Path + Path.DirectorySeparatorChar;
        public List<Item> ItemList { get; set; }
        public bool UnsavedData { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public TaskListDB CurrentList { get; set; }

        public MainViewModel()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddDays(14);

            var handler = new WebRequestHandler();
            var itemResult = handler.Get("http://localhost/TaskListAPI/api/Item/Test").Result;
            var listResult = handler.Get("http://localhost/TaskListAPI/api/List/Test").Result;
            if (itemResult != null && listResult != null)
            {
                LoadData();
            }
            else throw new Exception("Could not load data.");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "ItemList")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            UnsavedData = true;
        }

        public void AddItem(Item item)
        {
            if (item is null) return;

            var dto = item.DTO();
            dto.ListId = CurrentList.Id;
            var result = new WebRequestHandler().Post("http://localhost/TaskListAPI/api/Item/AddOrUpdate", dto).Result;
            if (result == null) return;

            if (item is Task)
            {
                var returnedItemDTO = JsonConvert.DeserializeObject<TaskDTO>(result);
                ItemList.Add(returnedItemDTO.Item());

                NotifyPropertyChanged();
            } else if (item is Appointment)
            {
                var returnedItemDTO = JsonConvert.DeserializeObject<AppointmentDTO>(result);
                ItemList.Add(returnedItemDTO.Item());

                NotifyPropertyChanged();
            }
        }

        public void EditItem(Item item)
        {
            if (item is null) return;

            var dto = item.DTO();
            dto.ListId = CurrentList.Id;
            var result = new WebRequestHandler().Post("http://localhost/TaskListAPI/api/Item/AddOrUpdate", dto).Result;
            if (result == null) return;

            if (item is Task)
            {
                var returnedItemDTO = JsonConvert.DeserializeObject<TaskDTO>(result);
                ItemList.Remove(item);
                ItemList.Add(returnedItemDTO.Item());

                NotifyPropertyChanged();
            }
            else if (item is Appointment)
            {
                var returnedItemDTO = JsonConvert.DeserializeObject<AppointmentDTO>(result);
                ItemList.Remove(item);
                ItemList.Add(returnedItemDTO.Item());

                NotifyPropertyChanged();
            }
        }

        public void DeleteItem(Item item)
        {
            if (item is null) return;

            var result = new WebRequestHandler().Post("http://localhost/TaskListAPI/api/Item/Delete", item.Id).Result;
            if (result == null) return;

            ItemList.Remove(item);

            NotifyPropertyChanged();
        }

        public void LoadData()
        {
            ItemList = null;

            var handler = new WebRequestHandler();
            var result = handler.Get("http://localhost/TaskListAPI/api/Item/Get").Result;
            if (result == null || result == "") return;

            // No lists created.
            if (result == "ERROR") return;
            
            var itemDTOs = JsonConvert.DeserializeObject<List<ItemDTO>>(result);
            ItemList = itemDTOs.Select(i => i.Item()).ToList();

            result = handler.Get("http://localhost/TaskListAPI/api/List/Get").Result;
            if (result != "ERROR" && result != "")
            {
                CurrentList = JsonConvert.DeserializeObject<TaskListDB>(result);
            }

            NotifyPropertyChanged();
        }

        public TaskListDB CreateList(string name, string description = "")
        {
            if (name == null || name == "") return null;

            var handler = new WebRequestHandler();
            var response = handler.Post("http://localhost/TaskListAPI/api/List/Create", name).Result;

            return JsonConvert.DeserializeObject<TaskListDB>(response);
        }

        public TaskListDB ChangeList(TaskListDB taskList)
        {
            var handler = new WebRequestHandler();
            var result = handler.Post($"http://localhost/TaskListAPI/api/List/Change", taskList.Id).Result;

            if (result != null && result != "ERROR")
            {
                LoadData();
                CurrentList = taskList;
                return JsonConvert.DeserializeObject<TaskListDB>(result);
            }
            else
            {
                return null;
            }
        }

        public List<TaskListDB> GetLists()
        {
            var handler = new WebRequestHandler();
            var result = handler.Get("http://localhost/TaskListAPI/api/List/GetAll").Result;
            if (result == null || result == "" || result == "ERROR") return null;

            return JsonConvert.DeserializeObject<List<TaskListDB>>(result);
        }

        public bool DeleteList(int id)
        {
            var handler = new WebRequestHandler();
            var result = handler.Post($"http://localhost/TaskListAPI/api/List/Delete", id).Result;

            if (result != null && result != "ERROR")
            {
                ItemList = null;
                NotifyPropertyChanged();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<Item> Search(string search)
        {
            if (search == null || search.Trim() == "") return null;

            var handler = new WebRequestHandler();
            var result = handler.Get($"http://localhost/TaskListAPI/api/Item/Search?search={search.Trim().ToLower()}").Result;

            if (result == null) return null;

            var items = JsonConvert.DeserializeObject<List<ItemDTO>>(result).Select(item => item.Item()).ToList();
            return items;
        }
    }
}