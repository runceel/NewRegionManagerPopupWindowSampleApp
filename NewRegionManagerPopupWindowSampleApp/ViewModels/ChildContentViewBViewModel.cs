using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;

namespace NewRegionManagerPopupWindowSampleApp.ViewModels
{
    public class ChildContentViewBViewModel : BindableBase
    {
        public InteractionRequest<INotification> CloseWindowRequest { get; } = new InteractionRequest<INotification>();

        public DelegateCommand CloseWindowCommand { get; }

        public ChildContentViewBViewModel()
        {
            this.CloseWindowCommand = new DelegateCommand(() => this.CloseWindowRequest.Raise(new Notification()));
        }
    }
}
