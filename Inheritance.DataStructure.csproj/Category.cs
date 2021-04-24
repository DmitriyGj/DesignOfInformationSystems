using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.DataStructure
{
    public class Category : IComparable
    {
        private string productName;
        private MessageType messageType;
        private MessageTopic messageTopic;
        private static int prime = 16777619;
        private static int offsetBasis = unchecked((int)2166136261);
        private int hashCode = offsetBasis;
        public Category(string productName, MessageType messageType, MessageTopic messageTopic)
        {
            this.productName = productName ?? string.Empty;
            this.messageType = messageType;
            this.messageTopic = messageTopic;
            hashCode = GetHashCode();
        }

        public int CompareTo(object obj)
        {
            int result = -1;
            if (obj is Category)
            {
                var diffMessage = obj as Category;
                result = productName.CompareTo(diffMessage.productName);
                result = result == 0 ? messageType.CompareTo(diffMessage.messageType) : result;
                result = result == 0 ? messageTopic.CompareTo(diffMessage.messageTopic) : result;
            }
            return result;
        }

        public override int GetHashCode()
        {
            if (hashCode == offsetBasis)
            {
                unchecked
                {
                    hashCode = (hashCode ^ productName.GetHashCode()) * prime;
                    hashCode = (hashCode ^ messageType.GetHashCode()) * prime;
                    hashCode = (hashCode ^ messageTopic.GetHashCode()) * prime;
                }
            }
            return hashCode;
        }

        public override string ToString() => $"{productName}.{messageType}.{messageTopic}";
        public override bool Equals(object obj) =>obj is null?false: ReferenceEquals(obj,this) || obj.GetHashCode() == hashCode;
        public static bool operator <(Category cat1, Category cat2) => cat1.CompareTo(cat2) < 0 ;
        public static bool operator >(Category cat1, Category cat2) => cat1.CompareTo(cat2) > 0 ;
        public static bool operator >=(Category cat1, Category cat2) => cat1.CompareTo(cat2) >= 0;
        public static bool operator <=(Category cat1, Category cat2) => cat1.CompareTo(cat2) <= 0 ;
    }
}
