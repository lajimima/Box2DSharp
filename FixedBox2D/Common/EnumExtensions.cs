using FixedBox2D.Dynamics;
using FixedBox2D.Dynamics.Contacts;

namespace FixedBox2D.Common
{
    public static class EnumExtensions
    {
        public static bool IsSet(this BodyFlags self, BodyFlags flag) => (self & flag) == flag;

        public static bool IsSet(this DrawFlag self, DrawFlag flag) => (self & flag) == flag;

        public static bool IsSet(this Contact.ContactFlag self, Contact.ContactFlag flag) => (self & flag) == flag;
    }
}