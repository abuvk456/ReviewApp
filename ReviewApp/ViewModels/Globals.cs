using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel;
namespace ReviewApp.ViewModels
{
    public class Globals
    {
    public const string BaseUrl = "http://localhost:7073";
    //public const string BaseUrl = "https://reviewapp101.azurewebsites.net/";

    public static int CurrentUserID{ get; set; }
       public static double deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        public static double MaxWidthAllowed = 420;
        public static User CurrentUser { get; set; }
    }
}
