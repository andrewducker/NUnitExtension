using System.Collections.Generic;
using NUnit.Framework;
using System;

namespace NUnitExtension
{
    [TestFixture]
    public class ExampleTests
    {
        [Test]
        public void TestEquality()
        {
            var x = 5;
            x += 3;
            x.MustBeEqualTo(8);

            //Does not compile, as the types do not match.
            //x.MustBeEqualTo("Bob"); 
        }

        [Test]
        public void TestPropertyComparison()
        {
            var testStruct = new TestStruct
                                 {
                                     ANumber = 3,
                                     TestStrings = new List<string> {"John", "Paul", "George"},
                                     ADateTime = DateTime.Now
                                 };

            testStruct.ANumber = 5;
            testStruct.TestStrings.Add("Ringo");

            var expectedStruct = new TestStruct
                                     {
                                         ANumber = 5,
                                         TestStrings = new List<string> {"John", "Paul", "George", "Ringo"}
                                     };

            testStruct.MustHaveSamePropertiesAs(expectedStruct);
            //Test fails with:
            //Expected string length 77 but was 90. Strings differ at index 69.
            //Expected: "...John, Paul, George, Ringo ], ADateTime: <null> }"
            //But was:  "...John, Paul, George, Ringo ], ADateTime: 03/02/2013 18:09:45 }"

        }


        [Test]
        public void TestContainment()
        {
            var stringList = new List<string> {"John", "Paul", "George", "Ringo"};
            stringList.MustContain("John","Paul","George","Ringo","Stuart");
            // Test fails with:
            //"Item not found: Stuart.  Items found were: John, Paul, George, Ringo"
        }


    }
}
