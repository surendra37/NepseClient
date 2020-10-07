namespace NepseClient.Commons.Contracts
{
    public interface IOHLCDataResponse
    {
        float ClosePrice { get;  }
        float HighPrice { get;  }
        float LastTradePrice { get;  }
        float LowPrice { get;  }
        float OpenPrice { get;  }
        float PerChange { get;  }
        string SecurityName { get;  }
        string Symbol { get;  }
    }
}