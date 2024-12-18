using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewApp.ViewModels
{
    public class LoaderFunction
    {
        private readonly TopicsViewModel TopicsViewModel;
        public LoaderFunction()

        {
            TopicsViewModel=MyServiceLocator.Services.GetService<TopicsViewModel>();

        }
        public async Task RefreshData()
        {
           var _TopicsViewModel= MyServiceLocator.Services.GetService<TopicsViewModel>();
            _TopicsViewModel.SelectedUser = null;
            await Task.Run(() =>
            {
                _TopicsViewModel.LoadTopics();
            });
                 
        }

    }
}
