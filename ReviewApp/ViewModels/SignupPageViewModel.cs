using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using ReviewApp.Model;
using CommonModel;
using Firebase.Auth;
using ReviewApp.Services;
using User = CommonModel.User;
using ReviewApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace ReviewApp.ViewModels
{
    public class SignupPageViewModel : BaseViewModel
    {

        FirebaseAuthProvider firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig("[YOUR_API_KEY]"));
        public Command SignupCommand { get; private set; }
        public ICommand CancelSignUpCommand { get; }
        public SignupPageViewModel()
        {

            GetData();
            SignupCommand = new Command(async () => await SignupFunction());
            CancelSignUpCommand = new Command(() =>
            {
                
                Console.Write("Signup Cancled");
                Application.Current.MainPage = new LoginPage();
            }
            );


        }

        private async Task SignupFunction()
        {
            await Task.Delay(1);
            //todo: signup call here
            //todo:send Email on signup

            ErrorMessage = await SignupAsync();
            ShowLoader = false;
            if (!string.IsNullOrEmpty(ErrorMessage))
                return;
            
            
            var LVM = MyServiceLocator.Services.GetService<LoginViewModel>();
            LVM.Username = Email;
            LVM.Password = Password;
           
            Application.Current.MainPage = new LoginPage();
            LVM.AutoLogin();

        }
        private async void GetData()
        {
            var countries = new ObservableCollection<Country>();
            await Task.Run(() =>
            {
                countries = new ObservableCollection<Country>
                  {
                new Country { Name = "USA", Code = "US" },
                new Country { Name = "Canada", Code = "CA" },
                new Country { Name = "United Kingdom", Code = "UK" },

                  };

            });
            Countries = countries;

            Languages = new ObservableCollection<string> { "English", "French", "German", "Spanish" };
            SelectedCountry = Countries.FirstOrDefault();
            SelectedLanguage = Languages.FirstOrDefault();
            OnPropertyChanged("Countries");
            OnPropertyChanged("Languages");
            OnPropertyChanged("SelectedLanguage");
            OnPropertyChanged("SelectedCountry");
        }

        // Properties for capturing user input
        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private int _age;
        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        private Country _selectedCountry;
        public Country SelectedCountry
        {
            get => _selectedCountry;
            set => SetProperty(ref _selectedCountry, value);
        }
        string _ErrorMessage = "";
        public string ErrorMessage
        {
            get => _ErrorMessage;
            set => SetProperty(ref _ErrorMessage, value);
        }
        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        // Dropdown list items
        public ObservableCollection<Country> Countries { get; set; }
        public ObservableCollection<string> Languages { get; set; }

        public async Task<string> SignupAsync()
        {
          
            // Validate user input
            if (string.IsNullOrEmpty(Email))
                return "Email is required.";
           
            if (string.IsNullOrEmpty(FirstName))
                return "First name is required.";
            if (string.IsNullOrEmpty(LastName))
                return "Last name is required.";
            if (string.IsNullOrEmpty(Password))
                return "Password is required.";
            if (Age <= 0)
                return "Age is required.";
            ShowLoader = true;
            // Register new user with Firebase
            try
            {


                var user = new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Age = Age,
                    Country = SelectedCountry?.Name,
                    Language = SelectedLanguage,
                    Email = Email,
                    Username = Email,
                    Password = Password

                };
                var apiclient = new ApiClient();
                var signupsucess = await apiclient.AddUser(user);
                if (signupsucess > 0)
                {
                    await App.Current.MainPage.DisplayAlert("Sucess", "You have sucessfully Signed up", "ok");
                    
                    return "";
                }
                else if(signupsucess==-1)
                {
                    await App.Current.MainPage.DisplayAlert("Sucess", "Signup Failed, a user with email already exits", "ok");

                }
                else 
                {
                    await App.Current.MainPage.DisplayAlert("Sucess", "Signup Failed, please try again", "ok");

                }
            }
            catch
            {

            }
            ShowLoader = false;
            return "Signup failed";
        }


        public void AddAdditionalInfo(User _user)
        {


        }
    }
}
