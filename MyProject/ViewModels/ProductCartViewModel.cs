using MyProject.Commands;
using MyProject.Data;
using MyProject.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class ProductCartViewModel : INotifyPropertyChanged
    {
        private readonly CartService _cart = CartService.Instance;

        public Products Product { get; }

        public int Quantity => _cart.GetQuantity(Product.Id);
        public bool IsInCart => Quantity > 0;
        private bool IsAuthorized => Backend.Auth.IsAuthenticated;
        public ICommand AddToCartCommand { get; }
        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        public ICommand DeleteCommand { get; }

        public ProductCartViewModel(Products product)
        {
            Product = product;
            if (Product.Sale != null) Product.Price = decimal.Round(Product.Price * (100 - (decimal)Product.Sale) / 100, 2);
            _cart.CartChanged += () =>
            {
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(IsInCart));
            };

            AddToCartCommand = new RelayCommand(
                execute: _ => _cart.Add(Product.Id),
                canExecute: _ => IsAuthorized
            );

            IncreaseCommand = new RelayCommand(
                execute: _ => _cart.Add(Product.Id),
                canExecute: _ => IsAuthorized
            );

            DecreaseCommand = new RelayCommand(
                execute: _ => _cart.Remove(Product.Id),
                canExecute: _ => IsAuthorized
            );

            DeleteCommand = new RelayCommand(
                execute: _ => _cart.Delete(Product.Id),
                canExecute: _ => IsAuthorized
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
