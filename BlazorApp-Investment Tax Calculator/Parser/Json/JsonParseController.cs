﻿using Model;

using System.Text.Json;

namespace Parser.InteractiveBrokersXml;

public class JsonParseController(AssetTypeToLoadSetting assetTypeToLoadSetting) : ITaxEventFileParser
{
    public TaxEventLists ParseFile(string data)
    {
        TaxEventLists? result = null;
        try
        {
            result = JsonSerializer.Deserialize<TaxEventLists>(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        TaxEventLists resultFiltered = new();
        if (result == null) return resultFiltered;
        if (assetTypeToLoadSetting.LoadDividends) resultFiltered.Dividends.AddRange(result.Dividends);
        if (assetTypeToLoadSetting.LoadStocks) resultFiltered.CorporateActions.AddRange(result.CorporateActions);
        if (assetTypeToLoadSetting.LoadStocks) resultFiltered.Trades.AddRange(result.Trades.Where(trade => trade.AssetType == Enumerations.AssetCatagoryType.STOCK));
        if (assetTypeToLoadSetting.LoadFutures) resultFiltered.Trades.AddRange(result.Trades.Where(trade => trade.AssetType == Enumerations.AssetCatagoryType.FUTURE));
        if (assetTypeToLoadSetting.LoadFx) resultFiltered.Trades.AddRange(result.Trades.Where(trade => trade.AssetType == Enumerations.AssetCatagoryType.FX));
        return resultFiltered;
    }

    public bool CheckFileValidity(string data, string contentType)
    {
        return contentType == "application/json";
    }
}
