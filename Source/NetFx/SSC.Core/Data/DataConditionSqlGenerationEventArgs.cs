using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSC.Data
{
    public enum DataConditionSqlGenerationMode
    {
        Auto = 0,
        Custom = 1,
        Ignore = 2,
        OtherConditions = 3,
        AutoWithCustomSqlPlaceHolder = 4,
    }
    public class DataConditionSqlGenerationEventArgs : EventArgs
    {
        public string ConditionUniqueId { get; set; }

        private DataCondition _Condition;
        /// <summary>
        /// Condition Property
        /// </summary>
        public DataCondition Condition
        {
            get { return _Condition; }
            set { _Condition = value; }
        }

        private DataConditionHolder _OtherConditions;
        /// <summary>
        /// OtherConditions Property
        /// </summary>
        public DataConditionHolder OtherConditions
        {
            get { return _OtherConditions; }
            set { _OtherConditions = value; }
        }

        private DataConditionSqlGenerationMode _Mode = DataConditionSqlGenerationMode.Auto;
        /// <summary>
        /// Mode Property
        /// </summary>
        public DataConditionSqlGenerationMode Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        private string _CustomSQL;
        /// <summary>
        /// CustomSQL Property
        /// </summary>
        public string CustomSQL
        {
            get { return _CustomSQL; }
            set { _CustomSQL = value; }
        }

        private List<string> _CustomDeclare = new List<string>();
        /// <summary>
        /// CustomDeclare Property
        /// </summary>
        public List<string> CustomDeclare
        {
            get { return _CustomDeclare; }
            set { _CustomDeclare = value; }
        }

        private Dictionary<string, DataConditionCombinationTypes> _CustomAdditionalConditions = new Dictionary<string, DataConditionCombinationTypes>();
        /// <summary>
        /// CustomAdditionalConditions Property
        /// </summary>
        public Dictionary<string, DataConditionCombinationTypes> CustomAdditionalConditions
        {
            get { return _CustomAdditionalConditions; }
            set { _CustomAdditionalConditions = value; }
        }

    }
}
