module Network.Yahoo.Interpolate

let private toMarker value =
    sprintf "#{%s}" value

let private replace (str : string) (marker : string) (replacement : string) =
    System.Text.RegularExpressions.Regex.Replace (str, marker, replacement)

let internal interpolate input xs =
    xs |> List.fold (fun s (a, b) -> replace s (toMarker a) b) input
