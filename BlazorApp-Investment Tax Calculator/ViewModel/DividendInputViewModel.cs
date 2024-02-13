﻿using Enumerations;

using Model;
using Model.TaxEvents;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace ViewModel;

public class DividendInputViewModel
{
    private static int _nextId = 0;
    public int Id { get; init; }
    [Required]
    public required string AssetName { get; set; }
    [Required]
    public required DateTime Date { get; set; }
    [Required]
    [CustomValidationCompanyLocationString]
    public required string CompanyLocationString { get; set; }
    [Required]
    [CustomValidationCurrencyString]
    public required string CurrencyString { get; set; }
    [Required]
    public decimal GrossAmount { get; set; } = 0;
    [Required]
    public decimal GrossPaymentInLieuAmount { get; set; } = 0;
    [Required]
    public decimal WithHoldingAmount { get; set; } = 0;
    [Required]
    public decimal FxRate { get; set; } = 1;
    public string Description { get; set; } = "";

    public DividendInputViewModel()
    {
        Id = Interlocked.Increment(ref _nextId);
    }

    public List<Dividend> Convert()
    {
        List<Dividend> result = [];
        if (GrossAmount != 0)
        {
            result.Add(Convert(DividendType.DIVIDEND, GrossAmount));
        }
        if (GrossPaymentInLieuAmount != 0)
        {
            result.Add(Convert(DividendType.DIVIDEND_IN_LIEU, GrossPaymentInLieuAmount));
        }
        if (WithHoldingAmount != 0)
        {
            result.Add(Convert(DividendType.WITHHOLDING, WithHoldingAmount));
        }
        return result;
    }

    private Dividend Convert(DividendType dividendType, decimal amount)
    {
        RegionInfo companyLocation = new(CompanyLocationString);
        WrappedMoney money = new(amount, CurrencyString);
        DescribedMoney describedMoney = new() { Amount = money, Description = Description, FxRate = FxRate };
        return new Dividend()
        {
            AssetName = AssetName,
            Date = Date,
            DividendType = dividendType,
            Proceed = describedMoney,
            CompanyLocation = companyLocation
        };
    }
}
