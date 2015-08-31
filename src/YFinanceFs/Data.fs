module Network.Yahoo.Data

open Chiron
open Chiron.Operators

type StockQuote =
    {
        name     : string
      ; symbol   : string
      ; price    : string
      ; change   : string
      ; dayHigh  : string
      ; dayLow   : string
      ; yearHigh : string
      ; yearLow  : string
    }
    static member FromJson (_ : StockQuote) =
            (fun n s p c d d' y y' -> 
             {
               name        = n
               symbol      = s
               price       = p
               change      = c
               dayHigh     = d
               dayLow      = d'
               yearHigh    = y
               yearLow     = y'
             })
            <!> Json.read "Name"
            <*> Json.read "Symbol"
            <*> Json.read "LastTradePriceOnly"
            <*> Json.read "Change"
            <*> Json.read "DaysHigh"
            <*> Json.read "DaysLow"
            <*> Json.read "YearHigh"
            <*> Json.read "YearLow"

type HistoricalQuote =
    {
         symbol        : string
       ; date          : string
       ; opening       : string
       ; high          : string
       ; low           : string
       ; close         : string
       ; volume        : string
       ; adjustedClose : string
    }
    static member FromJson (_ : HistoricalQuote) =
            (fun s d o h l c v a -> 
             {
               symbol        = s
               date          = d
               opening       = o
               high          = h
               low           = l
               close         = c
               volume        = v
               adjustedClose = a
             })
            <!> Json.read "Symbol"
            <*> Json.read "Date"
            <*> Json.read "Open"
            <*> Json.read "High"
            <*> Json.read "Low"
            <*> Json.read "Close"
            <*> Json.read "Volume"
            <*> Json.read "Adj_Close"