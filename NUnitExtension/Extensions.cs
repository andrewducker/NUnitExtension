using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace NUnitExtension
{
    public static class Extensions
    {
        /// <summary>
        /// Type-bound assertion for equality.
        /// </summary>
        /// <typeparam name="T">To ensure types match</typeparam>
        /// <param name="toCompare">The type to be compared</param>
        /// <param name="expected">The expected result to compare to</param>
        public static void MustBeEqualTo<T>(this T toCompare, T expected)
        {
            Assert.AreEqual(expected,toCompare);
        }

        /// <summary>
        /// Type-bound comparison of all properties for equality.
        /// </summary>
        /// <typeparam name="T">To ensure types match</typeparam>
        /// <param name="toCompare">The type to have its properties compared</param>
        /// <param name="expected">A model object to compare to</param>
        public static void MustHaveSamePropertiesAs<T>(this T toCompare, T expected)
        {
            var describedResultObject = Describe(toCompare);
            var describedModelObject = Describe(expected);
            Assert.AreEqual(describedModelObject,describedResultObject);
        }

        private static string Describe(object toDescribe)
        {
            if (toDescribe == null)
            {
                return "<null>";
            }
            var typeInfo = toDescribe.GetType();

            if (typeInfo.IsPrimitive || toDescribe is string || toDescribe is DateTime)
            {
                return toDescribe.ToString();
            }

            if (toDescribe is IEnumerable)
            {
                var enumerablesToDescribe = toDescribe as IEnumerable;
                var describedEnumerables = (from object enumerableToDescribe in enumerablesToDescribe select Describe(enumerableToDescribe)).ToList();
                return "[ " + string.Join(", ", describedEnumerables) + " ]";
            }

            var properties = GetFields(typeInfo);
            var describedProperties = properties.Select(propertyInfo => PrintableName(propertyInfo.Name) + ": " + Describe(propertyInfo.GetValue(toDescribe))).ToList();
            return "{ " + string.Join(", ", describedProperties) +" }";
        }

        private static string PrintableName(string name)
        {
            return name.Replace(">k__BackingField", "").Replace("<", "");
        }

        private static IEnumerable<FieldInfo> GetFields(Type typeInfo)
        {
            IEnumerable<FieldInfo> properties = typeInfo.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (typeInfo.BaseType != typeof(object))
            {
                properties = properties.Concat(GetFields(typeInfo.BaseType));
            }
            return properties;
        }

        public static void MustContain<T>(this IEnumerable<T> list, params T[] expectedItems)
        {
            var describedList = list.Select(itemToCheck => Describe(itemToCheck)).ToList();
            var describedExpectations = expectedItems.Select(expectedItem => Describe(expectedItem)).ToList();
            foreach (var describedExpectation in
                describedExpectations.Where(describedExpectation => !describedList.Contains(describedExpectation)))
            {
                Assert.Fail("Item not found: "+describedExpectation + ".  Items found were: " +string.Join(", ",describedList));
            }
        }
    }
}
