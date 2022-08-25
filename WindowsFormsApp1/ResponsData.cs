using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class ResponsData
    {
        public long blockNumber;
        public long blockTimestamp;
        public string txnHash;
        public int logIndex;
        public string type;
        public string priceUsd;
        public string volumeUsd;
        public string amount0;
        public string amount1;

       // "blockNumber":19454626,"blockTimestamp":1657535180000,"txnHash":"0x0bc0194c974d828a7cd3c2e4f2e5e0d0e02362f518b1aab7631ece5dcd816b3d",
       // "logIndex":324,"type":"buy","priceUsd":"0.1764","volumeUsd":"586.81","amount0":"3,325","amount1":"2.52
    }
}
