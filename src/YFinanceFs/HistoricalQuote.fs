module Network.Yahoo.HistoricalQuote

type HistoricalQuote =
    {
        symbol         : string
       ; date          : string
       ; opening       : string
       ; high          : string
       ; low           : string
       ; close         : string
       ; volume        : string
       ; adjustedClose : string
}