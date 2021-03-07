using System.Threading;

namespace RealTimeCharts_Server.Services.BackgroundWorker
{
    public interface IBackgroundWorkerServiceOptions
    {
        bool SendMessages { get; set; }
    }

    public class BackgroundWorkerServiceOptions : IBackgroundWorkerServiceOptions
    {
        private readonly ReaderWriterLockSlim _readerWriterLockSlim = new();
        private bool _sendMessages;


        public bool SendMessages
        {
            get
            {
                _readerWriterLockSlim.EnterReadLock();
                try
                {
                    return _sendMessages;
                }
                finally
                {
                    _readerWriterLockSlim.ExitReadLock();
                }
            }

            set
            {
                _readerWriterLockSlim.EnterWriteLock();
                try
                {
                    _sendMessages = value;
                }
                finally
                {
                    _readerWriterLockSlim.ExitWriteLock();
                }
            }
        }
    }
}