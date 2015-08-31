module Network.Yahoo.Finance

open System
open Chiron
open Chiron.Operators
open Aether
open Aether.Operators
open HttpClient
open Network.Yahoo.Data
open Network.Yahoo.Interpolate

module Option =
    let fromSingletonList xs =
        match xs with
        | [x] -> Some x
        | _ -> None

let jsonLens =
    idLens
    <-?> Json.ObjectPIso
    >??> mapPLens "query"
    <??> Json.ObjectPIso
    >??> mapPLens "results"
    <??> Json.ObjectPIso

let parseResponseToStockQuote json =
    let (plens, _) = jsonLens
    let node = plens (Json.parse json)
    match node with
    | Some values -> 
        let data = values |> Map.find "quote"
        match data with
        | Array quotes ->
            quotes |> List.map (Json.deserialize : _ -> StockQuote)
        | Object _ ->
            [Json.deserialize data]
        | _ -> []
    | _ -> []

let parseResponseToHistoricalQuote json =
    let (plens, _) = jsonLens
    let node = plens (Json.parse json)
    match node with
    | Some values -> 
        let data = values |> Map.find "quote"
        match data with
        | Array quotes ->
            quotes |> List.map (Json.deserialize : _ -> HistoricalQuote)
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
        let xs = parseResponseToStockQuote response
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
            return parseResponseToStockQuote response
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
        let! response = generateYQLQuery [stock] |> runRequest
        let xs = parseResponse response mapHistoricalQuote
        return xs |> Option.fromSingletonList
    }

/// Indexes

let nasdaqIndex() = getStockQuote "^NDX"

let sp500Index() = getStockQuote "^GSPC"

let ftseIndex() = getStockQuote "^FTSE"

let aimIndex() = getStockQuote "^FTAI"
