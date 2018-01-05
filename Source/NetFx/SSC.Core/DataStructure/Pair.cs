using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace SSC.DataStructure
{
    [DataContract]
    public class Pair<dataType1, dataType2> : IComparable<Pair<dataType1, dataType2>>, System.ComponentModel.INotifyPropertyChanged
    {
        public Pair()
        {
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public Pair(dataType1 data1, dataType2 data2)
        {
            Data1 = data1;
            Data2 = data2;
        }
        public Pair(dataType1 data1, dataType2 data2, bool asc)
        {
            Data1 = data1;
            Data2 = data2;
            _Ascending = asc;
        }
        public Pair(dataType1 data1, dataType2 data2, bool asc, bool useData1ForToString)
        {
            Data1 = data1;
            Data2 = data2;
            _Ascending = asc;
            _UseData1ForToString = useData1ForToString;
        }

        private bool _UseData1ForToString = false;
        /// <summary>
        /// UseData1ForToString Property
        /// </summary>
        [DataMember]
        public bool UseData1ForToString
        {
            get { return _UseData1ForToString; }
            set { _UseData1ForToString = value; }
        }

        /// <summary>
        /// True(default): Ascending , False=Descending
        /// </summary>
        [DataMember]
        public bool Ascending
        {
            get
            { return _Ascending; }
            set { _Ascending = value; }
        }
        private bool _Ascending = true;

        [DataMember]
        public dataType1 Data1
        {
            get
            {
                return mData1;
            }
            set
            {
                mData1 = value;
                OnPropertyChanged("Data1");
            }
        }
        private dataType1 mData1;

        [DataMember]
        public dataType2 Data2
        {
            get
            {
                return mData2;
            }
            set
            {
                mData2 = value;
                OnPropertyChanged("Data2");
            }
        }
        private dataType2 mData2;

        private string _Text = null;
        /// <summary>
        /// If not null, will be used for ToString method
        /// </summary>
        [DataMember]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_Text))
            {
                if (_UseData1ForToString)
                    return mData1.ToString();
                else
                    return mData2.ToString();
            }
            else
                return _Text;
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #region IComparable<SortablePair<toSortType,dataType>> Members

        public int CompareTo(Pair<dataType1, dataType2> other)
        {
            int r;
            if (_Ascending)
                r = (this.Data1 as IComparable<dataType1>).CompareTo(other.Data1);
            else
                r = -(this.Data1 as IComparable<dataType1>).CompareTo(other.Data1);

            if (r == 0)
            {
                if (_Ascending)
                    r = (this.Data2 as IComparable<dataType2>).CompareTo(other.Data2);
                else
                    r = -(this.Data2 as IComparable<dataType2>).CompareTo(other.Data2);
            }

            return r;
        }

        #endregion
    }
}
