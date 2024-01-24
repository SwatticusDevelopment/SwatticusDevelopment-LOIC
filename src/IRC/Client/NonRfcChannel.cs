using System;
using System.Collections;
using System.Collections.Specialized;

namespace Meebey.SmartIrc4net
{
    public class NonRfcChannel : Channel
    {
        private Hashtable _Halfops = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));
        internal NonRfcChannel(string name) : base(name)
        {
        }

#if LOG4NET
        ~NonRfcChannel()
        {
            Logger.ChannelSyncing.Debug("NonRfcChannel ("+Name+") destroyed");
        }
#endif
        public Hashtable Halfops {
            get {
                return (Hashtable)_Halfops.Clone();
            }
        }

        internal Hashtable UnsafeHalfops {
            get {
                return _Halfops;
            }
        }
    }
}
