using System;
namespace ReviewApp.ViewModels
{
    public class MenuItemViewModel : BaseViewModel
    {
        private string _title;
        private string _icon;
        private Type _pageType;
        private bool _isSelected;
        private Command _command;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        public Type PageType
        {
            get { return _pageType; }
            set { SetProperty(ref _pageType, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public Command Command
        {
            get { return _command; }
            set { SetProperty(ref _command, value); }
        }
    }

}

