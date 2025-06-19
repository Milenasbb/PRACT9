# Refactoring
В проекте содержится класс (файл OrderCreater.cs), который нарушает несколько принципов SOLID и имеет "дурной запах" кода.   
Класс представляет заказ (Order) и выполняет множество задач: валидацию, сохранение, отправку уведомлений и т.д.  
Выявите проблемы в коде.
# Refactoring
В проекте содержится класс (файл OrderCreater.cs), который нарушает несколько принципов SOLID и имеет "дурной запах" кода.   
Класс представляет заказ (Order) и выполняет множество задач: валидацию, сохранение, отправку уведомлений и т.д.  
Выявите проблемы в коде.

Я разделила код на методы (Extract Method)
(ValidateCustomerName, ValidateOrderItems и т.д.)
Так код читается как инструкция: "проверить имя → проверить товары → посчитать сумму"

Вынесла константы (Replace Magic Numbers):
числа типа 0.9m (скидка 10%) в константы с понятными именами (DiscountMultiplier)

Улучшила проверки:
— заменила IsNullOrEmpty на IsNullOrWhiteSpace (ловит пробелы)
— добавила nameof() в ошибки (nameof(customerName) вместо текста "customerName")
 Так надежнее и код сам подсказывает, какой параметр сломался

Обработала ошибки в в email, чтобы приложение не падало, если нет интернета:
— обернула отправку письма в try-catch
Провела рефакторинг данной части кода,с использованием словаря для хранения цен и более лаконичного синтаксиса и убрала var,сохраняя функционал
Dictionary<string, decimal> itemPrices = new Dictionary<string, decimal>
{
    ["Ноутбук"] = 1200m,
    ["Мышь"] = 25m,
    ["Клавиатура"] = 50m,
    ["Камера"] = 500m,
    ["Колонки"] = 150m
};
 return items.Sum(item => itemPrices.TryGetValue(item, out decimal price) ? price : 0m);
 Модифицировала метод, заменив var на явные типы
private void SendOrderConfirmation(string customerName, decimal totalAmount)gitgitit 
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
