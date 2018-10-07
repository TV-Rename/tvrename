using System;
using System.Windows.Forms;
using NLog;
using NLog.Config;
using NLog.Windows.Forms;

namespace TVRename
{
    public partial class LogViewer : Form
    {
        public LogViewer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RichTextBoxTarget target = new RichTextBoxTarget
            {
                Name ="UI Target",
                Layout = "${date:format=HH\\:MM\\:ss} ${level:uppercase=true} ${message}",
                ControlName = "logData",
                FormName = "LogViewer",
                AutoScroll = true,
                ToolWindow=true,
                UseDefaultRowColoringRules = true,
                MaxLines = 500,
                MessageRetention=RichTextBoxTargetMessageRetentionStrategy.All,
                SupportLinks=true,
                CreatedForm = false,
                AllowAccessoryFormCreation=false
            };

            LogManager.Configuration.AddTarget(target);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));

            LogManager.ReconfigExistingLoggers();
        }
    }
}
