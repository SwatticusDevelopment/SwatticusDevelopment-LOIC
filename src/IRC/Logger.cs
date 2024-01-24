using System.IO;
using System.Collections;

namespace Meebey.SmartIrc4net
{
#if LOG4NET
    /// <summary>
    ///
    /// </summary>
    public enum LogCategory
    {
        Main,
        Connection,
        Socket,
        Queue,
        IrcMessages,
        MessageTypes,
        MessageParser,
        ActionHandler,
        TimeHandler,
        MessageHandler,
        ChannelSyncing,
        UserSyncing,
        Modules,
        Dcc
    }

    /// <summary>
    ///
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    internal class Logger
    {
        private static SortedList _LoggerList = new SortedList();
        private static bool       _Init;

        private Logger()
        {
        }

        public static void Init()
        {
            if (_Init) {
                return;
            }

            _Init = true;


            _LoggerList[LogCategory.Main]           = log4net.LogManager.GetLogger("MAIN");
            _LoggerList[LogCategory.Socket]         = log4net.LogManager.GetLogger("SOCKET");
            _LoggerList[LogCategory.Queue]          = log4net.LogManager.GetLogger("QUEUE");
            _LoggerList[LogCategory.Connection]     = log4net.LogManager.GetLogger("CONNECTION");
            _LoggerList[LogCategory.IrcMessages]    = log4net.LogManager.GetLogger("IRCMESSAGE");
            _LoggerList[LogCategory.MessageParser]  = log4net.LogManager.GetLogger("MESSAGEPARSER");
            _LoggerList[LogCategory.MessageTypes]   = log4net.LogManager.GetLogger("MESSAGETYPES");
            _LoggerList[LogCategory.ActionHandler]  = log4net.LogManager.GetLogger("ACTIONHANDLER");
            _LoggerList[LogCategory.TimeHandler]    = log4net.LogManager.GetLogger("TIMEHANDLER");
            _LoggerList[LogCategory.MessageHandler] = log4net.LogManager.GetLogger("MESSAGEHANDLER");
            _LoggerList[LogCategory.ChannelSyncing] = log4net.LogManager.GetLogger("CHANNELSYNCING");
            _LoggerList[LogCategory.UserSyncing]    = log4net.LogManager.GetLogger("USERSYNCING");
            _LoggerList[LogCategory.Modules]        = log4net.LogManager.GetLogger("MODULES");
            _LoggerList[LogCategory.Dcc]            = log4net.LogManager.GetLogger("DCC");
        }

        public static log4net.ILog Main
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Main];
            }
        }

        public static log4net.ILog Socket
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Socket];
            }
        }

        public static log4net.ILog Queue
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Queue];
            }
        }

        public static log4net.ILog Connection
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Connection];
            }
        }

        public static log4net.ILog IrcMessages
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.IrcMessages];
            }
        }

        public static log4net.ILog MessageParser
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.MessageParser];
            }
        }

        public static log4net.ILog MessageTypes
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.MessageTypes];
            }
        }

        public static log4net.ILog ActionHandler
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.ActionHandler];
            }
        }

        public static log4net.ILog TimeHandler
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.TimeHandler];
            }
        }

        public static log4net.ILog MessageHandler
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.MessageHandler];
            }
        }

        public static log4net.ILog ChannelSyncing
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.ChannelSyncing];
            }
        }

        public static log4net.ILog UserSyncing
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.UserSyncing];
            }
        }

        public static log4net.ILog Modules
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Modules];
            }
        }

        public static log4net.ILog Dcc
        {
            get {
                return (log4net.ILog)_LoggerList[LogCategory.Dcc];
            }
        }
    }
#endif
}
