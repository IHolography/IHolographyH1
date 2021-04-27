using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.DataMan.SDK.Discovery;
using Cognex.DataMan.SDK;
using Cognex.DataMan.SDK.Utils;
using System.Threading;
using System.Xml;

namespace iHolography
{
    namespace CognexCamListener
    {
        public class Connector
        {
			public delegate void Barcode(string mess);
			public delegate void Connection(string mess);

			public event Barcode BarcodeDetect;
			public event Connection ConnectionSeccuess;
			public event Connection ConnectionDisconnected;

			private ResultCollector _results;
			private ISystemConnector _connector = null;
			private DataManSystem _system = null;

			private string _user="admin";
			private string _password = "";
			public System.Net.IPAddress _iPAddress = default;

			public string User
            {
                get
                {
					return _user;
                }
                set
                {
					_user = value;
                }
            }
			public string Password
            {
                get
                {
					return _password;
                }
				set
                {
					_password = value;
                }
            }
			public Status Status { get; private set; }
			public System.Net.IPAddress IPAddress { get; private set; }

			public Connector(string user, string password, string masterCameraIP)
            {
				User = user;
				Password = password;
				IPAddress = System.Net.IPAddress.Parse(masterCameraIP);
            }
            public void Connect()
            {
				try
				{
                    //EthSystemDiscoverer.SystemInfo eth_system_info = MasterCamera as EthSystemDiscoverer.SystemInfo;
                    //EthSystemConnector conn = new EthSystemConnector(eth_system_info.IPAddress, eth_system_info.Port);
                    EthSystemConnector conn = new EthSystemConnector(IPAddress);
                    conn.UserName = User;
						conn.Password = Password;

						_connector = conn;

					_system = new DataManSystem(_connector);
					_system.DefaultTimeout = 5000;


                    #region  Subscribe to events that are signalled when the system is connected / disconnected.
                    _system.SystemConnected += new SystemConnectedHandler(OnSystemConnected);
                    _system.SystemDisconnected += new SystemDisconnectedHandler(OnSystemDisconnected);
                    //_system.SystemWentOnline += new SystemWentOnlineHandler(OnSystemWentOnline);
                    //_system.SystemWentOffline += new SystemWentOfflineHandler(OnSystemWentOffline);
                    //_system.KeepAliveResponseMissed += new KeepAliveResponseMissedHandler(OnKeepAliveResponseMissed);
                    //_system.BinaryDataTransferProgress += new BinaryDataTransferProgressHandler(OnBinaryDataTransferProgress);
                    //_system.OffProtocolByteReceived += new OffProtocolByteReceivedHandler(OffProtocolByteReceived);
                    //_system.AutomaticResponseArrived += new AutomaticResponseArrivedHandler(AutomaticResponseArrived);
                    #endregion

                    #region Subscribe to events that are signalled when the device sends auto-responses.
                    //ResultTypes requested_result_types = ResultTypes.ReadXml | ResultTypes.Image | ResultTypes.ImageGraphics;
                    ResultTypes requested_result_types = ResultTypes.ReadXml;
					_results = new ResultCollector(_system, requested_result_types);
					_results.ComplexResultCompleted += Results_ComplexResultCompleted;
                    //_results.SimpleResultDropped += Results_SimpleResultDropped;
                    #endregion
                    
					//_system.SetKeepAliveOptions(cbEnableKeepAlive.Checked, 3000, 1000);

                    _system.Connect();

					try
					{
						_system.SetResultTypes(requested_result_types);
					}
					catch
					{ }
				}
				catch
				{
					CleanupConnection();
				}
			}
			private void CleanupConnection()
			{
				if (null != _system)
				{
					_system.SystemConnected -= OnSystemConnected;
					_system.SystemDisconnected -= OnSystemDisconnected;
					//_system.SystemWentOnline -= OnSystemWentOnline;
					//_system.SystemWentOffline -= OnSystemWentOffline;
					//_system.KeepAliveResponseMissed -= OnKeepAliveResponseMissed;
					//_system.BinaryDataTransferProgress -= OnBinaryDataTransferProgress;
					//_system.OffProtocolByteReceived -= OffProtocolByteReceived;
					//_system.AutomaticResponseArrived -= AutomaticResponseArrived;
				}

				_connector = null;
				_system = null;
			}
			private void Results_ComplexResultCompleted(object sender, ComplexResult e)
			{
				string read_result = null;
				int result_id = -1;
				foreach (var simple_result in e.SimpleResults)
				{
					switch (simple_result.Id.Type)
					{
						case ResultTypes.ReadXml:
							read_result = GetReadStringFromResultXml(simple_result.GetDataAsString());
							result_id = simple_result.Id.Id;
							BarcodeDetect?.Invoke(read_result);
							break;
					}
				}
			}
			private string GetReadStringFromResultXml(string resultXml)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.LoadXml(resultXml);

					XmlNode full_string_node = doc.SelectSingleNode("result/general/full_string");

					if (full_string_node != null && _system != null && _system.State == ConnectionState.Connected)
					{
						XmlAttribute encoding = full_string_node.Attributes["encoding"];
						if (encoding != null && encoding.InnerText == "base64")
						{
							if (!string.IsNullOrEmpty(full_string_node.InnerText))
							{
								byte[] code = Convert.FromBase64String(full_string_node.InnerText);
								return _system.Encoding.GetString(code, 0, code.Length);
							}
							else
							{
								return "";
							}
						}

						return full_string_node.InnerText;
					}
				}
				catch
				{
				}

				return "";
			}
			public void Disconnect()
			{
				try
				{
					if (_system == null || _system.State != ConnectionState.Connected)
						return;

					_system.Disconnect();

					CleanupConnection();

					_results.ClearCachedResults();
					_results = null;
				}
				catch { }
			}
			private void OnSystemConnected(object sender, EventArgs args)
			{
						ConnectionSeccuess?.Invoke("System connected");
			}
			private void OnSystemDisconnected(object sender, EventArgs args)
			{
						ConnectionDisconnected?.Invoke("System disconnected");
			}
		}
    }
}
