using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using System.Windows.Threading;

namespace WPFapp1TGbot
{
    class TGmsgClient
    {
        private MainWindow w;

        private TelegramBotClient botClient;
        public ObservableCollection<MessageLog> BotMessageLog { get; set; }


        public TGmsgClient(MainWindow W)
        {
            BotMessageLog = new ObservableCollection<MessageLog>();
            w = W;

            var botClient = new TelegramBotClient(System.IO.File.ReadAllText(@"\\Mac\Home\Desktop\token.txt"));
            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            botClient.StartReceiving(MyFunction.HandleUpdateAsync,
                MyFunction.HandleErrorAsync,
                receiverOptions,
                cts.Token);
            
            //cts.Cancel();
        }

        public void SendMsg (String Text, String ID)
        {
            long id = Convert.ToInt64(ID);
            botClient.SendTextMessageAsync(id, Text);
        }
    }
}
