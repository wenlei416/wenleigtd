using Ninject.Extensions.Interception;

namespace GTD.Util
{
    public class LogInterceptor: SimpleInterceptor
    {
        protected override void BeforeInvoke(IInvocation invocation)
        {
            LogHelper.WriteLog("BeforeInvoke");

        }

        protected override void AfterInvoke(IInvocation invocation)
        {

            LogHelper.WriteLog("AfterInvoke");
        }
    }
}