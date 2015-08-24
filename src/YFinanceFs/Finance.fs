module Network.Yahoo.Finance

open System
open Chiron
open Chiron.Operators
open HttpClient

module Option =
    let fromSingletonList xs =
        match xs with
        | [x] -> Some x
        | _ -> None

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

let parseResponse json =
    match Json.parse json with
    | Object values ->
      match values |> Map.find "query" with
      | Object values ->
        match values |> Map.find "results" with
        | Object values -> 
          let node = values |> Map.find "quote"
          match node with
          | Array quotes ->
            quotes |> List.map (Json.deserialize : _ -> StockQuote)
          | Object _ ->
            [Json.deserialize node]
          | _ -> []
        | _ -> []
      | _ -> []
    | _ -> []

let runRequest yql =
    let request = 
        let yahoo = "https://query.yahooapis.com/v1/public/yql"
        let datatable = "store://datatables.org/alltableswithkeys"
        createRequest Get yahoo
        |> withQueryStringItem {name = "q"; value = yql}
        |> withQueryStringItem {name = "env"; value = datatable}
        |> withQueryStringItem {name = "format"; value = "json"}
    async {
        let! body = getResponseBodyAsync request
        return body
    }

let generateYQLQuery xs =
    let quoteString s = sprintf "\"%s\"" s
    let addComma s1 s2 = sprintf "%s,%s" s1 s2
    let stocks = xs |> (List.map (quoteString) >> List.reduce (addComma))
    sprintf "select * from yahoo.finance.quote where symbol in (%s)" stocks

/// Fetch a stock quote from Yahoo Finance eg. getStockQuote "GOOGL".
/// Asynchronous version.
let getStockQuoteAsync symbol =
    async {
        let! response = generateYQLQuery [symbol] |> runRequest
        let xs = parseResponse response
        return xs |> Option.fromSingletonList
    }

/// Fetch a stock quote from Yahoo Finance eg. getStockQuote "GOOGL".
/// Asynchronous version.
let getStockQuotesAsync symbols =
    async {
        match symbols with
        | [] -> return []
        | _ ->
            let! response = generateYQLQuery symbols |> runRequest
            return parseResponse response
    }

/// Fetch a stock quote from Yahoo Finance eg. getStockQuote "GOOGL".
let getStockQuote symbol =
    symbol |> getStockQuoteAsync |> Async.RunSynchronously


/// Fetch stock quotes from Yahoo Finance eg. getStockQuotes ["MSFT"; "ORCL"].
let getStockQuotes symbols =
    symbols |> getStockQuotesAsync |> Async.RunSynchronously

/// Indexes

let nasdaqIndex() = getStockQuote "^NDX"

let sp500Index() = getStockQuote "^GSPC"

let ftseIndex() = getStockQuote "^FTSE"

let aimIndex() = getStockQuote "^FTAI"
