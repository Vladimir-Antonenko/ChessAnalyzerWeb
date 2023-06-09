﻿//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
//using System.Net.Http;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ChessAnalyzerApi.Common;

//public class PgnMediaTypeFormatter : MediaTypeFormatter
//{
//    public PgnMediaTypeFormatter() // назначить это свойству HttpMethodContext.ResponseFormatter
//    {
//        SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-chess-pgn")); // нужно будет по аналогии сделать для считывания сразу в pgn класс
//    }

//    public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
//    {
//        return ReadFromStreamAsync(type, readStream, content, formatterLogger, CancellationToken.None);
//    }

//    public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger, CancellationToken cancellationToken)
//    {
//        using (var streamReader = new StreamReader(readStream))
//        {
//            return await streamReader.ReadToEndAsync();
//        }
//    }

//    public override bool CanReadType(Type type)
//    {
//        return type == typeof(string);
//    }

//    public override bool CanWriteType(Type type)
//    {
//        return false;
//    }
//}
