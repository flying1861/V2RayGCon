﻿using System;

namespace VgcApis.Libs.Sys
{
    public class LazyGuy
    {
        Action task = null;
        int timeout = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task">()=>{ ... }</param>
        /// <param name="timeout">millisecond</param>
        public LazyGuy(Action task, int timeout)
        {
            if (task == null || timeout < 1)
            {
                throw new ArgumentException("I am not that lazy!");
            }

            this.task = task;
            this.timeout = timeout;

        }

        #region properties
        CancelableTimeout _lazyTimer = null;
        CancelableTimeout lazyTimer
        {
            get
            {
                if (_lazyTimer == null)
                {
                    _lazyTimer = new CancelableTimeout(task, timeout);
                }
                return _lazyTimer;
            }
        }
        #endregion

        #region public method
        public void ForgetIt()
        {
            lazyTimer.Cancel();
        }

        public void Remember(Action task)
        {
            this.task = task;
        }

        public void DoItLater()
        {
            lazyTimer.Start();
        }

        public void DoItNow()
        {
            lazyTimer.Cancel();

            // Don't hurt me.
            try
            {
                task?.Invoke();
            }
            catch { }
        }

        public void Quit()
        {
            lazyTimer.Cancel();
            lazyTimer.Release();
            task = null;
        }
        #endregion

        #region private method

        #endregion

    }
}
