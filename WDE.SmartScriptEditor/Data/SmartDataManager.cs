﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using WDE.Common.Database;
using WDE.Module.Attributes;

namespace WDE.SmartScriptEditor.Data
{
    public enum SmartType
    {
        SmartEvent = 0,
        SmartAction = 1,
        SmartTarget = 2,
        SmartCondition = 3,
        SmartConditionSource = 4,
        SmartSource = 5
    }

    public interface ISmartDataManager
    {
        bool Contains(SmartType type, int id);

        bool Contains(SmartType type, string id);

        SmartGenericJsonData GetRawData(SmartType type, int id);

        SmartGenericJsonData GetDataByName(SmartType type, string name);
    }

    [SingleInstance]
    [AutoRegister]
    public class SmartDataManager : ISmartDataManager
    {
        private readonly Dictionary<SmartType, Dictionary<int, SmartGenericJsonData>> smartIdData = new();
        private readonly Dictionary<SmartType, Dictionary<string, SmartGenericJsonData>> smartNameData = new();

        public SmartDataManager(SmartDataProvider provider)
        {
            Load(SmartType.SmartEvent, provider.GetEvents());
            Load(SmartType.SmartAction, provider.GetActions());
            Load(SmartType.SmartTarget, provider.GetTargets());
        }

        public bool Contains(SmartType type, int id)
        {
            if (!smartIdData.ContainsKey(type))
                return false;

            return smartIdData[type].ContainsKey(id);
        }

        public bool Contains(SmartType type, string id)
        {
            if (!smartNameData.ContainsKey(type))
                return false;

            return smartNameData[type].ContainsKey(id);
        }

        public SmartGenericJsonData GetRawData(SmartType type, int id)
        {
            if (!smartIdData[type].ContainsKey(id))
                throw new NullReferenceException();

            return smartIdData[type][id];
        }

        public SmartGenericJsonData GetDataByName(SmartType type, string name)
        {
            if (!smartNameData[type].ContainsKey(name))
                throw new NullReferenceException();

            return smartNameData[type][name];
        }

        private void Load(SmartType type, IEnumerable<SmartGenericJsonData> data)
        {
            foreach (var d in data)
                Add(type, d);
        }

        private void ActualAdd(SmartType type, SmartGenericJsonData data)
        {
            if (!smartIdData.ContainsKey(type))
            {
                smartIdData[type] = new Dictionary<int, SmartGenericJsonData>();
                smartNameData[type] = new Dictionary<string, SmartGenericJsonData>();
            }

            if (!smartIdData[type].ContainsKey(data.Id))
            {
                if (data.ValidTypes != null && data.ValidTypes.Contains(0))
                    data.ValidTypes.Add(SmartScriptType.Creature);
                smartIdData[type].Add(data.Id, data);
                smartNameData[type].Add(data.Name, data);
            }
            else
                throw new SmartDataWithSuchIdExists();
        }

        private void Add(SmartType type, SmartGenericJsonData data)
        {
            if (type == SmartType.SmartSource)
                ActualAdd(SmartType.SmartTarget, data);
            else if (type == SmartType.SmartTarget && !data.IsOnlyTarget)
                ActualAdd(SmartType.SmartSource, data);

            ActualAdd(type, data);
        }
    }

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class SmartDataWithSuchIdExists : Exception
    {
        public SmartDataWithSuchIdExists() { }

        public SmartDataWithSuchIdExists(string message) : base(message) { }

        public SmartDataWithSuchIdExists(string message, Exception innerException) : base(message, innerException) { }

        protected SmartDataWithSuchIdExists(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}