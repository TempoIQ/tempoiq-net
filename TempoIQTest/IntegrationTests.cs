using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TempoIQ;
using TempoIQ.Json;
using TempoIQ.Models;
using TempoIQ.Querying;
using NodaTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TempoIQTest
{
    [TestClass]
    class ClientIT
    {
        public static Client Client { get; set; }
        public static Client InvalidClient { get; set; }
        public static DateTimeZone UTC = DateTimeZone.Utc;
        public static ZonedDateTime start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 1));
        public static ZonedDateTime stop = UTC.AtStrictly(new LocalDateTime(2019, 1, 1, 0, 0, 1));
        public static Interval interval = new Interval(start.ToInstant(), stop.ToInstant());
        public static int SLEEP = 5000;
        public Credentials LoadedCredentials { get; private set; }

        [TestInitialize]
        public void InitCredentials()
        {
            XDocument credentialsSettings;
            try
            {
                credentialsSettings = XDocument.Load("credentials.config");
            }
            catch (FileNotFoundException e)
            {
                string message = "Missing credentials file for integration test.\n" +
                  "Please supply a file 'integration-credentials.configuration' with the following format:\n" +
                  " <credentials>" +
                  "  <add key=\"key\"value=\"YOUR KEY HERE\"" +
                  "  <add key=\"secret\" value=\"YOUR SECRET HERE\"" +
                  "  <add key=\"database_id\" value=\"YOUR DATABASE ID HERE\"" +
                  "  <add key=\"hostname\" value=\"YOUR HOSTNAME HERE\"" +
                  "  <add key=\"port\" value=\"YOUR PORT HERE\"" +
                  "  <add key=\"scheme\" value=\"YOUR SCHEME HERE\"" +
                  " </credentials>";
                throw new ApplicationException(message);
            }
            var dict = (Dictionary<string, string>)credentialsSettings.Root.Elements()
                .Select((kv =>
                    new KeyValuePair<string, string>(
                        kv.Attribute("key").Value,
                        kv.Attribute("value").Value)));
            string key = dict["key"];
            string secret = dict["secret"];
            string dbId = dict["database_id"];
            string hostName = dict["hostname"];
            string scheme = dict["scheme"];
            InvalidClient = new Client(new Credentials("key", "secret"), hostName, scheme);
            Client = new Client(new Credentials(key, secret), hostName, scheme);
        }


        [TestCleanup]
        static public void Cleanup()
        {
            Result<DeleteSummary> result = Client.DeleteAllDevices();
            Assert.AreEqual(State.Success, result.MultiStatus);
        }

        static public Device createDevice()
        {
            List<Sensor> sensors = new List<Sensor>();
            sensors.Add(new Sensor("sensor1"));
            sensors.Add(new Sensor("sensor2"));
            Device device = new Device("device1", "name", new Dictionary<String, String>(), sensors);
            Result<Device> result = Client.CreateDevice(device);
            return result.Value;
        }

        [TestMethod]
        public void testInvalidCredentials()
        {
            Device device = new Device();
            Result<Device> result = InvalidClient.CreateDevice(device);
            Result<Device> expected = new Result<Device>(null, 403, "Forbidden");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void testCreateDevices()
        {
            Device device = new Device("create-device", "name", new Dictionary<String, String>(), new List<Sensor>());

            Result<Device> result = Client.CreateDevice(device);
            Result<Device> expected = new Result<Device>(device, 200, "OK");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void testWriteDataPointBySensor()
        {
            Device device = createDevice();
            var points = new Dictionary<string, double>();
            points.Add("sensor1", 1.23);
            points.Add("sensor2", 1.67);
            var mp = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0)), points);
            var result = Client.WriteDataPoints(device, mp);
            Assert.Equals(State.Success, result.State);
        }

        [TestMethod]
        public void testReadDataPoints()
        {
            Device device = createDevice();
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2012, 1, 2, 0, 0, 0, 0));

            var points = new Dictionary<String, double>();
            points.Add("sensor1", 1.23);
            points.Add("sensor2", 1.677);
            MultiDataPoint mp = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 1, 0, 0, 0)), points);
            Result<Unit> result = Client.WriteDataPoints(device, mp);
            Assert.AreEqual(State.Success, result.State);

            var selection = new Selection().AddSelector(Selectors.Type.Devices, Selectors.Key(device.Key));
            var cursor = Client.Read(selection, start, stop).Value;

            Assert.IsTrue(cursor.Any());

            foreach (var row in cursor)
            {
                Assert.Equals(1.23, row.getValue(device.Key, "sensor1"));
                Assert.Equals(1.677, row.getValue(device.Key, "sensor2"));
            }
        }

        [TestMethod]
        public void testReadWithPipeline()
        {
            var device = createDevice();
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2012, 1, 2, 0, 0, 0, 0));

            var points = new Dictionary<string, double>();
            points.Add("sensor1", 4.0);
            points.Add("sensor2", 2.0);
            var mp = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 1, 0, 0, 0)), points);
            var mp2 = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 2, 0, 0, 0)), points);

            List<MultiDataPoint> allPoints = new List<MultiDataPoint>();
            allPoints.Add(mp);
            allPoints.Add(mp2);

            Result<Unit> result = Client.WriteDataPoints(device, allPoints);
            Assert.AreEqual(State.Success, result.State);

            Selection sel = new Selection().
              AddSelector(Selectors.Type.Devices, Selectors.Key(device.Key));

            Pipeline pipeline = new Pipeline()
              .Rollup(Period.FromDays(1), Fold.Sum, start)
              .Aggregate(Fold.Mean);
            var cursor = Client.Read(sel, pipeline, start, stop).Value;
            Assert.IsTrue(cursor.Any());
            foreach (var row in cursor)
            {
                Assert.Equals(6.0, row.getValue(device.Key, "mean"));
            }
        }

    }
}