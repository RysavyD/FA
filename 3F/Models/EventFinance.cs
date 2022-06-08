using System;
using System.Collections.Generic;
using System.Linq;
using _3F.Model.Accounting;
using _3F.Model.Extensions;
using _3F.Model.Model;

namespace _3F.Web.Models.Events
{
    public class EventFinanceViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public decimal FirstCost { get; set; }
        public string EventCostsDescription { get; set; }
        public string HtmlName { get; internal set; }
        public List<PaymentViewModel> Payments { get; set; }
        public decimal PaymentPaidSum
        {
            get
            {
                return (Payments == null)
                    ? 0m
                    : Payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount);
            }
        }
        public decimal PaymentActiveSum
        {
            get
            {
                return (Payments == null)
                    ? 0m
                    : Payments.Where(p => p.Status == PaymentStatus.Active).Sum(p => p.Amount);
            }
        }

        public string[][] PaymentItems
        {
            get
            {
                return (Payments == null) ? new string[][] { } :
                    Payments.Select(p => new[]
                    {
                        p.UserName,
                        p.CreateDate.ToString(),
                        p.Amount.ToString(),
                        p.PaidDate.ToString(),
                        p.Status.GetDescription(),
                        p.Description,
                    })
                    .ToArray();
            }
        }

        public string[][] CostsItems
        {
            get
            {
                return (Costs == null) ? new string[][] { } :
                    Costs.Select(p => new[]
                    {
                        p.Amount.ToString(),
                        p.Description,
                    })
                    .ToArray();
            }
        }

        public List<Cost> Costs { get; set; }
        public decimal CostsSum
        {
            get
            {
                return (Costs == null) ? 0m : Costs.Sum(p => p.Amount);
            }
        }
    }

    public class PaymentViewModel
    {
        public string UserName { get; set; }
        public string UserHtmlName { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Description { get; set; }
        public PaymentStatus Status { get; set; }
    }
}