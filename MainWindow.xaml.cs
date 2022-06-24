using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;

namespace WPFapp1TGbot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        static async Task Main(string[] args)
        {
            string path = "/Users/ilya/Desktop/token.txt";
            var botClient = new TelegramBotClient(System.IO.File.ReadAllText(path));

            using var cts = new CancellationTokenSource();

            // Bot = new TelegramBotClient(token);
            // using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            botClient.StartReceiving(MyFunction.HandleUpdateAsync,
                MyFunction.HandleErrorAsync,
                receiverOptions,
                cts.Token);

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Бот @{me.Username} запущен и ждет сообщений...");
            Console.ReadLine();

            cts.Cancel();
        }
    }
}
