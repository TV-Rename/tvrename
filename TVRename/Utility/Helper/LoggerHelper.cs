using NLog;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Text;
using System.Text.RegularExpressions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TVRename;

public static class LoggerHelper
{
    public static ILogger AsILogger(this Logger baseLogger, string filename) => new NlogILogger(baseLogger,filename);

    public static string ErrorText(this Exception e)
    {
        StringBuilder returnValue = new();
        try
        {
            MethodBase? site = e.TargetSite;//Get the methodname from the exception.
            string methodName = site == null ? string.Empty : site.Name;//avoid null ref if it's null.
            methodName = ExtractBracketed(methodName);

            returnValue.AppendLine($"{ThreadAndDateInfo}Exception: {methodName}: {e.Message} from {e.Source}");

            StackTrace stkTrace = new(e, true);
            for (int i = 0; i < 3; i++)
            {
                //In most cases GetFrame(0) will contain valid information, but not always. That's why a small loop is needed. 
                StackFrame? frame = stkTrace.GetFrame(i);
                if (frame == null)
                {
                    continue;
                }
                int lineNum = frame.GetFileLineNumber();//get the line and column numbers
                int colNum = frame.GetFileColumnNumber();
                string className = ExtractBracketed(frame.GetMethod()?.ReflectedType?.FullName);
                returnValue.AppendLine(Helpers.Tab + ": " + className + "." + methodName + ", Ln " + lineNum + " Col " + colNum);
                if (lineNum + colNum > 0)
                {
                    break; //exit the for loop if you have valid info. If not, try going up one frame...
                }
            }
        }
        catch (Exception ee)
        {
            //Avoid any situation that the Trace is what crashes you application. While trace can log to a file. Console normally not output to the same place.
            return "**** Exception in ErrorText(Exception e): " + ee.Message;
        }
        return returnValue.ToString();
    }

    private static string ExtractBracketed(string? str)
    {
        if (str is null)
        {
            return string.Empty;
        }
        string s = str.IndexOf('<') > -1
            ? Regex.Match(str, @"\<([^>]*)\>").Groups[1].Value
            : str; //using the Regex when the string does not contain <brackets> returns an empty string.

        return s == ""
            ? "'Empty'" //for log visibility we want to know if something it's empty.
            : s;
    }

    public static string ThreadAndDateInfo =>
        //returns thread number and precise date and time.
        "[" + Environment.CurrentManagedThreadId + " - " + DateTime.Now.ToString("dd/MM HH:mm:ss.ffffff") + "] ";
}
