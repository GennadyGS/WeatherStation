module WeatherStationUpload.TryParser

open System.Globalization

// convenient, functional TryParse wrappers returning option<'a>
let tryParseWith tryParseFunc = 
    tryParseFunc >> function 
    | true, v -> Some v
    | false, _ -> None
    
let parseDate = tryParseWith System.DateTime.TryParse
let parseInt = tryParseWith System.Int32.TryParse
let parseSingle = tryParseWith System.Single.TryParse
let parseDouble = tryParseWith System.Double.TryParse
let parseByte = tryParseWith System.Byte.TryParse

let parseDateInvariant style = 
    tryParseWith (fun str -> System.DateTime.TryParse(str, CultureInfo.InvariantCulture, style))
let parseIntInvariant style = 
    tryParseWith (fun str -> System.Int32.TryParse(str, style, CultureInfo.InvariantCulture))
let parseSingleInvariant style = 
    tryParseWith (fun str -> System.Single.TryParse(str, style, CultureInfo.InvariantCulture)) 
let parseDoubleInvariant style = 
    tryParseWith (fun str -> System.Double.TryParse(str, style, CultureInfo.InvariantCulture)) 
let parseDecimalInvariant style = 
    tryParseWith (fun str -> System.Decimal.TryParse(str, style, CultureInfo.InvariantCulture))
let parseByteInvariant style = 
    tryParseWith (fun str -> System.Byte.TryParse(str, style, CultureInfo.InvariantCulture))

// active patterns for try-parsing strings
let (|Date|_|) = parseDate
let (|Int|_|) = parseInt
let (|Single|_|) = parseSingle
let (|Double|_|) = parseDouble
let (|Decimal|_|) = parseDouble
let (|Byte|_|) = parseByte

// active patterns for try-parsing strings
let (|DateInvariant|_|) = parseDateInvariant
let (|IntInvariant|_|) = parseIntInvariant
let (|SingleInvariant|_|) = parseSingleInvariant
let (|DoubleInvariant|_|) = parseDoubleInvariant
let (|DecimalInvariant|_|) = parseDoubleInvariant
let (|ByteInvariant|_|) = parseByteInvariant
