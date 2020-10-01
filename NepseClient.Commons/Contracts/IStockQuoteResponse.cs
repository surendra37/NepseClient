using System;
using System.Collections.Generic;

namespace NepseClient.Commons.Contracts
{
    public interface IStockQuoteResponse
    {
        double AverageTradedPrice { get;  }
        double Change { get;  }
        double ChangePercentage { get;  }
        double ClosePrice { get;  }
        double DayHigh { get;  }
        double DayLow { get;  }
        int LastTradedQty { get;  }
        DateTime LastTradedTime { get;  }
        double Ltp { get;  }
        double OpenPrice { get;  }
        ISecurityResponse Security { get;  }
        IEnumerable<ITopOrderResponse> TopBuy { get;  }
        IEnumerable<ITopOrderResponse> TopSell { get;  }
        int TotalBuyQty { get;  }
        int TotalSellQty { get;  }
        int Volume { get;  }
    }
}