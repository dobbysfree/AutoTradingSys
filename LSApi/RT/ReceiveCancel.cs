using System.Text;
using XA_DATASETLib;

namespace LSApi.RT
{
    internal class ReceiveCancel
    {
        XARealClass xa;
        public ReceiveCancel()
        {
            xa = new XARealClass();
            xa.ResFileName = App.IConf["Ebest:Res"] + "SC3.res";
            xa.ReceiveRealData += ReceiveMessage;
            xa.AdviseRealData();
        }

        public void ReceiveMessage(string tr)
        {
            try
            {
                //if (xa.GetFieldData("OutBlock", "accno") != Conf.IConfig["Ebest:Acnt"]) return;

                string code = xa.GetFieldData("OutBlock", "shtnIsuno").Substring(1);   // Stock Ticker
                App.TradingItems.TryGetValue(code, out Item im);
                if (im == null) return;

                long cncl_qty           = long.Parse(xa.GetFieldData("OutBlock", "canccnfqty"));   // Confirmed Cancelled Quantity
                string bs               = xa.GetFieldData("OutBlock", "bnstp");                    // 1:ASK, 2:BID
                string name             = xa.GetFieldData("OutBlock", "Isunm");                    // Stock Name
                string ordno            = xa.GetFieldData("OutBlock", "ordno");                    // Order NO                
                string orgordno         = xa.GetFieldData("OutBlock", "orgordno");                 // Original Order NO                
                string orgordcancqty    = xa.GetFieldData("OutBlock", "orgordcancqty");            // Original Order Canceled Quantity

              
                //StringBuilder sb = new StringBuilder();
                //sb.Append("type:").Append(im.Buying.Qty == cncl_qty ? "Full" : "Partial").Append("Cancellation").Append(", ");
                //sb.Append("code:").Append(code).Append(", ");
                //sb.Append("name:").Append(name).Append(", ");
                //sb.Append("side:").Append(Dic.BuySell[bs]).Append(", ");
                //sb.Append("ordno:").Append(ordno).Append(", ");
                //sb.Append("orgordno:").Append(orgordno).Append(", ");
                //sb.Append("orgqty:").Append(im.Buying.Qty).Append(", ");
                //sb.Append("cnclqty:").Append(cncl_qty).Append(", ");
                //sb.Append("orgcnclqty:").Append(orgordcancqty).Append(", ");
                //sb.Append("askprc:").Append(im.HogaAsk.price).Append(", ");
                //sb.Append("askrem:").Append(im.HogaAsk.rem).Append(", ");
                //sb.Append("bidprc:").Append(im.HogaBid.price).Append(", ");
                //sb.Append("bidrem:").Append(im.HogaBid.rem).Append(", ");
                //sb.Append("pos:").Append(im.CrntPos);
                //Conf.ILog.Information(sb.ToString());
            }
            catch (Exception ex)
            {
                Program.ILog.Error(ex.ToString());
            }
        }
    }
}