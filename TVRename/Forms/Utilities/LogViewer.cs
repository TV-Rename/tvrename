//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Windows.Forms;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TVRename;

public partial class LogViewer : Form
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public LogViewer()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        RichTextBoxTarget target = new()
        {
            Name = "UI Target",
            Layout = "${date:format=HH\\:MM\\:ss} ${level:uppercase=true} ${message}",
            ControlName = "logData",
            FormName = "LogViewer",
            AutoScroll = true,
            ToolWindow = true,
            UseDefaultRowColoringRules = true,
            MaxLines = 500,
            MessageRetention = RichTextBoxTargetMessageRetentionStrategy.All,
            SupportLinks = true,
            CreatedForm = false,
            AllowAccessoryFormCreation = false
        };

        LogManager.Configuration.AddTarget(target);
        LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

        LogManager.ReconfigExistingLoggers();
    }

    private void btnFullLog_Click(object sender, EventArgs e)
    {
        FileTarget target = (FileTarget)LogManager.Configuration.FindTargetByName("logfile");
        string logFileName = ((SimpleLayout)target.FileName).FixedText;
        try
        {
            System.Diagnostics.Process.Start("notepad.exe", logFileName);
        }
        catch (Win32Exception)
        {
            try
            {
                System.Diagnostics.Process.Start(logFileName);
            }
            catch (Win32Exception wex2)
            {
                Logger.Error($"Could not open {logFileName}. {wex2.ErrorText()}");
            }
        }
    }
}
