using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ProgressControlSample
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }

            RegisterExceptionHandlingSynchronizationContext();
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private async void SynchronizationContext_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Synchronization Context Unhandled Exception:\r\n" + e.Exception.Message, "爆了 :(")
                .ShowAsync();
        }
    }

    public class ExceptionHandlingSynchronizationContext : SynchronizationContext
    {
        /// <summary>
        /// Registration method.  Call this from OnLaunched and OnActivated inside the App.xaml.cs
        /// </summary>
        /// <returns></returns>
        public static ExceptionHandlingSynchronizationContext Register()
        {
            var syncContext = Current;
            if (syncContext == null)
                throw new InvalidOperationException("Ensure a synchronization context exists before calling this method.");


            var customSynchronizationContext = syncContext as ExceptionHandlingSynchronizationContext;


            if (customSynchronizationContext == null)
            {
                customSynchronizationContext = new ExceptionHandlingSynchronizationContext(syncContext);
                SetSynchronizationContext(customSynchronizationContext);
            }


            return customSynchronizationContext;
        }

        /// <summary>
        /// Links the synchronization context to the specified frame
        /// and ensures that it is still in use after each navigation event
        /// </summary>
        /// <param name="rootFrame"></param>
        /// <returns></returns>
        public static ExceptionHandlingSynchronizationContext RegisterForFrame(Frame rootFrame)
        {
            if (rootFrame == null)
                throw new ArgumentNullException(nameof(rootFrame));

            var synchronizationContext = Register();

            rootFrame.Navigating += (sender, args) => EnsureContext(synchronizationContext);
            rootFrame.Loaded += (sender, args) => EnsureContext(synchronizationContext);

            return synchronizationContext;
        }

        private static void EnsureContext(SynchronizationContext context)
        {
            if (Current != context)
                SetSynchronizationContext(context);
        }


        private readonly SynchronizationContext _syncContext;


        public ExceptionHandlingSynchronizationContext(SynchronizationContext syncContext)
        {
            _syncContext = syncContext;
        }


        public override SynchronizationContext CreateCopy()
        {
            return new ExceptionHandlingSynchronizationContext(_syncContext.CreateCopy());
        }


        public override void OperationCompleted()
        {
            _syncContext.OperationCompleted();
        }


        public override void OperationStarted()
        {
            _syncContext.OperationStarted();
        }


        public override void Post(SendOrPostCallback d, object state)
        {
            _syncContext.Post(WrapCallback(d), state);
        }


        public override void Send(SendOrPostCallback d, object state)
        {
            _syncContext.Send(d, state);
        }


        private SendOrPostCallback WrapCallback(SendOrPostCallback sendOrPostCallback)
        {
            return state =>
            {
                try
                {
                    sendOrPostCallback(state);
                }
                catch (Exception ex)
                {
                    if (!HandleException(ex))
                        throw;
                }
            };
        }

        private bool HandleException(Exception exception)
        {
            if (UnhandledException == null)
                return false;

            var exWrapper = new UnhandledExceptionEventArgs
            {
                Exception = exception
            };

            UnhandledException(this, exWrapper);

#if DEBUG && !DISABLE_XAML_GENERATED_BREAK_ON_UNHANDLED_EXCEPTION
            if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
#endif

            return exWrapper.Handled;
        }


        /// <summary>
        /// Listen to this event to catch any unhandled exceptions and allow for handling them
        /// so they don't crash your application
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;
    }

    public class UnhandledExceptionEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public Exception Exception { get; set; }
    }

}
