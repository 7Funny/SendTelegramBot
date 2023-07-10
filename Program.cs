using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using System.Text.Json;
using System.Diagnostics;
using System.IO;

namespace TelegramBotExperiments
{

    class Program
    {
        static string TOKENBOT = "5793431266:AAFK1Py744xfBhM2A3Qkss18kqomE5iIWWA";// Изменить
        static ITelegramBotClient bot = new TelegramBotClient(TOKENBOT);
        // каталог, где находятся файлы, фотографии
        static string path = "C:\\Users\\1392680\\source\\repos\\SendTelegrambot\\";// Изменить

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Вывод словаря с информацией о сообщении
            //Console.WriteLine(JsonSerializer.Serialize(update));
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            // проверка типа обновления - если пользователь отправит сообщение
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                
                if (message.Text.ToLower() == "/start")
                {
                    /* 0 - отправка текстового сообщения */
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    return; // останавливаем выполнение метода
                }

                if (message.Text.ToLower() == "/picurl")
                {
                    /* 1 - отправка фотографии из интернета (через url) */
                    await botClient.SendPhotoAsync(
                        message.Chat, 
                        InputFileId.FromUri("https://bugaga.ru/uploads/posts/2022-08/1660840053_priroda-4.jpg"));

                    return;
                }

                if (message.Text.ToLower() == "/picfile")
                {
                    /* 2 - отправка файла (фотографии в виде файла) */
                    string picture = "pic.jpg";
                    await using Stream stream = System.IO.File.OpenRead(path + picture);
                    await bot.SendDocumentAsync(
                        chatId: message.Chat,
                        document: InputFile.FromStream(stream: stream, fileName: "picture.jpg"),//назание файла
                        caption: "Pic");

                    return;
                }

                if (message.Text.ToLower() == "/piclocale")
                {
                    /* 3 - отправка фотографии из локального хранилища */
                    string picture = "picture.jpg";
                    await using Stream stream = System.IO.File.OpenRead(path + picture);
                    await bot.SendPhotoAsync(
                        chatId: message.Chat,
                        photo: InputFile.FromStream(stream: stream, fileName: "picture.jpg"),//назание файла
                        caption: "Pic");

                    return;
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();//управляет и посылает уведомление об отмене токену
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}