using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PuntoAfiliacionVentas.ComponenteSQL.Trydicionary
{
    public class Trictionary<Key, Value, Control> : SUTrictionaryBase<Key, Value, Control>
    {
        public Trictionary() => IsSorted(false);
      

        public Trictionary(Trictionary<Key, Value, Control> Trictionary)
        {
            IsSorted(false);
            foreach (KeyValueTriple<Key, Value, Control> U in (Trictionary.MemberwiseClone() as Trictionary<Key, Value, Control>)) Add(U);
        }
    }

    public class SortedTrictionary<Key, Value, Control> : SUTrictionaryBase<Key, Value, Control>
    {
        public SortedTrictionary()
        {
            IsSorted(true);
        }

        public SortedTrictionary(SortedTrictionary<Key, Value, Control> Trictionary)
        {
            IsSorted(true);
            foreach (KeyValueTriple<Key, Value, Control> U in (Trictionary.MemberwiseClone() as SortedTrictionary<Key, Value, Control>)) Add(U);
        }
    }

    public class SUTrictionaryBase<Key, Value, Control> : IEnumerable
    {
        public IDictionary<Key, KeyValueTriple<Key, Value, Control>> DataTrack = new Dictionary<Key, KeyValueTriple<Key, Value, Control>>();

        internal void IsSorted(bool IS)
        {
            if (IS) DataTrack = new SortedDictionary<Key, KeyValueTriple<Key, Value, Control>>();
            else DataTrack = new Dictionary<Key, KeyValueTriple<Key, Value, Control>>();
        }

        public int Count { get { return DataTrack.Count(); } }

        public List<Key> Keys { get { return DataTrack.Keys.ToList(); } }
        public List<Value> Values { get { return DataTrack.Values.Select(c => c.Value).ToList(); } }
        public List<Control> Controls { get { return DataTrack.Values.Select(c => c.Control).ToList(); } }

        private Dictionary<U, V> Zip<U, V>(List<U> ULst, List<V> VLst)
        {
            return ULst.Zip(VLst, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        }

        public Dictionary<Key, Value> ValueDictionary { get { return Zip(Keys, Values); } }

        public Dictionary<Key, Control> ControlDictionary { get { return Zip(Keys, Controls); } }

        #region Dictionary

        public void Add(Key key, Value value, Control control) => Add(new KeyValueTriple<Key, Value, Control>(key, value, control));

        public void Add(KeyValueTriple<Key, Value, Control> KVT) => DataTrack.Add(KVT.Key, KVT);

        public void Remove(Key key) => DataTrack.Remove(key);


        public KeyValueTriple<Key, Value, Control> this[Key key]
        {
            get { return DataTrack[key]; }
            set { DataTrack[key] = value; }
        }

        #endregion Dictionary

        #region GetEnumerator

        public IEnumerator<KeyValueTriple<Key, Value, Control>> GetEnumerator()
        {
            foreach (KeyValueTriple<Key, Value, Control> val in DataTrack.Values) yield return val;
        }

        #endregion GetEnumerator

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)DataTrack).GetEnumerator();
        }

        #endregion IEnumerable

        public KeyValueTriple<Key, Value, Control> GetAtIndex(int Index)
        {
            int pointer = 0;
            foreach (KeyValueTriple<Key, Value, Control> val in DataTrack.Values)
            {
                if (pointer == Index) return val;
                pointer++;
            }
            return null;
        }

        public int IndexOf(Key _Key)
        {
            return Keys.IndexOf(_Key);
        }
    }

    public partial class KeyValueTriple<key, value, control>
    {
        public KeyValueTriple(key _key, value _value, control _control)
        {
            Key = _key;
            Value = _value;
            Control = _control;
        }

        public key Key { get; set; } //  { get; }
        public value Value { get; set; }
        public control Control { get; set; }
        public object Tag { get; set; }
    }
}