using System;
using System.Diagnostics;
using System.IO;

namespace _3F.Model.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ExceptionInfo(this Exception ex)
        {
            StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return string.Format("Message: {0}, File: {1}, Method:{2}, Line:{3}"
                , ex.Message
                , Path.GetFileName(trace.GetFrame(0).GetFileName())
                , trace.GetFrame(0).GetMethod()
                , trace.GetFrame(0).GetFileLineNumber());
        }
    }
}
