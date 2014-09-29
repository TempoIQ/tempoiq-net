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
using TempoIQ.Queries;
using TempoIQ.Results;
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
            string key = "YOUR KEY";
            string secret = "YOUR SECRET";
            string domain = "YOUR BACKENT DOMAIN";
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

        static public List<Device> MakeDevices(int n)
        {
            var lst = new List<Device>();
            for(int i=0; i<n; i++)
                lst.Add(PostNewDevice());
            return lst;
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
        public void TestDeleteDevices()
        {
            MakeDevices(10);
            var result = Client.DeleteAllDevices();
            var devices = Client.ListDevics(new Selection(Select.Type.Devices, Select.All()));
            Assert.IsFalse(devices.Value.Any());
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
            MakeDevices(10);
            var selection = new Selection().Add(Select.Type.Devices, Select.All());
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
        public void TestWriteDataPointsWithWriteRequest()
        {
            var devices = MakeDevices(10);
            var selection = new Selection().Add(Select.Type.Devices, Select.All());
            var points = new WriteRequest();
            var lst = new List<DataPoint>();
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 19.667));
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 11019.647));
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 5.090913));
            foreach(var device in devices)
                foreach(var sensor in device.Sensors)
                    points.Add(device, sensor, lst);
            var result = Client.WriteDataPoints(points);
            Assert.IsTrue(result.State.Equals(State.Success));
        }

        [TestMethod]
        public void TestReadDataPoints()
        {
            //Make some devices
            var devices = MakeDevices(10);

            //Write some data
            var points = new WriteRequest();
            var lst = new List<DataPoint>();
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 19.667));
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 11019.647));
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 5.090913));
            foreach(var device in devices)
                foreach(var sensor in device.Sensors)
                    points.Add(device, sensor, lst);
            var written = Client.WriteDataPoints(points);
            
            //Read that data out
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2021, 1, 1, 0, 0, 0, 0));
            var selection = new Selection().Add(
                Select.Type.Devices,
                Select.Or(devices.Select(d => Select.Key(d.Key)).ToArray()));
            var result = Client.Read(selection, start, stop);
            var cursor = result.Value;

            Assert.AreEqual(State.Success, result.State);
            Assert.IsTrue(cursor.Any());
        }

        [TestMethod]
        public void TestReadWithPipeline()
        {
            //Make some devices
            var devices = MakeDevices(10);

            //Write some data
            var points = new WriteRequest();
            var lst = new List<DataPoint>();
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 19.667));
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 11019.647));
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 5.090913));
            foreach(var device in devices)
                foreach(var sensor in device.Sensors)
                    points.Add(device, sensor, lst);
            var written = Client.WriteDataPoints(points);
            
            //Read that data out with a pipeline
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2021, 1, 1, 0, 0, 0, 0));
            var selection = new Selection().Add(
                Select.Type.Devices,
                Select.Or(devices.Select(d => Select.Key(d.Key)).ToArray()));
            var function = new Rollup(Period.FromDays(4), Fold.Count, start);
            var pipeline = new Pipeline().AddFunction(function);
            var result = Client.Read(selection, start, stop);
            var cursor = result.Value;

            Assert.AreEqual(State.Success, result.State);
            Assert.IsTrue(cursor.Any());
        }
    }
}