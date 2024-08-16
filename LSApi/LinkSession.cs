using XA_SESSIONLib;

namespace LSApi
{
    public class LinkSession
    {
        #region Instance
        public XASessionClass EAPI { get; set; } = new XASessionClass();
        #endregion

        #region Connect to the brokerage API server
        public LinkSession()
        {
            EAPI._IXASessionEvents_Event_Login += XASession_Login;

            bool isConnectServer = EAPI.ConnectServer("api.ls-sec.co.kr", 20001);
            if (!isConnectServer)
            {
                var errCode = EAPI.GetLastError();
                var errMsg  = EAPI.GetErrorMessage(errCode);
                Program.ILog.Warning($"Failed session > {errCode},{errMsg}");
                return;
            }

            bool isLogin = EAPI.Login(App.IConf["LS:ID"], App.IConf["LS:PW"], App.IConf["LS:Cert"], 0, false);
            if (!isLogin)
            {
                Program.ILog.Warning("Failed login");
                return;
            }
        }
        #endregion

        #region Received Login Event
        private void XASession_Login(string code, string msg)
        {
            Program.ILog.Information($"Success Login > {code}, {msg}");
            if (code != "0000") return;
            App.InitialApp();
        }
        #endregion

        #region Request Logout
        public void Logout()
        {
            EAPI.Logout();
            Program.ILog.Warning($"Logout > {EAPI.Logout()}");
        } 
        #endregion
    }
}