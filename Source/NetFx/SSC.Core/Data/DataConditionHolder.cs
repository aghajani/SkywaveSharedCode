using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using SSC.Linq;

namespace SSC.Data
{
    [DataContract]
    public enum DataConditionCombinationTypes
    {
        [EnumMember]
        AND = 1,
        [EnumMember]
        OR = 2
    }

    [DataContract]
    public class DataConditionHolder
    {
        public event EventHandler<DataConditionSqlGenerationEventArgs> DataConditionSqlGeneration;

        private Dictionary<DataCondition, DataConditionCombinationTypes> _Conditions = new Dictionary<DataCondition, DataConditionCombinationTypes>();
        /// <summary>
        /// Conditions Property
        /// </summary>
        [DataMember]
        public Dictionary<DataCondition, DataConditionCombinationTypes> Conditions
        {
            get { return _Conditions; }
            set { _Conditions = value; }
        }

        private Dictionary<DataConditionHolder, DataConditionCombinationTypes> _ConditionHolders = new Dictionary<DataConditionHolder, DataConditionCombinationTypes>();
        /// <summary>
        /// ConditionHolders Property
        /// </summary>
        [DataMember]
        public Dictionary<DataConditionHolder, DataConditionCombinationTypes> ConditionHolders
        {
            get { return _ConditionHolders; }
            set { _ConditionHolders = value; }
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

        private string _ParametersPrefix;
        /// <summary>
        /// ...
        /// </summary>
        [DataMember]
        public string ParametersPrefix
        {
            get { return _ParametersPrefix; }
            set { _ParametersPrefix = value; }
        }

        public string GenerateSql(string sqlBody)
        { return GenerateSql(sqlBody, ""); }
        public string GenerateSql(string sqlBody, string sqlWherePlaceHolderInBody)
        {
            //string r = "";
            //MA.DataStructure.Pair<string, List<string>> r_Internal = GenerateSqlWhere_Internal(1);
            ////
            //string sqlParams = "";
            //foreach (string fe1 in r_Internal.Data2)
            //{
            //    sqlParams += string.Format(",{0}\n\t", fe1);
            //}
            //sqlParams = sqlParams.Trim(new char[] { ',', '\t' });
            ////
            //if (!string.IsNullOrEmpty(sqlParams))
            //{
            //    r += string.Format("DECLARE \n\t{0};\n", sqlParams);
            //}
            ////
            //r += "\n" + sqlBody + "\n";
            ////
            //if (!string.IsNullOrEmpty(r_Internal.Data1))
            //{
            //    if (string.IsNullOrEmpty(sqlWherePlaceHolderInBody))
            //        r += string.Format("WHERE\n{0}", r_Internal.Data1);
            //    else
            //        r = r.Replace(sqlWherePlaceHolderInBody, string.Format("WHERE\n{0}", r_Internal.Data1));
            //}
            //else if (!string.IsNullOrEmpty(sqlWherePlaceHolderInBody))
            //    r = r.Replace(sqlWherePlaceHolderInBody, "");
            ////
            //return r;
            return GenerateSqlCommandString(sqlBody, sqlWherePlaceHolderInBody).GenerateSql();
        }
        public SqlCommandString GenerateSqlCommandString(string sqlBody, string sqlWherePlaceHolderInBody)
        {
            SqlCommandString r = new SqlCommandString();
            //
            SSC.DataStructure.Pair<string, List<string>> r_Internal = GenerateSqlWhere_Internal(1, 1);
            //
            r.Params = r_Internal.Data2;
            //
            r.CommandString = sqlBody + "\n";
            //
            if (!string.IsNullOrEmpty(r_Internal.Data1))
            {
                if (string.IsNullOrEmpty(sqlWherePlaceHolderInBody))
                    r.CommandString += string.Format("WHERE\n{0}", r_Internal.Data1);
                else
                    r.CommandString = r.CommandString.Replace(sqlWherePlaceHolderInBody, string.Format("WHERE\n{0}", r_Internal.Data1));
            }
            else if (!string.IsNullOrEmpty(sqlWherePlaceHolderInBody))
                r.CommandString = r.CommandString.Replace(sqlWherePlaceHolderInBody, "");
            //
            return r;
        }

        internal SSC.DataStructure.Pair<string, List<string>> GenerateSqlWhere_Internal(int level, int iLevelIndex)
        {
            SSC.DataStructure.Pair<string, List<string>> r = new DataStructure.Pair<string, List<string>>();
            List<string> declareLines = new List<string>();
            string sql = "";
            int iCondition = 1;
            Queue<DataStructure.Pair<DataCondition, DataConditionCombinationTypes>> toProcessConditions = new Queue<DataStructure.Pair<DataCondition, DataConditionCombinationTypes>>(from x1 in _Conditions
                                                                                                                                                                                      select new DataStructure.Pair<DataCondition, DataConditionCombinationTypes>(x1.Key, x1.Value));
            while (toProcessConditions.Count > 0)
            {
                DataStructure.Pair<DataCondition, DataConditionCombinationTypes> item1 = toProcessConditions.Dequeue();
                //
                DataConditionSqlGenerationEventArgs e = new DataConditionSqlGenerationEventArgs();
                e.ConditionUniqueId = string.Format("{0}_{1}_{2}", level, iLevelIndex, iCondition);
                e.Condition = item1.Data1;
                if (DataConditionSqlGeneration != null)
                    DataConditionSqlGeneration(this, e);
                //
                if (e.Mode == DataConditionSqlGenerationMode.Ignore)
                    continue;
                //
                if (e.Mode == DataConditionSqlGenerationMode.OtherConditions)
                {
                    _ConditionHolders.Add(e.OtherConditions, DataConditionCombinationTypes.AND);
                    continue;
                }
                //
                if (!string.IsNullOrEmpty(sql))
                {
                    if (item1.Data2 == DataConditionCombinationTypes.AND)
                        sql += "\nAND\n";
                    else
                        sql += "\nOR\n";
                }
                //
                sql += "(";
                //
                string sql_Condition = "";
                if (item1.Data1.IsNot)
                    sql_Condition = "NOT (";
                else
                    sql_Condition = "(";
                //
                if (e.Mode == DataConditionSqlGenerationMode.Auto || e.Mode == DataConditionSqlGenerationMode.AutoWithCustomSqlPlaceHolder)
                {
                    string paramName = string.Format("@{3}p_{0}_{1}_{2}", level, iLevelIndex, iCondition, _ParametersPrefix);
                    string paramType = "";
                    string paramValue = "";
                    bool paramHasValue = false;
                    switch (e.Condition.DataType)
                    {
                        case DataConditionDataTypes.Text:
                            paramHasValue = true;
                            paramValue = e.Condition.ValueString;
                            paramType = "nvarchar(1000)";
                            break;
                        case DataConditionDataTypes.NumberInteger:
                            paramHasValue = true;
                            paramType = "int";
                            paramValue = (e.Condition.ValueInt == null) ? "NULL" : e.Condition.ValueInt.ToString();
                            break;
                        case DataConditionDataTypes.NumberDouble:
                            paramHasValue = true;
                            paramType = "float";
                            paramValue = (e.Condition.ValueDouble == null) ? "NULL" : e.Condition.ValueDouble.ToString();
                            break;
                        case DataConditionDataTypes.SingleSelection:
                            paramHasValue = true;
                            paramType = "int";
                            paramValue = (e.Condition.ValueInt == null) ? "NULL" : e.Condition.ValueInt.ToString();
                            break;
                        case DataConditionDataTypes.MultiSelection:
                            paramHasValue = (e.Condition.ValueListOfInt != null && e.Condition.ValueListOfInt.Count > 0);
                            paramValue = (paramHasValue) ? "(" + e.Condition.ValueListOfInt.ToSeperated() + ")" : "";
                            break;
                        case DataConditionDataTypes.Date:
                        case DataConditionDataTypes.Time:
                        case DataConditionDataTypes.DateTime:
                            paramHasValue = true;
                            paramType = "datetime";
                            paramValue = (e.Condition.ValueDateTime == null) ? "NULL" : string.Format("'{0}'", e.Condition.ValueDateTime.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                            break;
                        case DataConditionDataTypes.Boolean:
                            paramHasValue = true;
                            paramType = "bit";
                            paramValue = (e.Condition.ValueInt == null) ? "NULL" : e.Condition.ValueInt.ToString();
                            break;
                        case DataConditionDataTypes.Custom:
                            break;
                        default:
                            break;
                    }
                    //
                    if (paramHasValue)
                    {
                        string sql_AfterTarget = "";
                        string sql_BeforeTarget = "";
                        //
                        switch (e.Condition.CompareType)
                        {
                            case DataCompareTypes.Equal:
                                if (e.Condition.DataType == DataConditionDataTypes.Text)
                                    paramValue = string.Format("N'{0}'", paramValue);
                                if (paramValue == "NULL")
                                    sql_AfterTarget += " IS NULL";
                                else
                                    sql_AfterTarget += " = " + paramName;
                                break;
                            case DataCompareTypes.GreaterThan:
                                sql_AfterTarget += " > " + paramName;
                                break;
                            case DataCompareTypes.LowerThan:
                                sql_AfterTarget += " < " + paramName;
                                break;
                            case DataCompareTypes.GreaterOrEqual:
                                sql_AfterTarget += " >= " + paramName;
                                break;
                            case DataCompareTypes.LowerOrEqual:
                                sql_AfterTarget += " <= " + paramName;
                                break;
                            case DataCompareTypes.Contains:
                                if (e.Condition.DataType == DataConditionDataTypes.Text)
                                {
                                    paramValue = string.Format("N'%{0}%'", paramValue);
                                    sql_AfterTarget += " LIKE " + paramName;
                                }
                                else if (e.Condition.DataType == DataConditionDataTypes.MultiSelection)
                                {
                                    sql_AfterTarget += " IN " + paramValue;
                                }
                                break;
                            case DataCompareTypes.StartsWith:
                                if (e.Condition.DataType == DataConditionDataTypes.Text)
                                {
                                    paramValue = string.Format("N'{0}%'", paramValue);
                                    sql_AfterTarget += " LIKE " + paramName;
                                }
                                break;
                            case DataCompareTypes.EndsWith:
                                if (e.Condition.DataType == DataConditionDataTypes.Text)
                                {
                                    paramValue = string.Format("N'%{0}'", paramValue);
                                    sql_AfterTarget += " LIKE " + paramName;
                                }
                                break;
                            case DataCompareTypes.Among:
                                if (e.Condition.DataType == DataConditionDataTypes.MultiSelection)
                                {
                                    sql_AfterTarget += " IN " + paramValue;
                                }
                                break;
                            case DataCompareTypes.BitwiseAnd:
                                if (e.Condition.DataType == DataConditionDataTypes.NumberInteger)
                                {
                                    sql_BeforeTarget = "(";
                                    sql_AfterTarget += string.Format(" & {0}) = {0}", paramName);
                                }
                                break;
                            default:
                                break;
                        }
                        if (!string.IsNullOrEmpty(paramType))
                        {
                            declareLines.Add(string.Format("{0} {1}={2}", paramName, paramType, paramValue));
                        }
                        if (!string.IsNullOrEmpty(sql_AfterTarget))
                        {
                            string sql_Target;
                            if (e.Mode == DataConditionSqlGenerationMode.Auto)
                                sql_Target = e.Condition.ColumnName;
                            else
                                sql_Target = e.CustomSQL;
                            //
                            sql_Condition += sql_BeforeTarget + sql_Target + sql_AfterTarget + ")";
                            if (paramValue != "NULL" && item1.Data1.IsNot)
                                sql_Condition = string.Format("(({0}) OR ({1} IS NULL))", sql_Condition, sql_Target);
                            else
                                sql_Condition = string.Format("({0})", sql_Condition);
                            //
                            sql += sql_Condition;
                        }
                    }
                }
                else if (e.Mode == DataConditionSqlGenerationMode.Custom)
                {
                    sql_Condition += e.CustomSQL + ")";
                    sql_Condition = string.Format("({0})", sql_Condition);
                    sql += sql_Condition;
                }
                //
                foreach (string fe1 in e.CustomDeclare)
                {
                    if (!declareLines.Contains(fe1))
                        declareLines.Add(fe1);
                }
                //
                foreach (var feCustomAdditionalCondition in e.CustomAdditionalConditions)
                {
                    if (feCustomAdditionalCondition.Value == DataConditionCombinationTypes.AND)
                        sql += "\nAND\n";
                    else
                        sql += "\nOR\n";
                    sql += feCustomAdditionalCondition.Key;
                }
                //
                sql += ")";
                //
                iCondition++;
            }
            //
            int iChildLevelIndex = 1;
            foreach (DataConditionHolder feDCH in _ConditionHolders.Keys)
            {
                EventHandler<DataConditionSqlGenerationEventArgs> feDCH_Event = new EventHandler<DataConditionSqlGenerationEventArgs>(feDCH_DataConditionSqlGeneration);
                feDCH.DataConditionSqlGeneration += feDCH_Event;
                if (!string.IsNullOrEmpty(sql))
                {
                    if (_ConditionHolders[feDCH] == DataConditionCombinationTypes.AND)
                        sql += "\nAND\n";
                    else
                        sql += "\nOR\n";
                }
                //
                if (feDCH.IsNot)
                    sql += "NOT (";
                else
                    sql += "(";
                //
                SSC.DataStructure.Pair<string, List<string>> feDCH_R = feDCH.GenerateSqlWhere_Internal(level + 1, iChildLevelIndex);
                sql += feDCH_R.Data1;
                declareLines.AddRange(from x1 in feDCH_R.Data2
                                      where !declareLines.Contains(x1)
                                      select x1);
                sql += ")";
                feDCH.DataConditionSqlGeneration -= feDCH_Event;
                //
                iChildLevelIndex++;
            }
            //
            r.Data1 = sql;
            r.Data2 = declareLines;
            //
            return r;
        }

        void feDCH_DataConditionSqlGeneration(object sender, DataConditionSqlGenerationEventArgs e)
        {
            if (DataConditionSqlGeneration != null)
                DataConditionSqlGeneration(this, e);
        }

        public List<DataCondition> GetContainingConditions()
        {
            List<DataCondition> r = new List<DataCondition>(Conditions.Keys);
            foreach (DataConditionHolder fe1 in ConditionHolders.Keys)
            {
                r.AddRange(fe1.GetContainingConditions());
            }
            return r;
        }
    }
}
