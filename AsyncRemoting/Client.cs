using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Services;

namespace AsyncRemoting
{
    public class Client : MarshalByRefObject
    {
        public ManualResetEvent e;

        // 声明两个委托，每个委托表示'返回字符串的函数。名字是严格的“为了代码的清晰性，两者之间没有区别
        //“两个代表。（实际上，相同的委托类型'可用于同步和异步'电话。）
        public delegate string RemoteSyncDelegate();
        public delegate string RemoteAsyncDelegate();

        // 这是AsyncCallback委托引用的调用
        public void OurRemoteAsyncCallback(IAsyncResult ar)
        {
            //RemoteAsyncDelegate del = typeof(AsyncResult).AsyncDelegate;

            AsyncResult asyncResult = ar as AsyncResult;
            RemoteAsyncDelegate del = asyncResult.AsyncDelegate as RemoteAsyncDelegate;
            Console.WriteLine("**SUCCESS**: Result of the remote AsyncCallBack: " + del.EndInvoke(ar));
            e.Set();
        }

        static void Main(string[] args)
        {
            // 重要信息：.NET Framework远程处理不会远程处理“静态成员”。该类必须是实例，然后异步调用的回调才能到达此客户端
            Client clientApp = new Client();
            clientApp.Run();

            Console.ReadKey();
        }

        public void Run()
        {
            // 如果您要进行多个异步调用，请启用此调用和底部的e.WaitOne调用
            e = new ManualResetEvent(false);

            Console.WriteLine("Remote synchronous and asynchronous delegates.");
            Console.WriteLine("_" + 80);
            Console.WriteLine();

            // 这是远程处理场景中“同步或异步编程”配置中唯一必须做的事情。
            RemotingConfiguration.Configure("AsyncRemoting.exe.config", false);

            // 其余步骤与单一的“AppDomain编程”相同。
            ServiceClass obj = new ServiceClass();

            // 此委托是远程同步委托。
            RemoteSyncDelegate Remotesyncdel = new RemoteSyncDelegate(obj.VoidCall);

            // 调用时，程序执行将等待直到方法返回。“此委托可以传递到另一个应用程序域”以用作对目标无效呼叫方法
            Console.WriteLine(Remotesyncdel());

            // 委托是异步委托。必须创建两个委托。第一个是系统定义的AsyncCallback委托，它引用远程类型在远程方法完成后调用的方法。
            AsyncCallback RemoteCallback = new AsyncCallback(OurRemoteAsyncCallback);

            // 创建要异步使用的远程方法的委托。
            RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(obj.TimeConsumingRemoteCall);

            // 启动方法调用。请注意，此“线程”的执行将立即继续，而无需等待“方法调用”的返回。
            IAsyncResult RemAr = RemoteDel.BeginInvoke(RemoteCallback, null);

            // 如果要停止此线程上的执行以“等待此特定调用的返回，请检索”从BeginIvoke调用返回的“IAsyncResult”，
            //获取其“WaitHandle”，并暂停线程，例如下一行：'RemAr.AsyncWaitHandle.WaitOne();

            // 一般来说，如果，例如，发生了许多异步调用，并且您希望得到其中任何一个的通知，或者，像这个例子一样，
            //因为在回调可以将结果打印到“控制台”之前，应用程序域可以被回收.e.WaitOne();

            // 当“async call”未返回时，这将模拟此线程中正在进行的其他一些工作。
            int count = 0;
            while (!RemAr.IsCompleted)
            {
                Console.WriteLine("Not completed: " + count);
                count = count + 1;

                // 确保回调线程可以调用回调。
                Thread.Sleep(1);
            }

        }
    }
}
