   using System;
   using System.ComponentModel;
   using System.Threading;
   using System.Timers;
   using System.Windows.Forms;
   using NLog;
   using Timer = System.Timers.Timer;

namespace TVRename
{
    public class SplashScreen
    {
        private static BackgroundWorker _worker;
        private static ISplashForm _displayForm;
        private delegate void UpdateText(string text);
        private delegate void UpdateInt(int number);
        private static Timer _fader;
        private static double _opacityIncrement = .05;
        private static readonly double OpacityDecrement = .125;
        private const int FadeSpeed = 50;
        private const int WaitTime = 2000;
        private static ManualResetEvent _windowCreated;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static double CheckOpacity()
        {
            return ((Form)_displayForm).Opacity;
        }

        public static void Show(ISplashForm displayForm)
        {
            //Singleton love baby
            if (!EnsureWorker() && _worker.IsBusy)
                return;

            if (!(displayForm is Form))
                throw new ArgumentException("DisplayForm must be a windows form", "displayForm");

            _displayForm = displayForm;
            ((Form)_displayForm).Opacity = 0;
            _fader.SynchronizingObject = _displayForm;  //Force the timer to run on the forms ui thread

            // Need to block the main thread until the worker 
            // has created the window handle
            _windowCreated = new ManualResetEvent(false);
            ((Form)_displayForm).HandleCreated += SplashScreenController_HandleCreated;
            _worker.RunWorkerAsync(_displayForm);
            if (_windowCreated.WaitOne(WaitTime))
            {
                ((Form)_displayForm).HandleCreated -= SplashScreenController_HandleCreated;

                _fader.Start();
            }
            else
            {
                throw new ApplicationException("Did not create form handle within a reasonable time.");
            }
        }

        static void SplashScreenController_HandleCreated(object sender, EventArgs e)
        {
            _windowCreated.Set();
        }

        public static void Close()
        {
            if (_worker != null && _worker.IsBusy && _fader != null)
            {
                _opacityIncrement = -OpacityDecrement;
                _fader.Start();
            }
        }

        private static bool EnsureWorker()
        {
            // TODO: Add error handling
            if (_worker == null)
            {
                _worker = new BackgroundWorker
                {
                    WorkerReportsProgress = false,
                    WorkerSupportsCancellation = true
                };
                _worker.DoWork += worker_DoWork;

                _fader = new Timer(FadeSpeed);
                _fader.Elapsed += fader_Elapsed;
            }
            return true;
        }

        static void fader_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Form UI thread
            _fader.Stop();
            Form splashForm = _displayForm as Form;
            if (splashForm != null && (_opacityIncrement > 0 && splashForm.Opacity < 1))
            {
                splashForm.Opacity += _opacityIncrement;
            }
            else
            {
                if (splashForm != null && splashForm.Opacity > -_opacityIncrement)
                    splashForm.Opacity += _opacityIncrement;
                else //Opacity is 0 so close the form
                {
                    if (splashForm != null)
                    {
                        splashForm.Opacity = 0;
                        splashForm.Close();
                    }
                }
            }

            // if the form is fading in or out keep the timer going
            if (splashForm != null && (splashForm.Opacity > 0 && splashForm.Opacity < 1))
                _fader.Start();
        }

        private static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Runs on background thread
            Thread.CurrentThread.Name = "SplashScreen";
            Application.Run((Form)_displayForm);
        }

        public static void UpdateStatus(string status)
        {
            if (_displayForm != null && ((Form)_displayForm).IsHandleCreated)
                _displayForm.Invoke(new UpdateText(_displayForm.UpdateStatus), new object[] { status });
            Logger.Info("Splash screen update status: {0}",status);
        }

        public static void UpdateProgress(int progress)
        {
            if (_displayForm != null && ((Form)_displayForm).IsHandleCreated)
                _displayForm.Invoke(new UpdateInt(_displayForm.UpdateProgress), new object[] { progress });
        }

        public static void UpdateInfo(string info)
        {
            if (_displayForm != null && ((Form)_displayForm).IsHandleCreated)
                _displayForm.Invoke(new UpdateText(_displayForm.UpdateInfo), new object[] { info });
            Logger.Info("Splash screen update info: {0}", info);
        }
    }
}
