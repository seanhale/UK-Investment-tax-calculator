﻿using Enum;
using NMoneys;

namespace Model;

public record Trade : TaxEvent
{
    public virtual required TradeType BuySell { get; set; }
    public virtual required decimal Quantity { get; set; }
    public virtual required DescribedMoney GrossProceed { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<DescribedMoney> Expenses { get; set; } = new List<DescribedMoney>();
    public virtual Money NetProceed
    {
        get
        {
            if (!Expenses.Any()) return GrossProceed.BaseCurrencyAmount;
            if (BuySell == TradeType.BUY) return GrossProceed.BaseCurrencyAmount + Expenses.Select(i => i.BaseCurrencyAmount).Sum();
            else return GrossProceed.BaseCurrencyAmount - Expenses.Select(i => i.BaseCurrencyAmount).Sum();
        }
    }
}

