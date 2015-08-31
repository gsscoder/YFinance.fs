# YFinance.fs
F# library for querying real time stock market data including indexes such as NASDAQ and the S&P500.
```fsharp
> #load @"../Your/Path/To/YFinance.fs/src/YFinanceFs/YFinanceFs.fsx";;
[Loading YFinanceFs.fsx]

namespace FSI_0004

> open Network.Yahoo.Finance;;
> getStockQuotes ["GOOGL";"YHOO"];;
val it : StockQuote list = [{name = "Google Inc.";
                             symbol = "GOOGL";
                             price = "610.25";
                             change = "-33.78";
                             dayHigh = "611.43";
                             dayLow = "593.09";
                             yearHigh = "713.33";
                             yearLow = "490.91";}; {name = "Yahoo! Inc.";
                                                    symbol = "YHOO";
                                                    price = "29.06";
                                                    change = "-3.87";
                                                    dayHigh = "29.48";
                                                    dayLow = "29.00";
                                                    yearHigh = "52.62";
                                                    yearLow = "29.00";}]
> getStockQuote "MSFT";;
val it : StockQuote option = Some {name = "Microsoft Corporation";
                                   symbol = "MSFT";
                                   price = "41.895";
                                   change = "-1.175";
                                   dayHigh = "41.910";
                                   dayLow = "39.720";
                                   yearHigh = "50.050";
                                   yearLow = "39.720";}
> nasdaqIndex();;
val it : StockQuote option = Some {name = "NASDAQ-100";
                                   symbol = "^NDX";
                                   price = "4003.41";
                                   change = "-193.86";
                                   dayHigh = "4045.11";
                                   dayLow = "3787.23";
                                   yearHigh = "4694.13";
                                   yearLow = "3700.23";}
>
> historical "GOOGL" "2014-01-01" "2015-01-01";;
val it : Network.Yahoo.Data.HistoricalQuote list =
                                   [{symbol = "GOOGL";
                                     date = "2014-12-31";
								     opening = "537.73999";
								     high = "538.400024";
								     low = "530.200012";
								     close = "530.659973";
								     volume = "1232400";
								     adjustedClose = "530.659973";}; ...
```

## Acknowledgements
Port of Haskell library [yfinance](https://github.com/owainlewis/yfinance) by @owainlewis.

## Dependencies
- [Http.fs](https://github.com/relentless/Http.fs) by @relentless.
- [Chiron](https://github.com/xyncro/chiron) by @xyncro.
