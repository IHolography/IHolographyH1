using System;
using Cognex.DataMan.SDK;
using Cognex.DataMan.SDK.Utils;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace iHolography
{
    namespace CognexCamListener
    {
        public class Connector
        {
			public delegate void Barcode(string mess);
			public delegate void Connection(string mess);

			public event Barcode BarcodeDetectOK;
			public event Barcode BarcodeDetectFailed;
			public event Barcode PictureSaved;
			public event Connection ConnectionSeccuess;
			public event Connection ConnectionDisconnected;

			private ResultCollector _results;
			private ISystemConnector _connector = null;
			private DataManSystem _system = null;

			private string _user="admin";
			private string _path = @"C:\Users\Public\Pictures";
			private string _password = "";

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
			public string Path
			{
				get
				{
					return _path;
				}
				set
				{
					string dir = value;
					if (dir[dir.Length - 1] == '\\') dir += DateTime.Now.ToString("yyyy-MM-dd");
					else dir += "\\"+DateTime.Now.ToString("yyyy-MM-dd");
					if (!Directory.Exists(dir))
						{
							try
							{
								Directory.CreateDirectory(dir);
							}
							catch
							{
								dir = @"C:\Users\Public\Pictures\iHolography\" + DateTime.Now.ToString("yyyy-MM-dd");
							}
						}
					_path = dir;
				}
			}
			public Status Status { get; private set; }
			public Size PictureSize { get; private set; }
			public int PctByScan { get; set; }
			public System.Net.IPAddress IPAddress { get; private set; }

			public Connector(string user, string password, string masterCameraIP, string picturePath, Size pictureSize, int pctByScan)
            {
				User = user;
				Password = password;
				IPAddress = System.Net.IPAddress.Parse(masterCameraIP);
				Path = picturePath;
				PictureSize = pictureSize;
				PctByScan = pctByScan;
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
                    ResultTypes requested_result_types = ResultTypes.ReadXml | ResultTypes.Image | ResultTypes.ImageGraphics;
                    //ResultTypes requested_result_types = ResultTypes.ReadXml;
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
				List<Image> images = new List<Image>();
				List<string> image_graphics = new List<string>();
				string read_result = null;
				int result_id = -1;
				int barcodeCount = 0;
				ResultTypes collected_results = ResultTypes.None;
				foreach (var simple_result in e.SimpleResults)
				{
					collected_results |= simple_result.Id.Type;
					switch (simple_result.Id.Type)
					{
						case ResultTypes.Image:
							Image image = ImageArrivedEventArgs.GetImageFromImageBytes(simple_result.Data);
							if (image != null)
							images.Add(image);
							break;

						case ResultTypes.ImageGraphics:
							image_graphics.Add(simple_result.GetDataAsString());
							break;

						case ResultTypes.ReadXml:
							read_result = GetReadStringFromResultXml(simple_result.GetDataAsString());
							result_id = simple_result.Id.Id;
							break;

						case ResultTypes.ReadString:
							read_result = simple_result.GetDataAsString();
							result_id = simple_result.Id.Id;
							break;
					}
				}

				//BarcodeDetect?.Invoke(read_result.Split('\n').Length.ToString());
				barcodeCount = Regex.Matches(read_result, "$", RegexOptions.Multiline).Count;

				if (images.Count > 0 && PctByScan!=barcodeCount)
				{
					Image first_image = images[0];
					Size image_size = Gui.FitImageInControl(first_image.Size, PictureSize);
					Image fitted_image = Gui.ResizeImageToBitmap(first_image, image_size);

					if (image_graphics.Count > 0)
					{
						using (Graphics g = Graphics.FromImage(fitted_image))
						{
							foreach (var graphics in image_graphics)
							{
								ResultGraphics rg = GraphicsResultParser.Parse(graphics, new Rectangle(0, 0, image_size.Width, image_size.Height));
								ResultGraphicsRenderer.PaintResults(g, rg);
							}
						}
					}
					fitted_image.Save(_path+"//"+ DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
					PictureSaved?.Invoke("Picture Saved");
				}

				if (PctByScan != barcodeCount) BarcodeDetectFailed?.Invoke(read_result);
				else BarcodeDetectOK?.Invoke(read_result);
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
