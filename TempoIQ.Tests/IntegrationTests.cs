using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TempoIQ;
using TempoIQ.Models;
using TempoIQ.Queries;
using TempoIQ.Results;

using NodaTime;
using NUnit.Framework;

namespace TempoIQTests
{
    [TestFixture]
    public class ClientIT
    {
        public static Client Client { get; set; }

        public static Client InvalidClient { get; set; }

        public static DateTimeZone UTC = DateTimeZone.Utc;
        public static ZonedDateTime start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 1));
        public static ZonedDateTime stop = UTC.AtStrictly(new LocalDateTime(2019, 1, 1, 0, 0, 1));
        public static Interval interval = new Interval(start.ToInstant(), stop.ToInstant());
        public static int NextKeyNumber;

        public Credentials LoadedCredentials { get; private set; }

        [SetUp]
        public void InitCredentials()
        {
            var data = new Dictionary<string, string>();
            foreach (var row in File.ReadLines("../../user.config"))
                data.Add(row.Split('=')[0], row.Split('=')[1]);
            var key = data["key"];
            var secret = data["secret"];
            var host = data["host"];
            var creds = new Credentials(key, secret);
            InvalidClient = new Client(new Credentials("invalidKey", "invalidSecret"), host);
            Client = new Client(creds, host, "https", 443);
        }

        [TearDown]
        public void Cleanup()
        {
            var allSelection = new Selection().Add(
                Select.Type.Devices, 
                Select.AttributeKey("tempoiq-net-test-device"));
            Client.DeleteDevices(allSelection);
        }

        static public Device PostNewDevice()
        {
            List<Sensor> sensors = new List<Sensor>() { new Sensor("sensor1"), new Sensor("sensor2") };
            var device = RandomKeyDevice();
            device.Name = "name";
            device.Sensors = sensors;
            Result<Device> result = Client.CreateDevice(device);
            return result.Value;
        }

        static public List<Device> MakeDevices(int n)
        {
            var lst = new List<Device>();
            for (int i = 0; i < n; i++)
                lst.Add(PostNewDevice());
            return lst;
        }

        static public Device RandomKeyDevice()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var key = new string(
                          Enumerable.Repeat(chars, 64)
                          .Select(s => s [random.Next(s.Length)])
                          .ToArray());
            var attrs = new Dictionary<String, String>();
            attrs.Add("tempoiq-net-test-device", "tempoiq-net-test-device");
            return new Device(key, "device-name", attrs, new List<Sensor>());
        }

        [Test]
        public void TestDeleteDevices()
        {
            MakeDevices(10);
            var selection = new Selection(Select.Type.Devices, Select.AttributeKey("tempoiq-net-test-device"));
            var result = Client.DeleteDevices(selection);
            var devices = Client.ListDevices(selection);
            Assert.AreEqual(result.State, State.Success);
            Assert.IsFalse(devices.Any());
        }

        [Test]
        public void TestInvalidCredentials()
        {
            var device = RandomKeyDevice();
            Result<Device> result = InvalidClient.CreateDevice(device);
            Assert.AreEqual(403, result.Code);
        }

        [Test]
        public void TestCreateDevices()
        {
            var device = RandomKeyDevice();
            Result<Device> result = Client.CreateDevice(device);
            Result<Device> expected = new Result<Device>(device, 200, "OK");
            Assert.AreEqual(expected.Value, result.Value);
        }

        [Test]
        public void TestListDevices()
        {
            MakeDevices(10);
            var selection = new Selection(Select.Type.Devices, Select.AttributeKey("tempoiq-net-test-device"));
            var query = new FindQuery(new Search(Select.Type.Devices, selection), new Find(6));
            var cursor = Client.ListDevices(query) as Cursor<Device>;
            Assert.AreEqual(6, cursor.First.Data.Count);
            Assert.AreEqual(10, cursor.Count());
        }

        [Test]
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

        [Test]
        public void TestWriteDataPointsWithWriteRequest()
        {
            var devices = MakeDevices(10);
            var points = new WriteRequest();
            var lst = new List<DataPoint>();
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 19.667));
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 11019.647));
            lst.Add(new DataPoint(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 5.090913));
            foreach (var device in devices)
                foreach (var sensor in device.Sensors)
                    points.Add(device, sensor, lst);
            var result = Client.WriteDataPoints(points);
            Assert.IsTrue(result.State.Equals(State.Success));
        }

        [Test]
        public void TestReadDataPoints()
        {
            //Make some devices
            var devices = MakeDevices(10);

            //Write some data
            var points = new WriteRequest();
            var lst = (from i in Enumerable.Range(0, 10)
                        let time = ZonedDateTime.Add(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), 
                            Duration.FromMilliseconds(i))
                        select new DataPoint(time, i)).ToList();

            foreach (var device in devices)
                foreach (var sensor in device.Sensors)
                    points.Add(device, sensor, lst);
            Client.WriteDataPoints(points);

            //Read that data out
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2021, 1, 1, 0, 0, 0, 0));
            var selection = new Selection().Add(
                                Select.Type.Devices,
                                Select.Or(devices.Select(d => Select.Key(d.Key)).ToArray()));
            var cursor = Client.Read(selection, start, stop);
            Assert.IsTrue(cursor.Any());
        }

        [Test]
        public void TestDeviceAndSensorSelectionReadPoints()
        {
            //Make some devices
            var devices = MakeDevices(10);

            //Write some data
            var points = new WriteRequest();
            var lst = (from i in Enumerable.Range(0, 10)
                let time = ZonedDateTime.Add(ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow), Duration.FromMilliseconds(i))
                select new DataPoint(time, i)).ToList();

            foreach (var device in devices)
                foreach (var sensor in device.Sensors)
                    points.Add(device, sensor, lst);
            Client.WriteDataPoints(points);

            //Read that data out
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2021, 1, 1, 0, 0, 0, 0));
            var selection = new Selection().Add(
                Select.Type.Devices,
                Select.Or(devices.Select(d => Select.Key(d.Key)).ToArray()));
            selection.Add(
                Select.Type.Sensors, 
                Select.Or(Select.Key("sensor1")));
            var cursor = Client.Read(selection, start, stop);
            Assert.IsTrue(cursor.Any());
        }

        [Test]
        public void TestPagingReadDataPoints()
        {
            //Make some devices
            var device = PostNewDevice();

            //Write some data
            var points = new WriteRequest();
            var lst = (from i in Enumerable.Range(0, 10)
                    let time = ZonedDateTime.Add(
                        ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow),
                        Duration.FromMilliseconds(i))
                    select new DataPoint(time, i)).ToList();

            foreach (var sensor in device.Sensors)
                points.Add(device, sensor, lst);
            Client.WriteDataPoints(points);

            //Read that data out
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2021, 1, 1, 0, 0, 0, 0));
            var selection = new Selection().Add(
                                Select.Type.Devices,
                                Select.Or(Select.Key(device.Key)));
            var query = new ReadQuery(new Search(Select.Type.Sensors, selection), new Read(start, stop, 6));
            var cursor = Client.Read(query) as Cursor<Row>;
            Assert.AreEqual(6, cursor.First.Data.Count);
            Assert.AreEqual(10, cursor.Count());
            Assert.AreEqual(20, cursor.Flatten().Count());
            foreach(var sensorKey in device.Sensors.Select((s) => s.Key))
                Assert.AreEqual(10, cursor.StreamForDeviceAndSensor(device.Key, sensorKey).Count());

            var byStream = cursor.PointsByStream();
            Assert.AreEqual(2, byStream.Values.ElementAt(0).Values.Count);
        }

        [Test]
        public void TestReadWithPipeline()
        {
            //Make some devices
            var devices = MakeDevices(10);

            //Write some data
            var req = new WriteRequest();
            var pts = from i in Enumerable.Range(0, 10)
                let time = ZonedDateTime.Add(
                    ZonedDateTime.FromDateTimeOffset(DateTimeOffset.UtcNow),
                    Duration.FromSeconds(i))
                select new DataPoint(time, i);

            foreach (var device in devices)
                foreach (var sensor in device.Sensors)
                    req.Add(device, sensor, pts.ToList());
            var result = Client.WriteDataPoints(req);

            //Read that data out with a pipeline
            var start = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2021, 1, 1, 0, 0, 0, 0));
            var selection = new Selection().Add(
                                Select.Type.Devices,
                                Select.Or(devices.Select(d => Select.Key(d.Key)).ToArray()));
            var function = new Rollup(Period.FromMinutes(1), Fold.Count, start);
            var pipeline = new Pipeline().AddFunction(function);
            var cursor = Client.Read(selection, pipeline, start, stop);
            Assert.AreEqual(State.Success, result.State);
            Assert.IsTrue(cursor.Any());
        }

        [Test]
        public void TestLatest()
        {
            var device = PostNewDevice();

            var points = new Dictionary<string, double> {
                { "sensor1", 4.0 },
                { "sensor2", 2.0 }
            };
            var mp = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 1, 0, 0, 0)), points);
            var mp2 = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 2, 0, 0, 0)), points);

            var allPoints = new List<MultiDataPoint> { mp, mp2 };

            var result = Client.WriteDataPoints(device, allPoints);
            Assert.AreEqual(State.Success, result.State);

            var sel = new Selection().Add(Select.Type.Devices, Select.Key(device.Key));

            var cursor = Client.Latest(sel);
            Assert.AreEqual(4.0, cursor.First().Data[device.Key]["sensor1"]);
        }

        [Test]
        public void TestBefore()
        {
            var device = PostNewDevice();
            var points = new Dictionary<string, double> {
                { "sensor1", 4.0 },
                { "sensor2", 2.0 }
            };
            var mp = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 1, 0, 0, 0)), points);
            var mp2 = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 2, 0, 0, 0)), points);
            var allPoints = new List<MultiDataPoint>{ mp, mp2 };

            var result = Client.WriteDataPoints(device, allPoints);
            Assert.AreEqual(State.Success, result.State);

            var sel = new Selection().Add(Select.Type.Devices, Select.Key(device.Key));
            var single = new SingleValueAction(DirectionFunction.Before, UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 4, 0, 0, 0)));
            var cursor = Client.Single(sel, single);
            Assert.AreEqual(4.0, cursor.First().Data[device.Key]["sensor1"]);
        }

        [Ignore]
        public void TestEarliest()
        {
            var device = PostNewDevice();
            var points1 = new Dictionary<string, double> {
                { "sensor1", 1.0 },
                { "sensor2", 2.0 }
            };

            var points2 = new Dictionary<string, double> {
                { "sensor1", 3.0 },
                { "sensor2", 4.0 }
            };

            var mp = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 1, 0, 0, 0)), points1);
            var mp2 = new MultiDataPoint(UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 2, 0, 0, 0)), points2);
            var allPoints = new List<MultiDataPoint> { mp, mp2 };

            var result = Client.WriteDataPoints(device, allPoints);
            Assert.AreEqual(State.Success, result.State);

            var sel = new Selection().Add(Select.Type.Devices, Select.Key(device.Key));
            var single = new SingleValueAction(DirectionFunction.Earliest);
            var cursor = Client.Single(sel, single);

            Assert.AreEqual(1.0, cursor.First().Data[device.Key]["sensor1"]);
        }

        [Test]
        public void TestDeleteDataPoints()
        {
            var device = PostNewDevice();
            var sensor1 = new Sensor("sensor1");
            var deviceKey = device.Key;

            var sensorKey1 = device.Sensors[0].Key;
            var sensorKey2 = device.Sensors[1].Key;
            var ts = UTC.AtStrictly(new LocalDateTime(2012, 1, 1, 0, 0, 0, 0));

            var points1 = new Dictionary<String, double> { { "sensor1", 1.0 } };
            var points2 = new Dictionary<String, double> { { "sensor1", 2.0 } };

            var mp = new MultiDataPoint(ts, points1);
            var mp2 = new MultiDataPoint(ts, points2);
            var allPoints = new List<MultiDataPoint> { mp, mp2 };

            var result = Client.WriteDataPoints(device, allPoints);

            Assert.AreEqual(State.Success, result.State);

            var start = UTC.AtStrictly(new LocalDateTime(2011, 1, 1, 1, 1, 0, 0));
            var stop = UTC.AtStrictly(new LocalDateTime(2013, 1, 3, 0, 0, 0, 0));

            var deleteResult = Client.DeleteDataPoints(device.Key, sensor1.Key, start, stop);

            Assert.AreEqual(State.Success, deleteResult.State);
            Assert.AreEqual(1, deleteResult.Value.Deleted);

            var cursor1 = Client.Read(new Selection().Add(Select.Type.Devices, Select.Key("sensor1")),
                                      UTC.AtStrictly(new LocalDateTime(2011, 1, 1, 0, 0, 0, 0)),
                                      UTC.AtStrictly(new LocalDateTime(2013, 1, 1, 0, 0, 0, 0)));

            Assert.AreEqual(0, cursor1.Count());
        }
    }
}
