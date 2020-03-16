using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDEManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RDEManagerTests
{
    [TestClass()]
    public class RecordErrorFinderTests
    {

        [TestMethod()]
        public void getCoordErrorsInvalidUnit()
        {

            DataRow row = getTestCoordsRow();

            row["llunit"] = "D";
            row["llres"] = "250m";
            row["lat"] = "22.24531";
            row["long"] = "35.24234";
            row["ew"] = "E";
            row["ns"] = "S";

            string errors = RecordErrorFinder.getCoordErrors(row);

            Assert.AreEqual(errors, "llunit not valid");
        }

        [TestMethod()]
        public void coordsMatchQDSTest()
        {
            string filePath = "C:\\Users\\engelbrechti\\source\\repos\\RDEManager\\RDEManagerTests\\CoordinatesCountriesQDSs.csv";

            List<CoordinatesCountryQDS> values = File.ReadAllLines(filePath)
                                           .Skip(1)
                                           .Select(line => new CoordinatesCountryQDS(line))
                                           .ToList();

            DataRow row = getTestCoordsRow();

            foreach (CoordinatesCountryQDS test in values)
            {
                row["lat"] = test.lat;
                row["long"] = test.lng;
                row["ns"] = "S";
                row["ew"] = "E";
                row["llunit"] = "DD";
                row["llres"] = "250m";
                row["qds"] = test.qds;
                row["country"] = test.country;

                bool coordsMatch = RecordErrorFinder.coordsMatchQDS(row);

                if (!coordsMatch)
                {
                    //stop here
                    Assert.Fail();
                }

            }

        }

        [TestMethod()]
        public void coordsAreValidTest()
        {
            DataRow row = getTestCoordsRow();

            //case where it should work
            row["lat"] = "35.2145";
            row["long"] = "25.24563";
            row["llunit"] = "DD";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            bool works = RecordErrorFinder.coordsAreValid(row);
            if(!works)
            {
                Assert.Fail();
            }

        }

        [TestMethod()]
        public void coordsAreValidOneEmptyTest()
        {
            DataRow row = getTestCoordsRow();

            //case where one is empty
            row["lat"] = "";
            row["long"] = "25.24563";
            row["llunit"] = "DD";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            bool failsForBlank = !RecordErrorFinder.coordsAreValid(row);
            if (!failsForBlank)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void coordsAreValidInvalidCoordStringTest()
        {
            DataRow row = getTestCoordsRow();

            //case where one is not a numeric string
            row["lat"] = "21.l1245";
            row["long"] = "25.24563";
            row["llunit"] = "DD";
            row["ew"] = "E";
            row["ns"] = "S";

            bool failsForNonNumeric = !RecordErrorFinder.coordsAreValid(row);
            if (!failsForNonNumeric)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void coordsAreValidZeroTest()
        {
            DataRow row = getTestCoordsRow();

            //case where one is zero
            row["lat"] = "21.21245";
            row["long"] = "0.0000";
            row["llunit"] = "DD";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            bool failsForZero = !RecordErrorFinder.coordsAreValid(row);
            if (!failsForZero)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void coordsAreValidLatOutOfRange()
        {
            DataRow row = getTestCoordsRow();

            row["lat"] = "91.11245";
            row["long"] = "32.5479";
            row["llunit"] = "DD";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            bool latOutOfRange = !RecordErrorFinder.coordsAreValid(row);
            if (!latOutOfRange)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void coordsAreValidLatMinOutOfRange()
        {
            DataRow row = getTestCoordsRow();

            row["lat"] = "21.71245";
            row["long"] = "32.5479";
            row["llunit"] = "DM";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            bool latMinOutOfRange = !RecordErrorFinder.coordsAreValid(row);
            if (!latMinOutOfRange)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void coordsAreValidLatSecOutOfRange()
        {
            DataRow row = getTestCoordsRow();

            row["lat"] = "21.21745";
            row["long"] = "32.5479";
            row["llunit"] = "DMS";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            bool latSecOutOfRange = !RecordErrorFinder.coordsAreValid(row);
            if (!latSecOutOfRange)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void coordsAreValidInValidUnit()
        {
            DataRow row = getTestCoordsRow();

            row["lat"] = "21.21245";
            row["long"] = "32.5479";
            row["llunit"] = "a unit!!";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            bool unitValid = RecordErrorFinder.coordsAreValid(row);
            if (unitValid)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void coordsAreValidInvalidNS()
        {
            DataRow row = getTestCoordsRow();

            row["lat"] = "21.21245";
            row["long"] = "32.5479";
            row["llunit"] = "DM";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "34";

            bool nsValid = RecordErrorFinder.coordsAreValid(row);
            if (nsValid)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void getDecimalCoordsDDTest()
        {

            //we assume our coordinates are already validated using coordsAreValid()

            DataRow row = getTestCoordsRow();

            row["lat"] = "24.24578";
            row["long"] = "32.25479";
            row["llunit"] = "DD";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            string coords = RecordErrorFinder.getDecimalCoords(row);

            Assert.AreEqual(coords, "-24.24578, 32.25479");

        }

        [TestMethod()]
        public void getDecimalCoordsDMTest()
        {

            //we assume our coordinates are already validated using coordsAreValid()

            DataRow row = getTestCoordsRow();

            row["lat"] = "24.24578";
            row["long"] = "32.25479";
            row["llunit"] = "DM";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";


            string calcCoords = RecordErrorFinder.getDecimalCoords(row);

            string correct = "-24.40963, 32.42465";

            Assert.AreEqual(calcCoords, correct);

        }

        [TestMethod()]
        public void getDecimalCoordsDMSTest()
        {
            //we assume our coordinates are already validated using coordsAreValid()

            DataRow row = getTestCoordsRow();

            row["lat"] = "24.24578";
            row["long"] = "32.25479";
            row["llunit"] = "DMS";
            row["llres"] = "250m";
            row["ew"] = "E";
            row["ns"] = "S";

            string calcCoords = RecordErrorFinder.getDecimalCoords(row);

            string correct = "-24.41606, 32.42997";

            Assert.AreEqual(calcCoords, correct);

        }

        [TestMethod()]
        public void getAgentNamesNotInListTest()
        {
            //mock the table
            DataTable masterAgents = new DataTable();

            masterAgents.Columns.Add("surname", typeof(string));
            masterAgents.Columns.Add("initials", typeof(string));

            DataRow row1 = masterAgents.NewRow();
            row1["surname"] = "Smith";
            row1["initials"] = "JLB";
            masterAgents.Rows.Add(row1);

            DataRow row2 = masterAgents.NewRow();
            row2["surname"] = "Doe";
            row2["initials"] = "J";
            masterAgents.Rows.Add(row2);

            DataRow row3 = masterAgents.NewRow();
            row3["surname"] = "Bob";
            row3["initials"] = "TMG";
            masterAgents.Rows.Add(row3);


            //case, one is not in
            string testAgents = "Alan, K.J.; Bob, T.M.G.";
            string notIn = RecordErrorFinder.getAgentNamesNotInList(testAgents, masterAgents, new List<string>());
            Assert.AreEqual(notIn, "Alan, K.J.");


            //case all are in
            testAgents = "Doe, J.; Bob, T.M.G.";
            notIn = RecordErrorFinder.getAgentNamesNotInList(testAgents, masterAgents, new List<string>());
            Assert.AreEqual(notIn, "");


            //case all not in
            testAgents = "Baggins, F.; Foster, G.";
            notIn = RecordErrorFinder.getAgentNamesNotInList(testAgents, masterAgents, new List<string>());
            Assert.AreEqual(notIn, Regex.Replace(testAgents, @"\s+", " ")); //we need to regex to remove accidental spaces in testAgents

        }

        [TestMethod()]
        public void isQDSValidTest()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("qds", typeof(string));
            DataRow row = dt.NewRow();

            row["qds"] = "3423AD"; //valid QDS
            if (!RecordErrorFinder.isQDSValid(row))
            {
                Assert.Fail();
            }

            row["qds"] = "3424BV"; //invalid letters
            if (RecordErrorFinder.isQDSValid(row))
            {
                Assert.Fail();
            }

            row["qds"] = "8424BC"; //lat out of bounds
            if (RecordErrorFinder.isQDSValid(row))
            {
                Assert.Fail();
            }

            row["qds"] = "3404BC"; //lng out of bounds
            if (RecordErrorFinder.isQDSValid(row))
            {
                Assert.Fail();
            }

        }

        [TestMethod()]
        public void DateIsValidTest()
        {


            if (!RecordErrorFinder.dateIsValid("0", "0", "0"))
            {
                Assert.Fail("Failed for all zeros");
            }

            if (!RecordErrorFinder.dateIsValid("1923", "0", "0"))
            {
                Assert.Fail("Failed valid year, no month, no day");
            }

            if (RecordErrorFinder.dateIsValid("1823", "0", "0"))
            {
                Assert.Fail("Failed for invalid year");
            }

            if (RecordErrorFinder.dateIsValid("0", "3", "0"))
            {
                Assert.Fail("Failed for no year, valid month, no day");
            }

            if (RecordErrorFinder.dateIsValid("1923", "24", "0"))
            {
                Assert.Fail("Failed for valid year, invalid month");
            }

            if (RecordErrorFinder.dateIsValid("1923", "0", "22"))
            {
                Assert.Fail("Failed for valid year, no month, valid day");
            }

            if (RecordErrorFinder.dateIsValid("1923", "12", "42"))
            {
                Assert.Fail("Failed for valid year, valid month, invalid day");
            }

            if (!RecordErrorFinder.dateIsValid("1923", "12", "22"))
            {
                Assert.Fail("Failed for valid date");
            }


        }

        [TestMethod()]
        public void detDateAfterCollDateTest()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("detdd", typeof(string));
            dt.Columns.Add("detmm", typeof(string));
            dt.Columns.Add("detyy", typeof(string));
            dt.Columns.Add("colldd", typeof(string));
            dt.Columns.Add("collmm", typeof(string));
            dt.Columns.Add("collyy", typeof(string));
            DataRow row = dt.NewRow();

            row["collyy"] = "asdf";
            try
            {
                bool result = RecordErrorFinder.detDateAfterCollDate(row);
                //if the above works then this failed
                Assert.Fail();
            }
            catch
            {
               //we should have an exception here
            }

            row["collyy"] = "1995";
            row["detyy"] = "asdf";
            try
            {
                bool result = RecordErrorFinder.detDateAfterCollDate(row);
                //if the above works then this failed
                Assert.Fail();
            }
            catch
            {
                //we should have an exception here
            }

            //det year before coll year
            row["detyy"] = "1994";
            if (RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

            //coll month, no det month
            row["detyy"] = "1995";
            row["collmm"] = "6";
            if (!RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

            //det month, no coll month
            row["collmm"] = "";
            row["detmm"] = "6";
            if (!RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

            //det month before coll month
            row["collmm"] = "6";
            row["detmm"] = "5";
            if (RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

            //det month same as coll month
            row["detmm"] = "6";
            if (!RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

            //det day, no coll day
            row["detdd"] = "12";
            if (!RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

            //coll day, no det day
            row["detdd"] = "";
            row["colldd"] = "12";
            if (!RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

            //det day before coll day
            row["detdd"] = "5";
            if (RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

            row["detdd"] = "0";
            row["detmm"] = "0";
            row["detyy"] = "0";
            row["colldd"] = "12";
            row["collmm"] = "4";
            row["collyy"] = "1973";

            if (!RecordErrorFinder.detDateAfterCollDate(row))
            {
                Assert.Fail();
            }

        }

        //HELPERS
        private DataRow getTestCoordsRow()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("unit", typeof(string));
            dt.Columns.Add("lat", typeof(string));
            dt.Columns.Add("ns", typeof(string));
            dt.Columns.Add("long", typeof(string));
            dt.Columns.Add("ew", typeof(string));
            dt.Columns.Add("llunit", typeof(string));
            dt.Columns.Add("llres", typeof(string));
            dt.Columns.Add("qds", typeof(string));
            dt.Columns.Add("country", typeof(string));
            DataRow row = dt.NewRow();

            return row;
        }

    }
}