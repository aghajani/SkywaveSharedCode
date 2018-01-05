using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace SSC.Data
{
    [DataContract]
    public enum DataCompareTypes
    {
        [EnumMember]
        Equal = 1,
        [EnumMember]
        GreaterThan = 2,
        [EnumMember]
        LowerThan = 3,
        [EnumMember]
        GreaterOrEqual = 4,
        [EnumMember]
        LowerOrEqual = 5,
        [EnumMember]
        Contains = 6,
        [EnumMember]
        StartsWith = 7,
        [EnumMember]
        EndsWith = 8,
        [EnumMember]
        Among = 9,
        [EnumMember]
        BitwiseAnd = 10,
    }
    [DataContract]
    public enum DataConditionDataTypes
    {
        [EnumMember]
        Text = 1,
        [EnumMember]
        NumberInteger = 2,
        [EnumMember]
        NumberDouble = 3,
        [EnumMember]
        SingleSelection = 4,
        [EnumMember]
        MultiSelection = 5,
        [EnumMember]
        Date = 6,
        [EnumMember]
        Time = 7,
        [EnumMember]
        DateTime = 8,
        [EnumMember]
        Custom = 9,
        [EnumMember]
        Boolean = 10,
    }

    [DataContract]
    public class DataCondition
    {
        private string _Text;
        /// <summary>
        /// Text Property
        /// </summary>
        [DataMember]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        private string _ColumnName;
        /// <summary>
        /// ColumnName Property
        /// </summary>
        [DataMember]
        public string ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }

        private string _Category;
        /// <summary>
        /// Category Property
        /// </summary>
        [DataMember]
        public string Category
        {
            get { return _Category; }
            set { _Category = value; }
        }

        private string _ValueString;
        /// <summary>
        /// ValueString Property
        /// </summary>
        [DataMember]
        public string ValueString
        {
            get { return _ValueString; }
            set { _ValueString = value; }
        }

        private int? _ValueInt;
        /// <summary>
        /// ValueInt Property
        /// </summary>
        [DataMember]
        public int? ValueInt
        {
            get { return _ValueInt; }
            set { _ValueInt = value; }
        }

        private double? _ValueDouble;
        /// <summary>
        /// ValueDouble Property
        /// </summary>
        [DataMember]
        public double? ValueDouble
        {
            get { return _ValueDouble; }
            set { _ValueDouble = value; }
        }

        private DateTime? _ValueDateTime;
        /// <summary>
        /// ValueDateTime Property
        /// </summary>
        [DataMember]
        public DateTime? ValueDateTime
        {
            get { return _ValueDateTime; }
            set { _ValueDateTime = value; }
        }

        private List<int> _ValueListOfInt;
        /// <summary>
        /// ValueListOfInt Property
        /// </summary>
        [DataMember]
        public List<int> ValueListOfInt
        {
            get { return _ValueListOfInt; }
            set { _ValueListOfInt = value; }
        }

        private DataConditionDataTypes _DataType = DataConditionDataTypes.Text;
        /// <summary>
        /// DataType Property
        /// </summary>
        [DataMember]
        public DataConditionDataTypes DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        private DataCompareTypes _CompareType = DataCompareTypes.Equal;
        /// <summary>
        /// CompareType Property
        /// </summary>
        [DataMember]
        public DataCompareTypes CompareType
        {
            get { return _CompareType; }
            set { _CompareType = value; }
        }

        private bool _CaseSensitive;
        /// <summary>
        /// CaseSensitive Property
        /// </summary>
        [DataMember]
        public bool CaseSensitive
        {
            get { return _CaseSensitive; }
            set { _CaseSensitive = value; }
        }

        private bool _IsNot = false;
        /// <summary>
        /// IsNot Property
        /// </summary>
        [DataMember]
        public bool IsNot
        {
            get { return _IsNot; }
            set { _IsNot = value; }
        }

        public object Clone()
        {
            DataCondition r = new DataCondition();
            r.CaseSensitive = CaseSensitive;
            r.ColumnName = ColumnName;
            r.DataType = DataType;
            r.Text = Text;
            r.Category = Category;
            r.CompareType = CompareType;
            return r;
        }
    }
}
