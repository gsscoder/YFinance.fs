module Network.Yahoo.StockQuote

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
