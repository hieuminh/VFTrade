using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFTrade
{
    public class Order
    {
        public string Date;
        public string SoTK;
        public string OrderType;
        public string MaCK;
        public string Price; // 50.0 , MP, ATC, ATO, MAK, MOK
        public decimal Volume;
        public string MatchedPrice;
        public string CtyCK;
        public string TKTong;
        public string SHL;
    }
}
