using System;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using ReviewApp.Services;
using CommonModel;
using ReviewApp.Views;
using System.Security.Cryptography;

namespace ReviewApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private UsersViewModel _usersViewModel;

        private string _username = "asg@gmail.com";
        private string _password = "Pakistan@321";
        private User CurrentUser;
        public LoginViewModel(IConfiguration config, IUserService userService)
        {
            _config = config;
            var apiKey = _config["willaddkeyhere"];
            LoginCommand = new Command(async () => await Login());
            ForgotPasswordCommand = new Command<string>(async (command) => await ForgotPassword(command));
            SignUpCommand = new Command(async () => await SignUp());
            _userService = userService;
            _usersViewModel = MyServiceLocator.Services.GetService<UsersViewModel>();
        }
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }

        public ICommand ForgotPasswordCommand { get; }

        public ICommand SignUpCommand { get; }


        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await Login());
            ForgotPasswordCommand = new Command<string>(async (command) => await ForgotPassword(command));
            SignUpCommand = new Command(async () => await SignUp());

        }

        private async Task Login()
        {
            ShowLoader = true;
            _usersViewModel.CurrentUser = null;

            bool LoginScuessfull = false;
            await Task.Run(async () =>
            {

                CurrentUser = await _userService.LoginUserAsync(_username, Password);
                if (CurrentUser != null)
                {
                    Globals.CurrentUserID = CurrentUser.UserId;
                    Helper.SetSessionAsync(CurrentUser.SessionInfo);
                    _usersViewModel.CurrentUser = CurrentUser;
                    Globals.CurrentUser = CurrentUser;
                    LoginScuessfull = true;
                }
                else
                {
                    _ErrorMessage = "Invaild Email or Password";
                }

                OnPropertyChanged(nameof(ErrorMessage));
            });
            if (LoginScuessfull)
            {

                Application.Current.MainPage = new AppShell();
                //  _usersViewModel.LoadRecomendations();


            }

            ShowLoader = false;
        }
        string _ErrorMessage = "";
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set => SetProperty(ref _ErrorMessage, value);
        }
        private bool _ShowForgetPassword;

        public bool ShowForgetPassword
        {
            get { return _ShowForgetPassword; }
            set { _ShowForgetPassword = value; OnPropertyChanged(); }
        }
        public string GeneratePassword()
        {
            int passwordLength = 8; // Length of the password
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"; // Characters allowed in the password
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            byte[] randomBytes = new byte[passwordLength];
            rng.GetBytes(randomBytes);

            string password = new string(randomBytes
                .Select(x => allowedChars[x % allowedChars.Length])
                .ToArray());
            return password;
        }
        private async Task ForgotPassword(string command)
        {
            switch (command)
            {
                case "open_forget_password":
                    ShowForgetPassword = true;
                    break;
                case "send_recovery":
                    ShowForgetPassword = false;
                    await _userService.ResetPassword(Username, GeneratePassword());
                    await App.Current.MainPage.DisplayAlert("Success", "We have sent any on your registered email address, plese check for further action", "ok");
                    break;
                case "cancel_recovery":
                    ShowForgetPassword = false;
                    break;


            }
            // Will Perform forgot password logic here
        }

        private async Task SignUp()
        {
            var signupviewmodel = MyServiceLocator.Services.GetService<SignupPageViewModel>();
            Application.Current.MainPage = new SignupPage(signupviewmodel);


            //Will Navigate to sign up page
        }

        internal async Task AutoLogin()
        {
            ShowLoader = true;
            await Task.Delay(2000);
            ShowLoader = false;
            LoginCommand.Execute(null);
        }
    }
}


