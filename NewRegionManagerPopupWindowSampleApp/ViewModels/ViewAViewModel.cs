using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace NewRegionManagerPopupWindowSampleApp.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        public InteractionRequest<INotification> PopupWindowRequest { get; } = new InteractionRequest<INotification>();

        public DelegateCommand PopupWindowCommand { get; }

        public ViewAViewModel()
        {
            this.PopupWindowCommand = new DelegateCommand(() =>
                this.PopupWindowRequest.Raise(new Notification { Title = "Sample", Content = "Sample" }));
        }
    }
}
