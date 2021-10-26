using System;
using System.Collections.Generic;

namespace Util.Csv.Data
{
    public class Storage<T> where T : IData
    {
        private static Storage<T> _instance;
        public static Storage<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Storage<T>();
                }
                return _instance;
            }
        }

        private SortedList<int, T> datas;

        public void ChangeDatas(SortedList<int, T> datas)
        {
            if (this.datas == null)
            {
                this.datas = new SortedList<int, T>();
            }

            this.datas = datas;
        }

        public T GetDataByInfoID(int infoID)
        {
            if (this.datas == null)
            {
                ReadAndStoreDatas();
            }

            if (this.datas.ContainsKey(infoID) == false)
                return default(T);

            return this.datas[infoID];
        }

        public T GetDataByIndex(int index)
        {
            if (this.datas == null)
            {
                ReadAndStoreDatas();
            }

            if (this.datas.Count <= index)
                return default(T);

            return this.datas.Values[index];
        }

        private void ReadAndStoreDatas()
        {
            string className = "Util.Csv.Data.Class." + typeof(T).Name;            
            Type[] typeArgs = { Type.GetType(className)};
            Type genericType = typeof(Reader<>);
            Type readerType = genericType.MakeGenericType(typeArgs);
            Reader<T> reader = (Reader<T>)Activator.CreateInstance(readerType);

            reader.Read();
            reader.StoreDatasInStorage();
        }
    }
}
