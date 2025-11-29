using System.ComponentModel.DataAnnotations.Schema;
using UEFASwissFormatSelector.Models;

namespace UEFASwissFormatSelector.Services
{
    /// <summary>
    /// This class was created as an alternative to Dictionary<Guid, Ienumerable<obj>> due to issues with ef core
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModifiedDictionary<T> where T : IEnumerable<object>
    {
        public Guid Id { get; set; }
        public Guid ScenarioInstanceId { get; set; }
        [NotMapped]
        private List<Guid> Guids { get; set; } = new List<Guid>();
        [NotMapped]
        private List<T?> TValues { get; set; } = new List<T?>();        
        public T? this[Guid guid]
        {
            get
            {
                int entityIndex = Guids.IndexOf(guid);
                if (entityIndex >= 0) return TValues[entityIndex];
                return default; 
            }
            set
            {
                int entityIndex = Guids.IndexOf(guid);
                if (entityIndex == -1) //Not already added
                { 
                    Guids.Add(guid);
                    TValues.Add(value);
                }
                else
                    TValues[entityIndex] = value;
            }
        }

        public void RePopulate(Dictionary<Guid, T> entries)
        {
            Guids = new List<Guid>();
            TValues = new List<T?>();
            foreach (var entry in entries)
            {
                Guids.Add((Guid)entry.Key);
                TValues.Add((T)entry.Value);
                var e = TValues.Where(x=> x.ToString != null);
            }
        }
        //return a dcitionary of key_value pair
        /// <summary>
        /// For getting the dictionary equivalent of this object
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, T> GetAsDictionary()
        {
            var dic = new Dictionary<Guid, T>();
            if (Guids != null)
                foreach (var guid in Guids)
                {
                    int index = Guids.IndexOf(guid);
                    dic[guid] = TValues[index];
                }
            return dic;
        }

        public IEnumerable<KeyValuePair<Guid, T>> Where(Func<KeyValuePair<Guid, T>, bool> predicate)
        {
            return GetAsDictionary().Where(predicate);
        }

        public List<DbModifiedDictionaryEntity<T>> GetEquivalentDbEntities<T>() where T : Identifiable
        {
            var list = new List<DbModifiedDictionaryEntity<T>>();
            foreach (var guid in Guids)
            {
                foreach (Identifiable entity in this[guid])
                {
                    list.Add( new DbModifiedDictionaryEntity<T>(ScenarioInstanceId)
                    {
                        DictionaryId = this.Id,
                        DictionaryKey = guid,
                        ObjectId = entity.Id
                    } );
                }
            }
            return list;
        }

        public List<R> GetAllValues<R>()
        {
            var list = new List<R>();
            foreach (Guid guid in Guids)
            {
                foreach (R item in this[guid])
                {
                    list.Add(item);
                }
            }
            return list;
        }
        
    }
}
