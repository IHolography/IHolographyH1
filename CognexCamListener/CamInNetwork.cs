
using System.Collections.Generic;
using Cognex.DataMan.SDK.Discovery;
namespace iHolography
{
    namespace CognexCamListener
    {
        public class CamInNetwork
        {
            public delegate void Cam(EthSystemDiscoverer.SystemInfo camInfo);
            public event Cam CamDetected;
            public List<EthSystemDiscoverer.SystemInfo> ListOfDiscoveredCam { get; set; }
            
            private EthSystemDiscoverer _ethSystemDiscoverer = null;

            public CamInNetwork()
            {
                _ethSystemDiscoverer = new EthSystemDiscoverer();
                _ethSystemDiscoverer.SystemDiscovered += new EthSystemDiscoverer.SystemDiscoveredHandler(OnEthSystemDiscovered);
                ListOfDiscoveredCam = new List<EthSystemDiscoverer.SystemInfo>();
            }
            public void Discovery()
            {
                _ethSystemDiscoverer.Discover();
            }
            private void OnEthSystemDiscovered(EthSystemDiscoverer.SystemInfo systemInfo)
            {
                ListOfDiscoveredCam.Add(systemInfo);
                CamDetected?.Invoke(systemInfo);
            }
        }
    }
}
