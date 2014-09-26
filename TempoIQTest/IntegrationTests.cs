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
    public class ClientIT
    {
        public static Client Client { get; set; }
        public static Client InvalidClient { get; set; }
        public static DateTimeZone UTC = DateTimeZone.Utc;
        public static ZonedDateTime start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 1));
        public static ZonedDateTime stop = UTC.AtStrictly(new LocalDateTime(2019, 1, 1, 0, 0, 1));
        public static Interval interval = new Interval(start.ToInstant(), stop.ToInstant());
        public static int SLEEP = 5000;
        public static int NextKeyNumber;
        public Credentials LoadedCredentials { get; private set; }

        [TestInitialize]
        public void InitCredentials()
        {
            string key = "2ab2bd61c9c7417c8161dbeb0d9a4f76";
            string secret = "97832ebfb1d749639f442826c6d6902b";
            string domain = "txedly-james.backend.tempoiq.com";
            InvalidClient = new Client(new Credentials("invalidKey", "invalidSecret"), domain);
            Client = new Client(new Credentials(key, secret), domain);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Client.DeleteAllDevices();
        }

        static public Device PostNewDevice()
        {
            List<Sensor> sensors = new List<Sensor>();
            sensors.Add(new Sensor("sensor1"));
            sensors.Add(new Sensor("sensor2"));
            var device = RandomKeyDevice();
            device.Name = "name";
            device.Sensors = sensors;
            Result<Device> result = Client.CreateDevice(device);
            return result.Value;
        }

        static public Device RandomKeyDevice()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var key = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return new Device(key, "device-name", new Dictionary<String, String>(), new List<Sensor>());
        }

        [TestMethod]
        public void TestInvalidCredentials()
        {
            var device = RandomKeyDevice();
            Result<Device> result = InvalidClient.CreateDevice(device);
            Assert.AreEqual(403, result.Code);
        }

        [TestMethod]
        public void TestCreateDevices()
        {
            var device = RandomKeyDevice();
            Result<Device> result = Client.CreateDevice(device);
            Result<Device> expected = new Result<Device>(device, 200, "OK");
            Assert.AreEqual(expected.Value, result.Value);
        }

        [TestMethod]
        public void TestListDevices()
        {
            var selection = new Selection().AddSelector(Selectors.Type.Devices, Selectors.All());
            var result = Client.ListDevics(selection);
            Assert.AreEqual(200, result.Code);
            Assert.IsTrue(result.Value.Any());
        }

        [TestMethod]
        public void TestWriteDataPointBySensor()
        {
            Device device = PostNewDevice();
            var points = new Dictionary<string, double>();
            points.Add("sensor1", 1.23);
            points.Add("sensor2", 1.67);
            var mp = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0)), points);
            var result = Client.WriteDataPoints(device, mp);
            Assert.AreEqual(State.Success, result.State);
        }

        [TestMethod]
        public void TestReadDataPoints()
        {
            Device device = PostNewDevice();
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2021, 1, 1, 0, 0, 0, 0));
            var result = Client.Read(new Selection().AddSelector(Selectors.Type.Devices, Selectors.All()), start, stop);
            Assert.AreEqual(State.Success, result.State);

            var selection = new Selection().AddSelector(Selectors.Type.Devices, Selectors.Key(device.Key));
            var cursor = Client.Read(selection, start, stop).Value;

            Assert.IsTrue(cursor.Any());

            foreach (var row in cursor)
            {
                Assert.AreEqual(1.23, row.Get(device.Key, "sensor1"));
                Assert.AreEqual(1.677, row.Get(device.Key, "sensor2"));
            }
        }

        [TestMethod]
        public void TestReadWithPipeline()
        {
            var device1 = PostNewDevice();
            var device2 = PostNewDevice();
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

            Result<Unit> result = Client.WriteDataPoints(device1, allPoints);
            Assert.AreEqual(State.Success, result.State);

            Selection sel = new Selection().
              AddSelector(Selectors.Type.Devices, Selectors.Key(device1.Key));

            Pipeline pipeline = new Pipeline()
              .Rollup(Period.FromDays(1), Fold.Sum, start)
              .Aggregate(Fold.Mean);
            var cursor = Client.Read(sel, pipeline, start, stop).Value;
            Assert.IsTrue(cursor.Any());
            foreach (var row in cursor)
            {
                Assert.AreEqual(6.0, row.Get(device1.Key, "mean"));
            }
        }
    }
}