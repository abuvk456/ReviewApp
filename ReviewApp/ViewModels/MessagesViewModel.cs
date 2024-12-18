using CommonModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReviewApp.Services;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using ReviewApp.Model;

namespace ReviewApp.ViewModels
{
    public class MessagesViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private ObservableCollection<Message> _Messages;

        public ObservableCollection<Message> Messages
        {
            get { return _Messages; }
            set { _Messages = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Message> _ConMessages;

        public ObservableCollection<Message> ConMessages
        {
            get { return _ConMessages; }
            set { _ConMessages = value; OnPropertyChanged(); }
        }
        public ICommand GeneralCommand{ get; private set; }

        private Message _SelectedMessage;

        public Message SelectedMessage
        {
            get { return _SelectedMessage; }
            set { _SelectedMessage = value; OnPropertyChanged(); }
        }
        private Message _SelectedMessageConv;

        public Message SelectedMessageConv
        {
            get { return _SelectedMessageConv; }
            set
            {
               
                _SelectedMessageConv = value;
                if (_SelectedMessageConv != null)
                    PerformAction("viewcon");
                OnPropertyChanged();
               
            }
        }


        public MessagesViewModel(IUserService userService, IMessageService messageService)
        {
            _userService = userService;
            _messageService = messageService;
            GeneralCommand = new Command(PerformAction);
            
        }

        private string _NewMessageText;

        public string NewMessageText
        {
            get { return _NewMessageText; }
            set { _NewMessageText = value; OnPropertyChanged(); }
        }


        public async void PerformAction(object obj)
        {
            var command = (string)obj.ToString().ToLower();
            switch (command)
            {
                case "sendmessage":
                    var NewMessage = new Message
                    {
                        MessageText = NewMessageText,
                        SentTo = SelectedMessageConv.SentBy,
                        SentBy = SelectedMessageConv.SentTo,
                        SentByName = SelectedMessageConv.SentToName,
                        SentToName = SelectedMessageConv.SentByName,
                        SentDatetime = DateTime.Now,
                        MessageID = 11,
                    };
                    ConMessages.Add(NewMessage);
                    NewMessageText = string.Empty;
                    OnPropertyChanged(nameof(ConMessages));
                    await _messageService.AddMessageAsync(NewMessage);
                    break;
                case "viewcon":
                    await Shell.Current.Navigation.PushAsync(new Views.MessageConversation());
                    await LoadConversation();
                    break;
                case "viewone":

                    break;
                case "dellconv":

                    break;
                case "dellone":

                    break;
                case "loadall":

                    break;
            }
        }
        public async Task<bool> SendDirectMessage(Message message)
        {

            return await _messageService.AddMessageAsync(message) > 0;
        }

        public async Task LoadData()
        {
            ShowLoader = true;
            try
            {
                var msgs = await _messageService.GetMessagesAsync(Globals.CurrentUserID);
                Messages = msgs.ToObservableCollection();
            }
            catch
            {

            }
            finally
            {
                ShowLoader = false;
            }
        }
        public async Task LoadConversation()
        {
            ShowLoader = true;
            try
            {
                var msgs = await _messageService.GetMessagesAsync(SelectedMessageConv.SentBy,SelectedMessageConv.SentTo);
                ConMessages = msgs.ToObservableCollection();
            }
            catch
            {

            }
            finally
            {
                ShowLoader = false;
            }
        }
    }


}
