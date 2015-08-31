module Network.Yahoo.Finance

open System
open Chiron
open Chiron.Operators
open Aether
open Aether.Operators
open HttpClient
open Network.Yahoo.Data
open Network.Yahoo.Interpolate

let toDouble s = Double.Parse(s)

module Option =
    let fromSingletonList xs =
        match xs with
        | [x] -> Some x
        | _ -> None

let stockQuoteMapper = (Json.deserialize : _ -> StockQuote)

let historicalQuoteMapper = (Json.deserialize : _ -> HistoricalQuote)

let inline parseResponse json (mapper : Json -> 'T) : 'T list =
    let (plens, _) =
        idLens
        <-?> Json.ObjectPIso
        >??> mapPLens "query"
        <??> Json.ObjectPIso
        >??> mapPLens "results"
        <??> Json.ObjectPIso
    let node = plens (Json.parse json)
    match node with
    | Some values -> 
        let data = values |> Map.find "quote"
        match data with
        | Array quotes ->
            quotes |> List.map mapper
        | Object _ ->
            [Json.deserialize data]
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

let quoteString s = sprintf "\"%s\"" s

let generateYQLQuery xs =
    let addComma s1 s2 = sprintf "%s,%s" s1 s2
    let stocks = xs |> (List.map (quoteString) >> List.reduce (addComma))
    sprintf "select * from yahoo.finance.quote where symbol in (%s)" stocks

/// Fetch a stock quote from Yahoo Finance eg. getStockQuote "GOOGL".
/// Asynchronous version.
let getStockQuoteAsync symbol =
    async {
        let! response = generateYQLQuery [symbol] |> runRequest
        let xs = parseResponse response stockQuoteMapper
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
            return parseResponse response stockQuoteMapper
    }

/// Fetch a stock quote from Yahoo Finance eg. getStockQuote "GOOGL".
let getStockQuote symbol =
    symbol |> getStockQuoteAsync |> Async.RunSynchronously


/// Fetch stock quotes from Yahoo Finance eg. getStockQuotes ["MSFT"; "ORCL"].
let getStockQuotes symbols =
    symbols |> getStockQuotesAsync |> Async.RunSynchronously

/// Historical data

/// Generates a query to retrieve all historical data for 1 or many stocks.
/// Dates for YQL in format YYYY-MM-DD.
let historicalYQLQuery stock startDate endDate =
    let query =
        [ "select * from yahoo.finance.historicaldata where "
        ; "symbol=#{x} and startDate=#{y} and endDate=#{z}"
        ] |> List.reduce (+)
    in
        interpolate query [("x", quoteString stock); ("y", quoteString startDate); ("z", quoteString endDate)]

/// Get historical prices for one or many stocks.
/// Dates should be in the form YYYY-MM-DD.
let historicalPrices stock startDate endDate =
    async {
        let! response = historicalYQLQuery stock startDate endDate |> runRequest
        let xs = parseResponse response historicalQuoteMapper
        return xs
    }

/// Fetch historical price information for a stock.
/// Asynchronous version.
let historicalAsync symbol startDate endDate =
    async {
        return! historicalPrices symbol startDate endDate
    }

/// Fetch historical price information for a stock.
let historical symbol startDate endDate =
    historicalAsync symbol startDate endDate |> Async.RunSynchronously

/// Indexes

let nasdaqIndex() = getStockQuote "^NDX"

let sp500Index() = getStockQuote "^GSPC"

let ftseIndex() = getStockQuote "^FTSE"

let aimIndex() = getStockQuote "^FTAI"
