using System;
using Windows.UI.Popups;

namespace Eredmenyek.Infrasturcture
{
    public class ExceptionHandler
    {
        public static async void HandleException(string message)
        {
            var messageDialog = new MessageDialog(message);
            messageDialog.Title = "Error";
            await messageDialog.ShowAsync();
        }
    }
}
