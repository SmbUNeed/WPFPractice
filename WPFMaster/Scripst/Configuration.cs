using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFMaster.Pages;

namespace WPFMaster
{
    internal class Configuration
    {
        public int TotalCost;
        public Color BodyColor;
        public string EngineType;
        public string Model;

        public List<string> _selectedOptions = new List<string>();
        public static Configuration Instance;

        public Configuration()
        {
            Instance = Instance == null ? this : Instance;
        }

        public enum ContactTypes
        {
            Name = 0,
            Email = 1,
            PhoneNumber = 2,
            Adress = 3
        }

        private Dictionary<string, int> EngineTypes = new Dictionary<string, int>()
        {
            { "Базовый", 0},
            { "Комфорт", 100000},
            { "Спорт", 200000},
            { "Спорт+", 500000},
            { "Турбированный", 700000},
            { "Ракетный двигатель", 1000000}
        };

        private Dictionary<ContactTypes, string> ContactInformation = new Dictionary<ContactTypes, string>()
        {
            {ContactTypes.Name, null },
            {ContactTypes.Email, null },
            {ContactTypes.PhoneNumber, null },
            {ContactTypes.Adress, null }
        };

        private Dictionary<string, int> AvaliableOptions = new Dictionary<string, int>()
        {
            {"Доводчики", 20000},
            {"Лидар", 50000},
            {"Пневмоподвеска", 100000},
            {"Электронные зеркала", 10000},
            {"Тонировка", 5000},
        };

        private Dictionary<string, int> AvaliableModels = new Dictionary<string, int>()
        {
            {"Lada Oka",           30000},
            {"Lada Priora",        150000},
            {"BMW M5 Competition", 10000000},
            {"Toyota Taiga",       6000000},
            {"Dio Matiz",          500000},
            {"Gazel V8 Sp",        14000000 }
        };

        public void SetContactInformation(string[] arr)
        {
            foreach(ContactTypes key in ContactInformation.Keys.ToList())
            {
                ContactInformation[key] = arr[(int)key];
            }
        }
        public string[] GetContactsInfo()
        {
            if (CheckContactIfFull()) return ContactInformation.Values.ToArray();
            return new string[]{};
        }
        public bool CheckContactIfFull()
        {
            foreach (ContactTypes key in ContactInformation.Keys.ToList())
            {
                if (ContactInformation[key] == null) return false;
            }
            return true;
        }
        public void SetCarColor(Color color) => BodyColor = color;
        public void SetModel(string model) => Model = model;
        public void SetEngineType(string engineType) => EngineType = engineType;
        public void SetOption(string option) => _selectedOptions.Add(option);
        public void RemoveOption(string option) => _selectedOptions.Remove(option);
        public List<string> GetAvaliable(string type) => DictByType(type).Keys.ToList();
        public int GetPrice(string type, string element) => DictByType(type)[element];
        private Dictionary<string, int> DictByType(string type)
        {
            switch (type)
            {
                case "models": return AvaliableModels;
                case "options": return AvaliableOptions;
                case "engines": return EngineTypes;
                default: throw new ArgumentException();
            }
        }

        public int CalculateTotalCost()
        {
            TotalCost = 0;
            TotalCost += EngineType == null ? 0 : EngineTypes[EngineType];
            TotalCost += Model == null ? 0 : AvaliableModels[Model];
            TotalCost += BodyColor == null || 
                //Проверка на черный(базовый) цвет
                (BodyColor.R == 0 && BodyColor.G == 0 && BodyColor.B == 0) ? 0 : 30000;
            foreach(string option in _selectedOptions) TotalCost += AvaliableOptions[option];
            return TotalCost;
        }
        

        public void ClearConfiguration() => Instance = new Configuration();
    }
}
