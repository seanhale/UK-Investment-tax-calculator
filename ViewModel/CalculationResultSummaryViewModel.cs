﻿using CapitalGainCalculator.Model;
using CapitalGainCalculator.Model.Interfaces;
using CapitalGainCalculator.ViewModel.Messages;
using CapitalGainCalculator.ViewModel.Options;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapitalGainCalculator.ViewModel;

public partial class CalculationResultSummaryViewModel : ObservableRecipient, IRecipient<CalculationFinishedMessage>
{
    private readonly TradeCalculationResult _tradeCalculationResult;
    private readonly DividendCalculationResult _dividendCalculationResult;
    [ObservableProperty]
    private int _numberOfDisposals;
    [ObservableProperty]
    private decimal _disposalProceeds;
    [ObservableProperty]
    private decimal _allowableCosts;
    [ObservableProperty]
    private decimal _totalGain;
    [ObservableProperty]
    private decimal _totalLoss;
    public YearOptions Years { get; init; }
    private readonly ITaxYear _taxYear;


    public CalculationResultSummaryViewModel(TradeCalculationResult tradeCalculationResult, DividendCalculationResult dividendCalculationResult, YearOptions years, ITaxYear taxYear)
    {
        _tradeCalculationResult = tradeCalculationResult;
        _dividendCalculationResult = dividendCalculationResult;
        Years = years;
        Years.PropertyChanged += Years_PropertyChanged;
        _taxYear = taxYear;
        IsActive = true;
    }

    public void Receive(CalculationFinishedMessage message)
        {
        Years.SetYears(GetSelectableYears());
        Years.SelectAllYears();
            UpdateSummary();
        }
    }

    public void Receive(YearSelectionChangedMessage message)
    {
        UpdateSummary();
        Years.SetYears(GetSelectableYears());
    }

    private void UpdateSummary()
    {
        if (_tradeCalculationResult.CalculatedTrade is null || Years is null) return;
        NumberOfDisposals = _tradeCalculationResult.NumberOfDisposals(Years.GetSelectedYears());
        DisposalProceeds = _tradeCalculationResult.DisposalProceeds(Years.GetSelectedYears());
        AllowableCosts = _tradeCalculationResult.AllowableCosts(Years.GetSelectedYears());
        TotalGain = _tradeCalculationResult.TotalGain(Years.GetSelectedYears());
        TotalLoss = _tradeCalculationResult.TotalLoss(Years.GetSelectedYears());
        TotalDividends = _dividendCalculationResult.GetTotalDividend(Years.GetSelectedYears());
        TotalForeignTaxPaid = _dividendCalculationResult.GetForeignTaxPaid(Years.GetSelectedYears());

    }

    private IEnumerable<int> GetSelectableYears()
    {
        IEnumerable<int> yearsWithDisposal = _tradeCalculationResult.CalculatedTrade.Where(trade => trade.BuySell == Enum.TradeType.SELL)
                                                 .Select(trade => trade.Date.Year)
                                                 .Distinct();
        IEnumerable<int> yearsWithDividend = _dividendCalculationResult.DividendSummary.Select(dividend => dividend.TaxYear).Distinct();
        return yearsWithDisposal.Union(yearsWithDividend).OrderByDescending(i => i);
    }
}
