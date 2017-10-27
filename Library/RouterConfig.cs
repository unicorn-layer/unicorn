﻿
using System;
using System.Collections.Generic;
using Unicorn.Internal;
using UnityEngine.Networking;

namespace Unicorn {
	/// <summary>
	/// Router configuration object.
	/// </summary>
	public class RouterConfig : IRouterConfigInternal {
		public RouterConfig() {
			_channelMap = new SortedDictionary<int, int>();
			_connectionConfig = new ConnectionConfig();
		}

		private SortedDictionary<int, int> _channelMap;
		private readonly ConnectionConfig _connectionConfig;
		
		/// <summary>
		/// Add a channel.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="qos"></param>
		public RouterConfig AddChannel(object key, QosType qos) {
			return this;
		}

		int IRouterConfigInternal.ReceiveBufferSize { get { throw new NotImplementedException(); } }

		SortedDictionary<int, int> IRouterConfigInternal.GetChannelMap() {
			return _channelMap;
		}

		ConnectionConfig IRouterConfigInternal.GetConnectionConfig() {
			return _connectionConfig;
		}
	}
}