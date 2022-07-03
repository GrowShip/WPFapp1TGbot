using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

// using Telegram.Bot.Extensions.Polling;
// using Telegram.Bot.Examples.Polling;
// using Telegram.Bot.Types.InputFiles;
// using File = Telegram.Bot.Types.File;

namespace WPFapp1TGbot
{
    class MyFunction
    {

        /// <summary>
        /// Обрабатывает поступающую инфу
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            await SaveToJSON(botClient, update);

            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(botClient, update, update.Message);
                return;
            }

            else if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallBackQuery(botClient, update.CallbackQuery);
                return;
            }

            #region MyRegion Определения переданных файлов

            else if (update.Message != null && update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                Console.WriteLine($"{update.Message.Document.FileId}\n" +
                                  $"{update.Message.Document.FileName}\n" +
                                  $"{update.Message.Document.FileSize}");
                string whatIsThis = "document";
                await d(botClient,
                    update.Message.Document.FileId,
                    update.Message.Document.FileName,
                    update.Message,
                    whatIsThis);
                return;
            }
            else if (update.Message != null && update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Audio)
            {
                Console.WriteLine($"{update.Message.Audio.FileId}\n" +
                                  $"{update.Message.Audio.FileName}\n" +
                                  $"{update.Message.Audio.FileSize}");
                string whatIsThis = "audio";
                await d(botClient,
                    update.Message.Audio.FileId,
                    update.Message.Audio.FileName,
                    update.Message,
                    whatIsThis);
                return;
            }
            else if (update.Message != null && update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Voice)
            {
                Console.WriteLine($"{update.Message.Voice.FileId}\n" +
                                  $"{update.Message.Voice.MimeType}\n" +
                                  $"{update.Message.Voice.FileSize}");
                string[] voice = update.Message.Voice.MimeType.Split("/");
                string mimo = voice[1];
                string whatIsThis = "voice";
                await d(botClient,
                    update.Message.Voice.FileId,
                    mimo,
                    update.Message,
                    whatIsThis);
                return;
            }
            else if (update.Message != null && update.Message.Type == MessageType.Photo)
            {
                Console.WriteLine($"{update.Message.Photo.Last().FileId}\n" +
                                  $"{update.Message.Photo.Last().FileUniqueId}");
                string whatIsThis = "photo";
                await d(botClient,
                    update.Message.Photo.Last().FileId,
                    update.Message.Photo.Last().FileUniqueId,
                    update.Message,
                    whatIsThis);
                return;
            }

            #endregion

            

        }


        /// <summary>
        /// Обрабатывает сообщения
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="message"></param>
        static async Task HandleMessage(ITelegramBotClient botClient, Update update, Message message)
        {
            // if (message.Text == "/start")
            // {
            //     await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери команду: /inline | /keyboard");
            //     return;
            // }

            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup keyboard = new(new[]
                {
                    new KeyboardButton($"Сохранить файл"),
                    new KeyboardButton($"Список файлов"),
                    new KeyboardButton($"Добавить продажу")
                })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, "Я отправил тебе клавиатуру, выбирай!", replyMarkup: keyboard);
                return;
            }

            else if (message.Text.Contains("Сохранить"))
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Отправь мне что ты хочешь, я жду");
                return;
            }

            else if (message.Text == "Список файлов")
            {
                InlineKeyboardMarkup keyboard0 = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Документы","File_Док"),
                        InlineKeyboardButton.WithCallbackData("Изображения","File_Фото"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Аудио и голосовые","File_Аудио"),
                        InlineKeyboardButton.WithCallbackData("Что-то придумаем","File_Все"),
                    }
                });
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Что хотите посмотреть?", replyMarkup: keyboard0);
                return;
            }

            else if (message.Text == "Добавить продажу")
            {
                InlineKeyboardMarkup keyboard1 = new(new[]
                {
                    new[] {InlineKeyboardButton.WithCallbackData("Места", "Выбор места")},
                    new[] {InlineKeyboardButton.WithCallbackData("Комфорт зал", "Комфорт зал")},
                    new[] {InlineKeyboardButton.WithCallbackData("Апгрейд класса", "Повышение класса")},
                    new[] {InlineKeyboardButton.WithCallbackData("Сумка-переноска за мили", "Сумку переноску за мили"),},
                    new[] {InlineKeyboardButton.WithCallbackData("Бизнес", "Бизнес")},
                    new[] {InlineKeyboardButton.WithCallbackData("Класс за мили", "Повыш. класса за мили")}
                });
                await botClient.SendTextMessageAsync(message.Chat.Id, "Выбери что?", replyMarkup: keyboard1);
                return;
            }

            await botClient.SendTextMessageAsync(message.Chat.Id, $"Зачем ты написал:\n{message.Text}");
        }

        /// <summary>
        /// Загрузка на ПК разных типов данных
        /// </summary>
        /// <param name="botClient">Бот</param>
        /// <param name="fileId">ID файла</param>
        /// <param name="path">Название файла</param>
        /// <param name="message">Готово или нет</param>
        /// <param name="thisis">Тип данных</param>
        static async Task d(ITelegramBotClient botClient, string fileId, string path, Message message, string thisis)
        {
            var file = await botClient.GetFileAsync(fileId);
            string way;
            string name;
            switch (thisis)
            {
                case "audio":
                    name = path;
                    way = "MyDir/Audio/" + $"{path}";
                    break;
                case "voice":
                    Random r = new Random();
                    int num = r.Next(0, 100);
                    name = $"{path}_{num}.mpeg";
                    way = "MyDir/Audio/" + $"{path}{num}.mpeg";
                    break;
                case "document":
                    name = path;
                    way = "MyDir/Documents/" + path;
                    break;
                case "photo":
                    name = path;
                    way = "MyDir/Photo/" + $"{path}.jpg";
                    break;
                default:
                    { return; }
            }
            FileStream fs = new FileStream(way, FileMode.Create);
            await botClient.DownloadFileAsync(file.FilePath, fs);
            await botClient.SendTextMessageAsync(message.Chat.Id, $"{name} сохранен!");
        }

        /// <summary>
        /// Добавления продажи по типу
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="callbackQuery"></param>
        static async Task HandleCallBackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            #region Работа со списком документов

            if (callbackQuery.Data.StartsWith("File"))
            {
                string[] fileDirectory = callbackQuery.Data.Split('_');
                string way;
                string type;
                switch (fileDirectory[1])
                {
                    case "Аудио":
                        type = "Audio";
                        way = "MyDir/Audio/";
                        break;
                    case "Док":
                        type = "Documents";
                        way = "MyDir/Documents/";
                        break;
                    case "Фото":
                        type = "Photo";
                        way = "MyDir/Photo/";
                        break;
                    default:
                        { return; }
                }
                List<string> catalogFile = Directory.GetFiles(way).ToList();
                foreach (var e in catalogFile)
                {
                    FileInfo nameFile = new FileInfo($"{e}");
                    InlineKeyboardMarkup kb = new(new[] { InlineKeyboardButton.WithCallbackData($"{nameFile.Name}", $"Sent|{type}|{nameFile.Name}") });
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "▼", replyMarkup: kb);
                }
                return;
            }

            if (callbackQuery.Data.StartsWith("Sent"))
            {
                string[] wholeInfoAboutWhatFileIs = callbackQuery.Data.Split('|');
                FileStream fs = new FileStream($"MyDir/{wholeInfoAboutWhatFileIs[1]}/{wholeInfoAboutWhatFileIs[2]}", FileMode.Open);
                switch (wholeInfoAboutWhatFileIs[1])
                {
                    case "Audio":
                        await botClient.SendAudioAsync(callbackQuery.Message.Chat.Id,
                            new InputMedia(fs, wholeInfoAboutWhatFileIs[2]));
                        break;
                    case "Photo":
                        await botClient.SendPhotoAsync(callbackQuery.Message.Chat.Id,
                            new InputMedia(fs, wholeInfoAboutWhatFileIs[2]));
                        break;
                    case "Documents":
                        await botClient.SendDocumentAsync(callbackQuery.Message.Chat.Id,
                            new InputMedia(fs, wholeInfoAboutWhatFileIs[2]));
                        break;
                    default:
                        break;
                }
                return;
            }
            #endregion

            #region Работа с добавление продажами

            if (callbackQuery.Data.StartsWith("Выбор"))
            {
                InlineKeyboardMarkup keyboard1 = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("300", "1_Места_30"),
                        InlineKeyboardButton.WithCallbackData("350", "1_Места_35"),
                        InlineKeyboardButton.WithCallbackData("400", "1_Места_40"),
                        InlineKeyboardButton.WithCallbackData("450", "1_Места_45"),
                        InlineKeyboardButton.WithCallbackData("500", "1_Места_50"),
                        InlineKeyboardButton.WithCallbackData("550", "1_Места_55")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("600", "1_Места_60"),
                        InlineKeyboardButton.WithCallbackData("650", "1_Места_65"),
                        InlineKeyboardButton.WithCallbackData("800", "1_Места_80"),
                        InlineKeyboardButton.WithCallbackData("850", "1_Места_85"),
                        InlineKeyboardButton.WithCallbackData("1200", "1_Места_120"),
                        InlineKeyboardButton.WithCallbackData("1400", "1_Места_140")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("1600", "1_Места_160"),
                        InlineKeyboardButton.WithCallbackData("2000", "1_Места_200"),
                        InlineKeyboardButton.WithCallbackData("2500", "1_Места_250"),
                        InlineKeyboardButton.WithCallbackData("3000", "1_Места_300"),
                        InlineKeyboardButton.WithCallbackData("3200", "1_Места_320"),
                        InlineKeyboardButton.WithCallbackData("3600", "1_Места_360")
                    },
                });
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $@"Выбор {callbackQuery.Data.ToLower()}:", replyMarkup: keyboard1);
                return;
            }
            if (callbackQuery.Data.StartsWith("Комфорт"))
            {
                InlineKeyboardMarkup keyboard2 = new(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Взрослый 2000", "1_Комфзал_200"),
                        InlineKeyboardButton.WithCallbackData("Ребенок 1000","1_Комфзал_100")
                    }
                );
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $@"{callbackQuery.Data}:", replyMarkup: keyboard2);
                return;
            }
            if (callbackQuery.Data.StartsWith("Повышение"))
            {
                InlineKeyboardMarkup keyboard3 = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("5000", "1_Класс_400"),
                        InlineKeyboardButton.WithCallbackData("6000", "1_Класс_480"),
                        InlineKeyboardButton.WithCallbackData("7000", "1_Класс_560"),
                        InlineKeyboardButton.WithCallbackData("8000", "1_Класс_640"),
                        InlineKeyboardButton.WithCallbackData("9000", "1_Класс_720"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("10000", "1_Класс_800"),
                        InlineKeyboardButton.WithCallbackData("11000", "1_Класс_880"),
                        InlineKeyboardButton.WithCallbackData("12000", "1_Класс_960"),
                        InlineKeyboardButton.WithCallbackData("13000", "1_Класс_1040"),
                        InlineKeyboardButton.WithCallbackData("14000", "1_Класс_1120"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("15000", "1_Класс_1200"),
                        InlineKeyboardButton.WithCallbackData("16000", "1_Класс_1280"),
                        InlineKeyboardButton.WithCallbackData("17000", "1_Класс_1360"),
                        InlineKeyboardButton.WithCallbackData("17500", "1_Класс_1400"),
                        InlineKeyboardButton.WithCallbackData("18000", "1_Класс_1440")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("19000", "1_Класс_1520"),
                        InlineKeyboardButton.WithCallbackData("20000", "1_Класс_1600"),
                        InlineKeyboardButton.WithCallbackData("21000", "1_Класс_1680"),
                        InlineKeyboardButton.WithCallbackData("22000", "1_Класс_1760"),
                        InlineKeyboardButton.WithCallbackData("23000", "1_Класс_1840")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("24000", "1_Класс_1920"),
                        InlineKeyboardButton.WithCallbackData("25000", "1_Класс_2000"),
                        InlineKeyboardButton.WithCallbackData("26000", "1_Класс_2080"),
                        InlineKeyboardButton.WithCallbackData("27000", "1_Класс_2160"),
                        InlineKeyboardButton.WithCallbackData("28000", "1_Класс_2240")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("30000", "1_Класс_2400"),
                        InlineKeyboardButton.WithCallbackData("31000", "1_Класс_2480")
                    }
                });
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $@"{callbackQuery.Data}:", replyMarkup: keyboard3);
                return;
            }
            if (callbackQuery.Data.StartsWith("Сумку"))
            {
                InlineKeyboardMarkup keyboard4 = new(new[]
                {
                    InlineKeyboardButton.WithCallbackData("1500", "1_Сумкпереноска_75"),
                    InlineKeyboardButton.WithCallbackData("2000", "1_Сумкпереноска_100"),
                });
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $@"{callbackQuery.Data}:", replyMarkup: keyboard4);
                return;
            }
            if (callbackQuery.Data.StartsWith("Бизнес"))
            {
                InlineKeyboardMarkup keyboard5 = new(new[]
                {
                    new[]
                    {
                    InlineKeyboardButton.WithCallbackData("МВЛ 3000", "1_МВЛ_300"),
                    InlineKeyboardButton.WithCallbackData("МВЛ 1500", "1_МВЛ_150")
                    },
                    new[]
                    {
                    InlineKeyboardButton.WithCallbackData("ВВЛ 3500", "1_ВВЛ_350"),
                    InlineKeyboardButton.WithCallbackData("ВВЛ 1750", "1_ВВЛ_175")
                    },
                });
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $@"{callbackQuery.Data}:", replyMarkup: keyboard5);
                return;
            }
            if (callbackQuery.Data.StartsWith("Повыш"))
            {
                InlineKeyboardMarkup keyboard6 = new(new[]
                {
                    InlineKeyboardButton.WithCallbackData("2500", "1_Классмили_250"),
                });
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"{callbackQuery.Data}:", replyMarkup: keyboard6);
                return;
            }

            //Выбор стоимости продажи и количества
            //Добавляем количество продаж
            if (callbackQuery.Data.StartsWith("1_"))
            {
                string[] arr = callbackQuery.Data.Split('_');
                string name = arr[1];
                int.TryParse(arr[2], out int priceSale);

                InlineKeyboardMarkup keyboard5 = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("1", $"2_{name}_{priceSale}_1"),
                        InlineKeyboardButton.WithCallbackData("2", $"2_{name}_{priceSale}_2"),
                        InlineKeyboardButton.WithCallbackData("3", $"2_{name}_{priceSale}_3"),
                        InlineKeyboardButton.WithCallbackData("4", $"2_{name}_{priceSale}_4"),
                        InlineKeyboardButton.WithCallbackData("5", $"2_{name}_{priceSale}_5"),
                        InlineKeyboardButton.WithCallbackData("6", $"2_{name}_{priceSale}_6")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("7", $"2_{name}_{priceSale}_7"),
                        InlineKeyboardButton.WithCallbackData("8", $"2_{name}_{priceSale}_8"),
                        InlineKeyboardButton.WithCallbackData("9", $"2_{name}_{priceSale}_9"),
                        InlineKeyboardButton.WithCallbackData("10", $"2_{name}_{priceSale}_10"),
                        InlineKeyboardButton.WithCallbackData("11", $"2_{name}_{priceSale}_11"),
                        InlineKeyboardButton.WithCallbackData("12", $"2_{name}_{priceSale}_12")
                    },
                });
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $@"Сколько смогла продать:", replyMarkup: keyboard5);
                return;
            }
            //Заносим в таблица
            if (callbackQuery.Data.StartsWith("2_"))
            {
                string[] arr = callbackQuery.Data.Split('_');
                await RecordNewSale(botClient, arr);
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Добавили {arr[1]} по {arr[2]} на {arr[3]} штук");
                return;
            }
            #endregion
        }

        /// <summary>
        /// Записть в таблицу новой продажи
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="arr"></param>
        static async Task RecordNewSale(ITelegramBotClient botClient, string[] arr)
        {
            string name = arr[1];
            int.TryParse(arr[2], out int priceSale);
            int.TryParse(arr[3], out int amountSale);
            string way = "Sales/sales.csv";
            using (StreamWriter sw = new StreamWriter(way, true, Encoding.Unicode))
            {
                await sw.WriteLineAsync(DateTime.Now.ToShortDateString() + "\t" + name + "\t" + priceSale + "\t" +
                                  amountSale);
            };
        }

        /// <summary>
        /// Сохраняем в JSON ID, имя, сообщения, время
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        static async Task SaveToJSON(ITelegramBotClient botClient, Update update)
        {
            JObject UserInfo = new JObject()
            {
                ["ID"] = $"{update.Id}\n",
                ["Name"] = $"{update.Message.Chat.FirstName}\n",
                ["Date"] = $"{DateTime.Now.ToLongTimeString()}\n",
                ["Message"] = $"{update.Message.Text}\n",
            };
            JObject User = new JObject();
            JArray MSG = new JArray();

            MSG.Add($"{User}\n");
            MSG.Add(UserInfo);

            JObject MainTree = new JObject() { ["MSG"] = MSG,};

            string json = JsonConvert.SerializeObject(MainTree);

            await System.IO.File.AppendAllTextAsync(@"JSON\txt.json", json);


        }

        public static Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка ТГ API:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }

}