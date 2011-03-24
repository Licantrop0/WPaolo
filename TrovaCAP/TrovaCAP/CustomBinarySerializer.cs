using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;

namespace TrovaCAP
{
    // classe di prova:
    /*public class SampleData
    {
        [DataMember]
        public string ContentText { get; set; }

        [DataMember]
        public List<int> SomeItems { get; set; }

        public SampleData()
        {
            ContentText = "some text";
            SomeItems = new List<int>() { 1, 2, 3 };
        }
    }*/

    public class CustomBinarySerializer
    {
        private List<PropertyInfo> serializableProperties = new List<PropertyInfo>();
        private Type serializableObjectType;

        public CustomBinarySerializer(Type objectType)
        {
            serializableObjectType = objectType;
            serializableProperties = GetMarkedProperties(objectType);
        }

        private List<PropertyInfo> GetMarkedProperties(Type type)
        {
            return (from property in type.GetProperties()
                    where property.GetCustomAttributes(true)
                    .Where((x) => x is System.Runtime.Serialization.DataMemberAttribute).Count() > 0
                    select property
                    ).ToList();
        }

        #region Write

        /*public void WriteObject(Stream stream, object graph)
        {
            if (stream == null || graph == null)
                return;

            BinaryWriter bw = new BinaryWriter(stream);

            foreach (PropertyInfo pi in serializableProperties)
            {
                var value = pi.GetValue(graph, null);

                if (pi.PropertyType == typeof(string))
                {
                    bw.Write(value as string ?? string.Empty);
                }
                else if (pi.PropertyType == typeof(List<int>))
                {
                    WriteIntegerList(bw, value as List<int>);
                }
            }
        }*/

        public void WriteObject(Stream stream, object graph)
        {
            if (stream == null || graph == null)
                return;

            BinaryWriter bw = new BinaryWriter(stream);

            foreach (PropertyInfo pi in serializableProperties)
            {
                /*var value = pi.GetValue(graph, graph[]);

                if (pi.PropertyType == typeof(Comune[]))
                {
                    WriteComuneArray(bw, value as Comune[]);
                }*/
            }

            WriteComuneArray(bw, graph as Comune[]);
        }


        private void WriteComuneArray(BinaryWriter bw, Comune[] comuni)
        {
            if (comuni == null || !comuni.Any())
            {
                bw.Write(0);
            }
            else
            {
                bw.Write(comuni.Length);
                foreach (var c in comuni)
                    WriteComune(bw, c);
            }
        }

        private void WriteComune(BinaryWriter bw, Comune comune)
        {
            bw.Write(comune.ComuneID as string ?? string.Empty);

            WriteCAPRecordsArray(bw, comune.CapRecords);
        }

        private void WriteCAPRecordsArray(BinaryWriter bw, CAPRecord[] capRecords)
        {
            if (capRecords == null || !capRecords.Any())
            {
                bw.Write(0);
            }
            else
            {
                bw.Write(capRecords.Length);
                foreach (var c in capRecords)
                    WriteCAPRecord(bw, c);
            }
        }

        private void WriteCAPRecord(BinaryWriter bw, CAPRecord capRecord)
        {
            bw.Write(capRecord.Frazione as string ?? string.Empty);
            bw.Write(capRecord.Indirizzo as string ?? string.Empty);
            bw.Write(capRecord.Cap as string ?? string.Empty);
        }

        private void WriteIntegerList(BinaryWriter bw, List<int> list)
        {
            if (list == null || !list.Any())
            {
                bw.Write(0);
            }
            else
            {
                bw.Write(list.Count);
                list.ForEach(x => bw.Write(x));
            }
        }

        #endregion Write

        #region Read

        /*public object ReadObject(Stream stream)
        {
            if (stream == null)
                return null;

            BinaryReader br = new BinaryReader(stream);

            object deserializedObject = Activator.CreateInstance(serializableObjectType);

            foreach (PropertyInfo pi in serializableProperties)
            {
                if (pi.PropertyType == typeof(string))
                {
                    pi.SetValue(deserializedObject, br.ReadString(), null);
                }
                else if (pi.PropertyType == typeof(List<int>))
                {
                    pi.SetValue(deserializedObject, ReadIntegerList(br), null);
                }
            }
            return deserializedObject;
        }*/

        public object ReadObject(Stream stream)
        {
            if (stream == null)
                return null;

            BinaryReader br = new BinaryReader(stream);

            /*object deserializedObject = Activator.CreateInstance(serializableObjectType);

            foreach (PropertyInfo pi in serializableProperties)
            {
                if (pi.PropertyType == typeof(Comune[]))
                {
                    pi.SetValue(deserializedObject, ReadComuneArray(br), null);
                }
            }*/

            //return deserializedObject;

            return ReadComuneArray(br);
        }

        private Comune[] ReadComuneArray(BinaryReader br)
        {
            Comune[] comuni = new Comune[br.ReadInt32()];

            for (int i = 0; i < comuni.Length; i++)
                comuni[i] = ReadComune(br);

            return comuni;
        }

        private Comune ReadComune(BinaryReader br)
        {
            return new Comune(br.ReadString(), ReadCAPRecordsArray(br));
        }

        private CAPRecord[] ReadCAPRecordsArray(BinaryReader br)
        {
            CAPRecord[] capRecords = new CAPRecord[br.ReadInt32()];

            for (int i = 0; i < capRecords.Length; i++)
            {
                capRecords[i] = new CAPRecord(br.ReadString(), br.ReadString(), br.ReadString());
            }

            return capRecords;
        }

        private List<int> ReadIntegerList(BinaryReader br)
        {
            List<int> list = new List<int>();
            int count = br.ReadInt32();

            int index = count;
            while (index > 0)
            {
                list.Add(br.ReadInt32());
                index--;
            }
            return list;
        }

        #endregion Read

    }
}
