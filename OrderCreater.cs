using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Windows;
using System.Linq;

namespace WpfAppWithoutRefactoring
{
    internal class OrderCreater
    {
        private const string LogFilePath = @".\logs\order.txt";
        private const int ItemsCountForDiscount = 2;
        private const decimal DiscountMultiplier = 0.9m;

        public OrderCreater(string customerName, List<string> items, string paymentMethod)
        {
            ValidateInputParameters(customerName, items);

            decimal orderTotal = CalculateTotalPrice(items);
            orderTotal = ApplyVolumeDiscount(items, orderTotal);

            ProcessPayment(paymentMethod, orderTotal);

            CreateOrderLog(customerName);
            SendOrderConfirmation(customerName, orderTotal);
        }

        private void ValidateInputParameters(string customerName, List<string> items)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Имя заказчика не может быть пустым", nameof(customerName));

            if (items == null || items.Count == 0)
                throw new ArgumentException("Список товаров не может быть пустым", nameof(items));
        }

        private decimal CalculateTotalPrice(List<string> items)
        {
            Dictionary<string, decimal> itemPrices = new Dictionary<string, decimal>
            {
                ["Ноутбук"] = 1200m,
                ["Мышь"] = 25m,
                ["Клавиатура"] = 50m,
                ["Камера"] = 500m,
                ["Колонки"] = 150m
            };

            return items.Sum(item => itemPrices.TryGetValue(item, out decimal price) ? price : 0m);
        }

        private decimal ApplyVolumeDiscount(List<string> items, decimal totalPrice)
        {
            return items.Count > ItemsCountForDiscount ?
                   totalPrice * DiscountMultiplier :
                   totalPrice;
        }

        private void ProcessPayment(string paymentMethod, decimal amount)
        {
            switch (paymentMethod)
            {
                case "По карте":
                    MessageBox.Show($"Обработка платежа по карте на сумму: {amount:C}");
                    break;
                case "PayPal":
                    Console.WriteLine($"Обработка PayPal платежа на сумму: {amount:C}");
                    break;
                default:
                    throw new ArgumentException($"Неподдерживаемый способ оплаты: {paymentMethod}");
            }
        }

        private void CreateOrderLog(string customerName)
        {
            string logEntry = $"Заказ от {customerName} создан {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
            File.WriteAllText(LogFilePath, logEntry);
        }

        private void SendOrderConfirmation(string customerName, decimal totalAmount)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                MailAddress from = new MailAddress("lomovala@gmail.com", "LomovaLA");
                MailAddress to = new MailAddress("lomovala@gmail.com");
                MailMessage message = new MailMessage(from, to)
                {
                    Subject = "Новый заказ",
                    Body = $"<h2>Новый заказ для {customerName} общей стоимостью {totalAmount:C}.</h2>",
                    IsBodyHtml = true
                };

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки уведомления: {ex.Message}");
            }
        }
    }
}