using Notify = System.Windows.Forms;

namespace AmbiledService.Services
{
    sealed class NotifyIconService
    {
        public NotifyIconService()
        {

        }
        public void ShowIcon()
        {
            Notify.NotifyIcon icon = new Notify.NotifyIcon();
        }
    }
}
