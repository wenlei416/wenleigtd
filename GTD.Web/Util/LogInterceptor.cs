using Ninject.Extensions.Interception;

namespace GTD.Util
{
    public class LogInterceptor : SimpleInterceptor
    {
        protected override void BeforeInvoke(IInvocation invocation)
        {
            LogHelper.WriteLog($"BeforeInvoke : Method: {invocation.Request.Method.Name} Arguments {invocation.Request.Arguments[0]} returned {invocation.ReturnValue}");
        }

        protected override void AfterInvoke(IInvocation invocation)
        {
            LogHelper.WriteLog($"AfterInvoke  : Method: {invocation.Request.Method.Name} Arguments: {invocation.Request.Arguments[0]} Returned: {invocation.ReturnValue}");
        }
    }
}