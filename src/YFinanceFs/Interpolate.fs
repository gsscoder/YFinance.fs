module Network.Yahoo.Interpolate

let toMarker value =
    sprintf "#{%s}" value

let replace (str : string) (marker : string) (replacement : string) =
    System.Text.RegularExpressions.Regex.Replace (str, marker, replacement)

