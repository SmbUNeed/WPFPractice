using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFMaster
{
    internal class Configuration
    {
        public enum EngineType
        {
            Petrol = 150000,
            Diesel = 500000,
            Electric = 400000
        }

        public Color BodyColor;
        public Dictionary<string, int> AvaliableOptions = new Dictionary<string, int>()
        {
            {"Доводчики", 20000},
            {"Лидар", 50000},
            {"Пневмоподвеска", 100000},
            {"Электронные зеркала", 10000},
            {"Тонировка", 5000},
        };
        public List<string> SelectedOptions;

        public int TotalCost;
        public EngineType Engine;

        public void CalculateCost()
        {
            TotalCost = (int)Engine;
            foreach(string option in SelectedOptions)
            {
                TotalCost += AvaliableOptions[option];
            }
        }
    }
}
