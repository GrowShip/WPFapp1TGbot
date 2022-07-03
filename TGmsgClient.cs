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

        private TelegramBotClient bot;
        public ObservableCollection<MessageLog> BotMessageLog { get; set; }


        public TGmsgClient(MainWindow W)
        {
            this.BotMessageLog = new ObservableCollection<MessageLog>();
            this.w = W;

            this.bot = new TelegramBotClient(System.IO.File.ReadAllText(@"\\Mac\Home\Desktop\token.txt"));
            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            bot.StartReceiving(Upd,
                MyFunction.HandleErrorAsync,
                receiverOptions,
                cts.Token);

            //cts.Cancel();
        }
        public void SendMsg(String Text, String Id)
        {
            try
            {
                long id = Convert.ToInt64(Id);
                bot.SendTextMessageAsync(id, Text);
                w.txtMsgSend.Text ="Отправил";
                Task.Delay(1000);
                w.txtMsgSend.Text = "";
            }
            catch (Exception)
            {
                w.txtMsgSend.Text = "Не получается отправить :С";
            }

        }
        private async Task Upd(ITelegramBotClient bot, Update e, CancellationToken cancellationToken)
        {
            await MyFunction.HandleUpdateAsync(bot, e, cancellationToken);

            if (e.Type == UpdateType.Message && e?.Message?.Text != null)
            {
                string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

                Debug.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

                if (e.Message.Text == null) return;

                var messageText = e.Message.Text;

                w.Dispatcher.Invoke(() =>
                {
                    BotMessageLog.Add(
                    new MessageLog(
                        DateTime.Now.ToLongTimeString(), messageText, e.Message.Chat.FirstName, e.Message.Chat.Id));
                });
            }
        }
    }
}
