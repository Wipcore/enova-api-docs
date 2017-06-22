using System;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Product
{
    public class StockInformationCompartmentModel
    {
        [PropertyPresentation("String", "Warehouse name", languageDependant: true, isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 300)]
        public string WarehouseName { get; set; }
        [PropertyPresentation("String", "Compartment name", languageDependant: true, isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 300)]
        public string CompartmentName { get; set; }
        [PropertyPresentation("NumberString", "Stock Quantity", isEditable: true, isFilterable: true, isGridColumn: false, sortOrder: 300)]
        public double Quantity { get; set; }
        public double ReservedQuantity { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
    }
    public class StockInformationHistoryModel
    {
        public DateTime Timestamp { get; set; }
        public double Stock { get; set; }
        public string User { get; set; }
        public string Comment { get; set; }
        public string CompartmentName { get; set; }
    }
}
