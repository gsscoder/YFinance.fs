module Network.Yahoo.HistoricalQuote

open Chiron
open Chiron.Operators

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
