namespace Meebey.SmartIrc4net
{
    public class NonRfcChannelUser : ChannelUser
    {
        private bool _IsHalfop;
        internal NonRfcChannelUser(string channel, IrcUser ircuser) : base(channel, ircuser)
        {
        }

#if LOG4NET
        ~NonRfcChannelUser()
        {
            Logger.ChannelSyncing.Debug("NonRfcChannelUser ("+Channel+":"+IrcUser.Nick+") destroyed");
        }
#endif
        public bool IsHalfop {
            get {
                return _IsHalfop;
            }
            set {
                _IsHalfop = value;
            }
        }
    }
}
