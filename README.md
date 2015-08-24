# YFinance.fs
F# library for querying real time stock market data including indexes such as NASDAQ and the S&P500.
```fsharp
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
```

## Acknowledgements
Port of Haskell library [yfinance](https://github.com/owainlewis/yfinance) by @owainlewis.

## Dependencies
- [Http.fs](https://github.com/relentless/Http.fs) by @relentless.
- [Chiron](https://github.com/xyncro/chiron) by @xyncro.
