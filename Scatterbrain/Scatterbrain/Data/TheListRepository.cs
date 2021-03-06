﻿using Newtonsoft.Json;
using Scatterbrain.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scatterbrain.Data
{
    public class ScatterbrainRepository
    {
        private static SQLiteConnection _db;

        static ScatterbrainRepository()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _db = new SQLiteConnection(Path.Combine(path, "ScatterbrainSQLite.db3"));

            var theListInfo = _db.GetTableInfo(Constants.TheListTableName);
            if (!theListInfo.Any())
            {
                _db.CreateTable<TheList>();
            }

            var departmentInfo = _db.GetTableInfo(Constants.DepartmentTableName);
            if (!departmentInfo.Any())
            {
                _db.CreateTable<Department>();
            }
        }

        public static Task<Models.TheList> GetSubjects()
        {
            var items = _db.Table<TheList>().ToArray();
            var item = items.FirstOrDefault();
            if (item == null)
            {
                return Task.FromResult<Models.TheList>(null);
            }
            return Task.FromResult(JsonConvert.DeserializeObject<Models.TheList>(item.Content));
        }

        public static Task StoreSubjects(Models.TheList theList)
        {
            _db.Table<TheList>().Connection.Execute($"DELETE FROM {Constants.TheListTableName};");
            _db.Insert(new TheList { Content = JsonConvert.SerializeObject(theList) });
            return Task.FromResult(0);
        }

        public static Task<ObservableCollection<string>> GetDepartments()
        {
            var items = _db.Table<Department>().ToArray();
            var item = items.FirstOrDefault();
            if(item == null)
            {
                return Task.FromResult<ObservableCollection<string>>(null);
            }
            return Task.FromResult(JsonConvert.DeserializeObject<ObservableCollection<string>>(item.Content));
        }

        public static Task StoreDepartments(IEnumerable<string> departments)
        {
            _db.Table<TheList>().Connection.Execute($"DELETE FROM {Constants.DepartmentTableName};");
            _db.Insert(new Department { Content = JsonConvert.SerializeObject(departments) });
            return Task.FromResult(0);
        }
    }
}
