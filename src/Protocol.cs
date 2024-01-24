

using System;

namespace LOIC {

	/// <summary>
	/// Protocol.
	/// </summary>
	public enum Protocol
	{
		None = 0,

		TCP = 1,

		UDP = 2,

		HTTP = 3,

		slowLOIC = 4,

		ReCoil = 5,
		
		ICMP = 6,
	}
}