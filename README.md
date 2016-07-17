# PopupWindowActionで出したWindowでRegionを使用する

表題の通りのことをしているサンプルプログラムです。

- MainWindow
    - ViewA
	    - ChildViewA(PopupWindowのContent)
		    - ChildContentViewA
			- ChildContentViewB

のViewが定義されています。

MainWindowがShellで、その中に定義されているRegionでViewAが表示されています。そして、PopupWindowでChildViewAが表示されて、その中で定義されているRegionでChildContentViewAとChildContentViewBが画面遷移します。

# PopupWindowActionで新しいRegionManagerを生成する

PopupWindowActionで表示されるWindow（正確には、そのコンテンツ）に新しいRegionManagerを割り当てるために、このサンプルプログラムではBehaviorを使用しています。
以下のようなBehaviorを定義しています。

```cs
using Microsoft.Practices.ServiceLocation;
using Prism.Common;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace NewRegionManagerPopupWindowSampleApp.Commons
{
    public class CreateNewRegionManagerBehavior : Behavior<DependencyObject>
    {
        protected override void OnAttached()
        {
            var rm = ServiceLocator.Current.GetInstance<IRegionManager>();
            var newRegionManager = rm.CreateRegionManager();
            RegionManager.SetRegionManager(this.AssociatedObject, newRegionManager);
            Action<IRegionManagerAware> setRegionManager = x => x.RegionManager = newRegionManager;
            MvvmHelpers.ViewAndViewModelAction(this.AssociatedObject, setRegionManager);
        }

        protected override void OnDetaching()
        {
            RegionManager.SetRegionManager(this.AssociatedObject, null);
            Action<IRegionManagerAware> resetRegionManager = x => x.RegionManager = null;
            MvvmHelpers.ViewAndViewModelAction(this.AssociatedObject, resetRegionManager);
        }
    }
}
```

グローバルなServiceLocatorからRegionManagerを取得して、そこから新しいRegionManagerを作成して、Behaviorを適用したオブジェクトに設定しています。
IRegionManagerAwareインターフェースは、RegionManagerをセットできるだけのシンプルなインターフェースです。

```cs
using Prism.Regions;

namespace NewRegionManagerPopupWindowSampleApp.Commons
{
    public interface IRegionManagerAware
    {
        IRegionManager RegionManager { get; set; }
    }
}
```

# Regionの画面遷移時に、その時使用されているRegionManagerをViewModelに設定する仕組み
DIコンテナからIRegionManagerを設定するデフォルトの方法だと、常にルートのRegionManagerが差し込まれてしまいます。
上記Behaviorで作成されたRegionManagerを差し込むために以下のようなPrismのRegionBehaviorという仕組みを使ってRegionManagerをViewModelに差し込むようにしました。

```cs
using Prism.Common;
using Prism.Regions;
using System;
using System.Collections.Specialized;

namespace NewRegionManagerPopupWindowSampleApp.Commons
{
    public class RegionManagerAwareBehavior : RegionBehavior
    {
        public static string Key { get; } = nameof(RegionManagerAwareBehavior);

        protected override void OnAttach()
        {
            this.Region.ActiveViews.CollectionChanged += this.ActiveViews_CollectionChanged;
        }

        private void ActiveViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Action<IRegionManagerAware> setRegionManager = x => x.RegionManager = this.Region.RegionManager;
                    MvvmHelpers.ViewAndViewModelAction(e.NewItems[0], setRegionManager);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Action<IRegionManagerAware> resetRegionManager = x => x.RegionManager = null;
                    MvvmHelpers.ViewAndViewModelAction(e.OldItems[0], resetRegionManager);
                    break;
            }
        }
    }
}
```

Viewが追加されたらIRegionManagerAwareが設定されてたらRegionManagerを設定するというだけのシンプルなしかけです。
これをBootstrapperのConfigureDefaultRegionBehaviorsをオーバーライドして登録しておきます。

```cs
protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
{
    var f = base.ConfigureDefaultRegionBehaviors();
    f.AddIfMissing(RegionManagerAwareBehavior.Key, typeof(RegionManagerAwareBehavior));
    return f;
}
```

# 使いかた

ChildViewAのViewModelを例に使い方を解説します。IRegionManagerAwareを実装することで、現在のViewに設定されているRegionManagerが設定されるようになります。
あとは、それを使って画面遷移を行うだけです。

```cs
using NewRegionManagerPopupWindowSampleApp.Commons;
using Prism.Mvvm;
using Prism.Regions;

namespace NewRegionManagerPopupWindowSampleApp.ViewModels
{
    public class ChildViewAViewModel : BindableBase, IRegionManagerAware
    {
        private IRegionManager regionManager;

        public IRegionManager RegionManager
        {
            get { return this.regionManager; }
            set { this.SetProperty(ref this.regionManager, value); }
        }

        public void Initialize()
        {
            this.RegionManager.RequestNavigate("Main", "ChildContentViewA");
        }
    }
}
```

あと、新しいRegionManagerを作成したいView（今回の場合はChildViewA）でCreateNewRegionManagerBehaviorを設定しておくことで、自動的にRegionManagerが作成されます。

```xml
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:local="clr-namespace:NewRegionManagerPopupWindowSampleApp.Views"
             xmlns:Prism="http://prismlibrary.com/"
             xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
             xmlns:Commons="clr-namespace:NewRegionManagerPopupWindowSampleApp.Commons"
             xmlns:ei="http://schemas.microsoft.com/expression/2010/interactions"
             x:Class="NewRegionManagerPopupWindowSampleApp.Views.ChildViewA"
             Prism:ViewModelLocator.AutoWireViewModel="True"
             mc:Ignorable="d"
             d:DesignHeight="300"
             d:DesignWidth="300">
    <i:Interaction.Triggers>
        <i:EventTrigger>
            <ei:CallMethodAction TargetObject="{Binding}"
                                 MethodName="Initialize" />
        </i:EventTrigger>
    </i:Interaction.Triggers>
    <i:Interaction.Behaviors>
        <Commons:CreateNewRegionManagerBehavior />
    </i:Interaction.Behaviors>
    <Grid>
        <ContentControl Prism:RegionManager.RegionName="Main" />
    </Grid>
</UserControl>
```

ChildViewA内で定義されているRegionでの画面遷移は、ChildViewAのLoadedイベントでViewModelのInitializeメソッドを呼ぶことで実現しています。
