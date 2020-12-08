using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Litium.Synchronization.Abstraction.Customers
{
    [Serializable]
    public class Field
    {
        public bool BooleanValue { get; set; }
        public int ChildIndex { get; set; }
        public string ChildOwnerId { get; set; }
        public string Culture { get; set; }
        public DateTime DateTimeValue { get; set; }
        public List<decimal> DecimalOptionValue { get; set; }
        public decimal DecimalValue { get; set; }
        public EntityType EntityType { get; set; }
        public Guid FieldDefinitionSystemId { get; set; }
        public FieldType FieldType { get; set; }
        public Guid GuidValue { get; set; }
        public int Index { get; set; }
        public bool IsArray { get; set; }
        public bool IsMultiSelect { get; set; }
        public string IndexedTextValue { get; set; }
        public List<int> IntOptionValue { get; set; }
        public int IntValue { get; set; }
        public string JsonValue { get; set; }
        public long LongValue { get; set; }
        public List<string> TextOptionValue { get; set; }
        public Guid OwnerSystemId { get; set; }
        public string TextValue { get; set; }
    }
    public enum FieldType
    {
        Boolean,
        DateTime,
        Decimal,
        DecimalOption,
        Editor,
        Int,
        IntOption,
        LimitedText,
        Long,
        MediaPointerFile,
        MediaPointerImageArray,
        MediaPointerImage,
        MultiFieldOption,
        MultirowText,
        Object,
        Pointer,
        TextField,
        TextOption
    }
    public enum EntityType
    {
        None,
        CustomersGroup,
        CustomersPerson,
        CustomersOrganization,
        CustomersTargetGroup,
        MediaFile,
        MediaImage,
        MediaVideo,
        ProductsAssortment,
        ProductsCategory,
        ProductsProduct,
        ProductsProductList,
        WebsitesPage,
        WebsitesWebsite,
        GlobalizationCountry
    }
}
