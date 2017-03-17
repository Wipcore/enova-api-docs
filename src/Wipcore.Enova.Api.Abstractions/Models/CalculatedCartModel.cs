using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces.Cart;

namespace Wipcore.Enova.Api.Abstractions.Models
{
    /// <summary>
    /// Model for a cart/order that has had it's prices calculated.
    /// </summary>
    public class CalculatedCartModel : CartModel, ICalculatedCartModel
    {
        public CalculatedCartModel(IEnumerable<CalculatedCartRowModel> rows) : base(rows) { }//binds to concerete type for http model binding
        
        public decimal TotalPriceInclTax { get; set; }
       
        public decimal TotalPriceExclTax { get; set; }

        public string TotalPriceInclTaxString { get; set; }
        public string TotalPriceExclTaxString { get; set; }

        public override string ToString()
        {
            var rows = Rows == null ? String.Empty : $"[{String.Join("| ", Rows.Select(x => x.ToString()))}]";
            var addVal = AdditionalValues == null ? String.Empty : $"[{String.Join(", ", AdditionalValues.Select(x => $"{x.Key}:{x.Value}"))}]";

            return $"CartModel: (Identifier: {Identifier}, Customer: {Customer}, Persist: {Persist}, TotalPriceExcl: {TotalPriceExclTax}, " +
                   $"TotalPriceInclTax: {TotalPriceInclTax}, Status: {Status}, Rows: {rows}, AdditionalValues: {addVal})";
        }

        /// <summary>
        /// Copy values from given model to the derived calculated model.
        /// </summary>
        public static CalculatedCartModel CreateFrom(ICartModel model)
        {
            var calculatedRows = new List<CalculatedCartRowModel>();
            var calcCart = new CalculatedCartModel(calculatedRows)
            {
                Identifier = model.Identifier,
                Customer = model.Customer,
                Persist = model.Persist,
                Status = model.Status,
                AdditionalValues = model.AdditionalValues,
                Rows = calculatedRows
            };

            calculatedRows.AddRange(model.Rows.Select(row => new CalculatedCartRowModel()
            {
                Identifier = row.Identifier,
                Type = row.Type,
                Quantity = row.Quantity,
                Password = row.Password,
                AdditionalValues = row.AdditionalValues
            }));

            return calcCart;
        }
    }
}
