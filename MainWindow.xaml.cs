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
        TGmsgClient bot;
        public MainWindow()
        {
            InitializeComponent();

            bot = new TGmsgClient(this);

            logList.ItemsSource = bot.BotMessageLog ;
        }

        void btnMsgSend_Click(object sender, RoutedEventArgs e)
        {
            bot.SendMsg(txtMsgSend.Text, IDtarget.Text);
        }
    }
}
