   using System;
   using System.Windows.Forms;
   using System.Threading;
   using System.ComponentModel;

namespace TVRename
{
    public class SplashScreen
    {
        private static BackgroundWorker worker;
        private static ISplashForm displayForm;
        private delegate void UpdateText(string text);
        private delegate void UpdateInt(int number);
        private static System.Timers.Timer fader;
        private static double opacityIncrement = .05;
        private static double opacityDecrement = .125;
        private const int FADE_SPEED = 50;
        private const int WAIT_TIME = 2000;
        private static ManualResetEvent windowCreated;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static double CheckOpacity()
        {
            return ((Form)displayForm).Opacity;
        }

        public static void Show(ISplashForm DisplayForm)
        {
            //Singleton love baby
            if (!EnsureWorker() && worker.IsBusy)
                return;

            if (!(DisplayForm is Form))
                throw new ArgumentException("DisplayForm must be a windows form", "DisplayForm");

            displayForm = DisplayForm;
            ((Form)displayForm).Opacity = 0;
            fader.SynchronizingObject = displayForm;  //Force the timer to run on the forms ui thread

            // Need to block the main thread until the worker 
            // has created the window handle
            windowCreated = new ManualResetEvent(false);
            ((Form)displayForm).HandleCreated += SplashScreenController_HandleCreated;
            worker.RunWorkerAsync(displayForm);
            if (windowCreated.WaitOne(WAIT_TIME))
            {
                ((Form)displayForm).HandleCreated -= SplashScreenController_HandleCreated;

                fader.Start();
            }
            else
            {
                throw new ApplicationException("Did not create form handle within a reasonable time.");
            }
        }

        static void SplashScreenController_HandleCreated(object sender, EventArgs e)
        {
            windowCreated.Set();
        }

        public static void Close()
        {
            if (worker != null && worker.IsBusy && fader != null)
            {
                opacityIncrement = -opacityDecrement;
                fader.Start();
            }
        }

        private static bool EnsureWorker()
        {
            // TODO: Add error handling
            if (worker == null)
            {
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += worker_DoWork;

                fader = new System.Timers.Timer(FADE_SPEED);
                fader.Elapsed += new System.Timers.ElapsedEventHandler(fader_Elapsed);
            }
            return true;
        }

        static void fader_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Form UI thread
            fader.Stop();
            Form splashForm = displayForm as Form;
            if (opacityIncrement > 0 && splashForm.Opacity < 1)
            {
                splashForm.Opacity += opacityIncrement;
            }
            else
            {
                if (splashForm.Opacity > -opacityIncrement)
                    splashForm.Opacity += opacityIncrement;
                else //Opacity is 0 so close the form
                {
                    splashForm.Opacity = 0;
                    splashForm.Close();
                }
            }

            // if the form is fading in or out keep the timer going
            if (splashForm.Opacity > 0 && splashForm.Opacity < 1)
                fader.Start();
        }

        private static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Runs on background thread
            Thread.CurrentThread.Name = "SplashScreen";
            Application.Run((Form)displayForm);
        }

        public static void UpdateStatus(string status)
        {
            if (displayForm != null && ((Form)displayForm).IsHandleCreated)
                displayForm.Invoke(new UpdateText(displayForm.UpdateStatus), new object[] { status });
            logger.Info("Splash screen update status: {0}",status);
        }

        public static void UpdateProgress(int progress)
        {
            if (displayForm != null && ((Form)displayForm).IsHandleCreated)
                displayForm.Invoke(new UpdateInt(displayForm.UpdateProgress), new object[] { progress });
        }

        public static void UpdateInfo(string info)
        {
            if (displayForm != null && ((Form)displayForm).IsHandleCreated)
                displayForm.Invoke(new UpdateText(displayForm.UpdateInfo), new object[] { info });
            logger.Info("Splash screen update info: {0}", info);
        }
    }
}
