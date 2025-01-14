﻿using InvestmentTaxCalculator.Model;

using Microsoft.AspNetCore.Components.Forms;

namespace InvestmentTaxCalculator.Parser;

public class FileParseController(IEnumerable<ITaxEventFileParser> taxEventFileParsers)
{
    private const long _maxFileSize = 1024 * 1024 * 1000; // 1000 MB

    public async Task<TaxEventLists> ReadFile(IBrowserFile file)
    {
        TaxEventLists taxEvents = new();
        using (StreamReader sr = new(file.OpenReadStream(_maxFileSize)))
        {
            string data = await sr.ReadToEndAsync();
            string contentType = file.ContentType;
            var parser = taxEventFileParsers.FirstOrDefault(parser => parser.CheckFileValidity(data, contentType));
            if (parser != null)
            {
                taxEvents.AddData(parser.ParseFile(data));
            }
        }
        return taxEvents;
    }
}
