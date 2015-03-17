using System;
using System.Collections.Generic;
using TempoIQ;
using NodaTime;
using TempoIQ.Models;
using TempoIQ.Queries;
using TempoIQ.Results;
using NUnit.Framework;

namespace TempoIQTests
{
    [TestFixture]
    public class Snippets
    {
        public static string KEY = "YOUR KEY";
        public static string SECRET = "YOUR SECRET";
        public static string HOST = "YOUR HOST";

        public static Client Client = new Client(new Credentials(KEY, SECRET), HOST);
        public static Interval Interval = new Interval(Start, End);

        [Test]
        public void TestCreateDevice()
        {
            // snippet-begin create-device
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;

            // create device attributes
            var attributes = new Dictionary<string, string> { { "model", "v1"} };

            // create sensors
            var sensors = new List<Sensor> { new Sensor("temperature"), new Sensor("humidity") };

            // create device with key "thermostat.0" with attributes and sensors
            var device = new Device("thermostat.0", "", attributes, sensors);

            // store in TempoIQ
            var result = Client.CreateDevice(device);

            // Check that the request was successful
            if(result.Value != State.Success) {
                Console.WriteLine(String.Format("Error creating device! {0}", result.Message));
            }

            // snippet-end
            var expected = new Result<Device>(device, 200, "OK");
            Assert.AreEqual(expected.Value, result.Value);
        }

        [Test]
        public void TestReadRawDataPoints() {
            // snippet-begin read-data-one-device
            // using System;
            // using TempoIQ;
            // using NodaTime;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;

            // Set up the time range to read [2015-01-01, 2015-01-02)
            var start = new ZonedDateTime(Instant.FromDateTimeUtc(new DateTime(2015, 1, 1, 0, 0, 0, 0)), Utc);
            var end = new ZonedDateTime(Instant.FromDateTimeUtc(new DateTime(2015, 1, 2, 0, 0, 0, 0)), Utc);

            var device = new Device("thermostat.0");

            var selection = new Selection(
                                Select.Type.Devices,
                                Select.Key(device.Key));
            var cursor = Client.Read(selection, start, end);
            foreach (var row in cursor)
                Console.WriteLine(String.Format("timestamp {0}, temperature: {1}, humidity: {2}",
                            row.Timestamp,
                            row.Get("thermostat.0", "temperature"),
                            row.Get("thermostat.0", "humidity")));
            // snippet-end
        }

        [Test]
        public void TestGetDevice()
        {
            // snippet-begin get-device
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;

            // get the device with key         "thermostat.1"
            var result = Client.GetDevice("thermostat.1");

            // Check that the request was successful
            if (result.State != State.Success) {
                Console.WriteLine(String.Format("Error getting device! {0}", result.Message));
            }
            // snippet-end
            Assert.AreEqual(State.Success, result.State);
        }
        
        [Test]
        public void TestGetDevices()
        {
            // snippet-begin get-device
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;
            var selection = new Selection(
                Select.Type.Devices, 
                Select.Or(
                    Select.Attributes("region", "south"),
                    Select.Attributes("region", "east")));
            var cursor = Client.ListDevices(selection);
            foreach (var device in cursor)
            {
                Console.WriteLine(String.Format("device: {0}", device.Key));
                foreach (var sensor in device.Sensors) {
                    Console.WriteLine(String.Format("\tsensor: {0}", sensor.Key));
                }
            }
            // snippet-end
        }

        [Test]
        public void TestUpdateDevice()
        {
            // snippet-begin update-device
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;
            var deviceResult = Client.GetDevice("thermostat.4");

            // Check that the request was successful
            if (deviceResult.State != State.Success)
                Console.WriteLine(String.Format("Error getting device! {0}", deviceResult.Message));

            // mutate the device
            var device = deviceResult.Value;
            device.Attributes.Add("customer", "internal-test");
            device.Attributes.Add("region", "east");

            // update in TempoIQ
            deviceResult = Client.UpdateDevice(device);

            if (deviceResult.State != State.Success)
                Console.WriteLine(String.Format("Error updating device! {0}", deviceResult.Message));
            // snippet-end
            Assert.AreEqual(State.Success, deviceResult.State);
        }

        [Test]
        public void TestDeleteDevices()
        {
            
            var create = new Device("thermostat.5");
            var createResult = Client.CreateDevice(create);

            // snippet-begin delete-devices
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;
            var selection = new Selection(Select.Type.Devices, Select.Key("thermostat.5"));
            var result = Client.DeleteDevices(selection);
            if (result.State == State.Success)
                Console.WriteLine(String.Format("Deleted {0} devices.", result.Value.Deleted));
            else
                Console.WriteLine(String.Format("Error deleting devices! {0}", result.Message));
            // snippet-end
            Assert.AreEqual(State.Success, result.State);
        }

        [Test]
        public void TestSinglePoint()
        {
            // snippet-begin single-point
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;
            // using NodaTime;
            var timezone = DateTimeZone.Utc;

            // select the sensor "temperature" in the device "thermostat.1"
            var selection = new Selection()
                .Add(Select.Type.Devices, Select.Key("thermosatat.1"))
                .Add(Select.Type.Sensors, Select.Key("temperature"));

            // get the first point before 2015-01-10T00:00:00.000Z
            var time = timezone.AtStrictly(new LocalDateTime(2012, 1, 1, 1, 0, 0, 0));
            var single = new SingleValueAction(DirectionFunction.Before, time);
            var cursor = Client.Single(selection, single);
            foreach (var row in cursor)
                Console.WriteLine(String.Format("ts: {0} value: {1}",
                    row.Timestamp,
                    row.Get("thermostat.1", "temperature"))
                );
            // snippet-end
        }

        [Test]
        public void TestDeleteDataPoints()
        {

            // snippet-begin delete-data
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;
            // using NodaTime;
            var timezone = DateTimeZone.Utc;
            var start = timezone.AtStrictly(new LocalDateTime(2015, 1, 1, 0, 0, 0, 0));
            var end = timezone.AtStrictly(new LocalDateTime(2015, 1, 1, 1, 0, 0, 0));
            // delete datapoints for sensor "humidity" in device "thermostat.1"
            // in the range [2015-01-01T00:00:00Z, 2015-01-01T01:00:00Z)
            var device = new Device("thermostat.1");
            var sensor = new Sensor("humidity");
            var result = Client.DeleteDataPoints(device, sensor, start, end);

            if (result.State == State.Success)
                Console.WriteLine("Deleted {0} datapoints", result.Value.Deleted);
            else
                Console.WriteLine("Error deleting datapoints! {0}", result.Message);
            // snippet-end
            Assert.AreEqual(State.Success, result.State);
        }

        [Test]
        public void TestPipeline() 
        {
            // snippet-begin pipeline
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;
            // using NodaTime;
            var timezone = DateTimeZone.Utc;

            // Set up the time range to read [2015-01-01, 2015-01-02)
            var start = timezone.AtStrictly(new LocalDateTime(2015, 1, 1, 0, 0, 0, 0));
            var end = timezone.AtStrictly(new LocalDateTime(2015, 1, 2, 0, 0, 0, 0));

            // select device with key "thermostat.0"
            var selection = new Selection(Select.Type.Devices, Select.Key("thermostat.0"));

            // create a pipeline that calculates the hourly max
            var pipeline = new Pipeline().Rollup(Period.FromHours(1), Fold.Max, start);

            var cursor = Client.Read(selection, pipeline, start, end);

            foreach (var row in cursor)
            {
                var ts = row.Timestamp;
                var temp = row.Get("thermostat.0", "temperature");
                var hum = row.Get("thermostat.0", "humidity");
                Console.WriteLine(String.Format("timestamp {0}, temperature: {1}, humidity: {2}", ts, temp, hum));
            }
            // snippet-end
        }
    
        [Test]
        public void TestSearch()
        {
            // snippet-begin search
            // using System;
            // using TempoIQ;
            // using TempoIQ.Models;
            // using TempoIQ.Queries;
            // using TempoIQ.Results;
            // using NodaTime;
        
            var selection = new Selection(new Dictionary<Select.Type, Selector>() 
                    {
                        { Select.Type.Devices, Select.Attributes("building", "headquarters") },
                        { Select.Type.Sensors, Select.Key("temperature") }
                    });
            // snippet-end
        }
 }
    }
}
