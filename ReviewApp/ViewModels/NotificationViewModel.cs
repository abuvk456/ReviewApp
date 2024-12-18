using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonModel;
using ReviewApp.Model;
using ReviewApp.Repository;
using ReviewApp.Services;

namespace ReviewApp.ViewModels
{
    public class NotificationViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private readonly int _userId;
        private ObservableCollection<Notification> _notifications;
        private Notification _selectedNotification;

        public NotificationViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService;

            RefreshCommand = new Command(async () => await LoadNotifications());
            MarkAsReadCommand = new Command<Notification>(async (notification) => await MarkAsRead(notification));
            DeleteCommand = new Command<Notification>(async (notification) => await DeleteNotification(notification));
            Task.Run(async () => await LoadNotifications());
        }


        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged();
            }
        }

        public Notification SelectedNotification
        {
            get => _selectedNotification;
            set
            {
                _selectedNotification = value;
                if (value != null)
                {

                    MarkAsRead(value);
                }
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; set; }
        public ICommand MarkAsReadCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public async Task LoadNotifications()
        {
            var notifications = await _notificationService.GetNotificationsAsync(Globals.CurrentUserID);
            Notifications = notifications.ToObservableCollection();

        }
        private async Task MarkAsRead(Notification notification)
        {
            var TPVM = MyServiceLocator.Services.GetService<TopicsViewModel>();
            var UVM = MyServiceLocator.Services.GetService<UsersViewModel>();
            switch (notification.TypeOfNotification.ToLower())
            {
                case "new message":
                    //todo: open messages
                    await Shell.Current.Navigation.PushAsync(new Views.MessagesPage());
                    break;
                case "new follower":
                    //todo: Open list of Follower
                    break;

                case "new comment":
                    if (notification.RelatedDataID != null)
                    {
                        
                       // await Shell.Current.Navigation.PushAsync(new Views.TopicDetail(TPVM, notification.RelatedDataID));
                        //todo: open Comments
                    }
                    else
                    {
                        
                        await UVM.LoadUserInfoAsync(notification.CreatedBy);
                    }
                        break;
                case "upvote":
                    
                    await UVM.LoadUserInfoAsync(notification.CreatedBy);
                    //todo: Show user who voted
                    break;
                case "downvote":
                    
                    await UVM.LoadUserInfoAsync(notification.CreatedBy);
                    //todo: Show user voted
                    break;
            }
            if (notification.IsRead)
            {
                notification.IsRead = true;
                await _notificationService.UpdateNotificationAsync(notification);
            }
            
        }
        private async Task DeleteNotification(Notification notification)
        {
            await _notificationService.DeleteNotificationAsync(notification.ID);
            Notifications.Remove(notification);
        }
    }

}
